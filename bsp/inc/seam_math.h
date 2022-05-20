/**
 * Function: Foundamental mathmatics routin for seam finding.
 *
 *  Modification: add the motor Library
 *  Created on: 20220315
 *      Author: jacalt1008
 *  Ver. 1.0.0
 */
#ifndef BSP_INC_SEAMMATH_H_
#define BSP_INC_SEAMMATH_H_

#include <math.h>
#include <stdlib.h>
#include <stddef.h>

float* vector(long n);
float** matrix(const long nrow, const long ncol);
int* ivector(const long n);
void free_vector(float* v);
void free_ivector(int* v);
void free_matrix(float** m, long nrow, long ncol);

#endif /* BSP_INC_SEAMMATH_H_ */