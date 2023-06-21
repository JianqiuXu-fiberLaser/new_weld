using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http.Headers;
using System.Security.Cryptography.Pkcs;
using System.Windows.Media.Media3D;
using static System.Math;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// weld the trace of intersection
    /// </summary>
    public class IntersectTrace
    {
        private MotionOperate? _mo;
        private MainModel? mainModel;
        private WorkPackage? workPackage;
        private IntersectModel? intersectModel;

        private PointListData? points;
        private double WeldSpeed;
        private double moveSpeed;
        private double upperRadius;
        private double lowerRadius;

        private nowState? nws;
        private List<int[]>? pots;
        private List<int>? Apots;
        private List<double>? CofWeld;

        public IntersectTrace() { }

        public void CopyHardware(MotionOperate mo, MainModel mm, WorkPackage wp, IntersectModel im)
        {
            this._mo = mo;
            this.mainModel = mm;
            this.workPackage = wp;
            this.intersectModel = im;

            points = workPackage.GetPointList();
            pots = new List<int[]>();
            Apots = new List<int>();
            CofWeld = new List<double>();
            LaserWeldParameter lwp = points.GetNoZeroParameter();

            // _mo.InitialLaser();
            _mo.SetupLaserParameter(in lwp, workPackage.GetMaxPower());
            InitialLaser(lwp);
            this.WeldSpeed = lwp.WeldSpeed;
            this.moveSpeed = lwp.LeapSpeed;

            this.upperRadius = intersectModel.UpperRadius;
            this.lowerRadius = intersectModel.LowerRadius;
        }

        private void InitialLaser(LaserWeldParameter lwp)
        {


            if (lwp.Frequency == 0)
            {
                _mo?.InitialLaser(0);
            }
            else
            {
                _mo?.InitialLaser(1);    // QCW mode
            }
        }

        /// <summary>
        /// The standard weld process
        /// </summary>
        public void WorkProcess()
        {
            if (points != null && pots != null && Apots != null)
            {
                PrepareData(points, pots, Apots);
            }

            _mo?.SetActionMethod(this);
            CheckPosition();

            // Loop thread to scan IO and action 
            int[] bitAddr = new int[1] { 0 };
            _mo?.ScanAction(bitAddr);
        }

        /// <summary>
        /// Receive the IO signal from motion hardware
        /// </summary>
        /// <param name="iActNum"></param>
        public void Action(int[] iActNum)
        {
            if (iActNum[0] == 1 && nws == nowState.PRE_FINISH)
            {
                WeldCycle();
            }
        }

        /// <summary>
        /// When the Task is finised, no run thread exist.
        /// </summary>
        /// <param name="iAct"></param>
        public void FinAction(string iAct)
        {
            if (iAct == "rotate")
            {
                BackPrepare();
            }
        }

        private void WeldCycle()
        {
            if (pots != null && Apots != null && CofWeld != null)
            {
                _mo?.RunRotateWeld(pots, Apots, moveSpeed, WeldSpeed, CofWeld);
            }
        }

        private void BackPrepare()
        {
            _mo?.CloseAir();
            if (pots != null)
            {
                _mo?.RunArrive(pots[0], 0.8 * moveSpeed);
            }
        }

        private void PrepareData(PointListData points, List<int[]> pots, List<int> Apots)
        {
            if (intersectModel != null && !intersectModel.ClampDirection)
            {
                PrepareVertical(points, pots, Apots);
            }
            else
            {
                PrepareHorizonal(points, pots, Apots);
            }
        }

        private void PrepareVertical(PointListData points, List<int[]> pots, List<int> Apots)
        {
            for (int i = 0; i < 2; i++)
            {
                int[] xyz = points.GetXYZ(i);
                pots.Add(xyz);
            }

            int stptZ = pots[1][2];

            int[] startXYZ = new int[3];
            startXYZ[0] = pots[1][0];
            startXYZ[1] = pots[1][1];
            startXYZ[2] = pots[1][2];

            //-- the key parameter
            double[] xcos2 = new double[5];
            int[] coordZ = new int[5];
            for (int i = 0; i < 5; i++)
            {
                xcos2[i] = Pow(upperRadius * UseCos(i * 22.5 - 90.0), 2);
                coordZ[i] = (int)(MotionOperate.OneMillimeter * Sqrt(lowerRadius * lowerRadius - xcos2[i]));
            }
            stptZ -= coordZ[0];

            // A axis, rotate coordinate
            for (int i = 0; i <= 16; i++)
            {
                Apots.Add((int)(MotionOperate.OneCycle * i / 16.0));
            }

            // Speed coefficients
            double[] cofweld = new double[4];
            for (int i = 0; i < 4; i++)
            {
                double standardLength = Sqrt(Pow((coordZ[1] - coordZ[0]), 2) + Pow((Apots[1] - Apots[0]), 2));
                double sectionLength = Sqrt(Pow((coordZ[i + 1] - coordZ[i]), 2) + Pow((Apots[1] - Apots[0]), 2));
                cofweld[i] = 1.0 * sectionLength / standardLength;
            }

            //---- Calculate the trace data ----
            // the 1st section
            CofWeld?.Add(cofweld[0]);    // the speed to the first point, which is not important
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { startXYZ[0], startXYZ[1], stptZ + coordZ[i + 1] };
                CofWeld?.Add(cofweld[i]);
                pots.Add(xyz);
            }

            // the 2nd section
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { startXYZ[0], startXYZ[1], stptZ + coordZ[3 - i] };
                CofWeld?.Add(cofweld[3 - i]);
                pots.Add(xyz);
            }

            // the 3rd section
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { startXYZ[0], startXYZ[1], stptZ + coordZ[i + 1] };
                CofWeld?.Add(cofweld[i]);
                pots.Add(xyz);
            }

            // the 4th section
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { startXYZ[0], startXYZ[1], stptZ + coordZ[3 - i] };
                CofWeld?.Add(cofweld[3 - i]);
                pots.Add(xyz);
            }
        }

        private void PrepareHorizonal(PointListData points, List<int[]> pots, List<int> Apots)
        {
            for (int i = 0; i < 2; i++)
            {
                int[] xyz = points.GetXYZ(i);
                pots.Add(xyz);
            }

            int stptX = pots[1][0];

            int[] startXYZ = new int[3];
            startXYZ[0] = pots[1][0];
            startXYZ[1] = pots[1][1];
            startXYZ[2] = pots[1][2];

            //-- the key parameter
            double[] xcos2 = new double[5];
            int[] coordX = new int[5];
            for (int i = 0; i < 5; i++)
            {
                xcos2[i] = Pow(upperRadius * UseCos(i * 22.5 - 90.0), 2);
                coordX[i] = (int)(MotionOperate.OneMillimeter * Sqrt(lowerRadius * lowerRadius - xcos2[i]));
            }
            stptX -= coordX[0];

            // A axis, rotate coordinate
            for (int i = 0; i <= 16; i++)
            {
                Apots.Add((int)(MotionOperate.OneCycle * i / 16.0));
            }

            // Speed coefficients
            double[] cofweld = new double[4];
            for (int i = 0; i < 4; i++)
            {
                double standardLength = Sqrt(Pow((coordX[1] - coordX[0]), 2) + Pow((Apots[1] - Apots[0]), 2));
                double sectionLength = Sqrt(Pow((coordX[i + 1] - coordX[i]), 2) + Pow((Apots[1] - Apots[0]), 2));
                cofweld[i] = 1.0 * sectionLength / standardLength;
            }

            //---- Calculate the trace data ----
            // the 1st section
            CofWeld?.Add(cofweld[0]);    // the speed to the first point, which is not important
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { stptX + coordX[i + 1], startXYZ[1], startXYZ[2] };
                CofWeld?.Add(cofweld[i]);
                pots.Add(xyz);
            }

            // the 2nd section
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { stptX + coordX[3 - i], startXYZ[1], startXYZ[2] };
                CofWeld?.Add(cofweld[3 - i]);
                pots.Add(xyz);
            }

            // the 3rd section
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { stptX + coordX[i + 1], startXYZ[1], startXYZ[2] };
                CofWeld?.Add(cofweld[i]);
                pots.Add(xyz);
            }

            // the 4th section
            for (int i = 0; i < 4; i++)
            {
                int[] xyz = new int[3] { stptX + coordX[3 - i], startXYZ[1], startXYZ[2] };
                CofWeld?.Add(cofweld[3 - i]);
                pots.Add(xyz);
            }
        }

        //
        // Cosin with degree of angle
        //
        private double UseCos(double angle)
        {
            double ret;
            ret = Cos(PI * angle / 180.0);
            return ret;
        }

        //
        // Check the axes positon whether at the prepared position.
        //
        private bool CheckPosition()
        {
            int[] pPosArray = new int[4];
            _mo?.ReadCoordinate(out pPosArray);

            if (pots != null && pPosArray[0] == pots[0][0])
            {
                nws = nowState.PRE_FINISH;
                return true;
            }

            nws = nowState.RESET_POS;
            return false;
        }

        //
        // constants: work statement
        //
        public enum nowState
        {
            PRE_FINISH,
            IN_WELD,
            RESET_POS,
        }
    }
}
