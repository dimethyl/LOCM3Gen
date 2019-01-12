using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
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
  /// Main window view model for <i>MainWindow.xaml</i> file.
  /// </summary>
  public partial class MainWindow
  {
    /// <summary>
    /// Project generator model instance.
    /// </summary>
    private readonly GeneratorModel _generator;

    /// <summary>
    /// Main window constructor.
    /// </summary>
    public MainWindow()
    {
      try
      {
        // Initializing window with compiled XAML code.
        InitializeComponent();

        // Getting generator model instance defined in XAML resources.
        _generator = (GeneratorModel) Resources["Generator"];

        // Filling window with data.
        _generator.ReadXmlSettings();
        BuildEnvironmentsList();
        BuildFamiliesList();
        BuildDevicesList(_generator.FamilyName);

        // Adding a callback for rebuilding of the devices list on microcontroller family change.
        FamiliesList.SelectionChanged += FamiliesList_OnSelectionChanged;
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
      if (Directory.Exists(Configuration.EnvironmentsDirectory))
      {
        foreach (var environmentXmlFile in Directory.EnumerateFiles(Configuration.EnvironmentsDirectory, "*.xml", SearchOption.TopDirectoryOnly))
          EnvironmentsList.Items.Add(Path.GetFileNameWithoutExtension(environmentXmlFile));
      }
      else
        Directory.CreateDirectory(Configuration.EnvironmentsDirectory);

      if (EnvironmentsList.Items.Contains(_generator.EnvironmentName))
        EnvironmentsList.SelectedItem = _generator.EnvironmentName;
      else
        EnvironmentsList.SelectedIndex = 0;
    }

    /// <summary>
    /// Fills the list of available microcontroller families using family XML files.
    /// </summary>
    private void BuildFamiliesList()
    {
      if (Directory.Exists(Configuration.FamiliesDirectory))
      {
        foreach (var familyXmlFile in Directory.EnumerateFiles(Configuration.FamiliesDirectory, "*.xml", SearchOption.TopDirectoryOnly))
          FamiliesList.Items.Add(Path.GetFileNameWithoutExtension(familyXmlFile));
      }
      else
        Directory.CreateDirectory(Configuration.FamiliesDirectory);

      if (FamiliesList.Items.Contains(_generator.FamilyName))
        FamiliesList.SelectedItem = _generator.FamilyName;
      else
        FamiliesList.SelectedIndex = 0;
    }

    /// <summary>
    /// Fills the list of available devices within the specified family.
    /// <param name="familyName">Name of the family.</param>
    /// </summary>
    private void BuildDevicesList(string familyName)
    {
      DevicesList.Items.Clear();

      if (string.IsNullOrWhiteSpace(familyName))
        return;

      var familyFileName = Path.Combine(Configuration.FamiliesDirectory, familyName + ".xml");
      if (File.Exists(familyFileName))
      {
        var rootNode = XDocument.Load(Path.Combine(Configuration.FamiliesDirectory, familyName + ".xml")).Root;
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

      if (DevicesList.Items.Contains(_generator.DeviceName))
        DevicesList.SelectedItem = _generator.DeviceName;
      else
        DevicesList.SelectedIndex = 0;
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
    /// Rebuilds the devices list on microcontroller family selection change.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void FamiliesList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        BuildDevicesList(FamiliesList.SelectedItem as string ?? "");
      }
      catch(Exception exception)
      {
        CatchException(exception);
      }
    }

    /// <summary>
    /// Project generation on <see cref="GenerationButton" /> button click.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void GenerationButton_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        GenerationButton.IsEnabled = false;

        // Validating generator model.
        if (_generator.IsValid)
        {
          _generator.GenerateProject();

          MessageBox.Show($"Project \"{_generator.ProjectName}\" has been successfully created in \"{_generator.ProjectDirectory}\".",
            Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          var errorMessage = new StringBuilder("Cannot generate a project:");
          foreach (var error in _generator.Validate(new ValidationContext(_generator)))
            errorMessage.Append($"\n - {error.ErrorMessage}");

          MessageBox.Show(errorMessage.ToString(), Title,MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
      finally
      {
        GC.Collect();
        GenerationButton.IsEnabled = true;
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
        MessageBox.Show("libopencm3 Project Generator v" + Configuration.Version.ToString(2) + "\n" +
          "Build " + Configuration.Version.Build + "." + Configuration.Version.Revision + "\n\n" +
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
        _generator.WriteXmlSettings();
      }
      catch (Exception exception)
      {
        CatchException(exception);
      }
    }
  }
}
