///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.AppModel
{
    public class DaemonModel
    {
        private readonly DaemonFile _dmFile;
        private readonly MainWindowViewModel _mainViewModel;
        private readonly ExtraController _extraController;
        private readonly MotionOperate _mo;

        private readonly RunTrace _runTrace;
        private readonly TakeTrace _takeTrace;

        private static int _traceCount;
        public const int TracePoint = 7;
        public const double VaneRadius = 23;
        public const int XDir = 1;
        public const int YDir = -1;
        public const int ZDir = -1;

        private int _traceIndex;
        private Dictionary<int, List<Point>>? _points;

        private bool _isChanged;

        public DaemonModel(MainWindowViewModel mvm)
        {
            _mainViewModel = mvm;
            _dmFile = new DaemonFile(mvm);

            _extraController = new(_dmFile, _mainViewModel);
            _runTrace = new();
            _takeTrace = new();
            _mo = new MotionOperate();
        }

        public static int TraceCount
        {
            get => _traceCount;
            private set => _traceCount = value;
        }

        /// <summary>
        /// setup system parameters at the start, 
        /// initial PLC ans show the configure message
        /// </summary>
        public void SetupParameter()
        {
            _mo.InitialCard();
            _mainViewModel.SelectedTracePlot = new TraceCanvas();
            _traceIndex = MaintainPoints.GetTraceIndex();
            GetPoints();

            _dmFile.TraceIndex = _traceIndex;
            _mainViewModel.TraceInfo = 1;
            _mainViewModel.TraceCount = _traceCount;
            if (_points != null) _dmFile.DisplayPointList(_points);
            _dmFile.PreparedState(false);

            _extraController.ConnectPLC();
            Thread.Sleep(100);
            _extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);

            _dmFile.FreshMsg();
        }

        //
        //---- transfer methods to DeamonFile class ----
        //

        internal void SetParameter()
        {
            _dmFile.SetParameter();
        }

        internal void AddMsg(string s)
        {
            _dmFile.AddMsg(s);
        }

        internal void Close()
        {
            _dmFile.Close();
        }

        internal void SaveDialog()
        {
            _dmFile.SaveDialog();
        }

        internal int GetTraceIndex()
        {
            return _dmFile.TraceIndex;
        }

        internal void PreparedState(bool s)
        {
            _dmFile.PreparedState(s);
        }

        internal void SetLaserParameter(InputViewModel ivm)
        {
            _dmFile.SetLaserParameter(ivm);
        }

        internal void GetLaserParameter(InputViewModel ivm)
        {
            _dmFile.GetLaserParameter(ivm);
        }

        /// <summary>
        /// Weld trace, waiting hand switch signal.
        /// </summary>
        /// <param name="tpy"></param>
        public void WeldTrace(Tracetype tpy)
        {
            if (!_dmFile.PmFlag)
            {
                _dmFile.PreparedState(true);
                if (_runTrace != null && _mo != null && _points != null)
                {
                    _runTrace.StartRunTrace(_dmFile, _mo);
                    _runTrace.Run(tpy, _points);
                }
            }
        }

        public void NewFile()
        {
            _traceIndex = MaintainPoints.NewFile();

            if (_points == null) return;
            MaintainPoints.NewPoints(_mainViewModel.TraceCount, ref _points);

            _mainViewModel.TraceInfo = 0;
            _dmFile.DisplayPointList(_points);
            _dmFile.PreparedState(false);
            _mainViewModel.PtIndex = 0;
        }

        public void OpenFile()
        {
            _points ??= new();
            int t = _dmFile.OpenDialog();

            if (t != 0) _traceIndex = t;
            var baseTreatPoint = new BaseTreatPoint();
            baseTreatPoint.GetPoints(_traceIndex, ref _points);
        }

        //
        // Get the point's list from the trace data file
        //
        private void GetPoints()
        {
            _points = new();
            var baseTreatPoint = new BaseTreatPoint();
            _traceCount = baseTreatPoint.GetPoints(_traceIndex, ref _points);
        }

        public void ResetCard()
        {
            _dmFile.PreparedState(false);
            _mo?.RunResetAxis();
        }

        public void JogLightOn()
        {
            _mo?.InitialLaser();
            Thread.Sleep(5);
            int laserPower = _dmFile.GetPointPower();
            _mo?.SetupLaserParameter(laserPower);
            _mo?.OpenAir();
            _mo?.LaserOnNoRise();
        }

        public void JogLightOff()
        {
            _mo?.CloseAir();
            _mo?.LaserOffNoFall();
        }

        //
        // NEW: methods from HERE
        //

        public void TakePoints()
        {
            MotionOperate.StopAllThread();

            if (_mainViewModel.TraceDir)
                _mainViewModel.SelectedTracePlot = new TraceBack();
            else _mainViewModel.SelectedTracePlot = new TraceFront();

            if (_dmFile != null && _points != null)
            {
                _dmFile.DisplayPointList(_points);
                _dmFile.PreparedState(false);
                _mainViewModel.PtIndex = 0;
            }

            if (_mo != null && _takeTrace != null)
                _takeTrace.OpenHandwheel(_mo, this);
        }

        /// <summary>
        /// Change point display for selection of different trace
        /// </summary>
        internal void ShowTraceInfo()
        {
            bool ret = true;
            if (_isChanged)
            {
                if (ret = Promption.Prompt("坐标改动，保存?"))
                {
                    SelectionOk();
                }
            }

            if (_points != null && ret)
            {
                _dmFile.DisplayPointList(_points);
                _mainViewModel.PtIndex = 0;
            }
        }

        internal void ChangeTraceDir()
        {
            if (_mainViewModel.TraceDir)
                _mainViewModel.SelectedTracePlot = new TraceBack();
            else _mainViewModel.SelectedTracePlot = new TraceFront();

            if (_points != null)
            {
                _dmFile.DisplayPointList(_points);
                _mainViewModel.PtIndex = 0;
            }
        }

        public void SetTraceCount()
        {
            _traceCount = _mainViewModel.TraceCount;
            if (_points == null) return;

            if (_traceCount != (_points.Count - 1) / 2)
                MaintainPoints.Modification(_traceCount, ref _points);
        }

        public void GotoSingleStep()
        {
            int[] ptxyz = new int[3];

            // goto or add a new point?
            if (_dmFile.GetPointIndex(ref ptxyz, out double speed))
            {
                var ss = new SingleStep();
                if (_mo != null)
                {
                    ss.SetupParameter(_mo, speed);
                    ss.GotoPosition(ptxyz);
                }
            }
            else
            {
                // do nothing
            }
        }

        //
        //---- Operate point coordinate
        //

        public void AddPoint()
        {
            _isChanged = true;
            _dmFile.AddPoint(_mainViewModel.X, _mainViewModel.Y, _mainViewModel.Z);
        }

        public void SetXYZ(int x, int y, int z)
        {
            _dmFile.SetXYZ(x, y, z);
        }

        public void SelectionOk()
        {
            var encryp = new Encryption();
            var psw = new PassWord();

            psw.GetEncryption(encryp);
            psw.ShowDialog();

            if (encryp.PasswordOk)
            {
                MotionOperate.StopAllThread();
                _mo?.ExitHandwheel();

                var pl = _dmFile.GetCopyPoint();
                if (pl != null && pl.Count > 0 && _points != null)
                {
                    int itr;
                    if (!_mainViewModel.TraceDir) itr = _mainViewModel.TraceInfo;
                    else itr = 0 - _mainViewModel.TraceInfo;

                    MaintainPoints.RevisePoints(itr, in pl, ref _points);
                    SaveTrace();
                    _isChanged = false;
                }
            }
        }

        public void SaveTrace()
        {
            var baseTreatPoint = new BaseTreatPoint();
            if (_points != null) baseTreatPoint.SavePoints(_traceIndex, _points);
        }

        public void CancelChoose()
        {
            _dmFile.CancelChoose();
        }
    }
}
