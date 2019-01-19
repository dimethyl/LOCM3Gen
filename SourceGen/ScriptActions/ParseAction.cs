using System.IO;
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
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("source-dir", true)]
    public string SourceDirectory { get; set; } = "";

    /// <summary>
    /// File pattern for choosing the files to be parsed.
    /// </summary>
    [ActionParameter("file-pattern", false)]
    public string FilePattern { get; set; } = "";

    /// <summary>
    /// If "true" search files in subdirectories, otherwise search only the top level directory.
    /// </summary>
    [ActionParameter("recursive", false)]
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

      foreach (var fileName in Directory.EnumerateFiles(Path.GetFullPath(SourceDirectory), FilePattern,
        Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
      {
        ParseFile(fileName);
      }
    }
  }
}
