/**
 * @brief Seam finding through peak simulation
 *    test in standalone for seam finding
 *
 * Company: Jacalt Laser Corp.
 *  Date: 20220520
 *  Author: J. Xu
 *  Version: 1.0.0
 */

#include "seamfind.h"
#include "seam_math.h"
#include "stdio.h"
// out test for read file for seam find. 
// C++ version.
#include <iostream>
#include <fstream>
#include <cstdlib>

#define PIX_NUMBER 3648
#define SWAP(a,b) {temp=(a); (a)=(b); (b)=temp;}

/******************************************************************************
 *                Pre-treat the raw data of CCD                               *
 ******************************************************************************/

/**
 * Function: filter the black pixel in raw data
 * @param input, seam_location: pointer of the raw data
 * @param output, light_data: pointer of the valid data, output
 *        output,       ypos: position of pixel
 * @retval: the size of valid data
 */
static int 
pretreat(const int *seam_location)
{
    int i, j;
    // the maximum is 4096 for 12 bits CCD
    // reverts the value of pixel brightness, 
    // since the seam corresponding to a valley of brigthness
    const int MaxBrigth = 4096;
    for (i=0; i<PIX_NUMBER; i++)
        *seam_location = MaxBrigth - *seam_location;

    int peak = 0; 
    int i_peak = lightPeak(seam_location, &peak);

	// pick the useful data, which brightness is larger than that of 0.1*peak
    // Note, 1/e^2 ~ 0.13
    int start_pixel, end_pixel;
    
    // make sure the finding of edge be stopped in the array range. 
    seam_location[0] = seam_location[PIX_NUMBER-1] = 0;
	int i = j = i_peak;
    while (seam_location[i] > 0.1*peak)
        i--;
    while (seam_location[j] > 0.1*peak)
        j++;
    
    start_pixel = i;
    end_pixel = j;

    // 将有用的值存入输出数组
    for (i=start_pixel; i<=end_pixel; i++)
    {
        *(++ligh_data) = seam_location[i];
		*(++ypos) = i;
    }

	return (end_pixel - start_pixel + 1);
}

/******************************************************************************
 *               mathematics of approximation method                          *
 ******************************************************************************/

/**
 * Function: void gaussj()
 *   Solution of matrix by Gauss-Jordan elimination.
 *   只针对右边1组系数的求解。
 *     a[1...n][1...n] * x[1...n]'= b[1...n]'
 *
 * @param a[1...n][1...n]: input matrix.
 * @param b[1...n]: the right hand side vector.
 *
 * @retval a: 输出逆矩阵 
 *         b: 输出解向量
 */
static int 
gaussj(float** a, const int n, float* b)
{
    float temp;

    // 整形数组 indxr, indxc 用来保存主元的角标
    // ipiv 用来保存每行的最大元素
    int *indxc, *indxr, *ipiv;
    indxc = ivector(n);  // column of the pivot element
    indxr = ivector(n);  // row of the pivot element
    ipiv = ivector(n);   // 主元检查标志。处理过的每一列，就标注一下。

    for (int row=0; row<n; row++) 
        ipiv[row] = 0;

    //---------------------------------------------------------------
    // 主循环，一列一列的处理
    //---------------------------------------------------------------
    int icol=0, irow=0;
    for (int column=0; column<n; column++)
    {
        //-----------------------------------------------------------
        // 整个矩阵循环，找出剩余单元中的最大值，并把它作为主元存入big
        // 记录big的角标 irow 和 icol，作为这一轮的处理列/行
        //   由于采用了ipiv进行标记，所以被处理过的列/行将不再处理。
        //   采用标记的办法，而不是互换元素，极大地提高了效率。
        //   ipiv=0: 没有处理，=1: 正常处理，>1：异常
        //-----------------------------------------------------------
        float big = 0.0;
        for (int row=0; row<n; row++)
        {
            if (ipiv[row] != 1)  // 只处理剩余的行
            {
                for (int k_column=0; k_column<n; k_column++)
                {
                    if (ipiv[k_column] == 0)  // 只处理剩余的列
                    {
                        if (fabs(a[row][k_column]) >= big)
                        {
                            big = fabs(a[row][k_column]);
                            irow = row;
                            icol = k_column;
                        }
                    }
                    else if (ipiv[k_column] > 1) 
                        return 1;  // 此轮没有找到最大值，原来的ipiv被加1: 01
                }
            }
        }

        ++(ipiv[icol]);  // ipiv正常情况下 =1

        // 记录主元位置
        indxr[column] = irow;
        indxc[column] = icol;

        // 把irow行的主元换到对角位置，相应地该行更换其它元素
        if (irow != icol)
        {
            SWAP(b[irow], b[icol]);
            for (int l_column=0; l_column<n; l_column++)
                SWAP(a[irow][l_column], a[icol][l_column]);
        }

        if (a[icol][icol] == 0) return 2;  // 主元为零: 02

        //----------------------------------------------
        //  对该行的所有元素，除以主元（主元直接设为1）
        //----------------------------------------------
        float pivinv = 1.0 / a[icol][icol];
        a[icol][icol] = 1.0;

        for (int l_column=0; l_column<n; l_column++)
            a[icol][l_column] *= pivinv;
        b[icol] *= pivinv;

        //-----------------------------------------------------
        // 对其它行的元素，减去主元行的元素（需要乘以对应系数）
        // 其它行的、整队主元的元素设为0
        //-----------------------------------------------------
        for (int ll_row=0; ll_row<n; ll_row++)
        {
            if (ll_row != icol)
            {
                float dum = a[ll_row][icol];
                a[ll_row][icol] = 0.0;

                b[ll_row] -= b[icol] * dum;
                for (int l_column=0; l_column<n; l_column++)
                    a[ll_row][l_column] -= a[icol][l_column] * dum;
            }
        }
    }

    //---------------------------------------------------------------
    // 主循环结束，互换对应的列元素，将其变成三角矩阵
    // 由于在主循环中，并没有实际交换主元素，只是采用了更换角标的方式
    // 因此需要将系数矩阵的对角元素与实际进行处理的元素进行互换
    // 得到实际的逆系数矩阵
    //---------------------------------------------------------------
    for (int l_column=n-1; l_column>=0; l_column--)
    {
        if (indxr[l_column] != indxc[l_column])
        {
            for (int k_row=0; k_row<n; k_row++)
                SWAP(a[k_row][indxr[l_column]], a[k_row][indxc[l_column]]);
        }
    }

    free_ivector(ipiv);
    free_ivector(indxr);
    free_ivector(indxc);
    return 0;
}


