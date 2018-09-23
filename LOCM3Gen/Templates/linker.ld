/* 
* Family:   {$FamilyName$}
* Device:   {$DeviceName$}
* ROM Size: {$ROMSize$} bytes
* RAM Size: {$RAMSize$} bytes
*/

MEMORY
{
  rom (rx) :  ORIGIN = 0x08000000, LENGTH = {$ROMSize$}
  ram (rwx) : ORIGIN = 0x20000000, LENGTH = {$RAMSize$}
}

INCLUDE libopencm3/cortex-m-generic.ld
