using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.AppModel
{
    public class DaemonModel
    {
        private readonly DaemonFile dmFile;
        private readonly MainWindowViewModel mainViewModel;
        private MoveAction? _ma;
        private ExtraController? extraController;

        private RunTrace? runTrace;
        private TakeTrace? takeTrace;

        private List<Point>? points;
        private List<Point>? chpoints;
        private List<Point>? copyPoints;

        //
        // NEW: ---- new method from HERE
        //
        private MotionOperate? _mo;

        public void SetupParameter()
        {
            points = new List<Point>();
            GetPoints(points);
            dmFile.DisplyPointList(points);
            dmFile.PmFlag = false;
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
            if (runTrace != null && _mo != null && points != null)
            {
                runTrace.StartRunTrace(dmFile, _mo);
                runTrace.Run(tpy, points);
            }

/*            if (extraController != null)
            {
                extraController.SelfResetTurnOn(ActionIndex.AUTO_MODE);
                Thread.Sleep(10);
                extraController.SelfResetTurnOn(ActionIndex.LEFT_READY);
                Thread.Sleep(10);
                extraController.SelfResetTurnOn(ActionIndex.RIGHT_READY);
            }*/

            dmFile.PreparedState(true);
        }

        //
        // END: ---- new method end
        //

        public DaemonModel(DaemonFile dmFile, MainWindowViewModel mainViewModel)
        {
            this.dmFile = dmFile;
            this.mainViewModel = mainViewModel;
        }

        public void Initial()
        {
            points = new List<Point>();
            GetPoints(points);
            dmFile.DisplyPointList(points);

            _ma = new();
            dmFile.FreshMsg();
            runTrace = new(dmFile);
            takeTrace = new(dmFile);

            extraController = new(dmFile, mainViewModel);
            extraController.ConnectPLC();
            Thread.Sleep(100);    // waiting for connection.
            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
        }

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

        public void RunTrace(Tracetype tpy)
        {
            MoveAction.StopHandWheel();
            if (_ma != null && runTrace != null)
            {
                _ma.SetupParameter(dmFile);
                if (dmFile.laserParameter != null)
                {
                    _ma.SetupMaxPower(2000);
                    _ma.SetupLaser(dmFile.laserParameter);
                }

                if (points != null && points.Count > 0)
                {
                    runTrace.WeldTrace(_ma, tpy, points);
                }
            }

            if (extraController != null)
            {
                extraController.SelfResetTurnOn(ActionIndex.AUTO_MODE);
                Thread.Sleep(10);
                extraController.SelfResetTurnOn(ActionIndex.LEFT_READY);
                Thread.Sleep(10);
                extraController.SelfResetTurnOn(ActionIndex.RIGHT_READY);
            }
        }

        public void DisplayList(Point dispt)
        {
            if (copyPoints == null) return;

            if (dmFile.PointIndex <= copyPoints.Count)
            {
                copyPoints[dmFile.PointIndex] = dispt;
            }
            else
            {
                copyPoints.Add(dispt);
            }
            dmFile.DisplyPointList(copyPoints, dmFile.PointIndex);
        }


        private void ParsePointList(string inString, List<Point> wPointList)
        {
            string[] strs = inString.Split('\n');

            for (short i = 1; i < strs.Length; i++)
            {
                string[] var = strs[i].Split(',');
                if (var.Length == 6)
                {
                    double x = Convert.ToDouble(var[0]);
                    double y = Convert.ToDouble(var[1]);
                    double z = Convert.ToDouble(var[2]);
                    Vector vector = new(x, y, z);

                    int lt = Convert.ToInt32(var[3]);
                    int ls = Convert.ToInt32(var[4]);
                    int pp = Convert.ToInt32(var[5]);
                    Point point = new(lt, ls, pp, vector);

                    wPointList.Add(point);
                }
            }
        }

        public void ResetCard()
        {
            // _ma?.ResetAll();
            _mo?.RunResetAxis();
        }

        public void JogLightOn()
        {
            _mo?.InitialLaser();
            Thread.Sleep(5);
            _mo?.SetupLaserParameter(dmFile.laserParameter);
            _mo?.LaserOnNoRise();
        }

        public void JogLightOff()
        {
            _mo?.LaserOffNoFall();
        }

        //
        // NEW: methods from HERE
        //

        public void TakePoints()
        {
            if (dmFile != null && points != null)
            {
                dmFile.PointIndex = 0;
            }

            if (_mo != null && dmFile != null && takeTrace != null)
            {
                takeTrace.OpenHandwheel(_mo, dmFile, this);

                // new point recorded in chpoint
                chpoints ??= new List<Point>();
                chpoints.Clear();

                // copy Points stored the point's information during taking trace.
                copyPoints ??= new List<Point>();
                if (points != null)
                {
                    copyPoints = points;
                }
                dmFile.DisplyPointList(copyPoints, dmFile.PointIndex);
                dmFile.PreparedState(false);
            }
        }

        public void GotoSingleStep()
        {
            int[] ptxyz = new int[3];
            int pd = mainViewModel.PointInfo;
            if (points != null)
            {
                ptxyz[0] = (int)points[pd].vector.X;
                ptxyz[1] = (int)points[pd].vector.Y;
                ptxyz[2] = (int)points[pd].vector.Z;
                dmFile.DisplyPointList(points, dmFile.PointIndex);
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
                chpoints?.Add(onept);
                DisplayList(onept);
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
                dmFile.DisplyPointList(points, dmFile.PointIndex);
        }

        //
        // END: new method end
        //
    }
}
