///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AppModel;
using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.Motions
{
    class WeldTrace : MotionOperate
    {
        private List<int[]>? ptsList;
        private double WeldSpeed;
        private double moveSpeed;
        private double synAcc = 0.5;
        private int maxPower;
        private double powerPercent;

        public WeldTrace(MotionOperate mo)
        {
            _mb = mo._mb;
            _laser = mo._laser;
            vm = mo.vm;
            methodName = mo.methodName;
            _laserParameter = mo._laserParameter;
        }

        public void SetParameter(int laserPower, List<int[]> ptsList, double WeldSpeed,
            double moveSpeed, double synAcc = 0.5)
        {
            this.ptsList = ptsList;
            this.WeldSpeed = WeldSpeed;
            this.moveSpeed = moveSpeed;
            this.synAcc = synAcc;

            if (_laserParameter != null && _laserParameter.MaxPower > 0)
            {
                maxPower = _laserParameter.MaxPower;
                powerPercent = (double)laserPower / (double)maxPower;
            }
        }

        public void WeldThread()
        {
            if (ptsList == null || ptsList.Count == 0)
            {
                DaemonFile.AddInfo("焊接参数错误");
                return;
            }

            // move to the prepared point, coding for protection
            MotionBoard.FlyMove(ptsList[0], moveSpeed, synAcc);

            for (int i = 0; i < DaemonModel.TraceCount; i++)
            {
                int sp = i * 7 + 1;
                var ptarc = new List<int[]>();
                ptarc.AddRange(ptsList.GetRange(sp + 1, DaemonModel.TracePoint - 1));

                _mutex.WaitOne(3000);
                MotionBoard.FlyMove(ptsList[sp], moveSpeed, synAcc);
                MotionBoard.WeldMove2(ptarc, WeldSpeed, synAcc);
                _mutex.ReleaseMutex();

                LaserDrives.LaserOff();
            }

            // move to the prepared point
            MotionBoard.FlyMove(ptsList[0], moveSpeed, synAcc);
            FinAction("weld");
        }
    }
}