/**
 * Function: void seamdata()
 *     the derivation of fitting data to calculate the \chisq.
 *       y = exp[-(x-a[0]/a[1])]^2
 *       where a[0] is the central and a[1] the half-width.
 *
 * Input: y = f(x,ma), x is position, and ma is the fit coefficients
 * @output dyda: derivation to the coefficients
 *            y: the value of Gassian function calculated at the measure position.
 */
static void
seamdata(const float x, const float ma[], 
         float *y, float dyda[])
{
    float arg = (x-ma[0])/ma[1];
    float ex = exp(-(double)arg * (double)arg);
    *y = ex;

    float factor = 2.0*arg*ex;
    dyda[0] = fac/ma[1];
    dyda[1] = fac*arg/ma[1];
}


/**
 * Function: void mrqcof
 *     evaluate the coefficient for mrqmin()
 *     Note: Specified for Gaussian function fitting.
 * Called: by mrqmin()
 *
 * Input: x[], measured position
 *        y[], measured CCD value
 *      ndata, the number of measure data
 *       ma[], the fitted coefficients, where ma[0] is the central position
 *             ma[1] is the half-width of Guassian function.
 * Output: alpha, the Hessian matrix * 1/2
 *          beta, the Jacobian matrix, the gradient
 *         chisq, the \chi^2 coefficient
 */
static void
mrqcof(const float x[], const float y[], const int ndata,
	   float ma[], 
       float** alpha, float beta[], float* chisq)
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
        seamdata(x[i_pixel], ma, &ymod, dyda);
        float sig2i = 1.0;  // the measure error is ignored.
        float dy = y[i_pixel] - ymod;

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


/**
 * Function: void mrqimin()
 *
 *     Levenberg-Marquardt method. >> For Gaussian fitting only.
 *     One iteration of LM method. This routine need be called repeatedly
 *     until convergence is achieved.
 *
 * Input: attempting to reduce the value \chi^2 of a fit between a set of
 *        data point x[1...ndata], y[1...ndata],
 *        and a nonlinear function dependent on coefficients of ma[1,2].
 *
 * covar and alpha: working space
 *        The arrays covar[1,2][1,2], alpha[1,2][1,2], are used as working
 *        space during most iterations.
 *
 * lambda: operation index
 *        On the first call provide an initial guess for the parameters ma, 
 *        and set alamda<0 for initialization (which then sets alamda=.001).
 *        If a step succeeds, \chisq becomes smaller and \alamda decreases
 *        by a factor of 10.
 *        If a step fails, alamda grows by a factor of 10. You MUST call 
 *        this routine repeatedly until convergence is achieved.
 *
 *        Then, make one final call with \alamda=0, so that covar[1,2][1,2]
 *        returns the covariance matrix, and \alpha the curvature matrix.
 *
 * output: the program returns current best-fit values for the parameters
 *        ma[1,2], and \chi^2 = chisq.
 */
static void 
mrqmin(const float x[], const float y[], const int ndata,
       float ma[],
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
        mrqcof(x, y, ndata, ma, alpha, beta, chisq);
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
    mrqcof(x, y, ndata, atry, covar, da, chisq);

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

/******************************************************************************
 *                find seam and move the step motor                           *
 ******************************************************************************/

/**
 * Function: float seekSeam()
 *   Find the center from the CCD data with iteraion to enough small \chisq
 * Input: measured data
 *     x: position
 *     y: light intensity
 *     n: the number of data
 * Return: the position of seam
 */
static float seekSeam(float* x, float* y, int n)
{
    float alamda, ochisq, chisq;
    static float ma[2];
    static float** covar;  // covariance matrix
    static float** alpha;  // Hessian matrix

    covar = matrix(2, 2);
    alpha = matrix(2, 2);

    alamda = -1;  // initial the lamda value.
    ochisq = chisq = 1;

    // initial value on the peak of light intensity
    ma[0] = lightPeak(x, y);  // position of the peak
    ma[1] = 125;  // set ~1 mm of half-width

    // until chisq changed small enough, i.e. 0.01
    for (int i = 0; i < 10; i++)
    {
        mrqmin(x, y, n, ma, covar, alpha, &chisq, &alamda);
        if (fabs((double)ochisq - (double)chisq) < 0.01)
        {
            alamda = 0;
            mrqmin(x, y, n, ma, covar, alpha, &chisq, &alamda);
            break;
        }
        ochisq = chisq;
    }

    free_matrix(covar, 2, 2);
    free_matrix(alpha, 2, 2);
    return a[0];
}


/**
 * Function: main program
 */
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
    float pixel[300];
    for (int i = 0; i < 300; i++)
    {
        inFile >> pixel[i];
        ch = inFile.get();  // a period.
    }

    // find the peak
    int i_peak = pretreat(pixel);

    inFile.close();
    return 0;
}


