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
        private LaserParameter _laserParameter;
        private Vector? _vector;
        private int _pointNum;

        public Point()
        {
            _lineType = 0;
            _laserState = 0;
            _laserParameter = new LaserParameter();
            _vector = new(0, 0, 0);
        }

        public Point(Point po)
        {
            _lineType = po.LineType;
            _laserState = po.LaserState;
            _laserParameter = po.Parameter;
            _vector = new Vector(po.vector);
        }

        public void SetNum(int iNum)
        {
            _pointNum = iNum;
        }

        /// <summary>
        /// Set point parametersw
        /// </summary>
        /// <param name="lt"> the line type</param>
        /// <param name="ls"> the laser on/off </param>
        /// <param name="pp"> the laser point power </param>
        /// <param name="vc"> the vector </param>
        public Point(int lt, int ls, LaserParameter lp, Vector vc)
        {
            _lineType = lt;
            _laserState = ls;
            _laserParameter = lp;
            _vector = vc;
        }

        public LaserParameter GetLaserParameter()
        {
            return _laserParameter;
        }

        public LaserParameter Parameter
        {
            get => _laserParameter;
            set => _laserParameter = value;
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
            set => _vector = value; 
        }

        public int LaserPointPower
        {
            get => _laserParameter.Power;
            set => _laserParameter.Power = value;
        }

        public int Frequency
        {
            get => _laserParameter.Frequency;
            set => _laserParameter.Frequency = value;
        }

        public int Pulse
        {
            get => _laserParameter.Pulse; 
            set => _laserParameter.Pulse = value;
        }

        public int Duty
        {
            get => _laserParameter.Duty; 
            set => _laserParameter.Duty = value;
        }

        public int Wobble
        {
            get => _laserParameter.Wobble; 
            set => _laserParameter.Wobble = value;
        }

        public int WeldSpeed
        {
            get => _laserParameter.WeldSpeed;
            set => _laserParameter.WeldSpeed = value;
        }

        public int Leap
        {
            get => _laserParameter.Leap;
            set => _laserParameter.Leap = value;
        }

        public double Xcoordinate
        {
            get => vector.X / 1000;
            set => vector.X = value * 1000;
        }

        public double Ycoordinate
        {
            get => vector.Y / 1000;
            set => vector.Y = value * 1000;
        }

        public double Zcoordinate
        {
            get => vector.Z / 1000; 
            set => vector.Z = value * 1000;
        }

        public int PointNum
        {
            get => _pointNum;
            set => _pointNum = value;
        }
    }
}
