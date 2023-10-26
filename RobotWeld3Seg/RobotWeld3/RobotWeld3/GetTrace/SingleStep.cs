///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.Motions;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld3.GetTrace
{
    /// <summary>
    /// Goto the position given by the point.
    /// </summary>
    public class SingleStep
    {
        private readonly MotionBus _mbus;
        private double moveSpeed;

        /// <summary>
        /// Single step to the position
        /// </summary>
        /// <param name="mbus"></param>
        public SingleStep(MotionBus mbus)
        {
            _mbus = mbus;
        }

        /// <summary>
        /// Set move speed
        /// </summary>
        /// <param name="moveSpeed"></param>
        public void SetupParameter(double moveSpeed = 30)
        {
            this.moveSpeed = moveSpeed;
        }

        /// <summary>
        /// Goto the position
        /// </summary>
        /// <param name="ptxyz"></param>
        public void GotoPosition(int[] ptxyz)
        {
            List<int[]> xyza = new();

            int x = MotionSpecification.XDirection * ptxyz[0];
            int y = MotionSpecification.YDirection * ptxyz[1];
            int z = MotionSpecification.ZDirection * ptxyz[2];
            int a = ptxyz[3];

            xyza.Add(new int[] { x, y, z, a });
            List<double> speed = new()
            {
                this.moveSpeed
            };

            new Thread(_ =>
            {
                _mbus.Move4D(xyza, speed, 0);
            }).Start();
        }
    }
}
