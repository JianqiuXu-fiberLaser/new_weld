using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using System.Collections.Generic;
using static System.Math;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// weld the trace of intersection
    /// </summary>
    public class IntersectTrace
    {
        private readonly MotionOperate _mo;
        private WorkPackage? _workPackage;
        private List<WeldMoveSection>? _wms;

        // special variables
        private double upperRadius;
        private double lowerRadius;
        private int InstalStyle;

        public IntersectTrace(MotionOperate mo)
        {
            _mo = mo;
        }

        public void CopyHardware(WorkPackage wp, IntersectModel im)
        {
            _workPackage = wp;

            upperRadius = im.UpperRadius;
            lowerRadius = im.LowerRadius;
            InstalStyle = im.InstalStyle;
            PrepareData();
        }

        /// <summary>
        /// The standard weld process
        /// </summary>
        public void WorkProcess()
        {
            if (_wms == null) { return; }

            RunWeld4D runWeld4D = new(_mo);
            runWeld4D.CPutParameter(_wms);
        }

        private void PrepareData()
        {
            if (_workPackage == null) return;

            PointListData points = _workPackage.GetPointList();
            List<double> ds = IntersectSeam();
            AssembleSeam(points, ds, out List<int[]> ptlist);

            if (points.GetCount() > 2)
            {
                CorrectSeam(points, ptlist);
            }

            TransferToWms(ptlist, points);
        }

        //
        // Calculate intersect seam by upper and lower radius
        //
        private List<double> IntersectSeam()
        {
            List<double> dist = new();

            //-- the key parameter, from -90 to 0 degree.
            double[] cos2 = new double[9];
            double[] coord = new double[9];
            for (int i = 0; i < 9; i++)
            {
                cos2[i] = Pow(upperRadius * UseCos(i * 11.25 - 90.0), 2);
                coord[i] = (Sqrt(lowerRadius * lowerRadius - cos2[i]));
            }

            // 4 section: 0~7, 8~1, 0~7, 8~1 
            for (int i = 0; i < 8; i++) dist.Add((coord[i] - coord[0]));
            for (int i = 8; i > 0; i--) dist.Add((coord[i] - coord[0]));
            for (int i = 0; i < 8; i++) dist.Add((coord[i] - coord[0]));
            for (int i = 8; i > 0; i--) dist.Add((coord[i] - coord[0]));

            // the last (33th) points is coord[0] - coord[0], back to original
            dist.Add(0);

            return dist;
        }

        //
        // Assemble the intersect seam to the real trace according to the instal style
        //
        private void AssembleSeam(PointListData pts, List<double> ds, out List<int[]> ptlist)
        {
            int direction;
            double xstep, zstep;
            if (InstalStyle == 0)
            {
                // vertical positive
                direction = 1;
                xstep = 0.0;
                zstep = MotionOperate.Zmillimeter;
            }
            else if (InstalStyle == 1)
            {
                // vertical negative
                direction = -1;
                xstep = 0.0;
                zstep = MotionOperate.Zmillimeter;
            }
            else if (InstalStyle == 2)
            {
                // horizontal positive
                direction = 1;
                xstep = MotionOperate.Xmillimeter;
                zstep = 0.0;
            }
            else
            {
                // horizontal negative
                direction = -1;
                xstep = MotionOperate.Xmillimeter;
                zstep = 0.0;
            }

            // Add the intersect seam to the position
            ptlist = new();
            int[] xyz = pts.GetXYZ(0);
            int angle = pts.GetAngle(0);
            int[] xyza = new int[4] { xyz[0], xyz[1], xyz[2], angle };
            ptlist.Add(xyza);

            // points in the seam
            int[] startxyz = pts.GetXYZ(1);
            int startangle = pts.GetAngle(1);

            for (int i = 0; i < 33; i++)
            {
                int x = startxyz[0] + (int)(ds[i] * direction * xstep);
                int y = startxyz[1];
                int z = startxyz[2] + (int)(ds[i] * direction * zstep);
                angle = startangle + (int)(MotionOperate.OneCycle * i / 32.0);
                int[] dxyza = new int[4] { x, y, z, angle };
                ptlist.Add(dxyza);
            }

            // back to original
            ptlist.Add(xyza);
        }

        //
        // If there are more two points, correct the trace using these points
        //
        private void CorrectSeam(PointListData pts, List<int[]> ptlist)
        {
            if (pts.GetCount() >= 4)
            {
                TwoPointCorrect(in pts, ptlist);
            }
            else if (pts.GetCount() >= 3)
            {
                OnePointCorrect(in pts, ptlist);
            }
        }

        private void TwoPointCorrect(in PointListData pts, List<int[]> ptlist)
        {
            int[] startxyz = ptlist[1];
            int startangle = ptlist[1][3];

            int[] corrtxyz0 = pts.GetXYZ(2);
            int corrtangle0 = pts.GetAngle(2);

            int[] corrtxyz1 = pts.GetXYZ(3);
            int corrtangle1 = pts.GetAngle(3);

            int delAngle0 = corrtangle0 - startangle;
            int delAngle1 = corrtangle1 - startangle;

            // the first correct point does not at the neighbour of 90 degree
            if ((delAngle0 <= MotionOperate.OneCycle * 0.245 && delAngle0 > 0)
                || (delAngle0 >= MotionOperate.OneCycle * 0.255))
            {
                MainModel.AddInfo("相贯线第一校正点选择错误");
            }

            // the second correct point does not at the neighbour of 270 degree
            // or -90 degree in other way.
            if ((delAngle1 <= MotionOperate.OneCycle * 0.745 && delAngle1 > 0)
                || (delAngle1 >= MotionOperate.OneCycle * 0.755)
                || (delAngle1 >= -MotionOperate.OneCycle * 0.245 && delAngle1 < 0)
                || delAngle1 <= --MotionOperate.OneCycle * 0.255)
            {
                MainModel.AddInfo("相贯线第二校正点选择错误");
            }

            // The z step
            if (InstalStyle == 0 || InstalStyle == 1)
            {
                double delz0 = corrtxyz0[2] - startxyz[2];
                double calz = ptlist[9][2] - startxyz[2];
                double coefz0 = delz0 / calz;

                for (int i = 1; i < 18; i++)
                {
                    ptlist[i][2] = startxyz[2] + (int)(coefz0 * (ptlist[i][2] - startxyz[2]));
                }

                double delz1 = corrtxyz1[2] - startxyz[2];
                double coefz1 = delz1 / calz;
                for (int i = 18; i < 34; i++)
                {
                    ptlist[i][2] = startxyz[2] + (int)(coefz1 * (ptlist[i][2] - startxyz[2]));
                }
            }
            // the x step
            else
            {
                double delx0 = corrtxyz0[0] - startxyz[0];
                double calx = ptlist[9][0] - startxyz[0];

                double coefx0;
                if (calx == 0 || delx0 == 0)
                    coefx0 = 1.0;
                else
                    coefx0 = delx0 / calx;

                for (int i = 1; i < 18; i++)
                {
                    ptlist[i][0] = startxyz[0] + (int)(coefx0 * (ptlist[i][0] - startxyz[0]));
                }

                double delx1 = corrtxyz1[0] - startxyz[0];
                double calx1 = ptlist[25][0] - startxyz[0];

                double coefx1;
                if (calx1 == 0)
                    coefx1 = 1.0;
                else
                    coefx1 = delx1 / calx1;

                for (int i = 18; i < 34; i++)
                {
                    ptlist[i][0] = startxyz[0] + (int)(coefx1 * (ptlist[i][0] - startxyz[0]));
                }
            }
        }

        //
        // one point is working for symmetric correction 
        //
        private void OnePointCorrect(in PointListData pts, List<int[]> ptlist)
        {
            int[] startxyz = ptlist[1];
            int startangle = ptlist[1][3];

            int[] corrtxyz = pts.GetXYZ(2);
            int corrtangle = pts.GetAngle(2);

            int delAngle = Abs(corrtangle - startangle);
            if (delAngle <= MotionOperate.OneCycle * 0.245
                || (delAngle >= MotionOperate.OneCycle * 0.255 && delAngle <= MotionOperate.OneCycle * 0.745)
                || delAngle >= MotionOperate.OneCycle * 0.755)
            {
                MainModel.AddInfo("相贯线校正点选择错误");
            }

            // The z step
            if (InstalStyle == 0 || InstalStyle == 1)
            {
                double delz = corrtxyz[2] - startxyz[2];
                double calz = ptlist[9][2] - startxyz[2];
                double coefz = delz / calz;

                for (int i = 1; i < 34; i++)
                {
                    ptlist[i][2] = startxyz[2] + (int)(coefz * (ptlist[i][2] - startxyz[2]));
                }
            }
            // The x step
            else
            {
                double delx = corrtxyz[0] - startxyz[0];
                double calx = ptlist[9][0] - startxyz[0];
                double coefx = delx / calx;

                for (int i = 1; i < 34; i++)
                {
                    ptlist[i][0] = startxyz[0] + (int)(coefx * (ptlist[i][0] - startxyz[0]));
                }
            }
        }

        //
        // Transfer point array list to weld-move-section data
        //
        private void TransferToWms(List<int[]> ptlist, in PointListData points)
        {
            _wms ??= new();

            // From preparation to the start point
            LaserWeldParameter lwp = points.GetLaserWeldParameter(0);
            List<double> speed = new() { lwp.LeapSpeed, lwp.LeapSpeed };
            LaserArgument lag = new(lwp);
            Dictionary<int, LaserArgument> args = new()
            {
                { 0, lag }
            };
            List<int[]> sectpts = ptlist.GetRange(0, 2);

            WeldMoveSection iwms = new(sectpts, speed)
            {
                Argument = args,
                MoveType = 0,
            };

            _wms.Add(iwms);

            // in the seam
            LaserWeldParameter lwp2 = points.GetLaserWeldParameter(1);
            List<double> speed2 = new();
            for (int i = 0; i < 33; i++)
            {
                speed2.Add(lwp2.WeldSpeed);
            }

            LaserArgument lag2 = new(lwp2);
            Dictionary<int, LaserArgument> args2 = new()
            {
                { 0, lag2 }
            };
            List<int[]> sectpts2 = ptlist.GetRange(1, 33);

            WeldMoveSection iwms2 = new(sectpts2, speed2)
            {
                Argument = args2,
                MoveType = 0,
            };

            _wms.Add(iwms2);

            // the last section
            // Because the speed in the last section is smoothed to rotating, it need compensate by
            // divided the SmoothCoefficient.
            List<int[]> sectpts3 = ptlist.GetRange(33, 2);
            List<double> speed3 = new() { speed[0] / MotionOperate.SmoothCoefficient, speed[1] / MotionOperate.SmoothCoefficient };
            WeldMoveSection iwms3 = new(sectpts3, speed3)
            {
                Argument = args,
                MoveType = 0,
            };

            _wms.Add(iwms3);
        }

        //
        // Cosin with degree of angle
        //
        private static double UseCos(double angle)
        {
            double ret;
            ret = Cos(PI * angle / 180.0);
            return ret;
        }
    }
}
