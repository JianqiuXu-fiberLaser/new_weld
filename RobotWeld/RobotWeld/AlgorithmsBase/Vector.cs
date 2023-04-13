using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// coordination of 3D Cartesian system
    /// </summary>
    public class Vector
    {
        private double _x = 0;
        private double _y = 0;
        private double _z = 0;

        public Vector(double xvalue, double yvalue, double zvalue)
        {
            _x = xvalue;
            _y = yvalue;
            _z = zvalue;
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
    }
}
