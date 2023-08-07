using Microsoft.Win32;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System;
using System.IO;
using System.Threading;

namespace RobotWeld2.AppModel
{
    public class MainModel
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly MotionOperate _mo;
        private readonly WorkPackage _workPackage;

        //-- hardware class
        //private ExtraController? extraController;
        private readonly RunTrace? _runTrace;
        private readonly TakeTrace? _takeTrace;

        //-- special trace class
        private IntersectModel? _intersectModel;
        private SpiralCurveModel? _spiralCurveModel;
        private StageCurveModel? _stageCurveModel;
        private FreeCurve? _freeCurve;
        private FreeCurveModel? freeCurveModel;

        private VisualPointList? _visualPointList;

        //
        // Initial functions: construct, config, and ReadRecord
        //
        public MainModel(MainWindowViewModel mvm)
        {
            _mainWindowViewModel = mvm;
            _workPackage = new();
            _mo = new MotionOperate();
            _mo.InitialCard();

            _takeTrace = new TakeTrace(_mo);
            _runTrace = new RunTrace(_mo);
        }

        public static void ConfigMachine()
        {
            BasicWorkFile bwf = new();
            int mp = bwf.GetMaxPower();
            MotionOperate.SetMaxPower(mp);

            MotionSpecification ms = new();
            ms.ReadParameter();
        }

        public void ReadRecord()
        {
            BasicWorkFile bwf = new();
            bwf.GetWorkPackage(_workPackage);
            ConfigHomePage();
            RefrashVisual();
        }

        //
        // updata functions when change trace type
        //
        private void UpdateRecord()
        {
            _workPackage.Clear();
            BasicWorkFile bwf = new();
            bwf.UpdateWorkPackage(_workPackage);
            RefrashVisual();
        }

        private void RefrashVisual()
        {
            PreparedState(false);

            // copy Points stored the point's information during taking trace.
            _visualPointList ??= new(_workPackage);
            _visualPointList.PutViewModel(_mainWindowViewModel);
            _visualPointList.RefrashParameter();
        }

        private void ConfigHomePage()
        {
            if (_workPackage == null) return;

            _mainWindowViewModel.SelectedTracePlot = _workPackage.TraceType switch
            {
                Tracetype.VANE_WHEEL => new TraceCanvas(),
                Tracetype.INTERSECT => new WeldPlatformIntersect(),
                Tracetype.STAGE_TRACE => new WeldPlatformStage(),
                Tracetype.SPIRAL => new WeldPlatformSpiral(),
                _ => new WeldPlatform(),
            };
        }

        //
        // Response functions for button clicking
        //
        public void NewFile()
        {
            BasicWorkFile bwf = new();
            if (_workPackage != null)
            {
                bwf.NewFile(_workPackage);
            }
        }

        public void Close()
        {
            BasicWorkFile bwf = new();
            if (_workPackage != null)
            {
                bwf.Save(_workPackage);
            }
        }

        public void SaveDialog()
        {
            SaveFileDialog dialog = new()
            {
                DefaultExt = ".wfd",
                Filter = "WeldData documents (.wfd)|*.wfd",
                AddExtension = true,
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = dialog.ShowDialog();
            if (rst == true)
            {
                string filename = dialog.FileName;
                BasicWorkFile bwf = new();
                if (_workPackage != null)
                {
                    bwf.Save(_workPackage, filename);
                }
            }
        }

        public void OpenDialog()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Weld Data files (*.wfd)|*.wfd",
                Title = "打开焊接文件",
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = openFileDialog.ShowDialog();

            if (rst == true)
            {
                string filename = openFileDialog.FileName;
                BasicWorkFile bwf = new();
                if (_workPackage != null)
                {
                    bwf.Open(_workPackage, filename);
                }
            }
        }

        public void ResetCard()
        {
            PreparedState(false);
            _mo?.RunResetAxis();
        }

        public void JogLightOn()
        {
            _mo?.SetLaserMode();
            Thread.Sleep(5);

            if (_workPackage != null)
            {
                PointListData pt = _workPackage.GetPointList();
                int i = _mainWindowViewModel.PtIndex;
                _mo?.SetupLaserParameter(pt.GetPoint(i).Parameter);
                _mo?.OpenAir();
                _mo?.LaserOnNoRise();
            }
        }

        public void JogLightOff()
        {
            _mo?.CloseAir();
            _mo?.LaserOffNoFall();
        }

