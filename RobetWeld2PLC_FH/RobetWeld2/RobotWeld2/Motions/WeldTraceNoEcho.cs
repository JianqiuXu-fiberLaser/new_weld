///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////


using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.Motions
{

    internal class WeldTraceNoEcho : MotionOperate
    {
        private List<int[]>? ptsList;
        private readonly LaserParameter? lp;
        private double WeldSpeed;
        private int[]? pts;
        private double moveSpeed;
        private double synAcc = 0.5;
        private double powerPercent;

        public WeldTraceNoEcho()
        {
            _mb = base._mb;
            _laser = base._laser;
            this.vm = base.vm;
            this.methodName = base.methodName;
            this.lp = base._laserParameter;
        }

        public void SetParameter(int laserPower, List<int[]> ptsList, double WeldSpeed, int[] pts, double moveSpeed,
            double synAcc = 0.5)
        {
            this.ptsList = ptsList;
            this.WeldSpeed = WeldSpeed;
            this.pts = pts;
            this.moveSpeed = moveSpeed;
            this.synAcc = synAcc;

            if (lp != null && lp.MaxPower > 0)
            {
                powerPercent = laserPower / (double)lp.MaxPower;
            }
        }

        public void WeldThread()
        {
            if (ptsList != null && ptsList.Count > 0 && pts != null)
            {
                _mutex.WaitOne(3000);

                MotionBoard.FlyMove(ptsList[0], WeldSpeed, synAcc);
                LaserDrives.LaserOn(powerPercent);
                MotionBoard.WeldMove2(ptsList, WeldSpeed, synAcc);
                LaserDrives.LaserOff();

                MotionBoard.FlyMove(pts, moveSpeed, synAcc);
                _mutex.ReleaseMutex();
                FinAction("arrive");
            }
            else
            {
                DaemonFile.AddInfo("焊接参数错误");
            }
        }
    }
}
