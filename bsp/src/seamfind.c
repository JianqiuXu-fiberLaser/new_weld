/**
 * @brief Seam finding through peak simulation
 * @device STM32F4xx M4 freeRTOS
 *
 * Function: Find the seam and then set the motor，
 *   so that the laser can follow the seam.
 *
 * Company: Jacalt Laser Corp.
 *  Date: 20220315
 *  Author: J. Xu
 *  Version: 1.0.0
 */

#include "seamfind.h"
#include "seam_math.h"
#include "stepmotor.h"

#define PIX_NUMBER 3648
#define SWAP(a,b) {temp=(a); (a)=(b); (b)=temp;}

// the pitch of X-axle is 1 mm and one pulse move the step motor
// along the axle with 1.8 degree.
// Thus, one pulse move the stage 1*1.8/360 = 0.005 mm along the X direction
const float PULDIS = 0.005;

/**
* Function: float lightPeak()
*     find the peak intensity with the bubble sort method
* @param input, seam_location: pointer of the raw data
* @param output, peak: intensity
* @retval output, i_peak: position of peak
*/
static uint16_t 
lightPeak(const uint16_t *seam_location, uint16_t *peak)
{
	uint16_t i_peak;

    *peak = 0;
	for (uint16_t i=0; i<PIX_NUMBER; i++)
	{
		if (seam_location[i] > *peak)
			*peak = seam_location[i];
            i_peak = i;
	}

    return i_peak;
}


/**
 * Function: filter the black pixel in raw data
 * @param input, seam_location: pointer of the raw data
 * @param output, light_data: pointer of the valid data, output
 *        output,       ypos: position of pixel
 * @retval: the size of valid data
 */
static uint16_t 
pretreat(const uint16_t *seam_location,
         uint16_t *light_data, uint16_t *ypos)
{
    // the maximum is 4096 for 12 bits CCD
    // reverts the value of pixel brightness, 
    // since the seam corresponding to a valley of brigthness
    const uint16_t MaxBrigth = 4096;
    for (uint16_t i=0; i<PIX_NUMBER; i++)
        *seam_location = MaxBrigth - *seam_location;

    uint16_t peak = 0; 
    uint16_t i_peak = lightPeak(seam_location, &peak);

	// pick the useful data, which brightness is larger than that of 0.1*peak
    // Note, 1/e^2 ~ 0.13
    uint16_t start_pixel, end_pixel;
	uint16_t i = i_peak;
    uint8_t arr_edge = 0;  // arrive the both edges
    while (seam_location[i] < 0.1*peak)
        i--;
    start_pixel = i;

    i = i_peak;
    while (seam_location[i] < 0.1*peak)
        i++;
    end_pixel = i;

        *(++ligh_data) = seam_location[i];


	while(arr_edge == 1)  
	{
		if (seam_location[i] > 0.1*peak)
        {
            start_pixel = i;
		    i++;
        }
	}

    // From the end, until the pixal brighter than that of 0.1*peak
	i = 3647;
	while (seam_location[i] < 0.1*peak)  // the threshold of valid pixel
	{
		end_pixel = i;
		i--;
	}

	// restore the light data and position
    for (i=start_pixel; i<=end_pixel; i++)
	{
		*(++light_data) = seam_location[i];
		*(++ypos) = i;
	}

	return (end_pixel - start_pixel + 1);
}


/**
* Function: void covsrt()
*     Expand in storage the covariance matrix covar, so as to take into account
*     parameters that are being held fixed.
*
*     Spreading the covariances back into the full ma x ma covariances set
*     for variables which were held frozen.
*/
static void
covsrt(float** covar, const int ma, const int mfit)
{
    int i, j, k;
    float temp;

    for (i=mfit; i<ma; i++)
        for (j=0; j<i; j++)
            covar[i][j] = covar[j][i] = 0.0;

    k = mfit;
    for (j=ma-1; j>=0; j--)
    {
        for (i = 0; i < ma; i++)
            SWAP(covar[i][k], covar[i][j]);
        for (i = 0; i < ma; i++)
            SWAP(covar[k][i], covar[j][i]);
        k--;
    }
}


