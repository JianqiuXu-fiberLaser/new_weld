namespace RobotWeld2.AlgorithmsBase
{
    public class LtVector
    {
        private Vector _vector;
        private int _linetype;

        public LtVector(Vector vector, int linetype)
        {
            _vector = vector;
            _linetype = linetype;
        }

        public Vector Vector
        { 
            get { return _vector; } 
            set { _vector = value; }
        }

        public int LineType
        { 
            get { return _linetype; } 
            set { _linetype = value; } 
        }

        public double X
        { get { return _vector.X; } set {  _vector.X = value; } }

        public double Y
        { get { return _vector.Y; } set { _vector.Y = value; } }

        public double Z
        { get { return _vector.Z; } set { _vector.Z = value; } }
    }
}
