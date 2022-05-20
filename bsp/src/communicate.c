/**
 * @brief Send commands for the master, laser, and CCD.
 *        get the data from the queues
 *
 * Company: Jacalt Laser Corp.
 * Author: J. Xu
 * Date: 20220506
 * Version: V1.2.0
 *
 */
#include "main.h"
#include "communicate.h"
#include "tcp_client.h"

/**
 * Function: send the command to the CCD to read pixel data.
 *           using HAL send function
 */
void CCD_UART_SendData(uint8_t* array, uint16_t len)
{
	osMutexAcquire((osMutexId_t)CCDUartMutexHandle,
				   (uint32_t)portMAX_DELAY);
	HAL_UART_Transmit(&CCD_UART, array, len, 100);
	osMutexRelease((osMutexId_t)CCDUartMutexHandle);
}

/**
 * Function: send the command to the master.
 *           using HAL send function
 */
void IPC_UART_SendData(uint8_t* array, uint16_t len)
{
	osMutexAcquire((osMutexId_t)IPCUartMutexHandle,
				   (uint32_t)portMAX_DELAY);
	HAL_UART_Transmit(&IPC_UART, array, len, 100);
	osMutexRelease((osMutexId_t)IPCUartMutexHandle);
}

