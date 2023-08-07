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

        public PointListData(PointListData PointList)
        {
            points = new List<Point>();
            for (int i = 0; i < PointList.GetCount(); i++)
            {
                var v = PointList.GetVector(i);
                var a = PointList.GetAngle(i);
                var lt = PointList.GetLineType(i);
                var pa = PointList.GetLaserWeldParameter(i);
                Point p = new(lt, v, a) { Parameter = pa };
                points.Add(p);
            }
        }

        public PointListData(ObservableCollection<Point> ptlist)
        {
            points = new List<Point>(ptlist);
        }

        /// <summary>
        /// Add a Point to the PointListData
        /// </summary>
        /// <param name="pt"></param>
        public void Add(Point pt)
        {
            points.Add(pt);
        }

        public void Insert(int index, Point pt)
        {
            points.Insert(index, pt);
        }

        public void Remove(int index)
        {
            points.RemoveAt(index);
        }

        public void Clear()
        {
            points.Clear();
        }

        /// <summary>
        /// The Count of PointListData
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// The last number (count - 1) of PointListData
        /// </summary>
        /// <returns></returns>
        public int LastPoint()
        {
            return (points.Count - 1);
        }

        public Vector GetVector(int index)
        {
            return points[index].vector;
        }

        /// <summary>
        /// Get A-axis pulse number
        /// </summary>
        /// <param name="index"></param>
        /// <returns> pulse number </returns>
        public int GetAngle(int index)
        {
            return (int)points[index].A;
        }

        public LaserWeldParameter GetLaserWeldParameter(int index)
        {
            return points[index].Parameter;
        }

        /// <summary>
        /// Modify the laser Weld parameter throghout.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="index"></param>
        public void ModifyLaserWeldParameter(LaserWeldParameter param, int index)
        {
            int paraIndex = points[index].Parameter.ParaIndex;

            for (int i = index; i < points.Count; i++)
            {
                if (points[i].Parameter.ParaIndex == paraIndex)
                {
                    points[i].Parameter = param;
                }
            }
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

        public int GetLineType(int index)
        {
            return points[index].LineType;
        }

        /// <summary>
        /// Set the number for each point by it's sit sequency.
        /// </summary>
        public void SetPointNum()
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].PointNum = i;
            }
        }

        /// <summary>
        /// Get the first laser weld parameter, which laser power is not zero
        /// </summary>
        /// <returns></returns>
        public LaserWeldParameter GetNoZeroParameter()
        {
            foreach (Point p in points)
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
