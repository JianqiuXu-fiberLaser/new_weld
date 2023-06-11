using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.GetTrace
{
    /// <summary>
    /// Chose a point and add it to the trace
    /// </summary>
    public class TakeTrace
    {
        private DaemonFile? dmFile;
        private DaemonModel? dmModel;
        private List<Point>? chpoints;

        private MotionOperate? _mo;

        public TakeTrace(DaemonFile dmFile)
        {
            this.dmFile = dmFile;
        }

        //
        // NEW: -- methods
        //
        public TakeTrace() { }

        public void OpenHandwheel(MotionOperate mo, DaemonFile dmFile, DaemonModel dmModel)
        {
            this._mo = mo;
            this.dmFile = dmFile;
            this.dmModel = dmModel;
            _mo.InitialHandwheel();
            _mo.SetActionMethod(this);
            _mo.RunHandWheel();
        }

        public void ChangeLinetype()
        {
            dmFile?.ChangeLinetype();
        }

        public void ChangeLaseronoff()
        {
            dmFile?.ChangeLaseronoff();
        }

        public void SetXYZ(int x, int y, int z)
        {
            dmFile?.SetXYZ(x, y, z);
            dmModel?.AddPoint(x, y, z);
        }

        public void DisplayXYZ(int x, int y, int z)
        {
            dmFile?.SetXYZ(x, y, z);
        }

        public void TakePoint(List<Point> chpoints)
        {
            // do nothing this time
        }

        //
        // END: -- new methods end
        //

        /// <summary>
        /// The task to take and the point
        /// </summary>
        public void TakePoint(DaemonModel dmModel, MoveAction ma, List<Point> chpoints)
        {
            ma.ReadCoordinate(this);
            this.dmModel = dmModel;
            this.chpoints = chpoints;
        }

        public void AddPoint(Vector vc)
        {
            if (dmModel == null) { return; }

            DisplyListDelegate dpl = new(dmModel.DisplayList);
            int lt;
            if (dmFile != null && dmFile.LineType) { lt = 0; }
            else { lt = 1; }

            int ls;
            if (dmFile != null && dmFile.LaserOnOff) { ls = 0; }
            else { ls = 1; }

            if (dmFile is not null)
            {
                Point onept = new(lt, ls, dmFile.LaserPower, vc);
                chpoints?.Add(onept);
                dpl(onept);
            }
        }
    }

    //
    // Delegate function to display list in the main window
    //
    public delegate void DisplyListDelegate(Point dispt);
}
