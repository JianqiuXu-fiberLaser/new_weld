/**
 * seamfind.c
 * Function: test in standalone for seam finding
 *   so that the laser can follow the seam.
 *
 *  Created on: Jacalt Laser Corp. 20220520
 *      Author: J.Xu
 *  Ver. 1.0.0
 */
#ifndef SEAMFIND_H_
#define SEAMFIND_H_

#include <math.h>
#include <stdlib.h>
#include <stddef.h>

void find_seam(const int *seam_location);

#endif /* SEAMFIND_H_ */
