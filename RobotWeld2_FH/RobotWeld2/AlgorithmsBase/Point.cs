using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System.Transactions;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// the struct of trace pointer
    /// </summary>

    public class Point
    {
        //
        // LineType: the curve after this pointer
        // 0: line 
        // 1: arc
        //
        private int _lineType;

        //
        // the laser power at this point
        //
        private int _laserPointPower;

        private Vector? _vector;
        private double _a;
        private LaserWeldParameter? _parameter;
        private int _pointNum;

        public Point()
        {
            _lineType = 0;
            _vector = new(0, 0, 0);
            _a = 0;
            _parameter = new LaserWeldParameter(0);
        }

        public Point(int lt, Vector vc, int a, LaserWeldParameter lp)
        {
            _lineType = lt;
            _vector = new(vc);
            _a = a;
            _parameter = new(lp);
        }

        public Point(Point p)
        {
            _lineType = p.LineType;
            _a = p.A;
            _vector = new(p.vector);
            _parameter = new(p.Parameter);
        }

        public Point(int lt, Vector vc, int a)
        {
            _lineType = lt;
            _vector = new(vc);
            _a = a;
        }

        /// <summary>
        /// Set the weld and leap speed for its laser weld parameters
        /// </summary>
        /// <param name="weldspeed"> weld speed </param>
        /// <param name="leapspeed"> leap speed </param>
        public void SetSpeed(double weldspeed, double leapspeed)
        {
            if (_parameter != null)
            {
                _parameter.WeldSpeed = weldspeed;
                _parameter.LeapSpeed = leapspeed;
            }
        }

        public override string ToString()
        {
            _vector ??= new(0, 0, 0);
            return (_vector.X.ToString() + "," + _vector.Y.ToString() + "," + _vector.Z.ToString() + ","
                + _lineType.ToString() + "," + _laserPointPower.ToString());
        }

        public string ToFile()
        {
            _vector ??= new(0, 0, 0);
            string svar = (_vector.X.ToString() + "," + _vector.Y.ToString() + "," + _vector.Z.ToString() + ","
                + _a.ToString() + "," + _lineType.ToString());

            return svar;
        }

        public string ToScreen()
        {
            string lineString;
            if (_lineType == 0)
            {
                lineString = "直线";
            }
            else
            {
                lineString = "圆弧";
            }

            _vector ??= new(0, 0, 0);
            string svar = string.Format("{0,-15}{1,-15}{2,-15}\t{3,-12}\t{4,-12}\n",
                _vector.X, _vector.Y, _vector.Z, lineString, _laserPointPower);
            return svar;
        }

        //---- properties ----
        public int LineType
        {
            get { return (_lineType); }
            set { _lineType = value; }
        }

        public Vector vector
        {
            get
            {
                _vector ??= new(0, 0, 0);
                return _vector;
            }
            set { _vector = value; }
        }

        public int LaserPointPower
        {
            get { return (int)_laserPointPower; }
            set { _laserPointPower = value; }
        }

        public LaserWeldParameter Parameter
        {
            get { return _parameter ?? new LaserWeldParameter(); }
            set { _parameter = value; }
        }

        public double Xcoordinate
        {
            get
            {
                if (_vector == null)
                {
                    return 0;
                }
                else
                {
                    return _vector.X / MotionOperate.Xmillimeter;
                }
            }
            set
            {
                _vector ??= new(0, 0, 0);
                _vector.X = value * MotionOperate.Ymillimeter;
            }
        }

        public double Ycoordinate
        {
            get
            {
                if (_vector == null)
                {
                    return 0;
                }
                else
                {
                    return _vector.Y / MotionOperate.Ymillimeter;
                }
            }
            set
            {
                _vector ??= new(0, 0, 0);
                _vector.Y = value * MotionOperate.Ymillimeter;
            }
        }

        public double Zcoordinate
        {
            get
            {
                if (_vector == null)
                {
                    return 0;
                }
                else
                {
                    return _vector.Z / MotionOperate.Zmillimeter;
                }
            }
            set
            {
                _vector ??= new(0, 0, 0);
                _vector.Z = value * MotionOperate.Zmillimeter;
            }
        }

        public int PointNum
        {
            get => _pointNum;
            set
            {
                _pointNum = value;
            }
        }

        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        public double Acoordinate
        {
            get { return 360.0 * _a / MotionOperate.OneCycle; }
            set { _a = MotionOperate.OneCycle * value / 360.0; }
        }

        public int LaserPower
        {
            get
            {
                _parameter ??= new();
                return _parameter.LaserPower;
            }
            set
            {
                _parameter ??= new();
                _parameter.LaserPower = value;
            }
        }

        public int WeldSpeed
        {
            get
            {
                _parameter ??= new();
                return (int)_parameter.WeldSpeed;
            }
            set
            {
                _parameter ??= new();
                _parameter.WeldSpeed = value;
            }
        }

        public int Frequency
        {
            get
            {
                _parameter ??= new();
                return _parameter.Frequency;
            }
            set
            {
                _parameter ??= new();
                _parameter.Frequency = value;
            }
        }

        public int PulseWidth
        {
            get
            {
                _parameter ??= new();
                return _parameter.PulseWidth;
            }
            set
            {
                _parameter ??= new();
                _parameter.PulseWidth = value;
            }
        }
    }
}
