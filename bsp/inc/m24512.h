/**
 * @brief API interface of M24512 EEPROM
 */
#ifndef _M24512_H__
#define _M24512_H__

#include "stm32f4xx_hal.h"
#include <string.h>

#include "eeprom_i2c.h"
#include "eeprom_address.h"
#include "main.h"

#define M24512_ADDR 0xA0

//-- pBuffer: pointer of data buffer
//-- WriteAddr: address to be write
//-- buffersize: size of data buffer

uint8_t M24512_WriteBuffer(uint8_t* pBuffer, uint16_t WriteAddr, uint8_t buffersize);
uint8_t M24512_ReadBuffer(uint8_t* pBuffer, uint16_t ReadAddr, uint8_t NumByteToRead);

//-- test M24512
// void M24512_TEST(void);
uint8_t M24512_CHECK(void);

#endif//_M24512_H__
