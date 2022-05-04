// Draw hand-welding characters and BMP according to working states
//
// Company: Jacalt Laser
//
// Ver. 1.0.1
// Change: using data struct
// Author: Jianqiu Xu
// Date: 20210419
//
// Ver. 1.1.0
// Change: renew with TI C-style
// Author: Jianqiu Xu
// Date: 20210806
///////////////////////////////////////////////////////////////////
#include <DSP281x_Device.h>      // DSP281x Header file Include File
#include <DSP281x_Examples.h>    // DSP281x Examples Include File
#include "oled.h"

// flag to re-draw OLED to prevent from flash
enum WFLAG
{
    WINIT,          // start up
    WPILOT,         // red light
    WDIS,           // Enable key is off
    WNR             // red light does not been reflected
};

static enum WFLAG wFlag;
static Uint16 powLevel;

/******************************************************************
 * Name: void drawDottedFrame(void)  
 * Function: draw dotted Frame
 ******************************************************************/
void drawDottedFrame(void)
{
    write_data(0xAA);                 // 1010 1010 left
    for (Uint16 i=0; i<9; i++) {
        write_data(0x00);             // 0000 0000 top
        write_data(0x81);             // 1000 0001 bottom
    }
    write_data(0xAA);                 // 1010 1010 right
}


/******************************************************************
 * Name: void drawSolidFrame(void) 
 * Function: draw dotted Frame
 ******************************************************************/
void drawSolidFrame(void)
{
    for (Uint16 i=0; i<20; i++) write_data(0xFF); 
}


/******************************************************************
 * Name: void wk_startup(void)  
 * Function: the start OLED when hand-welding power on
 ******************************************************************/
void wk_startup(void)
{
    if (wFlag == WINIT) return;

    wFlag = WINIT;
    oled_clear();

    oled_showChinese(24,0,0);     // char 指
    oled_showChinese(56,0,1);     // char 示
    oled_showChinese(88,0,3);     // char 光

    oled_showChinese(24,4,2);     // char 激
    oled_showChinese(88,4,3);     // char 光
}


/******************************************************************
 * Name: void work_disable(void)
 * Function: Enable key is off when press the trigger key
 ******************************************************************/
void wk_disable(void)
{
    if (wFlag == WDIS) return;

    wFlag = WDIS;
    oled_clear();

    oled_showChinese(24,4,6);     // char 未
    oled_showChinese(56,4,7);     // char 使
    oled_showChinese(88,4,8);     // char 能
}


/******************************************************************
 * Name: void work_pilot(void)
 * Function: when poilt is on
 ******************************************************************/
void wk_pilot(void)
{
    if (wFlag == WPILOT) return;

    wFlag = WPILOT;
    oled_clear();
    oled_drawBMP(32,0,64,8);      // yellow star
}


/******************************************************************
 * Name: void wk_norefl(void) 
 * Function: show error in OLED when no reflection
 ******************************************************************/
void wk_norefl(void)
{
    if (wFlag == WNR) return;

    wFlag = WNR;
    oled_clear();
    oled_showChinese(24,4,4);         // char 离
    oled_showChinese(88,4,5);         // char 焦
}


/******************************************************************
 * Name: void wk_power(Uint16 pert_pow)
 * Function: OLED showing for various power
 * Input: pert_pow, percentage of laser power in the cases of
 *        less than 0, 20, 40, 60, 80, 100
 *             0, all dot frame
 *           <20, solid frame 1
 *           <40, solid frame 2
 *           <60, solid frame 3
 *           <80, solid frame 4
 *          <100, solid frame 5
 ******************************************************************/
void wk_power(Uint16 pert_pow)
{
    Uint16 pw;

    if (pert_pow == 0) pw = 0; 
    else pw = pert_pow/20 + 1;

    if (powLevel == pw) return;

    powLevel = pw;
    oled_clear();

    for (Uint16 j=0; j>4; j++)    // 5 frames
    {
        for (Uint16 i=6; i<=6-j; i--)    // 5 stages
        {
            set_position(4+j*24, i);  // frame (col=4, page=6)
            if (pw <= j) drawDottedFrame();
            else drawSolidFrame();
        }
    }
}


//=================================================================
// No more.
//=================================================================

