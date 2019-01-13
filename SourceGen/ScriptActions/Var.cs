using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  [ActionName("var")]
  public class Var : ScriptAction
  {
    [ActionParameter("name")]
    public string Name { get; set; }

    [ActionParameter("value", true)]
    public string Value { get; set; }

    public Var(XElement element, ScriptDataContext dataContext) : base(element, dataContext)
    {
    }

    public override void Invoke()
    {
      if (string.IsNullOrEmpty(Name))
      {
        // TODO: Wrong variable name processing.
        return;
      }

      DataContext.Variables.Add(Name, Value);
    }
  }
}
