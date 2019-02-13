using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for processing of another script file.
  /// </summary>
  [ActionName("run")]
  public class RunAction : ScriptAction
  {
    /// <summary>
    /// Path to the script file to run.
    /// </summary>
    [ActionParameter("script-path")]
    public string ScriptPath { get; set; } = "";

    /// <inheritdoc />
    public RunAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      ScriptReader.RunScript(ScriptPath, DataContext);
    }
  }
}
