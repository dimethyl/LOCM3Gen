using System;
using System.Text.RegularExpressions;

namespace LOCM3Gen.SourceGen
{
  /// <summary>
  /// Attribute that assigns the action parameter name to the property of the <see cref="ScriptAction" />-derived class.
  /// The decorated property will be automatically set to the value provided by the action's XML element.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class ActionParameterAttribute : Attribute
  {
    /// <summary>
    /// The action parameter name assigned to the decorated property.
    /// </summary>
    public readonly string ActionParameterName;

    /// <summary>
    /// Defines whether to parse the parameter's value during property setting.
    /// </summary>
    public readonly bool ParseParameterValue;

    /// <summary>
    /// Attribute constructor assigning the action parameter name to the property of the <see cref="ScriptAction" />-derived class.
    /// </summary>
    /// <param name="actionParameterName">Action parameter name being assigned.</param>
    /// <param name="parseParameterValue">Defines whether to parse the parameter's value during property setting.</param>
    public ActionParameterAttribute(string actionParameterName, bool parseParameterValue)
    {
      if (string.IsNullOrWhiteSpace(actionParameterName) || !Regex.IsMatch(actionParameterName, @"^[-\w]+$"))
        throw new ArgumentNullException(nameof(actionParameterName),
          "Action parameter name cannot be empty and must contain only alphanumeric and hyphen characters.");

      ActionParameterName = actionParameterName;
      ParseParameterValue = parseParameterValue;
    }
  }
}
