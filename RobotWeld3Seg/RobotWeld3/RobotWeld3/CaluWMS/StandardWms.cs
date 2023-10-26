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
using RobotWeld3.Welding;
using System.Collections.Generic;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Standard Wms methods produced by the simple cases.
    /// </summary>
    internal class StandardWms
    {
        public StandardWms() { }

        /// <summary>
        /// Find a point list in the start wms, which from the start point
        ///   to the first one (include) whose power is not 0.
        /// The direction of hardware has been correct to positive value.  
        /// </summary>
        /// <param name="plds"> PointListData </param>
        /// <returns> List(DisPoint) </returns>
        internal static List<DisPoint> FindStartPoints(PointListData plds)
        {
            var pdl = new List<DisPoint>();

            for (int i = 0; i < plds.Count; i++)
            {
                double ax = plds[i].Vector.X;
                double ay = plds[i].Vector.Y;
                double az = plds[i].Vector.Z;
                double aa = plds[i].Vector.A;

                var vc = new Vector(ax, ay, az, aa);
                pdl.Add(new DisPoint(plds[i].Parameter, vc, plds[i].Linetype));

                if (plds[i].Power != 0) break;
            }

            return pdl;
        }

        /// <summary>
        /// Find a point list in the end wms, which from the last point
        ///   whose power is 0, to the end point.
        /// Intersect trace useful only, because the points has chosen in
        ///   a different way.
        /// </summary>
        /// <param name="plds"> PointListData </param>
        /// <returns> List(DisPoint) </returns>
        internal static List<DisPoint> FindIntersectEndPoints(PointListData plds)
        {
            var p1 = new DisPoint()
            {
                Power = 0,
                Frequency = plds[0].Frequency,
                Duty = plds[0].Duty,
                MaterialIndex = plds[0].MaterialIndex,
                Speed = plds[0].Speed,
                Wobble = plds[0].Wobble,
                Wire = plds[0].Wire,
                Thick = plds[0].Thick,
                Vector = new Vector(plds[1].Vector),
            };

            double ax = plds[0].Vector.X;
            double ay = plds[0].Vector.Y;
            double az = plds[0].Vector.Z;
            double aa = plds[0].Vector.A;

            var vc = new Vector(ax, ay, az, aa);
            var p0 = new DisPoint(plds[0].Parameter, vc, plds[0].Linetype);
            var pdl = new List<DisPoint>
            {
                p1,
                p0,
            };

            return pdl;
        }

        /// <summary>
        /// Find end point generally.
        /// </summary>
        /// <param name="plds"> display point list </param>
        /// <param name="vc"> vector of the last point in the normal wms </param>
        /// <returns> The point list to the prepared point </returns>
        internal static List<DisPoint> FindEndPoints(PointListData plds, Vector vc)
        {
            var p1 = new DisPoint()
            {
                Power = 0,
                Frequency = plds[0].Frequency,
                Duty = plds[0].Duty,
                MaterialIndex = plds[0].MaterialIndex,
                Speed = plds[0].Speed,
                Wobble = plds[0].Wobble,
                Wire = plds[0].Wire,
                Thick = plds[0].Thick,
                Vector = vc,
            };

            double ax = plds[0].Vector.X;
            double ay = plds[0].Vector.Y;
            double az = plds[0].Vector.Z;
            double aa = plds[0].Vector.A;

            var vc0 = new Vector(ax, ay, az, aa);
            var p0 = new DisPoint(plds[0].Parameter, vc0, plds[0].Linetype);

            var pdl = new List<DisPoint>
            {
                p1,
                p0,
            };

            return pdl;
        }

        /// <summary>
        /// The start wms
        /// </summary>
        /// <param name="order"> wms order </param>
        /// <param name="pdl"> point list </param>
        /// <returns> weld Move Section </returns>
        internal static void StartPointWms(int order, List<DisPoint> pdl, ref WeldMoveSection wms, in WeldingModel wmodel)
        {
            var vc = new List<Vector>();
            var sl = new List<double>();

            for (int i = 0; i < pdl.Count; i++)
            {
                vc.Add(pdl[i].Vector);
                sl.Add(pdl[i].Speed);
            }

            // Set the laser parameter.
            var ayp = new AnalysePoint();
            ayp.GetModel(wmodel);
            ayp.GetPoint(pdl, 0, pdl.Count - 1);
            (var ma, var wi, var lp) = ayp.CalculateParameter();
            wms = new WeldMoveSection(vc)
            {
                WmsIndex = order,
                LaserParameter = lp,
                Material = ma,
                Wire = wi,
                Speed = sl,
                Wobble = pdl[0].Wobble,
            };
        }

        /// <summary>
        /// The end wms
        /// </summary>
        /// <param name="order"> wms order </param>
        /// <param name="pdl"> point list </param>
        /// <returns> weld Move Section </returns>
        internal static WeldMoveSection EndPointWms(int order, List<DisPoint> pdl, in WeldingModel wmodel)
        {
            var vc = new List<Vector>();
            var sl = new List<double>();
            int ei = pdl.Count;
            for (int i = 0; i < ei; i++)
            {
                vc.Add(pdl[i].Vector);
                sl.Add(pdl[i].Speed);
            }

            // Set the laser parameter.
            var ayp = new AnalysePoint();
            ayp.GetModel(wmodel);
            ayp.GetPoint(pdl, 0, ei -1);
            (var ma, var wi, var lp) = ayp.CalculateParameter();
            var wms = new WeldMoveSection(vc)
            {
                WmsIndex = order,
                LaserParameter = lp,
                Material = ma,
                Wire = wi,
                Speed = sl,
                Wobble = pdl[ei - 1].Wobble,
            };

            return wms;
        }

        /// <summary>
        /// Find points in a line, include the start and the end.
        /// </summary>
        /// <param name="ptl"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns> point number </returns>
        internal static int FindLinePoint(in PointListData ptl, int start, int end)
        {
            if (end >= ptl.Count - 1) return ptl.Count - 1 - start;

            int i;
            for (i = start; i <= end; i++)
            {
                if (ptl[i].Linetype == 1) break;
            }

            return i - start;
        }

        /// <summary>
        /// Find points in an arc, includes the start, but not the end.
        /// </summary>
        /// <param name="ptl"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns> point number </returns>
        internal static int FindArcPoint(in PointListData ptl, int start, int end)
        {
            if (end >= ptl.Count - 1) return ptl.Count - 1 - start;

            int i;
            for (i = start; i <= end; i++)
            {
                if (ptl[i].Linetype == 0) break;
            }

            return i - start - 1;
        }

        /// <summary>
        /// if the angles between points are different, matched them
        ///   splist: speed coefficient.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static void LineSegment(ref List<Vector> vclist, ref List<double> splist, in List<Vector> vcIn)
        {
            int cn = vcIn.Count;
            vclist.AddRange(vcIn);
            if (vcIn[0].A == vcIn[cn - 1].A)
            {
                for (int i = 0; i < cn; i++)
                    splist.Add(1.0);
            }
            else
            {
                var dpair = AngleDistancePair(vcIn);
                var sp = CrdInterpolation.MatchSpeed(dpair);
                splist.AddRange(sp);
            }
        }

        /// <summary>
        /// split the vector to angle and distance pair, that being match
        ///   speed further.
        /// </summary>
        /// <param name="vcIn"> input vector </param>
        /// <returns> pair : angle and distance </returns>
        internal static List<double[]> AngleDistancePair(in List<Vector> vcIn)
        {
            var dpair = new List<double[]>();
            int cn = vcIn.Count;
            dpair.Add(new double[2] { 0, 0 });
            for (int i = 1; i < cn; i++)
            {
                double angle = vcIn[i].A - vcIn[i - 1].A;
                double dist = CrdInterpolation.DistanceTwoVector4D(vcIn[i], vcIn[i - 1]);
                var din = new double[2] { angle, dist };
                dpair.Add(din);
            }
            return dpair;
        }

        /// <summary>
        /// Interpolate the space arc.
        /// </summary>
        /// <param name="vclist"> ref output vector list </param>
        /// <param name="splist"> ref output speed coefficients list </param>
        /// <param name="vcIn"> input vector list </param>
        /// <returns> true: to treat the segment as line </returns>
        internal static bool ArcSegment(ref List<Vector> vclist, ref List<double> splist, in List<Vector> vcIn)
        {
            var t = new SArcInterpolate();
            bool ret = t.InterpolateArc(ref vclist, ref splist, in vcIn);

            return ret;
        }

        /// <summary>
        /// Obtain real world speed with the speed coefficients
        /// </summary>
        /// <param name="splist"> ref: output speed coefficient list </param>
        /// <param name="speed"> the base speed </param>
        internal static void RealSpeed(ref List<double> splist, double speed)
        {
            for (int i = 0; i < splist.Count; i++)
            {
                splist[i] *= speed;
            }
        }
    }
}
