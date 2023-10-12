///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AppModel;
using System.Collections.Generic;
using static System.Math;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Fundamental class for coordinate system transform
    /// 3D coordinate system, including rotation and translation.
    /// Interplate a 3D arc to many line fragment.
    /// </summary>
    internal class CoordTransform
    {
        private Dictionary<int, List<Point>>? _points;

        public CoordTransform() { }

        internal void PutPointList(Dictionary<int, List<Point>> p)
        {
            _points = p;
        }

        /// <summary>
        /// Complete points in the trace after the any one point has been given.
        /// Note: the point can not be the last one.
        /// </summary>
        /// <param name="vc"> vector of given point </param>
        /// <param name="cp"> the copy point list </param>
        internal void CompleteTrace(int index, Vector vc, ref List<Point> cp)
        {
            (int x, int y) = FindCenter(index, vc, out double deflection);

            if (deflection == 0) return;

            for (int i = 0; i < cp.Count; i++)
            {
                if (i == index) continue;
                double rd = TranslateCoordinate(i, x, y, out PolarVector pvc);
                PolarVector pp = CalculatePoint(rd, deflection, pvc);
                Point p = InverseTranslateCoordinate(i, x, y, pp);
                cp[i] = p;
            }
        }

        //
        // the X and Y coordinate of the iNum point
        //
        private PolarVector CalculatePoint(double rd, double defl, PolarVector pvc)
        {
            if (rd == 0 || _points == null) return new PolarVector();


            double angle = PI - 2 * defl;
            return new PolarVector(pvc.Radius, pvc.Angle + angle);
        }

        //
        // Return: the radius to the iNum point
        //
        private double TranslateCoordinate(int iNum, int x, int y, out PolarVector pvc)
        {
            if (_points == null)
            {
                pvc = new PolarVector();
                return 0;
            }

            double x0 = _points[1][iNum].vector.X;
            double y0 = _points[1][iNum].vector.Y;

            double xout = x0 - x;
            double yout = y0 - y;

            pvc = Vector.ToPolar(new Vector(xout, yout));
            return VectorDistance(x0, y0, new Vector(x, y));
        }

        //
        // Return: the iNum of point
        //
        private Point InverseTranslateCoordinate(int iNum, int x, int y, PolarVector pvc)
        {
            if (_points == null) return new Point();

            int lt = _points[1][iNum].LaserState;
            int ls = _points[1][iNum].LineType;
            var lp = _points[1][iNum].GetLaserParameter();

            double z0 = _points[1][iNum].vector.Z;
            var vc0 = PolarVector.ToVector(pvc);
            double x0 = vc0.X + x;
            double y0 = vc0.Y + y;

            var v0 = new Vector(x0, y0, z0);
            return new Point(lt, ls, lp , v0);
        }


        //
        // the distance that the point from the center in the horizonal plane
        //
        private double FindChord(int iNum, Vector vc)
        {
            if (_points == null) return 0;
            double x0 = _points[1][iNum].vector.X;
            double y0 = _points[1][iNum].vector.Y;
            double ra = VectorDistance(x0, y0, vc);
            return ra;
        }

        private static double VectorDistance(double x, double y, Vector vc)
        {
            return Sqrt(Pow(vc.X - x, 2) + Pow(vc.Y - y, 2));
        }

        //
        // Find the center in the horizonal plane.
        //
        private (int x, int y) FindCenter(int index, Vector vc, out double deflection)
        {
            int x, y;
            if (index <= 2)
                (x, y) = OneTraceCenter(index, vc, out deflection);
            else
                (x, y) = MoreTraceCenter(index, vc, out deflection);

            return (x, y);
        }

        //
        // Find the center on one trace only (the first trace).
        //
        private (int x, int y) OneTraceCenter(int index, Vector vc, out double deflection)
        {
            if (_points == null)
            {
                deflection = 0;
                return (0, 0);
            }

            double centralAngle = CentralAngle(index, vc);
            double realRadius = 0;
            deflection = 0;

            if (centralAngle != 0)
            {
                realRadius = RealRadius(index, vc, centralAngle);
                deflection = (PI - centralAngle) / 2.0;
            }

            int signZ;
            int x = 0;
            int y = 0;
            if (realRadius != 0)
            {
                double x0 = _points[1][index].vector.X;
                double y0 = _points[1][index].vector.Y;
                signZ = ChordDirection(index, vc);
                (x, y) = CoordinateByAngle(realRadius, signZ * deflection, new Vector(x0, y0), vc);
            }

            return (x, y);
        }

        //
        // the angel is limited in 0 - PI/2.
        //
        private double CentralAngle(int index, Vector vc)
        {
            if (_points == null) return 0;

            // pre-determined 
            double preAngle = 2 * PI / (DaemonModel.TraceCount + 1);

            // the point should be at a node, whose central angle is one
            //   or several times of pre-angle.
            double realAngle = Asin(0.5 * FindChord(index, vc) / DaemonModel.VaneRadius);
            int stp = (int)(realAngle / preAngle + 0.5);

            return stp * preAngle;
        }

        private double RealRadius(int index, Vector vc, double angle)
        {
            if (_points == null) return 0;

            double x0 = _points[1][index].vector.X;
            double y0 = _points[1][index].vector.Y;
            double chordLength = VectorDistance(x0, y0, vc);

            return 0.5 * chordLength / Sin(angle);
        }

        //
        // Find the direction of the chord by the cross multiplication of
        //   x-x2 line and x1-x2 line. That is the angle from vc to the first
        //   point list.
        // Return: the direction of Z.
        //   which is the same as the angle from the x0 to the center.
        //   In the right hand system, when the chord with a counter-clockwise
        //   direction, the Z direction is positive.
        //
        private int ChordDirection(int index, Vector vc)
        {
            if (_points == null || index >= _points.Count - 1) return 0;

            // point 3
            double x0 = _points[1][index].vector.X;
            double y0 = _points[1][index].vector.Y;

            // point 2
            double x1 = _points[1][index + 1].vector.Y;
            double y1 = _points[1][index + 1].vector.Y;

            double r = (x1 - vc.X) * (y0 - y1) - (y1 - vc.Y) * (x0 - x1);

            if (r < 0) return -1;
            else if (r > 0) return 1;
            else return 0;
        }

        //
        // Find the center on at least two traces.
        // More accurate by using the average value.
        //
        private (int x, int y) MoreTraceCenter(int index, Vector vc, out double deflection)
        {
            (int x, int y) = OneTraceCenter(index, vc, out deflection);

            if (_points != null && _points[2] != null)
            {
                double x0 = _points[2][index].vector.X;
                double y0 = _points[2][index].vector.Y;
                (int x1, int y1) = OneTraceCenter(index, new Vector(x0, y0), out deflection);

                if (x1 != 0 && y1 != 0)
                {
                    x = (x1 + x)/ 2;
                    y = (y1 + y)/ 2;
                }
            }

            return (x, y);
        }

        //
        // added X and Y coordinate by a deflection angle and a step length.
        //
        private static (int x, int y) CoordinateByAngle(double length, double deflection, Vector vc0, Vector vc)
        {
            double a = Atan((vc0.Y - vc.Y) / (vc0.X - vc.X));
            int x = (int)(vc0.X + length * Sin(PI - a - deflection));
            int y = (int)(vc0.Y + length * Cos(PI - a - deflection));

            return (x, y);
        }
    }
}
