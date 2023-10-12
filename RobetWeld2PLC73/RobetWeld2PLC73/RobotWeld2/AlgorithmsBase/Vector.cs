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
    /// Vector definition in the coordination of 3D Cartesian system
    /// </summary>
    public class Vector
    {
        private double _x;
        private double _y;
        private double _z;

        public Vector(double xvalue = 0, double yvalue = 0, double zvalue = 0)
        {
            _x = xvalue;
            _y = yvalue;
            _z = zvalue;
        }

        public Vector(Vector v)
        {
            _x = v.X;
            _y = v.Y;
            _z = v.Z;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// Transfor a Vector to the PolarVector
        /// </summary>
        /// <param name="vc"> the Vector </param>
        /// <returns> polarVector, 0 ~ 2 * PI </returns>
        public static PolarVector ToPolar(Vector vc)
        {
            if (vc.X == 0)
            {
                if (vc.Y == 0) return new PolarVector();
                else if (vc.Y > 0) return new PolarVector(PI / 2, Abs(vc.Y));
                else return new PolarVector(-PI / 2, Abs(vc.Y));
            }

            double angle = Atan(vc.Y / vc.X);
            double radius = Sqrt(vc.X * vc.X + vc.Y * vc.Y);

            return new PolarVector(angle, radius);
        }
    }
}
