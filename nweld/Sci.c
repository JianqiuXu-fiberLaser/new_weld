// TI File $Revision: /main/2 $
// Checkin $Date: April 29, 2005   11:08:40 $
//###########################################################################
//
// FILE:	DSP281x_Sci.c
//
// TITLE:	DSP281x SCI Initialization & Support Functions.
//
//###########################################################################
// $TI Release: DSP281x C/C++ Header Files V1.20 $
// $Release Date: July 27, 2009 $
//###########################################################################
//
// modified by Jacalt Laser
// Jianqiu Xu, 20210423

#include <DSP281x_Device.h>     // DSP281x Header file Include File
#include <DSP281x_Examples.h>   // DSP281x Examples Include File

//**********    SCI golbel paramenters   **************************
static Uint16 SciaRbuf[16] ={0};       // SCi receive buffer
static Uint16 sciFLAG;      // SCI receive flag, 0=idle, 1=buffer full

struct REV_DATA {
    Uint16 gPower;       // power percent, default 0%
    Uint16 gFreq;        // frequency Hz, 0=CW
    Uint16 gDuty;        // duty cycle, 100%
    Uint16 gRise;        // rising edge (us)
    Uint16 gFall;        // falling edge (us)
};

/******************************************************************
 * Name: void InitSci(void)
 * Function: This function initializes the SCI(s) to a known state.
 *     Ti propotype in DSP281x_Sci.c
 * modified by xu 20210423
 * ***************************************************************/
void InitSci(void)
{
    /*-------------------------------------------------------
     * SciaRegs.SCICTL1.bit.TXENA=1;    // enable transmit
     * SciaRegs.SCICTL1.bit.RXENA=1;    // enable receive
     * SciaRegs.SCICTL1.bit.RXERRINTENA=0; // disable RX error INT
     * ------------------------------------------------------*/
    SciaRegs.SCICTL1.all = 0x23;

    /*------------------------------------------------------
     *  SciaRegs.SCICCR.bit.STOPBITS=0;  // 1 stop
     *  SciaRegs.SCICCR.bit.PARITYENA=0;  // no odd-even check
     *  SciaRegs.SCICCR.bit.LOOPBKENA=0;  // no LoopBack
     *  SciaRegs.SCICCR.bit.ADDRIDLE_MODE=0; // idle mode
     *  SciaRegs.SCICCR.bit.SCICHAR=7;   // whole 8 bit
     *------------------------------------------------------*/
    SciaRegs.SCICCR.all = 0x07;

    SciaRegs.SCICTL2.bit.RXBKINTENA = 1;  // enable RX interrupt

    /*---------------------------------------------------*
     * 115200 is suitable for 60 MHz LSPCLK frequency    *
     * BRR+1 =[LSPCLK/(BUAD*8)]                          *
     * SYSCLK=120MHz, LSPCLK=60MHz, Baud=115.2KHz        *
     * ==> BRR = 64                                      *
     * --------------------------------------------------*/
    SciaRegs.SCIHBAUD = 0x00;
    SciaRegs.SCILBAUD = 0x40;

    // configure TxFIFO
    // Note: enhanced mode for both TX and RX
    SciaRegs.SCIFFTX.bit.SCIFFENA = 1;      // enable enhanced FIFO mode
    SciaRegs.SCIFFTX.bit.TXFFILIL = 0;      // TxFIFO=0 level
    SciaRegs.SCIFFTX.bit.TXFFIENA = 0;      // disable TxFFIVL INT
    SciaRegs.SCIFFTX.bit.TXINTCLR = 1;      // Clear Int Flag
    SciaRegs.SCIFFTX.bit.TXFIFOXRESET = 1;  // Reset TxFIFO
    SciaRegs.SCIFFTX.bit.SCIRST = 1;        // SCI FIFO Reset

    // configure RxFIFO
    SciaRegs.SCIFFRX.bit.RXFFIL = 16;       // RxFIFO=16 level
    SciaRegs.SCIFFRX.bit.RXFFIENA = 1;      // enable RxFIFO INT
    SciaRegs.SCIFFRX.bit.RXFFINTCLR = 1;    // Clear INT flag
    SciaRegs.SCIFFRX.bit.RXFIFORESET = 1;   // Reset RxFIFO
    SciaRegs.SCIFFRX.bit.RXFFOVRCLR = 1;    // clear RXFFOVF flag

    SciaRegs.SCIFFCT.all = 0;               // no delay between frames
    SciaRegs.SCIPRI.all = 0x08;             // free run when breakpoint
    SciaRegs.SCICTL1.bit.SWRESET = 1;       // SCIA reset
}


