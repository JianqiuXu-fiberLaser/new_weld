///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// CLASS: operation through the motion card, i.e., moving, laser and handwheel
    /// Manage threads about motion operation.
    /// </summary>

    public class MotionOperate
    {
        //-----------------------------------------------------
        // This smooth Coefficient depends on the properties of
        // rotating mechanics, Which in most case not need to be
        // changed.
        //-----------------------------------------------------
        private const double WIRE_VOLT = 5.0;
        
        public const double SmoothCoefficient = 0.25;

        private readonly object _lock = new();

        private MotionBoard? _mb;

        private static double oneCycle;
        private static double xMillimeter;
        private static double yMillimeter;
        private static double zMillimeter;

        public MotionOperate() { }

        /// <summary>
        /// In this initial method, we obtain the self-matching for various 
        ///   types of motion card.
        /// </summary>
        public void InitialCard()
        {
            // the machine has the A axis or not
            if (MotionSpecification.AaxisState == 1)
                _mb = new MotionBoard();
            else
                _mb = new MotionBoard3();

            _mb.InitialCard();
        }

        public MotionBoard GetMotionboard()
        {
            if (_mb != null) return _mb;
            else return new MotionBoard();
        }

        public static void SetMaxPower(int mPower)
        {
            LaserOperate.MaxPower = mPower;
        }

        public static int GetMaxPower()
        {
            return (int)LaserOperate.MaxPower;
        }

        public static double Xmillimeter
        {
            get => xMillimeter;
            set => xMillimeter = value;
        }

        public static double Ymillimeter
        {
            get => yMillimeter;
            set => yMillimeter = value;
        }

        public static double Zmillimeter
        {
            get => zMillimeter;
            set => zMillimeter = value;
        }

        public static double OneCycle
        {
            get => oneCycle;
            set => oneCycle = value;
        }

        /// <summary>
        /// Read coordinate of XYZ axes
        /// </summary>
        /// <param name="pos"></param>
        internal void ReadCoordinate(out int[] pos)
        {
            pos = new int[4];
            int[] barePos = new int[4];

            _mb?.ReadCoordinate(out barePos);
            pos[0] = MotionSpecification.XDirection * barePos[0];
            pos[1] = MotionSpecification.YDirection * barePos[1];
            pos[2] = MotionSpecification.ZDirection * barePos[2];
            pos[3] = barePos[3];
        }

        //-------------------------------------------------------------
        // Operate IO
        //-------------------------------------------------------------
        internal static void EchoBit(int bitAddr)
        {
            MotionBoard.SelfResetBit(bitAddr);
        }

        internal static void OpenAir()
        {
            MotionBoard.SetBit(MotionSpecification.ProtectedAir, false);
        }

        internal static void CloseAir()
        {
            MotionBoard.SetBit(MotionSpecification.ProtectedAir, true);
        }

        internal static void TurnOnBit(int bitAddr)
        {
            MotionBoard.SetBit(bitAddr, true);
        }

        internal static void TurnOffBit(int bitAddr)
        {
            MotionBoard.SetBit(bitAddr, false);
        }

        internal void ResetThread()
        {
            lock (_lock)
            {
                _mb?.ResetAll();
                Thread.Sleep(20);
            }
        }

        internal void ResetAaxis()
        {
            _mb?.ResetAaxis();
        }

        //-------------------------------------------------------------
        // Operate the wire feeder
        //-------------------------------------------------------------

        /// <summary>
        /// Feed in wire
        /// </summary>
        internal void FeedIn(double speed)
        {
            double icode = speed / WIRE_VOLT;
            MotionBoard.SetBit(MotionSpecification.FeedWire, true);
            MotionBoard.SetVoltage(MotionSpecification.WireDac - 1, icode);
        }

        /// <summary>
        /// Stop feed wire in.
        /// </summary>
        internal void FeedStop()
        {
            MotionBoard.SetBit(MotionSpecification.FeedWire, false);
            MotionBoard.SetVoltage(MotionSpecification.WireDac - 1, 0);
        }

        /// <summary>
        /// Withdraw wire
        /// </summary>
        internal void Withdraw(double speed)
        {
            double icode = speed / WIRE_VOLT;
            MotionBoard.SetBit(MotionSpecification.Withdraw, true);
            MotionBoard.SetVoltage(MotionSpecification.WireDac - 1, icode);
        }

        /// <summary>
        /// Stop withdraw wire
        /// </summary>
        internal void WithdrawStop()
        {
            MotionBoard.SetBit(MotionSpecification.Withdraw, false);
            MotionBoard.SetVoltage(MotionSpecification.WireDac - 1, 0);
        }

        //-------------------------------------------------------------
        // Axis moving functions
        //-------------------------------------------------------------

        /// <summary>
        /// 3D line move, even the hardware has the A-axis.
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="speed"></param>
        /// <param name="StartSegNum"> the number of start segment </param>
        internal void MoveLine(List<int[]> pts, List<double> speed, int StartSegNum)
        {
            lock (_lock)
            {
                MotionBoard.LineMove(pts, speed, startSeg: StartSegNum);
            }
        }

        /// <summary>
        /// 3D arc move, even the hardware system has A-axis.
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="speed"></param>
        /// <param name="StartSegNum"> the number of start segment </param>
        internal void MoveArc(List<int[]> pts, List<double> speed, int StartSegNum)
        {
            lock (_lock)
            {
                MotionBoard.CircleMove(pts, speed, startSeg: StartSegNum);
            }
        }

        /// <summary>
        /// 4D line move
        /// If the hardware has not A-axis, instead by 3D line
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="speed"></param>
        /// <param name="StartSegNum"> the number of start segment </param>
        internal void Move4D(List<int[]> pts, List<double> speed, int StartSegNum)
        {
            lock (_lock)
            {
                _mb?.RotateMove(pts, speed, startSeg: StartSegNum);
            }
        }

        internal static int GetSegNo()
        {
            return MotionBoard.GetSegNo();
        }
    }
}
