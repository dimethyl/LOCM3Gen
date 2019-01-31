using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Two script action variations for conditional executing of the nested actions.
  /// <i>if-eq</i> stands for "if arguments are equal" and <i>if-neq</i> stands for "if arguments are not equal".
  /// </summary>
  [ActionName("if-eq")]
  [ActionName("if-neq")]
  public class IfAction : ScriptAction
  {
    /// <summary>
    /// Argument A.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("a")]
    public string ArgumentA { get; set; } = "";

    /// <summary>
    /// Argument B.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("b")]
    public string ArgumentB { get; set; } = "";

    /// <inheritdoc />
    public IfAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      var negate = ActionName == "if-neq";
      if ((!negate && ArgumentA != ArgumentB) || (negate && ArgumentA == ArgumentB))
        return;

      foreach (var nestedXmlElement in NestedXmlElements)
        ScriptReader.ExecuteElement(nestedXmlElement, DataContext, ParentAction);
    }
  }
}
