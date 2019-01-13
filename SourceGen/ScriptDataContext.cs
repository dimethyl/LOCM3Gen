using System.Collections.Generic;

namespace LOCM3Gen.SourceGen
{
  /// <summary>
  /// Script data context class.
  /// </summary>
  public class ScriptDataContext
  {
    /// <summary>
    /// Dictionary of script variables.
    /// </summary>
    public readonly Dictionary<string, string> Variables = new Dictionary<string, string>();

    /// <summary>
    /// Dictionary of script lists.
    /// Keys represent list names and values represent the collections of strings contained in the list.
    /// </summary>
    public readonly Dictionary<string, List<string>> Lists = new Dictionary<string, List<string>>();
  }
}
