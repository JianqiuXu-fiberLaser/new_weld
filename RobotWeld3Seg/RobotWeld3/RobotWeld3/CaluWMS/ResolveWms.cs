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
using RobotWeld3.GetTrace;
using RobotWeld3.Motions;
using System.Collections.Generic;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Resolve and calculate the point list data to Weld move section.
    /// </summary>

    internal class ResolveWms
    {
        private List<WeldMoveSection>? _wmslist;
        private PointListData? _pointList;
        private List<double>? _pml;

        internal ResolveWms() { }

        internal List<WeldMoveSection> GetWmsList(in WorkPackage wk, ref List<WeldMoveSection> wmslist)
        {
            var tpy = wk.Tracetype;
            _pointList = wk.PointList;
            _pml = wk.GetParameter();

            _wmslist = wmslist;
            string sname = CTraceType.GetName(tpy);
            int icode = wk.TrCode[sname];
            var tm = wk.TimeStamp;

            switch (tpy)
            {
                case Tracetype.INTERSECT:
                    var its = new IntersectWms();
                    its.PutParameter(_pointList, _pml);
                    its.BackWms(icode, tm, ref _wmslist);
                    break;
                case Tracetype.SPIRAL:
                    var sps = new SpiralWms();
                    sps.PutParameter(_pointList, _pml);
                    sps.BackWms(icode, tm, ref _wmslist);
                    break;
                case Tracetype.STAGE_TRACE:
                    break;
                case Tracetype.FREE_TRACE:
                    var fts = new FreeWms();
                    fts.PutParameter(_pointList, _pml);
                    fts.BackWms(icode, tm, ref _wmslist);
                    break;
                default:
                    break;
            }

            return _wmslist;
        }
    }
}