/**
* Function: void gaussj()
*   Solution of matrix by Gauss-Jordan elimination.
*
* @param a[1...n][1...n] is the input matrix.
* @param b[1...n] is the input containing the right hand side vector.
*        Here, the m represents that we have m set of linear equations.
*
* @retval a: is replaced by its matrix inverse, 
*         b: is replaced by the corresponding set of solution vectors.
*/
static void 
gaussj(float** a, const int n, float* b)
{
    int* indxc, * indxr, * ipiv;
    int icol = 0;
    int irow = 0;
    int i, j, k, l, ll;
    float big, dum, pivinv, temp;

    // The integer arrays: ipiv, indxr, and indxc are used for bookkeeping
    // on the pivoting.
    indxc = ivector(n);  // column of the pivot element
    indxr = ivector(n);  // row of the pivot element
    ipiv = ivector(n);   // index of the biggest element

    for (j = 0; j < n; j++) ipiv[j] = 0;

    // This is the main loop over the rows to be reduced.
    for (i = 0; i < n; i++)
    {
        big = 0.0;  // the largest value of a_ii element.

        // This is the outer loop of the search for a pivot element,
        // when the ith row been reduced.
        for (j = 0; j < n; j++)  // over the row
        {
            // ipiv[j] = 1, the j_col has the biggest element
            // ipiv[j] = 0, the j_col has not been touched.
            // ipiv[j] > 1, two equations have the same value.
            if (ipiv[j] != 1)
            {
                for (k = 0; k < n; k++)  // along the column
                {
                    if (ipiv[k] == 0)  // no touched column only
                    {
                        if (fabs(a[j][k]) >= big)
                        {
                            big = fabs(a[j][k]);
                            irow = j;
                            icol = k;
                        }
                    }
                    else if (ipiv[k] > 1) return;  // Singular Matrix-1
                }
            }
        }

        ++(ipiv[icol]);  // set ipiv[k] once.

        //-----------------------------------------------------------------
        // We now have the pivot element, so we interchange rows, if needed,
        // to put the pivot element on the diagonal.
        //
        // The columns are not physically interchanged, only relabeled:
        // indxc[i], the column of the ith pivot element, is the ith column
        // that is reduced, while indxr[i] is the row in which that pivot
        // element was originally located.
        //
        // If indxr[i] \neqn indxc[i] there is an implied column interchange.
        // With this form of bookkeeping, the solution b's will end up in the
        // correct order, and the inverse matrix will be scrambled by columns.
        //-------------------------------------------------------------------
        if (irow != icol)
        {
            for (l = 0; l < n; l++)
                SWAP(a[irow][l], a[icol][l]);
            SWAP(b[irow], b[icol]);
        }

        // We are now ready to divide the pivot row by the pivot element,
        // located at irow and icol.
        indxr[i] = irow;
        indxc[i] = icol;

        if (a[icol][icol] == 0) return;  // Singular Matrix-2
        pivinv = 1.0 / a[icol][icol];
        a[icol][icol] = 1.0;

        for (l = 0; l < n; l++)
            a[icol][l] *= pivinv;
        b[icol] *= pivinv;

        // Next, we reduce the row, except for the pivot one.
        for (ll = 0; ll < n; ll++)
        {
            if (ll != icol)
            {
                dum = a[ll][icol];
                a[ll][icol] = 0.0;
                for (l = 0; l < n; l++)
                    a[ll][l] -= a[icol][l] * dum;
                b[ll] -= b[icol] * dum;
            }
        }
    }

    // This is the end of the main loop over columns of the reduction.
    // It only remains to unscramble the solution in view of the column
    // interchanges.
    //
    // We do this by interchanging pairs of columns in the reverse order
    // that the permutation was built up.
    for (l = n - 1; l >= 0; l--)
    {
        if (indxr[l] != indxc[l])
        {
            for (k = 0; k < n; k++)
                SWAP(a[k][indxr[l]], a[k][indxc[l]]);
        }
    }

    free_ivector(ipiv);
    free_ivector(indxr);
    free_ivector(indxc);
}


