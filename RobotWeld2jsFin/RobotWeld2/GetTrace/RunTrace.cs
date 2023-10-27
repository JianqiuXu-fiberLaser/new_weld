using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using RobotWeld2.Welding;

namespace RobotWeld2.GetTrace
{
    /// <summary>
    /// Weld along the trace
    /// </summary>
    public class RunTrace
    {
        private WorkPackage? workPackage;
        private IntersectModel? intersectModel;
        private SpiralCurveModel? spiralCurveModel;
        private StageCurveModel? stageCurveModel;
        private FreeCurveModel? freeCurveModel;

        //
        // NEW: new methods from HERE
        //
        private MotionOperate _mo;

        public RunTrace(MotionOperate mo)
        {
            this._mo = mo;
        }

        public void StartRunTrace(WorkPackage workPackage, IntersectModel im)
        {
            this.workPackage = workPackage;
            this.intersectModel = im;
            _mo.ExitHandwheel();
        }

        public void StartRunTrace(WorkPackage workPackage, SpiralCurveModel scm)
        {
            this.workPackage = workPackage;
            this.spiralCurveModel = scm;
            _mo.ExitHandwheel();
        }

        public void StartRunTrace(WorkPackage workPackage, StageCurveModel stm)
        {
            this.workPackage = workPackage;
            this.stageCurveModel = stm;
            _mo.ExitHandwheel();
        }

        public void StartRunTrace(WorkPackage workPackage, FreeCurveModel fcm)
        {
            this.workPackage = workPackage;
            this.freeCurveModel = fcm;
            _mo.ExitHandwheel();
        }

        public void Run(Tracetype tpy)
        {
            switch (tpy)
            {
                case Tracetype.VANE_WHEEL:
                    break;
                case Tracetype.INTERSECT:
                    RunIntersect();
                    break;
                case Tracetype.SPIRAL:
                    RunSpiral();
                    break;
                case Tracetype.STAGE_TRACE:
                    RunStage();
                    break;
                case Tracetype.FREE_TRACE:
                    RunFree();
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
        private void RunIntersect()
        {
            if (workPackage != null && intersectModel != null)
            {
                IntersectTrace intersectTrace = new(_mo);
                intersectTrace.CopyHardware(workPackage, intersectModel);
                intersectTrace.WorkProcess();
            }
        }

        private void RunSpiral()
        {

            if (workPackage != null && spiralCurveModel != null)
            {
                SpiralOnTrace spiralOnTrace = new(_mo);
                spiralOnTrace.CopyHardware(workPackage, spiralCurveModel);
                spiralOnTrace.WorkProcess();
            }

        }

        private void RunStage()
        {
            if (workPackage != null && stageCurveModel != null)
            {
                StageOnTrace stageOnTrace = new(_mo);
                stageOnTrace.CopyHardware(workPackage, stageCurveModel);
                stageOnTrace.WorkProcess();
            }

        }

        private void RunFree()
        {
            if (workPackage != null && freeCurveModel != null)
            {
                FreeOnTrace freeOnTrace = new(_mo);
                freeOnTrace.CopyHardware(workPackage, freeCurveModel);
                freeOnTrace.WorkProcess();
            }
        }

        //
        // END: new method end
        //
    }
}
