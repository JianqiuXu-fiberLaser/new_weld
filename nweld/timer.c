/////////////////////// Interrupt function  ///////////////////////
#include <DSP281x_Device.h>      // DSP281x Header file Include File
#include <DSP281x_Examples.h>    // DSP281x Examples Include File

/****** global variable in work() function ***********************/
Uint16 tmCount = 0;              // timer counter for laser frequency
Uint16 mFlag;                    // periodic status for moduRed()

/******************************************************************
 * name: interrupt void TINT0_ISR(void)
 * TI propotype: void cpu_timer0_isr(void)
 *     timer count for laser frequency
 *     timer count between sampling
 ******************************************************************/
interrupt void TINT0_ISR(void)
{
    CpuTimer0.InterruptCount++;
    tmCount++;

    // the flage for meduRed()
    switch (CpuTimer0.InterruptCount)
    {
        case 0:
            mFlag = 0;                     // initial status
            break;

        case 7:                            // start sampling when red on
            mFlag = 1;
            break;

        case 19:                           // stop sampling
            mFlag = 2;
            break;

        case 25:                           // red off
            mFlag = 3;
            break;

        case 32:                           // start sampling when red off
            mFlag = 4;
            break;

        case 44:                           // stop sampling
            mFlag = 5;
            break;

        case 50:                           // end of cycle
            mFlag = 6;
            CpuTimer0.InterruptCount = 0;
            break;
    }

    PieCtrlRegs.PIEACK.all = 0x0001;       // reset PIE1 ACK flag bit = 1
}

//=================================================================
// No more.
//=================================================================
