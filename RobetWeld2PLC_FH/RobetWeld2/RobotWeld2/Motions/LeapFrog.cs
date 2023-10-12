///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////


using RobotWeld2.Welding;

namespace RobotWeld2.Motions
{
    class LeapFrog : MotionOperate
    {
        int[]? pts;
        double moveSpeed;
        double synAcc;

        public LeapFrog()
        {
            this._mb = base._mb;
            this.vm = base.vm;
            this.methodName = base.methodName;
        }

        public void SetParameter(int[] pts, double moveSpeed, double synAcc = 0.5)
        {
            this.pts = pts;
            this.moveSpeed = moveSpeed;
            this.synAcc = synAcc;
        }

        public void LeapThread()
        {
            if (pts != null)
            {
                _mutex.WaitOne(3000);
                MotionBoard.FlyMove(pts, moveSpeed, synAcc);
                _mutex.ReleaseMutex();
            }
            else
            {
                DaemonFile.AddInfo("运动参数错误");
            }
        }
    }
}
