using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen
{
  /// <summary>
  /// Base class for all SourceGen script actions.
  /// Derived classes must be decorated with one or several <see cref="ActionNameAttribute" /> to assign the action names to this script action.
  /// Properties of the derived class can be decorated with single <see cref="ActionParameterAttribute" /> to assign the action parameter name
  /// to the property.
  /// </summary>
  public abstract class ScriptAction
  {
    /// <summary>
    /// Name of the script action.
    /// </summary>
    public readonly string ActionName;

    /// <summary>
    /// Script data context for the script action.
    /// </summary>
    protected readonly ScriptDataContext DataContext;

    /// <summary>
    /// Nested XML elements enumeration.
    /// </summary>
    protected readonly IEnumerable<XElement> NestedXmlElements;

    /// <summary>
    /// Script action constructor.
    /// </summary>
    /// <param name="element">XML element used to configure the script action.</param>
    /// <param name="dataContext">Script data context instance with variables and lists used on parsing.</param>
    public ScriptAction(XElement element, ScriptDataContext dataContext)
    {
      if (element == null)
        throw new ArgumentNullException(nameof(element));

      ActionName = element.Name.ToString();
      DataContext = dataContext ?? new ScriptDataContext();
      NestedXmlElements = element.Descendants();

      // Setting the values of member properties decorated with ActionParameterAttribute.
      foreach (var property in GetType().GetProperties())
      {
        var parameterAttribute = property.GetCustomAttribute(typeof(ActionParameterAttribute)) as ActionParameterAttribute;
        if (parameterAttribute == null)
          continue;

        var propertyValue = element.Attribute(parameterAttribute.ActionParameterName)?.Value ?? "";
        if (parameterAttribute.ParseParameterValue)
          propertyValue = ParseText(propertyValue);
        property.SetValue(this, propertyValue);
      }
    }

    /// <summary>
    /// Parses the patterns within the text.
    /// </summary>
    /// <param name="text">Text to parse.</param>
    /// <returns>Parsed text with all patterns being replaced.</returns>
    protected string ParseText(string text)
    {
      // Removing {%...%} patterns of comments.
      var result = Regex.Replace(text, @"\{\%(.*?)\%\}", "", RegexOptions.Compiled);

      // Parsing {#...{@...@}...#} patterns of lists.
      result = Regex.Replace(result, @"\{\#(.*?)\{\@(\w*)\@\}(.*?)\#\}",
        match => DataContext.Lists.ContainsKey(match.Groups[2].Value)
          ? match.Groups[1].Value + string.Join(match.Groups[3].Value + match.Groups[1].Value, DataContext.Lists[match.Groups[2].Value]) +
            match.Groups[3].Value
          : "", RegexOptions.Compiled | RegexOptions.Singleline);

      // Parsing {$...$} patterns of variables.
      result = Regex.Replace(result, @"\{\$(\w*)\$\}",
        match => DataContext.Variables.ContainsKey(match.Groups[1].Value) ? DataContext.Variables[match.Groups[1].Value] : "",
        RegexOptions.Compiled);

      return result;
    }

    /// <summary>
    /// Parses the patterns within the template file.
    /// </summary>
    /// <param name="fileName">Name of the file to parse.</param>
    protected void ParseFile(string fileName)
    {
      if (!File.Exists(fileName))
        return;

      var fileContents = File.ReadAllText(fileName);
      File.WriteAllText(fileName, ParseText(fileContents));
    }

    /// <summary>
    /// Defines all script action logic.
    /// </summary>
    public abstract void Invoke();
  }
}
