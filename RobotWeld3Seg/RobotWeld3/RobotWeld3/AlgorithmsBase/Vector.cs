///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// coordination of 3D Cartesian system
    /// </summary>
    public class Vector : IEquatable<Vector>
    {
        private double _x;
        private double _y;
        private double _z;
        private double _a;

        public Vector(double xvalue = 0, double yvalue = 0, double zvalue = 0, double avalue = 0)
        {
            _x = xvalue;
            _y = yvalue;
            _z = zvalue;
            _a = avalue;
        }

        public Vector(Vector vc)
        {
            _x = vc.X;
            _y = vc.Y;
            _z = vc.Z;
            _a = vc.A;
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

        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        public override bool Equals(object? other)
        {
            if (other is not Vector v) return false;

            if (this._z == v.Z && this._y == v.Y && this._x == v.X)
                return true;
            else
                return false;
        }

        /// <summary>
        /// The coordinate of two Vector are equal?
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool Equals(Vector? v)
        {
            if (v == null) return false;

            if (_z == v.Z && _y == v.Y && _x == v.X)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _x.GetHashCode() * _y.GetHashCode() * _z.GetHashCode();
        }
    }
}
