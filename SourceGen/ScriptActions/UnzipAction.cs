using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for unzipping a file from the archive.
  /// </summary>
  [ActionName("unzip")]
  public class UnzipAction : ScriptAction
  {
    /// <summary>
    /// Path to the zip-archive file.
    /// </summary>
    [ActionParameter("archive")]
    public string ArchivePath { get; set; } = "";

    /// <summary>
    /// Internal path in the archive of the file to be extracted.
    /// </summary>
    [ActionParameter("entry")]
    public string EntryPath { get; set; } = "";

    /// <summary>
    /// Target directory where the file will be extracted.
    /// </summary>
    [ActionParameter("target-dir")]
    public string TargetDirectory { get; set; } = "";

    /// <summary>
    /// If "true" existing file will not be overwritten during extraction.
    /// </summary>
    [ActionParameter("keep-existing")]
    public bool KeepExistingFiles { get; set; }

    /// <inheritdoc />
    public UnzipAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (string.IsNullOrWhiteSpace(ArchivePath) || !File.Exists(ArchivePath))
        throw new ScriptException($"Invalid archive path: \"{ArchivePath}\".", ActionXmlElement, "archive");

      if (string.IsNullOrWhiteSpace(TargetDirectory))
        throw new ScriptException("Empty target directory provided.", ActionXmlElement, "target-dir");

      using (var archive = ZipFile.OpenRead(Path.GetFullPath(ArchivePath)))
      {
        var entryStream = archive.GetEntry(EntryPath);
        if (entryStream == null)
          throw new ScriptException($"Invalid entry path \"{EntryPath}\" provided for the archive \"{ArchivePath}\".", ActionXmlElement, "entry");

        if (!Directory.Exists(TargetDirectory))
          Directory.CreateDirectory(TargetDirectory);

        var targetFileName = Path.Combine(TargetDirectory, Path.GetFileName(EntryPath));
        if (KeepExistingFiles && File.Exists(targetFileName))
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
