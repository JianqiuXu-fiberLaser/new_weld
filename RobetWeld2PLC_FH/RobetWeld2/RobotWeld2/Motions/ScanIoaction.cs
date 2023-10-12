///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.Threading;

namespace RobotWeld2.Motions
{
    internal class ScanIOaction : MotionOperate
    {
        int[]? bitAddr;

        public ScanIOaction(MotionOperate mo)
        {
            vm = mo.vm;
            methodName = mo.methodName;
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
                    if (MotionBoard.ReadIoBit(bitAddr[i]))
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
