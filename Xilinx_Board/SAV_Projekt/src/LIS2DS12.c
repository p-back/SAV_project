/*
 * LIS2DS12.c
 *
 *  Created on: 03.05.2019
 *      Author: Nico
 */

 // ----------------------------------------------
//		INCLUES
#include <stdio.h>
#include <unistd.h>
#include "xil_printf.h"
#include "xparameters.h"
#include "xiic.h"			// Driver for AXI IIC

#include "LIS2DS12_Registers.h"
#include "LIS2DS12.h"

// ----------------------------------------------
//		DEFINES 
// Base Address of AXI IIC
#define IIC_BASE_ADDRESS	XPAR_IIC_0_BASEADDR
#define MINIZED_MOTION_SENSOR_ADDRESS_SA0_HI  0x1D /* 0011101b for LIS2DS12 on MiniZed when SA0 is pulled high*/

// ----------------------------------------------
//		PRIVATE VARIABLES
static int ByteCount;
static u8 send_byte;
static u8 write_data [256];
static u8 i2c_device_addr = MINIZED_MOTION_SENSOR_ADDRESS_SA0_HI; //by default

// ----------------------------------------------
//		PRIVATE FUNCTIONS
static u8 LIS2DS12_WriteReg(u8 Reg, u8 *Bufp, u16 len);
static u8 LIS2DS12_ReadReg(u8 Reg, u8 *Bufp, u16 len);
static int u16_2s_complement_to_int(u16 word_to_convert);

// ----------------------------------------------
//		PUBLIC FUNCTION DEFINITIONS
void LIS2DS12_Init()
{
	send_byte = 0x00; //No auto increment
	LIS2DS12_WriteReg(LIS2DS12_ACC_CTRL2, &send_byte, 1);

	//Write 60h in CTRL1	// Turn on the accelerometer.  14-bit mode, ODR = 400 Hz, FS = 2g
	send_byte = 0x60;
	LIS2DS12_WriteReg(LIS2DS12_ACC_CTRL1, &send_byte, 1);

	//Enable interrupt
	send_byte = 0x01; //Acc data-ready interrupt on INT1
	LIS2DS12_WriteReg(LIS2DS12_ACC_CTRL4, &send_byte, 1);
}

int LIS2DS12_Get_Temperature()
{
	int temp;
	u8 read_value;

	LIS2DS12_ReadReg(LIS2DS12_ACC_OUT_T, &read_value, 1);

	// Convert VALUE to TEMP
	// Temperature is from -40 to +85 deg C.  So 125 range.  0 is 25 deg C.  +1 deg C/LSB.  So if value < 128 temp = 25 + value else temp = 25 - (256-value)
	if (read_value < 128)
		temp = 25 + read_value;
	else
		temp = 25 - (256 - read_value);

	return temp;
}

LIS2DS12_Motion_t * LIS2DS12_Get_Motion()
{
	static LIS2DS12_Motion_t motion;
	u8 read_value_LSB;
	u8 read_value_MSB;
	u16 accel_X;
	u16 accel_Y;
	u16 accel_Z;
	u8 accel_status;
	u8 data_ready;

	data_ready = 0;
	while (!data_ready)
	{ //wait for DRDY
		LIS2DS12_ReadReg(LIS2DS12_ACC_STATUS, &accel_status, 1);
		data_ready = accel_status & 0x01; //bit 0 = DRDY
	} //wait for DRDY


	//Read X:
	LIS2DS12_ReadReg(LIS2DS12_ACC_OUT_X_L, &read_value_LSB, 1);
	LIS2DS12_ReadReg(LIS2DS12_ACC_OUT_X_H, &read_value_MSB, 1);
	accel_X = (read_value_MSB << 8) + read_value_LSB;
	motion.Accel_X = u16_2s_complement_to_int(accel_X);
	//Read Y:
	LIS2DS12_ReadReg(LIS2DS12_ACC_OUT_Y_L, &read_value_LSB, 1);
	LIS2DS12_ReadReg(LIS2DS12_ACC_OUT_Y_H, &read_value_MSB, 1);
	accel_Y = (read_value_MSB << 8) + read_value_LSB;
	motion.Accel_Y = u16_2s_complement_to_int(accel_Y);
	//Read Z:
	LIS2DS12_ReadReg(LIS2DS12_ACC_OUT_Z_L, &read_value_LSB, 1);
	LIS2DS12_ReadReg(LIS2DS12_ACC_OUT_Z_H, &read_value_MSB, 1);
	accel_Z = (read_value_MSB << 8) + read_value_LSB;
	motion.Accel_Z = u16_2s_complement_to_int(accel_Z);

	//printf("  Acceleration = X: %+5d, Y: %+5d, Z: %+5d\r\n",iacceleration_X, iacceleration_Y, iacceleration_Z);

	return &motion;
}

// ----------------------------------------------
//		PRIVATE FUNCTION DEFINITIONS
static u8 LIS2DS12_WriteReg(u8 Reg, u8 *Bufp, u16 len)
{
	write_data[0] = Reg;
	for (ByteCount = 1;ByteCount <= len; ByteCount++)
	{
		write_data[ByteCount] = Bufp[ByteCount-1];
	}
	ByteCount = XIic_Send(IIC_BASE_ADDRESS, i2c_device_addr, &write_data[0], (len+1), XIIC_STOP);
	return(ByteCount);
}

static u8 LIS2DS12_ReadReg(u8 Reg, u8 *Bufp, u16 len)
{
	write_data[0] = Reg;
	ByteCount = XIic_Send(IIC_BASE_ADDRESS, i2c_device_addr, (u8*)&write_data, 1, XIIC_REPEATED_START);
	ByteCount = XIic_Recv(IIC_BASE_ADDRESS, i2c_device_addr, (u8*)Bufp, len, XIIC_STOP);
	return(ByteCount);
}

static int u16_2s_complement_to_int(u16 word_to_convert)
{
	u16 result_16bit;
	int result_14bit;
	int sign;

	if (word_to_convert & 0x8000)
	{ //MSB is set, negative number
		//Invert and add 1
		sign = -1;
		result_16bit = (~word_to_convert) + 1;
	}
	else
	{ //Positive number
		//No change
		sign = 1;
		result_16bit = word_to_convert;
	}
	//We are using it in 14-bit mode
	//All data is left-aligned.  So convert 16-bit value to 14-but value
	result_14bit = sign * (int)(result_16bit >> 2);
	return(result_14bit);
}
