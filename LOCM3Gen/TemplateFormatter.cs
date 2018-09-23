using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace LOCM3Gen
{
  /// <summary>
  /// Template files formatter class.
  /// </summary>
  public class TemplateFormatter
  {
    /// <summary>
    /// List of the key-value pairs of a list name and a list of replacing strings to be formated and replaced in template files.
    /// </summary>
    public Dictionary<string, List<string>> lists;

    /// <summary>
    /// List of the key-value pairs of a parameter name and a replacing string to be replaced in template files.
    /// </summary>
    public Dictionary<string, string> parameters;

    /// <summary>
    /// Template instance constructor.
    /// </summary>
    public TemplateFormatter()
    {
      this.lists = new Dictionary<string, List<string>>();
      this.parameters = new Dictionary<string, string>();
    }

    /// <summary>
    /// Replace all <c>{$...$}</c> parameter tags with their values within the template file.
    /// </summary>
    /// <param name="templateFileName">Template file name to process.</param>
    public void ProcessFile(string templateFileName)
    {
      var listCallback = new MatchEvaluator(match => {
        if (this.lists.ContainsKey(match.Groups[2].Value))
          return match.Groups[1].Value + String.Join(match.Groups[3].Value + match.Groups[1].Value, this.lists[match.Groups[2].Value]) + match.Groups[3].Value;
        else
          return "";
      });

      var parameterCallback = new MatchEvaluator(match => {
        if (this.parameters.ContainsKey(match.Groups[1].Value))
          return this.parameters[match.Groups[1].Value];
        else
          return "";
      });

      if (File.Exists(templateFileName))
      {
        //Reading template contents
        var fileContents = File.ReadAllText(templateFileName);

        //Processing {#...{$...$}...#} of lists
        fileContents = Regex.Replace(fileContents, @"\{\#(.*?)\{\$(\w*)\$\}(.*?)\#\}", listCallback, RegexOptions.Compiled | RegexOptions.Singleline);

        //Processing {$...$} patterns of parameters
        fileContents = Regex.Replace(fileContents, @"\{\$(\w*)\$\}", parameterCallback, RegexOptions.Compiled);

        //Writing processed template contents
        File.WriteAllText(templateFileName, fileContents);
      }
    }
  }
}
