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
        private readonly DaemonFile dmFile;
        private readonly MainWindowViewModel mainViewModel;
        private ExtraController? extraController;

        private RunTrace? runTrace;
        private TakeTrace? takeTrace;

        private List<Point>? points;
        private Point? chpoints;
        private List<Point>? copyPoints;

        public DaemonModel(DaemonFile dmFile, MainWindowViewModel mainViewModel)
        {
            this.dmFile = dmFile;
            this.mainViewModel = mainViewModel;
        }

        //
        // NEW: ---- new method from HERE
        //
        private MotionOperate? _mo;

        public void SetupParameter()
        {
            points = new List<Point>();
            GetPoints(points);
            dmFile.DisplayPointList(points);
            dmFile.PreparedState(false);

            _mo = new MotionOperate();
            _mo.InitialCard();
            dmFile.FreshMsg();

            runTrace = new();
            takeTrace = new();

            extraController = new(dmFile, mainViewModel);
            extraController.ConnectPLC();
            Thread.Sleep(100);    // waiting for connection.
            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
        }

        public void WeldTrace(Tracetype tpy)
        {
            if (!dmFile.PmFlag)
            {
                dmFile.PreparedState(true);
                if (runTrace != null && _mo != null && points != null)
                {
                    runTrace.StartRunTrace(dmFile, _mo);
                    runTrace.Run(tpy, points);
                }
            }
        }

        //
        // END: ---- new method end
        //

        public void NewFile()
        {
            if (points != null)
                points.Clear();
            else
                points = new List<Point>();
        }

        public void SaveTrace()
        {
            BaseTreatPoint baseTreatPoint = new(dmFile);
            if (points != null)
                baseTreatPoint.SavePoints(points);
        }

        private void SaveTrace(List<Point> wPointList)
        {
            BaseTreatPoint baseTreatPoint = new(dmFile);
            if (wPointList != null)
                baseTreatPoint.SavePoints(wPointList);
        }

        public void OpenFile()
        {
            BaseTreatPoint baseTreatPoint = new(dmFile);
            if (points != null)
                baseTreatPoint.GetPoints(points);
        }

        //
        // Get the point's list from the trace data file
        //
        private void GetPoints(List<Point> points)
        {
            // new file, not need to read point's data
            if (dmFile.TraceCode != 0)
            {
                BaseTreatPoint baseTreatPoint = new(dmFile);

                if (points != null)
                {
                    points.Clear();
                    baseTreatPoint.GetPoints(points);
                }
                else
                    dmFile.AddMsg("存在数据丢失风险");
            }
        }

        public void DisplayList(Point dispt)
        {
            if (copyPoints == null) return;

            if (dmFile.GetPointIndex() <= copyPoints.Count)
            {
                copyPoints[dmFile.GetPointIndex()] = dispt;
            }
            else
            {
                copyPoints.Add(dispt);
            }
            dmFile.DisplayPointList(copyPoints, dmFile.GetPointIndex());
        }

        public void ResetCard()
        {
            dmFile.PreparedState(false);
            _mo?.RunResetAxis();
        }

        public void JogLightOn()
        {
            _mo?.InitialLaser(0);
            Thread.Sleep(5);
            _mo?.SetupLaserParameter(dmFile.laserParameter);
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
            // ChkPre.StopChkThread();

            if (dmFile != null && points != null)
            {
                dmFile.SetPointIndex(0);
            }

            if (_mo != null && dmFile != null && takeTrace != null)
            {
                takeTrace.OpenHandwheel(_mo, dmFile, this);

                // new point recorded in chpoint
                chpoints = new Point();

                // copy Points stored the point's information during taking trace.
                copyPoints ??= new List<Point>();
                copyPoints.Clear();
                if (points != null)
                {
                    for (short i = 0; i < points.Count; i++)
                    {
                        copyPoints.Add(points[i]);
                    }
                }

                dmFile.DisplayPointList(copyPoints, dmFile.GetPointIndex());
                dmFile.PreparedState(false);
            }
        }

        public void GotoSingleStep()
        {
            int[] ptxyz = new int[3];
            int pd = dmFile.GetPointIndex();

            if (copyPoints != null)
            {
                if (pd < copyPoints.Count)
                {
                    ptxyz[0] = (int)copyPoints[pd].vector.X;
                    ptxyz[1] = (int)copyPoints[pd].vector.Y;
                    ptxyz[2] = (int)copyPoints[pd].vector.Z;
                    dmFile.DisplayPointList(copyPoints, pd);
                }
                else
                {
                    dmFile.SetPointIndex(pd++);
                    AddPoint(0, 0, 0);
                    dmFile.DisplayPointList(copyPoints, pd);
                    return;
                }
            }

            SingleStep ss = new(dmFile);
            if (_mo != null)
            {
                ss.SetupParameter(_mo, 0.5 * dmFile.LeapSpeed);
                ss.GotoPosition(ptxyz);
            }
        }

        public void AddPoint(int x, int y, int z)
        {
            int lt;
            if (dmFile != null && dmFile.LineType)
                lt = 0;
            else
                lt = 1;

            int ls;
            if (dmFile != null && dmFile.LaserOnOff)
                ls = 0;
            else
                ls = 1;

            if (dmFile is not null)
            {
                Vector vc = new(x, y, z);
                Point onept = new(lt, ls, dmFile.LaserPower, vc);
                chpoints = onept;

                if (copyPoints != null && points != null)
                {
                    if (dmFile.GetPointIndex() < copyPoints.Count)
                    {
                        copyPoints[dmFile.GetPointIndex()] = onept;
                    }
                    else
                    {
                        copyPoints.Add(onept);
                    }

                    dmFile.DisplayPointList(copyPoints, dmFile.GetPointIndex());
                }
            }
        }

        public void SelectionOk()
        {
            Encryption encryp = new Encryption();
            PassWord psw = new PassWord();

            psw.GetEncryption(encryp);
            psw.ShowDialog();

            if (encryp.PasswordOk)
            {
                if (_mo != null)
                {
                    MotionOperate.StopAllThread();
                    _mo.ExitHandwheel();
                }

                if (copyPoints != null)
                {
                    SaveTrace(copyPoints);
                }
            }
        }

        public void CancelChoose()
        {
            if (points != null)
                dmFile.DisplayPointList(points);
        }

        //
        // END: new method end
        //
    }
}
