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
using RobotWeld2.Motions;
using RobotWeld2.AppModel;
using System.Collections.Generic;

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
        //
        // NEW: ---- methods from HERE ----
        //

        private MotionOperate? _mo;
        private List<int[]>? _potsLeft;
        private List<int[]>? _potsRight;
        private List<int[]>? _potsOrigin;
        private int _traceCount;

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
            PrepareData(in points);
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
            if (!moveTaskFinish) { return; }

            // 1st: from the finish position (half part) to the weld positions.
            if (_nws == NowState.RESET_POS)
            {
                if (iActNum[0] == 1) FlyWeld_Left();
                else if (iActNum[2] == 1) FlyWeld_Right();
                moveTaskFinish = false;
            }
            // 2nd: from the left weld position
            else if (_nws == NowState.LEFT_FINISH)
            {
                if (iActNum[0] == 1) Weld_Left();
                else if (iActNum[2] == 1) FlyWeld_Right();
                moveTaskFinish = false;
            }
            // 3rd: from the right weld position
            else if (_nws == NowState.RIGHT_FINISH)
            {
                if (iActNum[2] == 1) Weld_Right();
                else if (iActNum[0] == 1) FlyWeld_Left();
                moveTaskFinish = false;
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
            if (_potsLeft == null) return;

            _nws = NowState.LEFT_WELD;
            _mo?.EchoBit(12);
            OpenAir();
            dmFile?.SetInputPower(_lpLeft);
            _mo?.RunWeld(_lpLeft, _potsLeft, _weldLeft, 0.5 * _moveSpeed);
        }

        private void Weld_Right()
        {
            if (_potsRight == null) return;

            _nws = NowState.RIGHT_WELD;
            _mo?.EchoBit(14);
            OpenAir();
            dmFile?.SetInputPower(_lpRight);
            _mo?.RunWeld(_lpRight, _potsRight, _weldRight, 0.5 * _moveSpeed);
        }

        private void FlyWeld_Left()
        {
            if (_potsLeft == null) return;

            _nws = NowState.LEFT_WELD;
            _mo?.EchoBit(12);
            _mo?.OpenAir();
            dmFile?.SetInputPower(_lpLeft);

            _mo?.RunFlyWeld(_lpLeft, _potsLeft, _weldLeft, _moveSpeed);
        }

        private void FlyWeld_Right()
        {
            if (_potsRight == null) return;

            _nws = NowState.RIGHT_WELD;
            _mo?.EchoBit(14);
            _mo?.OpenAir();
            dmFile?.SetInputPower(_lpRight);
            _mo?.RunFlyWeld(_lpRight, _potsRight, _weldRight, _moveSpeed);
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
                return;
            }

            _potsLeft = new List<int[]>();
            _potsRight = new List<int[]>();
            _potsOrigin = new List<int[]>();

            int[] orxyz = new int[3];
            // the prepared points
            var ol = ptlist[0][1];
            orxyz[0] = (int)ol.vector.X;
            orxyz[1] = (int)ol.vector.Y;
            orxyz[2] = (int)ol.vector.Z;
            _potsLeft.Add(orxyz);

            var or = ptlist[0][2];
            orxyz[0] = (int)or.vector.X;
            orxyz[1] = (int)or.vector.Y;
            orxyz[2] = (int)or.vector.Z;
            _potsRight.Add(orxyz);

            // It is assume each trace has just 7 points. Lese or more than 7, then give an error.
            for (int i = 1; i < _traceCount; i++)
            {
                // transfer
                var pl = ptlist[i];
                var pr = ptlist[-i];

                int[] plxyz = new int[3];
                int[] prxyz = new int[3];
                for (int j = 0; j < pl.Count; j++)
                {
                    plxyz[0] = (int)pl[j].vector.X;
                    plxyz[1] = (int)pl[j].vector.Y;
                    plxyz[2] = (int)pl[j].vector.Z;
                    _potsLeft.Add(plxyz);

                    prxyz[0] = (int)pr[j].vector.X;
                    prxyz[1] = (int)pr[j].vector.Y;
                    prxyz[2] = (int)pr[j].vector.Z;
                    _potsRight.Add(prxyz);
                }
            }

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
