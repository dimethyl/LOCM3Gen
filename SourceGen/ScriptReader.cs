/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/ .

 * Copyright (C) 2018-2019 Maxim Yudin <stibiu@yandex.ru>.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen
{
  /// <summary>
  /// Class of the SourceGen script reader.
  /// </summary>
  public partial class ScriptReader
  {
    /// <summary>
    /// Action method delegate.
    /// </summary>
    /// <param name="actionElement">XML element containing action data.</param>
    protected delegate void ActionMethod(XElement actionElement);

    /// <summary>
    /// Attribute that binds the array of action names to the specified method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    protected class ActionNameAttribute : Attribute
    {
      /// <summary>
      /// The action name assigned to the decorated method.
      /// </summary>
      public readonly string ActionName;

      /// <summary>
      /// Assigns the provided action name to the decorated method.
      /// </summary>
      /// <param name="actionName">Action name.</param>
      public ActionNameAttribute(string actionName)
      {
        if (string.IsNullOrWhiteSpace(actionName) || !Regex.IsMatch(actionName, @"^[-\w]+$"))
          throw new ArgumentNullException(nameof(actionName), "Action name cannot be null and must contain only alphanumeric and hyphen characters.");

        ActionName = actionName;
      }
    }

    /// <summary>
    /// Minimal supported SourceGen file version.
    /// </summary>
    protected static readonly Version MinVersion = new Version("1.0");

    /// <summary>
    /// Maximal supported SourceGen file version.
    /// </summary>
    protected static readonly Version MaxVersion = new Version("1.0");

    /// <summary>
    /// List of actions' name-to-method bindings.
    /// </summary>
    protected Dictionary<string, ActionMethod> ActionMethods;

    /// <summary>
    /// List of variables and their values for template lists.
    /// Variables' values are inserted while reading script files and parsing templates.
    /// </summary>
    public readonly Dictionary<string, string> Variables = new Dictionary<string, string>();

    /// <summary>
    /// List of key-value pairs for template lists.
    /// Keys represent list names and values represent strings contained in the list.
    /// Lists are formatted and inserted while reading script files and parsing templates.
    /// </summary>
    public readonly Dictionary<string, List<string>> Lists = new Dictionary<string, List<string>>();

    /// <summary>
    /// Script reader class constructor.
    /// </summary>
    public ScriptReader()
    {
      BindActionMethods();
    }

    /// <summary>
    /// Binds action methods with their action names depending on their <see cref="ActionNameAttribute" /> values.
    /// </summary>
    private void BindActionMethods()
    {
      ActionMethods = new Dictionary<string, ActionMethod>();

      foreach (var method in GetType().GetMethods())
      {
        foreach (var attribute in method.GetCustomAttributes(false))
        {
          if (!(attribute is ActionNameAttribute))
            continue;

          var actionName = ((ActionNameAttribute) attribute).ActionName;
          {
            if (!ActionMethods.ContainsKey(actionName))
              ActionMethods.Add(actionName, (ActionMethod) method.CreateDelegate(typeof(ActionMethod), this));
            else
              throw new DuplicateNameException($"Cannot bind action name \"{actionName}\" to the method \"{method.Name}\". " +
                $"It is already bound to the method \"{ActionMethods[actionName].Method.Name}\".");
          }
        }
      }
    }

    /// <summary>
    /// Executes SourceGen XML-based script tag by tag.
    /// </summary>
    /// <param name="scriptFileName">SourceGen script file name to read.</param>
    public void RunScript(string scriptFileName)
    {
      var rootNode = XDocument.Load(scriptFileName).Root;

      if (rootNode == null || rootNode.Name != "sourcegen-script")
        throw new FormatException("Wrong script file format.");

      var scriptVersion = new Version(rootNode.Attribute("version")?.Value.Trim() ?? "0.0");
      if (scriptVersion < MinVersion || scriptVersion > MaxVersion)
        throw new FormatException("Unknown script file version.");

      foreach (var action in rootNode.Elements())
        ProcessAction(action);
    }

    /// <summary>
    /// Get action's name from the element.
    /// </summary>
    /// <param name="actionElement"><i>XElement</i> instance to get data from.</param>
    /// <returns>String with the name of the action.</returns>
    private string GetActionName(XElement actionElement)
    {
      return actionElement?.Name.ToString().Trim() ?? "";
    }

    /// <summary>
    /// Get action parameter's value from the element.
    /// </summary>
    /// <param name="actionElement"><i>XElement</i> instance to get data from.</param>
    /// <param name="parameterName">Name of the parameter to get.</param>
    /// <param name="parseValue">Parse the var patterns in the parameter value.</param>
    /// <returns>String with the value of the action parameter.</returns>
    private string GetActionParameter(XElement actionElement, string parameterName, bool parseValue = false)
    {
      var value = actionElement?.Attribute(parameterName)?.Value.Trim() ?? "";
      if (value != "" && parseValue)
        value = ParseString(value);
      return value;
    }

    /// <summary>
    /// The method launches the correct action method depending on the element name.
    /// </summary>
    /// <param name="actionElement">XML element containing action data.</param>
    private void ProcessAction(XElement actionElement)
    {
      var actionName = GetActionName(actionElement);
      if (ActionMethods.ContainsKey(actionName))
        ActionMethods[actionName](actionElement);
      else
      {
        // TODO: Wrong action processing.
      }
    }

    /// <summary>
    /// Parses the patterns within the template string.
    /// </summary>
    /// <param name="str">String to parse.</param>
    /// <returns>Parsed string with all patterns being replaced.</returns>
    private string ParseString(string str)
    {
      //Removing {%...%} patterns of comments.
      var result = Regex.Replace(str, @"\{\%(.*?)\%\}", "", RegexOptions.Compiled);

      //Processing {#...{@...@}...#} patterns of lists.
      result = Regex.Replace(result, @"\{\#(.*?)\{\@(\w*)\@\}(.*?)\#\}",
        match => Lists.ContainsKey(match.Groups[2].Value)
          ? match.Groups[1].Value + string.Join(match.Groups[3].Value + match.Groups[1].Value, Lists[match.Groups[2].Value]) + match.Groups[3].Value
          : "", RegexOptions.Compiled | RegexOptions.Singleline);

      //Processing {$...$} patterns of variables.
      result = Regex.Replace(result, @"\{\$(\w*)\$\}", match => Variables.ContainsKey(match.Groups[1].Value) ? Variables[match.Groups[1].Value] : "",
        RegexOptions.Compiled);

      return result;
    }

    /// <summary>
    /// Parses the patterns within the template file.
    /// </summary>
    /// <param name="fileName">Name of the file to parse.</param>
    private void ParseFile(string fileName)
    {
      if (File.Exists(fileName))
      {
        var fileContents = File.ReadAllText(fileName);
        File.WriteAllText(fileName, ParseString(fileContents));
      }
    }
  }
}
