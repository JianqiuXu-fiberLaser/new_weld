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
using RobotWeld3.Motions;
using System.IO;
using System.Collections.Generic;
using RobotWeld3.Welding;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Weld the sprial trace.
    /// </summary>
    internal class SpiralWms : BasicWms
    {
        private WeldingModel? _wmodel;

        internal SpiralWms() { }

        /// <summary>
        /// Return back list of weld move section
        /// </summary>
        /// <param name="icode"></param>
        /// <param name="timeStamp"></param>
        /// <param name="wms"></param>
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
            var s1 = SpiralCurveWms(1, dpcount - 1);

            // the end wms
            Vector vc;
            if (s1 != null)
            {
                wms.Add(s1);
                vc = s1.GetLastPosition();
            }
            else
                vc = new Vector();

            var s2 = EndWms(2, vc);
            if (s2 != null) wms.Add(s2);
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
        /// <returns></returns>
        private int StartWms(int order, ref WeldMoveSection wms)
        {
            if (_ptList == null || _wmodel == null) return 0;
            var pdl = StandardWms.FindStartPoints(_ptList);
            StandardWms.StartPointWms(order, pdl, ref wms, _wmodel);
            return pdl.Count;
        }

        /// <summary>
        /// End weld move section
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private WeldMoveSection? EndWms(int order, Vector vc)
        {
            if (_ptList == null || _wmodel == null) return null;
            var pdl = StandardWms.FindEndPoints(_ptList, vc);
            return StandardWms.EndPointWms(order, pdl, _wmodel);
        }

        /// <summary>
        /// weld move section for a given spiral curve
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private WeldMoveSection? SpiralCurveWms(int order, int dpcount)
        {
            if (_ptList == null || _pml == null || _wmodel == null) return null;

            var p1 = _ptList[dpcount];
            var p2 = _ptList[dpcount + 1];
            var l = CrdInterpolation.DistanceTwoVector(p1.Vector, p2.Vector);

            double tAngle = MotionOperate.OneCycle * l / (_pml[0] * MotionOperate.Xmillimeter);

            var vclist = new List<Vector>
            {
                p1.Vector
            };

            var vc1 = new Vector(p2.Vector);
            vc1.A += tAngle;
            vclist.Add(vc1);

            // Set the laser parameter.
            var ayp = new AnalysePoint();
            ayp.GetModel(_wmodel);
            ayp.GetPoint(_ptList, dpcount, dpcount + 1);
            (var ma, var wi, var lp) = ayp.CalculateParameter();
            var w = ayp.ExtractWobble();
            var wms = new WeldMoveSection(vclist)
            {
                WmsIndex = order,
                LaserParameter = lp,
                Material = ma,
                Wire = wi,
                Speed = new List<double>()
                {
                    MotionOperate.SmoothCoefficient * p1.Speed,
                    MotionOperate.SmoothCoefficient * p1.Speed
                },
                Wobble = w,
            };

            return wms;
        }
    }
}
