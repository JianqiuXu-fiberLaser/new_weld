///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) new laser weld parameter from the server or local
// Ver. 3.0: (1) add 2 xml file to store points' information and using
//               5 data structures to represent moving, laser and
//               material specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using RobotWeld3.Welding;
using System.Collections.Generic;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Analyse the DisPoint and extract useful information.
    /// </summary>
    internal class AnalysePoint
    {
        private PointListData? _ptlist;
        private DisPoint? _disPoint;
        private int _st;
        private int _end;
        private WeldingModel? _wmodel;
        private readonly MetalMaterial _mi;
        private readonly Wire _wi;
        private readonly LaserParameter _lp;

        public AnalysePoint()
        {
            _mi = new MetalMaterial();
            _wi = new Wire();
            _lp = new LaserParameter();
        }

        /// <summary>
        /// Get the reference of Welding Model.
        /// </summary>
        /// <param name="m"></param>
        internal void GetModel(WeldingModel m)
        {
            _wmodel = m;
        }

        /// <summary>
        /// Get the reference of Display point.
        /// </summary>
        /// <param name="p"> point list </param>
        /// <param name="st"> start index </param>
        /// <param name="end"> end index </param>
        internal void GetPoint(PointListData p, int st, int end)
        {
            _ptlist = p;
            _disPoint = _ptlist[st];
            _st = st;
            _end = end;
        }

        /// <summary>
        /// Get the Point list data from List(DisPoint)
        /// </summary>
        /// <param name="p"> List of Dispoint </param>
        /// <param name="st"> start index </param>
        /// <param name="end"> end index </param>
        internal void GetPoint(List<DisPoint> p, int st, int end)
        {
            _ptlist = new PointListData(p);
            _disPoint = _ptlist[st];
            _st = st;
            _end = end;
        }

        /// <summary>
        /// Calculate Metal Material, wire and laser parameters.
        /// </summary>
        /// <returns> tuple (MetalMaterial, Wire, LaserParameter) </returns>
        internal (MetalMaterial mi, Wire wi, LaserParameter lp) CalculateParameter()
        {
            if (_wmodel == null || _disPoint == null) return (_mi, _wi, _lp);

            ExtractMaterial();
            ExtractWire();
            ExtractLaserParameter();

            return (_mi, _wi, _lp);
        }

        /// <summary>
        /// Calculate Laser parameter
        /// </summary>
        /// <returns> Laser Parameter </returns>
        private void ExtractLaserParameter()
        {
            if (_wmodel == null || _disPoint == null) return;

            _lp.Power = _disPoint.Power;
            _lp.Frequency = _disPoint.Frequency;
            _lp.DutyCycle = _disPoint.Duty;

            _lp.LaserRise = _wmodel.CalculateRise(_mi, _wi, _disPoint);
            _lp.LaserFall = _wmodel.CalculateFall(_mi, _wi, _disPoint);
            _lp.Air = _wmodel.CalculateAir(_mi, _wi, _disPoint);
            _lp.LaserHoldtime = 0;

            (_lp.WireBack, _lp.WireSpeed, _lp.WireLength, _lp.WireTime) = _wmodel.CalculateWire(_mi, _wi, _disPoint);
        }

        /// <summary>
        /// Calculate Metal Material parameters.
        /// </summary>
        /// <returns> Metal materila parameter </returns>
        private void ExtractMaterial()
        {
            if (_disPoint == null) return;

            _mi.MaterialType = _disPoint.MaterialIndex;
            _mi.Thickness = _disPoint.Thick;
        }

        /// <summary>
        /// Calculate Wire parameters
        /// </summary>
        /// <returns> Wire parameter </returns>
        private void ExtractWire()
        {
            if (_disPoint == null) return;

            _wi.Diameter = _disPoint.Wire;
            _wi.MaterialIndex = 0;
        }

        /// <summary>
        /// Extract wobble from all points. 
        /// One point's wobble is not zero, then all wms hold wobble.
        /// </summary>
        /// <returns></returns>
        internal double ExtractWobble()
        {
            if (_ptlist == null) return 0;

            for (int i = _st; i <= _end; i++)
            {
                if (_ptlist[i].Wobble != 0) return _ptlist[i].Wobble;
            }

            return 0;
        }
    }
}


