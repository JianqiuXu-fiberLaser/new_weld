using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Math;

namespace RobotWeld2
{
    /// <summary>
    /// FreeCurve.xaml 的交互逻辑
    /// </summary>
    public partial class FreeCurve : UserControl
    {
        private static readonly double RealHeight = 450;
        private static readonly double RealWidth = 400;

        private readonly FreeCurveViewModel viewModel;
        private ObservableCollection<LtVector>? ltvector;

        private double _baseX;
        private double _baseY;
        private double _sizeScale;

        public FreeCurve()
        {
            viewModel = new FreeCurveViewModel();

            InitializeComponent();
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Establist the plot of List, with new vector list.
        /// </summary>
        /// <param name="workPackage"></param>
        public void EstablishList(VisualPointList vpl)
        {
            if (vpl.Cpoints == null) return;

            ltvector ??= new ObservableCollection<LtVector>();
            ltvector.Clear();

            for (int i = 0; i < vpl.Cpoints.GetCount(); i++)
            {
                Vector vc = vpl.Cpoints.GetVector(i);
                int lt = vpl.Cpoints.GetLineType(i);
                ltvector.Add(new(vc, lt));
            }

            CreatePlots();
        }

        private void CreatePlots()
        {
            PathFigureCollection pfc = new();

            int count;
            if (ltvector != null)
            {
                count = ltvector.Count;
            }
            else return;

            FindLargest(ltvector);

            // plot the curve in various corresponding color
            int i = 0;
            while (i < count)
            {
                int vstep;

                if (ltvector[i].LineType == 0)
                    vstep = LinePointGroup(i, count, pfc);
                else vstep = ArcPointGroup(i, count, pfc);

                i = vstep;
            }

            FreeTraceData.Figures = pfc;
        }

        private int LinePointGroup(int i, int ct, PathFigureCollection pfc)
        {
            if (ltvector == null) return ct;

            int ret, j;
            for (j = i; j < ct; j++)
            {
                if (ltvector[j].LineType == 1) break;
            }

            // if arrive the end of curve?
            if (j == ct - 1) ret = ct;
            else ret = j;

            // plot line from i to j
            for (int k = i; k < j - 1; k++)
            {
                PathFigure CurvePath = LinePath(ltvector[k], ltvector[k + 1]);
                pfc.Add(CurvePath);
            }

            return ret;
        }

        private int ArcPointGroup(int i, int ct, PathFigureCollection pfc)
        {
            if (ltvector == null) return ct;

            int j;
            for (j = i; j < ct; j++)
            {
                if (ltvector[j].LineType == 0)
                {
                    // we need pure arc points.
                    j--;
                    break;
                }
            }

            if (j == ct) j--;

            if ((j - i + 1) % 2 == 0)
            {
                j--;
            }

            int k = i;
            while (k <= j - 2)
            {
                double sx1 = RealWidth * (_baseX + ltvector[k].X) / _sizeScale;
                double sx2 = RealWidth * (_baseX + ltvector[k + 1].X) / _sizeScale;
                double sx3 = RealWidth * (_baseX + ltvector[k + 2].X) / _sizeScale;

                double sy1 = -RealHeight * (_baseY + ltvector[k].Y) / _sizeScale;
                double sy2 = -RealHeight * (_baseY + ltvector[k + 1].Y) / _sizeScale;
                double sy3 = -RealHeight * (_baseY + ltvector[k + 2].Y) / _sizeScale;

                PathFigure CurvePath = ArcPath(sx1, sx2, sx3, sy1, sy2, sy3);
                pfc.Add(CurvePath);
                k += 2;
            }

            ltvector[j].LineType = 0;
            return j;
        }

        [STAThread]
        private PathFigure LinePath(LtVector vc1, LtVector vc2)
        {
            PathFigure mainpf = new();

            double sx = RealWidth * (_baseX + vc1.X) / _sizeScale;
            double sy = -RealHeight * (_baseY + vc1.Y) / _sizeScale;
            mainpf.StartPoint = new System.Windows.Point(sx, sy);

            LineSegment ls = new();
            sx = RealWidth * (_baseX + vc2.X) / _sizeScale;
            sy = -RealHeight * (_baseY + vc2.Y) / _sizeScale;
            ls.Point = new System.Windows.Point(sx, sy);

            PathSegmentCollection psc = new() { ls };
            mainpf.Segments = psc;

            return mainpf;
        }

        [STAThread]
        public static PathFigure ArcPath(double sx1, double sx2, double sx3, double sy1, double sy2, double sy3)
        {
            PathFigure mainpf = new()
            {
                StartPoint = new System.Windows.Point(sx1, sy1)
            };

            ArcSegment ars = CalculateArc(sx1, sx2, sx3, sy1, sy2, sy3);
            PathSegmentCollection psc = new()
            {
                ars,
                ars
            };
            mainpf.Segments = psc;

            return mainpf;
        }

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
                return new ArcSegment();
            }

            double radius = CalculateRadius(sx1, sx2, sx3, sy1, sy2, sy3);
            arc.Size = new System.Windows.Size(radius, radius);

            return arc;
        }

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

        private static bool? FindDirection(double sx1, double sx2, double sx3, double sy1, double sy2, double sy3)
        {
            double p12 = (sx2 - sx1) * (sy3 - sy2);
            double p23 = (sy2 - sy1) * (sx3 - sx2);

            if (p12 * p23 < 0) { return true; }
            else if (p12 * p23 > 0) { return false; }
            else { return null; }
        }

        private void FindLargest(ObservableCollection<LtVector> vc)
        {
            double plgx = 1, nlgx = -1;
            double plgy = 1, nlgy = -1;

            // re-scale the x and y to fill the screen
            foreach (LtVector v in vc)
            {
                ExposeRange(v.X, ref plgx, ref nlgx);
                ExposeRange(v.Y, ref plgy, ref nlgy);
            }

            if ((plgx - nlgx) >= (plgy - nlgy))
                _sizeScale = plgx - nlgx;
            else
                _sizeScale = plgy - nlgy;

            if (plgx >= Abs(nlgx))
                _baseX = Abs(nlgx);
            else
                _baseX = plgx;

            if (plgy >= Abs(nlgy))
                _baseY = Abs(nlgy);
            else
                _baseY = plgy;
        }

        private static void ExposeRange(double inX, ref double plg, ref double nlg)
        {
            if (inX < 0)
            {
                if (inX < nlg) nlg = inX;
            }

            if (inX > 0)
            {
                if (inX > plg) plg = inX;
            }
        }
    }
}
