///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) all method are static, that means, only one motion 
//               card can be used.
//
///////////////////////////////////////////////////////////////////////

using System;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// CLASS: the drive for laser
    /// </summary>
    internal class LaserDriver : MotionBoard
    {
        //
        // Constants
        //
        private const int POWER_CODE = 32767;
        private const int VOLTAGE_CODE = 65534;
        private const int VOLT_BASE = -32766;

        private const double WOBBLE_VOLT = 5.0;
        private const int LASER_CHANNEL = 0;

        internal LaserDriver() { }

        /// <summary>
        /// Setup the laser working mode, without input parameter, then be the CW mode
        /// </summary>
        /// <param name="frequency"> For CW, 1000 </param>
        /// <param name="duty"> For CW, 100 </param>
        internal static void InitialLaser(int frequency = 1000, int duty = 100)
        {
            NMC_SetDacMode(devHandle, 256, 1);
            NMC_LaserSetMode(devHandle, BASIC_OUTPUT_MODE, LASER_CHANNEL);
            NMC_LaserSetOutputType(devHandle, LASER_PWM_DUTY, 0, frequency, LASER_CHANNEL);
            NMC_LaserSetPowerEx(devHandle, duty, LASER_CHANNEL);
        }

        /// <summary>
        /// Reset PWM output
        /// </summary>
        public static void ResetPWM()
        {
            NMC_LaserOnOff(devHandle, 0, 0);
        }

        /// <summary>
        /// Set analog voltage from axis servo, which used to driver wobble.
        /// </summary>
        /// <param name="AxisNum"> Axis number from 0 </param>
        /// <param name="voltage"> the voltage </param>
        public static void SetAnalog(int AxisNum, double voltage)
        {
            short VCode = (short)(VOLT_BASE + VOLTAGE_CODE * voltage / WOBBLE_VOLT);
            NMC_SetDac(devHandle, (short)AxisNum, VCode);
        }

        /// <summary>
        /// Switch on the laser immediately.
        /// </summary>
        /// <param name="powerPercent"></param>
        /// <returns></returns>
        internal static bool LaserOn(double powerPercent)
        {
            short pcode = (short)(powerPercent * POWER_CODE);
            NMC_LaserOnOff(devHandle, 1, 0);
            NMC_SetDac(devHandle, 256, pcode);
            return true;
        }

        /// <summary>
        /// Switch off laser immediately.
        /// </summary>
        internal static void LaserOff()
        {
            NMC_SetDac(devHandle, 256, 0);
            NMC_LaserOnOff(devHandle, 0, 0);
            DisableSW();
        }

        /// <summary>
        /// Enable Laser switch on.
        /// </summary>
        internal static void EnableSW()
        {
            SetBit(MotionSpecification.LaserEnable, false);
        }

        /// <summary>
        /// Disable Laser switch off.
        /// </summary>
        internal static void DisableSW()
        {
            SetBit(MotionSpecification.LaserEnable, true);
        }

        /// <summary>
        /// the time pass in ms, used for slow rising and slow falling
        /// </summary>
        /// <returns> time span passed from the start point</returns>
        internal static long PassTime()
        {
            TimeSpan tSpan = DateTime.Now - new DateTime(2023, 4, 1, 0, 0, 0);
            return tSpan.Milliseconds;
        }
    }
}