/******************************************************************
 * Name: Uint16 rsp_ACK()
 * Function: response to receive command
 * Input: SciaRbuf[16]  --Sci global paremter
 * acknowledge: correct = 01 10 00 0A
 *            incorrect = 01 90 00 0A
 *
 * return 1: correct command
 *        0: incorrect command
 *****************************************************************/
Uint16 rsp_ACK()
{
    Uint16 iFrameCorrect = 0;         // correct for receive data

    // data header
    if (    SciaRbuf[0] == 0x01 && SciaRbuf[1] == 0x10
         && SciaRbuf[2] == 0x00 && SciaRbuf[3] == 0x0A)
    {
        Uint16 iCheckSum = 0;
        for (Uint16 j=4; j<=13; j++) iCheckSum += SciaRbuf[j];

        if ((iCheckSum & 0x00FF) == SciaRbuf[15]) iFrameCorrect = 1;
    }

    return iFrameCorrect;
}


/******************************************************************
 * Name: void ack_send(Uint16 ackn)
 * Function: send ack information to SCI port
 * Input: ackn -- ack number
 * acknowledge: 1 for correct = 01 10 00 0A
 *              0 --incorrect = 01 90 00 0A
 *****************************************************************/
void ack_send(Uint16 ackn)
{
    Uint16 SciaTbuf[4] = {0};
    Uint16 i;

    // data header
    SciaTbuf[0]=0x01; 
    switch (ackn)
    {
        case 1:
            SciaTbuf[1]=0x10;
            break;

        case 0:
            SciaTbuf[1]=0x90;
            break;

        default:
            SciaTbuf[1]=0x00;
    }
    SciaTbuf[2]=0x00;
    SciaTbuf[3]=0x0A; 

    // send the message
    for (i=0; i<4; i++) SciaRegs.SCITXBUF = SciaTbuf[i];
}


/******************************************************************
 * Name: decodeSciCmd()
 * Function: decode command to value
 * Input command: SciaRbuf
 *         mach_code   opr_code   data length     
 *     Byte[0]         [1]        [2] [3] 
 *         01h         10h        00h 0ah 
 *    
 *     power(%)  freq(Hz)  Duty(%)   rise(us)   fall(us)  sum_check
 *     [4][5]    [6][7]    [8][9]    [10][11]   [12][13]   [14][15]
 *     PH PL     fH fL     OH OL
 * Call: rsp_ACK()
 *       ack_send()
 * In program: recived data in struct REV_DATA
 *     receive data from HDMI screen for laser operation.
 *****************************************************************/
void decodeSciCmd(struct REV_DATA *revData)
{
    Uint16 ackn;

    // receive command from screen and back ACK
    ackn = rsp_ACK();
    ack_send(ackn);

    if (ackn) 
    {
        // translate received data
        // first assess the data range
        // unsigned int16 > 0, naturally
        revData->gPower = (SciaRbuf[4]<<8) + SciaRbuf[5];
        if(revData->gPower > 100) 
            revData->gPower = 100;

        revData->gFreq = (SciaRbuf[6]<<8) + SciaRbuf[7];
        if (revData->gFreq > 1000) 
            revData->gFreq = 0;

        revData->gDuty = (SciaRbuf[8]<<8) + SciaRbuf[9];
        if (revData->gDuty > 100) 
            revData->gDuty = 100;

        revData->gRise = (SciaRbuf[10]<<8) + SciaRbuf[11];
        if (revData->gRise > 300) 
            revData->gRise = 0;

        revData->gFall = (SciaRbuf[12]<<8) + SciaRbuf[13];
        if (revData->gFall > 300) 
            revData->gFall = 0;
    }
}

void ReceiceData(struct REV_DATA *revData)
{
    if (sciFLAG == 1)
    {
        decodeSciCmd(revData);
        sciFLAG = 0;
    }
            
}

/******************************************************************
 * interrupt void SCIRXINTA_ISR(void)
 * Function: response to SCI-A receive INT
 *     sciFLAG = 1 when have received data
 *****************************************************************/
interrupt void SCIRXINTA_ISR(void)
{
    for (Uint16 i=0; i<16; i++)
        SciaRbuf[i] = SciaRegs.SCIRXBUF.bit.RXDT;

    sciFLAG = 1;

    SciaRegs.SCIFFRX.bit.RXFFOVRCLR = 1;  //clear overflow flag, Xu 20210116
    SciaRegs.SCIFFRX.bit.RXFFINTCLR = 1;  //clear INT flag

    // enable SciRxIntA to PIE9.1
    PieCtrlRegs.PIEACK.bit.ACK9 = 1;
}

//=====================================================================
// No more.
//=====================================================================
