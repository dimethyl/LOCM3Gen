using System.IO;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  [ActionName("copy")]
  public class Copy : ScriptAction
  {
    [ActionParameter("source-dir", true)]
    public string SourceDirectory { get; set; }

    [ActionParameter("target-dir", true)]
    public string TargetDirectory { get; set; }

    [ActionParameter("file-pattern")]
    public string FilePattern { get; set; }

    [ActionParameter("recursive")]
    public string Recursive { get; set; }

    [ActionParameter("parse")]
    public string Parse { get; set; }

    [ActionParameter("keep-existing")]
    public string KeepExistingFiles { get; set; }

    public Copy(XElement element, ScriptDataContext dataContext) : base(element, dataContext)
    {
    }

    public override void Invoke()
    {
      var sourceDirectory = Path.GetFullPath(SourceDirectory);
      if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
      {
        // TODO: Wrong copy source path processing.
        return;
      }

      foreach (var fileName in Directory.EnumerateFiles(sourceDirectory, FilePattern,
        Recursive.ToLower() == "true" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
      {
        var targetFileName = fileName.Replace(sourceDirectory, TargetDirectory);
        var targetFileDirectory = Path.GetDirectoryName(targetFileName);

        if (!Directory.Exists(targetFileDirectory))
        {
          // ReSharper disable once AssignNullToNotNullAttribute
          Directory.CreateDirectory(targetFileDirectory);
        }

        if (KeepExistingFiles.ToLower() == "true" && File.Exists(targetFileName))
          continue;

        File.Copy(fileName, targetFileName, true);

        if (Parse.ToLower() == "true")
          ParseFile(targetFileName);
      }
    }

  }
}
