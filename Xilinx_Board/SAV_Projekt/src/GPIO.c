/*
 * GPIO.c
 *
 *  Created on: 28.04.2019
 *      Author: Nico Teringl
 */

// ----------------------------------------------
//		INCLUES
#include <stdio.h>
#include <unistd.h>
#include "xparameters.h"
#include "xgpiops.h"
#include "xgpio.h"
#include "xstatus.h"
#include "xplatform_info.h"
#include <xil_printf.h>

#include "GPIO.h"

// ----------------------------------------------
//		DEFINES
#define GPIO_DEVICE_ID  		XPAR_XGPIOPS_0_DEVICE_ID
#define GPIO_AXI0_DEVICE_ID  	XPAR_GPIO_0_DEVICE_ID

#define GPIO_PS_BUTTON_OFFSET				0 //MIO#0
#define GPIO_PS_LED_R_OFFSET				52 //MIO#52 (906+52=958)
#define GPIO_PS_LED_G_OFFSET				53 //MIO#53 (906+53=959)

// ----------------------------------------------
//		PRIVATE VARIABLES
static const u32 PSRedLedPin 	= 52; /* MIO_52 = Red LED */
static const u32 PSGreenLedPin 	= 53; /* MIO53 = Green LED */

// GPIO Instances
static XGpioPs 	PS_Gpio;	/* The driver instance for PS GPIO Device. */
static XGpio	PL_Gpio0;	/* The driver instance for PL GPIO Device. */

// ----------------------------------------------
//		PUBLIC FUNCTION DEFINITIONS
void GPIO_Set_PS_Color(unsigned char led_color)
{
	switch(led_color)
	{
		case LED_COLOR_OFF :
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_R_OFFSET, 0x0); //Red LED off
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_G_OFFSET, 0x0); //Green LED off
			break;
		case LED_COLOR_GREEN :
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_R_OFFSET, 0x0); //Red LED off
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_G_OFFSET, 0x1); //Green LED on
			break;
		case LED_COLOR_RED :
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_R_OFFSET, 0x1); //Red LED on
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_G_OFFSET, 0x0); //Green LED off
			break;
		case LED_COLOR_AMBER :
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_R_OFFSET, 0x1); //Red LED on
			XGpioPs_WritePin(&PS_Gpio, GPIO_PS_LED_G_OFFSET, 0x1); //Green LED on
			break;
		default : /* Error */
			//Do nothing
			break;
	}
}

void GPIO_Set_PL_Color(unsigned char led_color)
{
	switch(led_color)
	{
	case LED_COLOR_OFF :
		XGpio_DiscreteWrite(&PL_Gpio0, 1, 0x0); //Red LED off
		XGpio_DiscreteWrite(&PL_Gpio0, 2, 0x0); //Green LED off
		break;
	case LED_COLOR_GREEN :
		XGpio_DiscreteWrite(&PL_Gpio0, 1, 0x1); //Red LED on
		XGpio_DiscreteWrite(&PL_Gpio0, 2, 0x0); //Green LED off
		break;
	case LED_COLOR_RED :
		XGpio_DiscreteWrite(&PL_Gpio0, 1, 0x0); //Red LED off
		XGpio_DiscreteWrite(&PL_Gpio0, 2, 0x1); //Green LED on
		break;
	case LED_COLOR_AMBER :
		XGpio_DiscreteWrite(&PL_Gpio0, 1, 0x1); //Red LED on
		XGpio_DiscreteWrite(&PL_Gpio0, 2, 0x1); //Green LED on
		break;
		default : /* Error */
			//Do nothing
			break;
	}
}

int GPIO_Init()
{
	XGpioPs_Config *ConfigPtr;
	int Status;

	/* Initialize the PS GPIO driver. */
	ConfigPtr = XGpioPs_LookupConfig(GPIO_DEVICE_ID);
	Status = XGpioPs_CfgInitialize(&PS_Gpio, ConfigPtr,
					ConfigPtr->BaseAddr);
	if (Status != XST_SUCCESS) {
		return XST_FAILURE;
	}

	/* Set the direction for the LED pins to be outputs */
	XGpioPs_SetDirectionPin(&PS_Gpio, PSRedLedPin, 1);
	XGpioPs_SetDirectionPin(&PS_Gpio, PSGreenLedPin, 1);

	/* Set the output Enable for the LED pins */
	XGpioPs_SetOutputEnablePin(&PS_Gpio, PSRedLedPin, 1);
	XGpioPs_SetOutputEnablePin(&PS_Gpio, PSGreenLedPin, 1);

	/* Initialize the PL AXI GPIO0 driver */
	Status = XGpio_Initialize(&PL_Gpio0, GPIO_AXI0_DEVICE_ID);
	if (Status != XST_SUCCESS) {
		return XST_FAILURE;
	}

	/* Set the direction for all LED signals as outputs */
	XGpio_SetDataDirection(&PL_Gpio0, 1, 0x00); //All outputs
	return 0;
}