using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for parsing the files matching the file pattern.
  /// </summary>
  [ActionName("parse")]
  public class ParseAction : ScriptAction
  {
    /// <summary>
    /// Source directory where the files will be searched.
    /// </summary>
    [ActionParameter("source-dir")]
    public string SourceDirectory { get; set; } = "";

    /// <summary>
    /// File pattern for choosing the files to be parsed.
    /// </summary>
    [ActionParameter("file-pattern")]
    public string FilePattern { get; set; } = "";

    /// <summary>
    /// If "true" search files in subdirectories, otherwise search only the top level directory.
    /// </summary>
    [ActionParameter("recursive")]
    public bool Recursive { get; set; }

    /// <inheritdoc />
    public ParseAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (string.IsNullOrWhiteSpace(SourceDirectory) || !Directory.Exists(SourceDirectory))
        throw new ScriptException($"Invalid source directory: \"{SourceDirectory}\".", ActionXmlElement, "source-dir");

      // Checking for "*" and "?" wildcards in directory names, invalid characters and parent directory ".." links.
      if (Regex.IsMatch(FilePattern, @"[*?].*[/\\]|[""|<>:]|(?<=[/\\]|^)\.\.(?=[/\\]|$)"))
        throw new ScriptException("Invalid file pattern provided.", ActionXmlElement, "file-pattern");

      var sourceDirectory = Path.GetFullPath(SourceDirectory);
      if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(sourceDirectory, FilePattern))))
        return;

      foreach (var fileName in Directory.EnumerateFiles(sourceDirectory, FilePattern,
        Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
      {
        ParseFile(fileName);
      }
    }
  }
}
