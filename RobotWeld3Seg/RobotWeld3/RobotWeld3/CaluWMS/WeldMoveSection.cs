///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver 2.2: revise the point list to match single workpackage file.
// Ver. 3.0: (1) all=in-one data structure.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using RobotWeld3.Welding;
using System;
using System.Collections.Generic;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Fundamental data structure of move section for welding
    /// In one WMS, only trace point changed, all other parameters
    ///     do not change.
    /// </summary>
    public class WeldMoveSection : IComparable<WeldMoveSection>
    {
        //
        // wmsIndex: the order number of the section in a sequence points.
        // Position: corrdinates of x, y, z, a axes for each point,
        //           including the start point and the end point,
        //           which linked the previous and next WMS.
        //           In this case, we can check whether the WMS being lost.
        //           When transfering to hardware array of coordinate, generally
        //           the first position should be discarded.
        // Speed: the speed list. Note that the value is the speed running from
        //        this point to the next one. In this case, the last speed is
        //        not used.
        // LaserParameter: The full parameters of laser
        //

        private int _wmsIndex;
        private readonly List<Vector> _position;
        private List<double>? _speed;
        private LaserParameter? _lp;
        private MetalMaterial? _material;
        private Wire? _wire;
        private double _wobble;

        public WeldMoveSection()
        {
            _position = new List<Vector>();
        }

        public WeldMoveSection(List<Vector> vc)
        {
            _position = vc;
        }

        public int WmsIndex
        {
            get => _wmsIndex;
            set => _wmsIndex = value;
        }

        public List<double> Speed
        {
            get => _speed ?? new List<double>();
            set => _speed = value;
        }

        public MetalMaterial Material
        {
            get => _material ?? new MetalMaterial();
            set => _material = value;
        }

        public Wire Wire
        {
            get => _wire ?? new Wire();
            set => _wire = value;
        }

        public double Wobble
        {
            get => _wobble;
            set => _wobble = value;
        }

        public LaserParameter LaserParameter
        {
            get => _lp ?? new();
            set => _lp = value;
        }

        public Vector GetFirstPosition()
        {
            return _position[0];
        }

        public Vector GetLastPosition()
        {
            return _position[_position.Count - 1];
        }

        public int GetPointCount()
        {
            return _position.Count;
        }

        public int GetLaserPower()
        {
            if (_lp != null) return _lp.Power;
            else return 0;
        }

        /// <summary>
        /// Get the reference of positions
        /// </summary>
        /// <returns> List(vector) </returns>
        public List<Vector> GetPosition()
        {
            return _position;
        }

        public int CompareTo(WeldMoveSection? other)
        {
            if (other == null) return 0;
            else
                return this.WmsIndex.CompareTo(other.WmsIndex);
        }
    }
}
