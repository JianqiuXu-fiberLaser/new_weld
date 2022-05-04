// Basic function for operate OLED  
//
// Ver 1.0.1
// Change: Delete illegal defination
//
// Company: Jacalt Laser
// Author: Jianqiu Xu
// Deta: 20210419
///////////////////////////////////////////////////////////////////


#include <DSP281x_Device.h>     // DSP281x Headerfile Include File
#include <DSP281x_Examples.h>   // DSP281x Examples Include File
#include "font.h"

#define SCL GpioDataRegs.GPADAT.bit.GPIOA0
#define SDA GpioDataRegs.GPADAT.bit.GPIOA1

/******************************************************************
 * OLED 096 graphical RAM (64*128) is divided into 8 pages:
 * [page0] 0 1 2 3 ... 127
 * [page1] 0 1 2 3 ... 127
 * ...     ...     ...
 * [page7] 0 1 2 3 ... 127
 *
 * One page contains 8 rows represented by D0, D1, ..., D7
 *  of one bite. 
 * Each row with 1 bites represent 8 pixs 
 *****************************************************************/

// standard I2C transmit rate is 100 kb/s = 800 kpbs
// half cycle = delay 0.625 us/2
#define IIC_DELAY  DELAY_US(0.625)
#define HALF_DELAY DELAY_US(0.312)

//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
// Data format for communication to OLED
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/******************************************************************
 * Name: void iic_start()
 * Function: the START signal to transmit data (commnunication)
 *     SDA from HIGH to LOW
 *****************************************************************/
void iic_start(void)
{
    SDA = 1;
    SCL = 1;

    HALF_DELAY;
    SDA = 0;                          // SDA jump to LOW
    HALF_DELAY;

    SCL = 0;
    IIC_DELAY;
}


/******************************************************************
 * Name: void iic_stop()
 * Function: the START signal to transmit data
 *     SDA from LOW to HIGH
 *****************************************************************/
void iic_stop(void)
{
    SDA = 0;
    SCL = 1;

    HALF_DELAY;
    SDA = 1;                          // stop all communication

    HALF_DELAY;
    SCL = 0;
}


/******************************************************************
 * Name: void iic_ack() 
 * Function: the acknowledge bit
 *     hold SDA to high in one cycle
 *****************************************************************/
void iic_ack(void)
{
    SDA=1;
    SCL=1;
    IIC_DELAY;

    SCL=0;
    IIC_DELAY;
}


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
// basic function to operate OLED
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

/******************************************************************
 * Name: void send_byte(unsigned char) 
 * Function: write 1 byte to GDDRAM
 *     MSB first, this is normal send direction
 * Input: unsigned char is 16 bit, but only lower 8 bit is used
 *****************************************************************/
void send_byte(unsigned char byte)
{
    // transmit lower 8 bit
    for (Uint16 i=0; i<8; i++)
     {
        SCL = 0;
        (byte & 0x80)?(SDA=1):(SDA=0);    // send the MSB
        HALF_DELAY;

        SCL = 1;                          // write to OLED
        IIC_DELAY;
        SCL = 0;
        HALF_DELAY;

        byte<<=1;                         // byte left shift 1 bit
    }
}


/******************************************************************
 * Name: void send_byte_in(unsigned char)
 * Function: write 1 byte to GDDRAM in inversion direction
 *     LSB first
 * Input: unsigned char is 16 bit, but only lower 8 bit is used
 *****************************************************************/
void send_byte_inv(unsigned char byte)
{
    // transmit lower 8 bit
    for (Uint16 i=0; i<8; i++) 
    {
        SCL = 0;
        (byte & 0x01)?(SDA=1):(SDA=0);    // send the LSB
        HALF_DELAY;

        SCL = 1;                          // write to OLED
        IIC_DELAY;
        SCL = 0;
        HALF_DELAY;

        byte>>=1;                         // byte right shift 1 bit
    }
}


/******************************************************************
 * Name: void com_header()
 * Function: communication header to OLED
 *     every time, communication is start with header
 *     slave address 01111000 = 0x78
 *     SA0=0, W=0
 *****************************************************************/
void com_header(void)
{
    iic_start();
    send_byte(0x78);                  // SA0=0, W=0
    iic_ack();
}


/******************************************************************
 * Name: void write_cmd(unsigned char)
 * Function: write command to OLED
 *     this function followed to com_header if it is first command
 * Input: unsigned char is 16 bit, but only lower 8 bit is used
 *****************************************************************/
