using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.AppModel
{
    /// <summary>
    /// Visual Point List Data
    /// </summary>
    public class VisualPointList
    {
        private readonly WorkPackage _workPackage;
        private MainWindowViewModel? _mainWindowViewModel;
        private PointListData? _cpoints;

        public VisualPointList(WorkPackage workPackage)
        {
            _workPackage = workPackage;
        }

        public PointListData Cpoints
        {
            get
            {
                if (_cpoints == null) return new PointListData();
                else return _cpoints;
            }
        }

        public void PutViewModel(in MainWindowViewModel mvm)
        {
            _mainWindowViewModel = mvm;
        }

        /// <summary>
        /// Refresh the copy point list
        /// </summary>
        public void RefreshParameter(int index = 0)
        {
            _cpoints = new(_workPackage.GetPointList());
            _cpoints.SetPointNum();
            DisplayPointList();
            if (_mainWindowViewModel != null && _cpoints != null)
            {
                _mainWindowViewModel.PtIndex = index;
            }
        }

        private void DisplayPointList()
        {
            if (_mainWindowViewModel != null && _mainWindowViewModel.PointList != null && _cpoints != null)
            {
                _mainWindowViewModel.PointList = _cpoints.DisplayPoint();
            }
        }

        /// <summary>
        /// if the point exist, then go to the point.
        /// if the point index is larger than the count, add a new point.
        /// </summary>
        /// <param name="ptxyz"></param>
        public void GetXYZA(out int[] ptxyz)
        {
            _cpoints ??= new(_workPackage.GetPointList());

            if (_cpoints.GetCount() == 0)
            {
                ptxyz = new int[4];
                if (_mainWindowViewModel != null)
                {
                    ptxyz[0] = (int)(_mainWindowViewModel.X * MotionOperate.Xmillimeter);
                    ptxyz[1] = (int)(_mainWindowViewModel.Y * MotionOperate.Ymillimeter);
                    ptxyz[2] = (int)(_mainWindowViewModel.Z * MotionOperate.Zmillimeter);
                    ptxyz[3] = (int)(_mainWindowViewModel.A * MotionOperate.OneCycle / 360.0);
                }
                else
                {
                    ptxyz[0] = 0;
                    ptxyz[1] = 0;
                    ptxyz[2] = 0;
                    ptxyz[3] = 0;
                }

                var vc = new Vector(ptxyz[0], ptxyz[1], ptxyz[2]);
                int lt = 0;
                var lp = new LaserWeldParameter();
                Point onept = new(lt, vc, (int)ptxyz[3], lp);
                _cpoints.Add(onept);
                return;
            }

            int pd;
            if (_mainWindowViewModel == null)
            {
                pd = 0;
            }
            else
            {
                if (_mainWindowViewModel.PtIndex >= _cpoints.GetCount())
                {
                    pd = _cpoints.GetCount() - 1;
                    MorePoint();
                }
                else
                {
                    pd = _mainWindowViewModel.PtIndex;
                }
            }

            Point pt = _cpoints.GetPoint(pd);

            ptxyz = new int[4];
            ptxyz[0] = (int)pt.vector.X;
            ptxyz[1] = (int)pt.vector.Y;
            ptxyz[2] = (int)pt.vector.Z;
            ptxyz[3] = (int)pt.A;
        }

        private void MorePoint()
        {
            int lt;
            if (_mainWindowViewModel != null && _mainWindowViewModel.LineType) lt = 1;
            else lt = 0;

            if (_cpoints != null && _mainWindowViewModel != null)
            {
                List<Point> ptlist = new(_mainWindowViewModel.PointList);
                int i = _cpoints.LastPoint();
                Vector vc = _cpoints.GetVector(i);
                double angle = _cpoints.GetAngle(i);
                LaserWeldParameter lp = ptlist[i].Parameter;
                Point onept = new(lt, vc, (int)angle, lp);

                _cpoints.Add(onept);
                _cpoints.SetPointNum();
                _mainWindowViewModel.PtIndex = _cpoints.LastPoint();
            }

            DisplayPointList();
        }


        public void AddPoint(int x, int y, int z, int a)
        {
            SetXYZ(x, y, z, a);

            int lt;
            if (_mainWindowViewModel != null && _mainWindowViewModel.LineType) lt = 1;
            else lt = 0;

            if (_cpoints != null && _mainWindowViewModel != null)
            {
                var ptlist = _mainWindowViewModel.PointList;
                int i = _mainWindowViewModel.PtIndex;
                if (i < _cpoints.GetCount() && i >= 0)
                {
                    LaserWeldParameter lp = ptlist[i].Parameter;
                    Vector vc = new(x, y, z);
                    Point onept = new(lt, vc, a, lp);
                    _cpoints.ChangePoint(i, onept);
                }
            }

            DisplayPointList();
        }

        public void SetXYZ(int x, int y, int z, int a)
        {
            if (_mainWindowViewModel == null) return;

            _mainWindowViewModel.X = x / MotionOperate.Xmillimeter;
            _mainWindowViewModel.Y = y / MotionOperate.Ymillimeter;
            _mainWindowViewModel.Z = z / MotionOperate.Zmillimeter;
            _mainWindowViewModel.A = 360.0 * a / MotionOperate.OneCycle;
        }

        public void DeletePoint()
        {
            if (_mainWindowViewModel == null || _cpoints == null) return;

            int i = _mainWindowViewModel.PtIndex;
            if (i < 0) return;

            if (_cpoints.GetCount() == 1)
            {
                _cpoints.Clear();
            }
            else if (_cpoints.GetCount() > 1)
            {
                if (i < _cpoints.GetCount() - 1)
                {
                    _cpoints.Remove(i);
                    _mainWindowViewModel.PtIndex = i;
                }
                else if (i == _cpoints.GetCount() - 1)
                {
                    _cpoints.Remove(i);
                    _mainWindowViewModel.PtIndex = _cpoints.LastPoint();
                }
                else if (i >= _cpoints.GetCount() - 1)
                {
                    // error index, so do nothing
                }
            }
            else
            {
                return;
            }

            DisplayPointList();
        }

        public void CancelChoose()
        {
            _cpoints?.Clear();
            _cpoints = new PointListData(_workPackage.GetPointList());
            _cpoints.SetPointNum();
            if (_mainWindowViewModel != null)
            {
                _mainWindowViewModel.PtIndex = 0;
                _mainWindowViewModel.PointList.Clear();
                DisplayPointList();
            }
        }

        public void OkChoose()
        {
            if (_cpoints != null && _cpoints.GetCount() > 0)
            {
                _workPackage.SetPointList(_cpoints);
            }
            else
            {
                _workPackage.ClearPointList();
            }

            if (_workPackage != null)
            {
                BasicWorkFile bwf = new();
                bwf.Save(_workPackage);
            }
        }
    }
}
