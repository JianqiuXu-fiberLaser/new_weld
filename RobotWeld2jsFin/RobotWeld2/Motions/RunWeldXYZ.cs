using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Welding for rotating the A-axis, meanwhile the XYZ axis moveing accordingly
    /// </summary>
    public class RunWeldXYZ
    {
        private readonly Mutex _mutex = new(false);
        private Thread? wthread;
        private Thread? lthread;
        private bool _IsRuning;
        public int[]? bitAddr;

        private List<List<int[]>>? _moveSeg;
        private List<List<double>>? _splist;
        private List<int>? _type;
        private List<int>? _laserSeg;
        private List<int>? _laserState;
        private List<LaserArgument>? _laserArgs;

        // move function
        private readonly MotionOperate _mo;
        private readonly MoveLaserThread _mlt;
        public RunWeldXYZ(MotionOperate mo)
        {
            _mo = mo;
            _mlt = new(_mo);
        }

        /// <summary>
        /// Put weld section data to the clas
        /// </summary>
        /// <param name="wms"> weld move section data </param>
        public void PutParameter(List<List<int[]>> moveSeg, List<int> type, List<List<double>> splist,
            List<int> laserSeg, List<int> laserState, List<LaserArgument> laserArgs)
        {
            _moveSeg = moveSeg;
            _type = type;
            _splist = splist;
            _laserSeg = laserSeg;
            _laserState = laserState;
            _laserArgs = laserArgs;

            AnalyzeWeldData();
            CallActionMethon();
        }

        public void CallActionMethon()
        {
            _mo.SetActionMethod(this);

            bitAddr = new int[1] { 0 };
            _mo.ScanAction(bitAddr);
        }

        /// <summary>
        /// This function is called by the IO-ReadScan thread in the Class of MotionOperate,
        /// when special IO ports being triggered.
        /// </summary>
        /// <param name="iActNum"> IO ports array </param>
        public void Action(int[] iActNum)
        {
            if (this.bitAddr != null && iActNum[this.bitAddr[0]] == 1 && !_IsRuning)
            {
                Run();
                _IsRuning = true;
            }
        }

        /// <summary>
        /// This function is called by the IO-ReadScan thread in the Class of MotionOperate,
        /// when special IO ports NOT being triggered.
        /// </summary>
        /// <param name="iActNum">IO ports array </param>
        public void NoAction(int[] iActNum)
        {
            if (this.bitAddr != null && iActNum[this.bitAddr[0]] == 0 && _IsRuning)
            {
                _IsRuning = false;
            }
        }

        private void Run()
        {
            if (wthread != null && wthread.IsAlive)
            {
                Thread.Sleep(5);
                return;
            }

            wthread = new Thread(WeldThread)
            {
                Name = nameof(WeldThread),
                IsBackground = true,
            };
            wthread.Start();

            Thread.Sleep(10);
            lthread = new(LaserThread)
            {
                Name = nameof(LaserThread),
                IsBackground = true,
            };
            lthread.Start();
        }

        /// <summary>
        /// Analysis wms data, and mark them.
        /// </summary>
        public void AnalyzeWeldData()
        {
            if (_laserSeg == null || _laserState == null || _laserArgs == null)
                return;

            _mlt.PutWeldSectionData(_laserSeg, _laserState, _laserArgs);
        }

        public void WeldThread()
        {
            if (_moveSeg == null || _type == null || _splist == null)
            {
                MainModel.AddInfo("XYZA焊接参数错误");
                return;
            }

            _mutex.WaitOne(3000);
            _mlt.SetActionFinish(false);
            int j = 0;
            for (int i = 0; i < _moveSeg.Count; i++)
            {
                if (_type[i] == 0)
                {
                    _mo.MoveLine(_moveSeg[i], _splist[i], j);
                    j += _splist[i].Count;
                }
                else
                {
                    _mo.MoveArc(_moveSeg[i], _splist[i], j);
                    j += _splist[i].Count / 2;
                    j++;
                }

            }
            _mlt.SetActionFinish(true);

            int[] xyz = new int[4] { 0, 0, 0, 0 };
            var xyzl = new List<int[]>() { xyz };
            double ss = _splist[0][0];
            var sl = new List<double>() { ss };
            _mo.MoveLine(xyzl, sl, j);
            _mutex.ReleaseMutex();
        }

        public void LaserThread()
        {
            _mlt.LaserThread();
        }
    }
}
