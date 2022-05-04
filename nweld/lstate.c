// Company: Jacalt Laser
//
// Ver. 1.0.1
// Author: Jianqiu Xu
// Date: 20210429
//
// Ver 1.1
// Change: renew to TI C-style
// Author: Jianqiu Xu
// Date: 20210806
///////////////////////////////////////////////////////////////////

#include <DSP281x_Device.h>      // DSP281x Header file Include File
#include <DSP281x_Examples.h>    // DSP281x Examples Include File

#include "dac.h"                // DAC for setup tlv5638
#include "Sci.h"                // parameter for SCIA
#include "OLED/weld_oled.h"     // OLED
#include "adc.h"                // ADC for sampling
#include "timer.h"              // CPU timer0
#include "gpio.h"

static Uint16 max_code;      // code for maximum power
static Uint16 gLaser = 0;    // 1=laser on, 0=off, 2=no reflection
static struct REV_DATA revDate = {0, 0, 100, 0, 0};

/******************************************************************
 * Name: void DispOLED()
 * Function: display the laser power in OLED
 * Called by: HoldTirg()
 *****************************************************************/
void DispOLED(void)
{
    if (revDate.gPower == 0) 
        wk_power(0);    // no laser is a specific case
    else wk_power(revDate.gPower);    // display power level on OLED
}


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
// management and detect of RED light
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/******************************************************************
 * Name: Uint16 ModuRed()
 * Function: emit red light with 1 kHz modulation to suppress noise
 *           from circumstance and electrics, total 10 cycles
 * called by: PressTrig()
 * return 1: reflected light be detected
 *        0: no reflected light
 *****************************************************************/
Uint16 ModuRed(void)
{
    float PDon = 0;
    float PDoff = 0;

    //--------------------------------------------------
    // reset SEQ1 and re-start CpuTimer0,
    // and make SEQ1 delay 1 us after Timer0
    // so that, the SEQ1 sampled between the Timer0 period
    //--------------------------------------------------
    CpuTimer0Regs.TCR.bit.TSS = 0;
    DELAY_US(1);
    AdcRegs.ADCTRL2.bit.RST_SEQ1 = 1;

    Uint16 i = 0;
    while (i < 10)                         // 10 cycles
    {
        switch (mFlag) 
        {
            //-- The red pilot is on
            case 0:    // ignore rise and fall stage
                REDLIGHT = 1;
                break;

            case 1:
                REDLIGHT = 1;
                if (adcFLAG)
                {
                    PDon += adcPD/220.0;    // 220 sampling in 10 cycles
                    adcFLAG = 0;
                }
                break;

            case 2:
                REDLIGHT = 1;
                break;

            //-- The red pilot is off
            case 3:
                REDLIGHT = 0;
                break;

            case 4:
                REDLIGHT = 0;
                if (adcFLAG)
                {
                    PDoff += adcPD/220.0;   // 220 sampling in 10 cycles
                    adcFLAG = 0;
                }
                break;

            case 5:
                REDLIGHT = 0;
                break;

            //-- start next cycle
            case 6:
                i++;
                break;
        }
    } // end while

    // determine the reflection of red light
    if (PDon-PDoff > 0) return 1;
    else return 0;
}


/*****************************************************************
 * Name: void LasFreq()
 * Function: lasing in the given freqency through ENABLE GPIO
 *     0 Hz = CW output
 *     frequecy = gFreq, duty cycle = gDuty
 *     checking the emit state each interrupt timer
 * called by: HoldTrig()
 *****************************************************************/
void LasFreq(void)
{
    float iDur, iShot;

    REDLIGHT = 0;    // close red light in working

    if (revDate.gFreq==0 || revDate.gDuty==100)
    {
        ENABLE = 1;     // CW laser
    }
    else
    {
        iDur = 100.0*1000/revDate.gFreq;            // duration in 10 us
        iShot = iDur*revDate.gDuty/100.0;           // shot time in 10 us

        if (tmCount >= iDur) tmCount = 0;           // reset tmCount

        if (tmCount < iShot)
        {
            ENABLE = 1;            // emit the laser
        }
        else ENABLE = 0;  // stop the laser
    }
}


/******************************************************************
 * Name: void LasRsie()
 * Function: open the laser with a rising edge
 *     assume each step in 30 us (need experiments)
 *     with a gRise time to the maximum output
 *****************************************************************/
void LasRise(void)
{
    // write to tlv5638 with about 20us
    // thus we make each step of 30 us
    // need varified by experiments

    // maximum power 3542=20A
    Uint16 max_code = revDate.gPower*3542/100;     
    if (revDate.gRise != 0)
    {
        Uint16 code = 0;
        Uint16 iStep = revDate.gRise/30;

        for (Uint16 i=0; i<iStep; i++)
        {
            code += max_code/iStep;
            wr_2chip(code);
        }
    }

    wr_2chip(max_code);         // fast and completely
}


/******************************************************************
 * Name: void LasFall()
 * Function: close the laser with a falling edge
 *     assume each step in 30 us (need experiments)
 *     with a gFall time from the maximum output to zero
 *****************************************************************/
void LasFall(void)
{
    if (revDate.gFall != 0)
    {
        Uint16 code = max_code;
        Uint16 iStep = revDate.gFall/30;

        for (Uint16 i=iStep; i>0; i--)
        {
            code -= max_code/iStep;
            wr_2chip(code);
        }
    }

    wr_2chip(0);                // finally close all
}

//>>>>>>>>>>>>>>>>>>>>>>>>>
// Group: key state program
//>>>>>>>>>>>>>>>>>>>>>>>>>

/******************************************************************
 * Name: void FreeTrig()
 * Function: laser state when KEY_OFF
 ******************************************************************/
void FreeTrig(void)
{
    ENABLE = 1;
    if (CK_EN)                        // enable-key is on
    {                
        REDLIGHT = 1;                 // pilot on
        wk_pilot();                   // OLED red light
    }
    else 
    {                                 // Enable-key is off
        REDLIGHT = 0;
        wk_startup();
    }

    ReceiveData(&revDate);     // deal SCI port only in idle time
}


/******************************************************************
 * Name: void HoldTrig()
 * Function: laser state when KEY_HOLD
 *     glaser=0, enable key is off
 *     glaser=1, laser is emit
 *     glaser=2, no reflection is detected
 ******************************************************************/
void HoldTrig(void)
{
    switch (gLaser)
    {
        case 0:
            REDLIGHT = 1;
            wk_disable();            // show: enable off
            break;

        case 1:
            REDLIGHT = 0;            // pilot off
            DispOLED();
            tmCount = 0;
            LasFreq();
            break;

        case 2:
            REDLIGHT = 0;
            wk_norefl();            // no reflection
            break;
    }
}


/******************************************************************
 * Name: void PressTrig()
 * Function: laser state when KEY_PRESS
 ******************************************************************/
void PressTrig(void)
{
    Uint16 normalReflect = 0;

    ENABLE = 1;
    // enanble is on
    if (CK_EN) 
    {
        normalReflect = ModuRed();
        if (normalReflect)    // reflection is correct
        {
            REDLIGHT = 0;           // close redlight when lasing
            gLaser = 1;
            LasRise();
        }
        else    // no reflection
        {
            REDLIGHT = 0;
            gLaser = 2;
        }
    }
    else gLaser = 0;                // enable key off and do not lase
}


/******************************************************************
 * Name: void RelaxTrig()
 * Function: laser state when KEY_RELEASE
 ******************************************************************/
void RelaxTrig(void)
{
    if (gLaser == 1) LasFall();
    gLaser = 0;
}

//=================================================================
// End of file
//=================================================================

