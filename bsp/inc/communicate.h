#ifndef _COMMUNICATE_H__
#define _COMMUNICATE_H__

#include "stm32f4xx_hal.h"
#include "main.h"
#include "cmsis_os.h"
#include "tcp_client.h"

void CCD_UART_SendData(uint8_t* array, uint16_t len);
void IPC_UART_SendData(uint8_t* array, uint16_t len);

#endif//_COMMUNICATE_H__
