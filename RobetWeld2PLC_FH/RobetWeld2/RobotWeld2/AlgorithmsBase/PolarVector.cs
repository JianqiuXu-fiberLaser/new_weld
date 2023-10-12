///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) new point definition with laser paremeter
//
///////////////////////////////////////////////////////////////////////

using static System.Math;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Vector definition in the polar coordinate system.
    /// </summary>
    public class PolarVector
    {
        private double _angle;
        private double _radius;

        public PolarVector(double angle = 0, double radius = 0)
        {
            _angle = angle;
            _radius = radius;
        }

        public double Angle
        {
            get => _angle;
            set => _angle = value;
        }

        public double Radius
        {
            get => _radius;
            set => _radius = value;
        }

        /// <summary>
        /// Transfor a polar vector to a 3D vector with Z = 0
        /// </summary>
        /// <param name="pv"> polar vector </param>
        /// <returns> the Vector </returns>
        public static Vector ToVector(PolarVector pv)
        {
            double x = pv.Radius * Cos(pv.Angle);
            double y = pv.Radius * Sin(pv.Angle);

            return new Vector(x, y);
        }
    }
}
