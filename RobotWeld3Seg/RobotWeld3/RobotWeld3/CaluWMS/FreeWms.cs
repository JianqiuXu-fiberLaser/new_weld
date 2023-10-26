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
using RobotWeld3.Welding;
using System.IO;
using System.Collections.Generic;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// The free trace for various condition.
    /// </summary>
    internal class FreeWms : BasicWms
    {
        private WeldingModel? _wmodel;

        public override void BackWms(int icode, int timeStamp, ref List<WeldMoveSection> wms)
        {
            wms.Clear();
            string fname = "./Storage/" + icode.ToString() + ".wms";

            if (File.Exists(fname))
            {
                // if the wms has a correct time stamp, then pass the generate new wms
                if (ReadWms(timeStamp, fname, ref wms)) return;
            }

            GenerateWms(ref wms, timeStamp);
            SaveWms(timeStamp, fname, in wms);
        }

        /// <summary>
        /// Generate new weld-move-section list by the input display point list.
        /// </summary>
        /// <param name="wms"></param>
        private void GenerateWms(ref List<WeldMoveSection> wms, int timestamp)
        {
            _wmodel = new WeldingModel();
            _wmodel.ReadCoefficient(timestamp);

            // The sequence is important.
            var s0 = new WeldMoveSection();
            var dpcount = StartWms(0, ref s0);
            if (s0 != null) wms.Add(s0);

            // the middle wms list
            var s1 = FreeTraceWms(1, dpcount - 1);
            int wcount = 2;
            if (s1 != null)
            {
                wms.AddRange(s1);
                wcount = 1 + s1.Count;
            }

            // the end wms
            Vector vc;
            if (s1 != null)
            {
                int cn = s1.Count;
                vc = s1[cn - 1].GetLastPosition();
            }
            else
            {
                vc = new Vector();
            }
            var s2 = EndWms(wcount, vc);
            if (s2 != null) wms.Add(s2);

            //WriteMsgFile.WriteFile(wms);
        }

        /// <summary>
        /// Save the generated weld-move-section list to the disk
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="fname"></param>
        /// <param name="wms"></param>
        private static void SaveWms(int timeStamp, string fname, in List<WeldMoveSection> wms)
        {
            AccessWmsFile.Save(fname, timeStamp, wms);
        }

        /// <summary>
        /// if the time stamp of wms in the disk is in-correct, return false
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="fname"></param>
        /// <param name="wms"></param>
        /// <returns></returns>
        private static bool ReadWms(int timeStamp, string fname, ref List<WeldMoveSection> wms)
        {
            var ret = AccessWmsFile.Read(fname, timeStamp, ref wms);
            return ret;
        }

        //-------------------------------------------------------------
        // The concrete methods to make WMS
        //-------------------------------------------------------------

        /// <summary>
        /// Start weld move section.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="wms"></param>
        /// <returns></returns>
        private int StartWms(int order, ref WeldMoveSection wms)
        {
            if (_ptList == null || _wmodel == null) return 0;
            var pdl = StandardWms.FindStartPoints(_ptList);
            StandardWms.StartPointWms(order, pdl, ref wms, in _wmodel);
            return pdl.Count;
        }

        /// <summary>
        /// End weld move section
        /// </summary>
        /// <param name="order"></param>
        /// <param name="vc"></param>
        /// <returns></returns>
        private WeldMoveSection? EndWms(int order, Vector vc)
        {
            if (_ptList == null || _wmodel == null) return null;
            var pdl = StandardWms.FindEndPoints(_ptList, vc);
            return StandardWms.EndPointWms(order, pdl, in _wmodel);
        }

        /// <summary>
        /// weld move section for a given free trace.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="dpcount"> point number to be start the free trace. </param>
        /// <returns></returns>
        private List<WeldMoveSection>? FreeTraceWms(int order, int dpcount)
        {
            if (_ptList == null) return null;

            // eliminate the start point and repeated points.
            int iNum = _ptList.Count;
            var vptlist = _ptList.GetRange(dpcount, iNum - dpcount);
            DataAnalysis.DeleteDuplicatePoint(ref vptlist);

            var IndexPair = DataAnalysis.FilterWms(vptlist);
            var wmsList = new List<WeldMoveSection>();
            for (int i = 0; i < IndexPair.Count; i++)
            {
                int[] d = IndexPair[i];
                var wms = FreeMoveWms(in vptlist, in d, order + i);
                wmsList.Add(wms);
            }

            return wmsList;
        }

        /// <summary>
        ///  Wms btween a series points, which has the same laser parameter. 
        /// </summary>
        /// <param name="ptl"> point list that the start wms has been thrown out </param>
        /// <param name="d"> the start and end (included) index </param>
        /// <param name="order"> wms order </param>
        /// <returns> wms </returns>
        private WeldMoveSection FreeMoveWms(in PointListData ptl, in int[] d, int order)
        {
            if (_wmodel == null) return new WeldMoveSection();

            var vclist = new List<Vector>();
            var splist = new List<double>();

            int st = d[0];
            int end = d[1];
            int vstep;
            int cn = end - st + 1;
            while (st < end)
            {
                if (ptl[st].Linetype == 0 || cn < 3)
                {
                    vstep = StandardWms.FindLinePoint(in ptl, st, end);
                    List<Vector> vcIn = ptl.GetRangeVector(st, vstep);
                    StandardWms.LineSegment(ref vclist, ref splist, in vcIn);
                }
                else
                {
                    vstep = StandardWms.FindArcPoint(in ptl, st, end);
                    List<Vector> vcIn = ptl.GetRangeVector(st, vstep);
                    var isline = StandardWms.ArcSegment(ref vclist, ref splist, in vcIn);

                    // the segment is line, although the points marked as arc.
                    if (isline)
                    {
                        StandardWms.LineSegment(ref vclist, ref splist, in vcIn);
                    }
                }
                if (vstep == 0) break;
                st += vstep;
            }

            StandardWms.RealSpeed(ref splist, ptl[st].Parameter.Speed);

            // Set the laser parameter.
            var ayp = new AnalysePoint();
            ayp.GetModel(_wmodel);
            ayp.GetPoint(ptl, d[0], d[1]);
            (var ma, var wi, var lp) = ayp.CalculateParameter();
            var w = ayp.ExtractWobble();
            var wms = new WeldMoveSection(vclist)
            {
                WmsIndex = order,
                LaserParameter = lp,
                Material = ma,
                Wire = wi,
                Speed = splist,
                Wobble = w,
            };

            return wms;
        }
    }
}
