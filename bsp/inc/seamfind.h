/**
 * seamfind.c
 * Function: Find the seam and then set the motor，
 *   so that the laser can follow the seam.
 *
 *  Created on: 20220315
 *      Author: jacalt1008
 *  Ver. 1.0.0
 *
 *  Modification: add the motor Library
 *  Created on: 20220315
 *      Author: jacalt1008
 *  Ver. 1.2.0
 */
#ifndef BSP_INC_SEAMFIND_H_
#define BSP_INC_SEAMFIND_H_

#include <math.h>
#include <stdlib.h>
#include <stddef.h>

void find_seam(const uint16_t *seam_location);

#endif /* BSP_INC_SEAMFIND_H_ */
