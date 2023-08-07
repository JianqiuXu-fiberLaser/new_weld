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
        //-----------------------------------------------------
        // This smooth Coefficient depends on the properties of
        // rotating mechanics, Which in most case not need to be
        // changed.
        //-----------------------------------------------------
        public const double SmoothCoefficient = 0.25;

        private readonly object _lock = new();

        protected MotionBoard _mb;
        protected HandWheel _hand;
        protected LaserDrives _laser;
        private static double oneCycle;
        private static double xMillimeter;
        private static double yMillimeter;
        private static double zMillimeter;
        private static double maxPower;

        //
        // the varible used to operate the motion card
        //
        private LaserArgument? _laserArgument;
        private readonly double[] SubPowerPercent;
        private static Thread? wthread;

        private Thread? handthread;
        private static bool _handthreadFlag;

        private static bool _iothreadFlag;
        private string methodName = string.Empty;

        public MotionOperate()
        {
            _mb = new MotionBoard();
            _hand = new HandWheel();
            _laser = new LaserDrives();
            SubPowerPercent = new double[5];
        }

        public MotionBoard motionboard
        {
            get => _mb;
        }

        public void InitialCard()
        {
            _mb.InitialCard();
        }

        public void InitialHandwheel()
        {
            _hand.InitialHandwheel();
        }

        public static void SetMaxPower(int mPower)
        {
            maxPower = mPower;
        }

        public static int GetMaxPower()
        {
            return (int)maxPower;
        }

        public static double Xmillimeter
        {
            get => xMillimeter;
            set => xMillimeter = value;
        }

        public static double Ymillimeter
        {
            get => yMillimeter;
            set => yMillimeter = value;
        }

        public static double Zmillimeter
        {
            get => zMillimeter;
            set => zMillimeter = value;
        }

        public static double OneCycle
        {
            get => oneCycle;
            set => oneCycle = value;
        }

        //-------------------------------------------------------------
        // Operate Laser
        //-------------------------------------------------------------

        /// <summary>
        /// Setup the laser work mode
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="duty"></param>
        public void SetLaserMode(int frequency = 0, int duty = 0)
        {
            if (frequency == 0 && duty == 0)
            {
                _laser.InitialLaser();
            }
            else
            {
                _laser.InitialLaser(frequency, duty);
            }
        }


        public void SetupLaserParameter(in LaserWeldParameter parameter)
        {
            _laserArgument = new LaserArgument(parameter);
            PowerSubdivision();
        }

        public void SetupLaserParameter(in LaserArgument argument)
        {
            _laserArgument = argument;
            PowerSubdivision();
        }

        //
        // Caculate the subdivision for the laser power
        //
        private void PowerSubdivision()
        {
            if (_laserArgument == null || maxPower == 0)
            {
                Promption.Prompt("The laser maxpower is zero.", 001);
                return;
            }

            int LaserPower = _laserArgument.Power;
            SubPowerPercent[0] = 0.2 * LaserPower / maxPower;
            SubPowerPercent[1] = 0.4 * LaserPower / maxPower;
            SubPowerPercent[2] = 0.6 * LaserPower / maxPower;
            SubPowerPercent[3] = 0.8 * LaserPower / maxPower;
            SubPowerPercent[4] = 1.0 * LaserPower / maxPower;
        }

        public void LaserOnNoRise()
        {
            if (_laserArgument != null)
            {
                _laser.LaserOn(SubPowerPercent[4]);
            }
        }

        public void LaserOffNoFall()
        {
            LaserDrives.LaserOff();
        }

        public void AdjustLaserPower(in LaserArgument arg, in int power)
        {
            if (_laserArgument == null)
            {
                _laserArgument = arg;
                if (_laserArgument.Frequency == 0 || _laserArgument.Duty == 0 
                    || _laserArgument.Duty == 100)
                {
                    SetLaserMode();
                }
                else
                {
                    SetLaserMode(_laserArgument.Frequency, _laserArgument.Duty);
                }
            }

            if (maxPower != 0)
            {
                _laser.LaserOn(power / maxPower);
            }
            else
            {
                Promption.Prompt("The laser maxpower is zero.", 002);
            }
        }

        //-------------------------------------------------------------
        // Operate Hand Wheel
        //-------------------------------------------------------------
        public void ExitHandwheel()
        {
            _hand.Exit();
        }

        public void StopHandwheel()
        {
            _handthreadFlag = false;
        }

        public static void StopAllThread()
        {
            _handthreadFlag = false;
            _iothreadFlag = false;
            LaserDrives.LaserOff();
        }

        public void RunHandWheel()
        {
            StopAllThread();
            Thread.Sleep(10);
            handthread = new Thread(HandThread)
            {
                Name = nameof(RunHandWheel),
                IsBackground = true,
            };
            handthread.Start();
        }

        private void HandThread()
        {
            _handthreadFlag = true;

            // the  key index is set to -1, which is not real key index.
            // The first press of key can be captured.
            short axisIndex = -1;
            short axisNow = -1;
            int ratioCof = -1;
            int ratioCofNow = -1;
            bool SetFlag = false;

            while (true)
            {
                NoChanged();
                if (_mb.ReadIoGroup(out bool[] retinfo))
                {
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
                    else if (retinfo[(int)(InBit.Aaxis)])
                    {
                        if (axisNow != 3)
                        {
                            SetFlag = true;
                            axisIndex = axisNow = 3;
                        }
                    }

                    // setup the new axis and ratio
                    if (SetFlag)
                    {
                        _hand.SetHandWheel(axisIndex, ratioCof);
                        SetFlag = false;
                    }
                }

                if (!_handthreadFlag)
                {
                    return;
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        private TakeTrace? tkn;
        public void SetActionMethod(TakeTrace tkn)
        {
            this.tkn = tkn;
        }

        /// <summary>
        /// Read coordinate of XYZ axes
        /// </summary>
        /// <param name="pos"></param>
        public void ReadCoordinate(out int[] pos)
        {
            pos = new int[4];
            _mb.ReadCoordinate(out int[] barePos);

            pos[0] = MotionSpecification.XDirection * barePos[0];
            pos[1] = MotionSpecification.YDirection * barePos[1];
            pos[2] = MotionSpecification.ZDirection * barePos[2];
            pos[3] = barePos[3];
        }

        private void NoChanged()
        {
            _mb.ReadCoordinate(out int[] barePos);

            int[] pos = new int[4];
            pos[0] = MotionSpecification.XDirection * barePos[0];
            pos[1] = MotionSpecification.YDirection * barePos[1];
            pos[2] = MotionSpecification.ZDirection * barePos[2];
            pos[3] = barePos[3];
            tkn?.DisplayXYZ(pos[0], pos[1], pos[2], pos[3]);
        }

        //-------------------------------------------------------------
        // Operate IO
        //-------------------------------------------------------------
        public void EchoBit(int bitAddr)
        {
            _mb.SelfResetBit(bitAddr);
        }

        public void OpenAir()
        {
            _mb.SetBit(10, false);
        }

        public void CloseAir()
        {
            _mb.SetBit(10, true);
        }

        public void TurnOnBit(int bitAddr)
        {
            _mb.SetBit(bitAddr, true);
        }

        public void TurnOffBit(int bitAddr)
        {
            _mb.SetBit(bitAddr, false);
        }

        //-------------------------------------------------------------
        // THRED: reset all axes
        //-------------------------------------------------------------

        /// <summary>
        /// Reset 3 axes and A-axis
        /// </summary>
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

            Thread rthread = new(_mb.ResetAaxis)
            {
                IsBackground = true,
            };
            rthread.Start();
        }

        private void ResetThread()
        {
            lock (_lock)
            {
                _mb.ResetAll();
                Thread.Sleep(20);
            }
        }

        //-------------------------------------------------------------
        // Axis moving functions
        //-------------------------------------------------------------

        public void MoveLine(List<int[]> pts, List<double> speed, int StartSegNum)
        {
            lock (_lock)
            {
                _mb.LineMove(pts, speed, startSeg: StartSegNum);
            }
        }

        public void MoveArc(List<int[]> pts, List<double> speed, int StartSegNum)
        {
            lock (_lock)
            {
                _mb.CircleMove(pts, speed, startSeg:StartSegNum);
            }
        }

        public void Move4D(List<int[]> pts, List<double> speed, int StartSegNum)
        {
            lock(_lock)
            {
                _mb.RotateMove(pts, speed, startSeg:StartSegNum);
            }
        }

        public int GetSectionNumber()
        {
            return _mb.GetMoveSegmentNumber();
        }

        //-------------------------------------------------------------
        // ROUNTIN : Scan some specail in-ports, and trigger action
        //-------------------------------------------------------------

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

        private RunWeld4D? runWeld4D;
        private RunWeldXYZ? runWeldXYZ;
        private RunWeldHybrid? runWeldHybrid;
        protected void IoAction(int[] iActNum)
        {
            ActionDelegate act;

            if (this.runWeldXYZ != null && methodName == nameof(RunWeldXYZ))
            {
                act = new(runWeldXYZ.Action);
                act(iActNum);
            }
            else if (this.runWeld4D != null && methodName == nameof(RunWeld4D))
            {
                act = new(runWeld4D.Action);
                act(iActNum);
            }
            else if (this.runWeldHybrid != null && methodName == nameof(RunWeldHybrid))
            {
                act = new(runWeldHybrid.Action);
                act(iActNum);
            }
        }

        protected void NoAction(int[] iActNum)
        {
            NoActionDelegate nact;

            if (this.runWeldXYZ != null && methodName == nameof(RunWeldXYZ))
            {
                nact = new(runWeldXYZ.NoAction);
                nact(iActNum);
            }
            else if (this.runWeld4D != null && methodName == nameof(RunWeld4D))
            {
                nact = new(runWeld4D.NoAction);
                nact(iActNum);
            }
            else if (this.runWeldHybrid != null && methodName == nameof(RunWeldHybrid))
            {
                nact = new(runWeldHybrid.NoAction);
                nact(iActNum);
            }
        }

        public void SetActionMethod(RunWeld4D runWeld4D)
        {
            this.runWeld4D = runWeld4D;
            methodName = nameof(RunWeld4D);
        }

        public void SetActionMethod(RunWeldXYZ runWeldXYZ)
        {
            this.runWeldXYZ = runWeldXYZ;
            methodName = nameof(RunWeldXYZ);
        }

        public void SetActionMethod(RunWeldHybrid runWeldHybrid)
        {
            this.runWeldHybrid = runWeldHybrid;
            methodName = nameof(RunWeldHybrid);
        }

        //
        // CLASS : Scan the IO signal
        //
        class ScanIOaction : MotionOperate
        {
            private int[]? bitAddr;

            public ScanIOaction(MotionOperate mo)
            {
                this.runWeld4D = mo.runWeld4D;
                this.runWeldXYZ = mo.runWeldXYZ;
                this.runWeldHybrid = mo.runWeldHybrid;
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
                        else
                        {
                            NoAction(iActNum);
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
        Aaxis = 21,    // BIT 21
        Xaxis = 22,    // BIT 22
        Yaxis = 24,    // BIT 24
        Zaxis = 26,    // BIT 26
        RX1 = 23,
        RX10 = 25,
        RX100 = 27,
        Air = 8,       // BIT 8
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
    public delegate void ActionDelegate(int[] iActNum);
    public delegate void NoActionDelegate(int[] iActNum);
    public delegate void FinishDelegate(string iAct);
}
