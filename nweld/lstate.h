// complete it according to lstate.c
//
// Company: Jacalt Laser
// Author: jianqiu Xu 
// Date: 20210419
///////////////////////////////////////////////////////////////////

#ifndef LSTATE_H
    #define LSTATE_H

    #ifdef __cplusplus
        extern "C" {
    #endif

    //-- laser state
    extern void FreeTrig(void);
    extern void HoldTrig(void);
    extern void PressTrig(void);
    extern void RelaxTrig(void);

    #ifdef __cplusplus
        }
    #endif /* extern "C" */
#endif  // end of LSTATE_H definition

//=================================================================
// End of file
//=================================================================

