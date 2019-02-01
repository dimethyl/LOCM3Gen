using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using LOCM3Gen.SourceGen;

namespace LOCM3Gen
{
  /// <summary>
  /// Project generator model class.
  /// </summary>
  public class GeneratorModel : IValidatableObject, INotifyPropertyChanged
  {
    /// <summary>
    /// libopencm3 directory path.
    /// </summary>
    private string _locm3Directory = "";

    /// <summary>
    /// Project directory path.
    /// </summary>
    private string _projectDirectory = "";

    /// <summary>
    /// Project name.
    /// </summary>
    private string _projectName = "";

    /// <summary>
    /// Project subdirectory creation flag.
    /// </summary>
    private bool _createProjectSubdirectory = true;

    /// <summary>
    /// Target environment name.
    /// </summary>
    private string _environmentName = "";

    /// <summary>
    /// Microcontroller family name.
    /// </summary>
    private string _familyName = "";

    /// <summary>
    /// Microcontroller device name.
    /// </summary>
    private string _deviceName = "";

    /// <summary>
    /// libopencm3 directory path binding property.
    /// </summary>
    public string Locm3Directory
    {
      get => _locm3Directory;
      set
      {
        _locm3Directory = value.Trim();
        OnPropertyChanged(nameof(Locm3Directory));
      }
    }

    /// <summary>
    /// Project directory path binding property.
    /// </summary>
    public string ProjectDirectory
    {
      get => _projectDirectory;
      set
      {
        _projectDirectory = value.Trim();
        OnPropertyChanged(nameof(ProjectDirectory));
      }
    }

    /// <summary>
    /// Project name binding property.
    /// </summary>
    public string ProjectName
    {
      get => _projectName;
      set
      {
        _projectName = Regex.Replace(value, @"\W", "");
        OnPropertyChanged(nameof(ProjectName));
      }
    }

    /// <summary>
    /// Binding property for project subdirectory creation flag.
    /// </summary>
    public bool CreateProjectSubdirectory
    {
      get => _createProjectSubdirectory;
      set
      {
        _createProjectSubdirectory = value;
        OnPropertyChanged(nameof(CreateProjectSubdirectory));
      }
    }

    /// <summary>
    /// Environment name binding property.
    /// </summary>
    public string EnvironmentName
    {
      get => _environmentName;
      set
      {
        _environmentName = value.Trim();
        OnPropertyChanged(nameof(EnvironmentName));
      }
    }

    /// <summary>
    /// Microcontroller family name binding property.
    /// </summary>
    public string FamilyName
    {
      get => _familyName;
      set
      {
        _familyName = value.Trim();
        OnPropertyChanged(nameof(FamilyName));
      }
    }

    /// <summary>
    /// Microcontroller device name binding property.
    /// </summary>
    public string DeviceName
    {
      get => _deviceName;
      set
      {
        _deviceName = value.Trim();
        OnPropertyChanged(nameof(DeviceName));
      }
    }

    /// <summary>
    /// Reads saved generator model data from <i>Settings.xml</i> file.
    /// </summary>
    public void ReadXmlSettings()
    {
      var configFileName = Path.Combine(Configuration.AppDataDirectory, "Settings.xml");
      if (!File.Exists(configFileName))
        return;

      var settings = XDocument.Load(configFileName).Element("settings");
      Locm3Directory = settings?.Element("locm3-directory")?.Value.Trim() ?? "";
      ProjectDirectory = settings?.Element("project-directory")?.Value.Trim() ?? "";
      ProjectName = settings?.Element("project-name")?.Value.Trim() ?? "";
      CreateProjectSubdirectory = settings?.Element("create-subdirectory")?.Value.Trim().ToLower() == "true";
      EnvironmentName = settings?.Element("project-environment")?.Value.Trim() ?? "";
      FamilyName = settings?.Element("selected-family")?.Value.Trim() ?? "";
      DeviceName = settings?.Element("selected-device")?.Value.Trim() ?? "";
    }

