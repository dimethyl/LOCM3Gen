# LOCM3Gen

**LOCM3Gen** is a GUI project generator for **libopencm3** open-source
library.

## Description

**LOCM3Gen**'s primary task is to create compact self-sufficient
microcontroller firmware projects based on the **libopencm3** library.

It is written on C# and has graphical user interface for convenience. Through
configuring project settings like library's directory, project location,
microcontroller family, etc. the empty but ready-to-compile firmware project
will be generated.

## Features

The generator supports **STM32** microcontrollers of listed families:

* STM32F0
* STM32F1
* STM32F2
* STM32F3
* STM32F4
* STM32F7
* STM32G0
* STM32L0
* STM32L1
* STM32L4

Supported target environments for projects:

* **CLion** IDE (2019.2 or higher)
* **EmBitz** IDE
* None (only necessary files are generated)

The generator is customizable through special SourceGen scripts and templates
that contain all the data for successful project creation. Through them the
available support for microcontroller families and target environments can be
extended. Also the generator includes the packages of hardware description
*.svd* files necessary for firmware debugging.

## System requirements

To function program requires:

* **Windows 7 SP1** or later
* **.NET Framework 4.6.2** installed
* The last build of **libopencm3** library installed and compiled

When using **CLion** IDE, the latest versions of **MinGW**, **ARM GCC Toolchain**
and **OpenOCD** packages also need to be installed and configured.
After installation of **ARM GCC Toolchain** make sure that its `/bin` directory
is added to the system's `PATH` environment variable.

## Project generation

After a new **libopencm3** project was configured and generated, all necessary
project files are put inside the specified project directory.

#### CLion
If the project targets **CLion**, after project generation and opening
call `File > Reload CMake Project` so that the IDE updates CMake cache.

For firmware debugging also make sure you are using the ARM GDB debugger
provided with **ARM GCC Toolchain**, not the one from **MinGW**
(check `File > Settings > Build, Execution, Deployment > Toolchains > Debugger`).
If there is an *.svd* file for the device, it will be automatically loaded
but no registers will be visible by default during the debugging session.
To filter them click the `Configure` button on the side panel located on
the `Peripherals` tab of the `Debug` window.

#### EmBitz

If the project targets **EmBitz**, usually no specific actions need
to be performed.

## Documentation

Software documentation is available in the project's [wiki](https://github.com/Egiraht/LOCM3Gen/wiki).

## Downloads

You can download the source code and compiled packages
[here](https://github.com/Egiraht/LOCM3Gen/releases).

## License

Copyright (C) 2018-2019 Maxim Yudin <<stibiu@yandex.ru>>

Program's license statement can be found in [LICENSE.txt](LICENSE.txt).

## Links

* **LOCM3Gen** project GitHub page: <https://github.com/Egiraht/LOCM3Gen>
* **libopencm3** library: <https://github.com/libopencm3/libopencm3>
* **CLion**: <https://www.jetbrains.com/clion/>
* **CMake**: <https://cmake.org/>
* **MinGW**: <http://www.mingw.org/>
* **ARM GCC Toolchain**: <https://developer.arm.com/tools-and-software/open-source-software/developer-tools/gnu-toolchain/gnu-rm>
* **OpenOCD**: <http://openocd.org/>
* **EmBitz**: <https://www.embitz.org/>
