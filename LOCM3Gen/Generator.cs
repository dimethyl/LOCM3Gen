/*
 * Copyright (C) 2018 Maxim Yudin <i@hal.su>. All rights reserved.
 * 
 * This file is a part of the closed source section of LOCM3Gen project.
 * You may NOT use, distribute, copy or modify this file without special author's permission.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace LOCM3Gen
{
  /// <summary>
  /// Class of project generator.
  /// </summary>
  public class Generator
  {
    /// <summary>
    /// Project variables used for project generation.
    /// </summary>
    public readonly ProjectParameters parameters;

    /// <summary>
    /// Generator settings used for project generation.
    /// </summary>
    public readonly GeneratorSettings settings;

    /// <summary>
    /// Project generator constructor.
    /// </summary>
    /// <param name="parameters">Project variables used for project generation.</param>
    /// <param name="settings">Generator settings used for project generation.</param>
    public Generator(ProjectParameters parameters, GeneratorSettings settings)
    {
      this.parameters = parameters;
      this.settings   = settings;
    }

    /// <summary>
    /// Asyncronously generate project with the specified settings in the project directory.
    /// </summary>
    public void Generate()
    {
      //Checking the project generation settings.
      if (!Directory.Exists(parameters.locm3Directory))
        throw new DirectoryNotFoundException("Wrong libopencm3 directory \"" + parameters.locm3Directory + "\" specified.");
      if (!Directory.Exists(parameters.projectDirectory))
        Directory.CreateDirectory(parameters.projectDirectory);
      if (!Regex.IsMatch(parameters.projectName, @"\w+"))
        throw new FormatException("Inacceptable project name \"" + parameters.projectName + "\" specified. Only [A-Za-z0-9_] characters allowed.");
      var device = settings.GetTargetDevice(parameters.deviceName);
      if (device == null)
        throw new KeyNotFoundException("Device name \"" + parameters.deviceName + "\" was not found inside family \"" + settings.family.name + "\".");
      
      //Copying libopencm3 source files to the project directory.
      foreach (var route in settings.sourceRoutes)
      {
        var fullSourcePath = Path.Combine(parameters.locm3Directory, route.source);
        if (Directory.Exists(fullSourcePath))
        {
          foreach (var fullFileName in Directory.EnumerateFiles(fullSourcePath, route.fileNamePattern, SearchOption.TopDirectoryOnly))
          {
            var fullDestinationPath = Path.Combine(parameters.projectDirectory, route.destination);
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
      foreach (var route in settings.templateRoutes)
      {
        var fullTemplatePath = Path.Combine(Configuration.templatesDirectory, route.source);
        if (Directory.Exists(fullTemplatePath))
        {
          foreach (var fullFileName in Directory.EnumerateFiles(fullTemplatePath, route.fileNamePattern, SearchOption.TopDirectoryOnly))
          {
            var fullDestinationPath = Path.Combine(parameters.projectDirectory, route.destination);
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
      if (File.Exists(settings.family.svdArchive.source) && device.svdFileName != "")
      {
        using (var svdArchive = ZipFile.OpenRead(settings.family.svdArchive.source))
        {
          var svdStream = svdArchive.GetEntry(device.svdFileName);
          if (svdStream != null)
          {
            var fullDestinationFileName = Path.Combine(parameters.projectDirectory, settings.family.svdArchive.destination, Path.GetFileName(device.svdFileName));
            if (!File.Exists(fullDestinationFileName) || !settings.family.svdArchive.keepExistingFiles)
            {
              using (var svdFile = new StreamWriter(fullDestinationFileName))
              {
                svdStream.Open().CopyTo(svdFile.BaseStream);
                svdFile.Flush();
              }
            }
          }
        }
      }

      //Building project files list from files with determined extensions.
      var projectFiles = new List<string>();
      foreach (var projectFile in Directory.EnumerateFiles(parameters.projectDirectory, "*", SearchOption.AllDirectories))
      {
        var fileExtension = Path.GetExtension(projectFile);
        if (Configuration.projectFileExtensions.Contains(fileExtension))
          projectFiles.Add(projectFile.Replace(parameters.projectDirectory, "."));
      }

      //Preparing formatter variables and lists.
      var formatter = new TemplateFormatter();
      formatter.variables.Add("ProjectName",  parameters.projectName);
      formatter.variables.Add("FamilyName",   settings.family.name);
      formatter.variables.Add("Core",         settings.family.core);
      formatter.variables.Add("FPType",       settings.family.fpType);
      formatter.variables.Add("FPUnit",       settings.family.fpUnit);
      formatter.variables.Add("DeviceName",   device.name);
      formatter.variables.Add("ROMSize",      device.romSize);
      formatter.variables.Add("RAMSize",      device.ramSize);
      formatter.variables.Add("SVDFile",      device.svdFileName);
      formatter.variables.Add("Date",         DateTime.Now.ToShortDateString());
      formatter.variables.Add("Time",         DateTime.Now.ToShortTimeString());
      formatter.variables.Add("UserName",     Environment.UserName);
      formatter.variables.Add("MachineName",  Environment.MachineName);
      formatter.lists.Add("ProjectFiles",      projectFiles);      
      formatter.lists.Add("Libraries",         settings.libraries);      
      formatter.lists.Add("Definitions",       settings.definitions);

      //Formatting template files from the list.
      foreach (var templateFileName in templateFiles)
        formatter.ProcessFile(templateFileName);
    }
  }
}
