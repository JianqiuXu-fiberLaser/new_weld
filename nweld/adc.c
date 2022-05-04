//
// Modified by Jianqiu Xu 20210419
// ADC Sequence for hand-welding machine.
// correct the AdcRaw with over-sampling and middle filtering
//
///////////////////////////////////////////////////////////////////

#include <DSP281x_Device.h>     // DSP281x Headerfile Include File
#include <DSP281x_Examples.h>   // DSP281x Examples Include File

static Uint16 adcRaw[8] = {0};  // Raw sampling data
float adcPD;                    // PD value after filter
Uint16 adcFLAG = 0;             // new Adc flag

/******************************************************************
 * Name: InitAdc()
 * Function: initial Adc register
 *****************************************************************/
void InitAdc(void)
{
    // reset ADC modulus, xu 20210420
    AdcRegs.ADCTRL1.bit.RESET = 1;
    asm(" NOP");
    AdcRegs.ADCTRL1.bit.RESET = 0;

    // To powerup the ADC the ADCENCLK bit should be set first to enable
    // clocks, followed by powering up the bandgap and reference circuitry.
    // After a 5ms delay the rest of the ADC can be powered up. After ADC
    // powerup, another 20us delay is required before performing the first
    // ADC conversion. Please note that for the delay function below to
    // operate correctly the CPU_CLOCK_SPEED define statement in the
    // DSP28_Examples.h file must contain the correct CPU clock period in
    // nanoseconds.

    AdcRegs.ADCTRL3.bit.ADCBGRFDN=0x3;  // Power up bandgap/reference circuitry
    DELAY_US(5000L);                    // Delay before powering up rest of ADC
    AdcRegs.ADCTRL3.bit.ADCPWDN = 1;    // Power up rest of ADC
    DELAY_US(20L);                      // Delay after powering up ADC

    //------- ADC Sequence
    // xu 20210420
    AdcRegs.ADCTRL1.bit.ACQ_PS = 0x2;           // sampling windows = 1 ADCLK
    
    AdcRegs.ADCTRL1.bit.CPS = 0;                // 0 = none frequency divide 
    AdcRegs.ADCTRL3.bit.ADCCLKPS=0x2;           // ADCLK 5MHz

    // sampling mode
    AdcRegs.ADCTRL3.bit.SMODE_SEL =0;           // sampling in sequency mode
    AdcRegs.ADCTRL1.bit.SEQ_CASC = 0;           // channel A and B independent
    AdcRegs.ADCTRL1.bit.CONT_RUN = 0;           
                                                
    AdcRegs.ADCMAXCONV.all= 0x0007;             // maximum 8 conversion             

    // sampling sequence
    AdcRegs.ADCCHSELSEQ1.bit.CONV00 = 0x00;     // PD 1st time
    AdcRegs.ADCCHSELSEQ1.bit.CONV01 = 0x00;     // PD 2nd time
    AdcRegs.ADCCHSELSEQ1.bit.CONV02 = 0x00;     // PD 3rd time
    AdcRegs.ADCCHSELSEQ1.bit.CONV03 = 0x00;     // PD 4th time
    AdcRegs.ADCCHSELSEQ2.bit.CONV04 = 0x00;     // PD 5th time
    AdcRegs.ADCCHSELSEQ2.bit.CONV05 = 0x00;     // PD 6th time
    
    AdcRegs.ADCCHSELSEQ2.bit.CONV06 = 0x01;     // thermister 1
    AdcRegs.ADCCHSELSEQ2.bit.CONV07 = 0x02;     // thermister 2

    // conversion mode
    AdcRegs.ADCTRL2.bit.INT_ENA_SEQ1 = 1;       // ENABLE SEQ1 INT
    AdcRegs.ADCST.bit.INT_SEQ1_CLR =1;          // clear INT flag
    AdcRegs.ADCTRL2.bit.EVA_SOC_SEQ1 = 1;       // enable EVA for SEQ1
    AdcRegs.ADCTRL2.bit.INT_MOD_SEQ1 = 0;       // INT at each CONV end 
    AdcRegs.ADCTRL2.bit.SOC_SEQ1     = 0;       // clear SEQ1 sampling
}

/******************************************************************
 * interrupt void ADCINT_ISR()
 *     ADCA0: Detected reflection of polit, 6 times each cycle
 *     ADCA1: thermister 1
 *     ADCA2: thermister 2
 *****************************************************************/
interrupt void ADCINT_ISR(void)
{
    Uint16 adcAve = 0;

    adcFLAG = 1;

    // sample the PD value 6 times
    adcRaw[0] = (AdcRegs.ADCRESULT[0]>>4);
    adcRaw[1] = (AdcRegs.ADCRESULT[1]>>4);
    adcRaw[2] = (AdcRegs.ADCRESULT[2]>>4);
    adcRaw[3] = (AdcRegs.ADCRESULT[3]>>4);
    adcRaw[4] = (AdcRegs.ADCRESULT[4]>>4);
    adcRaw[5] = (AdcRegs.ADCRESULT[5]>>4);

    // thermister
    adcRaw[6] = (AdcRegs.ADCRESULT[6]>>4);
    adcRaw[7] = (AdcRegs.ADCRESULT[7]>>4);

    // middle filtering
    Uint16 R_max = adcRaw[0];
    Uint16 R_min = adcRaw[0];
    adcAve = adcRaw[0];

    for (Uint16 i=1; i<6; i++)
    {
        // find the largest and least value
        if (R_max < adcRaw[i]) R_max = adcRaw[i];
        if (R_min > adcRaw[i]) R_min = adcRaw[i];

        adcAve += adcRaw[i];
    }
    
    // subtract the large and least value
    // then average the rest
    adcPD = (adcAve-R_max-R_min)/4.0;

    AdcRegs.ADCST.bit.INT_SEQ1_CLR =1;          // clear INT flag
    AdcRegs.ADCTRL2.bit.RST_SEQ1 = 1;           // reset SEQ1
    // response to interrupt, send INT to CPU for operation
    PieCtrlRegs.PIEACK.all = 0x0001;            //PIEACK_GROUP1
}

