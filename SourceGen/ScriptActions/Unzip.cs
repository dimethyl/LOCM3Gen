using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  [ActionName("unzip")]
  public class Unzip : ScriptAction
  {
    [ActionParameter("archive", true)]
    public string ArchivePath { get; set; }

    [ActionParameter("entry", true)]
    public string EntryPath { get; set; }

    [ActionParameter("target-dir", true)]
    public string TargetDirectory { get; set; }

    [ActionParameter("keep-existing")]
    public string KeepExistingFiles { get; set; }

    public Unzip(XElement element, ScriptDataContext dataContext) : base(element, dataContext)
    {
    }

    public override void Invoke()
    {
      var archivePath = Path.GetFullPath(ArchivePath);
      if (string.IsNullOrWhiteSpace(archivePath) || !File.Exists(archivePath))
      {
        // TODO: Wrong zip archive path processing.
        return;
      }

      using (var archive = ZipFile.OpenRead(archivePath))
      {
        var entryStream = archive.GetEntry(EntryPath);
        if (entryStream == null)
        {
          // TODO: Wrong zip archive entry path.
          return;
        }

        if (!Directory.Exists(TargetDirectory))
          Directory.CreateDirectory(TargetDirectory);

        var targetFileName = Path.Combine(TargetDirectory, Path.GetFileName(EntryPath));
        if (KeepExistingFiles.ToLower() == "true" && File.Exists(targetFileName))
          return;

        using (var targetFile = new StreamWriter(targetFileName))
        {
          entryStream.Open().CopyTo(targetFile.BaseStream);
          targetFile.Flush();
        }
      }
    }
  }
}
