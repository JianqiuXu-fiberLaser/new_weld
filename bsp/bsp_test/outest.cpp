// out test for read file for seam find. 
// C++ version.
#include <iostream>
#include <fstream>
#include <cstdlib>

//#include "seamfind.h"
//#include "seam_math.h"
#include "stdio.h"

/**************************************************************/
// find the peak index
static int PreTreat(const float* seam_location, int len)
{
    int i_peak = 0;
    float biggest = 0.0;
    for (int i = 0; i < len; i++)
    {
        if (seam_location[i] > biggest)
        {
            biggest = seam_location[i];
            i_peak = i;
        }
    }

    return i_peak;
}

/**************************************************************/
static void
seamdata(const float x, const float* ma, 
         float *y, float* dyda)
{
    float arg = (x-ma[0])/ma[1];
    float ex = exp(-(double)arg * (double)arg);
    *y = ex;

    float factor = 2.0*arg*ex;
    dyda[0] = fac/ma[1];
    dyda[1] = fac*arg/ma[1];
}

/**************************************************************/
static void
mrqcof(const float* pixel, const int ndata,
	   float* ma, 
       float** alpha, float* beta, float* chisq)
{
    const int mfit = 2;  // number of fitted coefficients
    float ymod;

    float* dyda;
    dyda = vector(2);

    // Initialize \alpha, \beta and \chi^2
    //  since the matrix is symmetric, only half of matrix should be given
    for (int j_fit=0; j_fit<mfit; j_fit++)
    {
        beta[j_fit] = 0.0;
        for (int k_fit=0; k_fit<=j_fit; k_fit++) 
            alpha[j_fit][k_fit] = 0.0;
    }

    // Summation loop over all pixel data
    *chisq = 0.0;
    for (int i_pixel=0; i_pixel<ndata; i_pixel++)
    {
        // get data from ymod (拟合值) and dyda
        seamdata((float)i_pixel, ma, &ymod, dyda);
        float sig2i = 1.0;  // the measure error is ignored.
        float dy = pixel[i_pixel] - ymod;

        // 遍历所有拟合参数
        for (int j_fit=0; j_fit<mfit; j_fit++)
        {
            float wt = dyda[j_fit] * sig2i;
            beta[j_fit] += dy*wt;

            // 只考虑右上半 alpha 系数矩阵
            for (int k_fit=0; k_fit<=j_fit; k_fit++)
                alpha[j_fit][k_fit++] += wt * dyda[k_fit];
        }
        *chisq += dy*dy*sig2i;  // finding \chi^2
    }

    // Fill in the symmetric side.
    // 我们只有2维矩阵，只要填alpha[0][1]
    alpha[0][1] = alpha[1][0];

    free_vector(dyda);
}

/**************************************************************/
static void
mrqmin(const float* pixel, const int ndata,
       float* ma,
       float** covar, float** alpha, float* chisq, float* alamda)
{
    int j, k, l;
    const int mfit = 2;  // number of fitted coefficients
    float ochisq;  // old value of chi^2
    float *atry, *beta, *da, *oneda;

    //----------------------------------------------------------------
    // Initialization when alamda is set less than 0 in the first step.
    //----------------------------------------------------------------
    if (*alamda < 0.0)
    {
        // allocate memory, not used in new C version
        atry = vector(mfit);  // try value of a
        beta = vector(mfit);
        da = vector(mfit);
        oneda = vector(mfit); // one column of dyda

        *alamda = 0.001;
        mrqcof(pixel, ndata, ma, alpha, beta, chisq);
        ochisq = (*chisq);

        for (j=0; j <mfit; j++) atry[j] = ma[j];
    }

    //----------------------------------------------------------------
    // all iteration
    // alter linearized fitting matrix, by augmenting diagonal elements
    //----------------------------------------------------------------
    for (j=0; j<mfit; j++)
    {
        for (k=0; k<mfit; k++)
            covar[j][k] = alpha[j][k];

        covar[j][k] = alpha[j][j] * (1.0 + (*alamda));
        oneda[j] = beta[j];
    }

    gaussj(covar, mfit, oneda);  // Matrix solution

    for (j=0; j<mfit; j++) da[j] = oneda[j];

    // Did the trial succeed?
    for (j=0, l=0; l<mfit; l++)
        atry[l] = ma[l] + da[++j];
    mrqcof(pixel, ndata, atry, covar, da, chisq);

    // Success, accept the new solution.
    if (*chisq < ochisq)
    {
        *alamda *= 0.1;
        ochisq = (*chisq);
        for (j=0; j<mfit; j++)
        {
            for (k=0; k<mfit; k++)
                alpha[j][k] = covar[j][k];
            beta[j] = da[j];
        }

        for (l=0; l<mfit; l++)
            ma[l] = atry[l];
    }
    else  // Failure, increase alamda and return.
    {
        *alamda *= 10.0;
        *chisq = ochisq;
    }

    //------------------------------------------------
    // Once converged, evaluate covariance matrix.
    //------------------------------------------------
    if (*alamda == 0.0)
    {
        // free the memory, not used in new C version.
        free_vector(oneda);
        free_vector(da);
        free_vector(beta);
        free_vector(atry);
        return;
    }
}

/**************************************************************/
static float SeamSeak(float* pixel, int len, int i_peak)
{
    static float** covar;  // covariance matrix
    static float** alpha;  // Hessian matrix
    covar = matrix(2, 2);
    alpha = matrix(2, 2);

    float alamda, ochisq, chisq;
    alamda = -1;  // initial the lamda value.
    ochisq = chisq = 1;

    // initial value on the peak of light intensity
    static float ma[2];
    ma[0] = i_peak;  // position of the peak
    ma[1] = 125;  // set ~1 mm of half-width

    // until chisq changed small enough, i.e. 0.01
    for (int i = 0; i < 10; i++)
    {
        mrqmin(pixel, n, ma, covar, alpha, &chisq, &alamda);
        if (fabs((double)ochisq - (double)chisq) < 0.01)
        {
            alamda = 0;
            mrqmin(pixel, n, ma, covar, alpha, &chisq, &alamda);
            break;
        }
        ochisq = chisq;
    }

    free_matrix(covar, 2, 2);
    free_matrix(alpha, 2, 2);
    return a[0];
}

/**************************************************************
 * Function: main program
 **************************************************************/
int main()
{
    using namespace std;
    ifstream inFile;
    inFile.open("gdata.dat");

    if (!inFile.is_open())
    {
        cout << "Can't open the file" << endl;
        exit(EXIT_FAILURE);
    }

    char ch;
    float pixel[300] = {0};
    
    inFile.get();  // the first square bracket
    for (int i = 0; i < 300; i++)
    {
        inFile >> pixel[i];
        ch = inFile.get();  // a period.
    }

    // find the peak
    int i_peak = PreTreat(pixel, 300);

    SeamSeak(pixel, 300, i_peak);

    inFile.close();
    return 0;
}