        public void GotoSingleStep()
        {
            if (_visualPointList != null)
            {
                _visualPointList.GetXYZA(out int[] ptxyz);
                SingleStep ss = new(_mo);
                ss.SetupParameter(30);
                ss.GotoPosition(ptxyz);
            }
        }

        public void OkChoose()
        {
            _visualPointList?.OkChoose();
        }

        public void CancelChoose()
        {
            if (_visualPointList != null)
            {
                _visualPointList.CancelChoose();
                _freeCurve?.EstablishList(_visualPointList);
            }
        }

        /// <summary>
        /// Run weld trace for different traces.
        /// </summary>
        /// <param name="tracetype"></param>
        public void WeldTrace()
        {
            // switch the plot view
            if (_workPackage.TraceType == Tracetype.VANE_WHEEL)
            {
                _mainWindowViewModel.SelectedTracePlot = new TraceCanvas();
                WeldTraceVaneWheel();
            }
            else if (_workPackage.TraceType == Tracetype.INTERSECT)
            {
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatformIntersect();
                WeldTraceIntersect();
            }
            else if (_workPackage.TraceType == Tracetype.STAGE_TRACE)
            {
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatformStage();
                WeldStageCurve();
            }
            else if (_workPackage.TraceType == Tracetype.SPIRAL)
            {
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatformSpiral();
                WeldSpiralCurve();
            }
            else if (_workPackage.TraceType == Tracetype.FREE_TRACE)
            {
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatform();
                WeldTraceFreeCurve();
            }
        }

        private void WeldTraceVaneWheel()
        {
            //Tracetype tpy = Tracetype.VANE_WHEEL;
        }

        private void WeldTraceIntersect()
        {
            MotionOperate.StopAllThread();
            Tracetype tpy = Tracetype.INTERSECT;
            if (!_mainWindowViewModel.PmFlag)
            {
                PreparedState(true);

                _intersectModel ??= new IntersectModel();
                _intersectModel?.GetParameter(_workPackage);

                if (_runTrace != null && _workPackage != null && _intersectModel != null)
                {
                    _runTrace.StartRunTrace(_workPackage, _intersectModel);
                    _runTrace.Run(tpy);
                }
            }
        }

        private void WeldSpiralCurve()
        {
            MotionOperate.StopAllThread();
            Tracetype tpy = Tracetype.SPIRAL;
            if (!_mainWindowViewModel.PmFlag)
            {
                PreparedState(true);

                if (_spiralCurveModel == null)
                {
                    _spiralCurveModel ??= new SpiralCurveModel();
                    _spiralCurveModel.GetParameter(_workPackage);
                }

                if (_runTrace != null && _workPackage != null)
                {
                    _runTrace.StartRunTrace(_workPackage, _spiralCurveModel);
                    _runTrace.Run(tpy);
                }
            }
        }

        private void WeldStageCurve()
        {
            MotionOperate.StopAllThread();
            Tracetype tpy = Tracetype.STAGE_TRACE;
            if (!_mainWindowViewModel.PmFlag)
            {
                PreparedState(true);

                if (_stageCurveModel == null)
                {
                    _stageCurveModel ??= new StageCurveModel();
                }

                if (_runTrace != null && _workPackage != null)
                {
                    _runTrace.StartRunTrace(_workPackage, _stageCurveModel);
                    _runTrace.Run(tpy);
                }
            }
        }

        private void WeldTraceFreeCurve()
        {
            MotionOperate.StopAllThread();
            Tracetype tpy = Tracetype.FREE_TRACE;
            if (!_mainWindowViewModel.PmFlag)
            {
                PreparedState(true);

                if (freeCurveModel == null)
                {
                    freeCurveModel ??= new FreeCurveModel();
                }

                if (_runTrace != null && _workPackage != null && freeCurveModel != null)
                {
                    _runTrace.StartRunTrace(_workPackage, freeCurveModel);
                    _runTrace.Run(tpy);
                }
            }
        }

