using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Initial and maintain the motion card, including laser and HandWheel function
    /// </summary>
    public class MoveAction
    {
        private readonly MotionCard _mc;
        private readonly OperateLaser _opLas;

        public MoveAction()
        {
            _mc = new MotionCard();
            _mc.ConfigMotionSystem();
            _opLas = new OperateLaser();
            Thread.Sleep(100);
            _mc.ConfigHandWheel();
        }

        /// <summary>
        /// Initial the card, handWheel and Laser Operation condition.
        /// </summary>
        public void InitialHandWheel()
        {
            _mc.RunHand();
        }

        public void ReadCoordinate(TakeTrace tkc)
        {
            _mc.GetTakeTrace(tkc);
            Thread cThread = new(_mc.ReadCoordinate)
            {
                IsBackground = true,
            };
            cThread.Start();
        }

        /// <summary>
        /// Setup the moving parameters to the card
        /// </summary>
        public void SetupParameter(DaemonFile dmFile) 
        {
            double moveSpeed = dmFile.LeapSpeed;
            double weldSpeed = dmFile.WeldSpeed;
            _mc.SetupMoveParameter(weldSpeed, moveSpeed);
        }

        public void SetupMaxPower(int maxpower)
        {
            _opLas.MaxPower = maxpower;
        }

        /// <summary>
        /// Setup laser parameters
        /// </summary>
        public void SetupLaser(LaserParameter lp)
        {
            _opLas.GetHardware(_mc.devHandle, _mc.pcrdHandle, _mc.axisHandle);
            _opLas.SetupLaserParameter(lp);
            _opLas.SetupLaser(0);
        }

        public void SetupLaserPower(int power)
        {
            _opLas.SetupLaserPower(power);
        }

        public void LaserSwitchonShot()
        {
            _opLas.ClickLaserOn();
        }

        public void LaserSwitchoffShot()
        {
            _opLas.ClickLaserOff();
        }

        /// <summary>
        /// Welding the trace with the data of List
        /// </summary>
        /// <param name="pts"> the trace data </param>
        public void WeldInCard(List<int[]> ptlist, int bitAddr) 
        {
            _mc.WeldInCard(_opLas, ptlist, bitAddr);
        }

        /// <summary>
        /// Leap from without welding
        /// </summary>
        /// <param name="pts"></param>
        public void LeapFrog(int[] pts) 
        {
            _mc.LeapFrog(pts);
        }

        /// <summary>
        /// The thread for reset all 3 axes.
        /// </summary>
        public void ResetAll()
        {
            Thread rThread = new(_mc.ResetThread)
            {
                Name = nameof(ResetAll),
                IsBackground = true,
            };
            rThread.Start();
        }

        public void ReadIo(out int singnal)
        {
            _mc.ReadIo(out singnal);
        }

        public void ReadPosition(ref int[] pPosArray)
        {
            _mc.ReadPosition(ref pPosArray);
        }

        public void OpenAir()
        {
            _mc.AirOnOff(1);
        }

        public void CloseAir()
        {
            _mc.AirOnOff(0);
        }

        public void SendBit(int bitAddress)
        {
            _mc.SelfResetBit(bitAddress);
        }

        public static void StopHandWheel()
        {
            MotionCard.StopHandWheel = true;
        }

        public void SelectionOk()
        {
            MotionCard.SelectionOK = true;
        }

        public void GotoPosition(int[] ptxyz)
        {
            _mc.GotoPosition(ptxyz);
        }    
    }
}
