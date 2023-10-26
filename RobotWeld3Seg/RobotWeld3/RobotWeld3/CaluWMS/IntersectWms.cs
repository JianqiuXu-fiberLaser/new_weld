///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) new laser weld parameter from the server or local
// Ver. 3.0: (1) add 2 xml file to store points' information and using
//               5 data structures to represent moving, laser and
//               material specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using RobotWeld3.Welding;
using System.Collections.Generic;
using System.IO;
using static System.Math;

namespace RobotWeld3.CaluWMS
{
    internal class IntersectWms : BasicWms
    {
        private WeldingModel? _wmodel;

        internal IntersectWms() { }

        /// <summary>
        /// Return back list of weld move section
        /// </summary>
        /// <param name="icode"></param>
        /// <param name="timeStamp"></param>
        /// <param name="wms"></param>
        public override void BackWms(int icode, int timeStamp, ref List<WeldMoveSection> wms)
        {
            wms.Clear();
            string fname = "./Storage/" + icode.ToString() + ".wms";

            if (File.Exists(fname))
            {
                // if the wms has a correct time stamp, then pass the generate new wms
                if (ReadWms(timeStamp, fname, ref wms)) return;
            }

            GenerateWms(ref wms, timeStamp);
            SaveWms(timeStamp, fname, in wms);
        }

        /// <summary>
        /// Generate new weld-move-section list by the input display point list.
        /// </summary>
        /// <param name="wms"></param>
        internal void GenerateWms(ref List<WeldMoveSection> wms, int timestamp)
        {
            _wmodel = new WeldingModel();
            _wmodel.ReadCoefficient(timestamp);

            // The sequence is important.
            var s0 = new WeldMoveSection();
            var dpcount = StartWms(0, ref s0);
            if (s0 != null) wms.Add(s0);

            var s1 = IntersectionWms(1, dpcount);
            if (s1 != null) wms.Add(s1);

            var s2 = EndWms(2);
            if (s2 != null) wms.Add(s2);
        }

        /// <summary>
        /// Save the generated weld-move-section list to the disk
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="fname"></param>
        /// <param name="wms"></param>
        private static void SaveWms(int timeStamp, string fname, in List<WeldMoveSection> wms)
        {
            AccessWmsFile.Save(fname, timeStamp, wms);
        }

        /// <summary>
        /// if the time stamp is in-correct, return false
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="fname"></param>
        /// <param name="wms"></param>
        /// <returns></returns>
        private static bool ReadWms(int timeStamp, string fname, ref List<WeldMoveSection> wms)
        {
            var ret = AccessWmsFile.Read(fname, timeStamp, ref wms);
            return ret;
        }

        //-------------------------------------------------------------
        // The concrete methods to make WMS
        //-------------------------------------------------------------

        /// <summary>
        /// Start weld move section.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="wms"></param>
        /// <returns></returns>
        private int StartWms(int order, ref WeldMoveSection wms)
        {
            if (_ptList == null || _wmodel == null) return 0;
            var pdl = StandardWms.FindStartPoints(_ptList);
            StandardWms.StartPointWms(order, pdl, ref wms, _wmodel);
            return pdl.Count;
        }

        /// <summary>
        /// End Weld move section
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private WeldMoveSection? EndWms(int order)
        {
            if (_ptList == null || _wmodel == null) return null;
            var pdl = StandardWms.FindIntersectEndPoints(_ptList);
            return StandardWms.EndPointWms(order, pdl, _wmodel);
        }

        /// <summary>
        /// The Wms section of intersection trace.
        /// </summary>
        /// <param name="order"> Wms order </param>
        /// <param name="dpount"> point count of previous wms </param>
        /// <returns> wms </returns>
        private WeldMoveSection? IntersectionWms(int order, int dpcount)
        {
            if (_ptList == null || _pml == null || _wmodel == null) return null;

            double upperRadius = _pml[0];
            double lowerRadius = _pml[1];

            var g = GeometricalLocus(upperRadius, lowerRadius);
            var spc = CrdInterpolation.MatchSpeed(g);

            // the anchor point
            var p = _ptList[dpcount];
            var speed = CrdInterpolation.RotateSpeed(p.Speed, spc);

            // Set the laser parameter.
            var ayp = new AnalysePoint();
            ayp.GetModel(_wmodel);
            ayp.GetPoint(_ptList, dpcount, dpcount);
            (var ma, var wi, var lp) = ayp.CalculateParameter();
            var w = ayp.ExtractWobble();
            var vc = WmsTrace(_pml[2], p.Vector, g);
            var wms = new WeldMoveSection(vc)
            {
                WmsIndex = order,
                LaserParameter = lp,
                Material = ma,
                Wire = wi,
                Speed = speed,
                Wobble = w,
            };

            return wms;
        }

