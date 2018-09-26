/*
 * Copyright (C) 2018 Maxim Yudin <i@hal.su>. All rights reserved.
 * 
 * This file is a part of the closed source section of LOCM3Gen project.
 * You may NOT use, distribute, copy or modify this file without special author's permission.
 */
 
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace LOCM3Gen
{
  /// <summary>
  /// Generator settings class that determines project generation process.
  /// </summary>
  public class GeneratorSettings
  {
    /// <summary>
    /// Subclass for storing the route information for files copying during project generation.
    /// </summary>
    public class CopyRoute
    {
      /// <summary>
      /// Path to the source location where the files will be taken from.
      /// </summary>
      public string source;
  
      /// <summary>
      /// Path to the destination location where files will be copied to.
      /// </summary>
      public string destination;
  
      /// <summary>
      /// File name pattern to constrain the list of copying files.
      /// </summary>
      public string fileNamePattern;
  
      /// <summary>
      /// Flag for preventing the existing files from being overwritten.
      /// </summary>
      public bool keepExistingFiles;
    }
    
    /// <summary>
    /// Subclass for storing family's device properties.          
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
    /// Subclass for storing microcontroller family properties.
    /// </summary>
    public class Family
    {
      /// <summary>
      /// Microcontroller family name.
      /// </summary>
      public string name;    
  
      /// <summary>
      /// Architecture core type (cortex-mx).
      /// </summary>
      public string core;    
  
      /// <summary>
      /// Floating point code generation type (soft/softfp/hard).
      /// </summary>
      public string fpType; 
  
      /// <summary>
      /// Floating point hardware unit type.
      /// </summary>
      public string fpUnit;
  
      /// <summary>
      /// SVD files archive copy route.
      /// <i>fileNamePattern</i> field is ignored for the archive.
      /// </summary>
      public CopyRoute svdArchive;
  
      /// <summary>
      /// List of available family's devices (keys = device names).
      /// </summary>
      public Dictionary<string, Device> devices = new Dictionary<string, Device>();
    }
    
    /// <summary>
    /// List of libopencm3 source files copy routes.
    /// </summary>
    public List<CopyRoute> sourceRoutes = new List<CopyRoute>(); 
    
    /// <summary>
    /// List of template files copy routes.
    /// </summary>
    public List<CopyRoute> templateRoutes = new List<CopyRoute>();
  
    /// <summary>
    /// List of linked .a library files.
    /// </summary>
    public List<string> libraries = new List<string>();
  
    /// <summary>
    /// List of compiler symbols to be defined in the project.
    /// </summary>
    public List<string> definitions = new List<string>();
    
    /// <summary>
    /// Instance of the selected microcontroller family.
    /// </summary>
    public Family family;
    
    /// <summary>
    /// Name of the target project environment.
    /// </summary>
    public string environmentName;  
    
    /// <summary>
    /// Read complimentary data from the specified XML control file and update generator settings with it.
    /// </summary>
    /// <param name="controlFileName">Path to the XML control file.</param>
    public void ReadControlFile(string controlFileName)
    {
      //Root node
      var rootNode = XDocument.Load(controlFileName).Root;
      if (rootNode == null)
        return;
      
      //"family" node.
      var familyNode = rootNode?.Element("family");
      if (familyNode != null)
      {
        family = new Family
        {
          name    = familyNode.Attribute("name")?.Value?.Trim() ?? "Unknown",
          core    = familyNode.Attribute("core")?.Value?.Trim() ?? "",
          fpType  = familyNode.Attribute("fp-type")?.Value?.Trim() ?? "",
          fpUnit  = familyNode.Attribute("fp-unit")?.Value?.Trim() ?? ""
        };
      }
      
      //"environment" node.
      var environmentNode = rootNode.Element("environment");
      if (environmentNode != null)      
        environmentName = environmentNode.Attribute("name")?.Value?.Trim() ?? "Unknown"; 

      //"svd-archive" node.
      var svdArchiveNode = rootNode.Element("svd-archive");   
      if (svdArchiveNode != null)
      {
        family.svdArchive = new CopyRoute
        {
          source              = svdArchiveNode?.Value?.Trim() ?? "",
          destination         = svdArchiveNode?.Attribute("project-directory")?.Value?.Trim() ?? ".",
          keepExistingFiles   = (svdArchiveNode.Attribute("keep-existing")?.Value?.Trim() ?? "") == "true"
        };  
      }

      //"devices" node.
      var devicesNode = rootNode.Element("devices");
      if (devicesNode != null)
      {
        foreach (var node in devicesNode.Descendants("device") ?? new XElement[0])
        {
          var name = node.Attribute("name")?.Value?.Trim() ?? "Unknown";
          if (!family.devices.ContainsKey(name))
          {
            family.devices.Add(name, new Device
            {
              name        = name,
              romSize     = node.Attribute("rom")?.Value?.Trim() ?? "0",
              ramSize     = node.Attribute("ram")?.Value?.Trim() ?? "0",
              svdFileName = node.Attribute("svd-file")?.Value?.Trim() ?? ""
            });
          }
        }
      }
      
      //"sources" node.
      var sourcesNode = rootNode.Element("sources");
      if (sourcesNode != null)
      {
        foreach (var node in sourcesNode.Descendants("include") ?? new XElement[0])
        {
          sourceRoutes.Add(new CopyRoute {
            source              = Path.GetDirectoryName(node.Value?.Trim() ?? ""),
            destination         = node.Attribute("project-directory")?.Value?.Trim() ?? ".",
            fileNamePattern     = Path.GetFileName(node.Value?.Trim() ?? ""),
            keepExistingFiles   = (node.Attribute("keep-existing")?.Value?.Trim() ?? "") == "true"
          });
        }
      }

      //"templates" node.
      var templatesNode = rootNode.Element("templates");
      if (templatesNode != null)
      {
        foreach (var node in templatesNode.Descendants("include") ?? new XElement[0])
        {
          templateRoutes.Add(new CopyRoute {
            source              = Path.GetDirectoryName(node.Value?.Trim() ?? ""),
            destination         = node.Attribute("project-directory")?.Value?.Trim() ?? ".",
            fileNamePattern     = Path.GetFileName(node.Value?.Trim() ?? ""),
            keepExistingFiles   = (node.Attribute("keep-existing")?.Value?.Trim() ?? "") == "true"
          });
        }
      }

      //"libraries" node.
      var librariesNode = rootNode.Element("libraries");
      if (librariesNode != null)
      {
        foreach (var node in librariesNode.Descendants("library") ?? new XElement[0])
          libraries.Add(node.Value?.Trim() ?? "");
      }

      //"definitions" node.
      var definitionsNode = rootNode.Element("definitions");
      if (definitionsNode != null)
      {
        foreach (var node in definitionsNode.Descendants("define") ?? new XElement[0])
          definitions.Add(node.Value?.Trim() ?? "");
      }
    }
    
    /// <summary>
    /// Search for the device instance by its name inside the selected family.
    /// </summary>
    /// <param name="deviceName">Name of the device inside the selected family.</param>
    /// <returns>Device instance if it is found inside the family, otherwise null.</returns>
    public Device GetTargetDevice(string deviceName)
    {
      if (family.devices.ContainsKey(deviceName))
        return family.devices[deviceName];
      else
        return null;
    }
  }
}
