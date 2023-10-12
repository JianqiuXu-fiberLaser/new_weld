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

using RobotWeld2.AppModel;
using RobotWeld2.Motions;

namespace RobotWeld2.GetTrace
{
    /// <summary>
    /// Chose a point and add it to the trace
    /// </summary>
    public class TakeTrace
    {
        private DaemonModel? _dmModel;
        private MotionOperate? _mo;

        public TakeTrace() { }

        public void OpenHandwheel(MotionOperate mo, DaemonModel dmModel)
        {
            _mo = mo;
            _mo.InitialHandwheel();
            _mo.SetActionMethod(this);
            _mo.RunHandWheel();
            _dmModel = dmModel;
        }

        public void SetXYZ()
        {
            _dmModel?.AddPoint();
        }

        public void DisplayXYZ(int x, int y, int z)
        {
            _dmModel?.SetXYZ(x, y, z);
        }
    }
}
