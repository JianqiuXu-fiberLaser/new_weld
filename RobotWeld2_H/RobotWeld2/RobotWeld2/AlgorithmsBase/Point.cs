using RobotWeld2.Motions;
using RobotWeld2.Welding;

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
        // LaserState
        // 0: no lasing
        // 1: lasing
        //
        private int _laserState;

        //
        // the laser power at this point
        //
        private int _laserPointPower;




        private Vector? _vector;
        private double _a;
        private LaserWeldParameter? _parameter;

        public Point()
        {
            _lineType = 0;
            _laserState = 0;
            _laserPointPower = 0;
            _vector = new(0, 0, 0);
        }

        /// <summary>
        /// Set point parametersw
        /// </summary>
        /// <param name="lt"> the line type</param>
        /// <param name="ls"> the laser on/off </param>
        /// <param name="pp"> the laser point power </param>
        /// <param name="vc"> the vector </param>
        public Point(int lt, int ls, int pp, Vector vc)
        {
            _lineType = lt;
            _laserState = ls;
            _laserPointPower = pp;
            _vector = vc;
        }

        public Point(int lt, int ls, Vector vc, LaserWeldParameter lp)
        {
            _lineType = lt;
            _laserState = ls;
            _vector = vc;
            _parameter = lp;
        }

        public Point(int lt, int ls, Vector vc, int a, LaserWeldParameter lp)
        {
            _lineType = lt;
            _laserState = ls;
            _vector = vc;
            _a = a;
            _parameter = lp;
        }

        public override string ToString()
        {
            _vector ??= new(0, 0, 0);
            return (_vector.X.ToString() + "," + _vector.Y.ToString() + "," + _vector.Z.ToString() + ","
                + _lineType.ToString() + "," + _laserState.ToString() + "," + _laserPointPower.ToString());
        }

        public string ToFile()
        {
            _vector ??= new(0, 0, 0);
            string svar = (_vector.X.ToString() + "," + _vector.Y.ToString() + "," + _vector.Z.ToString() + ","
                + _a.ToString() + "," + _lineType.ToString() + "," + _laserState.ToString());

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

            string laserString;
            if (_laserState == 0)
            {
                laserString = "关";
            }
            else
            {
                laserString = "开";
            }

            _vector ??= new(0, 0, 0);
            string svar = string.Format("{0,-15}{1,-15}{2,-15}\t{3,-12}\t{4,-12}{5,-12}\n",
                _vector.X, _vector.Y, _vector.Z, lineString, laserString, _laserPointPower);
            return svar;
        }

        //---- properties ----
        public int LaserState
        {
            get { return _laserState; }
            set { _laserState = value; }
        }

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
                    return _vector.X / MotionOperate.OneMillimeter;
                }
            }
            set
            {
                _vector ??= new(0, 0, 0);
                _vector.X = value * MotionOperate.OneMillimeter;
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
                    return _vector.Y / MotionOperate.OneMillimeter;
                }
            }
            set
            {
                _vector ??= new(0, 0, 0);
                _vector.Y = value * MotionOperate.OneMillimeter;
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
                    return _vector.Z / MotionOperate.OneMillimeter;
                }
            }
            set
            {
                _vector ??= new(0, 0, 0);
                _vector.Z = value * MotionOperate.OneMillimeter;
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
