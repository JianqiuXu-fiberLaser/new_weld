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
using RobotWeld2.AppModel;
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
        private static Mutex _mutex = new(false);

        private MotionBoard _mb;
        private readonly HandWheel _hand;
        private LaserDrives _laser;

        //
        // the varible used to operate the motion card
        //
        private LaserParameter? _laserParameter;
        private readonly double[] SubPowerPercent;

        private static Thread? wthread;
        private static bool _threadFlag;
        private static bool _iothreadFlag;

        private string methodName = string.Empty;

        public MotionOperate()
        {
            _mb = new MotionBoard();
            _hand = new HandWheel();
            _laser = new LaserDrives();
            SubPowerPercent = new double[5];
        }

        public void InitialCard()
        {
            _mb.InitialCard();
        }

        public void InitialHandwheel()
        {
            _hand.InitialHandwheel();
        }

        public void InitialLaser()
        {
            _laser.InitialLaser(0);
        }

        public void ExitHandwheel()
        {
            _hand.Exit();
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

            int _maxPower = _laserParameter.MaxPower;
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
            _mb.ReadCoordinate(out p);
        }

        public void LaserOnNoRise()
        {
            if (_laserParameter != null)
            {
                _laser.LaserOn(SubPowerPercent[4]);
            }
        }

        public void LaserOffNoFall()
        {
            LaserDrives.LaserOff();
        }

        public void EchoBit(int bitAddr)
        {
            _mb.SelfResetBit(bitAddr);
        }

        public void OpenAir()
        {
            _mb.SetBit(8, false);
        }

        public void CloseAir()
        {
            _mb.SetBit(8, true);
        }

        public void TurnOnBit(int bitAddr)
        {
            _mb.SetBit(bitAddr, true);
        }

        public void TurnOffBit(int bitAddr)
        {
            _mb.SetBit(bitAddr, false);
        }

        /// <summary>
        /// Check axes if run?
        /// </summary>
        /// <returns>true for running </returns>
        public bool IsAxesRun()
        {
            if (_mb != null)
            {
                return _mb.CheckAxisRun();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return state of input port gaven by the bit address
        /// </summary>
        /// <param name="bitAddr"> bit address </param>
        /// <returns> true : low level </returns>
        public bool ReadIoBit(int bitAddr)
        {
            return _mb.ReadIoBit(bitAddr);
        }

        //
        // THREAD methods ---- ----
        //
        public static void StopAllThread()
        {
            _threadFlag = false;
            _iothreadFlag = false;
            LaserDrives.LaserOff();
        }

        //
        // THREAD: the thread to run handwheel and point chosing.
        //
        public void RunHandWheel()
        {
            StopAllThread();
            Thread.Sleep(10);
            wthread = new Thread(HandThread)
            {
                Name = nameof(RunHandWheel),
                IsBackground = true,
            };
            wthread.Start();
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
                if (_mb.ReadIoGroup(out bool[] retinfo))
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
                        _hand.SetHandWheel(axisIndex, ratioCof);
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

        private TakeTrace? tkn;
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
            _mb.ReadCoordinate(out int[] pos);
            tkn?.SetXYZ();
        }

        private void NoChanged()
        {
            _mb.ReadCoordinate(out int[] pos);
            tkn?.DisplayXYZ(pos[0], pos[1], pos[2]);
        }

        //
        // THRED: reset all axes
        //
        public void RunResetAxis()
        {
            StopAllThread();
            Thread.Sleep(10);

            wthread = new Thread(ResetThread)
            {
                Name = nameof(RunResetAxis),
                IsBackground = true,
            };
            wthread.Start();
        }

        private void ResetThread()
        {
            lock (_lock)
            {
                if (_mb.ResetZaxis() != 0) return;
                if (_mb.ResetYaxis() != 0) return;
                if (_mb.ResetXaxis() != 0) return;
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

            HandLeapFrog hlf = new(this);
            hlf.SetParameter(pts, moveSpeed, synAcc);
            wthread = new Thread(hlf.HandLeapThread)
            {
                Name = nameof(RunLeap),
                IsBackground = true,
            };
            wthread.Start();

            return true;
        }

        class HandLeapFrog : MotionOperate
        {
            int[]? pts;
            double moveSpeed;
            double synAcc;

            public HandLeapFrog(MotionOperate mo)
            {
                this._mb = mo._mb;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
            }

            public void SetParameter(int[] pts, double moveSpeed, double synAcc = 0.5)
            {
                this.pts = pts;
                this.moveSpeed = moveSpeed;
                this.synAcc = synAcc;
            }

            public void HandLeapThread()
            {
                if (pts != null)
                {
                    _mutex.WaitOne(3000);
                    _mb.FlyMove(pts, moveSpeed, synAcc);
                    _mutex.ReleaseMutex();
                }
                else
                {
                    DaemonFile.AddInfo("运动参数错误");
                }
            }
        }

        //
        // THREAD: Leap Frog to the position
        //
        public bool RunLeap(int[] pts, double moveSpeed, double synAcc = 0.5)
        {
            _threadFlag = false;
            Thread.Sleep(5);

            LeapFrog lf = new(this);
            lf.SetParameter(pts, moveSpeed, synAcc);
            wthread = new Thread(lf.LeapThread)
            {
                Name = nameof(RunLeap),
                IsBackground = true,
            };
            wthread.Start();

            return true;
        }

        class LeapFrog : MotionOperate
        {
            int[]? pts;
            double moveSpeed;
            double synAcc;

            public LeapFrog(MotionOperate mo)
            {
                this._mb = mo._mb;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
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
                    _mb.FlyMove(pts, moveSpeed, synAcc);
                    _mutex.ReleaseMutex();
                }
                else
                {
                    DaemonFile.AddInfo("运动参数错误");
                }
            }
        }

        //
        // THREAD: Arrive the positon, and echo an flag
        //

        /// <summary>
        /// fly to the position and send flag when arrive the position
        /// </summary>
        /// <param name="pts"> the position </param>
        /// <param name="moveSpeed"></param>
        /// <param name="synAcc"></param>
        /// <returns> if the action is not executed, return false </returns>
        public bool RunArrive(int[] pts, double moveSpeed, double synAcc = 0.5)
        {
            if (wthread != null && wthread.IsAlive) return false;

            //_threadFlag = false;
            Thread.Sleep(5);

            Arrive ar = new(this);
            ar.SetParameter(pts, moveSpeed, synAcc);
            wthread = new Thread(ar.ArriveThread)
            {
                Name = nameof(RunArrive),
                IsBackground = true,
            };
            wthread.Start();

            return true;
        }

        class Arrive : MotionOperate
        {
            int[]? pts;
            double moveSpeed;
            double synAcc;

            public Arrive(MotionOperate mo)
            {
                this._mb = mo._mb;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
            }

            public void SetParameter(int[] pts, double moveSpeed, double synAcc = 0.5)
            {
                this.pts = pts;
                this.moveSpeed = moveSpeed;
                this.synAcc = synAcc;
            }

            public void ArriveThread()
            {
                if (pts != null)
                {
                    _mutex.WaitOne(3000);
                    _mb.FlyMove(pts, moveSpeed, synAcc);
                    _mutex.ReleaseMutex();

                    FinAction("arrive");
                }
                else
                {
                    DaemonFile.AddInfo("运动参数错误");
                }
            }
        }

        //
        // THREAD: weld the trace
        //

        /// <summary>
        /// Welding the trace, give the echo and return to the first point
        /// </summary>
        /// <param name="laserPower"></param>
        /// <param name="ptsList"> weld points </param>
        /// <param name="WeldSpeed"></param>
        /// <param name="synAcc"></param>
        /// <returns> if the action is not executed, return false </returns>
        public bool RunWeld(int laserPower, List<int[]> ptsList, double WeldSpeed,
            double moveSpeed, double synAcc = 0.5)
        {
            if (wthread != null && wthread.IsAlive) return false;

            _threadFlag = false;
            Thread.Sleep(5);

            WeldTrace wt = new(this);
            wt.SetParameter(laserPower, ptsList, WeldSpeed, synAcc);

            wthread = new Thread(wt.WeldThread)
            {
                Name = nameof(RunWeld),
                IsBackground = true,
            };
            wthread.Start();

            return true;
        }

        class WeldTrace : MotionOperate
        {
            private List<int[]>? ptsList;
            private readonly LaserParameter? lp;
            private double WeldSpeed;
            private double moveSpeed;
            private double synAcc = 0.5;
            private int maxPower;
            private double powerPercent;

            public WeldTrace(MotionOperate mo)
            {
                _mb = mo._mb;
                _laser = mo._laser;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
                this.lp = mo._laserParameter;
            }

            public void SetParameter(int laserPower, List<int[]> ptsList, double WeldSpeed,
                double moveSpeed, double synAcc = 0.5)
            {
                this.ptsList = ptsList;
                this.WeldSpeed = WeldSpeed;
                this.moveSpeed = moveSpeed;
                this.synAcc = synAcc;

                if (lp != null && lp.MaxPower > 0)
                {
                    this.maxPower = lp.MaxPower;
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
                _mb.FlyMove(ptsList[0], moveSpeed, synAcc);

                for (int i = 0; i < DaemonModel.TraceCount; i++)
                {
                    int sp = i * 7 + 1;
                    var ptarc = new List<int[]>();
                    ptarc.AddRange(ptsList.GetRange(sp, DaemonModel.TracePoint));

                    _mutex.WaitOne(3000);
                    _mb.FlyMove(ptsList[sp], moveSpeed, synAcc);
                    _laser.LaserOn(powerPercent);
                    _mb.WeldMove(ptarc, WeldSpeed, synAcc);
                    _mutex.ReleaseMutex();

                    LaserDrives.LaserOff();
                }

                // move to the prepared point
                _mb.FlyMove(ptsList[0], moveSpeed, synAcc);
                FinAction("weld");

            }
        }

        //
        // THREAD: weld the trace without echo
        //

        /// <summary>
        /// Welding the trace, no echo, and stay the last point
        /// </summary>
        /// <param name="laserPower"></param>
        /// <param name="ptsList"> weld points </param>
        /// <param name="WeldSpeed"></param>
        /// <param name="pts"> the ready position </param>
        /// <param name="moveSpeed"></param>
        /// <param name="synAcc"></param>
        /// <returns> if the action is not executed, return false </returns>
        public bool RunWeld_NoEcho(int laserPower, List<int[]> ptsList, double WeldSpeed, int[] pts, double moveSpeed,
            double synAcc = 0.5)
        {
            if (wthread != null && wthread.IsAlive) return false;

            _threadFlag = false;
            Thread.Sleep(5);

            WeldTraceNoEcho wtn = new(this);
            wtn.SetParameter(laserPower, ptsList, WeldSpeed, pts, moveSpeed, synAcc);

            wthread = new Thread(wtn.WeldThread)
            {
                Name = nameof(RunWeld_NoEcho),
                IsBackground = true,
            };
            wthread.Start();

            return true;
        }

        class WeldTraceNoEcho : MotionOperate
        {
            private List<int[]>? ptsList;
            private readonly LaserParameter? lp;
            private double WeldSpeed;
            private int[]? pts;
            private double moveSpeed;
            private double synAcc = 0.5;
            private double powerPercent;

            public WeldTraceNoEcho(MotionOperate mo)
            {
                _mb = mo._mb;
                _laser = mo._laser;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
                this.lp = mo._laserParameter;
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

                    _mb.FlyMove(ptsList[0], WeldSpeed, synAcc);
                    _laser.LaserOn(powerPercent);
                    _mb.WeldMove(ptsList, WeldSpeed, synAcc);
                    LaserDrives.LaserOff();

                    _mb.FlyMove(pts, moveSpeed, synAcc);
                    _mutex.ReleaseMutex();
                    FinAction("arrive");
                }
                else
                {
                    DaemonFile.AddInfo("焊接参数错误");
                }
            }
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
            double moveSpeed, double synAcc = 0.5)
        {
            if (wthread != null && wthread.IsAlive) return false;

            _threadFlag = false;
            Thread.Sleep(5);

            FlyWeldTrace wft = new(this);
            wft.SetParameter(laserPower, ptsList, WeldSpeed, moveSpeed, synAcc);

            wthread = new Thread(wft.FlyWeldThread)
            {
                Name = nameof(RunFlyWeld),
                IsBackground = true,
            };
            wthread.Start();

            return true;
        }

        class FlyWeldTrace : MotionOperate
        {
            private List<int[]>? ptsList;
            private readonly LaserParameter? lp;
            private double WeldSpeed;
            private double moveSpeed;
            private double synAcc = 0.5;
            private int maxPower;
            private double powerPercent;

            public FlyWeldTrace(MotionOperate mo)
            {
                _mb = mo._mb;
                _laser = mo._laser;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
                this.lp = mo._laserParameter;
            }

            public void SetParameter(int laserPower, List<int[]> ptsList, double WeldSpeed,
                double moveSpeed, double synAcc = 0.5)
            {
                this.ptsList = ptsList;
                this.WeldSpeed = WeldSpeed;
                this.moveSpeed = moveSpeed;
                this.synAcc = synAcc;

                if (lp != null && lp.MaxPower > 0)
                {
                    maxPower = lp.MaxPower;
                    powerPercent = (double)laserPower / (double)maxPower;
                }
            }

            public void FlyWeldThread()
            {
                if (ptsList == null || ptsList.Count == 0)
                {
                    DaemonFile.AddInfo("焊接参数错误");
                    return;
                }

                // long moving to the prepared point, and a protected moving.
                _mb.FlyMove(ptsList[0], moveSpeed, synAcc);
                _mb.FlyMove(ptsList[0], 0.5 * moveSpeed, synAcc);

                for (int i = 0; i < DaemonModel.TraceCount; i++)
                {
                    int sp = i * 7 + 1;
                    var ptarc = new List<int[]>();
                    ptarc.AddRange(ptsList.GetRange(sp, DaemonModel.TracePoint));

                    _mutex.WaitOne(3000);
                    _mb.FlyMove(ptsList[sp], 0.5 * moveSpeed, synAcc);
                    _laser.LaserOn(powerPercent);
                    _mb.WeldMove(ptarc, WeldSpeed, synAcc);
                    _mutex.ReleaseMutex();

                    LaserDrives.LaserOff();
                }
                // move to the prepared point.
                _mb.FlyMove(ptsList[0], 0.5 * moveSpeed, synAcc);
                FinAction("flyweld");

            }
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

            ScanIOaction sc = new(this);
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

        private VaneWheelTrace? vm;
        public void SetActionMethod(VaneWheelTrace vm)
        {
            this.vm = vm;
            methodName = nameof(VaneWheelTrace);
        }

        class ScanIOaction : MotionOperate
        {
            int[]? bitAddr;

            public ScanIOaction(MotionOperate mo)
            {
                this.vm = mo.vm;
                this.methodName = mo.methodName;
            }

            public void SetParameter(in int[] bitAddr)
            {
                this.bitAddr = bitAddr;
            }

            public void ScanThread()
            {
                if (bitAddr == null) { return; }

                _iothreadFlag = true;
                int[] iActNum = new int[bitAddr.Length];

                while (true)
                {
                    for (int i = 0; i < bitAddr.Length; i++)
                    {
                        iActNum[i] = 0;
                    }

                    for (int i = 0; i < bitAddr.Length; i++)
                    {
                        if (_mb.ReadIoBit(bitAddr[i]))
                        {
                            iActNum[i] = 1;
                        }
                        else
                        {
                            iActNum[i] = 0;
                        }
                    }

                    for (int i = 0; i < bitAddr.Length; i++)
                    {
                        if (iActNum[i] != 0)
                        {
                            IoAction(iActNum);
                        }
                    }

                    if (!_iothreadFlag)
                    {
                        return;
                    }

                    Thread.Sleep(20);
                }
            }
        }
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

    //
    // Delegate function to operate the main window
    //
    public delegate void FlipLTypeDelegate();
    public delegate void FlipLOnOffDelegate();
    public delegate void SetCrdDelegate(int x, int y, int z);
    public delegate void ActionDelegate(int[] iActNum);
    public delegate void FinishDelegate(string iAct);

}
