/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/ .

 * Copyright (C) 2018-2019 Maxim Yudin <stibiu@yandex.ru>.
 */

using System;
using System.IO;
using System.Reflection;

namespace LOCM3Gen
{
  /// <summary>
  /// Class for storing program configuration constants.
  /// </summary>
  static class Configuration
  {
    /// <summary>
    /// Application version object.
    /// </summary>
    public static readonly Version version = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version;

    /// <summary>
    /// Path of the program root directory.
    /// </summary>
    public static readonly string programDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// Path of the application data directory.
    /// </summary>
    public static readonly string appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LOCM3Gen");

    /// <summary>
    /// Path of the <i>Environments</i> directory, containing project environments XML files.
    /// </summary>
    public static readonly string environmentsDirectory = Path.Combine(programDirectory, "Environments");

    /// <summary>
    /// Path of the <i>Families</i> directory, containing microcontroller family XML files.
    /// </summary>
    public static readonly string familiesDirectory = Path.Combine(programDirectory, "Families");

    /// <summary>
    /// Path of the <i>Templates</i> directory, containing template files.
    /// </summary>
    public static readonly string templatesDirectory = Path.Combine(programDirectory, "Templates");
  }
}