        /// <summary>
        /// Geometrical Locus of an intersection of normal placed tubes.
        /// Return g[0] is the angle, g[1] the delta distance
        /// </summary>
        /// <param name="up"></param>
        /// <param name="lo"></param>
        /// <returns></returns>
        private static List<double[]> GeometricalLocus(double up, double lo)
        {
            var g = new List<double[]>();
            double uangle = 0.5 * MotionOperate.OneCycle / PI;

            if (up > lo) (up, lo) = (lo, up);
            var da = CrdInterpolation.DeltaAngle(up);

            // the 1st phase space, 0 ~ pi/2
            double cn = 0.5 * PI / da;
            double angle = 0;
            for (int i = 0; i < cn; i++)
            {
                double Sin2 = Pow(up * Sin(angle), 2);
                double d = lo - Sqrt(lo * lo - Sin2);
                g.Add(new double[2] { angle * uangle, d });
                angle += da;
            }

            // Since the residual gap is large, it need to be added
            double cfrac = cn - (int)cn;
            if (cfrac > 0.25)
            {
                double Sin2 = Pow(up, 2);
                double d = lo - Sqrt(lo * lo - Sin2);
                g.Add(new double[2] { PI * uangle / 2, d });
            }

            // the last point at pi/2, whose step may be larger than one.
            // It is not included in the privous phase space.

            // the 2nd phase space, pi/2 ~ pi.
            double a1, d1;
            var c = g.Count;
            for (int i = 0; i < cn; i++)
            {
                a1 = PI * uangle / 2 + g[i][0];
                d1 = g[c - i - 1][1];
                g.Add(new double[2] { a1, d1 });
            }

            // the 3rd phase space, pi ~ 3*pi/2
            for (int i = 0; i < cn; i++)
            {
                a1 = PI * uangle + g[i][0];
                d1 = g[i][1];
                g.Add(new double[2] { a1, d1 });
            }

            // the 4th phase space, 3*pi/2 ~ 2*Pi
            for (int i = 0; i < cn; i++)
            {
                a1 = 3 * PI * uangle / 2 + g[i][0];
                d1 = g[c - i - 1][1];
                g.Add(new double[2] { a1, d1 });
            }

            // the last closed point should be added, because the start point in
            // the next has been elimated
            a1 = 2 * PI * uangle;
            d1 = g[0][1];
            g.Add(new double[2] { a1, d1 });

            return g;
        }

        /// <summary>
        /// Convert the Locus to Wms trace in according on the install style
        /// </summary>
        /// <param name="sty"></param>
        /// <param name="vc"></param>
        /// <param name="g"></param>
        /// <returns> list of vector </returns>
        private static List<Vector> WmsTrace(double sty, Vector vc, List<double[]> g)
        {
            var vc0 = new List<Vector>();
            int direction;
            double xstep, zstep;

            if (sty == 0)
            {
                direction = 1;
                xstep = 0.0;
                zstep = MotionOperate.Zmillimeter;
            }
            else if (sty == 1)
            {
                direction = -1;
                xstep = 0.0;
                zstep = MotionOperate.Zmillimeter;
            }
            else if (sty == 2)
            {
                direction = 1;
                xstep = MotionOperate.Xmillimeter;
                zstep = 0.0;
            }
            else
            {
                direction = -1;
                xstep = MotionOperate.Xmillimeter;
                zstep = 0.0;
            }

            for (int i = 0; i < g.Count; i++)
            {
                var x = vc.X + g[i][1] * direction * xstep;
                var y = vc.Y;
                var z = vc.Z + g[i][1] * direction * zstep;
                var a = vc.A + g[i][0];

                vc0.Add(new Vector(x, y, z, a));
            }

            return vc0;
        }
    }
}
