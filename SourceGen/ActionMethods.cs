/*
 * Copyright (C) 2018 Maxim Yudin <i@hal.su>. All rights reserved.
 * 
 * This file is a part of the closed source section of LOCM3Gen project.
 * You may NOT use, distribute, copy or modify this file without special author's permission.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace SourceGen
{
  /// <summary>
  /// Extension for script reader class describing action methods.
  /// </summary>
  public partial class ScriptReader
  {
    /// <summary>
    /// Action method for operating variables.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("var")]
    public void VarAction(XElement actionElement)
    {
      var name = GetActionParameter(actionElement, "name");
      if (name != "")
        variables.Add(name, GetActionParameter(actionElement, "value", true));
      else
      {
        // TODO: Wrong variable name.
      }
    }

    /// <summary>
    /// Action method for maintaining lists.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("list")]
    public void ListAction(XElement actionElement)
    {
      var listName = GetActionParameter(actionElement, "name");
      if (listName != "")
      {
        var list = new List<string>();
        foreach (var commandElement in actionElement.Elements())
        {
          switch (GetActionName(commandElement))
          {
            case "add":
            {
              list.Add(GetActionParameter(commandElement, "value", true));
            }
            break;

            case "remove":
            {
              list.Remove(GetActionParameter(commandElement, "value", true));
            }
            break;

            case "add-paths":
            {
              var sourcePath = Path.GetFullPath(GetActionParameter(commandElement, "source-dir", true));
              if (sourcePath != "" && Directory.Exists(sourcePath))
              {
                var fileNamePattern = GetActionParameter(commandElement, "file-pattern");
                var recursive       = GetActionParameter(commandElement, "recursive").ToLower() == "true";
                var relative        = GetActionParameter(commandElement, "relative").ToLower() == "true";

                foreach (var fileName in Directory.EnumerateFiles(sourcePath, fileNamePattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                  list.Add(relative ? fileName.Replace(sourcePath, ".") : fileName);
              }
              else
              {
                // TODO: Wrong source path.
              }
            }
            break;

            default:
            {
              // TODO: Wrong child action processing.
            }
            break;
          }
        }

        if (lists.ContainsKey(listName))
          lists[listName].AddRange(list);
        else
          lists.Add(listName, list);
      }
    }

    /// <summary>
    /// Action method for files copying.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("copy")]
    public void CopyAction(XElement actionElement)
    {
      var sourcePath = Path.GetFullPath(GetActionParameter(actionElement, "source-dir", true));
      if (sourcePath != "" && Directory.Exists(sourcePath))
      {
        var fileNamePattern   = GetActionParameter(actionElement, "file-pattern");
        var destinationPath   = Path.GetFullPath(GetActionParameter(actionElement, "target-dir", true));
        var recursive         = GetActionParameter(actionElement, "recursive").ToLower() == "true";
        var keepExistingFiles = GetActionParameter(actionElement, "keep-existing").ToLower() == "true";
        var parseFiles        = GetActionParameter(actionElement, "parse").ToLower() == "true";

        foreach (var fileName in Directory.EnumerateFiles(sourcePath, fileNamePattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
        {
          var destinationFileName = fileName.Replace(sourcePath, destinationPath);
          var destinationDirectory = Path.GetDirectoryName(destinationFileName);

          if (!Directory.Exists(destinationDirectory))
            Directory.CreateDirectory(destinationDirectory);

          if (!File.Exists(destinationFileName) || !keepExistingFiles)
          {
            File.Copy(fileName, destinationFileName, true);

            if (parseFiles)
              ParseFile(destinationFileName);
          }
        }
      }
      else
      {
        // TODO: Wrong source path.
      }
    }

    /// <summary>
    /// Action method for extracting a file from a zip archive.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("unzip")]
    public void UnzipAction(XElement actionElement)
    {
      var archivePath = Path.GetFullPath(GetActionParameter(actionElement, "archive", true));
      if (File.Exists(archivePath))
      {
        var entryPath = GetActionParameter(actionElement, "entry", true);
        using (var svdArchive = ZipFile.OpenRead(archivePath))
        {
          var svdStream = svdArchive.GetEntry(entryPath);
          if (svdStream != null)
          {
            var destinationPath     = Path.GetFullPath(GetActionParameter(actionElement, "target-dir", true));
            var destinationFileName = Path.Combine(destinationPath, Path.GetFileName(entryPath));
            var keepExistingFiles   = GetActionParameter(actionElement, "keep-existing").ToLower() == "true";

            if (!Directory.Exists(destinationPath))
              Directory.CreateDirectory(destinationPath);

            if (!File.Exists(destinationFileName) || !keepExistingFiles)
            {
              using (var svdFile = new StreamWriter(destinationFileName))
              {
                svdStream.Open().CopyTo(svdFile.BaseStream);
                svdFile.Flush();
              }
            }
          }
          else
          {
            // TODO: Wrong entry path.
          }
        }
      }
      else
      {
        // TODO: Wrong archive path.
      }
    }

    /// <summary>
    /// Action method for extracting a file from a zip archive.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("parse")]
    public void ParseAction(XElement actionElement)
    {
      var sourcePath = Path.GetFullPath(GetActionParameter(actionElement, "source-dir", true));
      if (sourcePath != "" && Directory.Exists(sourcePath))
      {
        var fileNamePattern = GetActionParameter(actionElement, "file-pattern");
        var recursive       = GetActionParameter(actionElement, "recursive").ToLower() == "true";

        foreach (var fileName in Directory.EnumerateFiles(sourcePath, fileNamePattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
          ParseFile(fileName);
      }
      else
      {
        // TODO: Wrong source path.
      }
    }

    /// <summary>
    /// Conditional action method that controls processing of nested actions.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("if-eq", "if-neq")]
    public void IfAction(XElement actionElement)
    {
      var a       = GetActionParameter(actionElement, "a", true);
      var b       = GetActionParameter(actionElement, "b", true);
      var negate  = GetActionName(actionElement) == "if-neq";

      if ((a == b && !negate) || (a != b && negate))
      {
        foreach (var nestedElement in actionElement.Elements())
          ProcessAction(nestedElement);
      }
    }
  }
}
