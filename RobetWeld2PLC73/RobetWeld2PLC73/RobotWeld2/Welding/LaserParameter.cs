///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) new point definition with laser paremeter
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld2.Welding
{
    public class LaserParameter
    {
        private int _power;
        private int _frequency;
        private int _pulse;
        private int _duty;
        private int _wobble;
        private int _weldspeed;
        private int _leap;

        public LaserParameter() { }

        public int Power
        {
            get => _power;
            set => _power = value;
        }

        public int Frequency
        {
            get => _frequency;
            set => _frequency = value;
        }

        public int Pulse
        {
            get => _pulse;
            set => _pulse = value;
        }

        public int Duty
        {
            get => _duty;
            set => _duty = value;
        }

        public int Wobble
        {
            get => _wobble; 
            set => _wobble = value;
        }

        public int WeldSpeed
        {
            get => _weldspeed; 
            set => _weldspeed = value;
        }

        public int Leap
        {
            get => _leap; 
            set => _leap = value;
        }
    }
}
