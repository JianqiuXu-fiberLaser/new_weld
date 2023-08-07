using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Welding for rotating the A-axis, meanwhile the XYZ axis moveing accordingly
    /// </summary>
    public class RunWeldXYZ : RunFrame
    {
        private readonly Mutex _mutex = new(false);

        // data for moving section
        private List<int>? _sectionState;
        private List<int>? _sectionPointNumber;
        private List<int[]>? _ptsList;
        private List<double>? _speed;

        // move function
        private readonly MotionOperate _mo;
        private readonly MoveLaserThread _mlt;
        public RunWeldXYZ(MotionOperate mo)
        {
            _mo = mo;
            _mlt = new(_mo);
        }

        public override void CallActionMethon()
        {
            _mo.SetActionMethod(this);

            bitAddr = new int[1] { 0 };
            _mo.ScanAction(bitAddr);
        }

        public override void AnalyzeWeldData()
        {
            if (this.wms == null) { return; }

            _sectionState = new List<int>();
            _ptsList = new List<int[]>();
            _speed = new List<double>();
            _sectionPointNumber = new List<int>();

            for (int i = 0; i < this.wms.Count; i++)
            {
                RemarkMoveData(i, this.wms[i]);
            }

            _mlt.PutWeldSectionData(wms);
            _mlt.LaserDataAnalyse();
        }

        public override void WeldThread()
        {
            if (_ptsList == null || _sectionPointNumber == null || _speed == null || _sectionState == null)
            {
                MainModel.AddInfo("XYZA焊接参数错误");
                return;
            }

            _mutex.WaitOne(3000);
            _mlt.SetActionFinish(false);
            int j = 0;
            for (int i = 0; i < _sectionState.Count; i++)
            {
                var pts = _ptsList.GetRange(j, _sectionPointNumber[i]);
                var spt = _speed.GetRange(j, _sectionPointNumber[i]);

                if (_sectionState[i] == 0)
                {
                    _mo.MoveLine(pts, spt, j);
                }
                else
                {
                    _mo.MoveArc(pts, spt, j);
                }

                j += _sectionPointNumber[i];
            }
            _mlt.SetActionFinish(true);
            _mutex.ReleaseMutex();
            _mo.CloseAir();
        }

        public override void LaserThread()
        {
            _mlt.LaserThread();
        }

        //
        // match the speed for different section for every points
        // and remove the start point in each wms, except the first wms.
        // Return the count numbers.
        //
        private void RemarkMoveData(int iNum, WeldMoveSection iwms)
        {
            if (_sectionPointNumber == null || _sectionState == null || _ptsList == null || _speed == null)
                return;

            // filter the ptlist
            List<int[]> pointlist = new(iwms.GetPosition());
            int ptcount = pointlist.Count;
            int start;
            if (iNum == 0)
            {
                start = 0;
                AddBareList(pointlist);
                _sectionPointNumber.Add(ptcount);
            }
            else
            {
                start = 1;
                AddBareList(pointlist.GetRange(1, ptcount - 1));
                _sectionPointNumber.Add(ptcount - 1);
            }

            // mark the moving type
            _sectionState.Add(iwms.MoveType);

            // mark the speed
            for (int j = start; j < ptcount; j++)
            {
                double newspeed = iwms.GetSpeed(j);
                _speed.Add(newspeed);
            }
        }

        //
        // Convert the calculated data to machine bare data
        //
        private void AddBareList(List<int[]> pointList)
        {
            if (_ptsList == null || _speed == null) return;

            int x, y, z, a;
            for (int i = 0; i < pointList.Count; i++)
            {
                x = MotionSpecification.XDirection * pointList[i][0];
                y = MotionSpecification.YDirection * pointList[i][1];
                z = MotionSpecification.ZDirection * pointList[i][2];
                a = pointList[i][3];

                int[] p = new int[4] { x, y, z, a };
                _ptsList.Add(p);
            }
        }
    }
}
