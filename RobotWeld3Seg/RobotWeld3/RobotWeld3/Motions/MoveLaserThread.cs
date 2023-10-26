///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: new added class to display laser parameters.
// Ver. 3.0: (1) two array to operate the laser.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld3.Motions
{
    public class MoveLaserThread
    {
        private bool _ActionFinish;

        // Laser parameters
        private List<double[]>? _lp;

        // Data for lasing
        private List<int[]>? _lasSec;

        // Move function
        private readonly LaserOperate _lasope;

        /// <summary>
        /// Laser thread when the machine is moving.
        /// </summary>
        public MoveLaserThread()
        {
            _lasope = new();
        }

        /// <summary>
        /// Put in the laser section and laser parameters.
        /// </summary>
        /// <param name="lasSec"> Laser section information </param>
        /// <param name="lp"> Laser parameters </param>
        public void PutParmeter(List<int[]> lasSec, List<double[]> lp)
        {
            _lasSec = lasSec;
            _lp = lp;
        }

        /// <summary>
        /// To switch on/off the laser thread.
        /// </summary>
        /// <param name="state"> Laser on/off state </param>
        public void SetActionFinish(bool state)
        {
            _ActionFinish = state;
        }

        /// <summary>
        /// Laser thread
        /// </summary>
        public void LaserThread()
        {
            if (_lasSec == null || _lp == null)
            {
                MainModel.AddInfo("激光线程参数错误");
                return;
            }

            int i = 0;
            int sectionAct;
            _ActionFinish = false;
            while (!_ActionFinish || i < _lasSec.Count)
            {
                sectionAct = MotionOperate.GetSegNo();
                if (i < _lasSec.Count - 1)
                {
                    if (sectionAct >= _lasSec[i][0] && sectionAct < _lasSec[i + 1][0])
                    {
                        LaserAction(_lasSec[i][1], _lp[i]);
                        i++;
                    }
                }
                else
                {
                    if (sectionAct >= _lasSec[^1][0])
                    {
                        LaserAction(_lasSec[^1][1], _lp[i]);
                        break;
                    }
                }
            }

            // Make sure the laser off at the last step.
            LaserDriver.LaserOff();
            MotionOperate.CloseAir();
        }

        //-------------------------------------------------------------
        // Laser actions with analysis methods
        //-------------------------------------------------------------

        /// <summary>
        /// Laser action for various action numbers.
        /// Sometime, the laser action comes from two combined action.
        /// </summary>
        /// <param name="istate"> Laser state </param>
        /// <param name="lp"> Laser parameters </param>
        private void LaserAction(int istate, double[] lp)
        {
            if (istate < 10) SingleLaserAction(istate, lp);
            else
            {
                int i1 = istate / 10;
                int i2 = istate % 10;
                double[] p1;
                switch (i1)
                {
                    case 1:
                    case 4:
                    case 5:
                        p1 = new double[1];
                        p1[0] = lp[0];
                        break;
                    case 2:
                        p1 = new double[2];
                        p1[0] = lp[0];
                        p1[1] = lp[1];
                        break;
                    case 3:
                        p1 = new double[4];
                        p1[0] = lp[0];
                        p1[1] = lp[1];
                        p1[2] = lp[2];
                        p1[3] = lp[3];
                        break;
                    default:
                        p1 = new double[] { 0 };
                        break;
                }

                double[] p2;
                p2 = new double[lp.Length - p1.Length];
                for (int i = 0; i < p2.Length; i++)
                    p2[i] = lp[i + 1];

                SingleLaserAction(i1, p1);
                SingleLaserAction(i2, p2);
            }
        }

        /// <summary>
        /// Laser action with a single laser parameter.
        /// </summary>
        /// <param name="istate"> Laser state </param>
        /// <param name="lp"> Laser parameter </param>
        private void SingleLaserAction(int istate, double[] lp)
        {
            switch (istate)
            {
                case 1:
                    MotionOperate.OpenAir();
                    break;
                case 2:
                    SwitchOnLaser(lp[0], lp[1]);
                    break;
                case 3:
                    ChangeMode(lp[0], lp[1], lp[2], lp[3]);
                    break;
                case 4:
                    CloseAir(lp[0]);
                    break;
                case 5:
                    SwitchOffLaser(lp[0]);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Open laser: power and rise time
        /// </summary>
        /// <param name="pow"></param>
        /// <param name="rise"></param>
        private void SwitchOnLaser(double pow, double rise)
        {
            _lasope.ChangePower((int)pow, r: (int)rise);
            _lasope.SlowLaserOn();
        }

        /// <summary>
        /// Close Laser: fall time
        /// </summary>
        /// <param name="fall"></param>
        private void SwitchOffLaser(double fall)
        {
            _lasope.SetEdge(f: (int)fall);
            _lasope.SlowLaserOff();
        }

        /// <summary>
        /// Change laser mode: power, rise, frequency and duty cycle
        /// </summary>
        /// <param name="pow"> power </param>
        /// <param name="rise"> Rise time </param>
        /// <param name="freq"> Frequency </param>
        /// <param name="duty"> Duty cycle </param>
        private void ChangeMode(double pow, double rise, double freq, double duty)
        {
            _lasope.SetParameter((int)pow, (int)freq, (int)duty, r: (int)rise);
            _lasope.SlowLaserOn();
        }

        /// <summary>
        /// After sleep the air time, then close air.
        /// </summary>
        /// <param name="air"> Air time </param>
        private static void CloseAir(double air)
        {
            new Thread(s =>
            {
                Thread.Sleep((int)air);
                MotionOperate.CloseAir();
            }).Start();
        }
    }
}
