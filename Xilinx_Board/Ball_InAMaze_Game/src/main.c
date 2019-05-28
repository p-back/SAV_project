/*
 * main.c
 *
 *  Created on: 28.04.2019
 *      Author: Nico
 */

// ----------------------------------------------
//		INCLUES
#include <stdbool.h>
#include "sleep.h"
#include "xuartps_hw.h"		// UART
#include "xil_printf.h"
#include "GPIO.h"
#include "LIS2DS12.h"

// ----------------------------------------------
// 		DEFINES

// 2 Hz desired counter frequency requires 4 Hz
// timer frequency
#define LED_COUNTER_FEQU 4

// to be used to send one byte via UART1
#define UART_SEND_BYTE(byte)	XUartPs_SendByte(XPAR_PS7_UART_1_BASEADDR, byte)

// to be used to BLOCK wait for UART data
#define UART_RECV_BYTE()		XUartPs_RecvByte(XPAR_PS7_UART_1_BASEADDR)

// to be used to clear RECV buffer of UART
#define UART_CLEAR_RECV_BUF()	XUartPs_WriteReg(XPAR_PS7_UART_1_BASEADDR, XUARTPS_CR_OFFSET, (u32)XUARTPS_CR_RXRST);

// START condition to be recieved from APPLICATION
#define START_CONDITION			0xAFFE

// RETURN VALUEs
#define SUCCESS					 0
#define FAILURE					-1

// ----------------------------------------------
// Internal FUNCTION Declaration
static void SendData_UART(void * data, uint32_t len);
static int StartUp_Phase();

// ----------------------------------------------
// 		MAIN FUNCTION
int main()
{
	// Pointer to Motion values struct
	LIS2DS12_Motion_t * motion;

	// Init GPIO (PS LEDs and PL LEDs)
    GPIO_Init();

    // Init LIS2DS12 Sensor
    LIS2DS12_Init();

START:
	// Discard all DATA in RECEIVE BUFFER
    UART_CLEAR_RECV_BUF();

    // STARTUP PHASE --> ***BLOCKING*** until start condition is received
    if(StartUp_Phase() != SUCCESS)
    {
    	// While USER has NOT pressed RESET button --> wake up every 100 ms and check
		while(!GPIO_Get_PS_Button())
		{
			// Send data every 100 ms (10 times in a second)
			usleep(100000);
		}

		// Then go back to START
		goto START;
    }

    // Prevent PROGRAM from EXITING
    while(1)
    {
    	if (GPIO_Get_PS_Button())
    	{
    		goto START;
    	}

    	// Check if Application sent command ...
    	if(XUartPs_IsReceiveData(XPAR_PS7_UART_1_BASEADDR))
    	{
    		// DECODE COMMAND
    		switch(UART_RECV_BYTE())
    		{
    		case 0x01:
    			// STOP COMMAND --> go to START
    			goto START;
    		case 0x02:
    			break;
    		default:
    			// Unknown COMMAND
    			break;
    		}
    	}

		// Read and Print Motion values
    	motion = LIS2DS12_Get_Motion();

    	// Send actual MOTION data
    	SendData_UART((void*)motion, sizeof(*motion));

    	// Send data every 100 ms (10 times in a second)
    	usleep(100000);
    }

    return SUCCESS;
}

// ----------------------------------------------
// Private FUNCTION Definitions

// Board waits to receive START condition from APPLICATION
// then sends ACK! --> SETS LEDs
static int StartUp_Phase()
{
	u16 const start_condition = START_CONDITION;

	// Let PL LED signalize that we are NOT connected to APPLICATION SW
	GPIO_Set_PL_Color(LED_COLOR_RED);

	// Wait for START signal from APPLICATION
	u16 start 	 = 0;
	start 		 = UART_RECV_BYTE() << 8;
	start 		|= UART_RECV_BYTE();

	// Check if we received the CORRECT start condition
	if (memcmp(&start, &start_condition, sizeof(start_condition)) != 0)
	{
		// and ERROR has occured --> turn PL LED orange
		GPIO_Set_PL_Color(LED_COLOR_AMBER);
		return FAILURE;
	}

	// If we received the CORRECT start condition --> turn PL LED green
	GPIO_Set_PL_Color(LED_COLOR_GREEN);

	// AND send ACK back
	UART_SEND_BYTE(0xAF);
	UART_SEND_BYTE(0xFE);

	return SUCCESS;
}

// Send each byte of DATA using UART1
static void SendData_UART(void * data, uint32_t len)
{
	for (uint32_t i = 0; i < len; i++)
	{
		UART_SEND_BYTE(((char)*((char*)data+i)));
	}
}

