namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// coordination of 3D Cartesian system
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
