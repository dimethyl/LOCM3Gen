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
      else
      {
        // TODO: Wrong variable name.
      }
    }

    /// <summary>
    /// Action method for operating list.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("list")]
    public void ListAction(XElement actionElement)
    {
      var listName = actionElement.Attribute("name")?.Value?.Trim() ?? "";
      if (listName != "")
      {
        var list = new List<string>();
        foreach (var nestedElement in actionElement.Elements())
        {
          var childActionName = nestedElement.Name.ToString().Trim();
          if (childActionName == "add")
          {
            var value = ParseString(nestedElement.Attribute("value")?.Value?.Trim() ?? "");
            list.Add(value);
          }
          else if (childActionName == "remove")
          {
            var value = ParseString(nestedElement.Attribute("value")?.Value?.Trim() ?? "");
            list.Remove(value);
          }
          else if (childActionName == "add-files")
          {
            var sourcePattern = ParseString(nestedElement.Attribute("source")?.Value?.Trim() ?? "");
            if (sourcePattern != "")
            {
              var sourcePath = Path.GetFullPath(Path.GetDirectoryName(sourcePattern));
              if (Directory.Exists(sourcePath))
              {
                var fileNamePattern = Path.GetFileName(sourcePattern);
                var recursive = (nestedElement.Attribute("recursive")?.Value?.Trim()?.ToLower() ?? "") == "true";
                var relative = (nestedElement.Attribute("relative")?.Value?.Trim()?.ToLower() ?? "") == "true";
                foreach (var fileName in Directory.EnumerateFiles(sourcePath, fileNamePattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                  list.Add(relative ? fileName.Replace(sourcePath, ".") : fileName);
              }
              else
              {
                // TODO: Wrong source path.
              }
            }
            else
            {
              // TODO: Wrong source path.
            }
          }
          else
          {
            // TODO: Wrong child action processing.
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
      var sourcePattern = ParseString(actionElement.Attribute("source")?.Value?.Trim() ?? "");
      if (sourcePattern != "")
      {
        var sourcePath = Path.GetFullPath(Path.GetDirectoryName(sourcePattern));
        if (Directory.Exists(sourcePath))
        {
          var fileNamePattern = Path.GetFileName(sourcePattern);
          var destinationPath = Path.GetFullPath(ParseString(actionElement.Attribute("destination")?.Value?.Trim() ?? ""));
          var recursive = (actionElement.Attribute("recursive")?.Value?.Trim()?.ToLower() ?? "") == "true";
          var keepExistingFiles = (actionElement.Attribute("keep-existing")?.Value?.Trim()?.ToLower() ?? "") == "true";
          var parseFiles = (actionElement.Attribute("parse")?.Value?.Trim()?.ToLower() ?? "") == "true";
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
    /// Conditional action method that controls processing of nested actions.
    /// </summary>
    /// <param name="data">XML element containing action data.</param>
    [ActionNames("if-eq", "if-neq")]
    public void IfAction(XElement actionElement)
    {
      var a = ParseString(actionElement.Attribute("a")?.Value?.Trim() ?? "");
      var b = ParseString(actionElement.Attribute("b")?.Value?.Trim() ?? "");
      var negate = actionElement.Name.ToString().ToLower() == "if-neq";
      if ((a == b && !negate) || (a != b && negate))
      {
        foreach (var nestedElement in actionElement.Elements())
          ProcessAction(nestedElement);
      }
    }
  }
}
