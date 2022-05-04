// Draw hand-welding characters and BMP according to working states
//
// Company: Jacalt Laser
//
// Ver. 0.9
// -- Fundametal draft
// Author: HuaCF
// Date: 20210401
//
// Ver. 1.0.1
// Change: using data struct
// Author: Jianqiu Xu
// Date: 20210419
//
///////////////////////////////////////////////////////////////////

#include "oled.h"

#ifndef WELD_OLED_H
    #define WELD_OLED_H

    #ifdef __cplusplus
        extern "C" {
    #endif

    extern void wk_startup();           // power on
    extern void wk_pilot();             // pilot on
    extern void wk_disable();           // enable key off
    extern void wk_norefl();            // no reflection
    extern void wk_power(Uint16 pert_pow);    // laser power = 0 (close)

    #ifdef __cplusplus
        }
    #endif /* extern "C" */
#endif /* WELD_OLED_H */
