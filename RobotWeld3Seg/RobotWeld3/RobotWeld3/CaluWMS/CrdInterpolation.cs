///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver 2.2: revise the point list to match single workpackage file.
// Ver. 3.0: (1) all-in-one data structure.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Math;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Fundamental static methods for interpolation
    /// </summary>
    internal static class CrdInterpolation
    {
        /// <summary>
        /// Delta angle to interpolate an arc with an acceptable deviation.
        /// This method is corrected only for the same screw pitch of XYZ axis.
        /// </summary>
        /// <param name="radius"> unit in millimeter </param>
        /// <returns> the angle in radian </returns>
        internal static double DeltaAngle(double radius)
        {
            return 2.0 * Sqrt(0.1 / radius);
        }

        /// <summary>
        /// Calculate rotate coefficients of multiply (>= 3 points) 
        ///   continous line sections. In those sections, the synthesis speed
        ///   is given and smoothly rotation are required.
        /// </summary>
        /// <param name="lt"> the rotate angle and distance pairs. 
        ///                   Note, the distance of the first point, even 
        ///                   it is zero, should be given.
        /// </param>
        /// <returns> rotate coefficients. 
        ///           Note: the speed of the first point has been added. 
        /// </returns>
        internal static List<double> MatchSpeed(List<double[]> lt)
        {
            var spc = new List<double>();
            if (lt.Count < 3) return spc;

            double da0 = lt[1][0] - lt[0][0];
            double dl0 = lt[1][1] - lt[1][0];
            double c0 = 1;
            if (da0 != 0) c0 = Sqrt(1 + Pow(dl0 / da0, 2));
            spc.Add(MotionOperate.SmoothCoefficient * 1.0);

            for (int i = 2; i < lt.Count; i++)
            {
                double anglei = lt[i][0];
                double ci = 1;
                if (anglei != 0) ci = MotionOperate.SmoothCoefficient * Sqrt(1 + Pow(lt[i][1] / anglei, 2));
                spc.Add(ci / c0);
            }

            // the speed for last point, which is not important, because usually,
            //   it stop to rotate at this point.
            // If it requires the smooth to the next wms, it should keep them in
            //   one wms.
            spc.Add(MotionOperate.SmoothCoefficient * 1.0);
            return spc;
        }

        /// <summary>
        /// calculate rotate speed with speed coefficients
        /// </summary>
        /// <param name="speed"> synthesis speed </param>
        /// <param name="spc"> the coefficient </param>
        /// <returns> real rotate speed </returns>
        internal static List<double> RotateSpeed(double speed, List<double> spc)
        {
            var spd = new List<double>();
            for (int i = 0; i < spc.Count; i++)
            {
                spd.Add(MotionOperate.SmoothCoefficient * speed * spc[i]);
            }

            return spd;
        }

        /// <summary>
        /// Distance between two vectors in 4D coordinate system.
        /// </summary>
        /// <param name="vc1"> vector 1 </param>
        /// <param name="vc2"> vector 2 </param>
        /// <returns> Absolute distance </returns>
        internal static double DistanceTwoVector4D(Vector vc1, Vector vc2)
        {
            if (vc1 == null || vc2 == null) return 0;
            return Sqrt(Pow(vc2.X - vc1.X, 2) + Pow(vc2.Y - vc1.Y, 2) + Pow(vc2.Z - vc1.Z, 2) + Pow(vc2.A - vc1.A, 2));
        }

        /// <summary>
        /// Distance between two vectors in XYZ coordinate system
        /// </summary>
        /// <param name="vc1"> vector 1 </param>
        /// <param name="vc2"> vector 2 </param>
        /// <returns> Absolute distance </returns>
        internal static double DistanceTwoVector(Vector vc1, Vector vc2)
        {
            if (vc1 == null || vc2 == null) return 0;
            return Sqrt(Pow(vc2.X - vc1.X, 2) + Pow(vc2.Y - vc1.Y, 2) + Pow(vc2.Z - vc1.Z, 2));
        }

        /// <summary>
        /// Distance between two plane vectors.
        /// </summary>
        /// <param name="vc1"></param>
        /// <param name="vc2"></param>
        /// <returns> Absolute distance </returns>
        internal static double DistanceTwoVector2D(PlaneVector vc1, PlaneVector vc2)
        {
            if (vc1 == null || vc2 == null) return 0;
            return Sqrt(Pow(vc2.X - vc1.X, 2) + Pow(vc2.Y - vc1.Y, 2));
        }

        /// <summary>
        /// A vector between two vectors at the point that splits the distance with the ratio. 
        /// </summary>
        /// <param name="vc1"> vector 1 </param>
        /// <param name="vc2"> vector 2 </param>
        /// <param name="ratio"> the ratio the point to vc1 to the whole distance </param>
        /// <returns> new vector </returns>
        internal static Vector MidTwoVector(Vector vc1, Vector vc2, double ratio)
        {
            double x = vc1.X + (vc2.X - vc1.X) * ratio;
            double y = vc1.Y + (vc2.Y - vc1.Y) * ratio;
            double z = vc1.Z + (vc2.Z - vc1.Z) * ratio;
            double a = vc1.A + (vc2.A - vc1.A) * ratio;

            return new Vector(x, y, z, a);
        }

        /// <summary>
        /// Calculate the distance to the second vector with distance ratio.
        /// </summary>
        /// <param name="vc0"></param>
        /// <param name="vc1"></param>
        /// <param name="rtdist"></param>
        /// <returns> vector from vc 0 </returns>
        internal static Vector CalculateResidualDistance(Vector vc0, Vector vc1, double rtdist)
        {
            double tdist = DistanceTwoVector4D(vc0, vc1);
            double ratio = rtdist / tdist;
            return MidTwoVector(vc0, vc1, ratio);
        }

        /// <summary>
        /// Distance from a point to a line described by x/l = y/m = z/n 
        /// </summary>
        /// <param name="vc0"> point vector </param>
        /// <param name="l"> x-axis direct cosin </param>
        /// <param name="m"> y-axis direct cosin </param>
        /// <param name="n"> z-axis direct cosin </param>
        /// <returns> the distance </returns>
        internal static double DistanceToLine(Vector vc, Vector vc0, double l, double m, double n)
        {
            var iv = (vc.Y - vc0.Y) * n - (vc.Z - vc0.Z) * m;
            var jv = (vc.Z - vc0.Z) * l - (vc.X - vc0.X) * n;
            var kv = (vc.X - vc0.X) * m - (vc.Y - vc0.Y) * l;
            return Sqrt(iv * iv + jv * jv + kv * kv);
        }

        /// <summary>
        /// Line functon connected to two points.
        /// Normalized to unit direction.
        /// </summary>
        /// <param name="vc0"> vector 0 </param>
        /// <param name="vc1"> vector 1 </param>
        /// <returns> x/l = y/m = z/n, the direct cosin of unit vector of the line </returns>
        internal static (double l, double m, double n) TwoPointLine(Vector vc0, Vector vc1)
        {
            var s = DistanceTwoVector(vc0, vc1);
            var l = (vc1.X - vc0.X) / s;
            var m = (vc1.Y - vc0.Y) / s;
            var n = (vc1.Z - vc0.Z) / s;
            return (l, m, n);
        }

        /// <summary>
        /// Equation of plane with three points. ax + by + cz = d
        /// </summary>
        /// <param name="vc0"></param>
        /// <param name="vc1"></param>
        /// <param name="vc2"></param>
        /// <returns> ax + by + cz = d, the intercept of plane to x, y and z axis </returns>
        internal static (double a, double b, double c, double d) ThreePointPlane(Vector vc0, Vector vc1, Vector vc2)
        {
            double a = (vc1.Y - vc0.Y) * (vc2.Z - vc0.Z) - (vc2.Y - vc0.Y) * (vc1.Z - vc0.Z);
            double b = (vc1.X - vc0.X) * (vc2.Z - vc0.Z) - (vc2.X - vc0.X) * (vc1.Z - vc0.Z);
            double c = (vc1.X - vc0.X) - (vc2.Y - vc0.Y) - (vc1.X - vc0.X) * (vc1.Y - vc0.Y);
            double d = vc0.X * (vc1.Y * vc2.Z - vc2.Y * vc1.Z)
                - vc1.X * (vc0.Y * vc2.Z - vc2.Y * vc0.Z)
                + vc2.X * (vc0.Y * vc1.Z) * (vc1.Y * vc0.Z);

            return (a, b, c, d);
        }

        /// <summary>
        /// The distance from a point to a plane given by ax + by + cz = d
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns> the distance </returns>
        internal static double DistanceToPlane(Vector vc, double a, double b, double c, double d)
        {
            var dist = a * vc.X + b * vc.Y + c * vc.Z - d;
            var mod = Sqrt(a * a + b * b + c * c);

            try
            {
                if (mod == 0) throw new DivideByZeroException();
                return dist / mod;
            }
            catch (DivideByZeroException)
            {
                return 0;
            }
        }

        /// <summary>
        /// the cotangent angle of the plane to the x axis, cot(z/x)
        /// </summary>
        /// <param name="vc1x"></param>
        /// <param name="vc2x"></param>
        /// <param name="pz1"></param>
        /// <param name="pz2"></param>
        /// <returns> cotangent of slope angle </returns>
        internal static double SlopeAngle(double vc1x, double vc2x, double pz1, double pz2)
        {
            double tvalue;
            try
            {
                if (vc1x == vc2x) throw new DivideByZeroException();
                tvalue = (pz2 - pz1) / (vc2x - vc1x);
            }
            catch (DivideByZeroException)
            {
                tvalue = 0;
            }

            return tvalue;
        }
    }
}