    /// <summary>
    /// Writes current generator model data to <i>Settings.xml</i> file.
    /// </summary>
    public void WriteXmlSettings()
    {
      if (!Directory.Exists(Configuration.AppDataDirectory))
        Directory.CreateDirectory(Configuration.AppDataDirectory);

      var settings = new XDocument(new XDeclaration("1.0", "utf-8", null),
        new XElement("settings",
          new XElement("locm3-directory", Locm3Directory),
          new XElement("project-directory", ProjectDirectory),
          new XElement("project-name", ProjectName),
          new XElement("create-subdirectory", CreateProjectSubdirectory.ToString().ToLower()),
          new XElement("project-environment", EnvironmentName),
          new XElement("selected-family", FamilyName),
          new XElement("selected-device", DeviceName)
        ));

      settings.Save(Path.Combine(Configuration.AppDataDirectory, "Settings.xml"));
    }

    /// <summary>
    /// Model validation method. Provides <see cref="Validator" />-compatible object validation logic.
    /// </summary>
    /// <param name="validationContext">Validation context instance.</param>
    /// <returns>Collection of model validation errors.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      var errors = new Collection<ValidationResult>();

      if (!Directory.Exists(Locm3Directory))
        errors.Add(new ValidationResult("libopencm3 directory does not exist."));

      if (string.IsNullOrWhiteSpace(ProjectDirectory))
        errors.Add(new ValidationResult("Project directory path cannot be empty."));

      if (!Regex.IsMatch(ProjectName, @"^\w+$"))
        errors.Add(new ValidationResult("Project name may only contain alphanumeric characters and underscores."));

      if (!File.Exists(Path.Combine(Configuration.FamiliesDirectory, $"{FamilyName}.xml")))
        errors.Add(new ValidationResult("Unknown microcontroller family specified."));

      if (!File.Exists(Path.Combine(Configuration.EnvironmentsDirectory, $"{EnvironmentName}.xml")))
        errors.Add(new ValidationResult("Unknown target environment specified."));

      return errors;
    }

    /// <summary>
    /// Determines whether generator model is valid or not.
    /// </summary>
    public bool IsValid => !Validate(new ValidationContext(this)).Any();

    /// <summary>
    /// Generates a project using provided generator model values.
    /// It defines general variables and runs the necessary family and environment script files.
    /// </summary>
    /// <returns></returns>
    public void GenerateProject()
    {
      // Validating the model.
      if (!IsValid)
        return;

      // Filling general variables.
      var projectData = new ScriptDataContext();
      projectData.Variables.Add("ProgramDir", Configuration.ProgramDirectory);
      projectData.Variables.Add("TemplatesDir", Configuration.TemplatesDirectory);
      projectData.Variables.Add("FamiliesDir", Configuration.FamiliesDirectory);
      projectData.Variables.Add("EnvironmentsDir", Configuration.EnvironmentsDirectory);
      projectData.Variables.Add("LOCM3Dir", Locm3Directory);
      projectData.Variables.Add("ProjectDir", CreateProjectSubdirectory ? Path.Combine(ProjectDirectory, ProjectName) : ProjectDirectory);
      projectData.Variables.Add("ProjectName", ProjectName);
      projectData.Variables.Add("EnvironmentName", EnvironmentName);
      projectData.Variables.Add("FamilyName", FamilyName);
      projectData.Variables.Add("DeviceName", DeviceName);
      projectData.Variables.Add("Date", DateTime.Now.ToShortDateString());
      projectData.Variables.Add("Time", DateTime.Now.ToShortTimeString());
      projectData.Variables.Add("UserName", Environment.UserName);
      projectData.Variables.Add("MachineName", Environment.MachineName);

      // Running family and environment script files.
      ScriptReader.RunScript(Path.Combine(Configuration.FamiliesDirectory, $"{FamilyName}.xml"), projectData);
      ScriptReader.RunScript(Path.Combine(Configuration.EnvironmentsDirectory, $"{EnvironmentName}.xml"), projectData);
    }

    /// <summary>
    /// Binding property change event.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Binding property change callback.
    /// </summary>
    /// <param name="propertyName">
    /// Name of the binding property being changed.
    /// The caller member name will be passed internally if value is omitted.
    /// </param>
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
