/*
 * Copyright (C) 2018 Maxim Yudin <i@hal.su>. All rights reserved.
 * 
 * This file is a part of the closed source section of LOCM3Gen project.
 * You may NOT use, distribute, copy or modify this file without special author's permission.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SourceGen
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
    public delegate void ActionMethod(XElement actionElement);

    /// <summary>
    /// Attribute that binds the array of action names to the specified method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionNamesAttribute : Attribute
    {
      /// <summary>
      /// Array of action names.
      /// </summary>
      public readonly string[] actionNames;

      /// <summary>
      /// Bind action names to the method.
      /// </summary>
      /// <param name="actionNames">Array or sequence of action names.</param>
      public ActionNamesAttribute(params string[] actionNames)
      {
        this.actionNames = actionNames;
      }
    }
    
    /// <summary>
    /// Minimal supported SourceGen file version.
    /// </summary>
    private static readonly Version minVersion = new Version("1.0");

    /// <summary>
    /// Maximal supported SourceGen file version.
    /// </summary>
    private static readonly Version maxVersion = new Version("1.0");

    /// <summary>
    /// List of actions' name-to-method bindings.
    /// </summary>
    public Dictionary<string, ActionMethod> actionMethods;

    /// <summary>
    /// List of variables and their values for template lists.
    /// Variables' values are inserted while reading script files and parsing templates.
    /// </summary>
    public Dictionary<string, string> variables = new Dictionary<string, string>();

    /// <summary>
    /// List of key-value pairs for template lists.
    /// Keys represent list names and values represent strings contained in the list.
    /// Lists are formated and inserted while reading script files and parsing templates.
    /// </summary>
    public Dictionary<string, List<string>> lists = new Dictionary<string, List<string>>();

    /// <summary>
    /// Script reader class constructor.
    /// </summary>
    public ScriptReader()
    {
      BindActionMethods();
    }

    /// <summary>
    /// Bind action methods with their action names depending on their <i>ActionNames</i> attribute values.
    /// </summary>
    private void BindActionMethods()
    {
      actionMethods = new Dictionary<string, ActionMethod>();
      foreach (var method in this.GetType().GetMethods())
      {
        foreach (var attribute in method.GetCustomAttributes(false))
        {
          if (attribute is ActionNamesAttribute)
          {
            foreach (var actionName in (attribute as ActionNamesAttribute).actionNames)
            {
              if (!actionMethods.ContainsKey(actionName))
                actionMethods.Add(actionName, method.CreateDelegate(typeof(ActionMethod), this) as ActionMethod);
              else
                throw new ArgumentException("Can not add action name to the \"" + method.Name + "\" method. " +
                  "Action name \"" + actionName + "\" is already bound to the \"" + actionMethods[actionName].Method.Name + "\" method.");
            }
          }
        }
      }
    }

    /// <summary>
    /// Sequencially run SourceGen XML-based script.
    /// </summary>
    /// <param name="scriptFileName">SourceGen script file name to read.</param>
    public void RunScript(string scriptFileName)
    {
      var rootNode = XDocument.Load(scriptFileName).Root;

      if (rootNode == null || rootNode.Name != "sourcegen-script")
        throw new FormatException("Wrong script file format.");

      var scriptVersion = new Version(rootNode.Attribute("version")?.Value?.Trim() ?? "0.0");
      if (scriptVersion < minVersion || scriptVersion > maxVersion)
        throw new FormatException("Unknown script file version.");

      foreach (var action in rootNode.Elements())
        ProcessAction(action);
    }

    /// <summary>
    /// The method launches the correct action method depending on the element name.
    /// </summary>
    /// <param name="actionElement">XML element containing action data.</param>
    private void ProcessAction(XElement actionElement)
    {
      var actionName = actionElement.Name.ToString().Trim();
      if (actionMethods.ContainsKey(actionName))
        actionMethods[actionName](actionElement);
      else
      {
        // TODO: Wrong action processing.
      }
    }

    /// <summary>
    /// Callback for processing list regex pattern.
    /// </summary>
    /// <param name="match">Regex match information.</param>
    /// <returns></returns>
    private string ListCallback(Match match)
    {
      if (this.lists.ContainsKey(match.Groups[2].Value))
        return match.Groups[1].Value + String.Join(match.Groups[3].Value + match.Groups[1].Value, this.lists[match.Groups[2].Value]) + match.Groups[3].Value;
      else
        return "";
    }

    /// <summary>
    /// Callback for processing variable regex pattern.
    /// </summary>
    /// <param name="match">Regex match information.</param>
    /// <returns></returns>
    private string VariableCallback(Match match)
    {
      if (this.variables.ContainsKey(match.Groups[1].Value))
        return this.variables[match.Groups[1].Value];
      else
        return "";
    }

    /// <summary>
    /// Parse the patterns within the template string.
    /// </summary>
    /// <param name="str">String to parse.</param>
    /// <returns></returns>
    public string ParseString(string str)
    {
      //Removing {%...%} patterns of comments.
      var result = Regex.Replace(str, @"\{\%(.*?)\%\}", "", RegexOptions.Compiled);

      //Processing {#...{$...$}...#} patterns of lists.
      result = Regex.Replace(result, @"\{\#(.*?)\{\$(\w*)\$\}(.*?)\#\}", ListCallback, RegexOptions.Compiled | RegexOptions.Singleline);

      //Processing {$...$} patterns of variables.
      result = Regex.Replace(result, @"\{\$(\w*)\$\}", VariableCallback, RegexOptions.Compiled);

      return result;
    }

    /// <summary>
    /// Parse the patterns within the template file.
    /// </summary>
    /// <param name="fileName">Name of the file to parse.</param>
    public void ParseFile(string fileName)
    {
      if (File.Exists(fileName))
      {
        var fileContents = File.ReadAllText(fileName);
        File.WriteAllText(fileName, ParseString(fileContents));
      }
    }
  }
}
