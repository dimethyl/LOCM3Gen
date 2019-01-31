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
using System.Windows.Threading;
using System.Xml.Linq;
using LOCM3Gen.SourceGen;
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
    public GeneratorModel Generator { get; } = new GeneratorModel();

    /// <summary>
    /// Main window constructor.
    /// </summary>
    public MainWindow()
    {
      // Initializing window with compiled XAML code.
      InitializeComponent();

      // Setting default exception handler.
      Dispatcher.UnhandledException += Dispatcher_OnUnhandledException;

      // Filling window with data.
      Generator.ReadXmlSettings();
      BuildEnvironmentsList();
      BuildFamiliesList();
      BuildDevicesList(Generator.FamilyName);

      // Adding a callback for rebuilding of the devices list on microcontroller family change.
      FamiliesList.SelectionChanged += FamiliesList_OnSelectionChanged;
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

      if (EnvironmentsList.Items.Contains(Generator.EnvironmentName))
        EnvironmentsList.SelectedItem = Generator.EnvironmentName;
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

      if (FamiliesList.Items.Contains(Generator.FamilyName))
        FamiliesList.SelectedItem = Generator.FamilyName;
      else
        FamiliesList.SelectedIndex = 0;
    }

    /// <summary>
    /// Fills the list of available devices within the specified family.
    /// </summary>
    /// <param name="familyName">Name of the family.</param>
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

      if (DevicesList.Items.Contains(Generator.DeviceName))
        DevicesList.SelectedItem = Generator.DeviceName;
      else
        DevicesList.SelectedIndex = 0;
    }

    /// <summary>
    /// Default exception handling routine.
    /// Shows common exception description dialog.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void Dispatcher_OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      e.Handled = true;

      if (e.Exception is ScriptException scriptError)
        MessageBox.Show(scriptError.Message, "Script error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      else
        MessageBox.Show($"[{e.GetType().Name}] {e.Exception.Message}", $"Unhandled exception on {e.Exception.Source}", MessageBoxButtons.OK,
          MessageBoxIcon.Error);
    }

    /// <summary>
    /// Shows libopencm3 directory selector dialog.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void Locm3DirectoryButton_OnClick(object sender, RoutedEventArgs e)
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

    /// <summary>
    /// Shows project directory selector dialog.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ProjectDirectoryButton_OnClick(object sender, RoutedEventArgs e)
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

    /// <summary>
    /// Project name input filtering to <c>^\w+$</c> pattern.
    /// Spaces are not filtered, they are filtered by <see cref="ProjectNameInput_OnPreviewKeyDown"/> method.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ProjectNameInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      if (!Regex.IsMatch(e.Text, @"^\w+$"))
        e.Handled = true;
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
      BuildDevicesList(FamiliesList.SelectedItem as string ?? "");
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
        if (Generator.IsValid)
        {
          Generator.GenerateProject();

          MessageBox.Show($"Project \"{Generator.ProjectName}\" has been successfully created in \"{Generator.ProjectDirectory}\".",
            Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          var errorMessage = new StringBuilder("Cannot generate a project:");
          foreach (var error in Generator.Validate(new ValidationContext(Generator)))
            errorMessage.Append($"\n - {error.ErrorMessage}");

          MessageBox.Show(errorMessage.ToString(), Title,MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
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
      MessageBox.Show(
        $"LOCM3Gen v{Configuration.Version.ToString(2)} Build {Configuration.Version.Build}\n" +
        "libopencm3 Project Generator\n\n" +
        "Copyright (c) 2018-2019 Maxim Yudin <stibiu@yandex.ru>", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Saves the program settings on main window closing.
    /// </summary>
    /// <param name="sender">Event sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
      Generator.WriteXmlSettings();
    }
  }
}
