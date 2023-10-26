///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: new class for planar vector
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// 2D vector in a coordinate plane
    /// </summary>
    public class PlaneVector
    {
        private double _x;
        private double _y;

        public PlaneVector(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public bool Equals(PlaneVector other)
        {
            if (other == null) return false;
            if (_x == other.X &&  _y == other.Y) return true;
            else return false;
        }

        public double X
        {
            get => _x; 
            set => _x = value;
        }

        public double Y
        {
            get => _y; 
            set => _y = value;
        }
    }
}
