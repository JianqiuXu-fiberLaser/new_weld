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
                _paraIndex = 0;
            }
            else if (index != 0)
            {
                _paraIndex = index;
            }
        }

        public LaserWeldParameter()
        {
            {
                DateTime dateTime2020 = new(2021, 6, 16, 0, 0, 0);

                DateTime DateNow = DateTime.Now;
                TimeSpan timespan = (DateNow - dateTime2020);
                _paraIndex = (int)timespan.TotalSeconds;
            }
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
