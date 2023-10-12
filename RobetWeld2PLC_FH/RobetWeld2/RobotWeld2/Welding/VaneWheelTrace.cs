///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 4.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
// Ver. 4.0: (1) weld all traces by the master computer and not
//               response for PLC.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using System.Collections.Generic;
using System.Threading;
using static System.Math;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Welding along the trace in VaneWheel trace Type
    /// </summary>
    public class VaneWheelTrace
    {
        //
        // welding parameter
        //
        private int _lpLeft;
        private int _lpRight;
        private DaemonFile? dmFile;

        private NowState? _nws;
        private double _weldLeft;
        private double _weldRight;
        private double _moveSpeed;
        private List<int>? _lSegNo;
        private List<int>? _rSegNo;

        //
        // NEW: ---- methods from HERE ----
        //

        private MotionOperate? _mo;
        private List<int[]>? _potsLeft;
        private List<int[]>? _potsRight;
        private List<int[]>? _potsOrigin;
        private int _traceCount;

        private Dictionary<int, int>? _ltpdic;
        private Dictionary<int, int>? _rtpdic;
        private readonly int _slowNum = 3;

        public VaneWheelTrace() { }

        internal void CopyHardware(MotionOperate mo, DaemonFile dmFile)
        {
            _mo = mo;
            this.dmFile = dmFile;
            _mo.InitialLaser();
        }

        /// <summary>
        /// The point list is recorded as a dictionary.
        /// when key > 0, for the left side
        /// key = 0, for the prepared point, there are three points: 
        ///          original, left ready, and right ready.
        /// key < 0, for the right side
        /// </summary>
        /// <param name="points"> the dictionary of point list </param>

        //--------------------------------------------------------------
        //     Respose to the Signal from the PLC
        //     Version 2.2
        //
        // In 12: start to weld left
        // Out 12: receive 12 signal
        // Out 13: finish welding
        //
        // In 14: start to weld right
        // Out 14: receive 14 signal
        // Out 15: finish welding
        //
        //--------------------------------------------------------------

        internal void WorkProcess(in Dictionary<int, List<Point>> points)
        {
            _ltpdic = new Dictionary<int, int>();
            _rtpdic = new Dictionary<int, int>();

            PrepareData(in points);
            //WriteLog.WrtLog(_lSegNo);

            moveTaskFinish = true;

            _mo?.SetActionMethod(this);
            CheckPosition();

            int[] bitAddr = new int[4] { 12, 13, 14, 15 };
            // Loop thread to scan IO and action 
            _mo?.ScanAction(bitAddr);
        }

        private bool moveTaskFinish;

        /// <summary>
        /// the weld and fly action according to receiving signals
        /// </summary>
        /// <param name="iActNum"> the port number that receiving signal </param>
        public void Action(int[] iActNum)
        {
            // if there is a task to run, do nothing
            if (!moveTaskFinish) return;

            // 1st: from the finish position (half part) to the weld positions.
            if (_nws == NowState.RESET_POS)
            {
                moveTaskFinish = false;
                Thread.Sleep(5);
                if (iActNum[0] == 1) FlyWeld_Left();
                else if (iActNum[2] == 1) FlyWeld_Right();
            }
            // 2nd: from the left weld position
            else if (_nws == NowState.LEFT_FINISH)
            {
                moveTaskFinish = false;
                Thread.Sleep(5);
                if (iActNum[0] == 1) Weld_Left();
                else if (iActNum[2] == 1) FlyWeld_Right();
            }
            // 3rd: from the right weld position
            else if (_nws == NowState.RIGHT_FINISH)
            {
                moveTaskFinish = false;
                Thread.Sleep(5);
                if (iActNum[2] == 1) Weld_Right();
                else if (iActNum[0] == 1) FlyWeld_Left();
            }
            else
            {
                // do nothing
            }
        }

        /// <summary>
        /// When the Task is finised, no run thread exist.
        /// </summary>
        /// <param name="iAct"> string of action name </param>
        public void FinAction(string iAct)
        {
            DoWeldFin();
        }

        private void DoWeldFin()
        {
            CloseAir();
            if (_nws == NowState.LEFT_WELD)
            {
                _mo?.EchoBit(13);
                _nws = NowState.LEFT_FINISH;
            }
            else
            {
                _mo?.EchoBit(15);
                _nws = NowState.RIGHT_FINISH;
            }

            moveTaskFinish = true;
        }

        //
        //---- Welding functions ----
        //

        private void Weld_Left()
        {
            if (_potsLeft == null || _lSegNo == null || _ltpdic == null) return;

            _mo?.EchoBit(12);
            _nws = NowState.LEFT_WELD;
            OpenAir();
            dmFile?.SetInputPower(_lpLeft);
            _mo?.SetupLaserParameter(_lpLeft);
            _mo?.RunWeld2(_lpLeft, _potsLeft, _weldLeft, _moveSpeed, _lSegNo, _ltpdic);
        }

        private void Weld_Right()
        {
            if (_potsRight == null || _rSegNo == null || _rtpdic == null) return;

            _nws = NowState.RIGHT_WELD;
            _mo?.EchoBit(14);
            OpenAir();
            dmFile?.SetInputPower(_lpRight);
            _mo?.SetupLaserParameter(_lpRight);
            _mo?.RunWeld2(_lpRight, _potsRight, _weldRight, _moveSpeed, _rSegNo, _rtpdic);
        }

        private void FlyWeld_Left()
        {
            if (_potsLeft == null || _lSegNo == null || _ltpdic == null) return;

            _nws = NowState.LEFT_WELD;
            _mo?.EchoBit(12);
            _mo?.OpenAir();
            dmFile?.SetInputPower(_lpLeft);
            _mo?.SetupLaserParameter(_lpLeft);
            _mo?.RunFlyWeld(_lpLeft, _potsLeft, _weldLeft, _moveSpeed, _lSegNo, _ltpdic);
        }

        private void FlyWeld_Right()
        {
            if (_potsRight == null || _rSegNo == null || _rtpdic == null) return;

            _nws = NowState.RIGHT_WELD;
            _mo?.EchoBit(14);
            _mo?.OpenAir();
            dmFile?.SetInputPower(_lpRight);
            _mo?.SetupLaserParameter(_lpRight);
            _mo?.RunFlyWeld(_lpRight, _potsRight, _weldRight, _moveSpeed, _rSegNo, _rtpdic);
        }

        //
        //---- Operate Air valve and other functions ----
        //

        private void CloseAir()
        {
            _mo?.CloseAir();
        }

        private void OpenAir()
        {
            _mo?.OpenAir();
        }

        //
        //---- prepare data of each trace ----
        //

        private void PrepareData(in Dictionary<int, List<Point>> ptlist)
        {
            if (!CheckPointList(in ptlist)) return;

            _potsLeft = new List<int[]>();
            _potsRight = new List<int[]>();

            AssemblyOrigin(in ptlist);
            AssemblyLeftList(in ptlist);
            AssemblyRightList(in ptlist);

            //WriteLog.WrtLog(_potsLeft);
            PutLaserPower(in ptlist);
        }

        private bool CheckPointList(in Dictionary<int, List<Point>> ptlist)
        {
            _traceCount = DaemonModel.TraceCount;
            int totalPoints = _traceCount * 14 + 3;
            int realpointCount = 0;
            foreach (var pt in ptlist)
                realpointCount += pt.Value.Count;

            if (ptlist == null || realpointCount != totalPoints)
            {
                new Werr().WaringMessage("点位数据错误");
                _lpLeft = 0;
                _lpRight = 0;
                return false;
            }

            return true;
        }

        private void AssemblyOrigin(in Dictionary<int, List<Point>> ptlist)
        {
            _potsOrigin = new List<int[]>();

            // the prepared points
            for (int i = 0; i < ptlist[0].Count; i++)
            {
                var ol = ptlist[0][i];
                int[] ooxyz = new int[3]
                {
                    (int)ol.vector.X,
                    (int)ol.vector.Y,
                    (int)ol.vector.Z,
                };

                _potsOrigin.Add(ooxyz);
            }
        }

        private void AssemblyLeftList(in Dictionary<int, List<Point>> ptlist)
        {
            if (_potsLeft == null) return;

            _lSegNo = new List<int>();

            // the prepared point
            var ol = ptlist[0][1];
            int[] ooxyz = new int[3]
            {
                (int)ol.vector.X,
                (int)ol.vector.Y,
                (int)ol.vector.Z,
            };
            _potsLeft.Add(ooxyz);

            // the weld trace
            for (int i = 1; i <= DaemonModel.TraceCount; i++)
            {
                var pl = ptlist[i];
                if (pl == null || _ltpdic == null) return;

                int icount = OneTrace(in pl, 0);
                _ltpdic.Add(i, icount);
                _lSegNo.Add(icount + 1 - _slowNum);
            }
        }

        private void AssemblyRightList(in Dictionary<int, List<Point>> ptlist)
        {
            if (_potsRight == null) return;
            _rSegNo = new List<int>();

            // the prepared point
            var ol = ptlist[0][2];
            int[] ooxyz = new int[3]
            {
                (int)ol.vector.X,
                (int)ol.vector.Y,
                (int)ol.vector.Z,
            };
            _potsRight.Add(ooxyz);

            for (int i = -1; i >= -DaemonModel.TraceCount; i--)
            {
                var pl = ptlist[i];
                if (pl == null || _rtpdic == null) return;

                int icount = OneTrace(in pl, 1);
                _rtpdic.Add(Abs(i), icount);
                _rSegNo.Add(icount + 1 - _slowNum);
            }
        }

        private int OneTrace(in List<Point> pl, int lr)
        {
            int ret = 0;
            for (int i = 0; i < pl.Count - 2; i += 2)
            {
                int t = InterpolateArc(pl[i], pl[i + 1], pl[i + 2], lr);
                ret += t;
            }

            // the last point
            int jl = pl.Count - 1;
            var xl = (int)pl[jl].vector.X;
            var yl = (int)pl[jl].vector.Y;
            var zl = (int)pl[jl].vector.Z;

            if (lr == 0)
                _potsLeft?.Add(new int[3] { xl, yl, zl });
            else
                _potsRight?.Add(new int[3] { xl, yl, zl });

            ret++;
            return ret;
        }

        private int InterpolateArc(Point p1, Point p2, Point p3, int lr)
        {
            (double x, double y) = FindCenter(p1, p2, p3);

            var pt = new List<double[]>();
            SubArc(p1, p3, x, y, ref pt);
            InverseSubArc(x, y, ref pt, lr);

            return pt.Count;
        }

        private static (double x, double y) FindCenter(Point p1, Point p2, Point p3)
        {
            double x1 = p1.vector.X;
            double y1 = p1.vector.Y;

            double x2 = p2.vector.X;
            double y2 = p2.vector.Y;

            double x3 = p3.vector.X;
            double y3 = p3.vector.Y;

            double dm = (x2 - x1) * (y3 - y2) + (x3 - x2) * (y2 - y1);

            if (dm == 0)
            {
                new Werr().WaringMessage("圆弧不存在");
                return (0, 0);
            }
            else
            {
                double A = x1 - x2;
                double B = y1 - y2;
                double C = x1 - x3;
                double D = y1 - y3;
                double E = ((x1 * x1 - x2 * x2) - (y2 * y2 - y1 * y1)) / 2;
                double F = ((x1 * x1 - x3 * x3) - (y3 * y3 - y1 * y1)) / 2;

                double x = -(D * E - B * F) / (B * C - A * D);
                double y = -(A * F - C * E) / (B * C - A * D);

                return (x, y);
            }
        }

        private static void SubArc(Point p1, Point p3, double x, double y, ref List<double[]> pt)
        {
            // shift the coordinate to the arc centre.
            double x1 = p1.vector.X - x;
            double y1 = p1.vector.Y - y;
            double z1 = p1.vector.Z;

            double x3 = p3.vector.X - x;
            double y3 = p3.vector.Y - y;
            double z3 = p3.vector.Z;

            double ra = Sqrt(x1 * x1 + y1 * y1);

            // the angle of point is set among 0 ~ 2*PI.
            double a3 = TAngle(y3, x3);
            double a1 = TAngle(y1, x1);

            // kept the arc less than half circle.
            if (Abs(a3 - a1) > PI)
            {
                if (a3 > a1) a3 -= 2 * PI;
                else a1 -= 2 * PI;
            }

            // the start point
            pt.Add(new double[3] { x1, y1, z1 });

            // the angle step to ensure the error in smaller than 0.1 mm
            double angle = a3 - a1;
            double detAngle;
            if (a3 > a1) detAngle = 10.0 / Sqrt(10 * ra);
            else detAngle = -10.0 / Sqrt(10 * ra);

            while (Abs(ra * Sin(detAngle)) > 0.5 * DaemonModel.MM)
            {
                detAngle *= 0.5;
            }

            // the accumelate swept angle
            double sumAngle = a1;

            double sn = angle / detAngle;
            for (int i = 1; i < sn; i++)
            {
                sumAngle += detAngle;
                double dx = ra * Cos(sumAngle);
                double dy = ra * Sin(sumAngle);
                double dz = z1 + i * (z3 - z1) / sn;
                pt.Add(new double[3] { dx, dy, dz });
            }

            // the end point
            //pt.Add(new double[3] { x3, y3, z3 });
        }

        private static double TAngle(double y, double x)
        {
            double angle;

            if (x > 0)
            {
                if (y >= 0) angle = Atan(y / x);
                else angle = 2 * PI + Atan(y / x);
            }
            else if (x < 0)
            {
                angle = PI + Atan(y / x);
            }
            else
            {
                if (y > 0) angle = PI / 2;
                else if (y < 0) angle = PI / 2;
                else angle = double.NaN;
            }

            return angle;
        }

        private void InverseSubArc(double x, double y, ref List<double[]> pt, int lr)
        {
            for (int i = 0; i < pt.Count; i++)
            {
                var xl = (int)(pt[i][0] + x);
                var yl = (int)(pt[i][1] + y);
                var zl = (int)pt[i][2];

                if (lr == 0)
                    _potsLeft?.Add(new int[3] { xl, yl, zl });
                else
                    _potsRight?.Add(new int[3] { xl, yl, zl });
            }
        }

        private void PutLaserPower(in Dictionary<int, List<Point>> ptlist)
        {
            // the speed and laser power
            var pts = ptlist[1];
            _lpLeft = pts[0].LaserPointPower;
            _weldLeft = pts[0].WeldSpeed;
            _moveSpeed = pts[0].Leap;

            var pt2 = ptlist[-1];
            _lpRight = pt2[0].LaserPointPower;
            _weldRight = pt2[0].WeldSpeed;
        }

        //
        // END -- the new method end
        //

        internal bool CheckPosition()
        {
            int[] pPosArray = new int[3];
            _mo?.ReadPosition(ref pPosArray);

            if (_potsOrigin != null && _potsOrigin.Count >= 3)
            {
                if (pPosArray[0] == _potsOrigin[1][0])
                {
                    _nws = NowState.LEFT_FINISH;
                    return true;
                }
                else if (pPosArray[0] == _potsOrigin[2][0])
                {
                    _nws = NowState.RIGHT_FINISH;
                    return true;
                }
            }

            _nws = NowState.RESET_POS;
            return false;
        }

        //
        // constants: work statement
        //
        internal enum NowState
        {
            LEFT_WELD,
            LEFT_FINISH,
            RIGHT_WELD,
            RIGHT_FINISH,
            RESET_POS,
        }
    }
}
