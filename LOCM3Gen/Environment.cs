using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace LOCM3Gen
{
  /// <summary>
  /// Project environment description class.
  /// </summary>
  public class Environment
  {
    /// <summary>
    /// Project environment name.
    /// </summary>
    public readonly string name;

    /// <summary>
    /// List of template files copy routes.
    /// </summary>
    public readonly List<CopyRoute> templateRoutes = new List<CopyRoute>();
    
    /// <summary>
    /// Project environment class constructor that reads data from the specified XML file. 
    /// </summary>
    /// <param name="environmentXMLFile">Path to the project environment XML file.</param>
    public Environment(string environmentXMLFile)
    {
      //"family" root node.
      var environmentNode = XDocument.Load(environmentXMLFile).Element("environment"); 
      name = environmentNode?.Attribute("name")?.Value?.Trim() ?? "Unknown";

      //"templates" nested node.
      foreach (var node in environmentNode?.Element("templates")?.Descendants("include") ?? new XElement[0])
      {
        templateRoutes.Add(new CopyRoute {
          source       = Path.GetDirectoryName(node.Value?.Trim() ?? ""),
          destination  = node.Attribute("project-directory")?.Value?.Trim() ?? ".",
          fileNamePattern       = Path.GetFileName(node.Value?.Trim() ?? ""),
          keepExistingFiles     = (node.Attribute("keep-existing")?.Value?.Trim() ?? "") == "true"
        });
      }   
    }
  }
}
