using System.Collections.Generic;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  /// <summary>
  /// Script action for defining a list of values.
  /// Invokes nested actions to manipulate the values in the list.
  /// </summary>
  [ActionName("list")]
  public class ListAction : ScriptAction
  {
    /// <summary>
    /// List name.
    /// </summary>
    [ActionParameter("name", false)]
    public string Name { get; set; }

    /// <summary>
    /// List of values that can be accessible by the nested actions.
    /// </summary>
    public List<string> ListValues;

    /// <inheritdoc />
    public ListAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction)
      : base(actionXmlElement, dataContext, parentAction)
    {
    }

    /// <inheritdoc />
    public override void Invoke()
    {
      if (string.IsNullOrEmpty(Name))
      {
        // TODO: Wrong list name processing.
        return;
      }

      if (!DataContext.Lists.ContainsKey(Name))
        DataContext.Lists.Add(Name, new List<string>());

      ListValues = DataContext.Lists[Name];

      foreach (var nestedXmlElement in NestedXmlElements)
        ScriptReader.ExecuteElement(nestedXmlElement, DataContext, this);
    }
  }
}
