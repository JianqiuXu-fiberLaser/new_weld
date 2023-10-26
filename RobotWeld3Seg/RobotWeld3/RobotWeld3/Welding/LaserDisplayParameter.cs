///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver.2.2: new added class to display laser parameters.
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.Welding
{
    class LaserDisplayParameter
    {
        private int _power;
        private int _frequency;
        private int _duty;

        private double _wobble;
        private double _speed;

        internal LaserDisplayParameter() { }

        /// <summary>
        /// Deep copy of laser parameter.
        /// </summary>
        /// <param name="ldp"></param>
        internal LaserDisplayParameter(LaserDisplayParameter ldp)
        {
            _power = ldp.Power;
            _frequency = ldp.Frequency;
            _duty = ldp.Duty;
            _wobble = ldp.Wobble;
            _speed = ldp.Speed;
        }

        /// <summary>
        /// Compare to laser and frequency between two laser parameter, 
        /// Because the wobble can not be adjusted from point to point, it is
        /// consider the wobble as a global parameter, and is not compared.
        /// </summary>
        /// <param name="ldp"> LaserDisplayParameter </param>
        /// <returns> True: equals </returns>
        public bool EqualsTo(LaserDisplayParameter ldp)
        {
            if (_power == ldp.Power && _frequency == ldp.Frequency && _duty == ldp.Duty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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

        public int Duty
        {
            get => _duty; 
            set => _duty = value;
        }

        public double Wobble
        {
            get => _wobble;
            set => _wobble = value;
        }

        public double Speed
        {
            get => _speed;
            set => _speed = value;
        }
    }
}
