///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: new added class to display laser parameters.
// Ver. 3.0: (1) new laser section with double bits for multiply
//               actions. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// Run and weld the trace all-in-one, no matter what Wms are.
    /// </summary>
    internal class RunWeldAll
    {
        private readonly Mutex _mutex = new(false);

        public readonly MotionBus _mbus;
        private readonly MoveLaserThread _mlt;

        private Thread? wthread;
        private Thread? lthread;
        private bool _IsRuning;
        private bool _finishRun;
        public int[]? _bitAddr;

        public List<int[]>? _ptlist;
        public List<double>? _splist;

        /// <summary>
        /// Run weld process.
        /// </summary>
        /// <param name="mbus"></param>
        internal RunWeldAll(MotionBus mbus)
        {
            _mbus = mbus;
            _mlt = new MoveLaserThread();
        }

        /// <summary>
        /// Transfer moving parameters to card driver.
        /// </summary>
        /// <param name="ptlist"> Point's coordinate value array </param>
        /// <param name="splist"> Speed array, whose length should equal that of point array </param>
        /// <param name="lasSec"> Array for laser actions. </param>
        /// <param name="lp"> Array of parameters of laser, depending on the value of lasSec. </param>
        internal void PutParameter(List<int[]> ptlist, List<double> splist, List<int[]> lasSec, 
            List<double[]> lp)
        {
            _IsRuning = false;
            _finishRun = false;

            _ptlist = ptlist;
            _splist = splist;

            _mlt.PutParmeter(lasSec, lp);
            CallActionMethod();
        }

        /// <summary>
        /// Set callback Action method for MotionBus.
        /// </summary>
        private void CallActionMethod()
        {
            _mbus.SetActionMethod(this);

            _bitAddr = new int[1] { MotionSpecification.PedalTrigger };
            _mbus.ScanAction(_bitAddr);
        }

        /// <summary>
        /// This function is called by the IO-ReadScan thread in the Class of MotionOperate,
        /// when special IO ports being triggered.
        /// </summary>
        /// <param name="iActNum"> IO ports array </param>
        public void Action(int[] iActNum)
        {
            if (_bitAddr != null && iActNum[0] == 1 && !_IsRuning && !_finishRun)
            {
                _IsRuning = true;
                MainModel.AddCount(1);
                Run();
                _finishRun = true;
            }
        }

        /// <summary>
        /// This function is called by the IO-ReadScan thread in the Class of MotionOperate,
        /// when special IO ports NOT being triggered.
        /// </summary>
        /// <param name="iActNum">IO ports array </param>
        public void NoAction(int[] iActNum)
        {
            if (_bitAddr != null && iActNum[0] == 0 && _IsRuning && _finishRun)
            {
                _IsRuning = false;
                _finishRun = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

            Thread.Sleep(30);
            lthread = new(LaserThread)
            {
                Name = nameof(LaserThread),
                IsBackground = true,
            };
            lthread.Start();
        }

        /// <summary>
        /// Weld thread.
        /// </summary>
        private void WeldThread()
        {
            if (_ptlist == null || _splist == null)
            {
                MainModel.AddInfo("WeldAll参数错误");
                return;
            }

            _mutex.WaitOne(3000);
            _mlt.SetActionFinish(false);
            _mbus.Move4D(_ptlist, _splist, 0);
            _mlt.SetActionFinish(true);
            _mutex.ReleaseMutex();
            MotionOperate.CloseAir();
        }

        /// <summary>
        /// Laser thread.
        /// </summary>
        private void LaserThread()
        {
            _mlt.LaserThread();
        }
    }
}
