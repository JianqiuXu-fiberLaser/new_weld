// TI File $Revision: /main/3 $
// Checkin $Date: July 9, 2009   10:51:58 $
//###########################################################################
//
// FILE:    DSP281x_CpuTimers.c
//
// TITLE:   CPU 32-bit Timers Initialization & Support Functions.
//
// NOTES:   CpuTimer2 is reserved for use with DSP BIOS and
//          other realtime operating systems.
//
//          Do not use these this timer in your application if you ever plan
//          on integrating DSP-BIOS or another realtime OS.
//
//###########################################################################
// $TI Release: DSP281x C/C++ Header Files V1.20 $
// $Release Date: July 27, 2009 $
//###########################################################################

#include <DSP281x_Device.h>     // DSP281x Headerfile Include File
#include <DSP281x_Examples.h>   // DSP281x Examples Include File

struct CPUTIMER_VARS CpuTimer0;

// When using DSP BIOS & other RTOS, comment out CPU Timer 2 code.
//struct CPUTIMER_VARS CpuTimer1;
//struct CPUTIMER_VARS CpuTimer2;

//---------------------------------------------------------------------------
// InitCpuTimers:
//---------------------------------------------------------------------------
// This function initializes all three CPU timers to a known state.
//
void InitCpuTimers(void)
{
    // Initialize CPU Timer 0 
    CpuTimer0.RegsAddr = &CpuTimer0Regs;
    CpuTimer0Regs.PRD.all  = 0x15752A00;        // 360*10^6/120MHz = 3 second
	
    // Initialize pre-scale counter
    CpuTimer0Regs.TPR.all  = 0;
    CpuTimer0Regs.TPRH.all = 0;

    CpuTimer0Regs.TCR.bit.TSS = 1;              // Make sure timer is stopped:
    CpuTimer0Regs.TCR.bit.TRB = 1;              // Reload all counter register with period value:
    CpuTimer0.InterruptCount = 0;               // Reset interrupt counters:

}

//---------------------------------------------------------------------------
// ConfigCpuTimer:
//---------------------------------------------------------------------------
// This function initializes the selected timer to the period specified
// by the "Freq" and "Period" parameters. The "Freq" is entered as "MHz"
// and the Period in "uSeconds".
// Restart Timer0 after configuration.
//
void ConfigCpuTimer(struct CPUTIMER_VARS *Timer, float Freq, float Period)
{
    // Set pre-scale counter to 0
    Timer->RegsAddr->TPR.all  = 0;
    Timer->RegsAddr->TPRH.all = 0;

    // Initialize timer period: 
    Timer->CPUFreqInMHz = Freq;
    Timer->PeriodInUSec = Period;
    Timer->RegsAddr->PRD.all = Timer->CPUFreqInMHz * Timer->PeriodInUSec;

    // Initialize timer control register:
    Timer->RegsAddr->TCR.bit.TRB = 1;           // 1 = reload timer
    Timer->RegsAddr->TCR.bit.SOFT = 1;
    Timer->RegsAddr->TCR.bit.FREE = 1;          // Timer run free when debug
    Timer->RegsAddr->TCR.bit.TIE  = 1;          // 1-- Enable Timer Interrupt

    // Reset interrupt counter:
    Timer->InterruptCount = 0;

    Timer->RegsAddr->TCR.bit.TSS = 0;           // 1=Stop, 0=Restart Timer
}

//===========================================================================
// End of file.
//===========================================================================
