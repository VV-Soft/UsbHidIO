# UsbHidIO Sample Device Firmware for PIC16F1454

Sample firmware for a device that has:
- one input pin (RA4) 
- one output pin (RC2)

Sample is based on Microchip MLA library (v 2016_11_07) example: USB device hid_custom.

[Microchip MLA library](http://www.microchip.com/mplab/microchip-libraries-for-applications) is required for compiling the project.

## Compiling

1. Download and install [Microchip MLA library](http://www.microchip.com/mplab/microchip-libraries-for-applications).
2. If your copy of MLA library is not installed in `C:/microchip/mla/v2016_11_07` update references in `low_pin_count_usb_development_kit_pic16f1459.x\nbproject\configurations.xml` accordingly.
3. Open project in MPLAB X ide.
4. Compile normally.



