// TI File $Revision: /main/3 $
// Checkin $Date: July 9, 2009   17:28:45 $
//###########################################################################
//
// FILE:    DSP281x_GlobalVariableDefs.c
//
// TITLE:   DSP281x Global Variables and Data Section Pragmas.
//
//###########################################################################

#include <DSP281x_Device.h>     // DSP281x Headerfile Include File

//---------------------------------------------------------------------------
// Define Global Peripheral Variables:
//

#pragma DATA_SECTION(AdcRegs,"AdcRegsFile");
volatile struct ADC_REGS AdcRegs;

//----------------------------------------

#pragma DATA_SECTION(CpuTimer0Regs,"CpuTimer0RegsFile");
volatile struct CPUTIMER_REGS CpuTimer0Regs;

#pragma DATA_SECTION(CpuTimer1Regs,"CpuTimer1RegsFile");
volatile struct CPUTIMER_REGS CpuTimer1Regs;


#pragma DATA_SECTION(CpuTimer2Regs,"CpuTimer2RegsFile");
volatile struct CPUTIMER_REGS CpuTimer2Regs;

//-------------ECanA---------------------------

#pragma DATA_SECTION(ECanaRegs,"ECanaRegsFile");
volatile struct ECAN_REGS ECanaRegs;

#pragma DATA_SECTION(ECanaMboxes,"ECanaMboxesFile");
volatile struct ECAN_MBOXES ECanaMboxes;

#pragma DATA_SECTION(ECanaLAMRegs,"ECanaLAMRegsFile");
volatile struct LAM_REGS ECanaLAMRegs;

#pragma DATA_SECTION(ECanaMOTSRegs,"ECanaMOTSRegsFile");
volatile struct MOTS_REGS ECanaMOTSRegs;

#pragma DATA_SECTION(ECanaMOTORegs,"ECanaMOTORegsFile");
volatile struct MOTO_REGS ECanaMOTORegs;

//---------------Eva/Evb-------------------------

#pragma DATA_SECTION(EvaRegs,"EvaRegsFile");
volatile struct EVA_REGS EvaRegs;

#pragma DATA_SECTION(EvbRegs,"EvbRegsFile");
volatile struct EVB_REGS EvbRegs;

//---------------Gpio-------------------------

#pragma DATA_SECTION(GpioDataRegs,"GpioDataRegsFile");
volatile struct GPIO_DATA_REGS GpioDataRegs;

#pragma DATA_SECTION(GpioMuxRegs,"GpioMuxRegsFile");
volatile struct GPIO_MUX_REGS GpioMuxRegs;

//---------------Mcbsp-------------------------

#pragma DATA_SECTION(McbspaRegs,"McbspaRegsFile");
volatile struct MCBSP_REGS McbspaRegs;

//---------------Pie-------------------------

#pragma DATA_SECTION(PieCtrlRegs,"PieCtrlRegsFile");
volatile struct PIE_CTRL_REGS PieCtrlRegs;

#pragma DATA_SECTION(PieVectTable,"PieVectTableFile");
struct PIE_VECT_TABLE PieVectTable;

//----------------Sci------------------------

#pragma DATA_SECTION(SciaRegs,"SciaRegsFile");
volatile struct SCI_REGS SciaRegs;

#pragma DATA_SECTION(ScibRegs,"ScibRegsFile");
volatile struct SCI_REGS ScibRegs;

//----------------Spi------------------------

#pragma DATA_SECTION(SpiaRegs,"SpiaRegsFile");
volatile struct SPI_REGS SpiaRegs;

//-----------------SysCtrl-----------------------

#pragma DATA_SECTION(SysCtrlRegs,"SysCtrlRegsFile");
volatile struct SYS_CTRL_REGS SysCtrlRegs;


#pragma DATA_SECTION(DevEmuRegs,"DevEmuRegsFile");
volatile struct DEV_EMU_REGS DevEmuRegs;

#pragma DATA_SECTION(CsmRegs,"CsmRegsFile");
volatile struct CSM_REGS CsmRegs;

#pragma DATA_SECTION(CsmPwl,"CsmPwlFile");
volatile struct CSM_PWL CsmPwl;


#pragma DATA_SECTION(FlashRegs,"FlashRegsFile");
volatile struct FLASH_REGS FlashRegs;

#if DSP28_F2812
//----------------------------------------
#pragma DATA_SECTION(XintfRegs,"XintfRegsFile");
volatile struct XINTF_REGS XintfRegs;
#endif

//----------------------------------------
#pragma DATA_SECTION(XIntruptRegs,"XIntruptRegsFile");
volatile struct XINTRUPT_REGS XIntruptRegs;



// The following are provided to support alternate notation
// that was used in an early version of the header files

#define ADCRegs       AdcRegs
#define CPUTimer0Regs CpuTimer0Regs
#define CPUTimer1Regs CpuTimer1Regs
#define CPUTimer2Regs CpuTimer2Regs
#define ECANARegs     ECanaRegs
#define ECANAMboxes   ECanaMboxes
#define EVARegs       EvaRegs
#define GPIODataRegs  GpioDataRegs
#define GPIOMuxRegs   GpioMuxRegs
#define MCBSPARegs    McbspaRegs
#define PIECtrlRegs   PieCtrlRegs
#define PIEVectTable  PieVectTable
#define SCIARegs      SciaRegs
#define SCIBRegs      ScibRegs
#define SYSCtrlRegs   SysCtrlRegs
#define DEVEmuRegs    DevEmuRegs
#define CSMRegs       CsmRegs
#define CSMPwl        CsmPwl
#define FLASHRegs     FlashRegs
#define XINTFRegs     XintfRegs
#define XINTRUPTRegs  XIntruptRegs

