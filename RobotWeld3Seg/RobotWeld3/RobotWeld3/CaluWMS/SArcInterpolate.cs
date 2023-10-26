///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) new class to interpolate with a series of arc. It explors
//               all points in this segment, to give more accurate inter-
//               polation.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using System;
using System.Collections.Generic;
using static System.Math;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Interpolate a serise points with quadric curves. Because quadric is 
    ///   much complex, it is not enough to fit a space curve using three
    ///   points.
    /// Extenable to generall quadric curves.
    /// </summary>
    public class SArcInterpolate
    {
        private List<Vector>? _vclist;
        private List<double>? _splist;
        private List<Vector>? _vcIn;
        public SArcInterpolate() { }

        /// <summary>
        /// Interpolate the arc, with a series of suitable quadric curves.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="s"></param>
        /// <param name="vin"></param>
        /// <returns></returns>
        public bool InterpolateArc(ref List<Vector> v, ref List<double> s, in List<Vector> vin)
        {
            _vclist = v;
            _splist = s;
            _vcIn = vin;

            // which the arc type we need to interpolate?
            (int arcty, int tNum) = ArcType();

            List<Vector> vcout = arcty switch
            {
                1 => SpaceArc(),
                2 => CylinderArc(tNum),
                3 => _vcIn,
                4 => new List<Vector>(),
                _ => new List<Vector>(),
            };

            if (vcout.Count > 0 && arcty < 3)
            {
                _vclist.AddRange(vcout);
                var dpair = StandardWms.AngleDistancePair(in _vclist);
                var sp = CrdInterpolation.MatchSpeed(dpair);
                _splist.AddRange(sp);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Find the trace type to be interpolated.
        /// </summary>
        /// <returns> 1: Space arc; 2: cylinder; 3: rotate trace; 4: line; 0: unkown arc; </returns>
        private (int arcty, int tNum) ArcType()
        {
            if (_vcIn == null) return (0, 0);

            int arcty, tNum;
            (arcty, tNum) = IsLine();
            if (arcty == 0) (arcty, tNum) = IsRotateTrace();
            if (arcty == 0) (arcty, tNum) = IsCyclinder();
            if (arcty == 0) arcty = 1;
            return (arcty, tNum);
        }

        /// <summary>
        /// Is it a space line?
        /// Method: we calculate the distance of every points to the line connected 
        ///   the first two points. If all the distance are less than 0.1 mm, then
        ///   we think they are on the a line.
        ///   (2) when the points count equals 2, it is line too. Interpolate a arc
        ///   need at lest 3 points.
        /// </summary>
        /// <returns> arcty 0: not a line; 4: line </returns>
        private (int arcty, int tNum) IsLine()
        {
            if (_vcIn == null || _vcIn.Count <= 2) return (0, 0);

            (double l, double m, double n) = CrdInterpolation.TwoPointLine(_vcIn[0], _vcIn[1]);
            for (int i = 2; i < _vcIn.Count; i++)
            {
                var dist = CrdInterpolation.DistanceToLine(_vcIn[i], _vcIn[0], l, m, n);
                if (dist > 0.2 * MotionOperate.Xmillimeter) return (0, 0);
            }

            return (4, 0);
        }

        /// <summary>
        /// Is it a rotate trace.
        /// For rotate trace, moving linear, when rotate a cycle
        /// </summary>
        /// <returns> 0: not a line; 3: is a line </returns>
        private (int arcty, int tNum) IsRotateTrace()
        {
            if (_vcIn == null) return (0, 0);

            int cn = _vcIn.Count;
            if (Abs(Abs(_vcIn[cn - 1].A - _vcIn[0].A) / MotionOperate.OneCycle - 1) < 0.02)
            {
                return (3, 0);
            }

            return (0, 0);
        }

        /// <summary>
        /// The type of trace.
        /// Method: calculate the distance of every points to the plane. When the 
        ///   distance is less than 0.1 mm, we take them as a space 3D arc.
        ///   (2) when the points count equals 3, it is space 3D arc, too. No method
        ///   to distinct the quadric type with only 3 points.
        /// </summary>
        /// <returns> 0: space trace; 2: cylinder, 
        ///           For tNum = 1, there two choice: xz or xy, the choice is not important.
        ///           2, xy or zy; 
        ///           3, xz or yz;
        ///           0: unknow, back to space arc 
        /// </returns>
        private (int arcty, int tNum) IsCyclinder()
        {
            if (_vcIn == null || _vcIn.Count <= 3) return (0, 0);

            // the first 3 points to construct a plane
            var vc0 = _vcIn[0];
            var vc1 = _vcIn[1];
            var vc2 = _vcIn[2];
            (double a, double b, double c, double d) = CrdInterpolation.ThreePointPlane(vc0, vc1, vc2);

            for (int i = 3; i < _vcIn.Count; i++)
            {
                var dist = CrdInterpolation.DistanceToPlane(_vcIn[i], a, b, c, d);
                if (dist > 0.2 * MotionOperate.Xmillimeter) return (0, 0);
            }

            int tNum;
            if (c == 0) tNum = 1;
            else if (b == 0) tNum = 2;
            else if (a == 0) tNum = 3;
            else tNum = 0;

            return (2, tNum);
        }

        /// <summary>
        /// interpolate space trace with a series of 3D points
        /// </summary>
        /// <param name="vclist"> the whole vector list that be inserted </param>
        /// <param name="vcIn"> input vector </param>
        /// <returns> the result vector list </returns>
        private List<Vector> SpaceArc()
        {
            if (_vcIn == null) return new List<Vector>();

            var vlistcout = new List<Vector>();
            var pvl = new List<PlaneVector>();

            for (int i = 0; i < _vcIn.Count - 2; i += 2)
            {
                var pv1 = _vcIn[i];
                var pv2 = _vcIn[i + 1];
                var pv3 = _vcIn[i + 2];
                FlipSpaceCoordinate(in pv1, in pv2, in pv3, ref pvl, out double[,] tranMatrix);

                var vcout = TranslateBack3D(in pvl, in tranMatrix);
                MatchAngle(ref vcout, pv1.A, pv3.A);
                vlistcout.AddRange(vcout);
            }

            return vlistcout;
        }

        /// <summary>
        /// Calculate the cylinder Arc
        /// </summary>
        /// <param name="vclist"></param>
        /// <param name="tNum"></param>
        /// <returns> the vector list </returns>
        internal List<Vector> CylinderArc(int tNum)
        {
            if (_vcIn == null) return new List<Vector>();

            // sub branch to space arc.
            if (tNum == 0) return SpaceArc();

            var vcout = new List<Vector>();
            var pvl = new List<PlaneVector>();
            var pvz = new List<double>();

            TranslateForward(tNum, ref pvl, ref pvz);

            var vpvl = new List<PlaneVector>();
            var vpvz = new List<double>();
            CircleLocus(in pvl, in pvz, ref vpvl, ref vpvz);

            if (vpvl.Count > 0)
                TranslateBack(in vpvl, in vpvz, tNum, ref vcout);

            return vcout;
        }

        /// <summary>
        /// Translate the coordinate to the virtual plane in which an arc is interpolated.
        /// </summary>
        /// <param name="tNum"></param>
        /// <param name="pvl"></param>
        /// <param name="pvz"></param>
        private void TranslateForward(int tNum, ref List<PlaneVector> pvl, ref List<double> pvz)
        {
            if (_vcIn == null) return;

            // translate to a virtual plane.
            for (int i = 0; i < _vcIn.Count; i++)
            {
                double ax, ay, az;
                switch (tNum)
                {
                    case 1:    // xz plane
                        ax = _vcIn[i].X;
                        ay = _vcIn[i].Z;
                        az = _vcIn[i].Y;
                        break;
                    case 2:    // xy plane
                        ax = _vcIn[i].X;
                        ay = _vcIn[i].Y;
                        az = _vcIn[i].Z;
                        break;
                    default:    // yz plane
                        ax = _vcIn[i].Y;
                        ay = _vcIn[i].Z;
                        az = _vcIn[i].X;
                        break;
                }
                pvl.Add(new PlaneVector(ax, ay));
                pvz.Add(az);
            }
        }

        /// <summary>
        /// Translate the locus from a virtual plane to the real world coordinate
        /// </summary>
        /// <param name="pvl"></param>
        /// <param name="pvz"></param>
        /// <param name="tNum"></param>
        /// <param name="vco"></param>
        private static void TranslateBack(in List<PlaneVector> pvl, in List<double> pvz,
            int tNum, ref List<Vector> vco)
        {
            double ax, ay, az;
            for (int i = 0; i < pvl.Count; i++)
            {
                switch (tNum)
                {
                    case 1:    // xz plane
                        ax = pvl[i].X;
                        az = pvl[i].Y;
                        ay = pvz[i];
                        break;
                    case 2:    // xy plane
                        ax = pvl[i].X;
                        ay = pvl[i].Y;
                        az = pvz[i];
                        break;
                    default:    // yz plane
                        ay = pvl[i].X;
                        az = pvl[i].Y;
                        ax = pvz[i];
                        break;
                }
                vco.Add(new Vector(ax, ay, az));
            }
        }

        /// <summary>
        /// flip the space point to the points they sit.
        /// </summary>
        /// <param name="vc1"></param>
        /// <param name="vc2"></param>
        /// <param name="vc3"></param>
        /// <param name="pvl"> the locus in the virtual plane </param>
        /// <param name="tranMatrix"> the translate matrix </param>
        private static void FlipSpaceCoordinate(in Vector vc1, in Vector vc2, in Vector vc3,
            ref List<PlaneVector> pvl, out double[,] tranMatrix)
        {
            CalculateTransMatrix(out tranMatrix, vc1, vc2, vc3);

            var pv1 = TransTo2DPoint(in tranMatrix, in vc1);
            var pv2 = TransTo2DPoint(in tranMatrix, in vc2);
            var pv3 = TransTo2DPoint(in tranMatrix, in vc3);

            (pvl, _) = ArcThreeVector(pv1, pv2, pv3);
        }

        /// <summary>
        /// Translate a list of plane Vector to space 3D points.
        /// Note: the translate Matrix should be inversed.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns> the vector list </returns>
        private static List<Vector> TranslateBack3D(in List<PlaneVector> pvl, in double[,] tranMatrix)
        {
            var vcout = new List<Vector>();

            for (int i = 0; i < pvl.Count; i++)
            {
                double ax = tranMatrix[0, 0] * pvl[i].X + tranMatrix[0, 1] * pvl[i].Y + tranMatrix[0, 3];
                double ay = tranMatrix[1, 0] * pvl[i].X + tranMatrix[1, 1] * pvl[i].Y + tranMatrix[1, 3];
                double az = tranMatrix[2, 0] * pvl[i].X + tranMatrix[2, 1] * pvl[i].Y + tranMatrix[2, 3];
                vcout.Add(new Vector(ax, ay, az));
            }

            return vcout;
        }

        /// <summary>
        /// Calculate the translate matrix when 3 space points flip to the coordinate system that they
        ///   all sit on the XY plane.
        /// The origin is chose at p0.
        /// </summary>
        /// <param name="tranMaxtrix"> the return matrix </param>
        /// <param name="vc1"></param>
        /// <param name="vc2"></param>
        /// <param name="vc3"></param>
        private static void CalculateTransMatrix(out double[,] tranMaxtrix, Vector vc0, Vector vc1, Vector vc2)
        {
            // p0p1 \times p0p2
            double a = (vc1.Y - vc0.Y) * (vc2.Z - vc0.Z) - (vc2.Y - vc0.Y) * (vc1.Z - vc0.Z);
            double b = (vc2.X - vc0.X) * (vc1.Z - vc0.Z) - (vc1.X - vc0.X) * (vc2.Z - vc0.Z);
            double c = (vc1.X - vc0.X) * (vc2.Y - vc0.Y) - (vc1.Y - vc0.Y) * (vc2.X - vc0.X);
            double d = Sqrt(a * a + b * b + c * c);

            // W axis
            double sx, sy, sz;
            try
            {
                if (d == 0) throw new DivideByZeroException();
                sx = a / d;
                sy = b / d;
                sz = c / d;
            }
            catch (DivideByZeroException)
            {
                sx = 0;
                sy = 0;
                sz = 0;
            }

            // U axis
            a = (vc1.X - vc0.X);
            b = (vc1.Y - vc0.Y);
            c = (vc1.Z - vc0.Z);
            double dr = Sqrt(a * a + b * b + c * c);
            double rx, ry, rz;

            try
            {
                if (dr == 0) throw new DivideByZeroException();
                rx = a / dr;
                ry = b / dr;
                rz = c / dr;
            }
            catch (DivideByZeroException)
            {
                rx = 0;
                ry = 0;
                rz = 0;
            }

            // V axis = W X U
            double tx = sy * rz - sz * ry;
            double ty = sz * rx - sx * rz;
            double tz = sx * ry - sy * rx;

            tranMaxtrix = new double[4, 4] { { rx, tx, sx, vc0.X }, { ry, ty, sy, vc0.Y }, { rz, tz, sz, vc0.Z }, { 0, 0, 0, 1 } };
        }

        /// <summary>
        /// Translate a 3D point to a 2D plane vector.
        /// p(xyz) coordinate -> p'(uvw) coordinate
        /// </summary>
        /// <param name="t"> transMatrix </param>
        /// <param name="vc"> 3D vector </param>
        /// <returns> the plane vector </returns>
        private static PlaneVector TransTo2DPoint(in double[,] t, in Vector vc)
        {
            double vpx = t[0, 0] * t[0, 3] + t[1, 0] * t[1, 3] + t[2, 0] * t[2, 3];
            double vpy = t[0, 1] * t[0, 3] + t[1, 1] * t[1, 3] + t[2, 1] * t[2, 3];
            //double vpz = t[0, 2] * t[0, 3] + t[1, 2] * t[1, 3] + t[2, 2] * t[2, 3];

            var vx = t[0, 0] * vc.X + t[1, 0] * vc.Y + t[2, 0] * vc.Z - vpx;
            var vy = t[0, 1] * vc.X + t[1, 1] * vc.Y + t[2, 1] * vc.Z - vpy;
            //var vz = t[0, 2] * vc.X + t[1, 2] * vc.Y + t[2, 2] * vc.Z - vpz;

            return new PlaneVector(vx, vy);
        }

        /// <summary>
        /// interpolate a mutply-arc locus in a plane. 
        /// The vertical coordinate has matched.
        /// </summary>
        /// <param name="pvl"> input plane vector </param>
        /// <param name="pvz"> input z list </param>
        /// <param name="vpvl"> output plane vectors </param>
        /// <param name="vpvz"> output z list </param>
        private static void CircleLocus(in List<PlaneVector> pvl, in List<double> pvz,
            ref List<PlaneVector> vpvl, ref List<double> vpvz)
        {
            for (int i = 0; i < pvl.Count - 2; i += 2)
            {
                var pv1 = pvl[i];
                var pv2 = pvl[i + 1];
                var pv3 = pvl[i + 2];

                (List<PlaneVector> vout, double sAngle) = ArcThreeVector(pv1, pv2, pv3);

                // the arc does not exist.
                if (vout.Count == 0) return;

                vpvl.AddRange(vout);

                var pz1 = pvz[i];
                var pz2 = pvz[i + 1];
                var zout = MatchVerticalCoordinate(vout, pv1, pv2, sAngle, pz1, pz2);
                vpvz.AddRange(zout);
            }
        }

        /// <summary>
        /// Match the vertical coordinate to the circle arc.
        /// </summary>
        /// <param name="plv"></param>
        /// <param name="pz1"> Z coordinate of the start point </param>
        /// <param name="pz2"> Z coordinate of the end point </param>
        /// <returns> List of Z coordiante </returns>
        private static List<double> MatchVerticalCoordinate(List<PlaneVector> vcout, PlaneVector pv1, PlaneVector pv2, double sAngle,
            double pz1, double pz2)
        {
            var zout = new List<double>
            {
                pz1
            };

            // the cotangent of the slope angle \theta.
            // z0, the height of ellispe plane to the xy plane.
            var cotvalue = CrdInterpolation.SlopeAngle(pv1.X, pv2.X, pz1, pz2);
            double z0 = pz1 - pv1.X * cotvalue;
            for (int i = 1; i < vcout.Count; i++)
            {
                var d1 = z0 + vcout[i].X * cotvalue;
                zout.Add(d1);
            }

            return zout;
        }

        /// <summary>
        /// interpolate an arc in a plane with three points
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns> plane vector list and the start angle </returns>
        private static (List<PlaneVector> vout, double sAngle) ArcThreeVector(PlaneVector v1, PlaneVector v2, PlaneVector v3)
        {
            (var vout, double sAngle) = InterpolateCircle2D.OneArc(v1, v2, v3);
            return (vout, sAngle);
        }

        /// <summary>
        /// Match the A-axis rotate angle for three points arc, after be interpolated.
        /// The angle is thought linearly matched to the space distance.
        /// </summary>
        /// <param name="vclist"> the vector list with xyz </param>
        /// <param name="a1"> the angle of start point </param>
        /// <param name="a2"> the angle of end point </param>
        private static void MatchAngle(ref List<Vector> vclist, double a1, double a2)
        {
            double da = a2 - a1;

            // total arc length
            double tarc = 0;
            for (int i = 0; i < vclist.Count - 1; i++)
            {
                tarc += Sqrt(Pow(vclist[i + 1].X - vclist[i].X, 2) + Pow(vclist[i + 1].Y - vclist[i].Y, 2) + Pow(vclist[i + 1].Z - vclist[i].Z, 2));
            }

            // arc length until to [i]
            double tri = 0;
            for (int i = 0; i < vclist.Count - 1; ++i)
            {
                vclist[i].A = a1 + tri * da / tarc;
                tri += Sqrt(Pow(vclist[i + 1].X - vclist[i].X, 2) + Pow(vclist[i + 1].Y - vclist[i].Y, 2) + Pow(vclist[i + 1].Z - vclist[i].Z, 2));
            }

            // the last point
            vclist[^1].A = a2;
        }
    }
}
