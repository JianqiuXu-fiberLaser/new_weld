///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) new laser weld parameter from the server or local
// Ver. 3.0: (1) add 2 xml file to store points' information and using
//               5 data structures to represent moving, laser and
//               material specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using RobotWeld3.GetTrace;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// The information integration of motion hardware's operation.
    /// </summary>

    public class MotionBus
    {
        //
        // Sub class being managed.
        //
        private readonly MotionOperate _mo;
        private HandOperate? _hand;
        private LaserOperate? _laser;
        private ScanIOaction? _sc;
        private readonly MainModel _mainModel;

        //
        // the varible used to operate threads.
        //        
        private static Thread? _wthread;
        private Thread? _hthread;

        private string _methodName = string.Empty;

        public MotionBus()
        {
            _mainModel = MainWindow.GetMainModel();
            _mo = new MotionOperate();
        }

        //
        //---- Transfer method to MotionOperate ----
        //
        public void InitialCard()
        {
            //MainModel.AddInfo("没有发现运动卡硬件\n");
            _mo.InitialCard();
            _hand = new HandOperate(this);
            _laser = new LaserOperate();
            _sc = new ScanIOaction(this);
        }

        public void ReadCoordinate(out int[] ptos)
        {
            _mo.ReadCoordinate(out ptos);
        }

        public static void StopAllThread()
        {
            ScanIOaction.SetIoThread(false);
            HandOperate.SetHandThread(false);
            LaserDriver.LaserOff();
        }

        public static void StopActionThread()
        {
            ScanIOaction.SetIoThread(false);
            LaserDriver.LaserOff();
        }

        public MotionBoard GetMotionBoard()
        {
            return _mo.GetMotionboard();
        }

        public void Move4D(List<int[]> pts, List<double> speed, int StartSegNum)
        {
            _mo.Move4D(pts, speed, StartSegNum);
        }

        //-------------------------------------------------------------
        // THRED: Run Handwheel
        //-------------------------------------------------------------

        public void RunHandWheel()
        {
            StopAllThread();
            if (_hand == null) return;

            Thread.Sleep(10);
            _hthread = new Thread(_hand.HandThread)
            {
                Name = nameof(RunHandWheel),
                IsBackground = true,
            };
            _hthread.Start();
        }

        public void ExitHandwheel()
        {
            HandWheel.Exit();
        }

        public static void InitialHandwheel()
        {
            HandWheel.InitialHandwheel();
        }

        private TakeTrace? _tkn;
        public TakeTrace GetTakeTrace()
        {
            return _tkn ?? new TakeTrace(this);
        }

        public void SetActionMethod(TakeTrace tkn)
        {
            _tkn = tkn;
        }

        private RunWeldAll? _runWeldAll;

        /// <summary>
        /// Set call back reference to the motion bus.
        /// </summary>
        /// <param name="rwa"></param>
        internal void SetActionMethod(RunWeldAll rwa)
        {
            _runWeldAll = rwa;
            _methodName = nameof(RunWeldAll);
        }

        //-------------------------------------------------------------
        // Operate the wire feeder
        //-------------------------------------------------------------

        /// <summary>
        /// Feed in wire
        /// </summary>
        public void FeedIn(double speed = 1.5)
        {
            _mo.FeedIn(speed);
        }

        /// <summary>
        /// Stop feed wire
        /// </summary>
        public void FeedStop()
        {
            _mo.FeedStop();
        }

        /// <summary>
        /// Withdraw wire
        /// </summary>
        /// <param name="speed"> withdraw speed in the unit mm/s </param>
        public void Withdraw(double speed = 1.5)
        {
            _mo.Withdraw(speed);
        }

        /// <summary>
        /// Stop to withdraw wire
        /// </summary>
        public void WithdrawStop()
        {
            _mo.WithdrawStop();
        }

        /// <summary>
        /// Set the voltage of wobble
        /// </summary>
        /// <param name="voltage"> voltage </param>
        public void SetWobble(double voltage)
        {
            _laser?.SetWobble(voltage);
        }

        //-------------------------------------------------------------
        // THRED: reset all axes
        //-------------------------------------------------------------

        /// <summary>
        /// Reset 3 axes and A-axis
        /// </summary>
        public void RunResetAxis()
        {
            StopActionThread();
            Thread.Sleep(10);

            _wthread = new Thread(_mo.ResetThread)
            {
                Name = nameof(RunResetAxis),
                IsBackground = true,
            };
            _wthread.Start();

            if (MotionSpecification.AaxisState == 1)
            {
                Thread rthread = new(_mo.ResetAaxis)
                {
                    IsBackground = true,
                };
                rthread.Start();
            }
        }

        //-------------------------------------------------------------
        // THRED: operate Laser, except welding functions.
        //-------------------------------------------------------------

        /// <summary>
        /// Open laser with slow rise edge.
        /// </summary>
        /// <param name="power"></param>
        public void OpenLaser(double power)
        {
            _laser?.SetParameter((int)power);
            _laser?.SlowLaserOn();
        }

        //-------------------------------------------------------------
        // ROUNTIN : Scan some specail in-ports, and trigger action
        //-------------------------------------------------------------

        /// <summary>
        /// Scan the in-port signal and do action according to the signal
        /// </summary>
        /// <param name="bitAddr"> in port address corresponding to the action number </param>
        public void ScanAction(int[] bitAddr)
        {
            if (_sc == null) return;

            ScanIOaction.SetIoThread(false);
            _sc.SetParameter(bitAddr);

            Thread.Sleep(5);

            Thread sThread = new(_sc.ScanThread)
            {
                Name = nameof(ScanAction),
                IsBackground = true,
            };
            sThread.Start();
        }

        public void IoAction(int[] iActNum)
        {
            ActionDelegate act;

            if (_runWeldAll != null && _methodName == nameof(RunWeldAll))
            {
                act = new(_runWeldAll.Action);
                act(iActNum);
            }
        }


        public void NoAction(int[] iActNum)
        {
            NoActionDelegate nact;

            if (_runWeldAll != null && _methodName == nameof(RunWeldAll))
            {
                nact = new(_runWeldAll.NoAction);
                nact(iActNum);
            }
        }
    }

    //
    // Delegate function to operate the main window
    //
    public delegate void ActionDelegate(int[] iActNum);
    public delegate void NoActionDelegate(int[] iActNum);
    public delegate void FinishDelegate(string iAct);
}
