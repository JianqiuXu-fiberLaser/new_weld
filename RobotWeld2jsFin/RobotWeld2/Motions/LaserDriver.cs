using RobotWeld2.AlgorithmsBase;
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

        public LaserDrives() { }

        /// <summary>
        /// Setup the laser working mode, without input parameter, then be the CW mode
        /// </summary>
        /// <param name="frequency"> For CW, 1000 </param>
        /// <param name="duty"> For CW, 100 </param>
        public void InitialLaser(int frequency = 1000, int duty = 100)
        {
            NMC_SetDacMode(devHandle, 256, 1);
            NMC_LaserSetMode(devHandle, BASIC_OUTPUT_MODE, LASER_CHANNEL);
            NMC_LaserSetOutputType(devHandle, LASER_PWM_DUTY, 0, frequency, LASER_CHANNEL);
            NMC_LaserSetPowerEx(devHandle, duty, LASER_CHANNEL);
        }

        public bool LaserOn(double powerPercent)
        {
            short pcode = (short)(powerPercent * POWER_CODE);
            NMC_LaserOnOff(devHandle, 1, 0);
            NMC_SetDac(devHandle, 256, pcode);
            return true;
        }

        public static void ResetPWM()
        {
            NMC_LaserOnOff(devHandle, 0, 0);
        }

        public static void LaserOff()
        {
            NMC_SetDac(devHandle, 256, 0);
            NMC_LaserOnOff(devHandle, 0, 0);
        }

        /// <summary>
        /// Set analog voltage from axis servo
        /// </summary>
        /// <param name="AxisNum"> Axis number from 0 </param>
        /// <param name="voltage"></param>
        public static void SetAnalog(int AxisNum, double voltage)
        {
            short mode;
            double reciMaxVol;
            if (AxisNum == 0)
            {
                mode = 0;
                reciMaxVol = 0.2;

            }
            else if (AxisNum == 1)
            {
                mode = 0;
                reciMaxVol = 0.2;
            }
            else
            {
                mode = 1;
                reciMaxVol = 0;
            }

            NMC_SetDacMode(devHandle, (short)AxisNum, mode);
            short VCode = (short)(reciMaxVol * voltage * POWER_CODE);
            NMC_SetDac(devHandle, (short)AxisNum, VCode);
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
