///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.AppModel;
using System.Collections.Generic;
using System.Text;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// The motion borad driver when the A axis is not installed.
    /// </summary>
    public class MotionBoard3 : MotionBoard
    {
        public override void InitialCard()
        {
            if (ConfigCard())
            {
                ConfigCoordinate();
                ConfigPort();
            }
        }

        /// <summary>
        /// Configure the motion card
        /// </summary>
        /// <returns></returns>
        private static bool ConfigCard()
        {
            // open motion card and load the configure file.
            short rtn = NMC_DevOpen(0, ref devHandle);
            if (rtn != 0)
            {
                MainModel.AddInfo("打开运动卡错误");
            }

            if (rtn == 0)
            {
                NMC_DevReset(devHandle);
                rtn = NMC_LoadConfigFromFile(devHandle, Encoding.Default.GetBytes("./GCN400.cfg"));
                Assertion.AssertError("装载运动卡配置失败", rtn);

                for (short i = 0; i < 3; i++)
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
            crd.port[0] = 0;
            crd.port[1] = 0;
            crd.port[2] = 0;
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
            NMC_CrdSetPara(pcrdHandle, ref crdPara);

            // clear the command buffer
            NMC_CrdClrError(pcrdHandle);
            NMC_CrdBufClr(pcrdHandle);
        }

        //-------------------------------------------------------------
        // Reset the Axes, scaping the A axis
        //-------------------------------------------------------------

        //
        // Reset A axis
        //
        public override void ResetAaxis()
        {
            // do nothing
            // This method should be kept to scape reseting of A-axis
        }

        /// <summary>
        /// 4D line move, data of list
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="Speed"></param>
        /// <param name="synAcc"></param>
        public override void RotateMove(List<int[]> pts, List<double> Speed, double synAcc = 0.5, int startSeg = 0)
        {
            ClearBuffer();
            if (Speed.Count < pts.Count)
            {
                MainModel.AddInfo("The speed list is less than pts.");
                return;
            }

            for (int i = 0; i < pts.Count; i++)
            {
                int[] tgPos = new int[3] { pts[i][0], pts[i][1], pts[i][2]};
                double speed = Speed[i];
                NMC_CrdLineXYZEx(pcrdHandle, i + startSeg, 0x0F, tgPos, 0, speed, synAcc, 0);
            }

            MoveCoordinateAxis();
        }

        /// <summary>
        /// the task to read coordinate
        /// </summary>
        public override void ReadCoordinate(out int[] pos)
        {
            int[] retin = new int[3];
            for (short i = 0; i < 3; i++)
            {
                NMC_MtGetPrfPos(axisHandle[i], ref retin[i]);
            }

            pos = retin;
        }
    }
}
