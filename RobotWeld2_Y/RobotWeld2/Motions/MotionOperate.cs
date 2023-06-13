using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Printing;
using System.Threading;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// CLASS: operation through the motion card, i.e., moving, laser and handwheel
    /// Manage threads about motion operation.
    /// </summary>
    public class MotionOperate
    {
        private readonly object _lock = new object();
        private static Mutex _mutex = new Mutex(false);

        private MotionBoard _mb;
        private HandWheel _hand;
        private LaserDrives _laser;

        //
        // the varible used to operate the motion card
        //
        private LaserParameter? _laserParameter;
        private double[] SubPowerPercent;

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

        public void SetupLaserParameter(in LaserParameter lp)
        {
            if (lp.MaxPower == 0)
            {
                GiveMsg.Show("最大激光功率不能为零");
            }
            else
            {
                _laserParameter ??= new LaserParameter(lp.MaxPower);
                _laserParameter.MaxPower = lp.MaxPower;
                _laserParameter.LaserPower = lp.LaserPower;
                _laserParameter.Frequency = lp.Frequency;
                _laserParameter.PulseWidth = lp.PulseWidth;
                _laserParameter.DutyCycle = lp.DutyCycle;
                _laserParameter.LaserRise = lp.LaserRise;
                _laserParameter.LaserFall = lp.LaserFall;
                _laserParameter.LaserHoldtime = lp.LaserHoldtime;
                _laserParameter.AirIn = lp.AirIn;
                _laserParameter.AirOut = lp.AirOut;
                _laserParameter.WireLength = lp.WireLength;
                _laserParameter.WireTime = lp.WireTime;
                _laserParameter.WireBack = lp.WireBack;
                _laserParameter.WireSpeed = lp.WireSpeed;
                PowerSubdivision();
            }
        }

        public void SetupLaserPower(int power, int MaxPower)
        {
            if (_laserParameter != null)
            {
                _laserParameter.MaxPower = MaxPower;
                _laserParameter.LaserPower = power;
            }
            else
            {
                DaemonFile.AddInfo("激光参数不存在");
                _laserParameter = new LaserParameter(MaxPower);
                _laserParameter.LaserPower = power;
            }
            PowerSubdivision();
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

        private void K1Changed()
        {
            tkn?.ChangeLinetype();
        }

        private void K2Changed()
        {
            tkn?.ChangeLaseronoff();
        }

        private void K3Changed()
        {
            _mb.ReadCoordinate(out int[] pos);
            tkn?.SetXYZ(pos[0], pos[1], pos[2]);
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
        public bool RunWeld(int laserPower, List<int[]> ptsList, double WeldSpeed,
            double synAcc = 0.5)
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
            private LaserParameter? lp;
            private double WeldSpeed;
            private double synAcc = 0.5;
            private int laserPower;
            private int maxPower;
            private double powerPercent;

            public WeldTrace(MotionOperate mo)
            {
                this._mb = mo._mb;
                this._laser = mo._laser;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
                this.lp = mo._laserParameter;
            }

            public void SetParameter(int laserPower, List<int[]> ptsList, double WeldSpeed,
                double synAcc = 0.5)
            {
                this.laserPower = laserPower;
                this.ptsList = ptsList;
                this.WeldSpeed = WeldSpeed;
                this.synAcc = synAcc;

                if (lp != null && lp.MaxPower > 0)
                {
                    this.maxPower = lp.MaxPower;
                    powerPercent = (double)laserPower / (double)maxPower;
                }
            }

            public void WeldThread()
            {
                if (ptsList != null && ptsList.Count > 0)
                {
                    _mutex.WaitOne(3000);
                    _mb.FlyMove(ptsList[0], WeldSpeed, synAcc);
                    
                    _laser.LaserOn(powerPercent);

                    _mb.WeldMove(ptsList, WeldSpeed, synAcc);

                    _mutex.ReleaseMutex();
                    LaserDrives.LaserOff();

                    FinAction("weld");
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

        public bool RunFlyWeld(int laserPower, List<int[]> ptsList, double WeldSpeed,
            int[] pts, double moveSpeed, double synAcc = 0.5)
        {
            if (wthread != null && wthread.IsAlive) return false;

            _threadFlag = false;
            Thread.Sleep(5);

            FlyWeldTrace wft = new(this);
            wft.SetParameter(laserPower, ptsList, WeldSpeed, pts, moveSpeed, synAcc);

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
            private LaserParameter? lp;
            private double WeldSpeed;
            private double moveSpeed;
            private int[]? pts;
            private double synAcc = 0.5;
            private int laserPower;
            private int maxPower;
            private double powerPercent;

            public FlyWeldTrace(MotionOperate mo)
            {
                this._mb = mo._mb;
                this._laser = mo._laser;
                this.vm = mo.vm;
                this.methodName = mo.methodName;
                this.lp = mo._laserParameter;
            }

            public void SetParameter(int laserPower, List<int[]> ptsList, double WeldSpeed,
                int[] pts, double moveSpeed, double synAcc = 0.5)
            {
                this.laserPower = laserPower;
                this.ptsList = ptsList;
                this.WeldSpeed = WeldSpeed;
                this.pts = pts;
                this.moveSpeed = moveSpeed;
                this.synAcc = synAcc;

                if (lp != null && lp.MaxPower > 0)
                {
                    this.maxPower = lp.MaxPower;
                    powerPercent = (double)laserPower / (double)maxPower;
                }
            }

            public void FlyWeldThread()
            {
                if (pts != null && ptsList != null && ptsList.Count > 0)
                {
                    _mutex.WaitOne(3000);
                    _mb.FlyMove(pts, moveSpeed, synAcc);
                    OpenAir();
                    _mb.FlyMove(ptsList[0], 0.5*moveSpeed, synAcc);

                    _laser.LaserOn(powerPercent);

                    _mb.WeldMove(ptsList, WeldSpeed, synAcc);
                    _mutex.ReleaseMutex();

                    LaserDrives.LaserOff();
                    FinAction("flyweld");
                }
                else
                {
                    DaemonFile.AddInfo("焊接参数错误");
                }
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
