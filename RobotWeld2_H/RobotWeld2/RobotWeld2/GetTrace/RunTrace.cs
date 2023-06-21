using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
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
        private DaemonFile? dmFile;
        private WorkPackage? workPackage;
        private MainModel? mainModel;
        private IntersectModel? intersectModel;

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

        public void StartRunTrace(WorkPackage workPackage, IntersectModel im, MainModel mm, MotionOperate mo)
        {
            this.workPackage = workPackage;
            this.intersectModel = im;
            this.mainModel = mm;
            this._mo = mo;
            _mo.ExitHandwheel();
        }

        public void Run(Tracetype tpy)
        {
            if (_mo != null && _mo.IsAxesRun())
            {
                return;
            }

            switch (tpy)
            {
                case Tracetype.VANE_WHEEL:
                    //RunVaneWheel(points);
                    break;
                case Tracetype.INTERSECT:
                    RunIntersect();
                    break;
                case Tracetype.FREE_TRACE:
                    break;
                case Tracetype.NONE:
                    MotionOperate.StopAllThread();
                    break;
                default:
                    break;
            }
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
                case Tracetype.INTERSECT:
                    //RunIntersect();
                    break;
                case Tracetype.NONE:
                    MotionOperate.StopAllThread();
                    break;
                default:
                    break;
            }
        }

        //
        // RUN: -- FROM HERE -- the detail methods
        //
        private void RunVaneWheel(List<Point> points)
        {
            VaneWheelTrace vwt = new();

            if (_mo != null && dmFile != null)
                vwt.CopyHardware(_mo, dmFile, dmFile.laserParameter, dmFile.WeldSpeed, dmFile.LeapSpeed);

            vwt.WorkProcess(points);
        }

        private void RunIntersect()
        {
            IntersectTrace intersectTrace = new();

            if (_mo != null && mainModel != null && workPackage != null && intersectModel != null)
            {
                intersectTrace.CopyHardware(_mo, mainModel, workPackage, intersectModel);
            }

            intersectTrace.WorkProcess();
        }

        //
        // END: new method end
        //
    }
}
