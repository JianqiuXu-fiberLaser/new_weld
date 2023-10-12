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
using System;
using System.Collections.Generic;
using System.Reflection;
using static System.Math;

namespace RobotWeld2.AlgorithmsBase
{
    internal class AutoMatchPoint
    {
        private Dictionary<int, List<Point>>? _points;

        public AutoMatchPoint() { }

        internal void PutPointList(Dictionary<int, List<Point>> p)
        {
            _points = p;
        }

        internal void CompleteTrace(bool trdir)
        {
            if (_points == null) return;

            (double cx, double cy) = FindCenter(trdir);
            if (cx == 0 && cy == 0) return;

            int ti;
            if (trdir) ti = -1;
            else ti = 1;

            for (int i = ti + ti; Abs(i) <= DaemonModel.TraceCount; i += ti)
            {
                double centralAngle = CalCentralAngle(i, i - ti, new Vector(cx, cy));

                for (int j = 1; j < _points[i].Count; j++)
                {
                    double rd = TranslateCoordinate(i - ti, j, cx, cy, out PolarVector pvc);
                    PolarVector pv2 = CalculatePoint(rd, centralAngle, pvc);
                    _points[i][j] = InverseTranslateCoordinate(i - ti, j, cx, cy, pv2);
                }
            }
        }

        private (double cx, double cy) FindCenter(bool trdir)
        {
            if (_points == null) return (0, 0);

            double x, y;
            if (ExamPoint(trdir))
            {
                (x, y) = ThreeCenter(trdir);
            }
            else
            {
                var cdt = new CoordTransform();
                cdt.PutPointList(_points);
                (x, y) = cdt.FindCenter(trdir);
            }

            return (x, y);
        }

        //
        // if there more than 3 trace have the 1st point, return true.
        //
        private bool ExamPoint(bool trdir)
        {
            if (_points == null) return false;

            int ti;
            if (trdir) ti = -1;
            else ti = 1;

            int pc = 0;
            for (int i = ti; Abs(i) <= DaemonModel.TraceCount; i += ti)
            {
                if (_points[i][0].vector.X != 0) pc++;
            }

            if (pc <= 2) return false;
            return true;
        }

        private double CalCentralAngle(int iNum, int iN0, Vector vcc)
        {
            if (_points == null) return 0;

            var vl = _points[iN0][0].vector;
            var vp = _points[iNum][0].vector;

            double sa = SweepAngle(vl, vp);
            return RealAngle(vcc, vl, vp, sa);
        }

        private static double RealAngle(Vector vcc, Vector v0, Vector vp, double sa)
        {
            // point 1
            double x0 = v0.X;
            double y0 = v0.Y;

            // point 2
            double x = vp.X;
            double y = vp.Y;

            // centre
            double xc = vcc.Y;
            double yc = vcc.Y;

            double rd = (x0 - xc) * (y - yc) - (y0 - yc) * (x - xc);

            /*            if (Abs(rd) < 2 * DaemonModel.MM)
                        {
                            double ld = (x0 - x) * (yc - y) - (y0 - y) * (xc - x);
                            if (ld > 0) return -Abs(sa);
                            else return Abs(sa);
                        }
                        else if (rd > 0)
                        {
                            return -Abs(sa);
                        }
                        else
                        {
                            return Abs(sa);
                        }*/

            return Abs(sa);
        }


        private double SweepAngle(Vector vl, Vector vp)
        {
            if (_points == null) return 0;

            // pre-determined 
            double preAngle = 2 * PI / DaemonModel.TraceCount;

            // the point should be at a node, whose central angle is one
            //   or several times of pre-angle.
            double asi = 0.5 * FindChord(vl, vp) / DaemonModel.VaneRadius;
            double realAngle;
            if (asi <= 1)
                realAngle = 2 * Asin(asi);
            else
                realAngle = PI;

            int stp = (int)(realAngle / preAngle + 0.5);
            return stp * preAngle;
        }

        //
        // the distance that the point from the center in the horizonal plane
        //
        private static double FindChord(Vector vl, Vector vp)
        {
            return Sqrt(Pow(vp.X - vl.X, 2) + Pow(vp.Y - vl.Y, 2));
        }

        private (double x, double y) ThreeCenter(bool trdir)
        {
            if (_points == null) return (0, 0);

            var vc = new List<Vector>();

            int ti;
            if (trdir) ti = -1;
            else ti = 1;

            int pc = 0;
            for (int i = ti; Abs(i) <= DaemonModel.TraceCount; i += ti)
            {
                if (_points[i][0].vector.X != 0)
                {
                    vc.Add(new Vector(_points[i][0].vector));
                    if (++pc >= 3) break;
                }
            }

            return Tcenter(vc);
        }

        private static (double x, double y) Tcenter(List<Vector> vc)
        {
            if (vc == null || vc.Count < 3) return (0, 0);

            double x1 = vc[0].X;
            double y1 = vc[0].Y;

            double x2 = vc[1].X;
            double y2 = vc[1].Y;

            double x3 = vc[2].X;
            double y3 = vc[2].Y;

            double dm = (x2 - x1) * (y3 - y2) + (x3 - x2) * (y2 - y1);

            if (dm == 0)
            {
                return (0, 0);
            }
            else
            {
                double a = x1 - x2;
                double b = y1 - y2;
                double c = x1 - x3;
                double d = y1 - y3;
                double e = ((x1 * x1 - x2 * x2) - (y2 * y2 - y1 * y1)) / 2;
                double f = ((x1 * x1 - x3 * x3) - (y3 * y3 - y1 * y1)) / 2;

                double x = -(d * e - b * f) / (b * c - a * d);
                double y = -(a * f - c * e) / (b * c - a * d);

                return (x, y);
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
        private double TranslateCoordinate(int itrc, int iNum, double cx, double cy, out PolarVector pvc)
        {
            if (_points == null)
            {
                pvc = new PolarVector();
                return 0;
            }

            double x = _points[itrc][iNum].vector.X;
            double y = _points[itrc][iNum].vector.Y;

            double xout = x - cx;
            double yout = y - cy;

            pvc = Vector.ToPolar(new Vector(xout, yout));
            return VectorDistance(cx, cy, new Vector(x, y));
        }

        //
        // Return: the iNum of point
        //
        private Point InverseTranslateCoordinate(int itrc, int iNum, double cx, double cy, PolarVector pvc)
        {
            if (_points == null) return new Point();

            int lt = _points[itrc][iNum].LaserState;
            int ls = _points[itrc][iNum].LineType;
            var lp = _points[itrc][iNum].GetLaserParameter();

            double z = _points[itrc][iNum].vector.Z;
            var vc0 = PolarVector.ToVector(pvc);
            double x = vc0.X + cx;
            double y = vc0.Y + cy;

            var v0 = new Vector(x, y, z);
            return new Point(lt, ls, lp, v0);
        }

        private static double VectorDistance(double x, double y, Vector vc)
        {
            return Sqrt(Pow(vc.X - x, 2) + Pow(vc.Y - y, 2));
        }
    }
}
