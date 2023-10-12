///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        protected static ushort devHandle;
        private static ushort pcrdHandle;
        private static readonly ushort[] axisHandle = new ushort[4];
        private static int pSegNo;

        public MotionBoard() { }

        public static void InitialCard()
        {
            if (ConfigCard()) ConfigCoordinate();
        }

        //
        // Configure the motion card
        //
        private static bool ConfigCard()
        {
            // open motion card and load the configure file.
            short rtn = NMC_DevOpen(0, ref devHandle);
            if (rtn != 0) DaemonFile.AddInfo("打开运动卡错误\n");

            if (rtn == 0)
            {
                NMC_DevReset(devHandle);
                rtn = NMC_LoadConfigFromFile(devHandle, Encoding.Default.GetBytes("./GCN400.cfg"));
                Assertion.AssertError("装载运动卡配置失败", rtn);

                for (short i = 0; i < 3; i++)
                {
                    NMC_MtOpen(devHandle, i, ref axisHandle[i]);
                    NMC_MtClrError(axisHandle[i]);
                    NMC_MtSetSvOn(axisHandle[i]);
                }

                NMC_SetCmdDebug(1, "cmdBug.log");
                return true;
            }
            else
                return false;
        }

        //
        // Configure the coordinate
        //
        private static void ConfigCoordinate()
        {
            // open the coordinate
            NMC_CrdOpen(devHandle, ref pcrdHandle);

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
            NMC_CrdConfig(pcrdHandle, ref crd);

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
            NMC_CrdSetPara(pcrdHandle, ref crdPara);

            // clear the command buffer
            NMC_CrdBufClr(pcrdHandle);
        }

        public static void ResetAll()
        {
            if (ResetZaxis() != 0) return;
            if (ResetYaxis() != 0) return;
            if (ResetXaxis() != 0) return;
        }

        //
        // Reset X axis
        //
        public static int ResetXaxis()
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
                scan1stVel = 50.000,
                scan2ndVel = 5.000,
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
        public static int ResetYaxis()
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
                scan1stVel = 50.000,
                scan2ndVel = 5.000,
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
        public static int ResetZaxis()
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
                scan1stVel = 50.000,
                scan2ndVel = 5.000,
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

        public static bool CheckAxisRun()
        {
            short crdState = 0;
            NMC_CrdGetSts(pcrdHandle, ref crdState);
            if ((crdState & BIT_AXIS_BUSY) == 0)
                return false;
            else
                return true;
        }

        public static void FlyMove(int[] pts, double moveSpeed, double synAcc)
        {
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);
            
            pSegNo = 0;
            // from the first prepare point to the first point
            NMC_CrdLineXYZEx(pcrdHandle, pSegNo, 0x07, pts, 0, moveSpeed, synAcc, 0);
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
                if ((crdState & BIT_AXIS_BUSY) == 0) break;
            }
        }

        /// <summary>
        /// Weld the arc trace through a list of points.
        /// </summary>
        /// <param name="ptsList"> point list </param>
        /// <param name="WeldSpeed"></param>
        /// <param name="synAcc"></param>
        public static void WeldMove(List<int[]> ptsList, double WeldSpeed, double synAcc)
        {
            int segNo = 1;
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            NMC_CrdLineXYZEx(pcrdHandle, ++segNo, 0x07, ptsList[0], 0, WeldSpeed, synAcc, 0);

            // NOTE: here the count must be odd number.
            int count = ptsList.Count;
            for (int i = 1; i < count - 1; i += 2)
                NMC_CrdArc3DEx(pcrdHandle, ++segNo, ptsList[i], ptsList[i + 1], 0, WeldSpeed, synAcc, 0);

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
                if ((crdState & BIT_AXIS_BUSY) == 0) break;
            }
        }

        /// <summary>
        /// Weld through a list of points, using memory package commands.
        /// </summary>
        /// <param name="ptsList"> point list </param>
        /// <param name="WeldSpeed"></param>
        /// <param name="synAcc"></param>
        public static void WeldMove2(List<int[]> ptsList, double WeldSpeed, double synAcc)
        {
            int segNo = 2;
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);

            int count = ptsList.Count;
            for (int i = 0; i < count; i++)
            {
                NMC_CrdLineXYZEx(pcrdHandle, segNo, 0x07, ptsList[i], 0, WeldSpeed, synAcc, 0);
                segNo++;
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
                NMC_CrdGetUserSegNo(pcrdHandle, ref pSegNo);
                NMC_CrdGetSts(pcrdHandle, ref crdState);

                // when the moving is stop, jump the loop
                if ((crdState & BIT_AXIS_BUSY) == 0) break;
            }
        }

        public static int GetSegNo()
        {
            return pSegNo;
        }

        /// <summary>
        /// Set an output port, then reset it after 20 ms
        /// </summary>
        /// <param name="bitAddress"> address of the output port </param>
        public static void SelfResetBit(int bitAddress)
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
        public static bool ReadIoGroup(out bool[] retinfo)
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
                if ((signal & mask) == 0) varArray[i] = true;
                else varArray[i] = false;

                mask <<= 1;
            }

            retinfo = varArray;
            return true;
        }

        /// <summary>
        /// Read the level at bit address
        /// </summary>
        /// <param name="bitAddress"> the port address </param>
        /// <returns> true: lower leve</returns>
        public static bool ReadIoBit(int bitAddress)
        {
            short retbit = 0;
            NMC_GetDIBit(devHandle, (short)bitAddress, ref retbit);
            if (retbit == 1) return false;
            else return true;
        }

        /// <summary>
        /// set the bit value
        /// </summary>
        /// <param name="bitAddress"> bit address </param>
        /// <param name="inValue"> the value of bit </param>
        public static void SetBit(int bitAddress, bool inValue)
        {
            if (inValue)
                NMC_SetDOBit(devHandle, (short)bitAddress, 1);
            else
                NMC_SetDOBit(devHandle, (short)bitAddress, 0);
        }

        /// <summary>
        /// the task to read coordinate
        /// </summary>
        public static void ReadCoordinate(out int[] pos)
        {
            int[] retin = new int[3];
            for (short i = 0; i < 3; i++)
                NMC_MtGetPrfPos(axisHandle[i], ref retin[i]);

            pos = retin;
        }
    }
}