/**
* Function: void seamdata()
*     Gaussian function fitting to the measure data.
*     y = exp[-(x-a[0]/a[1])]^2, a[1] is the central and a[1] the half-width.
*
* Input: y = f(x; a), x is position, and a is the fit coefficients
* Output: dyda
*/
static void
seamdata(const float x, const float a[], float *y,
	float dyda[])
{
    float fac, ex, arg;

    arg = (x-a[0])/a[1];
    ex = exp(- (double)arg * (double)arg);
    *y = ex;

    fac = 2.0*arg*ex;
    dyda[0] = fac/a[1];
    dyda[1] = fac*arg/a[1];
}


/**
* Function: void mrqcof.
*     Used by mrqmin to evaluate the linearized fitting matrix alpha, 
*     and vector beta as in equ.15.8.8, and calculate \chi^2.
*     Specified for Gaussian function fitting.
*
* Input: x[], measured position
*        y[], measured PD value, normalized to maximum value
*      ndata, the number of measure data
*        a[], the fitted coefficients, where a[0] is the central position
*             a[1] is the half-width of Guassian function.
* Output: alpha, the Hessian matrix * 1/2
*          beta, the Jacobian matrix, the gradient
*         chisq, the chi squared coefficient
*/
static void
mrqcof(const float x[], const float y[], const int ndata,
	   float a[], float** alpha, float beta[], float* chisq)
{
    int i, j, k, l, m;
    const int mfit = 2;  // number of fitted coefficients
    float ymod, wt, sig2i, dy, * dyda;

    dyda = vector(2);

    // Initialize (symmetric, only half of matrix) alpha, beta and chisq.
    for (j = 0; j < mfit; j++)
    {
        for (k = 0; k <= j; k++) alpha[j][k] = 0.0;
        beta[j] = 0.0;
    }

    // Summation loop over all data.
    *chisq = 0.0;
    for (i = 0; i < ndata; i++)
    {
        // input x[] and a,
        // get data from ymod (y value by the fitting modal) and dyda
        seamdata(x[i], a, &ymod, dyda);
        sig2i = 1.0;  // the measure error is ignored.
        dy = y[i] - ymod;

        for (j = 0, l = 0; l < 2; l++)
        {
            wt = dyda[l] * sig2i;
            for (j++, k = 0, m = 0; m <= l; m++)
                alpha[j][k++] += wt * dyda[m];
            beta[j] += dy * wt;
        }
        *chisq += dy * dy * sig2i;  // finding \chi^2
    }

    // Fill in the symmetric side.
    for (j = 1; j < mfit; j++)
        for (k = 1; k < j; k++) alpha[k][j] = alpha[j][k];

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
*        and a nonlinear function dependent on ma coefficients a[1,2].
*
*        The arrays covar[1,2][1,2], alpha[1,2][1,2], are used as working
*        space during most iterations.
*
*        On the first call provide an initial guess for the parameters a, 
*        and set alamda<0 for initialization (which then sets alamda=.001).
*        If a step succeeds, chisq becomes smaller and alamda decreases
*        by a factor of 10.
*        If a step fails, alamda grows by a factor of 10. You MUST call 
*        this routine repeatedly until convergence is achieved.
*
*        Then, make one final call with alamda=0, so that covar[1,2][1,2]
*        returns the covariance matrix, and alpha the curvature matrix.
*
*        The program returns current best-fit values for the parameters a[1,2],
*        and \chi^2 = chisq.
*/
static void 
mrqmin(const float x[], const float y[], const int ndata,
       float a[],
       float** covar, float** alpha, float* chisq, float* alamda)
{
    int j, k, l;
    const int mfit = 2;  // number of fitted coefficients
    static float ochisq;  // old value of chi^2
    static float* atry, * beta, * da, * oneda;

    // Initialization when alamda is set less than 0 in the first step.
    if (*alamda < 0.0)
    {
        // allocate memory, not used in new C version
        atry = vector(mfit);  // try value of a
        beta = vector(mfit);
        da = vector(mfit);
        oneda = vector(mfit); // one column of dyda

        *alamda = 0.001;
        mrqcof(x, y, ndata, a, alpha, beta, chisq);
        ochisq = (*chisq);

        for (j = 0; j < mfit; j++) atry[j] = a[j];
    }

    // Alter linearized fitting matrix, by augmenting diagonal elements
    for (j = 0; j < mfit; j++)
    {
        for (k = 0; k < mfit; k++)
            covar[j][k] = alpha[j][k];

        covar[j][k] = alpha[j][j] * (1.0 + (*alamda));
        oneda[j] = beta[j];
    }

    gaussj(covar, mfit, oneda);  // Matrix solution

    for (j = 0; j < mfit; j++) da[j] = oneda[j];

    // Did the trial succeed?
    for (j = 0, l = 0; l < mfit; l++)
        atry[l] = a[l] + da[++j];
    mrqcof(x, y, ndata, atry, covar, da, chisq);

    // Success, accept the new solution.
    if (*chisq < ochisq)
    {
        *alamda *= 0.1;
        ochisq = (*chisq);
        for (j = 0; j < mfit; j++)
        {
            for (k = 0; k < mfit; k++)
                alpha[j][k] = covar[j][k];
            beta[j] = da[j];
        }

        for (l = 0; l < mfit; l++)
            a[l] = atry[l];
    }
    else  // Failure, increase alamda and return.
    {
        *alamda *= 10.0;
        *chisq = ochisq;
    }

    //----------------------------------------------//
    // Once converged, evaluate covariance matrix.
    //----------------------------------------------//
    if (*alamda == 0.0)
    {
        // Spread out alpha to its full size too.
        covsrt(covar, mfit, mfit);
        covsrt(alpha, mfit, mfit);

        // free the memory, not used in new C version.
        free_vector(oneda);
        free_vector(da);
        free_vector(beta);
        free_vector(atry);
        return;
    }

    return;
}


/**
* Function: float seekSeam()
*  Find the center from the CCD data
* Input: measured data
*     x: position
*     y: light intensity
*     n: the number of data
* Return: the position of seam
*/
static float seekSeam(float* x, float* y, int n)
{
    float alamda, ochisq, chisq;
    static float a[2];
    static float** covar;  // covariance matrix
    static float** alpha;  // Hessian matrix

    covar = matrix(2, 2);
    alpha = matrix(2, 2);

    alamda = -1;  // initial the lamda value.
    ochisq = chisq = 1;

    // initial value on the peak of light intensity
    a[0] = lightPeak(x, y, n);
    a[1] = 125;  // set ~1 mm of half-width

    for (int i = 0; i < 10; i++)
    {
        mrqmin(x, y, n, a, covar, alpha, &chisq, &alamda);
        if (fabs((double)ochisq - (double)chisq) < 0.01)
        {
            alamda = 0;
            mrqmin(x, y, n, a, covar, alpha, &chisq, &alamda);
            break;
        }
        ochisq = chisq;
    }

    free_matrix(covar, 2, 2);
    free_matrix(alpha, 2, 2);
    return a[0];
}


/**
 * Function: solve the pulse number for the position shift
 * Input: position shift of the seam
 * Return: the degree that the x motor should be moved
 */
static float motion_dis(float diff_position)
{
	float x_pulse;
	x_pulse = diff_position / PULDIS;
	return x_pulse;
}


/**
 * Function: move the x motor to the given position
 * Input: the degree that the x motor should be moved
 * Return: void
 */
static void motor_action(float x_pulse)
{
	if (x_pulse > 0)
	{
		stepmotor_x_run(FORWARD, x_pulse);
	}
	else if (x_pulse < 0)
	{
		stepmotor_x_run(BACKWARD, x_pulse);
	}
}


/**
 * Function: allots finding tasks
 * Input: CCD raw data for seam location
 * Return: void
 */
void find_seam(const uint16_t *seam_location)
{
	static float p0_position = 0;  // the current position of seam
    static uint16_t USED_PIXEL[PIX_NUMBER];  // the largest length of used pixel is 3648
    static uint16_t POS_SPACE[PIX_NUMBER];   // the storage space of position

    uint16_t *light_data;
	uint16_t *ypos;


	light_data = USED_PIXEL;
	ypos = POS_SPACE;

	uint16_t ndata;

	ndata = pretreat(seam_location, light_data, ypos);
	float position = seekSeam((float*)light_data, (float*)ypos, ndata);

	if (position != p0_position && p0_position != 0)
	{
		float diff_position = position - p0_position;
		float x_pulse = motion_dis(diff_position);

		motor_action(x_pulse);

		p0_position = position;
	}
}

