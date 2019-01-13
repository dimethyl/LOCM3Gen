using System.IO;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  [ActionName("parse")]
  public class Parse : ScriptAction
  {
    [ActionParameter("source-dir", true)]
    public string SourceDirectory { get; set; }

    [ActionParameter("file-pattern")]
    public string FilePattern { get; set; }

    [ActionParameter("recursive")]
    public string Recursive { get; set; }

    public Parse(XElement element, ScriptDataContext dataContext) : base(element, dataContext)
    {
    }

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
