# Protocol

Device is operated in HID mode by sending a request and reading a response.

Requests and responses are basically messages in a form of byte arrays. Device specifies a fixed length for such arrays, and thus remaining bytes in messages shorter than maximum length will be ignored.

Request are prefixed by a request type (1 byte). Response is prefixed with the same byte.

(All examples are in hex)

## Request types
 - 1: Get capabilities
 - 2: Get input values
 - 3: Set output values
 
### Get capabilities (request type 1)

The idea of providing this capability information to host is to allow making of generic control program. It tells how many indicators for inputs and how many buttons (or the like) for outputs should be displayed and what selections for outputs are available.

There are things like high/low voltage levels, which pins/pad is which input/output number etc, that are not relevant to software. Each implementation of the device should have that information available for the person connecting the peripherals.

#### Request:

| Pos | Value | Description  |
| --- | --- | --- |
|  0 | 1 | Fixed request type |

Example: `01`

#### Response:

| Pos | Value | Description  |
| --- | --- | --- |
| 0 | 1 | Fixed request type |
| 1 | 0-255 | Amount of inputs |
| 2 | 0-255 | Amount of outputs (assumed 0 if msg len < 3) |
| 3 | 0-3 | All outputs are this [type](#outputtype) (assumed 0 if msg len < 4) |
| | 4 | Output [type](#outputtype) is specified for each output starting from pos 4 |
| 4 - (n+3) | 0-255 | Output [types](#outputtype), 4 types per byte, first type in MSBs |

n = (amount of outputs+3)/4, where "/" denotes integer division

#### Bit designations for <a name="outputtype"></a>output types:

| Bits | Dec | Supported modes |
| --- | --- |  --- |
| 00 | 0 | H + L (low impedance only) |
| 01 | 1 | H + L + Z (tristate support) |
| 10 | 2 | L + Z (⎐ open collector output to low/gnd ) |
| 11 | 3 | H + Z (⎏ open collector output to high/vcc ) |

*H = High, L = Low, Z = Floating / High impedance*

Example: `01 01 02 00` => This device has 1 input and 2 outputs, outputs support H + L + Z

Example: `01 01` => This device has 1 input and 0 outputs

### Get input values (request type 2)

#### Request: 

| Pos | Value | Description  |
| --- | --- | --- |
| 0 | 2 | Fixed request type |

Example: `02`

#### Response: 

| Pos | Value | Description  |
| --- | --- | --- |
| 0 | 2 | Fixed request type |
| 1-n | 0-255 | Input values, 8 inputs per byte, first input in MSB |

n = (amount of inputs+7)/8, where "/" denotes integer division

Example: `02 80` => Input 1 is high (rest of the inputs are low)

Example: `02 00` => All inputs are low

Example: `02 28` => Inputs 3 and 5 are high (rest are low)

### Set output values (request type 3)

Each output value is given with two bits:

| Bits | Meaning |
| --- | --- |
| 00 | No change. Output state should remain |
| 01 | Output is set to high impedance state |
| 10 | Output is set to low |
| 11 | Output is set to high |

#### Mode interpretation

Requested mode should be interpreted as follows per output type

| Output [type](#outputtype) | Description | 01 | 10 | 11 |
| --- | --- | --- | --- | --- |
| 0 | H + L (low impedance only) | No change | L | H |
| 1 | H + L + Z (tristate support) | Z | L | H |
| 2 | L + Z (⎐ open collector output to low/gnd ) | Z | L | Z |
| 3 | H + Z (⎏ open collector output to high/vcc ) | Z | Z | H |

*H = High, L = Low, Z = Floating / High impedance*

#### Request: 

| Pos | Value | Description  |
| --- | --- | --- |
| 0 | 3 | Fixed request type |
| 1-n | 0-255 | Output values, 4 inputs per byte, first input in MSBs |

n = (amount of outputs+3)/4, where "/" denotes integer division

Example: `03 36` => (0x36 = 00 11 01 10 ) No change on 1st output, 2nd output high, 3rd output to high imp., 4th output low

#### Response: 

| Pos | Value | Description  |
| --- | --- | --- |
| 0 | 3 | Fixed request type |

Example: `03` => Request was processed

