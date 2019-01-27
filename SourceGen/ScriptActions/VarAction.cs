using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for defining a script variable.
  /// </summary>
  [ActionName("var")]
  public class VarAction : ScriptAction
  {
    /// <summary>
    /// Name of the variable.
    /// </summary>
    [ActionParameter("name", false)]
    public string Name { get; set; } = "";

    /// <summary>
    /// Value of the variable.
    /// Parameter is parsed.
    /// </summary>
    [ActionParameter("value", true)]
    public string Value { get; set; } = "";

    /// <inheritdoc />
    public VarAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (!Regex.IsMatch(Name, @"^\w+$"))
        throw new ScriptException("Invalid variable name provided. Name cannot be empty and must contain only alphanumeric characters and underscores.",
          ActionXmlElement, "name");

      DataContext.Variables.Add(Name, Value);
    }
  }
}
