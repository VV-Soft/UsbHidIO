# UsbHidIO Sample Device Schematics
Sample schematics shows a device that has:
- one input pin connected to a button 
- one output connected to a led.

Schematic is drawn on Eagle 7.5.0

## Components

Name | Type | Description
--- | --- | ---
IC1 | Microchip PIC16F1454 | Simple inexpensive microcontroller with built in support for USB. Can sync 48 MHz internal oscillator with USB clock with only internal components.
CN1 | USB Connector | For connecting device to host
S1 | Button switch | Simple push button for providing input. Is connected to PICs input that has internal weak pullup.
L1 | Ferrite Bead 33 Ohm @ 100MHz | For suppressing high frequency noise on USB power
R1 | 1 Mohm resistor | Connects USB shield with ground
R2 | 300 ohm resistor | Current limiting resistor for led. For 2V forward Voltage led with 10mA Current.<br/>Formula: (5V - Led Forward Voltage) / Current
C1 | 0.47 µF capacitor | For stabilizing PICs internal 3.3V LDO regulator
C2 | 0.1 µF capacitor | For VDD decoupling (required for USB with internal oscillator)
C3 | 1 nF capacitor | For VDD decoupling (required for USB with internal oscillator)
LED1 | Led | Any led. Adjust R2 according to forward voltage and desired current. Max 10mA current recommended.
JP1 | Pinheader | In Circuit Serial Programming header