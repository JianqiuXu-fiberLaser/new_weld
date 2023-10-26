///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver 2.2: revise the point list to match single workpackage file.
// Ver. 3.0: (1) all=in-one data structure.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RobotWeld3.Welding
{
    /// <summary>
    /// Basic function for data analysis
    /// </summary>
    internal class DataAnalysis
    {
        internal DataAnalysis() { }

        /// <summary>
        /// Filter the laser power, frequency and duty in the DisPoint list.
        /// Then, seperate the DisPoint in the corresponding List of int pair.
        /// </summary>
        /// <param name="pts"></param>
        /// <returns> The start and end (included) indices of points of each wms </returns>
        internal static List<int[]> FilterWms(in PointListData pts)
        {
            var dict = new List<int[]>();

            LaserDisplayParameter lp = pts[0].Parameter;
            int istart = 0;
            int iend;

            for (int i = 1; i < pts.Count; i++)
            {
                LaserDisplayParameter lpt = pts[i].Parameter;
                if (!lp.EqualsTo(lpt))
                {
                    iend = i - 1;
                    dict.Add(new int[2] { istart, iend });

                    // new index
                    istart = i;
                    lp = lpt;
                }
            }

            // there are only one segment or arrive the end.
            if (istart == 0 || istart == pts.Count)
                dict.Add(new int[2] { istart, pts.Count - 1 });
            return dict;
        }

        /// <summary>
        /// Filter the duplicate points in the PointList
        /// </summary>
        /// <param name="pts"></param>
        internal static void DeleteDuplicatePoint(ref PointListData pts)
        {
            var ilt = new List<int>();
            var pt1 = pts[0];
            int i = 1;
            while (i < pts.Count)
            {
                var ptt = new DisPoint(pts[i]);
                if (ptt.Vector.Equals(pt1.Vector)) ilt.Add(i);

                pt1 = ptt;
                i++;
            }

            if (ilt.Count > 0)
            {
                for (int j = ilt.Count - 1; j >= 0; j--)
                    pts.Remove(ilt[j]);
            }
        }
    }
}
