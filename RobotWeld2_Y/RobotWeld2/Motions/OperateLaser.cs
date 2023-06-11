using RobotWeld2.AlgorithmsBase;
using System;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Laser operation controlled by the analog voltage 
    /// </summary>
    public class OperateLaser : MotionCard
    {
        //
        // Constants
        //
        private const int POWER_CODE = 32767;
        private const int LASER_CHANNEL = 0;

        //
        // variables in operation laser
        //
        private int _workmode;

        private long _firstTime;
        private long _lastTime;

        private short[] SubPowerCode;
        private int _maxPower;

        private LaserParameter _laserParameter;
        public SWLaser SWLASER;

        public OperateLaser()
        {
            SubPowerCode = new short[5];
            _laserParameter = new();
        }

        /// <summary>
        /// Setup the laser parameters
        /// </summary>
        /// <param name="lp"></param>
        public void SetupLaserParameter(LaserParameter lp)
        {
            _laserParameter.LaserPower = lp.LaserPower;
            _laserParameter.Frequency = lp.Frequency;
            _laserParameter.PulseWidth = lp.PulseWidth;
            _laserParameter.DutyCycle = lp.DutyCycle;
            _laserParameter.LaserRise = lp.LaserRise;
            _laserParameter.LaserFall = lp.LaserFall;
            _laserParameter.LaserHoldtime = lp.LaserHoldtime;
            _laserParameter.AirIn = lp.AirIn;
            _laserParameter.AirOut = lp.AirOut;
            _laserParameter.WireLength = lp.WireLength;
            _laserParameter.WireTime = lp.WireTime;
            _laserParameter.WireBack = lp.WireBack;
            _laserParameter.WireSpeed = lp.WireSpeed;

            PowerSubdivision();
        }

        public void SetupLaserPower(int power)
        {
            _laserParameter.LaserPower = power;
            PowerSubdivision();
        }

        public int MaxPower
        {
            get { return _maxPower; }
            set { _maxPower = value; }
        }

        //
        // Laser switch state
        //
        public enum SWLaser
        {
            CLOSE = -1,
            ON = 1,
            OFF = 0,
            HOLD = 2,
        }

        public void GetHardware(ushort devHandle, ushort pcrdHandle, ushort[] axisHandle)
        {
            this.devHandle = devHandle;
            this.pcrdHandle = pcrdHandle;
            this.axisHandle = axisHandle;
        }

        /// <summary>
        /// Setup the laser in the first connection to laser
        /// </summary>
        /// <param name="mode"> the laser working mode, 0: CW; 1: QCW; 2: time array </param>
        public void SetupLaser(int mode)
        {
            if (_laserParameter == null) return;

            if (mode == 0 & _laserParameter.Frequency == 0)
            {
                // CW
                NMC_SetDacMode(devHandle, 256, 1);
                NMC_LaserSetMode(devHandle, BASIC_OUTPUT_MODE, 0);
                NMC_LaserSetOutputType(devHandle, LASER_PWM_DUTY, 0, 1000, 0);
                NMC_LaserSetPowerEx(devHandle, 100, 0);
            }
            else if (_workmode == 1 || _laserParameter.Frequency != 0)
            {
                _workmode = 1;    // QCW
            }

            // setup the laser according to different modes
  /*          if (_workmode == 0)
            {

  
            }
            else if (_workmode == 1)
            {
                NMC_LaserSetMode(devHandle, SHIO_OUTPUT_MODE, LASER_CHANNEL);
            }
            else
            {
                NMC_LaserSetMode(devHandle, TIME_ARRAY_OUTPUT_MODE, LASER_CHANNEL);
            }*/
        }

        //
        // Switch on laser
        //
        public void LaserSwitchOn()
        {
            if (_laserParameter == null) return;

            if (_workmode == 0)
            {
                if (_laserParameter.LaserRise == 0)
                {
                    SwitchOnNoRise();
                }
                else
                {
                    SwitchOnRise();
                }
            }
            else if (_workmode == 1)
            {
                // QCW
            }
        }

        //
        // On the laser without slow rising
        //
        private void SwitchOnNoRise()
        {
            NMC_LaserOnOff(devHandle, 1, 0);
            NMC_SetDac(devHandle, 256, SubPowerCode[4]);
        }

        //
        // Set the laser power to zero and Switch off the laser immediately
        //
        public bool LaserSwitchOff()
        {
            if (_laserParameter == null) { return false; }

            if (_workmode == 0)
            {
                if (_laserParameter.LaserFall == 0)
                {
                    return SwitchOffNoFall();
                }
                else
                    return SwitchOffFall();
            }
            else if (_workmode == 1)
            {
                // QCW
                return true;
            }
            else
            {
                // follow
                return false;
            }
        }

        private bool SwitchOffNoFall()
        {
            NMC_SetDac(devHandle, 256, 0);
            NMC_LaserOnOff(devHandle, 0, 0);

            return true;
        }

        private bool SwitchOffFall()
        {
            if (_laserParameter == null) { return true; }

            double Fall = _laserParameter.LaserFall;

            long leftTime = PassTime() - _lastTime;
            if (leftTime >= 0 && leftTime < Fall * 0.25)
            {
                NMC_SetDac(devHandle, 256, SubPowerCode[3]);
                return false;
            }
            else if ((leftTime >= Fall * 0.25) && (leftTime < Fall * 0.5))
            {
                NMC_SetDac(devHandle, 256, SubPowerCode[2]);
                return false;
            }
            else if ((leftTime >= Fall * 0.5) && (leftTime < Fall * 0.75))
            {
                NMC_SetDac(devHandle, 256, SubPowerCode[1]);
                return false;
            }
            else if (leftTime >= Fall * 0.75 && (leftTime > Fall))
            {
                // the last laser power is hold at 20%
                NMC_SetDac(devHandle, 256, SubPowerCode[0]);
                return false;
            }
            else
            {
                NMC_SetDac(devHandle, 256, 0);
                return true;
            }
        }

        //
        // On the laser with slow rising
        //
        private void SwitchOnRise()
        {
            if (_laserParameter == null) return;

            long passTime = PassTime() - _firstTime;

            double Rise = _laserParameter.LaserRise;

            if ((passTime >= 0) && (passTime <= Rise * 0.25))
            {
                NMC_LaserOnOff(devHandle, 1, LASER_CHANNEL);
                NMC_SetDac(devHandle, 256, SubPowerCode[0]);
            }
            else if ((passTime >= Rise * 0.25) && (passTime < Rise * 0.5))
            {
                NMC_SetDac(devHandle, 256, SubPowerCode[1]);
            }
            else if ((passTime >= Rise * 0.5) && (passTime < Rise * 0.75))
            {
                NMC_SetDac(devHandle, 256, SubPowerCode[2]);
            }
            else if ((passTime >= Rise * 0.75) && (passTime < Rise))
            {
                NMC_SetDac(devHandle, 256, SubPowerCode[3]);
            }
            else if (passTime >= Rise)
            {
                NMC_SetDac(devHandle, 256, SubPowerCode[4]);
            }
        }

        //
        // Caculate the subdivision for the laser power
        //
        private void PowerSubdivision()
        {
            if (_laserParameter == null) return;
            if (_maxPower == 0) { GiveMsg.Show("最大功率为零"); return; }

            int LaserPower = _laserParameter.LaserPower;
            SubPowerCode[0] = (short)(0.2 * LaserPower * POWER_CODE / _maxPower);
            SubPowerCode[1] = (short)(0.4 * LaserPower * POWER_CODE / _maxPower);
            SubPowerCode[2] = (short)(0.6 * LaserPower * POWER_CODE / _maxPower);
            SubPowerCode[3] = (short)(0.8 * LaserPower * POWER_CODE / _maxPower);
            SubPowerCode[4] = (short)(LaserPower * POWER_CODE / _maxPower);
        }

        /// <summary>
        /// Switch on the laser when press the button
        /// </summary>
        /// <param name="laserPower"> the laser output power </param>
        public void ClickLaserOn()
        {
            SwitchOnNoRise();
        }

        /// <summary>
        /// Switch off the laser when release the button
        /// </summary>
        public void ClickLaserOff()
        {
            SwitchOffNoFall();
        }

        //
        // the time pass in ms, used for slow rising and slow falling
        //
        private static long PassTime()
        {
            TimeSpan tSpan = DateTime.Now - new DateTime(2023, 4, 1, 0, 0, 0); ;
            return tSpan.Milliseconds;
        }

        //
        // the time pass in ms, used for slow rising and slow falling
        //
        public void FirstTime()
        {
            TimeSpan tSpan = DateTime.Now - new DateTime(2023, 4, 1, 0, 0, 0); ;
            _firstTime = tSpan.Milliseconds;
        }

        public void LastTime(long time)
        {
            _lastTime = time;
        }
    }
}