/**
 * 文件：stepmotor.x
 * 说明：步进电机底层文件
 * 项目：焊接跟踪项目
 * 作者：张祺
 * 版本：V1.0.0
 *
 * 修订说明：根据实际使用精简
 *          replaces the reset status with the set status to match the
 *          co-anode connection method.
 * author: J. Xu
 * Date: 20220504
 * Version: V1.1.0
 */
#include "stepmotor.h"

/***************************************************************************
 * 转动电机-- 特定步数
 ***************************************************************************/
/*
 * 功能：给步进电机X发送脉冲使其运动
 *      pulse width of 2.0 ms and frequency of 500 Hz
 * 参数：step number;
 * 返回：void
 * 备注：给一个脉冲运动一步
 *     此处的脉冲频率并不精确，由于osDelay()函数没有考虑函数本身的运行时间
 */
static void stepmotor_x_pulse(uint8_t step)
{
	while(step--)
	{
		HAL_GPIO_WritePin(MOTOR_PU1_GPIO_Port, MOTOR_PU1_Pin, GPIO_PIN_RESET);
		osDelay(1);
		HAL_GPIO_WritePin(MOTOR_PU1_GPIO_Port, MOTOR_PU1_Pin, GPIO_PIN_SET);
		osDelay(1);
	}
}

/*
 * 功能：步进电机Y运动
 * 参数：步进数
 * 返回：void
 * 备注：同上
 */
static void stepmotor_y_pulse(uint8_t step)
{
	while(step--)
	{
		HAL_GPIO_WritePin(MOTOR_PU2_GPIO_Port, MOTOR_PU2_Pin, GPIO_PIN_RESET);
		osDelay(1);
		HAL_GPIO_WritePin(MOTOR_PU2_GPIO_Port, MOTOR_PU2_Pin, GPIO_PIN_SET);
		osDelay(1);
	}
}


/***************************************************************************
 * 转动电机 -- 方向和角度
 ***************************************************************************/
/*
 * 功能：步进电机X运行
 * 参数：方向，角度
 * 返回：void
 * 备注：运行原理：使能，设置方向，然后旋转，使能关
 */
void stepmotor_x_run(enum_direction direction, float degree)
{
	osMutexAcquire((osMutexId_t)StepMotorXMutexHandle,
				   (uint32_t)portMAX_DELAY);

	STEPMOTOR_X_ENABLE;

	if(direction == FORWARD)
		STEPMOTOR_X_FORWARD;
	else
		STEPMOTOR_X_BACKWARD;

	stepmotor_x_pulse(degree/DEGREE_PER_STEP);
	STEPMOTOR_X_DISABLE;

	osMutexRelease((osMutexId_t)StepMotorXMutexHandle);
}

/*
 * 功能：步进电机Y运行
 * 参数：方向，角度
 * 返回：void
 * 备注：运行原理：使能，设置方向，然后旋转，使能关
 */
void stepmotor_y_run(enum_direction direction, float degree)
{
	osMutexAcquire((osMutexId_t)StepMotorYMutexHandle,
				   (uint32_t)portMAX_DELAY);

	STEPMOTOR_Y_ENABLE;

	if(direction == FORWARD)
		STEPMOTOR_Y_FORWARD;
	else
		STEPMOTOR_Y_BACKWARD;

	stepmotor_y_pulse(degree/DEGREE_PER_STEP);
	STEPMOTOR_Y_DISABLE;

	osMutexRelease((osMutexId_t)StepMotorYMutexHandle);
}

/***************************************************************************
 * 电机归零
 ***************************************************************************/
/*
 * 功能：步进电机X归零
 * 参数：void
 * 返回：uint8_t
 *      1, 返回超出行程
 *      0，返回成功
 * 备注：运行原理：旋转直到检测到零位电平（by stopper）
 */
uint8_t stepmotor_x_find_zero(void)
{
	uint16_t count = 0;  //步进电机脉冲计数
	osMutexAcquire((osMutexId_t)StepMotorXMutexHandle,
				   (uint32_t)portMAX_DELAY);
	STEPMOTOR_X_ENABLE;
	STEPMOTOR_X_FORWARD;
	do
	{
		stepmotor_x_pulse(1);
		count++;
	} while (GET_STEPMOTOR_X_LIMIT == GPIO_PIN_SET || count<RUN_MAX_COUNT);

	STEPMOTOR_X_DISABLE;
	osMutexRelease((osMutexId_t)StepMotorXMutexHandle);
	return ((count >= RUN_MAX_COUNT) ? 1 : 0);
}

/*
 * 功能：步进电机Y归零
 * 参数：void
 * 返回：void
 * 备注：同步进电机X
 */
uint8_t stepmotor_y_find_zero(void)
{
	uint16_t count=0; //步进电机脉冲计数
	osMutexAcquire((osMutexId_t)StepMotorYMutexHandle,
				   (uint32_t)portMAX_DELAY);

	STEPMOTOR_Y_ENABLE;
	STEPMOTOR_Y_FORWARD;
	do
	{
		stepmotor_x_pulse(1);
		count++;
	} while(GET_STEPMOTOR_Y_LIMIT == GPIO_PIN_SET || count<RUN_MAX_COUNT);
	STEPMOTOR_Y_DISABLE;
	osMutexRelease((osMutexId_t)StepMotorYMutexHandle);

	return ((count >= RUN_MAX_COUNT) ? 1 : 0);
}
