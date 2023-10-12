///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.Motions
{
    class FlyWeldTrace : MotionOperate
    {
        private List<int[]>? ptsList;
        private readonly LaserParameter? lp;
        private double WeldSpeed;
        private double moveSpeed;
        private Dictionary<int, int>? tpdic;
        private double synAcc = 0.5;

        public FlyWeldTrace(MotionOperate mo)
        {
            _mb = mo._mb;
            _laser = mo._laser;
            this.vm = mo.vm;
            this.methodName = mo.methodName;
            this.lp = mo._laserParameter;
        }

        public void SetParameter(List<int[]> ptsList, double WeldSpeed,
            double moveSpeed, Dictionary<int, int> tpdic, double synAcc = 0.5)
        {
            this.ptsList = ptsList;
            this.WeldSpeed = WeldSpeed;
            this.moveSpeed = moveSpeed;
            this.tpdic = tpdic;
            this.synAcc = synAcc;
        }

        public void FlyWeldThread()
        {
            if (ptsList == null || ptsList.Count == 0 || tpdic == null)
            {
                DaemonFile.AddInfo("焊接参数错误");
                return;
            }

            // long moving to the prepared point, and a protected moving.
            MotionBoard.FlyMove(ptsList[0], 1.5 * moveSpeed, synAcc);

            int sp = 1;
            for (int i = 1; i <= DaemonModel.TraceCount; i++)
            {
                var ptarc = new List<int[]>();
                ptarc.AddRange(ptsList.GetRange(sp + 1, tpdic[i] - 1));

                _mutex.WaitOne(3000);
                MotionBoard.FlyMove(ptsList[sp], 0.5 * moveSpeed, synAcc);
                MotionBoard.WeldMove2(ptarc, WeldSpeed, synAcc);
                _mutex.ReleaseMutex();
                sp += tpdic[i];
            }
            // move to the prepared point.
            MotionBoard.FlyMove(ptsList[0], moveSpeed, synAcc);
            FinAction("flyweld");
        }
    }
}
