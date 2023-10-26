///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver.2.2: new added class to display laser parameters.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using static System.Math;

namespace RobotWeld3.AppModel
{
    public class FreeCurveModel
    {
        private static readonly double RealHeight = 450;
        private static readonly double RealWidth = 400;

        private ObservableCollection<LtVector>? _ltvector;

        private double _baseX;
        private double _baseY;
        private double _sizeScale;

        public FreeCurveModel() { }

        /// <summary>
        /// Put the display point into plane Vector with line type.
        /// </summary>
        /// <param name="ptl"> display point list </param>
        internal void PutVisualPoint(List<DisPoint> ptl)
        {
            _ltvector ??= new ObservableCollection<LtVector>();
            _ltvector.Clear();

            for (int i = 0; i < ptl.Count; i++)
            {
                var vc = ptl[i].Vector;
                var lt = ptl[i].Linetype;

                // elimated the repeated point.
                // use the new coordinate value.
                if (i > 0)
                {
                    var vc0 = ptl[i - 1].Vector;
                    if (vc.Equals(vc0))
                    {
                        var cn = _ltvector.Count;
                        _ltvector[cn - 1].LineType = ptl[i].Linetype;
                        if (ptl[i].Power == 0) _ltvector[cn - 1].LaserState = 0;
                        else _ltvector[cn - 1].LaserState = 1;
                        continue; 
                    }
                }

                // laser on / off
                int ls;
                if (ptl[i].Power == 0) ls = 0;
                else ls = 1;

                _ltvector.Add(new LtVector(vc, lt, ls));
            }
        }

        /// <summary>
        /// Create plane plot
        /// </summary>
        /// <returns></returns>
        public List<PathFigureCollection> CreatePlot()
        {
            var pfcList = new List<PathFigureCollection>();
            var pfcFrog = new PathFigureCollection();
            var pfcLaser = new PathFigureCollection();

            if (_ltvector == null || _ltvector.Count == 0) return pfcList;

            FindLargest();
            ScaleVector();

            var spline = new ScreenPlotLine();
            spline.CreateLines(ref pfcFrog, ref pfcLaser);
            spline.PutVector(_ltvector);

            var sparc = new ScreenPlotArc();
            sparc.CreateArcs(ref pfcFrog, ref pfcLaser);
            sparc.PutVector(_ltvector);

            // the back line
            int count = _ltvector.Count;
            PathFigure pfclast = LinePath(_ltvector[count - 1], _ltvector[0]);
            pfcFrog.Add(pfclast);

            // two parts: one for laser and anther for frog-jump.
            pfcList.Add(pfcFrog);
            pfcList.Add(pfcLaser);
            return pfcList;
        }

        //
        // Scale the vector to the match size
        //
        private void ScaleVector()
        {
            if (_ltvector == null) return;
            for (int i = 0; i < _ltvector.Count; i++)
            {
                _ltvector[i].X = RealWidth * (_baseX + _ltvector[i].X) / _sizeScale;
                _ltvector[i].Y = -RealHeight * (_baseY + _ltvector[i].Y) / _sizeScale;
            }
        }

        //
        // path for straight line
        //
        private static PathFigure LinePath(LtVector vc1, LtVector vc2)
        {
            PathFigure mainpf = new();
            mainpf.StartPoint = new System.Windows.Point(vc1.X, vc1.Y);
            LineSegment ls = new();
            ls.Point = new System.Windows.Point(vc2.X, vc2.Y);

            PathSegmentCollection psc = new() { ls };
            mainpf.Segments = psc;

            return mainpf;
        }

        //
        // Find the largest coordinate of points no matter whos in x or y axis.
        // Also, find the matched larger direction.
        // In this find method, we do not used the direction of machine hardware.
        //
        private void FindLargest()
        {
            if (_ltvector == null) return;
            var vc = _ltvector;

            // positive and negative value of coordinate.
            double plgx = 1, nlgx = -1;
            double plgy = 1, nlgy = -1;

            // re-scale the x and y to fill the screen
            foreach (LtVector v in vc)
            {
                ExposeRange(v.X, ref plgx, ref nlgx);
                ExposeRange(v.Y, ref plgy, ref nlgy);
            }

            // find the range of plot to match the screen.
            if ((plgx - nlgx) >= (plgy - nlgy))
                _sizeScale = plgx - nlgx;
            else
                _sizeScale = plgy - nlgy;

            // move axis to the bottom of screen.
            if (plgx >= Abs(nlgx))
                _baseX = Abs(nlgx);
            else
                _baseX = plgx;

            if (plgy >= Abs(nlgy))
                _baseY = Abs(nlgy);
            else
                _baseY = plgy;
        }

        //
        // match the figure size to the given plot.
        //
        private static void ExposeRange(double inX, ref double plg, ref double nlg)
        {
            if (inX < 0 && inX < nlg) nlg = inX;
            if (inX > 0 && inX > plg) plg = inX;
        }
    }
}
