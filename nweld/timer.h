#ifndef TIMER_H
    #define TIMER_H

    #ifdef __cplusplus
        extern "C" {
    #endif

    extern Uint16 tmCount;          // timer counter for laser frequency
    extern Uint16 mFlag;            // periodic status for moduRed()

    #ifdef __cplusplus
        }
    #endif /* extern "C" */
#endif /* TIMER_H */
