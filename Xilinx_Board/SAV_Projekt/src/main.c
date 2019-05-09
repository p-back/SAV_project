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
#include "Private_TIMER.h"
#include "LIS2DS12.h"

// ----------------------------------------------
// 		DEFINES

// 2 Hz desired counter frequency requires 4 Hz
// timer frequency
#define LED_COUNTER_FEQU 4

// to be used to send one byte via UART1
#define UART_SEND_BYTE(byte)	XUartPs_SendByte(STDOUT_BASEADDRESS, byte)

// ----------------------------------------------
// Internal FUNCTION Declaration
static void Timer_Expired();
static void SendData_UART(void * data, uint32_t len);

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

    // Init TIMER (for LED counter)
    if(Private_TIMER_Init(LED_COUNTER_FEQU, (Private_TIMER_CB)Timer_Expired) != XST_SUCCESS)
    {
    	xil_printf("ERROR: Private Timer Init!");
    	return -1;
    }

    // Start Timer
    Private_TIMER_Start();

    // Prevent PROGRAM from EXITING
    while(1)
    {
		// Read and Print Motion values
    	motion = LIS2DS12_Get_Motion();

    	xil_printf("Acceleration = X: %5d, Y: %5d, Z: %5d\r\n", motion->Accel_X, motion->Accel_Y, motion->Accel_Z);

    	// Send recognition BYTES
    	UART_SEND_BYTE(0xFF);
    	UART_SEND_BYTE(0xFF);

    	// Send actual MOTION data
    	SendData_UART((void*)motion, sizeof(*motion));

    	// Send data every 100 ms (10 times in a second)
    	usleep(100000);
    }

    // End of MAIN
    Private_TIMER_DeInit();

    return 0;
}

// ----------------------------------------------
// Private FUNCTION Definitions

// Function that is called when timer is expired
static void Timer_Expired()
{
	// Implement 4 bit binary counter with PS (red, green and amber)

	// use this shift register to realise bit counter
	static uint8_t bit_counter = 0;

	// PL LED (Bit 0 and 1)
	if ((bit_counter & 0x03) == 0x03) // 0000.0011
		GPIO_Set_PL_Color(LED_COLOR_AMBER);
	else if ((bit_counter & (1 << 0)))
		GPIO_Set_PL_Color(LED_COLOR_GREEN);
	else if ((bit_counter & (1 << 1)))
		GPIO_Set_PL_Color(LED_COLOR_RED);
	else
		GPIO_Set_PL_Color(LED_COLOR_OFF);

	// PS LED (Bit 2 and 3)
	if ((bit_counter & 0x0C) == 0x0C) // 0000.1100
		GPIO_Set_PS_Color(LED_COLOR_AMBER);
	else if ((bit_counter & (1 << 2)))
		GPIO_Set_PS_Color(LED_COLOR_GREEN);
	else if ((bit_counter & (1 << 3)))
		GPIO_Set_PS_Color(LED_COLOR_RED);
	else
		GPIO_Set_PS_Color(LED_COLOR_OFF);

	/* Alternative implementation
	GPIO_Set_PL_Color((bit_counter & 0x03));
	GPIO_Set_PS_Color((bit_counter & 0x0C) >> 2);
	*/

	// Increment COUNTER 0 to 15
	bit_counter++;
	bit_counter %= (1 << 4); // it's only a 4 bit counter
}

// Send each byte of DATA using UART1
static void SendData_UART(void * data, uint32_t len)
{
	for (uint32_t i = 0; i < len; i++)
	{
		UART_SEND_BYTE(((char)*((char*)data+i)));
	}
}

