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
//           (2) the common references in Main Model are workpackage,
//               which contain the data, and the motion bus, which
//               which keep only one motion borad be used.
//
///////////////////////////////////////////////////////////////////////

using Microsoft.Win32;
using RobotWeld3.AlgorithmsBase;
using RobotWeld3.CaluWMS;
using RobotWeld3.GetTrace;
using RobotWeld3.Motions;
using RobotWeld3.View;
using RobotWeld3.ViewModel;
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.AccessControl;

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// The main model to connect the view page with the action center.
    /// </summary>
    public class MainModel
    {
        private MainWindowViewModel? _mainWindowViewModel;
        private static MotionBus? _mbus;
        private TakeTrace? _takeTrace;
        private WorkPackage? _workPackage;
        private bool _pmFlag;
        private int _wmsTimeStamp;
        private List<WeldMoveSection> _wms;

        private static List<string>? errMsg;

        // because it has several place to replot the figure,
        // and the figure must be only one instance, so it is
        // a class property.
        private FreeCurve? _freeCurve;
        private PointListModel? _ptModel;

        /// <summary>
        /// Initial functions: construct, config, and ReadRecord
        /// </summary>
        public MainModel()
        {
            _wms = new List<WeldMoveSection>();
            errMsg = new List<string>();
        }

        /// <summary>
        /// Give MainWindowsViewModel to the class
        /// </summary>
        /// <param name="mvm"></param>
        public void PutViewModel(MainWindowViewModel mvm)
        {
            _mainWindowViewModel = mvm;
        }

        /// <summary>
        /// Initialize machine and home page.
        /// </summary>
        public void Startup()
        {
            // Configure Machine
            int mp = MaxPowerFile.GetMaxPower();
            MotionOperate.SetMaxPower(mp);
            MotionSpecification.ReadParameter();
            _mbus = new MotionBus();
            _takeTrace = new TakeTrace(_mbus);
            _mbus.InitialCard();

            // read record
            _workPackage = AccessWorkpackageFile.Open();
            ConfigHomePage();
        }

        /// <summary>
        /// call back the reference of point list model 
        /// </summary>
        /// <param name="ptm"></param>
        internal void PutPointListModel(PointListModel ptm)
        {
            _ptModel = ptm;
        }

        /// <summary>
        /// updata functions when change trace type
        /// Refresh the point list manually.
        /// </summary>
        private void UpdateRecord()
        {
            if (_workPackage != null && _ptModel != null)
            {
                AccessWorkpackageFile.UpdateWorkPackage(ref _workPackage);
                _ptModel.UpdateWorkPackage(_workPackage);
            }
        }

        /// <summary>
        /// Chose suitable homepage to be shown in left windows.
        /// </summary>
        private void ConfigHomePage()
        {
            if (_workPackage == null || _mainWindowViewModel == null) return;

            _mainWindowViewModel.SelectedTracePlot = _workPackage.Tracetype switch
            {
                Tracetype.INTERSECT => new WeldPlatformIntersect(),
                Tracetype.STAGE_TRACE => new WeldPlatformStage(),
                Tracetype.SPIRAL => new WeldPlatformSpiral(),
                _ => new WeldPlatform(),
            };
        }

        /// <summary>
        /// Return the workpackage used in the main model
        /// </summary>
        /// <returns> WorkPackage </returns>
        internal WorkPackage GetWorkPackage()
        {
            return _workPackage ?? new WorkPackage();
        }

        /// <summary>
        /// Return the motion bus used in the main model.
        /// </summary>
        /// <returns> MotionBus </returns>
        internal MotionBus GetMotionBus()
        {
            return _mbus ??= new MotionBus();
        }

        /// <summary>
        /// Return the TakeTrace reference in the main model.
        /// </summary>
        /// <returns></returns>
        internal TakeTrace GetTakeTrace()
        {
            if (_mbus == null) return new TakeTrace(new MotionBus());
            return _takeTrace ??= new TakeTrace(_mbus);
        }

        /// <summary>
        /// Response functions for button clicking
        /// </summary>
        public void NewFile()
        {
            _workPackage = AccessWorkpackageFile.NewFile();
        }

        /// <summary>
        /// Save as menu to new file, new file index.
        /// </summary>
        public void SaveDialog()
        {
            var dialog = new SaveFileDialog()
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
                if (_workPackage != null)
                    AccessWorkpackageFile.Save(filename, in _workPackage);
            }
        }

        /// <summary>
        /// Open stored record file.
        /// </summary>
        public void OpenDialog()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Weld Data files (*.wfd)|*.wfd",
                Title = "打开焊接文件",
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = openFileDialog.ShowDialog();

            if (rst == true)
            {
                string filename = openFileDialog.FileName;
                _workPackage = AccessWorkpackageFile.Open(filename);
            }
        }

        /// <summary>
        /// Save the current trace type to workpacke file.
        /// </summary>
        public void CloseWindow()
        {
            Tracetype typ;
            if (_workPackage != null) typ = _workPackage.Tracetype;
            else typ = Tracetype.FREE_TRACE;

            _mbus?.SetWobble(0);
            AccessWorkpackageFile.FreshCurrent(typ);

        }

        /// <summary>
        /// Reset the motion card to the original positions.
        /// </summary>
        public void ResetCard()
        {
            if (_pmFlag) return;
            _mbus?.RunResetAxis();
        }

        /// <summary>
        /// Turn on the laser by clicking the mouse left button.
        /// </summary>
        public void JogLightOn()
        {
            if (_pmFlag) return;

            MotionBus.StopAllThread();
            Thread.Sleep(5);
            if (_workPackage == null || _workPackage.PointList.Count < 2) return;

            var pt = _workPackage.PointList;
            int power = pt[1].Power;
            MotionOperate.OpenAir();
            _mbus?.OpenLaser(power);
        }

        /// <summary>
        /// Turn off the laser by releasing mouse left button.
        /// </summary>
        public void JogLightOff()
        {
            if (_pmFlag) return;
            MotionBus.StopAllThread();

            MotionOperate.CloseAir();
            LaserDriver.LaserOff();
        }

        /// <summary>
        /// When select TabControl item - VideoShow, back to the work 
        /// page, but do not triggle the working prepared state.
        /// </summary>
        public void SelectItemVideo()
        {
            if (_workPackage == null || _mainWindowViewModel == null) return;

            // switch the plot view
            if (_workPackage.Tracetype == Tracetype.INTERSECT)
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatformIntersect();
            else if (_workPackage.Tracetype == Tracetype.SPIRAL)
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatformSpiral();
            else if (_workPackage.Tracetype == Tracetype.FREE_TRACE)
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatform();

            _takeTrace?.StopHandwheel();
        }

        /// <summary>
        /// When select TabControl item - PointList, back to the config
        /// trace, and unset the working prepared state.
        /// </summary>
        public void SelectItemPoint()
        {
            PreparedState(false);

            if (_workPackage == null || _mainWindowViewModel == null) return;

            var tracetype = _workPackage.Tracetype;

            switch (tracetype)
            {
                case Tracetype.INTERSECT:
                    _mainWindowViewModel.SelectedTracePlot = new Intersect3D();
                    break;
                case Tracetype.STAGE_TRACE:
                    break;
                case Tracetype.SPIRAL:
                    _mainWindowViewModel.SelectedTracePlot = new SpiralTrace();
                    break;
                case Tracetype.FREE_TRACE:
                    _freeCurve ??= new FreeCurve();
                    _mainWindowViewModel.SelectedTracePlot = _freeCurve;
                    _freeCurve.EstablishList(_workPackage.PointList.Ptlist);
                    break;
                default:
                    break;
            }
            _takeTrace?.OpenHandwheel();
        }

        /// <summary>
        /// Run weld trace for different traces.
        /// </summary>
        /// <param name="tracetype"> trace type </param>
        public void WeldTrace()
        {
            if (_workPackage == null || _mainWindowViewModel == null) return;

            // switch the plot view
            if (_workPackage.Tracetype == Tracetype.INTERSECT)
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatformIntersect();
            else if (_workPackage.Tracetype == Tracetype.SPIRAL)
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatformSpiral();
            else if (_workPackage.Tracetype == Tracetype.FREE_TRACE)
                _mainWindowViewModel.SelectedTracePlot = new WeldPlatform();

            DoWeldTrace();
        }

        /// <summary>
        /// prepare and ready to weld.
        /// The method include three steps:
        ///   (1) get wms from the point List of workpackage
        ///   (2) set the parameter of runtrace from wms.
        ///   (3) initial the trigger to be run.
        /// </summary>
        private void DoWeldTrace()
        {
            MotionBus.StopAllThread();
            if (_workPackage == null || _pmFlag) return;
            PreparedState(true);

            if (_wmsTimeStamp != _workPackage.TimeStamp)
            {
                var r = new ResolveWms();
                r.GetWmsList(_workPackage, ref _wms);
                _wmsTimeStamp = _workPackage.TimeStamp;
            }

            if (_mbus == null) return;
            var runTrace = new RunTrace(_mbus);
            runTrace.StartRunTrace(in _wms);
            runTrace.Run();
        }

        /// <summary>
        /// Configure the trace point, which is independent of trace type.
        /// </summary>
        /// <param name="tracetype"> The view changed according to trace Type </param>
        public void ConfigTrace(Tracetype tracetype)
        {
            PreparedState(false);
            MotionBus.StopAllThread();
            if (_workPackage == null || _mainWindowViewModel == null) return;

            _workPackage.Tracetype = tracetype;

            switch (tracetype)
            {
                case Tracetype.INTERSECT:
                    _mainWindowViewModel.SelectedTracePlot = new Intersect3D();
                    UpdateRecord();
                    break;
                case Tracetype.STAGE_TRACE:
                    break;
                case Tracetype.SPIRAL:
                    _mainWindowViewModel.SelectedTracePlot = new SpiralTrace();
                    UpdateRecord();
                    break;
                case Tracetype.FREE_TRACE:
                    _freeCurve ??= new FreeCurve();
                    UpdateRecord();
                    _mainWindowViewModel.SelectedTracePlot = _freeCurve;
                    _freeCurve.EstablishList(_workPackage.PointList.Ptlist);
                    break;
                default:
                    break;
            }

            ConfigTrace();

            _takeTrace?.OpenHandwheel();
        }

        /// <summary>
        /// Re-plot the trace UI in free curve scenario.
        /// </summary>
        internal void RePlotFreeCurve(ObservableCollection<DisPoint> cpt)
        {
            if (_workPackage == null) return;

            var pt = new List<DisPoint>(cpt);
            if (_workPackage.Tracetype == Tracetype.FREE_TRACE && _freeCurve != null)
                _freeCurve.EstablishList(pt);
        }

        /// <summary>
        /// Configure each trace.
        /// </summary>
        private void ConfigTrace()
        {
            if (_workPackage != null && _workPackage.Tracetype == Tracetype.INTERSECT)
            {
                var its = new Intersect();
                its.PutWorkPackage(_workPackage);
                its.ShowDialog();
            }

            if (_workPackage != null && _workPackage.Tracetype == Tracetype.SPIRAL)
            {
                var ist = new InputSpiral();
                ist.PutWorkPackage(_workPackage);
                ist.ShowDialog();
            }
        }

        /// <summary>
        /// Setup weld parameter beyond the default value.
        /// </summary>
        public void SuperUserSettings()
        {
            if (_workPackage == null || _mbus == null) return;

            MotionBus.StopAllThread();
            PreparedState(false);
            var s = new SuperUser(_workPackage.TimeStamp);
            s.PutMotionBus(_mbus);
            s.ShowDialog();
        }

        /// <summary>
        /// Setup the lase type and manufactory.
        /// </summary>
        public void SetupLaserType()
        {
            MotionBus.StopAllThread();
            PreparedState(false);
            new LaserType().ShowDialog();
        }

        /// <summary>
        /// Calibrate the machine to find the limit and direction.
        /// </summary>
        public void CalibrateMachine()
        {
            MotionBus.StopAllThread();
            PreparedState(false);

            if (_mbus != null)
            {
                var cbmd = new CalibrateMachineDialog();
                cbmd.PutMotionOperate(_mbus);
                cbmd.ShowDialog();
            }
        }

        /// <summary>
        /// To indicate the run state or get point state
        /// </summary>
        /// <param name="bState"></param>
        private void PreparedState(bool bState)
        {
            if (_mainWindowViewModel == null) return;

            if (!bState)
            {
                _pmFlag = false;
                _mainWindowViewModel.PmFlag = true;
            }
            else
            {
                _pmFlag = true;
                _mainWindowViewModel.PmFlag = false;
            }
        }

        /// <summary>
        /// Show the XYZ positon in the menu bar
        /// </summary>
        /// <param name="x"> X position in mm </param>
        /// <param name="y"> Y position in mm </param>
        /// <param name="z"> Z position in mm </param>
        /// <param name="a"> A position in degree </param>
        public void SetXYZ(int x, int y, int z, int a)
        {
            if (_mainWindowViewModel == null) return;

            _mainWindowViewModel.X = x / MotionOperate.Xmillimeter;
            _mainWindowViewModel.Y = y / MotionOperate.Ymillimeter;
            _mainWindowViewModel.Z = z / MotionOperate.Zmillimeter;
            _mainWindowViewModel.A = 360.0 * a / MotionOperate.OneCycle;
        }

        /// <summary>
        /// Add notice message in the UI page.
        /// </summary>
        /// <param name="msg"></param>
        public static void AddInfo(string msg)
        {
            if (errMsg == null) return;

            DateTime DateNow = DateTime.Now;
            var showMsg = string.Empty;
            var smg = DateNow.ToString() + ", " + msg;
            if (errMsg.Count >= 3)
            {
                errMsg.RemoveAt(0);
            }
            errMsg.Add(smg);
            for (int i = 0; i < errMsg.Count; i++)
                showMsg += errMsg[i] + "\n";

            MainWindowViewModel.ErrMsg = showMsg;
        }

        /// <summary>
        /// working count monitor during each machine running.
        /// </summary>
        /// <param name="ic"></param>
        internal static void AddCount(int ic)
        {
            MainWindowViewModel.WorkCount += ic;
        }
    }
}
