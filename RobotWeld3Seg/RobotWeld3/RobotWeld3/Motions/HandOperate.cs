
///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.AppModel;
using RobotWeld3.GetTrace;
using System.CodeDom;
using System.Threading;
using System.Windows.Media.Imaging;

namespace RobotWeld3.Motions
{
    internal class HandOperate
    {
        private readonly object _lock = new();
        private readonly MotionBoard _mb;
        private readonly TakeTrace _tkn;

        private static bool _hThreadFlag;

        internal HandOperate(MotionBus mbus)
        {
            _mb = mbus.GetMotionBoard();
            _tkn = mbus.GetTakeTrace();
        }

        internal static void SetHandThread(bool flag)
        {
            _hThreadFlag = flag;
        }

        internal void HandThread()
        {
            _hThreadFlag = true;

            HandWheel.SetHandWheel(0, 1);

            // the  key index is set to -1, which is not real key index.
            // The first press of key can be captured.
            short axisIndex = -1;
            short axisNow = -1;
            int ratioCof = -1;
            int ratioCofNow = -1;
            bool SetFlag = false;

            lock (_lock)
            {
                while (true)
                {
                    if (!_hThreadFlag) return;
                    else Thread.Sleep(10);

                    NoChanged();
                    bool? readIo;
                    bool[] retinfo;
                    if (MotionSpecification.AaxisState == 1)
                        readIo = MotionBoard.ReadIoGroup(out retinfo);
                    else
                        readIo = MotionBoard.ReadIoGroup(out retinfo);

                    if (readIo == null || readIo == false) continue;

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
                        if (axisNow != 3 && MotionSpecification.AaxisState == 1)
                        {
                            SetFlag = true;
                            axisIndex = axisNow = 3;
                        }
                    }

                    // setup the new axis and ratio
                    if (SetFlag)
                    {
                        HandWheel.SetHandWheel(axisIndex, ratioCof);
                        SetFlag = false;
                    }
                }
            }
        }

        private void NoChanged()
        {
            _mb.ReadCoordinate(out int[] barePos);

            int[] pos = new int[4];
            pos[0] = MotionSpecification.XDirection * barePos[0];
            pos[1] = MotionSpecification.YDirection * barePos[1];
            pos[2] = MotionSpecification.ZDirection * barePos[2];
            pos[3] = barePos[3];
            _tkn.DisplayXYZ(pos[0], pos[1], pos[2], pos[3]);
        }
    }
}
