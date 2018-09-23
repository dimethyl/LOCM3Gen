using System;
using System.Collections.Generic;

namespace LOCM3Gen
{
  /// <summary>
  /// Class for storing the route information for files copying during project generation.
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
}
