using System;
using System.Text.RegularExpressions;

namespace LOCM3Gen.SourceGen
{
  /// <summary>
  /// Attribute that assigns the action name to the <see cref="ScriptAction" />-derived class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class ActionNameAttribute : Attribute
  {
    /// <summary>
    /// Action name of the the <see cref="ScriptAction" />-derived class.
    /// </summary>
    public readonly string ActionName;

    /// <summary>
    /// Attribute constructor assigning the action name to the <see cref="ScriptAction" />-derived class.
    /// </summary>
    /// <param name="actionName">Action name.</param>
    public ActionNameAttribute(string actionName)
    {
      if (string.IsNullOrWhiteSpace(actionName) || !Regex.IsMatch(actionName, @"^[-\w]+$"))
        throw new ArgumentNullException(nameof(actionName), "Action name cannot be empty and must contain only alphanumeric and hyphen characters.");

      ActionName = actionName;
    }
  }
}
