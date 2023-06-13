using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.GetTrace
{
    /// <summary>
    /// Weld along the trace
    /// </summary>
    public class RunTrace
    {
        private DaemonFile? dmFile;

        public RunTrace(DaemonFile dmFile)
        {
            this.dmFile = dmFile;
        }

        //
        // NEW: new methods from HERE
        //
        private MotionOperate? _mo;

        public RunTrace() { }

        public void StartRunTrace(DaemonFile dmFile, MotionOperate mo)
        {
            this.dmFile = dmFile;
            this._mo = mo;
            _mo.ExitHandwheel();
        }

        public void Run(Tracetype tpy, List<Point> points)
        {
            if (_mo != null && _mo.IsAxesRun())
            {
                return;
            }

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

        private void RunVaneWheel(List<Point> points)
        {
            VaneWheelTrace vwt = new();

            if (_mo != null && dmFile != null)
                vwt.CopyHardware(_mo, dmFile, dmFile.laserParameter, dmFile.WeldSpeed, dmFile.LeapSpeed);

            vwt.WorkProcess(points);
        }

        //
        // END: new method end
        //

        public void WeldTrace(MoveAction ma, Tracetype tpy, List<Point> points)
        {
            switch (tpy)
            {
                case Tracetype.VANE_WHEEL:
                    VaneWheelTrace.AbortThread();
                    RunVaneWheel(ma, points);
                    break;
                case Tracetype.NONE:
                    VaneWheelTrace.AbortThread();
                    break;
                default:
                    break;
            }
        }

        private void RunVaneWheel(MoveAction ma, List<Point> points)
        {
            VaneWheelTrace vaneWheelTrace = new(ma);
            vaneWheelTrace.FormSeam(points);
            if (dmFile != null)
            {
                vaneWheelTrace.UsingDaemon(dmFile);
            }
            vaneWheelTrace.CheckPosition();

            Thread vThread = new(vaneWheelTrace.Run)
            {
                Name = nameof(vaneWheelTrace),
                IsBackground = true,
            };
            vThread.Start();
        }
    }
}
