//###########################################################################
// $TI Release: DSP281x C/C++ Header Files V1.20 $
// $Release Date: July 27, 2009 $
//###########################################################################
//
// Designed by Jacalt Laser
// Modified by Jianqiu Xu, 20210420
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#include "DSP281x_Device.h"     // DSP281x Headerfile Include File
#include "DSP281x_Examples.h"   // DSP281x Examples Include File

/******************************************************************
 * InitEv: start the AD conversion using T1 timer
 * Configurate the general timer_1, through EvaRegs
 * At the end of each EV T1 cycle, trigger the ADC INT.
 * EVA Clock is enabled in InitSysCtrl();
 * ----------------------------------------------------------------
 * This function initializes to a known state.
 ******************************************************************/
void InitEv(void)
{
   EvaRegs.T1CMPR = 0x0080;               // Setup T1 compare value
   EvaRegs.T1PR = 0x0064;                 // ADC period = 200 kHz
   EvaRegs.GPTCONA.bit.T1TOADC = 0x02;    // Enable EVASOC by periodic INT
   
   // TECMPR=1, Enable timer 1 compare 
   // TMODE=2, upcount mode
   // TPS = 0, TCLK=HSPCLK=20 MHz
   // TCLS10=00, internal clock
   // TCLD=0, re-count from zero
   EvaRegs.T1CON.all = 0x1042;

   EvaRegs.EVAIMRA.bit.T1PINT = 1;        // Enable T1 INT
   EvaRegs.EVAIFRA.bit.T1PINT = 1;        // clear T1 INT flag
   EvaRegs.T1CNT = 0;                     // setup T1 Count=0            
}	
	
//===========================================================================
// No more.
//===========================================================================
