#ifndef GPIO_H
    #define GPIO_H

    #ifdef __cplusplus
        extern "C" {
    #endif

    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // ENABLE: GPIO for enable signal out
    //     connection EN ==> the enable key ==> CK_EN
    //     1 = hight level
    //     0 = low level    
    // CK_EN: read ENABLE state
    //     compare to ENABLE to detect the enable key state
    //
    // KEY: gun trigger connection with OC gate, 0=off; 1=on;
    //      read KEY to detect the gun trigger state
    //
    // REDLIGHT: control the switch on/off of the red light
    //         = 1, red lighting
    //         = 0, red close
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    #define ENABLE        GpioDataRegs.GPBDAT.bit.GPIOB0
    #define CK_EN         GpioDataRegs.GPBDAT.bit.GPIOB2
    #define KEY           GpioDataRegs.GPBDAT.bit.GPIOB3
    #define REDLIGHT      GpioDataRegs.GPBDAT.bit.GPIOB1

    #ifdef __cplusplus
        }
    #endif /* extern "C" */
#endif /* GPIO_H */
