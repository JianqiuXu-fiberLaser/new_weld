using RobotWeld.AlgorithmsBase;
using System.Collections.Generic;
using System;

namespace RobotWeld.GetTrace
{
    /// <summary>
    /// the struct of trace pointer
    /// </summary>
    
    public class Point
    {
        // LineType: the curve after this pointer
        // 0: start pointer = prepared pointer
        // 1: line 
        // 2: arc, start pointer
        // 3: arc, mid pointer
        // 4: arc, end pointer
        // 5: end of all curve
        private int _lineType;

        // LaserState
        // 0: no lasing
        // 1: lasing
        private int _laserState;

        private Vector _vector = new(0,0,0);

        public Point(int lt, int ls, Vector vc)
        {
            _lineType = lt;
            _laserState = ls;
            _vector = vc;
        }

        public override string ToString()
        {
            return (_vector.X.ToString() + "," + _vector.Y.ToString()
                + "," + _vector.Z.ToString() + _lineType.ToString()
                + "," + _laserState.ToString());
        }

        //-- properties
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
            get { return _vector; }
            set { _vector = value; }
        }
    }
}
