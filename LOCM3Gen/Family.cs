using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace LOCM3Gen
{
  /// <summary>
  /// Microcontroller family description class.
  /// </summary>
  public class Family
  {
    /// <summary>
    /// Class for storing family's device properties.
    /// </summary>
    public class Device
    {
      /// <summary>
      /// Name (part number) of the device.
      /// </summary>
      public string name;

      /// <summary>
      /// ROM size of the device.
      /// </summary>
      public string romSize;

      /// <summary>
      /// RAM size of the device.
      /// </summary>
      public string ramSize;

      /// <summary>
      /// Name of the related to the device SVD file inside the SVD files zip-archive.
      /// </summary>
      public string svdFileName;
    }

    /// <summary>
    /// Microcontroller family name.
    /// </summary>
    public readonly string name;    

    /// <summary>
    /// Architecture core type (cortex-mx).
    /// </summary>
    public readonly string core;    

    /// <summary>
    /// Floating point code generation type (soft/softfp/hard).
    /// </summary>
    public readonly string fpType; 

    /// <summary>
    /// Floating point hardware unit type.
    /// </summary>
    public readonly string fpUnit; 

    /// <summary>
    /// List of libopencm3 source files copy routes.
    /// </summary>
    public readonly List<CopyRoute> sourceRoutes = new List<CopyRoute>(); 
    
    /// <summary>
    /// List of template files copy routes.
    /// </summary>
    public readonly List<CopyRoute> templateRoutes = new List<CopyRoute>();

    /// <summary>
    /// List of linked .a library files.
    /// </summary>
    public readonly List<string> libraries = new List<string>();

    /// <summary>
    /// List of compiler symbols to be defined in the project.
    /// </summary>
    public readonly List<string> definitions = new List<string>();

    /// <summary>
    /// SVD files archive definition
    /// </summary>
    public readonly CopyRoute svdArchive;

    /// <summary>
    /// List of available family's devices (keys = device names).
    /// </summary>
    public readonly Dictionary<string, Family.Device> devices = new Dictionary<string, Family.Device>();

    /// <summary>
    /// Microcontroller family class constructor that reads data from the specified XML file. 
    /// </summary>
    /// <param name="familyXMLFile">Path to the microcontroller family XML file.</param>
    public Family(string familyXMLFile)
    {
      //"family" root node.
      var familyNode = XDocument.Load(familyXMLFile).Element("family");
      name = familyNode?.Attribute("name")?.Value?.Trim() ?? "Unknown";
      core = familyNode?.Attribute("core")?.Value?.Trim() ?? "";
      fpType = familyNode?.Attribute("fp-type")?.Value?.Trim() ?? "";
      fpUnit = familyNode?.Attribute("fp-unit")?.Value?.Trim() ?? "";

      //"sources" nested node.
      foreach (var node in familyNode?.Element("sources")?.Descendants("include") ?? new XElement[0])
      {
        sourceRoutes.Add(new CopyRoute {
          source              = Path.GetDirectoryName(node.Value?.Trim() ?? ""),
          destination         = node.Attribute("project-directory")?.Value?.Trim() ?? ".",
          fileNamePattern     = Path.GetFileName(node.Value?.Trim() ?? ""),
          keepExistingFiles   = (node.Attribute("keep-existing")?.Value?.Trim() ?? "") == "true"
        });
      }

      //"templates" nested node.
      foreach (var node in familyNode?.Element("templates")?.Descendants("include") ?? new XElement[0])
      {
        templateRoutes.Add(new CopyRoute {
          source              = Path.GetDirectoryName(node.Value?.Trim() ?? ""),
          destination         = node.Attribute("project-directory")?.Value?.Trim() ?? ".",
          fileNamePattern     = Path.GetFileName(node.Value?.Trim() ?? ""),
          keepExistingFiles   = (node.Attribute("keep-existing")?.Value?.Trim() ?? "") == "true"
        });
      }

      //"libraries" nested node.
      foreach (var node in familyNode?.Element("libraries")?.Descendants("library") ?? new XElement[0])
      {
        libraries.Add(node.Value?.Trim() ?? "");
      }

      //"definitions" nested node.
      foreach (var node in familyNode?.Element("definitions")?.Descendants("define") ?? new XElement[0])
      {
        definitions.Add(node.Value?.Trim() ?? "");
      }

      //"svd-archive" nested node.
      var svdArchiveNode = familyNode?.Element("svd-archive");
      svdArchive = new CopyRoute
      {
        source              = svdArchiveNode?.Value?.Trim() ?? "",
        destination         = svdArchiveNode?.Attribute("project-directory")?.Value?.Trim() ?? ".",
        fileNamePattern     = "*.svd",
        keepExistingFiles   = (svdArchiveNode.Attribute("keep-existing")?.Value?.Trim() ?? "") == "true"
      };


      //"devices" nested node.
      foreach (var node in familyNode?.Element("devices")?.Descendants("device") ?? new XElement[0])
      {
        var name = node.Attribute("name")?.Value ?? "Unknown";
        if (!devices.ContainsKey(name))
        {
          devices.Add(name, new Family.Device
          {
            name        = name,
            romSize     = node.Attribute("rom")?.Value ?? "0",
            ramSize     = node.Attribute("ram")?.Value ?? "0",
            svdFileName = node.Attribute("svd-file")?.Value ?? ""
          });
        }
      }
    }
  }
}
