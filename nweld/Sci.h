// complete it according Sci.c
//
// Company: Jacalt Laser
// Author: Jianqiu Xu
// Date: 20210419
//
///////////////////////////////////////////////////////////////////

#ifndef SCI_H
    #define SCI_H

    #ifdef __cplusplus
        extern "C" {
    #endif

    struct REV_DATA {
        Uint16 gPower;         // power percent, default 0%
        Uint16 gFreq;          // frequency Hz, 0=CW
        Uint16 gDuty;          // duty cycle, 100%
        Uint16 gRise;          // rising edge (us)
        Uint16 gFall;          // falling edge (us)
    };

    extern void ReceiveData(struct REV_DATA *revData);

    #ifdef __cplusplus
        }
    #endif /* extern "C" */
#endif /* SCI_H */
