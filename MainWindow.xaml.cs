using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace LOCM3Gen
{
  /// <summary>
  /// Main window class with interaction logic for <i>MainWindow.xaml</i> file.
  /// </summary>
  public partial class MainWindow
  {
    /// <summary>
    /// Main window constructor.
    /// </summary>
    public MainWindow()
    {
      try
      {
        InitializeComponent();

        BuildEnvironmentsList();
        BuildFamiliesList();
        ReadXmlSettings();

        MainWindow_OnValuesValidation(this, new EventArgs());
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Shows common exception description dialog.
    /// </summary>
    /// <param name="exception">Exception instance being described.</param>
    private static void CatchException(Exception exception)
    {
      MessageBox.Show(exception.ToString(), $"Exception caught on {exception.Source}.", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Fills the list of supported project environments using environment XML files.
    /// </summary>
    private void BuildEnvironmentsList()
    {
      if (Directory.Exists(Configuration.environmentsDirectory))
      {
        foreach (var environmentXmlFile in Directory.EnumerateFiles(Configuration.environmentsDirectory, "*.xml", SearchOption.TopDirectoryOnly))
          EnvironmentsList.Items.Add(Path.GetFileNameWithoutExtension(environmentXmlFile));
      }
      else
        Directory.CreateDirectory(Configuration.environmentsDirectory);
    }

    /// <summary>
    /// Fills the list of available microcontroller families using family XML files.
    /// </summary>
    private void BuildFamiliesList()
    {
      if (Directory.Exists(Configuration.familiesDirectory))
      {
        foreach (var familyXmlFile in Directory.EnumerateFiles(Configuration.familiesDirectory, "*.xml", SearchOption.TopDirectoryOnly))
          FamiliesList.Items.Add(Path.GetFileNameWithoutExtension(familyXmlFile));
      }
      else
        Directory.CreateDirectory(Configuration.familiesDirectory);
    }

    /// <summary>
    /// Fills the list of available devices within the specified family.
    /// <param name="familyName">Name of the family.</param>
    /// </summary>
    private void BuildDevicesList(string familyName)
    {
      DevicesList.Items.Clear();

      if (familyName == "")
        return;

      var familyFileName = Path.Combine(Configuration.familiesDirectory, familyName + ".xml");
      if (File.Exists(familyFileName))
      {
        var rootNode = XDocument.Load(Path.Combine(Configuration.familiesDirectory, familyName + ".xml")).Root;
        foreach (var element in rootNode?.Elements("list") ?? new XElement[0])
        {
          if (element.Attribute("name")?.Value.Trim() != "DevicesList")
            continue;

          foreach (var deviceNode in element.Elements("add"))
          {
            var deviceName = deviceNode.Attribute("value")?.Value.Trim() ?? "";
            if (deviceName != "")
              DevicesList.Items.Add(deviceName);
          }
        }
      }

      if (DevicesList.Items.Count > 0)
        DevicesList.SelectedIndex = 0;
    }

    /// <summary>
    /// Reads saved program settings from <i>Settings.xml</i> file.
    /// </summary>
    private void ReadXmlSettings()
    {
      var configFileName = Path.Combine(Configuration.appDataDirectory, "Settings.xml");
      var selectedDevice = "";
      if (File.Exists(configFileName))
      {
        var settings = XDocument.Load(configFileName).Element("settings");
        Locm3DirectoryInput.Text = settings?.Element("locm3-directory")?.Value.Trim() ?? "";
        ProjectDirectoryInput.Text = settings?.Element("project-directory")?.Value.Trim() ?? "";
        ProjectNameInput.Text = settings?.Element("project-name")?.Value.Trim() ?? "";
        ProjectSubdirectoryCheckbox.IsChecked = settings?.Element("create-subdirectory")?.Value.Trim().ToLower() == "true";
        EnvironmentsList.SelectedItem = settings?.Element("project-environment")?.Value.Trim() ?? "";
        FamiliesList.SelectedItem = settings?.Element("selected-family")?.Value.Trim() ?? "";
        selectedDevice = settings?.Element("selected-device")?.Value.Trim() ?? "";
      }

      if (EnvironmentsList.SelectedIndex < 0 && EnvironmentsList.Items.Count > 0)
        EnvironmentsList.SelectedIndex = 0;
      if (FamiliesList.SelectedIndex < 0 && FamiliesList.Items.Count > 0)
        FamiliesList.SelectedIndex = 0;

      BuildDevicesList(FamiliesList.Text);
      DevicesList.SelectedItem = selectedDevice;
      if (DevicesList.SelectedIndex < 0 && DevicesList.Items.Count > 0)
        DevicesList.SelectedIndex = 0;
    }

    /// <summary>
    /// Writes current program settings to <i>Settings.xml</i> file.
    /// </summary>
    private void WriteXmlSettings()
    {
      if (!Directory.Exists(Configuration.appDataDirectory))
        Directory.CreateDirectory(Configuration.appDataDirectory);

      var settings = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("settings",
        new XElement("locm3-directory",     Locm3DirectoryInput.Text.Trim()),
        new XElement("project-directory",   ProjectDirectoryInput.Text.Trim()),
        new XElement("project-name",        ProjectNameInput.Text.Trim()),
        new XElement("create-subdirectory", ProjectSubdirectoryCheckbox.IsChecked.ToString()),
        new XElement("project-environment", EnvironmentsList.Text.Trim()),
        new XElement("selected-family",     FamiliesList.Text.Trim()),
        new XElement("selected-device",     DevicesList.Text.Trim())
      ));
      settings.Save(Path.Combine(Configuration.appDataDirectory, "Settings.xml"));
    }

    /// <summary>
    /// Validates the entered values.
    /// </summary>
    /// <returns><c>true</c> if validation succeeded, otherwise <c>false</c>.</returns>
    private bool ValidateValues()
    {
      return Directory.Exists(Locm3DirectoryInput.Text.Trim()) && ProjectDirectoryInput.Text.Trim().Length > 0 &&
             Regex.IsMatch(ProjectNameInput.Text.Trim(), @"^\w+$") && EnvironmentsList.SelectedIndex >= 0 && FamiliesList.SelectedIndex >= 0 &&
             DevicesList.SelectedIndex >= 0;
    }

    /// <summary>
    /// Shows libopencm3 directory selector dialog.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void Locm3DirectoryButton_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        using (var directorySelector = new FolderBrowserDialog
        {
          SelectedPath = Locm3DirectoryInput.Text.Trim(),
          Description = "Select libopencm3 directory:",
          ShowNewFolderButton = false
        })
        {
          if (directorySelector.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            Locm3DirectoryInput.Text = directorySelector.SelectedPath;
        }
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Shows project directory selector dialog.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ProjectDirectoryButton_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        using (var directorySelector = new FolderBrowserDialog
        {
          SelectedPath = ProjectDirectoryInput.Text.Trim(),
          Description = "Select project directory:",
          ShowNewFolderButton = true
        })
        {
          if (directorySelector.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            ProjectDirectoryInput.Text = directorySelector.SelectedPath;
        }
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Project name input filtering to <c>^\w+$</c> pattern.
    /// Spaces are not filtered, they are filtered by <see cref="ProjectNameInput_OnPreviewKeyDown"/> method.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ProjectNameInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      try
      {
        if (!Regex.IsMatch(e.Text, @"^\w+$"))
          e.Handled = true;
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Project name input filtering to <c>^\w+$</c> pattern.
    /// Only spaces are filtered, other characters are filtered by <see cref="ProjectNameInput_OnPreviewTextInput"/> method.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ProjectNameInput_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Space)
        e.Handled = true;
    }

    /// <summary>
    /// Devices list rebuilding on microcontroller family selection change.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void McFamilyList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        BuildDevicesList(FamiliesList.SelectedItem as string ?? "");
        MainWindow_OnValuesValidation(sender, e);
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Changes <i>IsEnabled</i> state of <see cref="GenerateButton" /> after validation of entered values.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MainWindow_OnValuesValidation(object sender, EventArgs e)
    {
      try
      {
        GenerateButton.IsEnabled = ValidateValues();
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Project generation on <see cref="GenerateButton" /> button click.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        GenerateButton.IsEnabled = false;

        // Filling general variables.
        var generator = new SourceGen.ScriptReader();
        generator.Variables.Add("ProgramDir", Configuration.programDirectory);
        generator.Variables.Add("TemplatesDir", Configuration.templatesDirectory);
        generator.Variables.Add("FamiliesDir", Configuration.familiesDirectory);
        generator.Variables.Add("EnvironmentsDir", Configuration.environmentsDirectory);
        generator.Variables.Add("LOCM3Dir", Locm3DirectoryInput.Text.Trim());
        generator.Variables.Add("ProjectDir", ProjectSubdirectoryCheckbox.IsChecked ?? false
          ? Path.Combine(ProjectDirectoryInput.Text.Trim(), ProjectNameInput.Text.Trim())
          : ProjectDirectoryInput.Text.Trim());
        generator.Variables.Add("ProjectName", ProjectNameInput.Text.Trim());
        generator.Variables.Add("DeviceName", DevicesList.Text.Trim());
        generator.Variables.Add("Date", DateTime.Now.ToShortDateString());
        generator.Variables.Add("Time", DateTime.Now.ToShortTimeString());
        generator.Variables.Add("UserName", Environment.UserName);
        generator.Variables.Add("MachineName", Environment.MachineName);

        // Reading script files.
        generator.RunScript(Path.Combine(Configuration.familiesDirectory, FamiliesList.Text.Trim() + ".xml"));
        generator.RunScript(Path.Combine(Configuration.environmentsDirectory, EnvironmentsList.Text.Trim() + ".xml"));

        // Showing generation success dialog.
        MessageBox.Show($"Project \"{generator.Variables["ProjectName"]}\" has been successfully created in \"{generator.Variables["ProjectDir"]}\".",
          Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
      finally
      {
        GC.Collect();
        GenerateButton.IsEnabled = true;
      }
    }

    /// <summary>
    /// Shows program info on <see cref="AboutLabel" /> label click.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void AboutLabel_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        MessageBox.Show("libopencm3 Project Generator v" + Configuration.version.ToString(2) + "\n" +
          "Build " + Configuration.version.Build + "." + Configuration.version.Revision + "\n\n" +
          "Copyright (c) 2018-2019 Maxim Yudin <stibiu@yandex.ru>",
          "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Saves the program settings on main window closing.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
      try
      {
        WriteXmlSettings();
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }
  }
}
