/*
 * Private_TIMER.h
 *
 *  Created on: 02.05.2019
 *      Author: Nico
 */

#ifndef SRC_PRIVATE_TIMER_H_
#define SRC_PRIVATE_TIMER_H_

// ----------------------------------------------
//		INCLUES
#include <stdint.h>
#include "xstatus.h"

// ----------------------------------------------
//		DECLARATIONS
// Function declaration of TIMER callback function
typedef void (*Private_TIMER_CB)(void);

// Private TIMER functions

// Freqency: desired timer freq., timer_callback: function that is called when timer expires
int Private_TIMER_Init(uint32_t Frequency, Private_TIMER_CB user_callback);
void Private_TIMER_Start();
void Private_TIMER_Stop();
void Private_TIMER_DeInit();


#endif /* SRC_PRIVATE_TIMER_H_ */
