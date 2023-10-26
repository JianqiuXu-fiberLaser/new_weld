///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Manage time stamp
    /// </summary>
    public class CTimeStamp
    {
        public CTimeStamp() { }

        /// <summary>
        /// Create Time stampe from a given time.
        /// </summary>
        /// <returns> Total Seconds </returns>
        public static int CreateTimeStamp()
        {
            DateTime dateTime2020 = new(2020, 6, 15, 0, 0, 0);

            DateTime DateNow = DateTime.Now;
            TimeSpan timespan = (DateNow - dateTime2020);
            return (int)timespan.TotalSeconds;
        }
    }
}
