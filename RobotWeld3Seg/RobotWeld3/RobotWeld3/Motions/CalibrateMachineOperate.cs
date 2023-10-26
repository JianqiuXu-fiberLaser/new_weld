///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using System;
using System.Threading;
using static System.Math;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// Basic motion operation for Calibrate Machine
    /// </summary>
    public class CalibrateMachineOperate
    {
        private readonly MotionBus _mbus;
        private readonly MotionBoard _mb;
        private CalibrateMachineModel calibrateMachinemodel;

        public CalibrateMachineOperate(MotionBus mbus, CalibrateMachineModel cbm)
        {
            _mbus = mbus;
            _mb = mbus.GetMotionBoard();
            calibrateMachinemodel = cbm;
        }

        public MotionBoard motionboard
        {
            get => _mb;
        }

        public void CalibrateMachine(double acc = 0.1, double dcc = 0.1, double smoothCoef = 10, double mSpeed = 10)
        {
            MotionBus.StopAllThread();
            Thread.Sleep(10);

            SubCalMachine scm = new(this);
            scm.SetupParameter(acc, dcc, smoothCoef, mSpeed);
            Thread wthread = new(scm.CalMachineThread)
            {
                IsBackground = true,
            };
            wthread.Start();
        }

        public void RecordCoordinate(int[] axisValue, int axisIndex)
        {
            FinAxisCallBack fac;
            if (calibrateMachinemodel != null)
            {
                fac = calibrateMachinemodel.FinishAxis;
                fac(axisValue, axisIndex);
            }
        }
    }

    //
    // CLASS : calibrate the machine
    //
    class SubCalMachine
    {
        private readonly CalibrateMachineOperate cmo;
        private readonly MotionBoard _mb;
        private double acc;
        private double dcc;
        private double smoothCoef;
        private double mSpeed;

        public SubCalMachine(CalibrateMachineOperate cmo)
        {
            this.cmo = cmo;
            this._mb = cmo.motionboard;
        }

        public void SetupParameter(double acc, double dcc, double smoothCoef, double mSpeed)
        {
            this.acc = acc;
            this.dcc = dcc;
            this.smoothCoef = smoothCoef;
            this.mSpeed = mSpeed;
        }

        /// <summary>
        /// The thread to run calibrate axis, now 4 axis is assumed.
        /// </summary>
        public void CalMachineThread()
        {
            if (_mb == null) return;

            for (int iAxis = 2; iAxis >= 0; iAxis--)
            {
                if (!CalMachineAxis(iAxis)) break;
            }
        }

        private bool CalMachineAxis(int iAxis)
        {
            int[] AxisValue = new int[3];
            bool hasPostive = false;
            bool hasNagetive = false;
            double runSpeed = this.mSpeed;

            AxisValue[2] = MotionBoard.FindOriginal(iAxis);

            _mb.SetupJog(iAxis, acc, dcc, smoothCoef);
            for (int iActNum = 0; iActNum < 2; iActNum++)
            {
                MotionBoard.SetLimit(iAxis, 2);
                _mb.JogMove(iAxis, runSpeed);

                while (true)
                {
                    int istate = MotionBoard.CheckAxisState(iAxis);

                    if (istate == 1 && !hasPostive)
                    {
                        // this is the Postive limit 
                        hasPostive = true;
                        MotionBoard.JogStop(iAxis);
                        int CoordValue = MotionBoard.ReadAxisCoordinate(iAxis);

                        MotionBoard.ReleaseLimit(iAxis, 2);

                        if (runSpeed > 0)
                        {
                            AxisValue[0] = Abs(CoordValue);
                        }
                        else
                        {
                            AxisValue[0] = -Abs(CoordValue);
                        }

                        runSpeed *= -1;
                        _mb.JogMove(iAxis, runSpeed);
                        Thread.Sleep(2000);
                        break;
                    }
                    else if (istate == 2 && !hasNagetive)
                    {
                        // this is the negative limit
                        hasNagetive = true;
                        MotionBoard.JogStop(iAxis);
                        int CoordValue = MotionBoard.ReadAxisCoordinate(iAxis);

                        MotionBoard.ReleaseLimit(iAxis, 2);
                        if (runSpeed > 0)
                        {
                            AxisValue[1] = Abs(CoordValue);
                        }
                        else
                        {
                            AxisValue[1] = -Abs(CoordValue);
                        }

                        runSpeed *= -1;
                        _mb.JogMove(iAxis, runSpeed);
                        Thread.Sleep(2000);
                        break;
                    }
                    else if (istate == 3)
                    {
                        return false;
                    }

                    Thread.Sleep(10);
                }
            }

            MotionBoard.JogStop(iAxis);
            Thread.Sleep(50);

            cmo.RecordCoordinate(AxisValue, iAxis);
            return true;
        }
    }

    public delegate void FinAxisCallBack(int[] axisValue, int axisIndex);
}
