//  Ver 0.9
//  Author: HuaCF
//  Date: 20210330
//
// Ver 1.0
// Change: define new function wr_2chip()
//         confirm I/O
//         complete comments
// Author: Jianqiu Xu
// Date: 20210429
//
// Ver 1.1
// Change: renew for TI C-style
// Author: Jiangiu Xu
// Date: 20210816
///////////////////////////////////////////////////////////////////

#ifndef DAC_H_
    #define DAC_H_

    #ifdef __cplusplus
        extern "C" {
    #endif

    extern void cfg_t5638(void);              // dac.c
    extern void wr_2chip(Uint16 wr_data);     // Xu 20210419

    #ifdef __cplusplus
        }
    #endif /* extern "C" */
#endif /* DAC_H_ */
