///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Basic method to manange the PointListData
    /// </summary>
    internal class ManagePointList
    {
        public ManagePointList() { }

        /// <summary>
        /// Sorts Dictionary according its key, then converts to the List
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        internal static PointListData SortPointDic(in Dictionary<int, DisPoint> dic)
        {
            var pl = new PointListData();
            var sorted = dic.OrderBy(x => x.Key);

            foreach (var d in sorted)
            {
                pl.Add(d.Value);
            }

            return pl;
        }
    }
}
