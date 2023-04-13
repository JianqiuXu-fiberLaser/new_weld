using RobotWeld.AlgorithmsBase;
using System.Collections.Generic;
using System;

namespace RobotWeld.GetTrace
{
    /// <summary>
    /// the struct of trace pointer
    /// </summary>
    
    public class Pointer
    {
        private int _laserState;
        private int _lineType;
        private Vector _vector = new Vector(0,0,0);

        public Pointer(int ls, int lt, Vector vc)
        {
            _laserState = ls;
            _lineType = lt;
            _vector = vc;
        }

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

        public Vector Corrdinate
        {
            get { return _vector; }
            set { _vector = value; }
        }
    }

    /// <summary>
    /// the collection of a set of pointers 
    /// which is the pointer data of a specially trace 
    /// </summary>

    [Serializable]
    public class PointerList
    {
        private readonly List<Pointer> _pointerList;

        public PointerList ()
        {
            _pointerList = new List<Pointer>();
        }

        public PointerList(int ls, int lt, Vector vc) 
        {
            if (_pointerList == null)
                _pointerList = new List<Pointer> ();
            else
                _pointerList.Add(new Pointer(ls, lt, vc));
        }

        public List<Pointer> PList
        { 
            get 
            { 
                if (_pointerList != null)
                    return _pointerList; 
                else
                    return new List<Pointer>();
            } 
        }

        public bool Open()
        {
            return true;
        }

        public void Save()
        {
            
        }
    }
}
