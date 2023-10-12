///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.GetTrace
{
    /// <summary>
    /// Weld along the trace
    /// </summary>
    public class RunTrace
    {
        private DaemonFile? _dmFile;
        private MotionOperate? _mo;

        public RunTrace() { }

        public void StartRunTrace(DaemonFile dmFile, MotionOperate mo)
        {
            _dmFile = dmFile;
            _mo = mo;
            _mo.ExitHandwheel();
        }

        public void Run(Tracetype tpy, Dictionary<int, List<Point>> points)
        {
            if (_mo != null && _mo.IsAxesRun()) return;

            switch (tpy)
            {
                case Tracetype.VANE_WHEEL:
                    RunVaneWheel(points);
                    break;
                case Tracetype.NONE:
                    MotionOperate.StopAllThread();
                    break;
                default:
                    break;
            }
        }

        private void RunVaneWheel(Dictionary<int, List<Point>> points)
        {
            VaneWheelTrace vwt = new();

            if (_mo != null && _dmFile != null)
            {
                vwt.CopyHardware(_mo, _dmFile);
                vwt.WorkProcess(in points);
            }
        }
    }
}
