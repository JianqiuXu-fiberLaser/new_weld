///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) refrash saved file and point list.
//               Interlligent to calculate the rise and fall time of laser.
//           (2) add time stamp of each workpackage.
//           (3) add the trace parameter.
//
// Ver. 3.0: (1) Keep the point and trace information in this file, and
//               other weld moving information in WMS data.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.GetTrace;
using RobotWeld3.Welding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// Data structure stored in the disk or server for one welding works. 
    ///   The workpackage is main data structure used to store, operate and 
    ///   display DisPoints.
    ///   (1) timeStampe: the time establish this point list, and the 
    ///       corresponding Wms should has the same time stamp.
    ///   (2) TrCode: the dictionary for tracetype (string in Xml) and its
    ///       file index.
    ///   (3) order: the trace order, for the case more than one traces.
    ///   (4) PointList: the DisPoint List
    ///   (5) tracetype
    ///   (6) material
    ///   (7) wire
    ///   (8) itp: Interface for various trace type, used for parameter list.
    /// </summary>
    internal class WorkPackage
    {
        private int _timeStamp;
        private Dictionary<string, int>? _trCode;
        private int _order;
        private PointListData? _pointList;
        private Tracetype _tracetype;
        private MetalMaterial? _material;
        private Wire? _wire;
        private List<double>? _pml;

        public WorkPackage() { }

        /// <summary>
        /// Set the point list
        /// </summary>
        /// <param name="p"> PointListData </param>
        public void SetPointList(PointListData p)
        {
            _pointList = p;
        }

        /// <summary>
        /// Set the point list with new List of Display point
        /// </summary>
        /// <param name="p"></param>
        public void SetPointList(ObservableCollection<DisPoint> p)
        {
            _pointList = new PointListData(p);
        }

        /// <summary>
        /// Get parameter of trace type, if it has.
        /// </summary>
        /// <returns> the trace parameters </returns>
        internal List<double> GetParameter()
        {
            return _pml ?? new List<double>();
        }

        /// <summary>
        /// Set trace parameters into Pmlist, which explicit means depends on trace type.
        /// </summary>
        /// <param name="pml"></param>
        internal void SetParameter(List<double> pml)
        {
            _pml = pml;
        }

        public Tracetype Tracetype
        {
            get => _tracetype;
            set => _tracetype = value;
        }

        public int TimeStamp
        {
            get => _timeStamp;
            set => _timeStamp = value;
        }

        public PointListData PointList
        {
            get => _pointList ?? new PointListData();
            set => _pointList = value;
        }

        public Dictionary<string, int> TrCode
        {
            get => _trCode ?? new Dictionary<string, int>();
            set => _trCode = value;
        }

        public int Order
        {
            get => _order;
            set => _order = value;
        }

        /// <summary>
        /// Clear point list
        /// </summary>
        internal void ClearPointList()
        {
            _pointList?.Clear();
        }

        internal MetalMaterial Material
        {
            get => _material ?? new MetalMaterial();
            set => _material = value;
        }

        internal Wire Wire
        {
            get => _wire ?? new Wire();
            set => _wire = value;
        }
    }
}
