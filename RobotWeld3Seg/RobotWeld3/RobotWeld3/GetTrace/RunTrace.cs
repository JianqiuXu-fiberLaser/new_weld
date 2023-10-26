///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) WMS based trace runing. Independent of the trace type.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.CaluWMS;
using RobotWeld3.Motions;
using System.Collections.Generic;

namespace RobotWeld3.GetTrace
{
    /// <summary>
    /// Weld along the trace, basic class
    /// </summary>
    public class RunTrace
    {
        private List<WeldMoveSection>? _wmslist;
        private readonly MotionBus _mbus;

        public RunTrace(MotionBus mbus)
        {
            _mbus = mbus;
        }

        /// <summary>
        /// Setup the circumstance to run
        /// </summary>
        /// <param name="wms"></param>
        internal void StartRunTrace(in List<WeldMoveSection> wms)
        {
            _wmslist = wms;
            _mbus.ExitHandwheel();
        }

        public void Run()
        {
            if (_wmslist == null) return;

            var doTrace = new DoTrace(_mbus);
            var b = doTrace.CopyHardware(_wmslist);
            if (b) doTrace.WorkProcess();
        }
    }
}
