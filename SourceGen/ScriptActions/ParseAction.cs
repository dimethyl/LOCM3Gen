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
    public string SourceDirectory { get; set; }

    /// <summary>
    /// File pattern for choosing the files to be parsed.
    /// </summary>
    [ActionParameter("file-pattern", false)]
    public string FilePattern { get; set; }

    /// <summary>
    /// If "true" search files in subdirectories, otherwise search only the top level directory.
    /// </summary>
    [ActionParameter("recursive", false)]
    public string Recursive { get; set; }

    /// <inheritdoc />
    public ParseAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      var sourceDirectory = Path.GetFullPath(SourceDirectory);
      if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
      {
        // TODO: Wrong parse source path processing.
        return;
      }

      foreach (var fileName in Directory.EnumerateFiles(sourceDirectory, FilePattern,
        Recursive.ToLower() == "true" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
      {
        ParseFile(fileName);
      }
    }
  }
}
