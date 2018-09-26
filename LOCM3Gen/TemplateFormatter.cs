/*
 * Copyright (C) 2018 Maxim Yudin <i@hal.su>. All rights reserved.
 * 
 * This file is a part of the closed source section of LOCM3Gen project.
 * You may NOT use, distribute, copy or modify this file without special author's permission.
 */

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
    /// List of the key-value pairs of a variable name and a replacing string to be replaced in template files.
    /// </summary>
    public Dictionary<string, string> variables;

    /// <summary>
    /// Template instance constructor.
    /// </summary>
    public TemplateFormatter()
    {
      this.lists = new Dictionary<string, List<string>>();
      this.variables = new Dictionary<string, string>();
    }

    /// <summary>
    /// Replace all <c>{$...$}</c> variable tags with their values within the template file.
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

      var variableCallback = new MatchEvaluator(match => {
        if (this.variables.ContainsKey(match.Groups[1].Value))
          return this.variables[match.Groups[1].Value];
        else
          return "";
      });

      if (File.Exists(templateFileName))
      {
        //Reading template contents.
        var fileContents = File.ReadAllText(templateFileName);

        //Removing {%...%} patterns of comments.
        fileContents = Regex.Replace(fileContents, @"\{\%(\w*)\%\}", "", RegexOptions.Compiled);

        //Processing {#...{$...$}...#} patterns of lists.
        fileContents = Regex.Replace(fileContents, @"\{\#(.*?)\{\$(\w*)\$\}(.*?)\#\}", listCallback, RegexOptions.Compiled | RegexOptions.Singleline);

        //Processing {$...$} patterns of variables.
        fileContents = Regex.Replace(fileContents, @"\{\$(\w*)\$\}", variableCallback, RegexOptions.Compiled);

        //Writing processed template contents.
        File.WriteAllText(templateFileName, fileContents);
      }
    }
  }
}
