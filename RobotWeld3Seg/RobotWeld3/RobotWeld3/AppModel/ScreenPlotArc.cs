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
using System.Collections.ObjectModel;
using System.Windows.Media;
using static System.Math;

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// Plot arcs in the screen
    /// </summary>
    internal class ScreenPlotArc
    {
        private PathFigureCollection? _pfFrog;
        private PathFigureCollection? _pfLaser;

        /// <summary>
        /// Plot arcs in the screen.
        /// </summary>
        internal ScreenPlotArc() { }

        /// <summary>
        /// The entry method to plot acrs.
        /// </summary>
        /// <param name="pf"></param>
        /// <param name="pf2"></param>
        internal void CreateArcs(ref PathFigureCollection pf, ref PathFigureCollection pf2)
        {
            _pfFrog = pf;
            _pfLaser = pf2;
        }

        /// <summary>
        /// Put in the point list.
        /// </summary>
        /// <param name="ltv"></param>
        internal void PutVector(ObservableCollection<LtVector> ltv)
        {
            FilterVectorArc(ltv);
        }

        /// <summary>
        /// Filter the point only for arcs
        /// </summary>
        /// <param name="ltv"></param>
        private void FilterVectorArc(ObservableCollection<LtVector> ltv)
        {
            int i = 0;
            int vstep;
            while (i < ltv.Count)
            {
                if (ltv[i].LineType == 1) vstep = ArcSegment(i, ltv);
                else vstep = 1;
                i += vstep;
            }
        }

        /// <summary>
        /// An arc segment between two arcs
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ltv"></param>
        /// <returns></returns>
        private int ArcSegment(int i, ObservableCollection<LtVector> ltv)
        {
            var ltvector = new ObservableCollection<LtVector>();

            int j;
            for (j = i; j < ltv.Count; j++)
            {
                if (ltv[j].LineType == 0) break;
                ltvector.Add(ltv[j]);
            }

            PlotArcs(ltvector);

            if (j == ltv.Count) return j - i;
            else return j - i;
        }

        /// <summary>
        /// Plot each arcs
        /// </summary>
        /// <param name="ltvector"></param>
        private void PlotArcs(ObservableCollection<LtVector> ltvector)
        {
            if (_pfFrog == null || _pfLaser == null) return;

            for (int i = 0; i < ltvector.Count - 2; i += 2)
            {
                var f = OneArc(ltvector[i], ltvector[i + 1], ltvector[i + 2]);
                if (ltvector[i].LaserState == 0) _pfFrog.Add(f);
                else _pfLaser.Add(f);
            }
        }

        /// <summary>
        /// One arc segment.
        /// </summary>
        /// <param name="lt1"></param>
        /// <param name="lt2"></param>
        /// <param name="lt3"></param>
        /// <returns></returns>
        private static PathFigure OneArc(LtVector lt1, LtVector lt2, LtVector lt3)
        {
            PathFigure mainpf = new()
            {
                StartPoint = new System.Windows.Point(lt1.X, lt1.Y)
            };

            ArcSegment ars = CalculateArc(lt1.X, lt2.X, lt3.X, lt1.Y, lt2.Y, lt3.Y);
            PathSegmentCollection psc = new() { ars };
            mainpf.Segments = psc;

            return mainpf;
        }

        /// <summary>
        /// Calculate arc radius and direction.
        /// </summary>
        /// <param name="sx1"></param>
        /// <param name="sx2"></param>
        /// <param name="sx3"></param>
        /// <param name="sy1"></param>
        /// <param name="sy2"></param>
        /// <param name="sy3"></param>
        /// <returns></returns>
        private static ArcSegment CalculateArc(double sx1, double sx2, double sx3, double sy1, double sy2, double sy3)
        {
            ArcSegment arc = new()
            {
                RotationAngle = 0,
                IsLargeArc = false,
                Point = new System.Windows.Point(sx3, sy3)
            };

            bool? bdirec = FindDirection(sx1, sx2, sx3, sy1, sy2, sy3);
            if (bdirec != null)
            {
                if ((bool)bdirec)
                    arc.SweepDirection = SweepDirection.Clockwise;
                else
                    arc.SweepDirection = SweepDirection.Counterclockwise;
            }
            else
            {
                // the three points is collinear.
                MainModel.AddInfo("圆弧半径过大");
                return new ArcSegment();
            }

            double radius = CalculateRadius(sx1, sx2, sx3, sy1, sy2, sy3);
            arc.Size = new System.Windows.Size(radius, radius);

            return arc;
        }

        /// <summary>
        /// Find the direction and check it is whether a three-point-line
        /// </summary>
        /// <param name="sx1"></param>
        /// <param name="sx2"></param>
        /// <param name="sx3"></param>
        /// <param name="sy1"></param>
        /// <param name="sy2"></param>
        /// <param name="sy3"></param>
        /// <returns></returns>
        private static bool? FindDirection(double sx1, double sx2, double sx3, double sy1, double sy2, double sy3)
        {
            double p12 = (sx2 - sx1) * (sy3 - sy2);
            double p23 = (sy2 - sy1) * (sx3 - sx2);

            // when the coefficients is zero, then the three points is collinear.
            if (p12 * p23 < 0) return true;
            else if (p12 * p23 > 0) return false;
            else return null;
        }

        /// <summary>
        /// Calcualte arc radius.
        /// </summary>
        /// <param name="sx1"></param>
        /// <param name="sx2"></param>
        /// <param name="sx3"></param>
        /// <param name="sy1"></param>
        /// <param name="sy2"></param>
        /// <param name="sy3"></param>
        /// <returns></returns>
        private static double CalculateRadius(double sx1, double sx2, double sx3, double sy1, double sy2, double sy3)
        {
            double ca = sx1 - sx2;
            double cb = sy1 - sy2;
            double cc = sx1 - sx3;
            double cd = sy1 - sy3;
            double ce = ((sx1 * sx1 - sx2 * sx2) - (sy2 * sy2 - sy1 * sy1)) / 2.0;
            double cf = ((sx1 * sx1 - sx3 * sx3) - (sy3 * sy3 - sy1 * sy1)) / 2.0;

            double lower = ca * cd - cb * cc;
            double x0 = (cd * ce - cb * cf) / lower;
            double y0 = (ca * cf - cc * ce) / lower;

            double ra = Sqrt(Pow(x0 - sx1, 2) + Pow(y0 - sy1, 2));
            return ra;
        }
    }
}
