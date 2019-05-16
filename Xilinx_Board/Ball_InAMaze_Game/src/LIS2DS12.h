/*
 * LIS2DS12.h
 *
 *  Created on: 03.05.2019
 *      Author: Nico
 */

#ifndef SRC_LIS2DS12_H_
#define SRC_LIS2DS12_H_

// ----------------------------------------------
//		INCLUES
#include <stdint.h>

// ----------------------------------------------
//		DECLARATIONS
typedef struct
{
	int Accel_X;	// 4 Bytes each
	int Accel_Y;
	int Accel_Z;
} LIS2DS12_Motion_t;

void 				LIS2DS12_Init();
int 				LIS2DS12_Get_Temperature();
LIS2DS12_Motion_t * LIS2DS12_Get_Motion();

#endif /* SRC_LIS2DS12_H_ */
