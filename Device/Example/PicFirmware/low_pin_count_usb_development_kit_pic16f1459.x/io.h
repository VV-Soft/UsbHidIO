/*
Copyright 2017 VV-Soft Oy  (www.vvsoft.fi)

Licensed under the Apache License, Version 2.0

http://www.apache.org/licenses/LICENSE-2.0
*/

#include <stdbool.h>

#ifndef IO_H
#define IO_H

/* 
 Function: void InitializeIO();
 
 Overview: Initializes IO ports to default values
 */
void InitializeIO();

/*
 Function: void GetInputs();

 Overview: Gets values of input ports to send buffer
*/
void GetInputs(char* sendBuffer);

/*
 Function: void GetInputs();

 Overview: Sets values of output ports from receive buffer
*/
void SetOutputs(char* receiveBuffer);


#endif //IO_H
