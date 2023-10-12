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
        private List<Point>? _stPoint;

        public CoordTransform() { }

        /// <summary>
        /// Put ref of _pointList to the Class of Coordinate Transform
        /// </summary>
        /// <param name="p"></param>
        internal void PutPointList(Dictionary<int, List<Point>> p)
        {
            _points = p;
            _stPoint = new List<Point>();
        }

        /// <summary>
        /// Complete points in the trace after the any one point has been given.
        /// Note: the point can not be the last one.
        /// </summary>
        /// <param name="trdir"> the side number, left:false, right:true </param>
        /// <param name="trIndex"> the trace number </param>
        /// <param name="index"> the point index </param>
        /// <param name="vc"> vector of given point </param>
        /// <param name="cp"> the copy point list </param>
        internal void CompleteTrace(bool trdir, int trIndex, int index, Vector vc, ref List<Point> cp)
        {
            (double cx, double cy) = FindCenter(trdir, trIndex, vc, out double centralAngle);
            if (centralAngle == 0) return;

            for (int i = 0; i < cp.Count; i++)
            {
                if (i == index)
                {
                    Point p1 = KeepPointFrom(trdir, i, vc);
                    cp[i] = p1;
                }
                else
                {
                    double rd = TranslateCoordinate(i, cx, cy, out PolarVector pvc);
                    PolarVector pv2 = CalculatePoint(rd, centralAngle, pvc);
                    cp[i] = InverseTranslateCoordinate(i, cx, cy, pv2);
                }
            }
        }

        //
        // the X and Y coordinate of the iNum point
        //
        private PolarVector CalculatePoint(double rd, double centAn, PolarVector pvc)
        {
            if (rd == 0 || _points == null) return new PolarVector();

            return new PolarVector(pvc.Angle + centAn, pvc.Radius);
        }

        //
        // Return: the radius to the iNum point
        //
        private double TranslateCoordinate(int iNum, double cx, double cy, out PolarVector pvc)
        {
            if (_stPoint == null)
            {
                pvc = new PolarVector();
                return 0;
            }

            double x = _stPoint[iNum].vector.X;
            double y = _stPoint[iNum].vector.Y;

            double xout = x - cx;
            double yout = y - cy;

            pvc = Vector.ToPolar(new Vector(xout, yout));
            return VectorDistance(cx, cy, new Vector(x, y));
        }

        //
        // Return: the iNum of point
        //
        private Point InverseTranslateCoordinate(int iNum, double cx, double cy, PolarVector pvc)
        {
            if (_stPoint == null) return new Point();

            int lt = _stPoint[iNum].LaserState;
            int ls = _stPoint[iNum].LineType;
            var lp = _stPoint[iNum].GetLaserParameter();

            double z = _stPoint[iNum].vector.Z;
            var vc0 = PolarVector.ToVector(pvc);
            double x = vc0.X + cx;
            double y = vc0.Y + cy;

            var v0 = new Vector(x, y, z);
            return new Point(lt, ls, lp, v0);
        }

        //
        // Keep the coordinate to the point
        //
        private Point KeepPointFrom(bool trdir, int iNum, Vector vc)
        {
            if (_points == null) return new Point();

            int ti;
            if (trdir) ti = -1;
            else ti = 1;

            int lt = _points[ti][iNum].LaserState;
            int ls = _points[ti][iNum].LineType;
            var lp = _points[ti][iNum].GetLaserParameter();

            double x0 = vc.X;
            double y0 = vc.Y;
            double z0 = vc.Z;

            var v0 = new Vector(x0, y0, z0);
            return new Point(lt, ls, lp, v0);
        }

        //
        // the distance that the point from the center in the horizonal plane
        //
        private double FindChord(Vector vc)
        {
            if (_points == null || _stPoint == null) return 0;

            double x0 = _stPoint[0].vector.X;
            double y0 = _stPoint[0].vector.Y;
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
        private (double x, double y) FindCenter(bool trdir, int trIndex, Vector vc, out double centralAngle)
        {
            double x = 0;
            double y = 0;
            centralAngle = 0;

            int ti;
            if (trdir) ti = -1;
            else ti = 1;

            if (Abs(trIndex) > 1 && _points != null)
            {
                _stPoint = _points[trIndex - ti];
                (x, y) = OneTraceCenter(vc, out centralAngle);
            }

            return (x, y);
        }

        public (double x, double y) FindCenter(bool trdir)
        {
            double x = 0;
            double y = 0;

            int ti;
            if (trdir) ti = -1;
            else ti = 1;

            if (_points != null)
            {
                _stPoint = _points[ti]?? new List<Point>();
                var vc = _points[ti + ti][0].vector ?? new Vector();
                (x, y) = OneTraceCenter(vc, out double centralAngle);
                if (centralAngle == 0) return (0,0);
            }

            return (x, y);
        }

        //
        // Find the center on one trace only (the first trace).
        //
        private (double x, double y) OneTraceCenter(Vector vc, out double centralAngle)
        {
            if (_stPoint == null)
            {
                centralAngle = 0;
                return (0, 0);
            }

            double sweptAngle = CentralAngle(vc);
            double realRadius = 0;
            double deflection = 0;

            if (sweptAngle != 0)
            {
                realRadius = RealRadius(vc, sweptAngle);
                deflection = (PI - sweptAngle) / 2.0;
            }

            int signZ;
            double x = 0;
            double y = 0;
            if (realRadius != 0)
            {
                double x0 = _stPoint[0].vector.X;
                double y0 = _stPoint[0].vector.Y;
                signZ = ChordDirection(vc);
                (x, y) = CoordinateByAngle(realRadius, signZ * deflection, new Vector(x0, y0), vc);
            }

            centralAngle = CalCentralAngle(new Vector(x, y), vc, sweptAngle);
            return (x, y);
        }

        //
        // The angle sweptting to next trace.
        // the angle is limited in 0 ~ PI.
        //
        private double CentralAngle(Vector vc)
        {
            if (_points == null) return 0;

            // pre-determined 
            double preAngle = 2 * PI / DaemonModel.TraceCount;

            // the point should be at a node, whose central angle is one
            //   or several times of pre-angle.
            double asi = 0.5 * FindChord(vc) / DaemonModel.VaneRadius;
            double realAngle;
            if (asi <= 1)
                realAngle = 2 * Asin(asi);
            else
                realAngle = PI;

            int stp = (int)(realAngle / preAngle + 0.5);
            return stp * preAngle;
        }

        private double RealRadius(Vector vc, double angle)
        {
            if (_points == null || _stPoint == null) return 0;

            double x0 = _stPoint[0].vector.X;
            double y0 = _stPoint[0].vector.Y;
            double chordLength = VectorDistance(x0, y0, vc);

            return 0.5 * chordLength / Sin(0.5 * angle);
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
        private int ChordDirection(Vector vc)
        {
            if (_stPoint == null || _stPoint.Count <= 1) return 0;

            // point 2
            double x0 = _stPoint[0].vector.X;
            double y0 = _stPoint[0].vector.Y;

            // point 3
            double x1 = _stPoint[1].vector.Y;
            double y1 = _stPoint[1].vector.Y;

            double r = (vc.X - x0) * (y1 - y0) - (vc.Y - y0) * (x1 - x0);

            // the angle of center is the negative of that of p2->p1->p
            if (r < 0) return -1;
            else if (r > 0) return 1;
            else return 0;
        }

        private double CalCentralAngle(Vector vcc, Vector vc, double sweptAngle)
        {
            if (_stPoint == null) return 0;

            // point 1
            double x0 = _stPoint[0].vector.X;
            double y0 = _stPoint[0].vector.Y;

            // point 2
            double x = vc.X;
            double y = vc.Y;

            // centre
            double xc = vcc.Y;
            double yc = vcc.Y;

            double rd = (x0 - xc) * (y - yc) - (y0 - yc) * (x - xc);

            if (rd > 0) return -Abs(sweptAngle);
            else
                return Abs(sweptAngle);
        }

        //
        // added X and Y coordinate by a deflection angle and a step length.
        //
        private static (double x, double y) CoordinateByAngle(double length, double deflection, Vector vc0, Vector vc)
        {
            // a, the angle of p -> p0
            // deflection, the angle of P0->c
            double a;
            double xt = vc.X - vc0.X;
            double yt = vc.Y - vc0.Y;
            if (xt == 0)
            {
                if (yt > 0) a = PI / 2;
                else a = 3 * PI / 2;
            }
            else if (xt < 0)
            {
                if (yt < 0) a = -PI + Atan(yt / xt);
                else a = PI + Atan(yt / xt);
            }
            else
            {
                a = Atan(yt / xt);
            }

            double x = vc0.X + length * Cos(a - deflection);
            double y = vc0.Y + length * Sin(a - deflection);

            return (x, y);
        }
    }
}
