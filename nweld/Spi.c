//
// FILE:   DSP281x_Spi.c
//
// TITLE:  DSP281x SPI Initialization & Support Functions.
//
//###########################################################################
// $TI Release: DSP281x C/C++ Header Files V1.20 $
// $Release Date: July 27, 2009 $
//###########################################################################
//
// Special for tlv5639 communications:
// FIFO does not need for tlv5638
// the data is 16 bit
// tlv5638 receive data at failing edges of SCLK
// only one chip can be communincate in one time
//
// Company: Jacalt Laser
// Author: Jianqiu Xu
// Date: 20210423
//########################################################################### 

#include <DSP281x_Device.h>     // DSP281x Headerfile Include File
#include <DSP281x_Examples.h>   // DSP281x Examples Include File

/******************************************************************
 * Name: InitSPI(void)
 * Function: Initial SPI channel
 *     Ti propotype
 ******************************************************************/
void InitSpi(void)
{
    SpiaRegs.SPICCR.all = 0x4F;
    SpiaRegs.SPICTL.all = 0x0E;
    SpiaRegs.SPIFFTX.bit.TXFFIENA = 0;           // disable FIFO

	// SPICLK: LSPCLK/(SPIBRR+1)=2MHz
    SpiaRegs.SPIBRR = 0x1D;
    SpiaRegs.SPIPRI.bit.FREE = 0x01;            // breakpoints don't disturb x_mission

	SpiaRegs.SPICCR.bit.SPISWRESET = 0x01;		// Enable SPI, Relinquish SPI from Reset
}

//===========================================================================
// No more.
//===========================================================================

