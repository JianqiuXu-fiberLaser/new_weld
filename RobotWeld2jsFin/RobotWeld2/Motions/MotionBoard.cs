using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
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
        protected static ushort pcrdHandle;
        protected readonly static ushort[] axisHandle = new ushort[4];
        private int _pSegNo;

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
        private static bool ConfigCard()
        {
            // open motion card and load the configure file.
            short rtn = NMC_DevOpen(0, ref devHandle);
            if (rtn != 0)
            {
                MainModel.AddInfo("打开运动卡错误\n");
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
        private static void ConfigCoordinate()
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

        //-------------------------------------------------------------
        // Reset the Axes
        //-------------------------------------------------------------

        public void ResetAll()
        {
            if (ResetZaxis() != 0) { return; }
            if (ResetYaxis() != 0) { return; }
            if (ResetXaxis() != 0) { return; }
        }

        //
        // Reset X axis
        //
        public int ResetXaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = (short)MotionSpecification.XHomeDir,
                reScanEn = 1,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 40.000,
                scan2ndVel = 3.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[0], ref homeSetting);
            NMC_MtSwLmtOnOff(axisHandle[0], 0);
            NMC_MtHome(axisHandle[0]);

            int result = CheckResetAxes(0);
            return result;
        }

        //
        // Reset Y axis
        //
        public int ResetYaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = (short)MotionSpecification.YHomeDir,
                reScanEn = 1,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 40.000,
                scan2ndVel = 3.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[1], ref homeSetting);
            NMC_MtSwLmtOnOff(axisHandle[1], 0);
            NMC_MtHome(axisHandle[1]);

            int result = CheckResetAxes(1);
            return result;
        }

        //
        // Reset Z axis
        //
        public int ResetZaxis()
        {
            THomeSetting homeSetting = new()
            {
                mode = (short)THomeMode.HM_MODE1,
                dir = (short)MotionSpecification.ZHomeDir,
                reScanEn = 1,
                zEdge = 0,
                homeEdge = 0,
                lmtEdge = 0,
                usePreSetPtpPara = 0,
                safeLen = 0,
                scan1stVel = 40.000,
                scan2ndVel = 3.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[2], ref homeSetting);
            NMC_MtSwLmtOnOff(axisHandle[2], 0);
            NMC_MtHome(axisHandle[2]);

            int result = CheckResetAxes(2);
            return result;
        }

        //
        // Reset A axis
        //
        public void ResetAaxis()
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
                scan1stVel = 8.000,
                scan2ndVel = 0.800,
                offset = 0,
                acc = 0.0200,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[3], ref homeSetting);
            NMC_MtHome(axisHandle[3]);

            int _ = CheckResetAxes(3);
            return;
        }

        //-------------------------------------------------------------
        // The Initialization of Machine.
        //-------------------------------------------------------------

        /// <summary>
        /// This function only used in initializing the machine, where the homeback direction is not known.
        /// </summary>
        /// <param name="iAxis"></param>
        /// <returns></returns>
        public int FindOriginal(int iAxis)
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
                scan1stVel = 10.000,
                scan2ndVel = 1.000,
                offset = 0,
                acc = 0.100,
                iniRetPos = 0,
                retSwOffset = 0,
            };

            NMC_MtSetHomePara(axisHandle[iAxis], ref homeSetting);
            NMC_MtSwLmtOnOff(axisHandle[iAxis], 0);
            NMC_MtHome(axisHandle[iAxis]);

            CheckResetAxes(iAxis);

            return 0;
        }

        private static int CheckResetAxes(int iAxis)
        {
            short homeSts = 0;
            while (true)
            {
                NMC_MtGetHomeSts(axisHandle[iAxis], ref homeSts);
                if ((homeSts & BIT_AXHOME_OK) != 0)
                {
                    NMC_MtSwLmtOnOff(axisHandle[1], 1);
                    return 0;
                }

                if ((homeSts & BIT_AXHOME_FAIL) != 0)
                {
                    NMC_MtSwLmtOnOff(axisHandle[1], 1);
                    return 1;
                }

                if ((homeSts & BIT_AXHOME_ERR_MV) != 0)
                {
                    NMC_MtSwLmtOnOff(axisHandle[1], 1);
                    return 2;
                }

                if ((homeSts & BIT_AXHOME_ERR_SWT) != 0)
                {
                    NMC_MtSwLmtOnOff(axisHandle[1], 1);
                    return 3;
                }
                Thread.Sleep(50);
            }
        }

        private double acc;
        private double dcc;
        private double smoothCoef;
        public void SetupJog(int iAxis, double acc = 0.1, double dcc = 0.1, double smoothCoef = 10)
        {
            this.acc = acc;
            this.dcc = dcc;
            this.smoothCoef = smoothCoef;

            TJogPara tjogPara = new()
            {
                acc = this.acc,
                dec = this.dcc,
                smoothCoef = this.smoothCoef
            };

            NMC_MtSetJogPara(axisHandle[iAxis], ref tjogPara);
        }

        public void JogMove(int iAxis, double mSpeed)
        {
            NMC_MtMoveJog(axisHandle[iAxis], this.acc, this.dcc, mSpeed, (short)this.smoothCoef, 0);
        }

        public void JogStop(int iAxis)
        {
            NMC_MtStop(axisHandle[iAxis]);
        }

        /// <summary>
        /// Check axis state success
        /// </summary>
        /// <param name="iAxis"> the axis number </param>
        /// <returns> 1: positive limit; 2: negative limit; 3: errors; 0: nothing happen </returns>
        public int CheckAxisState(int iAxis)
        {
            short pStsWord = 0;
            NMC_MtGetSts(axisHandle[iAxis], ref pStsWord);

            if ((pStsWord & BIT_AXIS_LMTP) != 0)
            {
                return 1;
            }
            else if ((pStsWord & BIT_AXIS_LMTN) != 0)
            {
                return 2;
            }
            else if ((pStsWord & BIT_AXIS_MVERR) != 0 || (pStsWord & BIT_AXIS_ALM) != 0 || (pStsWord & BIT_AXIS_POSERR) != 0 || (pStsWord & BIT_AXIS_HWERR) != 0)
            {
                return 3;
            }

            return 0;
        }

        /// <summary>
        /// Release the hard limit of axis
        /// </summary>
        /// <param name="iAxis"> axis number </param>
        /// <param name="iDirection"> 0: positive; 1: negative; 2: both; </param>
        public void ReleaseLimit(int iAxis, int iDirection)
        {
            if (iDirection == 0)
            {
                NMC_MtSetLmtCfg(axisHandle[iAxis], 0, 1, 0, 0);
            }
            else if (iDirection == 1)
            {
                NMC_MtSetLmtCfg(axisHandle[iAxis], 1, 0, 0, 0);
            }
            else if (iDirection == 2)
            {
                NMC_MtSetLmtCfg(axisHandle[iAxis], 0, 0, 0, 0);
            }
            NMC_MtClrError(axisHandle[iAxis]);
        }

        /// <summary>
        /// Set hard limit of axis
        /// </summary>
        /// <param name="iAxis"> axis number </param>
        /// <param name="iDirection"> 0: positive; 1: negative; 2: both; </param>
        public void SetLimit(int iAxis, int iDirection)
        {
            if (iDirection == 0)
            {
                NMC_MtSetLmtCfg(axisHandle[iAxis], 1, 0, 0, 0);
            }
            else if (iDirection == 1)
            {
                NMC_MtSetLmtCfg(axisHandle[iAxis], 0, 1, 0, 0);
            }
            else if (iDirection == 2)
            {
                NMC_MtSetLmtCfg(axisHandle[iAxis], 1, 1, 0, 0);
            }
        }

        //-------------------------------------------------------------
        // Basic moving function
        //-------------------------------------------------------------

        /// <summary>
        /// 3D Circle move, data of list.
        /// </summary>
        /// <param name="ptsList"></param>
        /// <param name="WeldSpeed"></param>
        /// <param name="synAcc"></param>
        public void CircleMove(List<int[]> ptsList, List<double> WeldSpeed, double synAcc = 0.5, int startSeg = 0)
        {
            ClearBuffer();

            // NOTE: here the count must be odd number.
            for (int i = 0; i < ptsList.Count; i += 2)
            {
                int[] rpts1 = new int[3] { ptsList[i][0], ptsList[i][1], ptsList[i][2] };
                int[] rpts2 = new int[3] { ptsList[i + 1][0], ptsList[i + 1][1], ptsList[i + 1][2] };
                var speed = WeldSpeed[i];
                if (speed == 0) speed = 25;
                NMC_CrdArc3DEx(pcrdHandle, i + startSeg, rpts1, rpts2, 0, speed, synAcc, 0);
            }

            MoveCoordinateAxis();
        }

        /// <summary>
        /// 3D line move, data of list.
        /// </summary>
        /// <param name="ptsList"></param>
        /// <param name="WeldSpeed"></param>
        /// <param name="synAcc"></param>
        public void LineMove(List<int[]> ptsList, List<double> WeldSpeed, double synAcc = 0.5, int startSeg = 0)
        {
            ClearBuffer();
            for (int i = 0; i < ptsList.Count; i++)
            {
                int[] rpts = new int[3] { ptsList[i][0], ptsList[i][1], ptsList[i][2] };
                var speed = WeldSpeed[i];
                if (speed == 0) speed = 25;
                NMC_CrdLineXYZEx(pcrdHandle, i + startSeg, 0x07, rpts, 0, speed, synAcc, 0);
            }

            MoveCoordinateAxis();
        }

        /// <summary>
        /// Move to single point
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="Speed"></param>
        /// <param name="synAcc"></param>
        /// <param name="startSeg"></param>
        public void LineSingleMove(int[] pts, double Speed, double synAcc = 0.5, int startSeg = 0)
        {
            ClearBuffer();
            NMC_CrdLineXYZEx(pcrdHandle, startSeg, 0x07, pts, 0, Speed, synAcc, 0);
            MoveCoordinateAxis();
        }

        /// <summary>
        /// 4D line move, data of list
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="Speed"></param>
        /// <param name="synAcc"></param>
        public void RotateMove(List<int[]> pts, List<double> Speed, double synAcc = 0.5, int startSeg = 0)
        {
            ClearBuffer();
            if (Speed.Count < pts.Count)
            {
                MainModel.AddInfo("The speed list is less than pts.");
                return;
            }

            for (int i = 0; i < pts.Count; i++)
            {
                int[] tgPos = new int[4] { pts[i][0], pts[i][1], pts[i][2], pts[i][3] };
                double speed = Speed[i];
                if (speed == 0) speed = 25;
                NMC_CrdLineXYZA(pcrdHandle, i + startSeg, 0x0F, tgPos, 0, speed, synAcc, 0);
            }

            MoveCoordinateAxis();
        }

        private static void ClearBuffer()
        {
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);
        }

        private bool CheckAxesMove()
        {
            short crdState = 0;
            while (true)
            {
                // when the moving is stop, jump the loop
                NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & BIT_AXIS_BUSY) == 0)
                {
                    return false;
                }
                else
                {
                    NMC_CrdGetUserSegNo(pcrdHandle, ref _pSegNo);
                    Thread.Sleep(10);
                }
            }
        }

        public int GetMoveSegmentNumber()
        {
            return _pSegNo;
        }

        private void MoveCoordinateAxis()
        {
            NMC_CrdEndMtn(pcrdHandle);
            short rtn = NMC_CrdStartMtn(pcrdHandle);
            Promption.Prompt("运动轨迹数值错误", rtn);

            // when coordinate has an error, it need to be reset all process.
            if (rtn != 0)
            {
                LaserDrives.LaserOff();
                MotionOperate.StopAllThread();
                return;
            }

            CheckAxesMove();
        }

        //-------------------------------------------------------------
        // IO operation
        //-------------------------------------------------------------

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
            for (short i = 0; i < 4; i++)
            {
                NMC_MtGetPrfPos(axisHandle[i], ref retin[i]);
            }

            pos = retin;
        }

        /// <summary>
        /// Read coordinate of single axis
        /// </summary>
        /// <param name="axisNum"> the number of axis </param>
        /// <returns> the value of coordinate  </returns>
        public int ReadAxisCoordinate(int axisNum)
        {
            int ret = 0;
            NMC_MtGetPrfPos(axisHandle[axisNum], ref ret);
            return ret;
        }
    }




}
