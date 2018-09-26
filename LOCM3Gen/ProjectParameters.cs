/*
 * Copyright (C) 2018 Maxim Yudin <i@hal.su>. All rights reserved.
 * 
 * This file is a part of the closed source section of LOCM3Gen project.
 * You may NOT use, distribute, copy or modify this file without special author's permission.
 */

namespace LOCM3Gen
{
  /// <summary>
  /// Class for storing project variables used during generation process.
  /// </summary>
  public class ProjectParameters
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
    /// Target device name within the target microcontroller family.
    /// </summary>
    public string deviceName;
  }
}
