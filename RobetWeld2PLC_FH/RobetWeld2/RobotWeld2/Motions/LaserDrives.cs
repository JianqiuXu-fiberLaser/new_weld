///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld2.Motions
{
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

        public LaserDrives() {}

        /// <summary>
        /// Setup the laser working mode
        /// </summary>
        /// <param name="mode">0: CW; 1: QCW; 2: follow</param>
        /// <param name="maxPower"> Maximum Power </param>
        public static void InitialLaser(int mode = 0)
        {
            if (mode == 2)
            {
                NMC_LaserSetMode(devHandle, TIME_ARRAY_OUTPUT_MODE, LASER_CHANNEL);
            }
            else if (mode == 1)
            {
                NMC_LaserSetMode(devHandle, SHIO_OUTPUT_MODE, LASER_CHANNEL);
            }
            else
            {
                NMC_SetDacMode(devHandle, 256, 1);
                NMC_LaserSetMode(devHandle, BASIC_OUTPUT_MODE, 0);
                NMC_LaserSetOutputType(devHandle, LASER_PWM_DUTY, 0, 1000, 0);
                NMC_LaserSetPowerEx(devHandle, 100, 0);
            }
        }

        public static bool LaserOn(double powerPercent)
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
