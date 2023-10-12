///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using System;
using System.Collections.Generic;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Maintain points
    /// </summary>
    public class MaintainPoints
    {
        public MaintainPoints() { }

        /// <summary>
        /// Get Trace index from Workpackage file
        /// </summary>
        /// <returns></returns>
        internal static int GetTraceIndex()
        {
            var baseTreatFile = new BaseTreatFile();
            return baseTreatFile.Open();
        }

        /// <summary>
        /// create a new engineering project
        /// </summary>
        /// <returns> trace index </returns>
        internal static int NewFile()
        {
            var dataTime2041 = new DateTime(2020, 4, 1, 0, 0, 0);
            var DateNow = DateTime.Now;
            TimeSpan timespan = (DateNow - dataTime2041);
            return (int)timespan.TotalSeconds;
        }

        //
        // The dictionary has exist yet
        //
        public static void RevisePoints(int td, in List<Point> cp, ref Dictionary<int, List<Point>> pd)
        { 
            if (Math.Abs(td) > DaemonModel.TraceCount) return;

/*            if (td > 0) pd[0][1] = cp[0];
            else pd[0][2] = cp[0];*/

            for (int i = 0; i < cp.Count; i++) pd[td][i] = cp[i];
        }

        /// <summary>
        /// Establish the new point's dictionary before press the confirm button 
        ///     in the page of PointList.
        /// </summary>
        /// <param name="cn"> the count </param>
        /// <param name="pd"> the dictionary </param>
        internal static void NewPoints(int cn, ref Dictionary<int, List<Point>> pd)
        {
            if (pd == null) pd = new Dictionary<int, List<Point>>();
            else pd.Clear();

            var ol = new List<Point>();
            for (int j = 0; j < 3; j++)
                ol.Add(new Point());

            pd.Add(0, ol);

            for (int i = 1; i < cn; i++)
            {
                var pl = new List<Point>();
                for (int k=0; k<DaemonModel.TraceCount; k++)
                    pl.Add(new Point());

                pd.Add(i, pl);
            }

            for (int i = 1; i < cn; i++)
            {
                var nl = new List<Point>();
                for (int k = 0; k < DaemonModel.TraceCount; k++)
                    nl.Add(new Point());

                pd.Add(-i, nl);
            }
        }

        /// <summary>
        /// When the traceCount do not equal the number of the exist point's 
        ///     dictionary, modifying it.
        /// </summary>
        /// <param name="cn"> the new count </param>
        /// <param name="pd"> the dictionary </param>
        internal static void Modification(int cn, ref Dictionary<int, List<Point>> pd)
        {
            int t = cn - (pd.Count - 1) / 2;
            if (cn > pd.Count)
                AddList(t, ref pd);
            else
                RemoveList(t, ref pd);
        }

        private static void AddList(int t, ref Dictionary<int, List<Point>> pd)
        {
            int d = (pd.Count - 1) / 2;
            var plast = pd[d];
            var nlast = pd[pd.Count - 1];
            for (int i = 0; i < t; i++)
            {
                var pt = new List<Point>();
                CopyFrom(in plast, pt);
                pd.Add(d + i, pt);

                var nt = new List<Point>();
                CopyFrom(in nlast, pt);
                pd.Add(-d - i, nt);
            }
        }

        private static void RemoveList(int t, ref Dictionary<int, List<Point>> pd)
        {
            int d = (pd.Count - 1) / 2;
            for (int i = 0; i < t; ++i)
            {
                pd.Remove(d - i);
                pd.Remove(-d + i);
            }
        }

        private static void CopyFrom(in List<Point> pin, List<Point> pout)
        {
            int c = pin.Count;
            for (int i = 0; i < c; i++)
            {
                var p = new Point(pin[i]);
                pout.Add(p);
            }
        }
    }
}
