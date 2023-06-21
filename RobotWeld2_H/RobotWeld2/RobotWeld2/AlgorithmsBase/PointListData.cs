using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Treat the data for point's list
    /// </summary>
    public class PointListData
    {
        private List<Point> points;

        public PointListData()
        {
            points = new List<Point>();
        }

        public PointListData(ObservableCollection<Point> ptlist)
        {
            points = new List<Point>(ptlist);
        }

        public void Add(Point pt)
        {
            points.Add(pt);
        }

        public void Clear()
        {
            points.Clear();
        }

        public int GetCount()
        {
            return points.Count;
        }

        public void ChangePoint(int index, Point pt)
        {
            points[index] = pt;
        }

        public Point GetPoint(int index)
        {
            return points[index];
        }

        public void CopyList(PointListData? pts)
        {
            if (pts != null)
            {
                for (short i = 0; i < pts.GetCount(); i++)
                {
                    points.Add(pts.points[i]);
                }
            }
        }

        /// <summary>
        /// Get the first laser weld parameter, which laser power is not zero
        /// </summary>
        /// <returns></returns>
        public LaserWeldParameter GetNoZeroParameter()
        {
            foreach(Point p in points)
            {
                if (p.Parameter.LaserPower > 0)
                {
                    return p.Parameter;
                }
            }

            return new LaserWeldParameter();
        }

        public int[] GetXYZ(int index)
        {
            int[] coords = new int[3];
            coords[0] = (int)points[index].vector.X;
            coords[1] = (int)points[index].vector.Y;
            coords[2] = (int)points[index].vector.Z;

            return coords;
        }

        public ObservableCollection<Point> DisplayPoint()
        {
            ObservableCollection<Point> opt = new(points);
            return opt;
        }
    }
}
