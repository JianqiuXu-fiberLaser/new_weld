/**
 * Function: test in standalone for seam finding
 *     mostly for curve fitting.
 *
 *  Created on: Jacalt Laser Corp. 20220520
 *      Author: J. Xu
 *  Ver. 1.0.0
 */
#ifndef SEAMMATH_H_
#define SEAMMATH_H_

#include <math.h>
#include <stdlib.h>
#include <stddef.h>

float* vector(long n);
float** matrix(const long nrow, const long ncol);
int* ivector(const long n);
void free_vector(float* v);
void free_ivector(int* v);
void free_matrix(float** m, long nrow, long ncol);

#endif /* SEAMMATH_H_ */