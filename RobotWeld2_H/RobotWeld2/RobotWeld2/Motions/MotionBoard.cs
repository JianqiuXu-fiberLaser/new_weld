using Microsoft.VisualBasic;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// The Lowest layer in our five-layers model.
    /// The drivers for motion hardware, including three parts:
    /// (1) the fundamental moving action
    /// (2) the laser function
    /// (3) the hand wheel
    /// </summary>
    public class MotionBoard
    {
        //
        // Fundamental Handle for the motion card and the coordinate system
        //
        public static ushort devHandle;
        public static ushort pcrdHandle;
        public static ushort[] axisHandle = new ushort[4];

        public MotionBoard() { }

        public void InitialCard()
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
            if (rtn != 0)
            {
                DaemonFile.AddInfo("打开运动卡错误\n");
            }

            if (rtn == 0)
            {
                NMC_DevReset(devHandle);
                rtn = NMC_LoadConfigFromFile(devHandle, Encoding.Default.GetBytes("./GCN400.cfg"));
                Assertion.AssertError("装载运动卡配置失败", rtn);

                for (short i = 0; i < 4; i++)
                {
                    NMC_MtOpen(devHandle, i, ref axisHandle[i]);
                    NMC_MtClrError(axisHandle[i]);
                    NMC_MtZeroPos(axisHandle[i]);
                    NMC_MtSetSvOn(axisHandle[i]);
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
            NMC_CrdOpen(devHandle, ref pcrdHandle);

            // configure the coordinate
            TCrdConfig crd = new(true)
            {
                axCnts = 4
            };
            crd.pAxArray[0] = 0;
            crd.pAxArray[1] = 1;
            crd.pAxArray[2] = 2;
            crd.pAxArray[3] = 3;
            crd.port[0] = 0;
            crd.port[1] = 0;
            crd.port[2] = 0;
            crd.port[3] = 0;
            NMC_CrdConfig(pcrdHandle, ref crd);

            // configure moving parameters
            TCrdPara crdPara = new(true)
            {
                orgFlag = 1,
                synAccMax = 50,
                synVelMax = 200
            };
            crdPara.offset[0] = 0;
            crdPara.offset[1] = 0;
            crdPara.offset[2] = 0;
            crdPara.offset[3] = 0;
            NMC_CrdSetPara(pcrdHandle, ref crdPara);

            // clear the command buffer
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);
        }

        public void ResetAll()
        {
            if (ResetZaxis() != 0)
            {
                return;
            }

            if (ResetYaxis() != 0)
            {
                return;
            }
            if (ResetXaxis() != 0) return;
        }

        //
        // Reset X axis
        //
        public int ResetXaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = 0,
                reScanEn = 1,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 30.000,
                scan2ndVel = 3.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[0], ref homeSetting);
            NMC_MtHome(axisHandle[0]);

            short homeSts = 0;
            while (true)
            {
                NMC_MtGetHomeSts(axisHandle[0], ref homeSts);
                if ((homeSts & BIT_AXHOME_OK) != 0)
                {
                    return 0;
                }

                if (((homeSts & BIT_AXHOME_FAIL) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_MV) != 0) ||
                    ((homeSts & BIT_AXHOME_ERR_SWT) != 0))
                {
                    return 1;
                }
                Thread.Sleep(10);
            }
        }

        //
        // Reset Y axis
        //
        public int ResetYaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = 1,
                reScanEn = 1,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 30.000,
                scan2ndVel = 3.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[1], ref homeSetting);
            NMC_MtHome(axisHandle[1]);

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
                    return 1;
                }
                Thread.Sleep(10);
            }
        }

        //
        // Reset Z axis
        //
        public int ResetZaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = 1,
                reScanEn = 1,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 30.000,
                scan2ndVel = 3.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[2], ref homeSetting);
            NMC_MtHome(axisHandle[2]);

            short homeSts = 0;
            while (true)
            {
                NMC_MtGetHomeSts(axisHandle[2], ref homeSts);
                if ((homeSts & BIT_AXHOME_OK) != 0)
                {
                    return 0;
                }

                if (((homeSts & BIT_AXHOME_FAIL) != 0) || ((homeSts & BIT_AXHOME_ERR_MV) != 0) || ((homeSts & BIT_AXHOME_ERR_SWT) != 0))
                {
                    return 1;
                }
                Thread.Sleep(10);
            }
        }

        //
        // Reset A axis
        //
        public void ResetAaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = 1,
                reScanEn = 1,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 8.000,
                scan2ndVel = 0.800,
                offset = 0,
                acc = 0.0200,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[3], ref homeSetting);
            NMC_MtHome(axisHandle[3]);

            short homeSts = 0;
            while (true)
            {
                NMC_MtGetHomeSts(axisHandle[3], ref homeSts);
                if ((homeSts & BIT_AXHOME_OK) != 0)
                {
                    return;
                }

                if (((homeSts & BIT_AXHOME_FAIL) != 0) || ((homeSts & BIT_AXHOME_ERR_MV) != 0) || ((homeSts & BIT_AXHOME_ERR_SWT) != 0))
                {
                    return;
                }
                Thread.Sleep(10);
            }
        }

        public bool CheckAxisRun()
        {
            short crdState = 0;
            NMC_CrdGetSts(pcrdHandle, ref crdState);
            if ((crdState & BIT_AXIS_BUSY) == 0)
                return false;
            else
                return true;
        }

        public void FlyMove(int[] pts, double moveSpeed, double synAcc)
        {
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            // from the first prepare point to the first point
            int[] rpts = new int[3] { pts[0], -pts[1], -pts[2] };
            NMC_CrdLineXYZEx(pcrdHandle, 0, 0x07, rpts, 0, moveSpeed, synAcc, 0);
            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Assertion.AssertError("飞行坐标数值错误", rtn);

            // when coordinate has an error, it need to be reset all process.
            if (rtn != 0)
            {
                LaserDrives.LaserOff();
                MotionOperate.StopAllThread();
                return;
            }

            short crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0) { break; }
            }
        }

        public void FlyRotateMove(int[] pts, int Apts, double moveSpeed, double synAcc)
        {
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            // from the first prepare point to the first point
            int[] rpts = new int[4] { pts[0], -pts[1], -pts[2], Apts };
            NMC_CrdLineXYZA(pcrdHandle, 0, 0x0F, rpts, 0, moveSpeed, synAcc, 0);
            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Assertion.AssertError("飞行旋转达到错误", rtn);

            // when coordinate has an error, it need to be reset all process.
            if (rtn != 0)
            {
                LaserDrives.LaserOff();
                MotionOperate.StopAllThread();
                return;
            }

            short crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0) { break; }
            }
        }


        public void WeldMove(List<int[]> ptsList, double WeldSpeed, double synAcc)
        {
            int segNo = 0;
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            int[] rpts = new int[3] { ptsList[0][0], -ptsList[0][1], -ptsList[0][2] };
            NMC_CrdLineXYZEx(pcrdHandle, ++segNo, 0x07, rpts, 0, WeldSpeed, synAcc, 0);

            // NOTE: here the count must be odd number.
            int count = ptsList.Count;
            int[] rpts1;
            int[] rpts2;
            for (int i = 1; i < count - 1; i += 2)
            {
                rpts1 = new int[3] { ptsList[i][0], -ptsList[i][1], -ptsList[i][2] };
                rpts2 = new int[3] { ptsList[i + 1][0], -ptsList[i + 1][1], -ptsList[i + 1][2] };
                NMC_CrdArc3DEx(pcrdHandle, ++segNo, rpts1, rpts2, 0, WeldSpeed, synAcc, 0);
            }

            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Promption.Prompt("焊接坐标数值错误", rtn);

            // when coordinate has an error, it need to be reset all process.
            if (rtn != 0)
            {
                LaserDrives.LaserOff();
                MotionOperate.StopAllThread();
                return;
            }

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

        public void RotateMove(List<int[]> pts, List<int> Apts, double WeldSpeed, List<double> CofWeld, double synAcc = 0.1)
        {
            int segNo = 0;
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            int count = pts.Count;
            for (int i = 0; i < count; i++)
            {
                int[] tgPos = new int[4] { pts[i][0], -pts[i][1], -pts[i][2], Apts[i] };
                NMC_CrdLineXYZA(pcrdHandle, ++segNo, 0x0F, tgPos, 0, 0.3 * WeldSpeed * CofWeld[i], synAcc, 0);
            }

            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Promption.Prompt("旋转焊接数值错误", rtn);

            // when coordinate has an error, it need to be reset all process.
            if (rtn != 0)
            {
                LaserDrives.LaserOff();
                MotionOperate.StopAllThread();
                return;
            }

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

        public void LeapRotateMove(int[] pts, double moveSpeed, double synAcc)
        {
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            // from the first prepare point to the first point
            int[] rpts = new int[4] { pts[0], -pts[1], -pts[2], pts[3] };
            NMC_CrdLineXYZA(pcrdHandle, 0, 0x0F, rpts, 0, moveSpeed, synAcc, 0);
            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Assertion.AssertError("飞行旋转坐标错误", rtn);

            // when coordinate has an error, it need to be reset all process.
            if (rtn != 0)
            {
                LaserDrives.LaserOff();
                MotionOperate.StopAllThread();
                return;
            }

            short crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0) { break; }
            }
        }

        /// <summary>
        /// This function should used only for A axis, otherwise there has the coordinate error.
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="moveSpeed"></param>
        /// <param name="synAcc"></param>
        public void LeapRotate(int pts, double moveSpeed, double synAcc = 0.5)
        {
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            short rtn = NMC_MtMovePtpAbs(axisHandle[3], synAcc, synAcc, 0, 0, moveSpeed, 0, pts);
            Promption.Prompt("旋转运动错误", rtn);
        }

        public void SelfResetBit(int bitAddress)
        {
            NMC_SetDOBit(devHandle, (short)bitAddress, 0);
            Thread.Sleep(20);
            NMC_SetDOBit(devHandle, (short)bitAddress, 1);
        }

        /// <summary>
        /// Read all Input ports
        /// </summary>
        /// <param name="retinfo">out: the bool array for IO information of Input </param>
        /// <returns> all ports are lower level, return false </returns>
        public bool ReadIoGroup(out bool[] retinfo)
        {
            bool[] varArray = new bool[32];
            NMC_GetDIGroup(devHandle, out int signal, 0);
            if (signal == 0x7FFF_FFFF)
            {
                retinfo = varArray;
                return false;
            }

            int mask = 0x01;
            for (int i = 0; i < 32; i++)
            {
                if ((signal & mask) == 0)
                {
                    varArray[i] = true;
                }
                else
                {
                    varArray[i] = false;
                }
                mask <<= 1;
            }

            retinfo = varArray;
            return true;
        }

        /// <summary>
        /// Read the level at bit address
        /// </summary>
        /// <param name="bitAddress"></param>
        /// <returns> true: lower leve</returns>
        public bool ReadIoBit(int bitAddress)
        {
            short retbit = 0;
            NMC_GetDIBit(devHandle, (short)bitAddress, ref retbit);
            if (retbit == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// set the bit value
        /// </summary>
        /// <param name="bitAddress"> bit address </param>
        /// <param name="inValue"> the value of bit </param>
        public void SetBit(int bitAddress, bool inValue)
        {
            if (inValue)
                NMC_SetDOBit(devHandle, (short)bitAddress, 1);
            else
                NMC_SetDOBit(devHandle, (short)bitAddress, 0);
        }

        /// <summary>
        /// the task to read coordinate
        /// </summary>
        public void ReadCoordinate(out int[] pos)
        {
            int[] retin = new int[4];
            int[] rretin = new int[4];
            for (short i = 0; i < 4; i++)
            {
                NMC_MtGetPrfPos(axisHandle[i], ref retin[i]);
            }

            rretin[0] = retin[0];
            rretin[1] = -retin[1];
            rretin[2] = -retin[2];
            rretin[3] = retin[3];

            pos = rretin;
        }
    }

    /// <summary>
    /// CLASS: Drive for handwheel
    /// </summary>
    public class HandWheel : MotionBoard
    {
        private readonly short _followRatio = 50;
        private readonly double _acceSpeed = 0.1;
        private readonly double _followSpeed = 2000;

        public HandWheel() { }

        /// <summary>
        /// Configurate the Hand Wheel
        /// </summary>
        public void InitialHandwheel()
        {
            NMC_ClrHandWheel(devHandle);
            short rtn = NMC_SetHandWheelInput(devHandle, 256);
            if (rtn != 0)
                DaemonFile.AddInfo("设置手轮错误\n");
            else
                DaemonFile.AddInfo("设置手轮成功\n");

            NMC_SetHandWheel(devHandle, 0, _followRatio, _acceSpeed, _followSpeed);
        }

        public void Exit()
        {
            NMC_ClrHandWheel(devHandle);
        }

        /// <summary>
        /// Change the follow ratio for hand wheel
        /// </summary>
        /// <param name="axisIndex"> the axis to be set </param>
        /// <param name="ratio"> the following ratio </param>
        public void SetHandWheel(short axisIndex, int Cof)
        {
            double ratio = Cof * _followRatio;
            if (axisIndex == 3)
            {
                ratio *= 0.5;
            }

            NMC_ClrHandWheel(devHandle);
            NMC_SetHandWheel(devHandle, axisIndex, ratio, _acceSpeed, _followSpeed);
        }
    }


    /// <summary>
    /// CLASS: the drive for laser
    /// </summary>
    public class LaserDrives : MotionBoard
    {
        //
        // Constants
        //
        private const int POWER_CODE = 32767;
        private const int LASER_CHANNEL = 0;


        public LaserDrives() { }

        /// <summary>
        /// Setup the laser working mode
        /// </summary>
        /// <param name="mode">0: CW; 1: QCW; 2: follow</param>
        /// <param name="maxPower"> Maximum Power </param>
        public void InitialLaser(int mode, int frequency, int duty)
        {
            if (mode == 2)
            {
                NMC_LaserSetMode(devHandle, TIME_ARRAY_OUTPUT_MODE, LASER_CHANNEL);
            }
            else if (mode == 1)
            {
                // QCW mode
                // NMC_LaserSetMode(devHandle, SHIO_OUTPUT_MODE, LASER_CHANNEL);
                NMC_SetDacMode(devHandle, 256, 1);
                NMC_LaserSetMode(devHandle, BASIC_OUTPUT_MODE, 0);
                NMC_LaserSetOutputType(devHandle, LASER_PWM_DUTY, 0, frequency, 0);
                NMC_LaserSetPowerEx(devHandle, duty, 0);
            }
            else
            {
                NMC_SetDacMode(devHandle, 256, 1);
                NMC_LaserSetMode(devHandle, BASIC_OUTPUT_MODE, 0);
                NMC_LaserSetOutputType(devHandle, LASER_PWM_DUTY, 0, 1000, 0);
                NMC_LaserSetPowerEx(devHandle, 100, 0);
            }
        }

        public bool LaserOn(double powerPercent)
        {
            short pcode = (short)(powerPercent * POWER_CODE);
            NMC_LaserOnOff(devHandle, 1, 0);
            NMC_SetDac(devHandle, 256, pcode);
            return true;
        }

        public static void LaserOff()
        {
            NMC_SetDac(devHandle, 256, 0);
            NMC_LaserOnOff(devHandle, 0, 0);
        }

        /// <summary>
        /// the time pass in ms, used for slow rising and slow falling
        /// </summary>
        /// <returns> time span passed from the start point</returns>
        public static long PassTime()
        {
            TimeSpan tSpan = DateTime.Now - new DateTime(2023, 4, 1, 0, 0, 0);
            return tSpan.Milliseconds;
        }
    }
}
