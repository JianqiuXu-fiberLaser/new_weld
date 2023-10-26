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

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// Plot lines in the screen
    /// </summary>
    internal class ScreenPlotLine
    {
        private PathFigureCollection? _pfFrog;
        private PathFigureCollection? _pfLaser;

        internal ScreenPlotLine() { }

        internal void CreateLines(ref PathFigureCollection pf, ref PathFigureCollection pf2)
        {
            _pfFrog = pf;
            _pfLaser = pf2;
        }

        internal void PutVector(ObservableCollection<LtVector> ltv)
        {
            FilterVectorLine(ltv);
        }

        //
        // Filter the point only for lines
        //
        private void FilterVectorLine(ObservableCollection<LtVector> ltv)
        {
            int i = 0;
            int vstep;
            while (i < ltv.Count)
            {
                if (ltv[i].LineType == 0) vstep = LineSegment(i, ltv);
                else vstep = 1;
                i += vstep;
            }
        }

        //
        // A line segment between two arcs
        //
        private int LineSegment(int i, ObservableCollection<LtVector> ltv)
        {
            var ltvector = new ObservableCollection<LtVector>();

            if (i > 0) ltvector.Add(ltv[i - 1]);

            int j;
            for (j = i; j < ltv.Count; j++)
            {
                ltvector.Add(ltv[j]);
                if (ltv[j].LineType == 1) break;
            }

            PlotLines(ltvector);
            return j - i;
        }

        private void PlotLines(ObservableCollection<LtVector> ltvector)
        {
            if (_pfFrog == null || _pfLaser == null) return;

            for (int i = 0; i < ltvector.Count - 1; i++)
            {
                var f = OneLine(ltvector[i], ltvector[i + 1]);
                if (ltvector[i].LaserState == 0) _pfFrog.Add(f);
                else _pfLaser.Add(f);
            }
        }

        private static PathFigure OneLine(LtVector lt1, LtVector lt2)
        {
            var mainpf = new PathFigure()
            {
                StartPoint = new System.Windows.Point(lt1.X, lt1.Y)
            };

            LineSegment ls = new()
            {
                Point = new System.Windows.Point(lt2.X, lt2.Y)
            };

            PathSegmentCollection psc = new() { ls };
            mainpf.Segments = psc;

            return mainpf;
        }
    }
}
