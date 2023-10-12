///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AppModel;
using System.Threading;
using System.Collections.Generic;
using System.Security.AccessControl;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Welding;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Thread and methods to operate the laser
    /// </summary>
    internal class LaserOperate : MotionOperate
    {
        private List<int>? _segLasOff;
        private bool _laserOnState;
        private bool _laserOffState;
        private int maxPower;
        private readonly int[] _riseMillis;
        private readonly int[] _fallMillis;

        public LaserOperate(MotionOperate mo)
        {
            _mb = mo._mb;
            _laser = mo._laser;
            _laserParameter = mo._laserParameter;
            _riseMillis = new int[4];
            _fallMillis = new int[4];
        }

        public void SetParameter(int laserPower, List<int> slo)
        {
            _segLasOff = slo;
            if (DaemonFile.MaxPower > 0)
            {
                maxPower = DaemonFile.MaxPower;
                CalSubPower(laserPower);
            }
        }

        public void LaserThread()
        {
            if (_segLasOff == null) return;

            _laserOnState = false;
            _laserOffState = false;
            int iNum = 0;
            while (true)
            {
                int segNo = MotionBoard.GetSegNo();

                if (!_laserOnState && segNo > 1)
                {
                    SlowLaserOn();
                    _laserOnState = true;
                }

                if (segNo < 1)
                {
                    _laserOnState = false;
                    _laserOffState = false;
                }

                if (!_laserOffState && segNo > _segLasOff[iNum])
                {
                    SlowLaserOff();
                    _laserOffState = true;
                    Thread.Sleep((int)DaemonModel.Fall);

                    iNum++;
                    if (iNum >= _segLasOff.Count)
                    {
                        LaserDrives.LaserOff();
                        break;
                    }
                }

                if (_lthreadFlag)
                {
                    LaserDrives.LaserOff();
                    return;
                }

                Thread.Sleep(20);
            }
        }

        private void CalSubPower(int lpow)
        {
            for (int i = 0; i < 5; i++)
            {
                SubPowerPercent[i] = lpow * 0.2 * (i + 1) / maxPower;
            }
            for (int j = 0; j < 4; j++)

            {
                _riseMillis[j] = (int)(DaemonModel.Rise * 0.25);
                _fallMillis[j] = (int)(DaemonModel.Fall * 0.25);
            }
        }

        private void SlowLaserOn()
        {
            for (int i = 0; i < 4; i++)
            {
                LaserDrives.LaserOn(SubPowerPercent[i]);
                Thread.Sleep(_riseMillis[i]);
            }

            LaserDrives.LaserOn(SubPowerPercent[4]);
        }

        private void SlowLaserOff()
        {
            for (int i = 0; i < 4; i++)
            {
                LaserDrives.LaserOn(SubPowerPercent[3 - i]);
                Thread.Sleep(_fallMillis[i]);
            }

            LaserDrives.LaserOff();
        }
    }
}