        /// <summary>
        /// Configure the trace point, which is independent of trace type.
        /// </summary>
        /// <param name="tracetype"> The view changed according to trace Type </param>
        public void ConfigTrace(Tracetype tracetype)
        {
            switch (tracetype)
            {
                case Tracetype.VANE_WHEEL:
                    _workPackage.TraceType = Tracetype.VANE_WHEEL;
                    UpdateRecord();
                    ConfigTraceVaneWheel();
                    break;
                case Tracetype.INTERSECT:
                    _workPackage.TraceType = Tracetype.INTERSECT;
                    UpdateRecord();
                    _mainWindowViewModel.SelectedTracePlot = new Intersect3D();
                    ConfigTraceIntersect();
                    break;
                case Tracetype.STAGE_TRACE:
                    _workPackage.TraceType = Tracetype.STAGE_TRACE;
                    UpdateRecord();
                    _mainWindowViewModel.SelectedTracePlot = new StageTrace();
                    break;
                case Tracetype.SPIRAL:
                    _workPackage.TraceType = Tracetype.SPIRAL;
                    UpdateRecord();
                    _mainWindowViewModel.SelectedTracePlot = new SpiralTrace();
                    ConfigTraceSpiral();
                    break;
                default:
                    _workPackage.TraceType = Tracetype.FREE_TRACE;
                    UpdateRecord();
                    _freeCurve = new FreeCurve();
                    if (_visualPointList != null)
                    {
                        _freeCurve.EstablishList(_visualPointList);
                    }
                    _mainWindowViewModel.SelectedTracePlot = _freeCurve;
                    break;
            }

            MotionOperate.StopAllThread();
            SetPointIndex(0);

            if (_mo != null && _takeTrace != null)
            {
                _takeTrace.OpenHandwheel(this);
            }
        }

        private void ConfigTraceVaneWheel()
        {
        }

        private void ConfigTraceIntersect()
        {
            if (_workPackage == null) return;

            _intersectModel ??= new IntersectModel();
            _intersectModel.GetParameter(_workPackage);
            Intersect its = new(_intersectModel);
            its.ShowDialog();
        }

        private void ConfigTraceSpiral()
        {
            if (_workPackage == null) return;

            _spiralCurveModel ??= new SpiralCurveModel();
            _spiralCurveModel.GetParameter(_workPackage);
            InputSpiral ist = new(_spiralCurveModel);
            ist.ShowDialog();
        }

        public void SetupMaterial()
        {
            if (_workPackage == null) return;

            SelectMaterial st = new(_workPackage);
            st.ShowDialog();
        }

        public void Userparameter()
        {
            int i = _mainWindowViewModel.PtIndex;
            InputLaserParameterModel inputLaserParameterModel = new(_workPackage, i);
            InputLaserParameter ilp = new(inputLaserParameterModel);
            ilp.ShowDialog();

            _visualPointList?.RefrashParameter(i);
        }

        public void SetupLaserType()
        {
            LaserType lt = new();
            lt.ShowDialog();
        }

        public void CalibrateMachine()
        {
            CalibrateMachineDialog cbmd = new();
            if (_mo != null)
            {
                cbmd.PutMotionOperate(_mo);
            }
            cbmd.ShowDialog();
        }

        //
        // To indicate the run state or get point state
        //
        private void PreparedState(bool bState)
        {
            if (!bState)
            {
                _mainWindowViewModel.PmText = "待机....";
                _mainWindowViewModel.PmFlag = false;
            }
            else
            {
                _mainWindowViewModel.PmText = "准备运行";
                _mainWindowViewModel.PmFlag = true;
            }
        }

        //
        // Point information for display and treat
        //
        private void SetPointIndex(int pd)
        {
            if (pd < _mainWindowViewModel.PointList.Count)
            {
                _mainWindowViewModel.PtIndex = pd;
            }
            else if (_mainWindowViewModel.PointList.Count > 1)
            {
                _mainWindowViewModel.PtIndex = _mainWindowViewModel.PointList.Count - 1;
            }
            else
            {
                _mainWindowViewModel.PtIndex = 0;
            }
        }

        //
        // POINT : functions to treat point with copyPoint.
        //         Refresh only when they are confirmed by click ok button.
        //
        public void ChangePoint()
        {
            int[] ptos = new int[4];
            _takeTrace?.ReadXYZ(ref ptos);

            if (_visualPointList != null)
            {
                _visualPointList.AddPoint(ptos[0], ptos[1], ptos[2], ptos[3]);
                _freeCurve?.EstablishList(_visualPointList);
            }
        }

        public void DeletePoint()
        {
            if (_visualPointList != null)
            {
                _visualPointList.DeletePoint();
                _freeCurve?.EstablishList(_visualPointList);
            }
        }

        public void SetXYZ(int x, int y, int z, int a)
        {
            _visualPointList?.SetXYZ(x, y, z, a);
        }

        public static void AddInfo(string msg)
        {
            string mFile = "./err.log";
            DateTime DateNow = DateTime.Now;
            string errMsg = DateNow.ToString() + "," + msg;

            try
            {
                FileStream afile = new(mFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new(afile);

                sw.WriteLine(errMsg);
                sw.Close();
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1011);
            }
        }
    }
}
