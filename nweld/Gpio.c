//###########################################################################
//
// FILE:	DSP281x_Gpio.c
//
// TITLE:	DSP281x General Purpose I/O Initialization & Support Functions.
//
//###########################################################################
//
//  Ver | dd mmm yyyy | Who  | Description of changes
// =====|=============|======|===============================================
//  1.00| 11 Sep 2003 | L.H. | No change since previous version (v.58 Alpha)
//###########################################################################

//2020-0902  Hua Remark define GPIO
// modified by Xu 20210426

#include <DSP281x_Device.h>     // DSP281x Headerfile Include File
#include <DSP281x_Examples.h>   // DSP281x Examples Include File

/******************************************************************
 * Name: void InitGpio(void)
 * Function: reload to initializes the Gpio to a known state.
 *     TI propotype: InitGpio()
 ******************************************************************/
void InitGpio(void)
{
    EALLOW;

	// SCI-A
    GpioMuxRegs.GPFMUX.bit.SCITXDA_GPIOF4 = 1;  // Using as SCIA TxD
    GpioMuxRegs.GPFMUX.bit.SCIRXDA_GPIOF5 = 1;  // Using as SCIA RxD

    // SPI
    GpioMuxRegs.GPFMUX.bit.SPISIMOA_GPIOF0 =1;  // SPI SIMOA Using as DAC DATA
    GpioMuxRegs.GPFMUX.bit.SPICLKA_GPIOF2  =1;  // SPI CLK   Using as DAC CLK
    // chip selection
    GpioMuxRegs.GPFMUX.bit.SPISTEA_GPIOF3  =0;  // IO: DAC CS1
    GpioMuxRegs.GPFMUX.bit.SPISOMIA_GPIOF1 =0;  // IO: DAC CS2
    GpioMuxRegs.GPFDIR.bit.GPIOF3 = 1;          // output
    GpioMuxRegs.GPFDIR.bit.GPIOF1 = 1;          // output

    // OLED
    GpioMuxRegs.GPAMUX.bit.PWM1_GPIOA0 = 0;     // IO: SCL
    GpioMuxRegs.GPAMUX.bit.PWM2_GPIOA1 = 0;     // IO: SDA
    GpioMuxRegs.GPADIR.bit.GPIOA0 = 1;          // output
    GpioMuxRegs.GPADIR.bit.GPIOA1 = 1;          // output


    // GPIO outlet
    GpioMuxRegs.GPBMUX.bit.PWM7_GPIOB0 = 0;     // IO: ENABLE
    GpioMuxRegs.GPBMUX.bit.PWM8_GPIOB1 = 0;     // IO: REDLIGHT
    GpioMuxRegs.GPBDIR.bit.GPIOB0 = 1;          // output
    GpioMuxRegs.GPBDIR.bit.GPIOB1 = 1;          // output
    // GPIO inlet
    GpioMuxRegs.GPBMUX.bit.PWM9_GPIOB2 = 0;     // IO: CK_EN
    GpioMuxRegs.GPBMUX.bit.PWM10_GPIOB3= 0;     // IO: KEY
    GpioMuxRegs.GPBDIR.bit.GPIOB2  = 0;         // input
    GpioMuxRegs.GPBDIR.bit.GPIOB3  = 0;         // input

    EDIS;
}

//===========================================================================
// No more.
//===========================================================================
