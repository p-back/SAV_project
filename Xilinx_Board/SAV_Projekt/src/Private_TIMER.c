/*
 * Private_TIMER.c
 *
 *  Created on: 02.05.2019
 *      Author: Nico
 */


// ----------------------------------------------
//		INCLUES
#include "xparameters.h"
#include "xscutimer.h"
#include "xscugic.h"
#include "xil_exception.h"
#include "xil_printf.h"
#include "Private_TIMER.h"

// ----------------------------------------------
// 		DEFINES
#define TIMER_DEVICE_ID			XPAR_XSCUTIMER_0_DEVICE_ID
#define INTC_DEVICE_ID			XPAR_SCUGIC_SINGLE_DEVICE_ID
#define TIMER_IRPT_INTR			XPAR_SCUTIMER_INTR
#define TIMER_PRESCALER_VALUE	0x43 - 0x01	// 333.33 MHz / 67 = ca. 5 MHz (- 1 because it is written + 1)
#define CHECK_RETURN(status) 	{	if (status != XST_SUCCESS) { 	\
										return XST_FAILURE;			\
									}								\
							 	}

// ----------------------------------------------
// Internal VARIABLES
static XScuTimer 		TimerInstance;		/* Cortex A9 Scu Private Timer Instance */
static XScuGic 			IntcInstance;		/* Interrupt Controller Instance */
static Private_TIMER_CB user_cb;

// ----------------------------------------------
// Internal FUNCTION Declaration
static int TimerSetupIntrSystem(XScuGic *IntcInstancePtr, XScuTimer *TimerInstancePtr, u16 TimerIntrId);
static void TimerIntrHandler(void *CallBackRef);
static void TimerDisableIntrSystem(XScuGic *IntcInstancePtr, u16 TimerIntrId);

// ----------------------------------------------
// Public FUNCTION Definitions

// Init Function
int Private_TIMER_Init(uint32_t Frequency, Private_TIMER_CB timer_callback)
{
	XScuTimer_Config *ConfigPtr;

	// Save user callback function
	user_cb = timer_callback;

	// Initialize the Scu Private Timer driver.
	ConfigPtr = XScuTimer_LookupConfig(TIMER_DEVICE_ID);

	// This is where the virtual address would be used, this example
	//uses physical address.
	CHECK_RETURN(XScuTimer_CfgInitialize(&TimerInstance, ConfigPtr, ConfigPtr->BaseAddr));

	// Perform a self-test to ensure that the hardware was built correctly.
	CHECK_RETURN(XScuTimer_SelfTest(&TimerInstance));

	// Connect the device to interrupt subsystem so that interrupts can occur.
	CHECK_RETURN(TimerSetupIntrSystem(&IntcInstance, &TimerInstance, TIMER_IRPT_INTR));

	// Set PRESCALER --> Timer freq. is 333.33 MHz --> Prescale is 67 --> Timer freq. = 5 MHz
	XScuTimer_SetPrescaler(&TimerInstance, TIMER_PRESCALER_VALUE);

	// Enable Auto reload mode.
	XScuTimer_EnableAutoReload(&TimerInstance);

	// Example: desired freq: 2 Hz -> 1 / 2 Hz = 0.5 s * 1.000.000.000 = 500.000.000 ns
	// => 5 * 10^8 ns / 200 ns (5 MHz) = 2.500.000 ticks until timer fires
	// Load value = 2.500.000 * 200 ns = 2 Hz
	u32 load_value = ((1*1.0f / Frequency*1.0f) * 1000000000) / 200;

	// Load the timer counter register.
	XScuTimer_LoadTimer(&TimerInstance, load_value - 1);	// - 1 because in registers it's + 1

	// Print Timer Init
	xil_printf("Private Timer was configured with a frequency of %d Hz.\n Using a Prescaler value of %d and a Reload value of %d.\n",
			Frequency, TIMER_PRESCALER_VALUE, load_value);

	return XST_SUCCESS;
}

// Start Timer Function
void Private_TIMER_Start()
{
	XScuTimer_Start(&TimerInstance);
}

// Stop Timer Function
void Private_TIMER_Stop()
{
	XScuTimer_Stop(&TimerInstance);
}

// Completely DeInit the PRIVATE TIMER
void Private_TIMER_DeInit()
{
	XScuTimer_Stop(&TimerInstance);
	TimerDisableIntrSystem(&IntcInstance, TIMER_IRPT_INTR);
}

// ----------------------------------------------
// Private FUNCTION Definitions

// This function sets up the interrupt system such that interrupts can occur for the device.
static int TimerSetupIntrSystem(XScuGic *IntcInstancePtr,
			      XScuTimer *TimerInstancePtr, u16 TimerIntrId)
{
	int Status;

	XScuGic_Config *IntcConfig;

	// Look ip the Interrupt Controller Device Config
	IntcConfig = XScuGic_LookupConfig(INTC_DEVICE_ID);
	if (NULL == IntcConfig) {
		return XST_FAILURE;
	}

	// Initialize the interrupt controller driver so that it is ready to use
	CHECK_RETURN(XScuGic_CfgInitialize(IntcInstancePtr, IntcConfig,IntcConfig->CpuBaseAddress));

	// Function does nothing but ensures backward compatibility
	Xil_ExceptionInit();

	// Connect the interrupt controller interrupt handler to the hardware
	// interrupt handling logic in the processor.
	Xil_ExceptionRegisterHandler(XIL_EXCEPTION_ID_IRQ_INT, (Xil_ExceptionHandler)XScuGic_InterruptHandler, IntcInstancePtr);

	// Connect the device driver handler that will be called when an
	// interrupt for the device occurs, the handler defined above performs
	// the specific interrupt processing for the device.
	Status = XScuGic_Connect(IntcInstancePtr, TimerIntrId, (Xil_ExceptionHandler)TimerIntrHandler, (void *)TimerInstancePtr);
	if (Status != XST_SUCCESS) {
		return Status;
	}

	// Enable the interrupt for the device.
	XScuGic_Enable(IntcInstancePtr, TimerIntrId);

	// Enable the timer interrupts for timer mode.
	XScuTimer_EnableInterrupt(TimerInstancePtr);

	// Enable interrupts in the Processor.
	Xil_ExceptionEnable();

	return XST_SUCCESS;
}

// This function is the Interrupt handler for the Timer interrupt of the
// Timer device. It is called on the expiration of the timer counter in
// interrupt context.
static void TimerIntrHandler(void *CallBackRef)
{
	XScuTimer *TimerInstancePtr = (XScuTimer *) CallBackRef;

	// Clear Interrupt Flag
	XScuTimer_ClearInterruptStatus(TimerInstancePtr);

	// Call USER CALLBACK
	if (user_cb != NULL)
		user_cb();
}

// This function disables the interrupts that occur for the device.
static void TimerDisableIntrSystem(XScuGic *IntcInstancePtr, u16 TimerIntrId)
{
	// Disconnect and disable the interrupt for the Timer.
	XScuGic_Disconnect(IntcInstancePtr, TimerIntrId);
}
