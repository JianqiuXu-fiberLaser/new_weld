///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) redistribute function of visual point list.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.GetTrace;
using RobotWeld3.Motions;
using RobotWeld3.ViewModel;
using RobotWeld3.Welding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// The model for PointList user control. 
    /// </summary>
    internal class PointListModel
    {
        private WorkPackage? _workPackage;
        private PointListViewModel? _pointListViewModel;
        private ObservableCollection<DisPoint>? _cpoints;
        private MainModel? _mw;

        internal PointListModel() { }

        /// <summary>
        /// Put point list view model to visual point list
        /// </summary>
        /// <param name="pvt"></param>
        internal void PutViewModel(in PointListViewModel pvt)
        {
            _pointListViewModel = pvt;
        }

        /// <summary>
        /// Put workPackage of user control.
        /// </summary>
        /// <param name="ptv"></param>
        /// <param name="wk"></param>
        internal void PutWorkPackage(in WorkPackage wk)
        {
            _workPackage = wk;
            _mw = MainWindow.GetMainModel();
            _cpoints = new ObservableCollection<DisPoint>();
            SetViewPoint(wk.PointList.Ptlist);
        }

        /// <summary>
        /// Refresh the copy point list when any point property being changed.
        /// </summary>
        public void RefreshParameter(int index = 0)
        {
            if (_pointListViewModel == null || _cpoints == null) return;

            int di;
            if (index != 0) di = index;
            else di = _pointListViewModel.PtIndex;

            var tp = new ObservableCollection<DisPoint>();
            for (int i = 0; i < _cpoints.Count; i++)
                tp.Add(_pointListViewModel.PointList[i]);

            _pointListViewModel.PtIndex = 0;
            _cpoints.Clear();
            for (int i = 0; i < tp.Count; i++)
                _cpoints.Add(tp[i]);

            _pointListViewModel.PtIndex = di;

            _mw?.RePlotFreeCurve(_cpoints);
        }

        /// <summary>
        /// Automatically refresh the view, it need to add the point list once more.
        /// </summary>
        /// <param name="cp"></param>
        private void SetViewPoint(List<DisPoint> cp)
        {
            if (_pointListViewModel == null || _cpoints == null) return;

            _pointListViewModel.PtIndex = 0;
            _cpoints.Clear();
            for (int i = 0; i < cp.Count; i++)
                _cpoints.Add(new DisPoint(cp[i]));

            _pointListViewModel.PointList = _cpoints;

            _mw?.RePlotFreeCurve(_cpoints);
        }

        /// <summary>
        /// When change trace type, put a new point list view model to the class of 
        ///   visual point list.
        /// </summary>
        /// <param name="ptv"></param>
        /// <param name="wk"></param>
        internal void UpdateWorkPackage(WorkPackage wk)
        {
            _workPackage = wk;
            SetViewPoint(wk.PointList.Ptlist);
        }


        internal ObservableCollection<DisPoint> GetPointList()
        {
            _cpoints ??= new ObservableCollection<DisPoint>();
            return _cpoints;
        }

        //-----------------------------------------------------------------
        // POINT : functions to treat point with copyPoint.
        //         Refresh only when they are confirmed by click ok button.
        //-----------------------------------------------------------------

        /// <summary>
        /// when chose the OK buttton
        /// </summary>
        internal void OkChoose()
        {
            if (_workPackage == null) return;
            if (_pointListViewModel != null && _pointListViewModel.PointList.Count > 0)
            {
                var p = new PointListData(_pointListViewModel.PointList);
                _workPackage.SetPointList(p);
            }
            else
                _workPackage.ClearPointList();

            // save the point list to corresponding trace file
            string sr = CTraceType.GetName(_workPackage.Tracetype);
            int icode = _workPackage.TrCode[sr];
            _workPackage.TimeStamp = CTimeStamp.CreateTimeStamp();
            AccessTraceFile.Save(icode, _workPackage);
        }

        /// <summary>
        /// when chose to cancel button, then back to old point list.
        /// </summary>
        internal void CancelChoose()
        {
            if (_workPackage != null)
            {
                SetViewPoint(_workPackage.PointList.Ptlist);
            }
        }

        /// <summary>
        /// when chose the change button
        /// </summary>
        /// <param name="takeTrace"> TakeTrace in the main model </param>
        internal void ChangePoint(TakeTrace takeTrace)
        {
            if (_pointListViewModel == null || _cpoints == null) return;
            int[] ptos = new int[4];

            takeTrace.ReadXYZ(ref ptos);

            int i = _pointListViewModel.PtIndex;
            if (i < _cpoints.Count && i >= 0)
            {
                _cpoints[i].Vector = new Vector(ptos[0], ptos[1], ptos[2], ptos[3]);
                RefreshParameter();
            }
        }

        /// <summary>
        /// Delete point
        /// </summary>
        internal void DeletePoint()
        {
            if (_cpoints == null || _pointListViewModel == null || _pointListViewModel.PtIndex < 0) return;

            int i = _pointListViewModel.PtIndex;
            if (_cpoints.Count == 1)
            {
                _cpoints.Clear();
                return;
            }
            else if (_cpoints.Count > 1)
            {
                if (i < _cpoints.Count - 1)
                {
                    _cpoints.RemoveAt(i);
                    _pointListViewModel.PtIndex = i;
                }
                else if (i == _cpoints.Count - 1)
                {
                    _cpoints.RemoveAt(i);
                    _pointListViewModel.PtIndex = _cpoints.Count - 1;
                }
            }

            _mw?.RePlotFreeCurve(_cpoints);
        }

        /// <summary>
        /// Change the point type to Line
        /// </summary>
        internal void ChangeLine()
        {
            if (_pointListViewModel == null || _pointListViewModel.PtIndex < 0 || _cpoints == null) return;

            int i = _pointListViewModel.PtIndex;
            _cpoints[i].Linetype = 0;
            RefreshParameter();
        }

        /// <summary>
        /// Change the point type to arc
        /// </summary>
        internal void ChangeArc()
        {
            if (_pointListViewModel == null || _pointListViewModel.PtIndex < 0 || _cpoints == null) return;

            int i = _pointListViewModel.PtIndex;
            _cpoints[i].Linetype = 1;
            RefreshParameter();
        }

        /// <summary>
        /// Add point with the current position after this point
        /// </summary>
        /// <param name="takeTrace"> TakeTrace in the main model </param>
        internal void AddPoint(TakeTrace takeTrace)
        {
            if (_pointListViewModel == null || _cpoints == null) return;

            int[] ptos = new int[4];
            takeTrace?.ReadXYZ(ref ptos);

            var di = _pointListViewModel.PtIndex;
            var d = _cpoints[di];
            var lp = new LaserDisplayParameter(d.Parameter);

            DisPoint onept = new()
            {
                Vector = new Vector(ptos[0], ptos[1], ptos[2], ptos[3]),
                Parameter = lp
            };

            if (di >= 1 && d.Power != 0 && _cpoints[di - 1].Power != 0)
                onept.Linetype = _cpoints[di - 1].Linetype;
            else
                onept.Linetype = 0;

            _cpoints.Insert(di + 1, onept);
            _pointListViewModel.PtIndex = _cpoints.Count;
            RefreshParameter();
        }

        /// <summary>
        /// When chose goto the position button
        /// </summary>
        /// <param name="mbus"></param>
        internal void GotoSingleStep(MotionBus mbus)
        {
            GetXYZA(out int[] ptxyz);
            var ss = new SingleStep(mbus);
            ss.SetupParameter();
            ss.GotoPosition(ptxyz);
        }

        /// <summary>
        /// Get mechanical position from display XYZ values.
        /// </summary>
        /// <param name="ptxyz"></param>
        private void GetXYZA(out int[] ptxyz)
        {
            int pd;
            ptxyz = new int[4];
            if (_pointListViewModel == null)
            {
                ptxyz[0] = 0;
                ptxyz[1] = 0;
                ptxyz[2] = 0;
                ptxyz[3] = 0;
                return;
            }

            pd = _pointListViewModel.PtIndex;
            DisPoint pt = _pointListViewModel.PointList[pd];

            ptxyz[0] = (int)pt.Vector.X;
            ptxyz[1] = (int)pt.Vector.Y;
            ptxyz[2] = (int)pt.Vector.Z;
            ptxyz[3] = (int)pt.Vector.A;
        }
    }
}
