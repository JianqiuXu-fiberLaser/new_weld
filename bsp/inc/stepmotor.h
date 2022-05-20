/**
 * 文件：
 * 说明：步进电机底层文件
 * 项目：焊接跟踪项目
 * 作者：张祺
 * 版本：V1.0.0
  *
 * 修订说明：根据实际使用精简
 * author: J. Xu
 * Date: 20220504
 * Version: V1.1.0
 */
#ifndef _STEPMOTOR_H__
#define _STEPMOTOR_H__

#include "stm32f4xx_hal.h"
#include "cmsis_os.h"
#include "main.h"

// the stepper motor x moves the stage up and down
//-- using co-anode connection, ENANBLE with EN_Port to the earth
// 步进电机X开使能
#define STEPMOTOR_X_ENABLE HAL_GPIO_WritePin(MOTOR_EN1_GPIO_Port, MOTOR_EN1_Pin, GPIO_PIN_RESET)
// 步进电机X关使能
#define STEPMOTOR_X_DISABLE HAL_GPIO_WritePin(MOTOR_EN1_GPIO_Port, MOTOR_EN1_Pin, GPIO_PIN_SET)
// 步进电机X前进
#define STEPMOTOR_X_FORWARD HAL_GPIO_WritePin(MOTOR_DR1_GPIO_Port, MOTOR_DR1_Pin, GPIO_PIN_RESET)
// 步进电机X后退
#define STEPMOTOR_X_BACKWARD HAL_GPIO_WritePin(MOTOR_DR1_GPIO_Port, MOTOR_DR1_Pin, GPIO_PIN_SET)

// the stepper motor Y moves the stage left and right
// 步进电机Y开使能
#define STEPMOTOR_Y_ENABLE HAL_GPIO_WritePin(MOTOR_EN2_GPIO_Port, MOTOR_EN2_Pin, GPIO_PIN_RESET)
// 步进电机Y关使能
#define STEPMOTOR_Y_DISABLE HAL_GPIO_WritePin(MOTOR_EN2_GPIO_Port, MOTOR_EN2_Pin, GPIO_PIN_SET)
// 步进电机Y前进
#define STEPMOTOR_Y_FORWARD HAL_GPIO_WritePin(MOTOR_DR2_GPIO_Port, MOTOR_DR2_Pin, GPIO_PIN_RESET)
// 步进电机Y后退
#define STEPMOTOR_Y_BACKWARD HAL_GPIO_WritePin(MOTOR_DR2_GPIO_Port, MOTOR_DR2_Pin, GPIO_PIN_SET)

//-- Read input pin for the limit detection
//步进电机X检测零位电平
#define GET_STEPMOTOR_X_LIMIT HAL_GPIO_ReadPin(LIMIT_SW1_GPIO_Port, LIMIT_SW1_Pin)
//步进电机Y检测零位电平
#define GET_STEPMOTOR_Y_LIMIT HAL_GPIO_ReadPin(LIMIT_SW2_GPIO_Port, LIMIT_SW2_Pin)

#define DEGREE_PER_STEP 1.8f  // 步进1.8°
#define RUN_MAX_COUNT 1000    // 最大1000个脉冲

typedef enum{
	STEPMOTOR_X,   // X轴步进电机
	STEPMOTOR_Y,   // Y轴步进电机
} enum_stepmotor;  // 步进电机

typedef enum{
	FORWARD,  // 前进
	BACKWARD  // 后退
} enum_direction;  // 电机方向

typedef struct{
	enum_stepmotor stepmotor;
	enum_direction direction;  // 电机方向
	float degree;              // 旋转角度
} typedef_stepmotor_para;      // 电机参数

// motor running functions
// the motor runs by degrees of angle in the direction.
void stepmotor_x_run(enum_direction direction, float degree);
void stepmotor_y_run(enum_direction direction, float degree);

// back to the original point
uint8_t stepmotor_x_find_zero(void);
uint8_t stepmotor_y_find_zero(void);

#endif  //_STEPMOTOR_H__
