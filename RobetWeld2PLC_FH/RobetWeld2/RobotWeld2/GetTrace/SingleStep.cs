///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.Welding;

namespace RobotWeld2.GetTrace
{
    public class SingleStep
    {
        private MotionOperate? _mo;
        private double moveSpeed;

        public SingleStep() {}

        public void SetupParameter(MotionOperate mo, double moveSpeed)
        {
            this._mo = mo;

            if (moveSpeed == 0) this.moveSpeed = 80;
            else this.moveSpeed = moveSpeed;
        }


        /// <summary>
        /// Goto the Position indicated by the points
        /// Add the check of IO port of 7
        /// </summary>
        /// <param name="ptxyz"> the corrdinate of XYZ </param>
        public void GotoPosition(int[] ptxyz)
        {
            if ( _mo == null || !_mo.ReadIoBit(7)) return;

            _mo?.RunHandLeap(ptxyz, moveSpeed);
        }
    }
}
