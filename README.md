# UsbHidIO
VV-Soft Usb Hid IO aims to offer a versatile protocol for creating multitude of I/O devices with a single VID/PID number (0x1209 / 0x2345).

## Design goals
 - Simplicity
 - Versatility
 - Expandability
 
## Features
 - [Simple protocol](PROTOCOL.md)
 - Possibility to use 0-255 inputs and 0-255 outputs
 - Device capabilities easily discoverable by software on host
 
## License
 - [Apache license v 2.0](LICENSE)
 - Project uses libraries under different license (not included in repository)

## What's included
- [Protocol definition](PROTOCOL.md)
- [Sample device firmware](Device/Example/PicFirmware) using Microchip PIC16F1454 microcontroller
- [Sample device schematics](Device/Example/Schematics) using Microchip PIC16F1454 microcontroller
- [Sample control application](Host/CsSample) for host in C#