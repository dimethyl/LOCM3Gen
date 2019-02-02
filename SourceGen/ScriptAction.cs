using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen
{
  /// <summary>
  /// Base class for all SourceGen script actions.
  /// Derived classes must be decorated with one or several <see cref="ActionNameAttribute" /> to assign the action names to this script action.
  /// Properties of the derived class can be decorated with single <see cref="ActionParameterAttribute" /> to assign the action parameter name
  /// to the property.
  /// </summary>
  public abstract class ScriptAction
  {
    /// <summary>
    /// Name of the script action.
    /// </summary>
    public readonly string ActionName;

    /// <summary>
    /// Script data context for the script action.
    /// </summary>
    protected readonly ScriptDataContext DataContext;

    /// <summary>
    /// Parent script action this action depends on.
    /// </summary>
    protected readonly ScriptAction ParentAction;

    /// <summary>
    /// Script action XML element.
    /// </summary>
    protected readonly XElement ActionXmlElement;

    /// <summary>
    /// Nested XML elements enumeration.
    /// </summary>
    protected readonly IEnumerable<XElement> NestedXmlElements;

    /// <summary>
    /// Script action constructor.
    /// </summary>
    /// <param name="actionXmlElement">Action's XML element used to configure the script action.</param>
    /// <param name="dataContext">Script data context instance with variables and lists used on parsing.</param>
    /// <param name="parentAction">Parent script action this XML element is nested to. Can be null.</param>
    public ScriptAction(XElement actionXmlElement, ScriptDataContext dataContext, ScriptAction parentAction = null)
    {
      if (actionXmlElement == null)
        throw new ArgumentNullException(nameof(actionXmlElement));

      ActionName = actionXmlElement.Name.ToString();
      DataContext = dataContext ?? new ScriptDataContext();
      ParentAction = parentAction;
      ActionXmlElement = actionXmlElement;
      NestedXmlElements = actionXmlElement.Descendants();

      // Setting the values of member properties decorated with ActionParameterAttribute.
      foreach (var property in GetType().GetProperties())
      {
        var parameterAttribute = property.GetCustomAttribute(typeof(ActionParameterAttribute)) as ActionParameterAttribute;
        if (parameterAttribute == null)
          continue;

        var propertyValue = ParseText(actionXmlElement.Attribute(parameterAttribute.ActionParameterName)?.Value ?? "");
        if (property.PropertyType == typeof(string))
          property.SetValue(this, propertyValue);
        else if (property.PropertyType == typeof(bool))
          property.SetValue(this, propertyValue.ToLower() == "true");
        else if (property.PropertyType == typeof(int))
        {
          property.SetValue(this,
            int.TryParse(propertyValue, out var intValue)
              ? intValue
              : throw new InvalidCastException(
                $"Unable to cast string value \"{propertyValue}\" for property \"{property.Name}\" of type \"{property.PropertyType.Name}\"."));
        }
        else
        {
          throw new InvalidCastException(
            $"Unable to cast string value \"{propertyValue}\" for property \"{property.Name}\" of type \"{property.PropertyType.Name}\".");
        }
      }
    }

    /// <summary>
    /// Parses the patterns within the text.
    /// </summary>
    /// <param name="text">Text to parse.</param>
    /// <returns>Parsed text with all patterns being replaced.</returns>
    protected string ParseText(string text)
    {
      // Removing {%...%} patterns of comments.
      var result = Regex.Replace(text, @"\{\%(.*?)\%\}", "", RegexOptions.Compiled | RegexOptions.Singleline);

      // Parsing {$...$} patterns of variables.
      result = Regex.Replace(result, @"\{\$(\w*)\$\}", match =>
        DataContext.Variables.ContainsKey(match.Groups[1].Value) ? DataContext.Variables[match.Groups[1].Value] : "", RegexOptions.Compiled);

      // Parsing {#...#} patterns of iterators.
      result = Regex.Replace(result, @"\{\#(.*?)\#\}", match =>
        {
          // Checking the specified lists for the maximal iteration count.
          var iteratorString = match.Groups[1].Value;
          var iterations = 0;
          foreach (Match listMatch in Regex.Matches(iteratorString, @"\{\@(\w*)\@\}"))
          {
            var listName = listMatch.Groups[1].Value;
            if (!DataContext.Lists.ContainsKey(listName))
              continue;

            iterations = Math.Max(iterations, DataContext.Lists[listName].Count);
          }

          // Iteratively parsing {@...@} patterns of lists.
          var content = new StringBuilder(iterations);
          for (var index = 0; index < iterations; index++)
          {
            // ReSharper disable AccessToModifiedClosure
            content.Append(Regex.Replace(iteratorString, @"\{\@(\w*)\@\}",
              listMatch => DataContext.Lists.TryGetValue(listMatch.Groups[1].Value, out var list) && index < list.Count ? list[index] : ""));
            // ReSharper restore AccessToModifiedClosure
          }

          return content.ToString();
        }, RegexOptions.Compiled | RegexOptions.Singleline);

      return result;
    }

    /// <summary>
    /// Parses the patterns within the template file.
    /// </summary>
    /// <param name="fileName">Name of the file to parse.</param>
    protected void ParseFile(string fileName)
    {
      if (!File.Exists(fileName))
        return;

      var fileContents = File.ReadAllText(fileName);
      File.WriteAllText(fileName, ParseText(fileContents));
    }

    /// <summary>
    /// Defines all script action logic.
    /// </summary>
    public abstract void Invoke();
  }
}
