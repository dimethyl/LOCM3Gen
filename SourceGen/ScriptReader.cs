/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/ .

 * Copyright (C) 2018-2019 Maxim Yudin <stibiu@yandex.ru>.
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen
{
  /// <summary>
  /// Class of the SourceGen script reader.
  /// </summary>
  public class ScriptReader
  {
    /// <summary>
    /// Minimal supported SourceGen file version.
    /// </summary>
    protected static readonly Version MinVersion = new Version("1.0");

    /// <summary>
    /// Maximal supported SourceGen file version.
    /// </summary>
    protected static readonly Version MaxVersion = new Version("1.0");

    /// <summary>
    /// Dictionary of <see cref="ScriptAction" />-derived types with script action names as keys.
    /// </summary>
    public static readonly Dictionary<string, Type> ScriptActions = new Dictionary<string, Type>();

    /// <summary>
    /// Script data context of the script reader.
    /// Contains variables and lists for parsing.
    /// </summary>
    public readonly ScriptDataContext DataContext = new ScriptDataContext();

    /// <summary>
    /// Script reader static constructor.
    /// Builds the dictionary of available <see cref="ScriptAction" />-derived types with script action names as keys.
    /// </summary>
    static ScriptReader()
    {
      // ReSharper disable once PossibleNullReferenceException
      foreach (var type in typeof(ScriptReader).Assembly.GetTypes())
      {
        if (!type.IsSubclassOf(typeof(ScriptAction)))
          continue;

        var actionNameAttributes = type.GetCustomAttributes(typeof(ActionNameAttribute), false) as ActionNameAttribute[];
        if (actionNameAttributes == null || actionNameAttributes.Length == 0)
          continue;

        foreach (var actionNameAttribute in actionNameAttributes)
        {
          if (ScriptActions.ContainsKey(actionNameAttribute.ActionName))
            continue;

          ScriptActions.Add(actionNameAttribute.ActionName, type);
        }
      }
    }

    /// <summary>
    /// Creates an instance of the necessary <see cref="ScriptAction" />-derived class and executes its <see cref="ScriptAction.Invoke" /> method.
    /// </summary>
    /// <param name="actionXmlElement">Action's XML element containing script action data.</param>
    /// <param name="dataContext">Script data context instance.</param>
    /// <param name="parentAction">Parent script action this XML element is nested to. Can be null.</param>
    public static void ExecuteElement(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction = null)
    {
      var actionName = actionXmlElement.Name.ToString();
      if (!ScriptActions.ContainsKey(actionName))
        return;

      var scriptAction = (ScriptAction) Activator.CreateInstance(ScriptActions[actionName], actionXmlElement, dataContext, parentAction);
      scriptAction.Invoke();
    }

    /// <summary>
    /// Executes SourceGen XML-based script interpreting root-nested nodes into script actions.
    /// </summary>
    /// <param name="scriptFileName">SourceGen script file name to read.</param>
    /// <exception cref="FileNotFoundException">Script file was not found.</exception>
    /// <exception cref="FormatException">Invalid script file header.</exception>
    /// <exception cref="VersionNotFoundException">Unknown script file version.</exception>
    public void RunScript(string scriptFileName)
    {
      if (string.IsNullOrWhiteSpace(scriptFileName) || !File.Exists(scriptFileName))
        throw new FileNotFoundException($"Script \"{scriptFileName}\" was not found.", scriptFileName);

      var rootNode = XDocument.Load(scriptFileName).Root;
      if (rootNode == null || rootNode.Name != "sourcegen-script")
        throw new FormatException("Invalid script file header.");

      var scriptVersion = new Version(rootNode.Attribute("version")?.Value.Trim() ?? "0.0");
      if (scriptVersion < MinVersion || scriptVersion > MaxVersion)
        throw new VersionNotFoundException($"Unknown script file version: {scriptVersion}.");

      foreach (var scriptAction in rootNode.Elements())
        ExecuteElement(scriptAction, DataContext);
    }
  }
}
