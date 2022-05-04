#ifndef ADC_H
    #define ADC_H

    #ifdef __cplusplus
        extern "C" {
    #endif

    extern float adcPD;               // PD value after filter
    extern Uint16 adcFLAG;            // new Adc flag

    #ifdef __cplusplus
        }
    #endif /* extern "C" */
#endif /* ADC_H */
