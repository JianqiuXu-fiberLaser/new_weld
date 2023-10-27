using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using System.Collections.Generic;
using System.Threading;
using static System.Math;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Move along the XYZ so that arc trace can be interpolated.
    /// </summary>
    public class RunWeld4D : RunFrame
    {
        private readonly Mutex _mutex = new(false);

        // data for moving section
        private List<int[]>? _ptsList;
        private List<double>? _speed;

        // move function
        private readonly MotionOperate _mo;
        private readonly MoveLaserThread _mlt;
        public RunWeld4D(MotionOperate mo)
        {
            this._mo = mo;
            _mlt = new(mo);
        }

        public override void CallActionMethon()
        {
            _mo.SetActionMethod(this);

            this.bitAddr = new int[1] { 0 };
            _mo.ScanAction(this.bitAddr);
        }

        public override void CAnalyzeWeldData()
        {
            if (wms == null) return;

             //WriteMsgFile.WriteFile(wms);

            _ptsList = new List<int[]>();
            _speed = new List<double>();

            for (int i = 0; i < wms.Count; i++)
            {
                RemarkMoveData(i, this.wms[i]);
            }

            //WriteMsgFile.WriteFile(_ptsList);
            _mlt.CPutWeldSectionData(this.wms);
            _mlt.CLaserDataAnalyse();
        }

        public override void WeldThread()
        {
            if (_ptsList == null || _speed == null)
            {
                MainModel.AddInfo("4D焊接参数错误");
                return;
            }

            _mutex.WaitOne(3000);
            _mlt.SetActionFinish(false);
            _mo.Move4D(_ptsList, _speed, 0);
            _mlt.SetActionFinish(true);
            _mutex.ReleaseMutex();
        }

        public override void LaserThread()
        {
            _mlt.CLaserThread();
        }

        private void RemarkMoveData(int inum, WeldMoveSection iwms)
        {
            if (_ptsList == null || _speed == null) return;

            List<int[]> pointlist = new(iwms.GetPosition());
            double baseSpeed = iwms.Speed;
            if (inum == 0)
            {
                AddBareList(pointlist);
                _speed.Add(baseSpeed);
            }
            else
            {
                int t = pointlist.Count;
                AddBareList(pointlist.GetRange(1, t - 1));
            }

            int ptcount = iwms.GetPosition().Count;

            // when the count less than 3, do not make speed compensation 
            if (ptcount > 1 && ptcount <= 3)
            {
                for (int i = 1; i < ptcount; i++)
                {
                    double ptSpeed;
                    // when no rotating, smoothing does not needed.
                    if (pointlist[0][3] == pointlist[ptcount - 1][3])
                    {
                        ptSpeed = baseSpeed;
                    }
                    else
                    {
                        ptSpeed = MotionOperate.SmoothCoefficient * baseSpeed;
                    }
                    _speed.Add(ptSpeed);
                }
            }
            // when the rotate points larger than 3, compensation is needed
            // to make rotating smoothly.
            // This processure is only for the consequent rotated points.
            // The speed is the speed that comes to the points.
            else if (ptcount > 3)
            {
                double c0 = CoeffSpeed(pointlist[1], pointlist[0]);

                for (int j = 0; j < ptcount - 1; j++)
                {
                    double c = CoeffSpeed(pointlist[j + 1], pointlist[j]);
                    double ptSpeed = MotionOperate.SmoothCoefficient * baseSpeed * c / c0;

                    _speed.Add(ptSpeed);
                }
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

        private static double CoeffSpeed(int[] a1, int[] a0)
        {
            double d2 = PointDistance(a1, a0);
            double a2 = PointAngle(a1, a0);

            return Sqrt(a2 + d2);
        }

        private static double PointDistance(int[] a1, int[] a0)
        {
            double dx = a1[0] - a0[0];
            double dy = a1[1] - a0[1];
            double dz = a1[2] - a0[2];

            return Pow(dx, 2) + Pow(dy, 2) + Pow(dz, 2);
        }

        private static double PointAngle(int[] a1, int[] a0)
        {
            return Pow(a1[3] - a0[3], 2);
        }
    }
}
