///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.Welding;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// The core parameter of laser, for display, calculation and keep data
    /// </summary>

    public class LaserParameter
    {
        private int _maxPower;

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
        private double _wireSpeed;
        private double _wireLength;
        private double _wireBack;

        public LaserParameter(int maxPower)
        {
            _maxPower = maxPower;
        }

        public LaserParameter()
        {
            _maxPower = DaemonFile.MaxPower;
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

        public double LaserHoldtime
        {
            get { return _laserHoldtime; }
            set { _laserHoldtime = value; }
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

        public int MaxPower
        {
            get { return _maxPower; }
            set { _maxPower = value; }
        }
    }
}
