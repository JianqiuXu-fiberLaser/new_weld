using System;

namespace RobotWeld.LaserOperation
{
    /// <summary>
    /// The core parameter of laser, for display and calculation
    /// </summary>

    public class LaserParameter
    {
        private readonly double _maxPower = 1000;

        private double _laserPower = 0;
        private int _frequency = 0;
        private int _pulseWidth = 0;
        private int _dutyCycle = 0;
        private double _laserRise;
        private double _laserFall = 0;
        private double _airIn = 0;
        private double _airOut = 0;
        private double _laserHoldtime = 0;
        private double _wireTime = 0;
        private double _wireSpeed = 0;
        private double _wireLength = 0;
        private double _wireBack = 0;

        public LaserParameter(double maxPower)
        {
            try
            {
                if (maxPower <= 0)
                    throw new ArgumentException("错误的激光功率！");
            }
            catch (ArgumentException)
            {
                _maxPower = 1000;
            }
            finally
            {
                _maxPower = maxPower;
            }
        }

        public LaserParameter()
        {
            // default for 1000W power
        }

        public double LaserPower
        {
            get { return _laserPower; }
            set
            {
                value /= _maxPower;
                value = (value <= 0 ? 0 : value);
                value = (value > 100 ? 100 : value);
                _laserPower = value;
            }
        }

        public int Frequency
        { get { return _frequency; } set { _frequency = value; } }

        public int PulseWidth
        {
            get { return _pulseWidth; }
            set
            {
                _pulseWidth = value;
            }
        }

        public int DutyCycle
        {
            get { return _dutyCycle; }
            set
            {
                _dutyCycle = value;
            }
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

        public double LaserHoldtime
        {
            get { return _laserHoldtime; }
            set
            {
                _laserHoldtime = value;
            }
        }

        public double WireTime
        {
            get { return _wireTime; }
            set { _wireTime = value; }
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

        public double WireSpeed
        {
            get { return _wireSpeed; }
            set { _wireSpeed = value; }
        }

        public double WireLength
        {
            get { return _wireLength; }
            set { _wireLength = value; }
        }

        public double WireBack
        {
            get { return _wireBack; }
            set { _wireBack = value; }
        }
    }
}
