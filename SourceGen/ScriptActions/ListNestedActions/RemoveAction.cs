using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions.ListNestedActions
{
  /// <summary>
  /// Nested script action for the <see cref="ListAction" />.
  /// Removes the value from the list.
  /// </summary>
  [ActionName("remove")]
  public class RemoveAction : ScriptAction
  {
    /// <summary>
    /// The value to remove from the list.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("value")]
    public string Value { get; set; } = "";

    /// <inheritdoc />
    public RemoveAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (!(ParentAction is ListAction))
        throw new ScriptException("No parent \"list\" action provided.", ActionXmlElement);

      var list = ((ListAction) ParentAction).ListValues;
      list.Remove(Value);
    }
  }
}
