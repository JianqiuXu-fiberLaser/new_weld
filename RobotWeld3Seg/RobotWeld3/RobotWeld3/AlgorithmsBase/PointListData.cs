///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver 2.2: revise the point list to match single workpackage file.
//          (1) using laserDisplayParameter for display on UI only.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Treat the data for point's list for display.
    /// </summary>
    internal class PointListData
    {
        private readonly List<DisPoint> _ptlist;

        public PointListData()
        {
            _ptlist = new List<DisPoint>();
        }

        public PointListData(ObservableCollection<DisPoint> ptlist)
        {
            _ptlist = new List<DisPoint>(ptlist);
        }

        public PointListData(List<DisPoint> ptlist)
        {
            _ptlist = ptlist;
        }

        public int Count
        {
            get => _ptlist.Count;
        }

        public List<DisPoint> Ptlist
        {
            get => _ptlist;
        }

        /// <summary>
        /// the Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DisPoint this[int index]
        {
            get { return _ptlist[index]; }
            set { _ptlist[index] = value; }
        }

        /// <summary>
        /// Add a Point to the PointListData
        /// </summary>
        /// <param name="pt"></param>
        public void Add(DisPoint pt)
        {
            _ptlist.Add(pt);
        }

        public void Insert(int index, DisPoint pt)
        {
            _ptlist.Insert(index, pt);
        }

        public void Remove(int index)
        {
            _ptlist.RemoveAt(index);
        }

        public void Clear()
        {
            _ptlist.Clear();
        }

        public PointListData GetRange(int index, int count)
        {
            var ptl = _ptlist.GetRange(index, count);
            return new PointListData(ptl);
        }

        public void CopyList(PointListData? pts)
        {
            if (pts == null) return;

            _ptlist.Clear();
            for (int i = 0; i < pts.Count; i++)
            {
                _ptlist.Add(pts[i]);
            }
        }

        /// <summary>
        /// Extract moving vectors from the start to the end. 
        ///   (includes the start and the end)
        /// </summary>
        /// <param name="start"> start index </param>
        /// <param name="end"> end index </param>
        /// <returns> vector list </returns>
        public List<Vector> GetRangeVector(int start, int end)
        {
            var vc = new List<Vector>();
            if (end > _ptlist.Count - 1) return vc;

            for (int i = start; i <= end; i++)
            {
                vc.Add(_ptlist[i].Vector);
            }

            return vc;
        }
    }
}
