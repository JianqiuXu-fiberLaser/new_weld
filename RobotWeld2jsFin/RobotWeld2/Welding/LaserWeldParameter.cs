using System;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Parameters in the laser welding
    /// </summary>
    public class LaserWeldParameter
    {
        private int _paraIndex;
        private int _laserPower;
        private int _frequency;
        private int _pulseWidth;
        private int _dutyCycle;

        private double _laserRise;
        private double _laserFall;
        private double _airIn;
        private double _airOut;
        private double _laserHoldtime;
        private double _wireTime;
        private double _weldSpeed;
        private double _wobbleSpeed;
        private double _leapSpeed;

        public LaserWeldParameter(int index)
        {
            if (index == 0)
            {
                DateTime dateTime2020 = new(2021, 6, 16, 0, 0, 0);

                DateTime DateNow = DateTime.Now;
                TimeSpan timespan = (DateNow - dateTime2020);
                _paraIndex = (int)timespan.TotalSeconds;
            }
            else if (index != 0)
            {
                _paraIndex = index;
            }
        }

        public LaserWeldParameter() { }

        public LaserWeldParameter(LaserWeldParameter lwp)
        {
            _paraIndex = lwp.ParaIndex;
            _laserPower = lwp.LaserPower;
            _pulseWidth = lwp.PulseWidth;
            _frequency = lwp.Frequency;
            _dutyCycle = lwp.DutyCycle;

            _laserRise = lwp.LaserRise;
            _laserFall = lwp.LaserFall;
            _airIn = lwp.AirIn;
            _airOut = lwp.AirOut;
            _laserHoldtime = lwp.LaserHoldTime;
            _wireTime = lwp.WireTime;

            _weldSpeed = lwp.WeldSpeed;
            _wobbleSpeed = lwp.WobbleSpeed;
            _leapSpeed = lwp.LeapSpeed;
        }

        /// <summary>
        /// Compare to laser and frequency between two laser parameter
        /// </summary>
        /// <param name="lwp"></param>
        /// <returns></returns>
        public bool EqualsTo(LaserWeldParameter lwp)
        {
            if (this.LaserPower == lwp.LaserPower && this.Frequency == lwp.Frequency && this.DutyCycle == lwp.DutyCycle)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void FullCopy(LaserWeldParameter lwp)
        {
            ParaIndex = lwp.ParaIndex;
            LaserPower = lwp.LaserPower;
            PulseWidth = lwp.PulseWidth;
            Frequency = lwp.Frequency;
            DutyCycle = lwp.DutyCycle;

            LaserRise = lwp.LaserRise;
            LaserFall = lwp.LaserFall;
            AirIn = lwp.AirIn;
            AirOut = lwp.AirOut;
            LaserHoldTime = lwp.LaserHoldTime;
            WireTime = lwp.WireTime;

            WeldSpeed = lwp.WeldSpeed;
            WobbleSpeed = lwp.WobbleSpeed;
            LeapSpeed = lwp.LeapSpeed;
        }

        public int ParaIndex
        {
            get { return _paraIndex; }
            set { _paraIndex = value; }
        }

        public int LaserPower
        {
            get { return _laserPower; }
            set { _laserPower = value; }
        }

        public int Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        public int PulseWidth
        {
            get { return _pulseWidth; }
            set { _pulseWidth = value; }
        }

        public int DutyCycle
        {
            get { return _dutyCycle; }
            set { _dutyCycle = value; }
        }

        public double LaserRise
        {
            get { return _laserRise; }
            set { _laserRise = value; }
        }

        public double LaserFall
        {
            get { return _laserFall; }
            set { _laserFall = value; }
        }

        public double AirIn
        {
            get { return _airIn; }
            set { _airIn = value; }
        }

        public double AirOut
        {
            get { return _airOut; }
            set { _airOut = value; }
        }

        public double LaserHoldTime
        {
            get { return _laserHoldtime; }
            set { _laserHoldtime = value; }
        }

        public double WireTime
        {
            get { return _wireTime; }
            set { _wireTime = value; }
        }

        public double LeapSpeed
        {
            get { return _leapSpeed; }
            set { _leapSpeed = value; }
        }

        public double WeldSpeed
        {
            get { return _weldSpeed; }
            set { _weldSpeed = value; }
        }

        public double WobbleSpeed
        {
            get { return _wobbleSpeed; }
            set { _wobbleSpeed = value; }
        }
    }
}
