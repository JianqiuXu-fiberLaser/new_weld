// Design by Jacalt Laser
// Modified by Jianqiu Xu 20210419
//
// delete illegal defination
///////////////////////////////////////////////////////////////////

#ifndef __OLED_H
    #define __OLED_H

    #ifdef __cplusplus
        extern "C" {
    #endif

    //control OLED
    extern void set_position(unsigned char col, unsigned char page);
    extern void write_data(unsigned char byte);

    extern void oled_init(void);
    extern void oled_clear(void);

    extern void oled_drawBMP(unsigned char col, unsigned char page,
                             unsigned char xsize, unsigned char ysize);
    extern void oled_showChinese(unsigned char col, unsigned char page, 
                                 unsigned char xcode);

    #ifdef __cplusplus
        }
    #endif /* extern "C" */

#endif // end of _OLED_H
