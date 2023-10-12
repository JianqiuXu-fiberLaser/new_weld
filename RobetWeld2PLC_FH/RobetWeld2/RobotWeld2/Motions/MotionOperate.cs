///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) weld 7 traces in one command.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// CLASS: operation through the motion card, i.e., moving, laser and handwheel
    /// Manage threads about motion operation.
    /// </summary>
    public class MotionOperate
    {
        private readonly object _lock = new();
        protected static Mutex _mutex = new(false);

        internal MotionBoard _mb;
        internal readonly HandWheel _hand;
        internal LaserDrives _laser;

        //
        // the varible used to operate the motion card
        //
        internal LaserParameter? _laserParameter;
        protected readonly double[] SubPowerPercent;

        private static Thread? _wthread;
        private static Thread? _lthread;
        internal static bool _lthreadFlag;
        internal static bool _threadFlag;
        internal static bool _iothreadFlag;

        internal string methodName = string.Empty;

        public MotionOperate()
        {
            _mb = new MotionBoard();
            _hand = new HandWheel();
            _laser = new LaserDrives();
            SubPowerPercent = new double[5];
        }

        public void InitialCard()
        {
            MotionBoard.InitialCard();
        }

        public void InitialHandwheel()
        {
            HandWheel.InitialHandwheel();
        }

        public void InitialLaser()
        {
            LaserDrives.InitialLaser(0);
        }

        public void ExitHandwheel()
        {
            HandWheel.Exit();
        }

        public void SetupLaserParameter(in int power)
        {
            if (DaemonFile.MaxPower == 0)
            {
                GiveMsg.Show("最大激光功率不能为零");
            }
            else
            {
                _laserParameter ??= new LaserParameter();
                _laserParameter.MaxPower = DaemonFile.MaxPower;
                _laserParameter.LaserPower = power;

                PowerSubdivision();
            }
        }

        //
        // Caculate the subdivision for the laser power
        //
        private void PowerSubdivision()
        {
            if (_laserParameter == null) return;

            int _maxPower = DaemonFile.MaxPower;
            if (_maxPower == 0) { GiveMsg.Show("最大功率为零"); return; }

            int LaserPower = _laserParameter.LaserPower;
            SubPowerPercent[0] = 0.2 * LaserPower / _maxPower;
            SubPowerPercent[1] = 0.4 * LaserPower / _maxPower;
            SubPowerPercent[2] = 0.6 * LaserPower / _maxPower;
            SubPowerPercent[3] = 0.8 * LaserPower / _maxPower;
            SubPowerPercent[4] = 1.0 * LaserPower / _maxPower;
        }

        internal void ReadPosition(ref int[] p)
        {
            MotionBoard.ReadCoordinate(out p);
        }

        public void LaserOnNoRise()
        {
            if (_laserParameter != null)
            {
                LaserDrives.LaserOn(SubPowerPercent[4]);
            }
        }

        public void LaserOffNoFall()
        {
            LaserDrives.LaserOff();
        }

        public void EchoBit(int bitAddr)
        {
            MotionBoard.SelfResetBit(bitAddr);
        }

        public void OpenAir()
        {
            MotionBoard.SetBit(8, false);
        }

        public void CloseAir()
        {
            MotionBoard.SetBit(8, true);
        }

        public void TurnOnBit(int bitAddr)
        {
            MotionBoard.SetBit(bitAddr, true);
        }

        public void TurnOffBit(int bitAddr)
        {
            MotionBoard.SetBit(bitAddr, false);
        }

        /// <summary>
        /// Check axes if run?
        /// </summary>
        /// <returns>true for running </returns>
        public bool IsAxesRun()
        {
            return MotionBoard.CheckAxisRun();
        }

        /// <summary>
        /// Return state of input port gaven by the bit address
        /// </summary>
        /// <param name="bitAddr"> bit address </param>
        /// <returns> true : low level </returns>
        public bool ReadIoBit(int bitAddr)
        {
            return MotionBoard.ReadIoBit(bitAddr);
        }

        //
        // THREAD methods ---- ----
        //
        public static void StopAllThread()
        {
            _threadFlag = false;
            _iothreadFlag = false;
            _lthreadFlag = false;
            LaserDrives.LaserOff();
        }

        //
        // THREAD: the thread to run handwheel and point chosing.
        //
        public void RunHandWheel()
        {
            StopAllThread();
            Thread.Sleep(10);
            _wthread = new Thread(HandThread)
            {
                Name = nameof(RunHandWheel),
                IsBackground = true,
            };
            _wthread.Start();
        }

        private void HandThread()
        {
            _threadFlag = true;
            short axisIndex = 0;
            short axisNow = 5;
            int ratioCof = 1;
            int ratioCofNow = 0;
            int keyNow = 0;
            bool SetFlag = false;

            while (true)
            {
                if (MotionBoard.ReadIoGroup(out bool[] retinfo))
                {
                    // Scan the note key being pressed
                    if (retinfo[(int)InBit.K1])
                    {
                        if (keyNow != 1)
                        {
                            K1Changed();
                            keyNow = 1;
                        }
                    }
                    else if (retinfo[(int)InBit.K2])
                    {
                        if (keyNow != 2)
                        {
                            K2Changed();
                            keyNow = 2;
                        }
                    }
                    else if (retinfo[(int)InBit.K3])
                    {
                        if (keyNow != 3)
                        {
                            K3Changed();
                            keyNow = 3;
                        }
                    }
                    else
                    {
                        NoChanged();
                        keyNow = 4;
                    }

                    // Choose the ratio
                    if (retinfo[(int)InBit.RX1])
                    {
                        if (ratioCofNow != 1)
                        {
                            SetFlag = true;
                            ratioCof = ratioCofNow = 1;
                        }
                    }
                    else if (retinfo[(int)InBit.RX10])
                    {
                        if (ratioCofNow != 3)
                        {
                            SetFlag = true;
                            ratioCof = ratioCofNow = 3;
                        }
                    }
                    else if (retinfo[(int)InBit.RX100])
                    {
                        if (ratioCofNow != 5)
                        {
                            SetFlag = true;
                            ratioCof = ratioCofNow = 5;
                        }
                    }

                    // choose the Axis
                    if (retinfo[(int)InBit.Xaxis])
                    {
                        if (axisNow != 0)
                        {
                            SetFlag = true;
                            axisIndex = axisNow = 0;
                        }
                    }
                    else if (retinfo[(int)InBit.Yaxis])
                    {
                        if (axisNow != 1)
                        {
                            SetFlag = true;
                            axisIndex = axisNow = 1;
                        }
                    }
                    else if (retinfo[(int)InBit.Zaxis])
                    {
                        if (axisNow != 2)
                        {
                            SetFlag = true;
                            axisIndex = axisNow = 2;
                        }
                    }

                    // setup the new axis and ratio
                    if (SetFlag)
                    {
                        HandWheel.SetHandWheel(axisIndex, ratioCof);
                        SetFlag = false;
                    }
                }
                else if (!_threadFlag)
                {
                    return;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        protected TakeTrace? tkn;
        public void SetActionMethod(TakeTrace tkn)
        {
            this.tkn = tkn;
        }

        private static void K1Changed()
        {
            // do nothing
        }

        private static void K2Changed()
        {
            // do nothing
        }

        private void K3Changed()
        {
            MotionBoard.ReadCoordinate(out int[] pos);
            tkn?.SetXYZ();
        }

        private void NoChanged()
        {
            MotionBoard.ReadCoordinate(out int[] pos);
            tkn?.DisplayXYZ(pos[0], pos[1], pos[2]);
        }

        //
        // THRED: reset all axes
        //
        public void RunResetAxis()
        {
            StopAllThread();
            Thread.Sleep(10);

            _wthread = new Thread(ResetThread)
            {
                Name = nameof(RunResetAxis),
                IsBackground = true,
            };
            _wthread.Start();
        }

        private void ResetThread()
        {
            lock (_lock)
            {
                if (MotionBoard.ResetZaxis() != 0) return;
                if (MotionBoard.ResetYaxis() != 0) return;
                if (MotionBoard.ResetXaxis() != 0) return;
                Thread.Sleep(20);
            }
        }

        //
        // THREAD: Leap Frog to the position
        //
        public bool RunHandLeap(int[] pts, double moveSpeed, double synAcc = 0.5)
        {
            //_threadFlag = false;
            Thread.Sleep(5);

            var hlf = new HandLeapFrog(this);
            hlf.SetParameter(pts, moveSpeed, synAcc);
            _wthread = new Thread(hlf.HandLeapThread)
            {
                Name = nameof(RunHandLeap),
                IsBackground = true,
            };
            _wthread.Start();

            return true;
        }

        //
        // THREAD: weld the trace with echo, using 2D arc interpolation and Z-axis
        //         linear interpolation.
        //

        /// <summary>
        /// Welding the trace, give the echo and return to the first point
        /// </summary>
        /// <param name="laserPower"></param>
        /// <param name="ptsList"> weld points </param>
        /// <param name="WeldSpeed"> weld speed </param>
        /// <param name="moveSpeed"> move speed </param>
        /// <param name="synAcc"></param>
        /// <returns> if the action is not executed, return false </returns>
        public bool RunWeld2(int laserPower, List<int[]> ptsList, double WeldSpeed,
            double moveSpeed, List<int> segLasOff, Dictionary<int, int> tpdic, double synAcc = 0.5)
        {
            if (_wthread != null && _wthread.IsAlive) return false;

            _threadFlag = false;
            Thread.Sleep(5);

            var wt = new WeldTrace2(this);
            wt.SetParameter(ptsList, WeldSpeed, moveSpeed, tpdic, synAcc);

            _wthread = new Thread(wt.WeldThread)
            {
                Name = nameof(RunWeld2),
                IsBackground = true,
            };
            _wthread.Start();

            var lt = new LaserOperate(this);
            lt.SetParameter(laserPower, segLasOff);
            _lthread = new Thread(lt.LaserThread)
            {
                Name = nameof(LaserOperate),
                IsBackground = true,
            };

            Thread.Sleep(10);
            _lthread.Start();

            return true;
        }

        //
        // THREAD: before welding, the header fly a distance
        //

        /// <summary>
        /// Before welding, the header fly a distance
        /// </summary>
        /// <param name="laserPower"></param>
        /// <param name="ptsList"> welding points </param>
        /// <param name="WeldSpeed"></param>
        /// <param name="pts"> fly points </param>
        /// <param name="moveSpeed"></param>
        /// <param name="synAcc"></param>
        /// <returns> if the action is not executed, return false </returns>
        public bool RunFlyWeld(int laserPower, List<int[]> ptsList, double WeldSpeed,
            double moveSpeed, List<int> segLasOff, Dictionary<int, int> tpdic, double synAcc = 0.5)
        {
            if (_wthread != null && _wthread.IsAlive) return false;

            _threadFlag = false;
            Thread.Sleep(5);

            var wft = new FlyWeldTrace(this);
            wft.SetParameter(ptsList, WeldSpeed, moveSpeed, tpdic, synAcc);

            _wthread = new Thread(wft.FlyWeldThread)
            {
                Name = nameof(RunFlyWeld),
                IsBackground = true,
            };
            _wthread.Start();

            Thread.Sleep(10);
            var lt = new LaserOperate(this);
            lt.SetParameter(laserPower, segLasOff);
            _lthread = new Thread(lt.LaserThread)
            {
                Name = nameof(LaserOperate),
                IsBackground = true,
            };

            _lthread.Start();

            return true;
        }

        //
        // Scan some specail In ports, and trigger action
        //

        /// <summary>
        /// Scan the in-port signal and do action according to the signal
        /// </summary>
        /// <param name="bitAddr"> in port address corresponding to the action number </param>
        public void ScanAction(int[] bitAddr)
        {
            _iothreadFlag = false;
            Thread.Sleep(5);

            var sc = new ScanIOaction(this);
            sc.SetParameter(bitAddr);
            Thread sThread = new(sc.ScanThread)
            {
                Name = nameof(ScanAction),
                IsBackground = true,
            };
            sThread.Start();
        }

        protected void FinAction(string iAct)
        {
            FinishDelegate act;

            if (this.vm != null && methodName == nameof(VaneWheelTrace))
            {
                act = new(vm.FinAction);
                act(iAct);
            }
        }

        protected void IoAction(int[] iActNum)
        {
            ActionDelegate act;

            if (this.vm != null && methodName == nameof(VaneWheelTrace))
            {
                act = new(vm.Action);
                act(iActNum);
            }
        }

        internal VaneWheelTrace? vm;
        public void SetActionMethod(VaneWheelTrace vm)
        {
            this.vm = vm;
            methodName = nameof(VaneWheelTrace);
        }

        /// <summary>
        /// Input port map
        /// </summary>
        public enum InBit
        {
            Xaxis = 22,     // BIT 22
            Yaxis = 24,    // BIT 24
            Zaxis = 26,    // BIT 26
            RX1 = 23,
            RX10 = 25,
            RX100 = 27,
            Air = 8,    // BIT 8
            Bit12 = 12,
            Bit13 = 13,
            Bit14 = 14,
            Bit15 = 15,
            Reset = 0x7FFF_FFFF,    // reset all bits

            K1 = 16,
            K2 = 17,
            K3 = 18,
        }
    }

    //
    // Delegate function to operate the main window
    //
    public delegate void FlipLTypeDelegate();
    public delegate void FlipLOnOffDelegate();
    public delegate void SetCrdDelegate(int x, int y, int z);
    public delegate void ActionDelegate(int[] iActNum);
    public delegate void FinishDelegate(string iAct);

}
