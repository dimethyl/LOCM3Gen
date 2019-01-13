using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  [ActionName("if-eq")]
  [ActionName("if-neq")]
  public class If : ScriptAction
  {
    [ActionParameter("a", true)]
    public string ArgumentA { get; set; }

    [ActionParameter("b", true)]
    public string ArgumentB { get; set; }

    public If(XElement element, ScriptDataContext dataContext) : base(element, dataContext)
    {
    }

    public override void Invoke()
    {
      var negate = ActionName == "if-neq";
      if ((!negate && ArgumentA != ArgumentB) || (negate && ArgumentA == ArgumentB))
        return;

      foreach (var nestedElement in NestedXmlElements)
        ScriptReader.ExecuteElement(nestedElement, DataContext);
    }
  }
}
