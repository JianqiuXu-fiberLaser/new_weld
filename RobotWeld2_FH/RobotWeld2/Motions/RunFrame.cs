using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.Motions
{
    public abstract class RunFrame
    {
        public List<WeldMoveSection>? wms;
        private Thread? wthread;
        private Thread? lthread;
        private bool _IsRuning;
        public int[]? bitAddr;

        /// <summary>
        /// Put weld section data to the clas
        /// </summary>
        /// <param name="wms"> weld move section data </param>
        public void PutParameter(List<WeldMoveSection> wms)
        {
            this.wms = wms;
            AnalyzeWeldData();
            CallActionMethon();
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

            Thread.Sleep(30);
            lthread = new(LaserThread)
            {
                Name = nameof(LaserThread),
                IsBackground = true,
            };
            lthread.Start();
        }

        /// <summary>
        /// Analyze the weld section data for move and laser thread.
        /// </summary>
        public abstract void AnalyzeWeldData();

        /// <summary>
        /// Give the name and delegate to the class of MotionOperate
        /// </summary>
        public abstract void CallActionMethon();

        /// <summary>
        /// The weld thread
        /// </summary>
        public abstract void WeldThread();

        /// <summary>
        /// The laser thread.
        /// </summary>
        public abstract void LaserThread();
    }
}
