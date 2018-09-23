# LOCM3Gen

<b>LOCM3Gen</b> is a project generator for <b>libopencm3</b> open-source library.

# Description

<b>LOCM3Gen</b>'s primary task is to create concise self-sufficient microcontroller firmware projects based on the <b>libopencm3</b> library. It is written on C# and has standard Windows Forms graphical user interface to maintain the generation process. Through configuring project settings like library's directory, project location, microcontroller family, etc. the empty but ready-to-compile firmware project will be generated.

# Features

As for now the only supported target environment is <b>EmBitz</b> IDE.

The generator supports <b>STM32</b> microcontrollers of listed families:
* STM32F0
* STM32F1
* STM32F2
* STM32F3
* STM32F4
* STM32F7
* STM32L0
* STM32L1
* STM32L4

The generator is customizable through the XML files and templates that contain neccessary information to correctly build the project. Through such XML files available support for microcontroller families and environments can be extended. Also it features almost all needed description <i>.svd</i> files for firmware debugging.

# Links

* <b>libopencm3</b> library: https://github.com/libopencm3
* <b>EmBitz</b> IDE: https://www.embitz.org/