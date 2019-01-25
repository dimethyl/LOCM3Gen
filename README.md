# LOCM3Gen

**LOCM3Gen** is a project generator for **libopencm3** open-source
library.

Description
-----------

**LOCM3Gen**'s primary task is to create compact self-sufficient
microcontroller firmware projects based on the **libopencm3** library.

It is written on C# and has graphical user interface for convenience. Through
configuring project settings like library's directory, project location,
microcontroller family, etc. the empty but ready-to-compile firmware project
will be generated.

Features
--------

The generator supports **STM32** microcontrollers of listed families:

* STM32F0
* STM32F1
* STM32F2
* STM32F3
* STM32F4
* STM32F7
* STM32L0
* STM32L1
* STM32L4

Supported target environments for projects:

* **EmBitz** IDE
* None (only necessary files are created)

The generator is customizable through special SourceGen scripts and templates
that contain all the data for successful project creation. Through them the
available support for microcontroller families and target environments can be
extended. Also the generator includes the packages of hardware description
*.svd* files necessary for firmware debugging.

System requirements
-------------------

To function program requires:

* **Windows 7 SP1** or later
* **.NET Framework 4.6.2** installed
* The last build of **libopencm3** library installed and compiled

Downloads
---------

You can download the source code and compiled packages
[here](https://github.com/Egiraht/LOCM3Gen/releases).

License
-------

Copyright (C) 2018-2019 Maxim Yudin <<stibiu@yandex.ru>>

Program's license statement can be found in [LICENSE.txt](LICENSE.txt).

Links
-----

* **LOCM3Gen** project GitHub page: https://github.com/Egiraht/LOCM3Gen
* **libopencm3** library: <https://github.com/libopencm3/libopencm3>
* **EmBitz** IDE: <https://www.embitz.org/>