void write_cmd(unsigned char byte)
{
    com_header();

    send_byte(0x00);                  // control for command
    iic_ack();
    send_byte(byte);                  // command
    iic_ack();

    iic_stop();
}


/*****************************************************************
 * Name: void write_data(unsigned char)
 * Function: write data to OLED
 *     this function followed to com_header if it is first command
 *     bit: 0=black, 1=brigth
 * Input: unsigned char is 16 bit, but only lower 8 bit is used
 ****************************************************************/
void write_data(unsigned char byte)
{
    com_header();

    send_byte(0x40);                  // control for data
    iic_ack();
    send_byte(byte);                  // data
    iic_ack();

    iic_stop();
}


/******************************************************************
 * Name: void write_data_inv(unsigned char)
 * Function: write data to OLED in the inversion direction
 *     this function followed to com_header if it is first command
 *     bit: 0=black, 1=brigth
 * Input: unsigned char is 16 bit, but only lower 8 bit is used
 *****************************************************************/
void write_data_inv(unsigned char byte)
{
    com_header();

    send_byte(0x40);                  // control for data
    iic_ack();
    send_byte_inv(byte);              // send data inversely
    iic_ack();

    iic_stop();
}


/******************************************************************
 * Name:  void set_position(unsigned char, unsigned char)
 * Function: set position under page addressing mode (default)
 *     column and page, only lower 8 bit
 * Input: column, page
 *****************************************************************/
void set_position(unsigned char col, unsigned char page)
{ 
    write_cmd(0xb0 + page);             // start page
    write_cmd(col & 0x0f);              // start column
    write_cmd(((col & 0xf0)>>4)|0x10);  // end column
}


//>>>>>>>>>>>>>>>>>>>>>>>>
// API for display on OLED
//>>>>>>>>>>>>>>>>>>>>>>>>

/*****************************************************************
 * Name: void oled_on(void)
 * Function: switch OLED on after initial
 *     remapping mode for yellow dots on the upper half part
 ****************************************************************/
void oled_on(void)
{
    write_cmd(0X8D);                  // setup DC-DC enable
    write_cmd(0X14);                  // DC-DC ON
    write_cmd(0XAF);                  // turn on oled

    write_cmd(0xC8);                  // remapping mode
    write_cmd(0xA1);
}


/******************************************************************
 * Name: void oled_off(void)
 * Function: switch off OLED
 *****************************************************************/
void oled_off(void)
{
    write_cmd(0x8D);
    write_cmd(0x10);
    write_cmd(0xAE);
}

/******************************************************************
 * Name: void oled_clear(void)
 * Function: clear OLED display to black screen
 *****************************************************************/
void oled_clear(void)
{
	for (Uint16 i=0; i<8; i++)
	{
	    set_position(0, i);
		for (Uint16 n=0; n<128; n++) write_data(0);
	}
}

/******************************************************************
 * Name: void oled_init(void)
 * Function: initialize OLED
 *****************************************************************/
void oled_init(void)
{
    oled_on();
    oled_clear();
}

/******************************************************************
 * Name: void oled_showChinses(unsigned char, unsigned char,
 *                             unsigned char)
 * Function: show Chinese character on OLED
 *     16x16 dot matrix each character
 *     2 characters
 * Input: columne position
 *        page position
 *        character array index 
 *****************************************************************/
void oled_showChinese(unsigned char col, unsigned char page,
                      unsigned char xcode)
{
    // first character
    set_position(col, page);
    for (Uint16 i=0; i<16; i++) 
        write_data(CNch[xcode][i]);   // 16x16 character in CNch bank

    // second character
    // page+1 when arrive the end of columun of 16 time writing
    set_position(col, ++page);
    for (Uint16 i=0; i<16; i++)
        write_data(CNch[xcode][16+i]);    // 16x16 character in CNch bank
}


/******************************************************************
 * Name: void oled_drawBMP(unsigned char, unsigned char, 
 *                         unsigned char,
 *                         unsigned char)
 * Function: draw BMP picture in x*y dot
 *     Because BMP only give the upper half part
 *     we need draw twice, one in the right direction and other
 *     in inverse direction.
 * Input: column position
 *        page position
 *        BMP data index
 *****************************************************************/
void oled_drawBMP(unsigned char col, unsigned char page,
                  unsigned char xsize, unsigned char ysize)
{
    set_position(col, page);
	for (Uint16 j=0; j<xsize; j++) write_data(BMP[j]);

	set_position(col, page+1);
	for (Uint16 j=0; j<xsize; j++) write_data_inv(BMP[j]);
}


//=================================================================
// No more.
//=================================================================

