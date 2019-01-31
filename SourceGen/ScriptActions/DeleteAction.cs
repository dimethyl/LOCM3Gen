using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for deleting files matching the pattern from the directory.
  /// </summary>
  [ActionName("delete")]
  public class DeleteAction : ScriptAction
  {
    /// <summary>
    /// Source directory where to delete entries from.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("source-dir")]
    public string SourceDirectory { get; set; } = "";

    /// <summary>
    /// Pattern for choosing the entries to be deleted.
    /// </summary>
    [ActionParameter("pattern")]
    public string EntryPattern { get; set; } = "";

    /// <summary>
    /// If "true" search for files for deleting.
    /// </summary>
    [ActionParameter("search-files")]
    public bool SearchFiles { get; set; }

    /// <summary>
    /// If "true" search for directories for deleting.
    /// </summary>
    [ActionParameter("search-dirs")]
    public bool SearchDirectories { get; set; }

    /// <summary>
    /// If "true" search entries in subdirectories, otherwise search only the top level directory.
    /// </summary>
    [ActionParameter("recursive")]
    public bool Recursive { get; set; }

    /// <summary>
    /// If "true" the directory will be deleted permanently, otherwise it will be moved to the recycle bin.
    /// </summary>
    [ActionParameter("permanently")]
    public bool Permanently { get; set; }

    /// <inheritdoc />
    public DeleteAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (string.IsNullOrWhiteSpace(SourceDirectory) || !Directory.Exists(SourceDirectory))
        return;

      // Checking for "*" and "?" wildcards in directory names, invalid characters and parent directory ".." links.
      if (Regex.IsMatch(EntryPattern, @"[*?].*[/\\]|[""|<>:]|(?<=[/\\]|^)\.\.(?=[/\\]|$)"))
        throw new ScriptException("Invalid entry pattern provided.", ActionXmlElement, "pattern");

      var sourceDirectory = Path.GetFullPath(SourceDirectory);
      if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(sourceDirectory, EntryPattern))))
        return;

      if (SearchFiles)
      {
        foreach (var fileName in Directory.EnumerateFiles(sourceDirectory, EntryPattern,
          Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
        {
          FileSystem.DeleteFile(fileName, UIOption.OnlyErrorDialogs,
            Permanently ? RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin);
        }
      }

      if (SearchDirectories)
      {
        foreach (var directoryName in Directory.EnumerateDirectories(sourceDirectory, EntryPattern,
          Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
        {
          FileSystem.DeleteDirectory(directoryName, UIOption.OnlyErrorDialogs,
            Permanently ? RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin);
        }
      }
    }
  }
}
