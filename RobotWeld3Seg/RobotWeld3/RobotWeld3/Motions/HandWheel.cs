///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// CLASS: Drive for handwheel
    /// </summary>
    public class HandWheel : MotionBoard
    {
        private static readonly short _followRatio = 50;
        private static readonly double _acceSpeed = 0.1;
        private static readonly double _followSpeed = 2000;

        public HandWheel() { }

        /// <summary>
        /// Configurate the Hand Wheel
        /// </summary>
        public static void InitialHandwheel()
        {
            NMC_ClrHandWheel(devHandle);
            short rtn = NMC_SetHandWheelInput(devHandle, 256);
            if (rtn != 0)
                MainModel.AddInfo("设置手轮错误");
            else
                MainModel.AddInfo("设置手轮成功");

            NMC_SetHandWheel(devHandle, 0, _followRatio, _acceSpeed, _followSpeed);
        }

        public static void Exit()
        {
            NMC_ClrHandWheel(devHandle);
        }

        /// <summary>
        /// Change the follow ratio for hand wheel
        /// </summary>
        /// <param name="axisIndex"> the axis to be set </param>
        /// <param name="ratio"> the following ratio </param>
        public static void SetHandWheel(short axisIndex, int Cof)
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
}
