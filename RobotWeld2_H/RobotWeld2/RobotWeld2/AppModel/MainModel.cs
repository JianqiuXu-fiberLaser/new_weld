using Microsoft.Win32;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RobotWeld2.AppModel
{
    public class MainModel
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private DaemonFile? dmFile;
        private WorkPackage workPackage;
        private readonly MotionOperate? _mo;

        //private ExtraController? extraController;
        private RunTrace? runTrace;
        private readonly TakeTrace? takeTrace;
        private IntersectModel? intersectModel;

        private PointListData? copyPoints;

        public MainModel(MainWindowViewModel mvm)
        {
            mainWindowViewModel = mvm;
            mainWindowViewModel.SelectedTracePlot = new WeldPlatform();
            workPackage = new();
            _mo = new MotionOperate();
            _mo.InitialCard();

            takeTrace = new TakeTrace();
            runTrace = new RunTrace();
        }

        public void ConfigMachine()
        {
            BasicWorkFile bwf = new();
            int mp = bwf.GetMaxPower();
            workPackage.SetMaxPower(mp);
        }

        public void ReadRecord()
        {
            BasicWorkFile bwf = new();
            bwf.GetWorkPackage(workPackage);
            PreparedState(false);
            DisplayPointList(workPackage.GetPointList(), 0);
        }

        public void NewFile()
        {
            BasicWorkFile bwf = new();
            if (workPackage != null)
            {
                bwf.NewFile(workPackage);
            }
        }

        public void Close()
        {
            BasicWorkFile bwf = new();
            if (workPackage != null)
            {
                bwf.Save(workPackage);
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
                if (workPackage != null)
                {
                    bwf.Save(workPackage, filename);
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
                if (workPackage != null)
                {
                    bwf.Open(workPackage, filename);
                }
            }
        }

        public void WeldTrace(Tracetype tracetype)
        {

            // switch the plot view
            switch (tracetype)
            {
                case Tracetype.VANE_WHEEL:
                    mainWindowViewModel.SelectedTracePlot = new TraceCanvas();
                    WeldTraceVaneWheel();
                    break;
                case Tracetype.INTERSECT:
                    mainWindowViewModel.SelectedTracePlot = new WeldPlatform();
                    WeldTraceIntersect();
                    break;
                default:
                    mainWindowViewModel.SelectedTracePlot = new WeldPlatform();
                    WeldTraceFreeCurve();
                    break;
            }
        }

        private void WeldTraceVaneWheel()
        {
            //Tracetype tpy = Tracetype.VANE_WHEEL;
            if (dmFile != null && !dmFile.PmFlag)
            {
                dmFile.PreparedState(true);
/*                if (runTrace != null && _mo != null && points != null)
                {
                    runTrace.StartRunTrace(dmFile, _mo);
                    runTrace.Run(tpy, new List<Point>());
                }*/
            }
        }

        private void WeldTraceIntersect()
        {
            MotionOperate.StopAllThread();
            Tracetype tpy = Tracetype.INTERSECT;
            if (!mainWindowViewModel.PmFlag)
            {
                PreparedState(true);

                if (intersectModel == null)
                {
                    intersectModel ??= new IntersectModel();
                    intersectModel.GetParameter(workPackage);
                }
                
                if (_mo != null && runTrace != null && workPackage != null)
                {
                    runTrace.StartRunTrace(workPackage, intersectModel, this, _mo);
                    runTrace.Run(tpy);
                }
            }
        }

        private void WeldTraceFreeCurve()
        {
            Tracetype tpy = Tracetype.FREE_TRACE;
            if (!mainWindowViewModel.PmFlag)
            {
                PreparedState(true);
                if (_mo != null && runTrace != null && workPackage != null)
                {
                    //runTrace.StartRunTrace(workPackage, this, _mo);
                    runTrace.Run(tpy);
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
                    ConfigTraceVaneWheel();
                    break;
                case Tracetype.INTERSECT:
                    mainWindowViewModel.SelectedTracePlot = new Intersect3D();
                    ConfigTraceIntersect();
                    break;
                default:
                    break;
            }

            MotionOperate.StopAllThread();
            SetPointIndex(0);

            if (_mo != null && workPackage != null && takeTrace != null)
            {
                takeTrace.OpenHandwheel(_mo, this);

                // copy Points stored the point's information during taking trace.
                copyPoints = new PointListData();
                copyPoints.Clear();
                copyPoints.CopyList(workPackage.GetPointList());
                DisplayPointList(copyPoints, GetPointIndex());
                PreparedState(false);
            }
        }

        private void ConfigTraceVaneWheel()
        {
            if (dmFile != null)
            {
                VaneWheel vw = new(dmFile);
                vw.ShowDialog();
            }
            else
            {
                VaneWheel vw = new();
                vw.ShowDialog();
            }
        }

        private void ConfigTraceIntersect()
        {
            intersectModel ??= new IntersectModel();
            intersectModel.GetParameter(workPackage);

            if (workPackage != null)
            {
                Intersect its = new(intersectModel);
                its.ShowDialog();
            }
        }

        public void SetupMaterial()
        {
            if (workPackage != null)
            {
                SelectMaterial st = new(workPackage);
                st.ShowDialog();
            }
        }

        public void Userparameter()
        {
            InputLaserParameterModel inputLaserParameterModel = new(workPackage);
            InputLaserParameter ilp = new(inputLaserParameterModel);
            ilp.ShowDialog();
        }

        public void SetupLaserType()
        {
            LaserType lt = new(workPackage);
            lt.ShowDialog();
        }

        public void ResetCard()
        {
            PreparedState(false);
            _mo?.RunResetAxis();
        }

        public void JogLightOn()
        {
            _mo?.InitialLaser(0);
            Thread.Sleep(5);

            if (workPackage != null)
            {
                PointListData pt = workPackage.GetPointList();
                _mo?.SetupLaserParameter(pt.GetPoint(0).Parameter, workPackage.GetMaxPower());
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
            int[] ptxyz = new int[4];
            int pd = GetPointIndex();

            if (copyPoints != null)
            {
                if (pd < copyPoints.GetCount())
                {
                    ptxyz[0] = (int)copyPoints.GetPoint(pd).vector.X;
                    ptxyz[1] = (int)copyPoints.GetPoint(pd).vector.Y;
                    ptxyz[2] = (int)copyPoints.GetPoint(pd).vector.Z;
                    ptxyz[3] = (int)copyPoints.GetPoint(pd).A;
                    DisplayPointList(copyPoints, pd);
                }
                else
                {
                    SetPointIndex(pd++);
                    AddPoint(0, 0, 0, 0);
                    DisplayPointList(copyPoints, pd);
                    return;
                }
            }

            SingleStep ss = new();
            if (_mo != null && workPackage != null)
            {
                ss.SetupParameter(_mo, 30);
                ss.GotoPosition(ptxyz);
            }
        }

        private int GetPointIndex()
        {
            return mainWindowViewModel.PointInfo;
        }

        private void SetPointIndex(int pd)
        {
            mainWindowViewModel.PointInfo = pd;
        }

        //
        // To indicate the run state or get point state
        //
        private void PreparedState(bool bState)
        {
            if (!bState)
            {
                mainWindowViewModel.PmText = "待机....";
                mainWindowViewModel.PmFlag = false;
            }
            else
            {
                mainWindowViewModel.PmText = "准备运行";
                mainWindowViewModel.PmFlag = true;
            }
        }

        public void OkChoose()
        {
            if (copyPoints != null)
            {
                workPackage.SetPointList(copyPoints);
            }
        }

        public void CancelChoose()
        {
            DisplayPointList(workPackage.GetPointList(), 0);
        }

        private void DisplayPointList(PointListData pld, int ptIndex)
        {
            mainWindowViewModel.PointList = pld.DisplayPoint();
            mainWindowViewModel.ptIndex = ptIndex;
        }

        public void AddPoint(int x, int y, int z, int a)
        {
            int lt;
            if (mainWindowViewModel.LineType)
                lt = 1;
            else
                lt = 0;

            int ls;
            if (mainWindowViewModel.LaserOnOff)
                ls = 0;
            else
                ls = 1;

            if (workPackage is not null)
            {
                Vector vc = new(x, y, z);
                List<Point> ptlist = new(mainWindowViewModel.PointList);
                int pdindex = GetPointIndex();
                LaserWeldParameter lp = ptlist[pdindex].Parameter;

                Point onept = new(lt, ls, vc, a, lp);

                if (copyPoints != null)
                {
                    if (GetPointIndex() < copyPoints.GetCount())
                    {
                        copyPoints.ChangePoint(pdindex, onept);
                        int tmpvar = GetPointIndex();
                    }
                    else
                    {
                        copyPoints.Add(onept);
                    }

                    DisplayPointList(copyPoints, GetPointIndex());
                }
            }
        }

        public void ChangeLinetype()
        {
            mainWindowViewModel.LineType = !mainWindowViewModel.LineType;
        }

        public void ChangeLaseronoff()
        {
            mainWindowViewModel.LaserOnOff = !mainWindowViewModel.LaserOnOff;
        }

        public void SetXYZ(int x, int y, int z)
        {
            mainWindowViewModel.X = x / MotionOperate.OneMillimeter;
            mainWindowViewModel.Y = y / MotionOperate.OneMillimeter; ;
            mainWindowViewModel.Z = z / MotionOperate.OneMillimeter; ;
        }

        public void SetXYZ(int x, int y, int z, int a)
        {
            mainWindowViewModel.X = x / MotionOperate.OneMillimeter; ;
            mainWindowViewModel.Y = y / MotionOperate.OneMillimeter; ;
            mainWindowViewModel.Z = z / MotionOperate.OneMillimeter; ;
            mainWindowViewModel.A = 360.0 * a / MotionOperate.OneCycle; ;
        }
    }
}
