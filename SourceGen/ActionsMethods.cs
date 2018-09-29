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
      var name = actionElement.Attribute("name")?.Value?.Trim() ?? "";
      if (name != "")
      {
        var value = ParseString(actionElement.Attribute("value")?.Value?.Trim() ?? "");
        variables.Add(name, value);
      }
    }

    /// <summary>
    /// Action method for operating list.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("list")]
    public void ListAction(XElement actionElement)
    {
      var name = actionElement.Attribute("name")?.Value?.Trim() ?? "";
      if (name != "")
      {
        var list = new List<string>();
        foreach (var childElement in actionElement.Elements())
        {
          var childActionName = childElement.Name.ToString().Trim();
          var value = ParseString(childElement.Attribute("value")?.Value?.Trim() ?? "");
          if (childActionName == "add")
            list.Add(value);
        }
        lists.Add(name, list);
      }
    }

    /// <summary>
    /// Action method for files copying.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("copy")]
    public void CopyAction(XElement actionElement)
    {
      var sourcePattern = ParseString(actionElement.Attribute("source")?.Value?.Trim() ?? "");
      var sourcePath = Path.GetFullPath(Path.GetDirectoryName(sourcePattern));
      var fileNamePattern = Path.GetFileName(sourcePattern);
      if (Directory.Exists(sourcePath))
      {
        var destinationPath = Path.GetFullPath(ParseString(actionElement.Attribute("destination")?.Value?.Trim() ?? ""));                        
        var keepExistingFiles = (actionElement.Attribute("keep-existing")?.Value?.Trim()?.ToLower() ?? "") == "true";
        var parseFiles = (actionElement.Attribute("parse")?.Value?.Trim()?.ToLower() ?? "") == "true";
        var addToLists = actionElement.Attribute("add-to-lists")?.Value?.Trim() ?? "";
        
        // Copying and parsing files.
        var filePaths = new List<string>();
        foreach (var fileName in Directory.EnumerateFiles(sourcePath, fileNamePattern, SearchOption.TopDirectoryOnly))
        {
          var destinationFileName = Path.Combine(destinationPath, Path.GetFileName(fileName));
          if (!Directory.Exists(destinationPath))
            Directory.CreateDirectory(destinationPath);
          if (!File.Exists(destinationFileName) || !keepExistingFiles)
          {
            File.Copy(fileName, destinationFileName, true);
            if (parseFiles)
              ParseFile(destinationFileName);
          }
          filePaths.Add(destinationFileName);
        }

        // Filling lists.
        if (addToLists != "")
        {
          foreach (var listName in addToLists.Split(';'))
          {
            if (lists.ContainsKey(listName))
              lists[listName].AddRange(filePaths);
            else
              lists.Add(listName, filePaths);
          }
        }
      }
    }

    /// <summary>
    /// Action method for extracting a file from a zip archive.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("unzip")]
    public void UnzipAction(XElement actionElement)
    {
      var archivePath = Path.GetFullPath(ParseString(actionElement.Attribute("archive")?.Value?.Trim() ?? ""));
      if (File.Exists(archivePath))
      {
        var entryPath = ParseString(actionElement.Attribute("entry")?.Value?.Trim() ?? "");
        using (var svdArchive = ZipFile.OpenRead(archivePath))
        {
          var svdStream = svdArchive.GetEntry(entryPath);
          if (svdStream != null)
          {
            var destinationPath = Path.GetFullPath(ParseString(actionElement.Attribute("destination")?.Value?.Trim() ?? ""));
            var destinationFileName = Path.Combine(destinationPath, Path.GetFileName(entryPath));
            var keepExistingFiles = (actionElement.Attribute("keep-existing")?.Value?.Trim()?.ToLower() ?? "") == "true";
            if (!File.Exists(destinationFileName) || !keepExistingFiles)
            {
              using (var svdFile = new StreamWriter(destinationFileName))
              {
                svdStream.Open().CopyTo(svdFile.BaseStream);
                svdFile.Flush();
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Conditional action method that controls processing of nested actions.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("if-eq", "if-neq")]
    public void IfAction(XElement actionElement)
    {
      var variable = actionElement.Attribute("var")?.Value?.Trim() ?? "";
      if (variables.ContainsKey(variable))
      {
        var value = ParseString(actionElement.Attribute("value")?.Value?.Trim() ?? "");
        var negate = actionElement.Name.ToString().ToLower() == "if-neq";
        if ((variables[variable] == value && !negate) || (variables[variable] != value && negate))
        {
          foreach (var nestedElement in actionElement.Elements())
            ProcessAction(nestedElement);
        }
      }
    }
  }
}
