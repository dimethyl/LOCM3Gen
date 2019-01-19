using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LOCM3Gen.SourceGen
{
  public class ScriptException : Exception
  {
    public readonly string ScriptName;

    public readonly XElement ActionXmlElement;

    public readonly string ActionParameter;

    public readonly string ErrorMessage;

    private static string GetMessage(string errorMessage, XElement actionXmlElement, string actionParameter, string scriptName)
    {
      var message = new StringBuilder("SourceGen script error found");

      message.Append(scriptName != null ? $" while processing \"{scriptName}\":\n" : ":\n");
      message.Append(actionXmlElement != null ? $"Action node: {Regex.Match(actionXmlElement.ToString(), "<[^>]+>")}\n" : "");
      message.Append(actionParameter != null ? $"Parameter: {actionParameter}\n" : "");
      message.Append($"Error: {errorMessage}");

      return message.ToString();
    }

    public ScriptException(string errorMessage, XElement actionXmlElement = null, string actionParameter = null, string scriptName = null)
      : base(GetMessage(errorMessage, actionXmlElement, actionParameter, scriptName))
    {
      ScriptName = scriptName;
      ActionXmlElement = actionXmlElement;
      ActionParameter = actionParameter;
      ErrorMessage = errorMessage;
    }
  }
}
