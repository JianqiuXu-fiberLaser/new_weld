// Hand-weding control software
//
// Change: 1) Calibrate the current parameters
//         2) renew to the politely TI C-style
//
// Company: Desigened by Jacalt Laser
// Author: Jianqiu Xu
// Date: 20210806
// Verion. 1.1
///////////////////////////////////////////////////////////////////

#include <DSP281x_Device.h>      // DSP281x Header file Include File
#include <DSP281x_Examples.h>    // DSP281x Examples Include File
#include "lstate.h"              // state to operate laser
#include "dac.h"                 // initialize TLV5638
#include "gpio.h"                // alias GPIO 

void init_state();      // initialized system at startup 
void work();            // main loop
Uint16 isKeyFixed();    // avoid error message when the key being touched 
void check_keystate();

// ************** state of trigger key ***************************/
enum KEY_STATE
{
    KEY_OFF = 0,                 // key in switch off
    KEY_PRESS = 1,               // press key to switch on
    KEY_HOLD = 2,                // hold key to switch on
    KEY_RELEASE = 3              // release key to switch off
};

static enum KEY_STATE key_state;

struct ST_KEY
{
    Uint16 nowKey;               // now key state, 0:off; 1: on
    Uint16 preKey;               // previous key state, 0:off; 1:on
};

static struct ST_KEY st_key = {0, 0};

///////////////////////  main Program   ///////////////////////////
void main(void)
{
    InitSysCtrl();          // Initialize DSP system
    InitGpio();             // Initialize GPIO

    // Disable CPU interrupts and clear all CPU interrupt flags:
    DINT;
    IER = 0x0000;           // clear CPU interrupt
    IFR = 0x0000;           // clear CPU interrupt flag

    //---- Initialize PIE
    InitPieCtrl();          // PIE control register to default
    InitPieVectTable();     // initialize PIE vector table

    //---- Initialize peripheral device
    InitCpuTimers();        // Initialize CPU timer
    InitSci();              // Initialize SCIA in Sci.c
    InitSpi();              // Initialize SPI
    InitAdc();              // initialize Adc
    InitEv();               // Initialize event manager in EV.c

    //---- re-mapping vector table
    EALLOW;
    PieVectTable.TINT0  = &TINT0_ISR;      // CPU timer
    PieVectTable.RXAINT = &SCIRXINTA_ISR;  // SCIA read
    PieVectTable.ADCINT = &ADCINT_ISR;     // Adc sampling
    EDIS;

    //---- enable interrupt
    PieCtrlRegs.PIEIER1.bit.INTx7 = 1;     // enable PIE1 CpuTimer 0 interrupt
    PieCtrlRegs.PIEIER1.bit.INTx6 = 1;     // enable PIE1 in-chip ADC INT
    PieCtrlRegs.PIEIER9.bit.INTx1 = 1;     // enable PIE9 SCIA Receive INT

    IER |= (M_INT1 | M_INT9);              // enable CPU interrupt: INT1, INT9

    ConfigCpuTimer(&CpuTimer0,120,10);     // start CPU timer0, 10 us each interrupt

    EINT;                                  // enable global INTM=0(CPU level INT EN bit)
    ERTM;                                  // enable global Debug INT, DBGM=0

    cfg_t5638();                           // setup 3 TLV5638 with Vref=2.048V
    init_state();                          // initalize system state

    while (1)
        work();
}


/******************************************************************
 * Name: void work()
 * Function: main program: check key state
 *     operate the laser according key state
 ******************************************************************/
void work()
{
    check_keystate();
    switch (key_state)
    {
        case KEY_OFF:
            FreeTrig();
            break;

        case KEY_HOLD:
            HoldTrig();
            break;

        case KEY_PRESS:
            PressTrig();
            break;

        case KEY_RELEASE:
            RelaxTrig();
            break;
    }
}


/******************************************************************
 * Name: void init_state()
 * Function: initialize system state when the machine is powered on
 ******************************************************************/
void init_state()
{
    ENABLE = 1;                       // work-in the enable key
    REDLIGHT = 0;                     // close polit
    oled_init();                      // OLED startup

    SciaRegs.SCITXBUF = 0x55;         // startup successful
    SciaRegs.SCITXBUF = 0xaa;
}


/******************************************************************
 * Name: isKeyFixed()
 * Function: check whether the key being pushed
 *     to avoid push error, check the key status three times
 *     each time delay 10 uS
 * Return 1: key is fixed to a certain state
 *        0: key is just kicked
 ******************************************************************/
Uint16 isKeyFixed()
{
    Uint16 k_fixed = 0;
    Uint16 key_sum = 0;

    // check key state three times and add the values
    for (Uint16 i=0; i<3; i++)
    {
        key_sum += KEY;
        DELAY_US(10);
    }

    switch (key_sum)
    {
        case 0:
            st_key.nowKey = 0;
            k_fixed = 1;
            break;

        case 3:
            st_key.nowKey = 1;
            k_fixed = 1;
            break;

        default:
            k_fixed = 0;
    }

    return k_fixed; 
}


/******************************************************************
 * Name: void check_keystate() 
 * Function: check the state of trigger key
 *     depend on the pre-time level and in-time level 
 *     nowKey = 1: on;
 *           preKEY = 1, KEY_HOLD
 *           preKEY = 0, KEY_PRESS 
 *     nowKey = 0: off;
 *           PreKEY = 1, KEY_RELEASE
 *           preKEY = 0, KEY_OFF
 ******************************************************************/
void check_keystate()
{
    if (!isKeyFixed())           // key not fixed
        return;

    if (st_key.nowKey)           // switch on key
        key_state = (st_key.preKey ? KEY_HOLD : KEY_PRESS); 
    else                         // switch off key 
        key_state = (st_key.preKey ? KEY_RELEASE : KEY_OFF);         

    st_key.preKey = st_key.nowKey;
}


//=================================================================
// No more.
//=================================================================

