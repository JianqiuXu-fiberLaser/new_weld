/**
 * @brief maniplate matrix
 * Function: Foundamental mathmatics routins for seam finding.
 *
 *  Modification: add the motor Library
 *  Created on: 20220315
 *      Author: jacalt1008
 *  Ver. 1.0.0
 */

#include "seam_math.h"

#define NR_END 1  // extra storage for representable of ANSI
#define SWAP(a,b) {temp=(a); (a)=(b); (b)=temp;}

/**
* Function: float *vector(long n)
*     allocate a float vector with subscript range v[nl...nh]
* Return: a pointer to the top of memory.
*/
float* vector(long n)
{
    float* v;
    v = (float*)malloc((size_t)(n + NR_END * sizeof(float)));
    if (!v) exit(1);
    return v;
}

/**
* Function: float **matrix()
*     allocate a float matrix with subscript range m[nrl...nrh][ncl...nch]
* Return: a pointer to the top of memory.
*/
float** matrix(const long nrow, const long ncol)
{
    float** m;

    // allocate the pointers to rows
    m = (float**)malloc((size_t)(nrow * sizeof(float*)));

    // allocate rows and set pointers to them.
    m[0] = (float*)malloc((size_t)(nrow * ncol * sizeof(float)));
    if (!m[0]) exit(1);

    for (long i=1; i<nrow; i++)
        m[i] = m[i-1] + ncol;

    return m;
}

/**
* Function: int *ivector()
*     allocate an int vector with subscript range v[nl...nh]
* Return: a pointer to the top of memory.
*/
int* ivector(const long n)
{
    int* v;
    v = (int*)malloc((size_t)((n + NR_END) * sizeof(int)));
    if (!v) exit(1);
    return v;
}

/**
* Function: free_vector()
*     free a float vector allocated with vector()
*/
void free_vector(float* v)
{
    free((char*)v);
}


/**
* Function: free_ivector()
*     free an int vector allocated with ivector()
*/
void free_ivector(int* v)
{
    free((char*)v);
}

/**
* Function: free_matrix()
*     free a float matrix allocated by matrix()
*/
void free_matrix(float** m, long nrow, long ncol)
{
    for (int i=0; i<nrow; i++)
        free((char*)m[i]);
    free((char**)m);
}