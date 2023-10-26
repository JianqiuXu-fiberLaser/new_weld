///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver.2.2: new added class to display laser parameters.
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// point struct for ploting in the screen.
    /// </summary>
    public class LtVector
    {
        private Vector _vector;
        private int _linetype;
        private int _laserState;

        public LtVector(Vector vector, int linetype, int laserState)
        {
            _vector = new Vector(vector);
            _linetype = linetype;
            _laserState = laserState;
        }

        public Vector Vector
        {
            get { return _vector; }
            set { _vector = value; }
        }

        /// <summary>
        /// 0: line; 1: arc
        /// </summary>
        public int LineType
        {
            get { return _linetype; }
            set { _linetype = value; }
        }

        public int LaserState
        {
            get => _laserState;
            set => _laserState = value;
        }

        public double X
        {
            get => _vector.X;
            set => _vector.X = value;
        }

        public double Y
        { 
            get { return _vector.Y; } 
            set { _vector.Y = value; } 
        }

        public double Z
        { 
            get { return _vector.Z; } 
            set { _vector.Z = value; } 
        }
    }
}
