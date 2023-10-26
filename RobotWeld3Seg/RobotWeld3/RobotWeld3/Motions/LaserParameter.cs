///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: All information about laser.
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.Motions
{
    /// <summary>
    /// The core parameter of laser, for display, calculation and keep data
    /// WireTime: the time to cut wire.
    /// WireLength: withdraw back length
    /// WireBack: withdraw time
    /// </summary>

    public class LaserParameter
    {
        private int _power;
        private int _frequency;
        private int _dutyCycle;

        private double _laserRise;
        private double _laserFall;
        private double _air;
        private double _laserHoldtime;

        // time to burning out wire
        private double _wireTime;
        
        // feed speed
        private double _wireSpeed;
        
        // re-feed length
        private double _wireLength;
        
        // withdraw back wire length
        private double _wireBack;

        public LaserParameter(int p = 0)
        {
            _power = p;
        }

        public int Power
        {
            get { return _power; }
            set { _power = value; }
        }

        public int Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
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

        public double Air
        {
            get { return _air; }
            set { _air = value; }
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
