/*
Copyright 2017 VV-Soft Oy  (www.vvsoft.fi)

Licensed under the Apache License, Version 2.0

http://www.apache.org/licenses/LICENSE-2.0
*/

#include <xc.h>
#include <stdbool.h>
#include <buttons.h>

/*** IO Definitions ***/
#define INPUT1  PORTAbits.RA4
#define OUTPUT1 PORTCbits.RC2

/* 
 Function: void InitializeIO();
 
 Overview: Initializes IO ports to default values
 */
void InitializeIO() {
    ANSELA = 0;     //Analog module must be turned off for IO to work
                    //EVEN ON 16F1454 which does not have analog module
    WPUA = 0x10;    //Weak pull-up for RA4
    OPTION_REGbits.nWPUEN = 0; //Weak pull-up enabled
    TRISA = 0x10;   //RA4 is input
    
    TRISC = 4;      //RC2 defaults to high impedance
}

/*
 Function: void GetInputs();

 Overview: Gets values of input ports to send buffer
*/
void GetInputs(char* sendBuffer)
{
    sendBuffer[0] = (INPUT1 == 0) ? 0 : 0x80;
}

/*
 Function: void GetInputs();

 Overview: Sets values of output ports from receive buffer
*/
void SetOutputs(char* receiveBuffer)
{
    char input1 = (receiveBuffer[0] & 0xc0) >> 6;
    
    switch (input1) {
        //case 0 => no change
        case 1:
            //Z
            TRISCbits.TRISC2 = 1;   //High impedance
            break;
        case 2:
            //L
            LATCbits.LATC2 = 0;     //Low
            TRISCbits.TRISC2 = 0;   //Low impedance
            break;
        case 3:
            //H
            LATCbits.LATC2 = 1;     //High
            TRISCbits.TRISC2 = 0;   //Low impedance
            break;
    }
}

