using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Dirct write and read to motion card, but wrapped with lock function
    ///  to avoid conflection on the motion card.
    /// </summary>
    public class MotionCard
    {
        protected static Mutex mut = new();

        //
        // Fundamental Handle for the motion card and the coordinate system
        //
        public ushort devHandle;
        public ushort pcrdHandle;
        public ushort[] axisHandle = new ushort[4];

        private readonly short _followRatio = 50;
        private readonly double _acceSpeed = 0.1;
        private readonly double _followSpeed = 2000;

        protected double WeldSpeed;
        protected double moveSpeed;

        private KeyMap oldAxis;
        private KeyMap oldRatio;
        private bool _hasSet;
        protected double synAcc = 0.5;

        private static bool _stopHandWheel;
        public static bool StopHandWheel
        {
            get { return _stopHandWheel; }
            set { _stopHandWheel = value; }
        }

        private static bool _selectionOK;
        private static bool _keyReset;

        public static bool SelectionOK
        {
            get { return _selectionOK; }
            set { _selectionOK = value; }
        }

        public TakeTrace? tkc;
        public int[]? ptxyz;

        public MotionCard() { }

        public void ConfigMotionSystem()
        {
            if (ConfigCard())
            {
                ConfigCoordinate();
            }
        }
        //
        // Configure the motion card
        //
        private bool ConfigCard()
        {
            // open motion card and load the configure file.
            short rtn = NMC_DevOpen(0, ref devHandle);
            //Assertion.AssertError("打开运动卡错误", rtn);
            if (rtn != 0)
            {
                DaemonFile.AddInfo("打开运动卡错误\n");
            }

            if (rtn == 0)
            {
                rtn = NMC_LoadConfigFromFile(devHandle, Encoding.Default.GetBytes("./GCN400.cfg"));
                Assertion.AssertError("装载运动卡配置失败", rtn);

                for (short i = 0; i < 3; i++)
                {
                    rtn = NMC_MtOpen(devHandle, i, ref axisHandle[i]);
                    Assertion.AssertError("打开运动轴错误", rtn);
                    rtn = NMC_MtClrError(axisHandle[i]);
                    Assertion.AssertError("运动轴限位去除错误", rtn);
                    rtn = NMC_MtSetSvOn(axisHandle[i]);
                    Assertion.AssertError("设置运动轴错误", rtn);
                }
                return true;
            }
            else
                return false;
        }

        //
        // Configure the coordinate
        //
        private void ConfigCoordinate()
        {
            // open the coordinate
            short rtn = NMC_CrdOpen(devHandle, ref pcrdHandle);
            //Assertion.AssertError("打开坐标系统错误", rtn);

            // configure the coordinate
            TCrdConfig crd = new(true)
            {
                axCnts = 3
            };
            crd.pAxArray[0] = 0;
            crd.pAxArray[1] = 1;
            crd.pAxArray[2] = 2;
            crd.port[0] = 0;
            crd.port[1] = 0;
            crd.port[2] = 0;
            rtn = NMC_CrdConfig(pcrdHandle, ref crd);
            //Assertion.AssertError("配置坐标系错误", rtn);

            // configure moving parameters
            TCrdPara crdPara = new(true)
            {
                orgFlag = 1,
                synAccMax = 50,
                synVelMax = 800
            };
            crdPara.offset[0] = 0;
            crdPara.offset[1] = 0;
            crdPara.offset[2] = 0;
            rtn = NMC_CrdSetPara(pcrdHandle, ref crdPara);
            //Assertion.AssertError("配置坐标系运动参数错误", rtn);

            // clear the command buffer
            rtn = NMC_CrdBufClr(pcrdHandle);
            //Assertion.AssertError("清除坐标缓存错误", rtn);
        }

        /// <summary>
        /// close the motion card and dicard the memory
        /// </summary>
        public void CloseCard()
        {
            short rtn = NMC_CrdClose(ref pcrdHandle);
            //Assertion.AssertError("关闭坐标系错误", rtn);

            rtn = NMC_DevClose(ref devHandle);
            //Assertion.AssertError("关闭运动卡错误", rtn);
        }

        /// <summary>
        /// Configurate the Hand Wheel
        /// </summary>
        public void ConfigHandWheel()
        {
            NMC_ClrHandWheel(devHandle);
            short rtn = NMC_SetHandWheelInput(devHandle, 256);
            // Assertion.AssertError("设置手轮错误", rtn);
            if (rtn != 0)
            {
                DaemonFile.AddInfo("设置手轮错误\n");
            }
            else
            {
                DaemonFile.AddInfo("设置手轮成功\n");
            }

            rtn = NMC_SetHandWheel(devHandle, 0, _followRatio, _acceSpeed, _followSpeed);
            //Assertion.AssertError("设置手轮初始参数错误", rtn);
        }

        /// <summary>
        /// Start Hand Wheel thread and capture the action of HandWheel
        /// </summary>
        public void RunHand()
        {
            StopHandWheel = true;
            Thread handThread = new(HandKey)
            {
                IsBackground = true,
            };
            handThread.Start();
        }

        //
        // Task: Scan key action in the Hand wheel
        //
        private void HandKey()
        {
            StopHandWheel = false;
            short axisIndex = 0;
            double ratio = _followRatio;
            _hasSet = true;
            oldAxis = KeyMap.Xaxis;

            while (true)
            {
                NMC_GetDIGroup(devHandle, out int isignal, 0);

                //---- set the handWheel by the key selection ----//

                // Choice of the hand axis
                if ((~isignal & (int)ComBit.Xaxis) == (int)ComBit.Xaxis)
                {
                    if (oldAxis != KeyMap.Xaxis && _hasSet)
                    {
                        axisIndex = 0;
                        _hasSet = false;
                        oldAxis = KeyMap.Xaxis;
                    }
                }
                else if ((~isignal & (int)ComBit.Yaxis) == (int)ComBit.Yaxis)
                {
                    if (oldAxis != KeyMap.Yaxis && _hasSet)
                    {
                        axisIndex = 1;
                        _hasSet = false;
                        oldAxis = KeyMap.Yaxis;
                    }
                }
                else if ((~isignal & (int)ComBit.Zaxis) == (int)ComBit.Zaxis)
                {
                    if (oldAxis != KeyMap.Zaxis && _hasSet)
                    {
                        axisIndex = 2;
                        _hasSet = false;
                        oldAxis = KeyMap.Yaxis;
                    }
                }
                else if (~(isignal & (int)ComBit.RX1) == (int)ComBit.RX1)
                {
                    if (oldRatio != KeyMap.RX1 && _hasSet)
                    {
                        ratio = _followRatio;
                        _hasSet = false;
                        oldRatio = KeyMap.RX1;
                    }
                }
                else if ((~isignal & (int)ComBit.RX10) == (int)ComBit.RX10)
                {
                    ratio = (double)KeyMap.RX10 * _followRatio;
                    if (oldRatio != KeyMap.RX10 && _hasSet)
                    {
                        oldRatio = KeyMap.RX10;
                        _hasSet = false;
                        oldAxis = KeyMap.Xaxis;
                    }
                }
                else if ((~isignal & (int)ComBit.RX100) == (int)ComBit.RX100)
                {
                    if (oldRatio != KeyMap.RX100 && _hasSet)
                    {
                        ratio = (double)KeyMap.RX100 * _followRatio;
                        _hasSet = false;
                        oldRatio = KeyMap.RX100;
                    }
                }

                if (!_hasSet)
                {
                    ChangeHandWheel(axisIndex, ratio);
                    _hasSet = true;
                }

                if (StopHandWheel) { break; }
                Thread.Sleep(100);
            }
        }

        //
        // Change the follow ratio for hand wheel
        //
        private void ChangeHandWheel(short axisIndex, double ratio)
        {
            NMC_ClrHandWheel(devHandle);
            NMC_SetHandWheel(devHandle, axisIndex, ratio, _acceSpeed, _followSpeed);
        }

        public void GetTakeTrace(TakeTrace tkc)
        {
            this.tkc = tkc;
        }

        //
        // the task to read coordinate
        //
        public void ReadCoordinate()
        {
            SelectionOK = false;
            _keyReset = true;

            if (tkc == null) return;

            FlipLineTypeDelegate flt = new(tkc.ChangeLinetype);
            FlipLaserOnOffDelegate flof = new(tkc.ChangeLaseronoff);
            SetXYZDelegate stxyz = new(tkc.SetXYZ);

            while (true)
            {
                _keyReset = true;

                int[] pos = new int[3];
                for (short i = 0; i < 3; i++)
                {
                    if (axisHandle != null)
                        NMC_MtGetPrfPos(axisHandle[i], ref pos[i]);
                }
                stxyz(pos[0], pos[1], pos[2]);

                NMC_GetDIGroup(devHandle, out int isignal, 0);

                if (((~isignal & (int)ComBit.K1) == 0) && _keyReset)
                {
                    _keyReset = false;
                    flt();
                }
                else if (((~isignal & (int)ComBit.K2) == 0) && _keyReset)
                {
                    _keyReset = false;
                    flof();
                }
                else if (((~isignal & (int)ComBit.K3) == 0) && _keyReset)
                {
                    _keyReset = false;
                    AddPoint(tkc);
                }
                else if (SelectionOK || StopHandWheel)
                {
                    return;
                }
                else if ((~isignal & (int)ComBit.K1) != 0 && (~isignal & (int)ComBit.K2) != 0 &&
                    (~isignal & (int)ComBit.K2) != 0)
                {
                    _keyReset = true;
                }

                Thread.Sleep(100);
            }
        }

        //
        // Add a point to the point's list
        //
        private void AddPoint(TakeTrace tkc)
        {
            AddPointDelegate addpt = new(tkc.AddPoint);
            int[] pos = new int[3];
            for (short i = 0; i < 3; i++)
            {
                if (axisHandle != null)
                    NMC_MtGetPrfPos(axisHandle[i], ref pos[i]);
            }

            Vector vc = new(pos[0], pos[1], pos[2]);
            addpt(vc);
        }

        /// <summary>
        /// The routin to weld the trace, usually in the background thread
        /// </summary>
        /// <param name="lsop"> laser operation reference </param>
        /// <param name="mode">0: CW no rise; 1: CW with rise; 2: QCW </param>
        public void TraceWeldding(OperateLaser lsop, List<int[]> pts, double WeldSpeed,
            double moveSpeed, double synAcc, int addr, int mode = 0)
        {
            if (mode == 0)
            {
                WeldProcess weldProcess = new();
                weldProcess.Setup(devHandle, pcrdHandle, axisHandle, WeldSpeed, moveSpeed, synAcc);
                weldProcess.SetWeldParameter(lsop, pts);
                weldProcess.WeldThread(addr);
            }
            else if (mode == 1)
            {
                // rise and Fall
            }
            else if (mode == 2)
            {
                // QCW
            }
        }

        public void WeldInCard(OperateLaser lsop, List<int[]> ptlist, int addr)
        {
            TraceWeldding(lsop, ptlist, WeldSpeed, moveSpeed, synAcc, addr, 0);
        }

        public void LeapFrog(int[] pts)
        {
            Flyover flyover = new(pts);
            flyover.Setup(devHandle, pcrdHandle, axisHandle, moveSpeed, synAcc);
            flyover.Run();
        }

        public void SetupMoveParameter(double WeldSpeed, double moveSpeed)
        {
            this.WeldSpeed = WeldSpeed;
            this.moveSpeed = moveSpeed;
        }

        public void ReadIo(out int signal)
        {
            NMC_GetDIGroup(devHandle, out signal, 0);
        }

        public void ReadPosition(ref int[] pPosArray)
        {
            for (short i = 0; i < 3; i++)
            {
                if (axisHandle != null)
                    NMC_MtGetPrfPos(axisHandle[i], ref pPosArray[i]);
            }
        }

        public void SelfResetBit(int bitAddress)
        {
            NMC_SetDOBit(devHandle, (short)bitAddress, 0);
            Thread.Sleep(20);
            NMC_SetDOBit(devHandle, (short)bitAddress, 1);
        }

        /// <summary>
        /// open/close the air
        /// </summary>
        /// <param name="mode"> 1: open; 0: close </param>
        public void AirOnOff(int mode)
        {
            if (mode == 1)
            {
                NMC_SetDOBit(devHandle, 8, 0);
            }
            else
            {
                NMC_SetDOBit(devHandle, 8, 1);
            }
        }

        public void ResetThread()
        {
            mut.WaitOne(-1);
            if (ResetZaxis() != 0) return;
            if (ResetYaxis() != 0) return;
            if (ResetXaxis() != 0) return;
            mut.ReleaseMutex();
        }

        //
        // Reset X axis
        //
        private int ResetXaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = 0,
                reScanEn = 0,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 50.000,
                scan2ndVel = 5.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[0], ref homeSetting);
            short rtn = NMC_MtHome(axisHandle[0]);
            //Assertion.AssertError("X轴复位错误", rtn);

            short homeSts = 0;
            while (true)
            {
                NMC_MtGetHomeSts(axisHandle[0], ref homeSts);
                if ((homeSts & BIT_AXHOME_OK) != 0)
                {
                    Thread.Sleep(100);
                    return 0;
                }

                if (((homeSts & BIT_AXHOME_FAIL) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_MV) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_SWT) != 0))
                {
                    Thread.Sleep(100);
                    return 1;
                }
            }
        }

        //
        // Reset Y axis
        //
        private int ResetYaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = 1,
                reScanEn = 0,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 50.000,
                scan2ndVel = 5.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[1], ref homeSetting);
            short rtn = NMC_MtHome(axisHandle[1]);
            //Assertion.AssertError("Y轴复位错误", rtn);

            short homeSts = 0;
            while (true)
            {
                NMC_MtGetHomeSts(axisHandle[1], ref homeSts);
                if ((homeSts & BIT_AXHOME_OK) != 0)
                {
                    Thread.Sleep(100);
                    return 0;
                }

                if (((homeSts & BIT_AXHOME_FAIL) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_MV) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_SWT) != 0))
                {
                    Thread.Sleep(100);
                    return 1;
                }
            }
        }

        //
        // Reset Z axis
        //
        private int ResetZaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = 1,
                reScanEn = 0,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 50.000,
                scan2ndVel = 5.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[2], ref homeSetting);

            short rtn = NMC_MtHome(axisHandle[2]);
            //Assertion.AssertError("Z轴复位错误", rtn);

            short homeSts = 0;
            while (true)
            {
                NMC_MtGetHomeSts(axisHandle[2], ref homeSts);
                if ((homeSts & BIT_AXHOME_OK) != 0)
                {
                    Thread.Sleep(100);
                    return 0;
                }

                if (((homeSts & BIT_AXHOME_FAIL) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_MV) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_SWT) != 0))
                {
                    Thread.Sleep(100);
                    return 1;
                }
            }
        }

        public void GotoPosition(int[] ptxyz)
        {
            this.ptxyz = ptxyz;
            Thread gThread = new(GotoThread)
            {
                IsBackground = true,
            };
            gThread.Start();
        }

        private void GotoThread()
        {
            if (ptxyz == null) return;

            // from the first prepare point to the first point
            NMC_CrdLineXYZEx(pcrdHandle, 0, 0x07, ptxyz, 0, 50, 0.5, 0);
            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Assertion.AssertError("单点运行错误", rtn);

            short crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0)
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Weld process
    /// </summary>
    class WeldProcess : MotionCard
    {
        OperateLaser? lsop;
        List<int[]>? pts;

        public WeldProcess() { }

        public void Setup(ushort devHandle, ushort pcrdHandle, ushort[] axisHandle,
            double WeldSpeed, double moveSpeed, double synAcc)
        {
            this.devHandle = devHandle;
            this.pcrdHandle = pcrdHandle;
            this.axisHandle = axisHandle;
            this.WeldSpeed = WeldSpeed;
            this.moveSpeed = moveSpeed;
            this.synAcc = synAcc;
        }

        public void SetWeldParameter(OperateLaser lsop, List<int[]> pts)
        {
            this.lsop = lsop;
            this.pts = pts;
        }

        public void WeldThread(int addr)
        {
            if (lsop == null || pts == null) { return; }

            short crdState;
            int segNo = 0;

            mut.WaitOne(-1);
            AirOnOff(1);
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            // from the first prepare point to the first point
            NMC_CrdLineXYZEx(pcrdHandle, segNo, 0x07, pts[0], 0, moveSpeed, synAcc, 1);

            NMC_CrdEndMtn(pcrdHandle);
            NMC_CrdStartMtn(pcrdHandle);

            crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0)
                {
                    break;
                }
            }

            lsop.LaserSwitchOn();
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            NMC_CrdLineXYZEx(pcrdHandle, ++segNo, 0x07, pts[0], 0, moveSpeed, synAcc, 0);
            int count = pts.Count;
            for (int i = 1; i < count; i += 2)
            {
                NMC_CrdArc3DEx(pcrdHandle, ++segNo, pts[i], pts[i+1], 0, WeldSpeed, synAcc, 0);
            }

            // open the protect air before lasing with the air time

            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            // Assertion.AssertError("运行错误", rtn);
            Promption.Prompt("焊接坐标数值错误，请先复位", rtn);

            crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0)
                {
                    break;
                }
            }

            lsop.LaserSwitchOff();
            Thread.Sleep(50);
            SelfResetBit(addr);
            mut.ReleaseMutex();
        }


    }

    class Flyover : MotionCard
    {
        int[] pts;

        public Flyover(int[] pts)
        {
            this.pts = pts;
        }

        public void Setup(ushort devHandle, ushort pcrdHandle, ushort[] axisHandle,
            double moveSpeed, double synAcc)
        {
            this.devHandle = devHandle;
            this.pcrdHandle = pcrdHandle;
            this.axisHandle = axisHandle;
            this.moveSpeed = moveSpeed;
            this.synAcc = synAcc;
        }

        public void Run()
        {

            mut.WaitOne(-1);
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            // from the first prepare point to the first point
            NMC_CrdLineXYZEx(pcrdHandle, 0, 0x07, pts, 0, moveSpeed, synAcc, 0);
            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Assertion.AssertError("运行错误", rtn);
            //Promption.Prompt("飞行坐标数值错误，请先复位", rtn);

            short crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0) { break; }
            }
            mut.ReleaseMutex();
        }
    }

    /// <summary>
    /// The KeyMap for HandWheel
    /// </summary>
    public enum KeyMap
    {
        Xaxis = 0,
        Yaxis = 1,
        Zaxis = 2,
        RX1 = 1,
        RX10 = 3,
        RX100 = 5,
    }

    /// <summary>
    /// Input port map
    /// </summary>
    public enum ComBit
    {
        Xaxis = 0x40_0000,     // BIT 22
        Yaxis = 0x100_0000,    // BIT 24
        Zaxis = 0x400_0000,    // BIT 26
        RX1 = 0x80_0000,
        RX10 = 0x200_0000,
        RX100 = 0x800_0000,
        Air = 0x100,    // BIT 8
        Bit12 = 0x1000,
        Bit13 = 0x2000,
        Bit14 = 0x4000,
        Bit15 = 0x8000,
        Reset = 0x7FFF_FFFF,    // reset all bits

        K1 = 0X1_0000,
        K2 = 0X2_0000,
        K3 = 0X4_0000,
    }

    //
    // Delegate function to operate the main window
    //
    public delegate void FlipLineTypeDelegate();
    public delegate void FlipLaserOnOffDelegate();
    public delegate void AddPointDelegate(Vector vector);
    public delegate void SetXYZDelegate(int x, int y, int z);
    public delegate void AddPointMsgDelegate(int x, int y, int z);
}
