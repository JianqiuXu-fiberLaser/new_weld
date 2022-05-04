// According the specification of hand welding machine
// 1. Change the laser power simultaneously
// 2. Pre-setting the current in poilt light
// Modified by Jianqiu Xu, 2021.04.19
//##################################################################

#include <DSP281x_Device.h>     // DSP281x Header file Include File
#include <DSP281x_Examples.h>   // DSP281x Examples Include File

//----- external DAC I/O define  Hua
#define DAC_CS1 GpioDataRegs.GPFDAT.bit.GPIOF3     // U2.A=LD1, B=LD2
#define DAC_CS2 GpioDataRegs.GPFDAT.bit.GPIOF1     // U4.A LD3
#define DAC_CS3 GpioDataRegs.GPFDAT.bit.GPIOF6     // U6.A poilt

struct  TLV5638_BITS {       // bits  description
   Uint16    newData:12;     // 0-11

   // control bit, D15..D12
   Uint16    R0:1;           // 12
   Uint16    PWR:1;          // 13
   Uint16    SPD:1;          // 14
   Uint16    R1:1;           // 15

};

union TLV5638_DataFormat {
   Uint16               all;
   struct TLV5638_BITS  bit;
};
    
/*******************************************************************
 * Name: void wr_t5638(Uint16 DAC_SCn, Uint16 wr_data)
 * Function: write to tlv5638 with wr_data
 *    select tlv5638 through chip selection
 *******************************************************************/
void wr_t5638(Uint16 DAC_CSn, Uint16 wr_data)
{
    // chip selection
    if (DAC_CSn == 1) DAC_CS1 = 0;
    else DAC_CS2 = 0;

    //+++++++++++++++++++++++++++++++++++++++++++++++
    // SPICLK=2MHz, each bit cost 0.5us, 16bit~8us
    // write data to tlv5638 through SPI
    // waiting to clear the buffer flag
    //+++++++++++++++++++++++++++++++++++++++++++++++
    DELAY_US(20);
    if (SpiaRegs.SPISTS.bit.BUFFULL_FLAG == 0) 
        SpiaRegs.SPITXBUF = wr_data;
    
    // avoid being block by while
    Uint16 i = 0;
    while (SpiaRegs.SPISTS.bit.BUFFULL_FLAG == 1) 
    {
        i++;
        if (i >= 100) break;
    }
    DELAY_US(20);

    if (DAC_CSn == 1) DAC_CS1 = 1;
    else DAC_CS2 = 1;
}


/*******************************************************************
 * Name: void cfg_t5638(void)
 * Function: setup tlv5638 working state
 *     setup vref=2.048V
 *******************************************************************/
void cfg_t5638(void)
{
	union TLV5638_DataFormat TLV5638_Data;

    TLV5638_Data.bit.R1 = 1;           // Write data to control register
    TLV5638_Data.bit.SPD = 1;          // fast mode
    TLV5638_Data.bit.PWR = 0;          // normal operation
    TLV5638_Data.bit.R0 = 1;
    TLV5638_Data.bit.newData = 0x02;   // Vref=2.048V

    wr_t5638(1,TLV5638_Data.all);
    wr_t5638(2,TLV5638_Data.all);
}


/******************************************************************
 * Name: volt2Code(float V)
 * Function: convert volt to code of tlv5638
 * input: voltage, V in Volt
 *     Code = 4096/(2*Vref) =1000*V, Vref=2.048
 ******************************************************************/
Uint16 volt2Code(float V)
{
    return (Uint16)(V*1000+0.5);       // rounding-off method
}


/********** Uint16 wr_2chip(Uint16 wr_data)  **********************
 * Name: void wr_2chip(Uint16 wr_data)
 * Function: write U2, U4 simultaneously
 *     A+B port, although port B of U4 is not used
 * input: data in tlv5638 Format
 ******************************************************************/
void wr_2chip(Uint16 wr_data)
{
    union TLV5638_DataFormat TLV5638_Data;

    TLV5638_Data.all =0x0;             // Init TLV5638_Data

    TLV5638_Data.bit.PWR =0;           // normal operation
    TLV5638_Data.bit.SPD =1;           // fast mode

    wr_data &= 0x0FFF;                 // data in lower 12 bit only
    TLV5638_Data.bit.newData = wr_data;

    //---------------------- ADC1, U2 -------------------
    // Write data to B and buffer
    TLV5638_Data.bit.R1 = 0;
    TLV5638_Data.bit.R0 = 0;
    wr_t5638(1,TLV5638_Data.all);

    // updata to A and B port
    TLV5638_Data.bit.R1 = 1;
    TLV5638_Data.bit.R0 = 0;
    wr_t5638(1,TLV5638_Data.all);

    //---------------------- ADC2, U4 -------------------
    // write to A only
    TLV5638_Data.bit.R1 = 1;
    TLV5638_Data.bit.R0 = 0;
    wr_t5638(2,TLV5638_Data.all);
}


//=================================================================
// No more.
//=================================================================

