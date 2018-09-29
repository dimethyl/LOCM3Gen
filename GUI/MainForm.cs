/*
 * Copyright (C) 2018 Maxim Yudin <i@hal.su>. All rights reserved.
 * 
 * This file is a part of the closed source section of LOCM3Gen project.
 * You may NOT use, distribute, copy or modify this file without special author's permission.
 */

using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;

namespace LOCM3Gen.GUI
{
  /// <summary>
  /// Class of the GUI main form.
  /// </summary>
  public partial class MainForm : Form
  {
    /// <summary>
    /// Main form constructor.
    /// </summary>
    public MainForm()
    {
      try
      {
        InitializeComponent();

        BuildEnvironmentsList();
        BuildFamiliesList();
        ReadXMLSettings();

        foreach (Control control in this.Controls)
          control.TextChanged += new System.EventHandler(AllControls_TextChanged);
        AllControls_TextChanged(null, null);
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }
    
    /// <summary>
    /// Showing common exception description dialog.
    /// </summary>
    /// <param name="exception">Exception instance being described.</param>
    private void CatchException(Exception exception)
    {
      MessageBox.Show(exception.ToString(), "Exception caught on " + exception.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    
    /// <summary>
    /// Filling the list of supported project environments using environment XML files.
    /// </summary>
    private void BuildEnvironmentsList()
    {
      if (Directory.Exists(Configuration.environmentsDirectory))
      {
        foreach (var environmentXMLFile in Directory.EnumerateFiles(Configuration.environmentsDirectory, "*.xml", SearchOption.TopDirectoryOnly))
          environmentsList.Items.Add(Path.GetFileNameWithoutExtension(environmentXMLFile));
      }
      else
        Directory.CreateDirectory(Configuration.environmentsDirectory);
    }

    /// <summary>
    /// Filling the list of available microcontroller families using family XML files.
    /// </summary>
    private void BuildFamiliesList()
    {
      if (Directory.Exists(Configuration.familiesDirectory))
      {
        foreach (var familyXMLFile in Directory.EnumerateFiles(Configuration.familiesDirectory, "*.xml", SearchOption.TopDirectoryOnly))
          familiesList.Items.Add(Path.GetFileNameWithoutExtension(familyXMLFile));
      }
      else
        Directory.CreateDirectory(Configuration.familiesDirectory);
    }

    /// <summary>
    /// Filling the list of available family's devices.
    /// <param name="familyName">Name of the family of the devices.</param>
    /// </summary>
    private void BuildDevicesList(string familyName)
    {
      devicesList.Items.Clear();

      if (familyName == "")
        return;

      var familyFileName = Path.Combine(Configuration.familiesDirectory, familyName + ".xml");
      if (File.Exists(familyFileName))
      {
        var deviceNodes = XDocument.Load(Path.Combine(Configuration.familiesDirectory, familyName + ".xml")).Root?.Element("devices")?.Descendants("device") ?? new XElement[0];
        foreach (var node in deviceNodes)
        {
          var deviceName = node.Attribute("name")?.Value?.Trim();
          if (deviceName != null)
            devicesList.Items.Add(deviceName);
        }
      }

      if (devicesList.Items.Count > 0)
        devicesList.SelectedIndex = 0;
    }

    /// <summary>
    /// Reading saved program settings.
    /// </summary>
    private void ReadXMLSettings()
    {
      var configFileName = Path.Combine(Configuration.appDataDirectory, "Settings.xml");
      var selectedDevice = "";
      if (File.Exists(configFileName))
      {
        var settings = XDocument.Load(configFileName).Element("settings");
        locm3DirectoryInput.Text            = settings?.Element("locm3-directory")?.Value?.Trim() ?? "";
        projectDirectoryInput.Text          = settings?.Element("project-directory")?.Value?.Trim() ?? "";
        projectNameInput.Text               = settings?.Element("project-name")?.Value?.Trim() ?? "";
        projectSubdirectoryCheckbox.Checked = settings?.Element("create-subdirectory")?.Value?.Trim().ToLower() == "true";
        environmentsList.SelectedItem       = settings?.Element("project-environment")?.Value?.Trim() ?? "";
        familiesList.SelectedItem           = settings?.Element("selected-family")?.Value?.Trim() ?? "";
        selectedDevice                      = settings?.Element("selected-device")?.Value?.Trim() ?? "";
      }

      if (environmentsList.SelectedIndex < 0 && environmentsList.Items.Count > 0)
        environmentsList.SelectedIndex = 0;
      if (familiesList.SelectedIndex < 0 && familiesList.Items.Count > 0)
        familiesList.SelectedIndex = 0;

      BuildDevicesList(familiesList.Text);
      devicesList.SelectedItem = selectedDevice;
      if (devicesList.SelectedIndex < 0 && devicesList.Items.Count > 0)
        devicesList.SelectedIndex = 0;
    }

    /// <summary>
    /// Writing current program settings.
    /// </summary>
    private void WriteXMLSettings()
    {
      if (!Directory.Exists(Configuration.appDataDirectory))
        Directory.CreateDirectory(Configuration.appDataDirectory);

      var settings = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("settings",
        new XElement("locm3-directory",     locm3DirectoryInput.Text.Trim()),
        new XElement("project-directory",   projectDirectoryInput.Text.Trim()),
        new XElement("project-name",        projectNameInput.Text.Trim()),
        new XElement("create-subdirectory", projectSubdirectoryCheckbox.Checked.ToString()),
        new XElement("project-environment", environmentsList.Text.Trim()),
        new XElement("selected-family",     familiesList.Text.Trim()),
        new XElement("selected-device",     devicesList.Text.Trim())
      ));
      settings.Save(Path.Combine(Configuration.appDataDirectory, "Settings.xml"));
    }

    /// <summary>
    /// Showing libopencm3 directory selector dialog.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void Locm3DirectoryButton_Click(object sender, EventArgs e)
    {
      try
      {
        directorySelector.SelectedPath = locm3DirectoryInput.Text.Trim();
        if (directorySelector.ShowDialog() == DialogResult.OK)
        {
          locm3DirectoryInput.Text = directorySelector.SelectedPath;
        }
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Showing project directory selector dialog.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ProjectDirectoryButton_Click(object sender, EventArgs e)
    {
      try
      {
        directorySelector.SelectedPath = projectDirectoryInput.Text.Trim();
        if (directorySelector.ShowDialog() == DialogResult.OK)
        {
          projectDirectoryInput.Text = directorySelector.SelectedPath;
        }
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Project name input cleaning event for complying <c>[A-Za-z0-9_]</c> pattern.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ProjectNameInput_TextChanged(object sender, EventArgs e)
    {   
      try
      {
        var input = sender as TextBoxBase;
        var cursorPosition = input.SelectionStart;
        input.Text = Regex.Replace(input.Text.Trim(), @"\W+", "");
        input.SelectionStart = cursorPosition;
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Devices list building on family selection event.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void FamiliesList_SelectedIndexChanged(object sender, EventArgs e)
    {
      try
      {
        BuildDevicesList(familiesList.Text.Trim());
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Number inputs cleaning event to accept only digits.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void NumberInputs_TextChanged(object sender, EventArgs e)
    {   
      try
      {
        var input = sender as TextBoxBase;
        var cursorPosition = input.SelectionStart;
        input.Text = Regex.Replace(input.Text.Trim(), @"\D+", "");
        input.SelectionStart = cursorPosition;
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Changing <i>Enabled</i> state of project generation button depending on the correctness of the entered values.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void AllControls_TextChanged(object sender, EventArgs e)
    {
      try
      {
        if (Directory.Exists(locm3DirectoryInput.Text.Trim()) &&
          projectDirectoryInput.Text.Trim().Length > 0 &&
          Regex.IsMatch(projectNameInput.Text.Trim(), @"\w+") &&
          environmentsList.SelectedIndex >= 0 &&
          familiesList.SelectedIndex >= 0 &&
          devicesList.SelectedIndex >= 0)
        {
          generateButton.Enabled = true;
        }
        else
        {
          generateButton.Enabled = false;
        }
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Project generation button click processing.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void GenerateButton_Click(object sender, EventArgs e)
    {
      try
      {
        generateButton.Enabled = false;

        // Filling general variables.
        var generator = new SourceGen.ScriptReader();
        generator.variables.Add("ProgramDir", Configuration.programDirectory);
        generator.variables.Add("TemplatesDir", Configuration.templatesDirectory);
        generator.variables.Add("FamiliesDir", Configuration.familiesDirectory);
        generator.variables.Add("EnvironmentsDir", Configuration.environmentsDirectory);
        generator.variables.Add("LOCM3Dir", locm3DirectoryInput.Text.Trim());
        generator.variables.Add("ProjectDir", projectSubdirectoryCheckbox.Checked ? Path.Combine(projectDirectoryInput.Text.Trim(), projectNameInput.Text.Trim()) : projectDirectoryInput.Text.Trim());
        generator.variables.Add("ProjectName", projectNameInput.Text.Trim());
        generator.variables.Add("DeviceName", devicesList.Text.Trim());
        generator.variables.Add("Date", DateTime.Now.ToShortDateString());
        generator.variables.Add("Time", DateTime.Now.ToShortTimeString());
        generator.variables.Add("UserName", Environment.UserName);
        generator.variables.Add("MachineName", Environment.MachineName);
                
        //HACK: Processing temporary script files.
        generator.ReadScript(@"D:\Projects\C#\LOCM3Gen\SourceGen\STM32F0.xml");
        generator.ReadScript(@"D:\Projects\C#\LOCM3Gen\SourceGen\EmBitz.xml");

        /*
        var parameters = new ProjectParameters
        {
          locm3Directory    = locm3DirectoryInput.Text.Trim(),
          projectDirectory  = projectSubdirectoryCheckbox.Checked ? Path.Combine(projectDirectoryInput.Text.Trim(), projectNameInput.Text.Trim()) : projectDirectoryInput.Text.Trim(),
          projectName       = projectNameInput.Text.Trim(),
          deviceName        = devicesList.Text.Trim()
        };

        var settings = new GeneratorSettings();
        settings.ReadControlFile(Path.Combine(Configuration.familiesDirectory, familiesList.Text.Trim() + ".xml"));
        settings.ReadControlFile(Path.Combine(Configuration.environmentsDirectory, environmentsList.Text.Trim() + ".xml"));

        var generator = new Generator(parameters, settings);
        generator.Generate();
        */

        // Showing generation success dialog.
        MessageBox.Show("Project \"" + projectNameInput.Text.Trim() + "\" has been successfully created in \"" + projectDirectoryInput.Text.Trim() + "\".",
          this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
      finally
      {
        GC.Collect();

        generateButton.Enabled = true;
      }
    }

    /// <summary>
    /// Show program info on <i>About</i>> label click.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void AboutLabel_Click(object sender, EventArgs e)
    {
      try
      {
        MessageBox.Show("libopencm3 Project Generator v" + Configuration.version.ToString(2) + "\n" +
          "Build " + Configuration.version.Build + "." + Configuration.version.Revision + "\n\n" +       
          "Copyright (c) 2018 Maxim Yudin <i@hal.su>",
          "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Close the main form and terminate the application.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        WriteXMLSettings();
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }
  }
}
