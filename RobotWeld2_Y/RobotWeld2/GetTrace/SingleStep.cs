using RobotWeld2.Motions;
using RobotWeld2.Welding;

namespace RobotWeld2.GetTrace
{
    public class SingleStep
    {
        private DaemonFile dmFile;
        private MotionOperate? _mo;
        private double moveSpeed;

        public SingleStep(DaemonFile dmFile)
        {
            this.dmFile = dmFile;
        }

        public void SetupParameter(MotionOperate mo, double moveSpeed)
        {
            this._mo = mo;
            this.moveSpeed = moveSpeed;
        }

        public void GotoPosition(int[] ptxyz)
        {
            _mo?.RunHandLeap(ptxyz, moveSpeed);
        }
    }
}
