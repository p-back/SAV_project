/*
 * GPIO.h
 *
 *  Created on: 28.04.2019
 *      Author: Nico
 */

#ifndef SRC_GPIO_H_
#define SRC_GPIO_H_

// ----------------------------------------------
//		INCLUDES
#include <stdbool.h>

// ----------------------------------------------
//		DEFINES
#define LED_COLOR_OFF	0
#define LED_COLOR_GREEN	1
#define LED_COLOR_RED	2
#define LED_COLOR_AMBER	3

// ----------------------------------------------
//		DECLARATIONS
int 	GPIO_Init(void);
void 	GPIO_Set_PS_Color(unsigned char led_color);
void 	GPIO_Set_PL_Color(unsigned char led_color);
bool	GPIO_Get_PS_Button(void);

#endif /* SRC_GPIO_H_ */
