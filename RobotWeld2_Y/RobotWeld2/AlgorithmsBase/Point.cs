using System.Windows.Markup;

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

        public override string ToString()
        {
            _vector ??= new(0, 0, 0);
            return (_vector.X.ToString() + "," + _vector.Y.ToString()
                + "," + _vector.Z.ToString() + "," + _lineType.ToString()
                + "," + _laserState.ToString() + "," + _laserPointPower.ToString());
        }

        public string ToScreen()
        {
            _vector ??= new(0, 0, 0);
            string svar = string.Format("{0,-15}{1,-15}{2,-15}\t{3,-12}\t{4,-12}{5,-12}\n", _vector.X, _vector.Y, _vector.Z, _lineType, _laserState, _laserPointPower);
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
    }
}
