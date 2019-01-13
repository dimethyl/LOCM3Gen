using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen.ScriptActions
{
  [ActionName("list")]
  public class List : ScriptAction
  {
    [ActionParameter("name")]
    public string Name { get; set; }

    public List(XElement element, ScriptDataContext dataContext) : base(element, dataContext)
    {
    }

    private string GetNestedActionParameter(XElement nestedActionElement, string parameterName, bool parseValue = false)
    {
      var value = nestedActionElement?.Attribute(parameterName)?.Value.Trim() ?? "";
      if (value != "" && parseValue)
        value = ParseText(value);
      return value;
    }

    public override void Invoke()
    {
      if (string.IsNullOrEmpty(Name))
      {
        // TODO: Wrong list name processing.
        return;
      }

      var list = new List<string>();
      foreach (var nestedElement in NestedXmlElements)
      {
        switch (nestedElement?.Name.ToString().ToLower())
        {
          case "add":
            list.Add(GetNestedActionParameter(nestedElement, "value", true));
          break;

          case "remove":
            list.Remove(GetNestedActionParameter(nestedElement, "value", true));
          break;

          case "add-paths":
            var sourceDirectory = Path.GetFullPath(GetNestedActionParameter(nestedElement, "source-dir", true));
            if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
            {
              // TODO: Wrong list source path processing.
              return;
            }

            var filePattern = GetNestedActionParameter(nestedElement, "file-pattern");
            var recursive = GetNestedActionParameter(nestedElement, "recursive").ToLower() == "true";
            var relative = GetNestedActionParameter(nestedElement, "relative").ToLower() == "true";

            foreach (var fileName in Directory.EnumerateFiles(sourceDirectory, filePattern,
              recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
              list.Add(relative ? fileName.Replace(sourceDirectory, ".") : fileName);
            }
          break;

          /* TODO: Wrong list nested action processing.
          default:
          break;
          */
        }
      }

      if (DataContext.Lists.ContainsKey(Name))
        DataContext.Lists[Name].AddRange(list);
      else
        DataContext.Lists.Add(Name, list);
    }
  }
}
