using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LOCM3Gen
{
  /// <summary>
  /// Class used for <i>libopencm3</i> project generation.
  /// </summary>
  class ProjectGenerator
  {
    /// <summary>
    /// Class for storing project generator settings.
    /// </summary>
    public class Settings
    {
      /// <summary>
      /// <i>libopencm3</i> directory.
      /// </summary>
      public string locm3Directory;

      /// <summary>
      /// Target project generation directory.
      /// </summary>
      public string projectDirectory;

      /// <summary>
      /// Name of the generating project.
      /// </summary>
      public string projectName;

      /// <summary>
      /// Target project environment.
      /// </summary>
      public Environment environment;

      /// <summary>
      /// Target microcontroller family.
      /// </summary>
      public Family family; 

      /// <summary>
      /// Target device name (part number).
      /// </summary>
      public string deviceName;
    }

    /// <summary>
    /// Generator settings used for project generation.
    /// </summary>
    public readonly ProjectGenerator.Settings settings;

    /// <summary>
    /// Project generator constructor.
    /// </summary>
    /// <param name="settings">Generator settings used for project generation.</param>
    public ProjectGenerator(ProjectGenerator.Settings settings)
    {
      this.settings = settings;
    }

    /// <summary>
    /// Asyncronously generate project with the specified settings in the project directory.
    /// </summary>
    public void GenerateAsync()
    {
      //Checking the project generation settings.
      if (!Directory.Exists(settings.locm3Directory))
        throw new DirectoryNotFoundException("Wrong libopencm3 directory \"" + settings.locm3Directory + "\" specified.");
      if (!Directory.Exists(settings.projectDirectory))
        Directory.CreateDirectory(settings.projectDirectory);
      if (!Regex.IsMatch(settings.projectName, @"\w+"))
        throw new FormatException("Inacceptable project name \"" + settings.projectName + "\" specified. Only [A-Za-z0-9_] characters allowed.");
      if (!settings.family.devices.ContainsKey(settings.deviceName))
        throw new KeyNotFoundException("Device name \"" + settings.deviceName + "\" not found inside family \"" + settings.family.name + "\".");
      
      //Copying libopencm3 source files to the project directory.
      foreach (var route in settings.family.sourceRoutes)
      {
        var fullSourcePath = Path.Combine(settings.locm3Directory, route.source);
        if (Directory.Exists(fullSourcePath))
        {
          foreach (var fullFileName in Directory.EnumerateFiles(fullSourcePath, route.fileNamePattern, SearchOption.TopDirectoryOnly))
          {
            var fullDestinationPath = Path.Combine(settings.projectDirectory, route.destination);
            var fullDestinationFileName = Path.Combine(fullDestinationPath, Path.GetFileName(fullFileName));
            if (!Directory.Exists(fullDestinationPath))
              Directory.CreateDirectory(fullDestinationPath);
            if (!File.Exists(fullDestinationFileName) || !route.keepExistingFiles)
              File.Copy(fullFileName, fullDestinationFileName, true);
          }
        }
      }

      //Copying templates to the project directory.
      var templateFiles = new List<string>();
      var templateRoutes = new List<CopyRoute>();
      templateRoutes.AddRange(settings.family.templateRoutes);
      templateRoutes.AddRange(settings.environment.templateRoutes);     
      foreach (var route in templateRoutes)
      {
        var fullTemplatePath = Path.Combine(Configuration.templatesDirectory, route.source);
        if (Directory.Exists(fullTemplatePath))
        {
          foreach (var fullFileName in Directory.EnumerateFiles(fullTemplatePath, route.fileNamePattern, SearchOption.TopDirectoryOnly))
          {
            var fullDestinationPath = Path.Combine(settings.projectDirectory, route.destination);
            var fullDestinationFileName = Path.Combine(fullDestinationPath, Path.GetFileName(fullFileName));
            if (!Directory.Exists(fullDestinationPath))
              Directory.CreateDirectory(fullDestinationPath);
            if (!File.Exists(fullDestinationFileName) || !route.keepExistingFiles)
            {
              File.Copy(fullFileName, fullDestinationFileName, true);
              templateFiles.Add(fullDestinationFileName);
            }
          }
        }
      }

      //Copying the neccessary SVD file from the SVD files archive.
      if (File.Exists(settings.family.svdArchive.source))
      {
        var svdArchive = ZipFile.OpenRead(settings.family.svdArchive.source);
        var svdFileName = settings.family.devices[settings.deviceName].svdFileName;
        if (svdFileName != "" && svdArchive.GetEntry(svdFileName) != null)
        {
          var fullDestinationFileName = Path.Combine(settings.projectDirectory, settings.family.svdArchive.destination, Path.GetFileName(svdFileName));
          if (!File.Exists(fullDestinationFileName) || !settings.family.svdArchive.keepExistingFiles)
          {
            var svdFile = new StreamWriter(fullDestinationFileName);
            svdArchive.GetEntry(svdFileName).Open().CopyTo(svdFile.BaseStream);
            svdFile.Flush();
          }  
        }
      }

      //Building project files list from files with determined extensions.
      var projectFiles = new List<string>();
      foreach (var projectFile in Directory.EnumerateFiles(settings.projectDirectory, "*", SearchOption.AllDirectories))
      {
        var fileExtension = Path.GetExtension(projectFile);
        if (Configuration.projectFileExtensions.Contains(fileExtension))
          projectFiles.Add(projectFile.Replace(settings.projectDirectory, "."));
      }

      //Preparing template parameters.
      var template = new TemplateFormatter();
      template.parameters.Add("ProjectName",  settings.projectName);
      template.parameters.Add("FamilyName",   settings.family.name);
      template.parameters.Add("Core",         settings.family.core);
      template.parameters.Add("FPType",       settings.family.fpType);
      template.parameters.Add("FPUnit",       settings.family.fpUnit);
      template.parameters.Add("DeviceName",   settings.family.devices[settings.deviceName].name);
      template.parameters.Add("ROMSize",      settings.family.devices[settings.deviceName].romSize);
      template.parameters.Add("RAMSize",      settings.family.devices[settings.deviceName].ramSize);
      template.parameters.Add("SVDFile",      settings.family.devices[settings.deviceName].svdFileName);
      template.parameters.Add("Date",         DateTime.Now.ToShortDateString());
      template.parameters.Add("Time",         DateTime.Now.ToShortTimeString());
      template.parameters.Add("UserName",     System.Environment.UserName);
      template.parameters.Add("MachineName",  System.Environment.MachineName);
      template.lists.Add("ProjectFiles",      projectFiles);      
      template.lists.Add("Libraries",         settings.family.libraries);      
      template.lists.Add("Definitions",       settings.family.definitions);

      //Processing template files from the list.
      foreach (var templateFileName in templateFiles)
        template.ProcessFile(templateFileName);
    }
  }
}
