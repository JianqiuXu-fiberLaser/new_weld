///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) new class to interpolate circle in a plane. The plane
//               is virtual plane with AX and AY axes.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.AppModel;
using RobotWeld3.Motions;
using System;
using System.Collections.Generic;
using static System.Math;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Fundemental static methods to interpolate a circle in a plane.
    /// </summary>
    public class InterpolateCircle2D
    {
        public InterpolateCircle2D() { }

        /// <summary>
        /// Interpolate an arc in a plane with three points
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns> the vector list and the start angle </returns>
        public static (List<PlaneVector> plout, double sAngle) OneArc(PlaneVector v1, PlaneVector v2, PlaneVector v3)
        {
            (double ax, double ay) = FindCenter(v1, v2, v3);

            // the arc is line.
            if (ax == int.MaxValue) return (new List<PlaneVector>(), 0);

            var plout = new List<PlaneVector>();
            double sAngle = SubArc(v1, v2, v3, ax, ay, ref plout);

            return (plout, sAngle);
        }

        /// <summary>
        /// Find the center of these 3 points, if the circle exist.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns> if the circle is not exist, return (IntMax, IntMax). </returns>
        private static (double ax, double ay) FindCenter(PlaneVector v1, PlaneVector v2, PlaneVector v3)
        {
            double x1 = v1.X;
            double y1 = v1.Y;

            double x2 = v2.X;
            double y2 = v2.Y;

            double x3 = v3.X;
            double y3 = v3.Y;

            double dm = (x1 - x2) * (y3 - y2) - (x3 - x2) * (y1 - y2);

            if (dm == 0)
            {
                MainModel.AddInfo("圆弧不存在");
                return (int.MaxValue, int.MaxValue);
            }
            else
            {
                double A = x1 - x2;
                double B = y1 - y2;
                double C = x1 - x3;
                double D = y1 - y3;
                double E = (x1 * x1 - x2 * x2 - (y2 * y2 - y1 * y1)) / 2;
                double F = (x1 * x1 - x3 * x3 - (y3 * y3 - y1 * y1)) / 2;

                double x = -(D * E - B * F) / (B * C - A * D);
                double y = -(A * F - C * E) / (B * C - A * D);

                return (x, y);
            }
        }

        /// <summary>
        /// A part of arc between v1 and v3. The arc is always less than half circle.
        /// It include the extrema case of half circle.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="ax"> X coordinate of center </param>
        /// <param name="ay"> Y coordinate of center </param>
        /// <param name="plout"> the outpur PlaneVector list </param>
        /// <returns> the rotate angle of start point </returns>
        private static double SubArc(PlaneVector v1, PlaneVector v2, PlaneVector v3, double ax, double ay,
            ref List<PlaneVector> plout)
        {
            // shift the coordinate to the arc centre.
            double x1 = v1.X - ax;
            double y1 = v1.Y - ay;

            double x2 = v2.X - ax;
            double y2 = v2.Y - ay;

            double x3 = v3.X - ax;
            double y3 = v3.Y - ay;

            double ra = Sqrt(x1 * x1 + y1 * y1);

            // the angle of point is set among 0 ~ 2*PI.
            double a3 = TAngle(y3, x3);
            double a2 = TAngle(x2, y2);
            double a1 = TAngle(y1, x1);

            // if the arc pass the X axis, it should add 2*PI to a3.
            if (Abs(a3 - a1) < Abs(a2 - a1))
            {
                if (a3 < a1) a3 += 2 * PI;
                else a3 -= 2 * PI;
            }

            // the angle step to ensure the error in smaller than 0.1 mm
            double angle = a3 - a1;

            double detAngle;
            if (a3 > a1) detAngle = 10.0 / Sqrt(10 * ra);
            else detAngle = -10.0 / Sqrt(10 * ra);

            // the start point
            plout.Add(new PlaneVector(ax + x1, ay + y1));

            // the accumelate swept angle
            double sn = angle / detAngle;
            double sumAngle = a1;
            for (int i = 1; i < sn; i++)
            {
                sumAngle += detAngle;
                double dx = ax + ra * Cos(sumAngle);
                double dy = ay + ra * Sin(sumAngle);
                plout.Add(new PlaneVector(dx, dy));
            }

            // if the distance to V3 is larger than 0.5 sn, then add it.
            // otherwise, ignore it.
            int cn = plout.Count;
            var da = CrdInterpolation.DistanceTwoVector2D(plout[cn - 1], new PlaneVector(x3, y3));
            if (da > sn * 0.5) plout.Add(new PlaneVector(ax + x3, ay + y3));

            return a1;
        }

        /// <summary>
        /// True angle from arc tangent function, which is in the range
        ///   from 0 ~ 2*pi.
        /// </summary>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns> angle: 0 ~ 2*pi</returns>
        private static double TAngle(double y, double x)
        {
            double angle;

            if (x > 0)
            {
                if (y >= 0) angle = Atan(y / x);
                else angle = 2 * PI + Atan(y / x);
            }
            else if (x < 0)
            {
                angle = PI + Atan(y / x);
            }
            else
            {
                if (y > 0) angle = PI / 2;
                else if (y < 0) angle = 3 * PI / 2;
                else angle = double.NaN;
            }

            return angle;
        }
    }
}
