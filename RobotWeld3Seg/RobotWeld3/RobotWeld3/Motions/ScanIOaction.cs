///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) new laser weld parameter from the server or local
// Ver. 3.0: (1) add 2 xml file to store points' information and using
//               5 data structures to represent moving, laser and
//               material specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using System.Threading;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// Scan the IO signal
    /// </summary>

    internal class ScanIOaction
    {
        private static bool _iothreadFlag;
        private int[]? bitAddr;

        private readonly MotionBus _mbus;
        
        public ScanIOaction(MotionBus mbus)
        {
            _mbus = mbus;
        }

        internal static void SetIoThread(bool flag)
        {
            _iothreadFlag = flag;
        }

        public void SetParameter(in int[] bitAddr)
        {
            this.bitAddr = bitAddr;
        }

        public void ScanThread()
        {
            if (bitAddr == null) return;

            _iothreadFlag = true;
            int[] iActNum = new int[bitAddr.Length];

            while (true)
            {
                for (int i = 0; i < bitAddr.Length; i++) iActNum[i] = 0;

                // Read IO for each bit
                for (int i = 0; i < bitAddr.Length; i++)
                {
                    bool ReadIo = MotionBoard.ReadIoBit(bitAddr[i]);

                    if (ReadIo) iActNum[i] = 1;
                    else iActNum[i] = 0;
                }

                // Check efficient IO input, and callback.
                for (int i = 0; i < bitAddr.Length; i++)
                {
                    if (iActNum[i] != 0) _mbus.IoAction(iActNum);
                    else _mbus.NoAction(iActNum);
                }

                // close the Read Thread.
                if (!_iothreadFlag) return;

                Thread.Sleep(20);
            }
        }
    }
}
