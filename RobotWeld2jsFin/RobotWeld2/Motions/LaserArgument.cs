using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// The laser argument for directly driving laser on and off
    /// </summary>
    public class LaserArgument
    {
        private readonly int _power;
        private readonly int _frequency;
        private readonly int _width;
        private readonly int _duty;

        private readonly double _rise;
        private readonly double _fall;
        private readonly double _airIn;
        private readonly double _airOut;
        private readonly double _holdtime;
        private readonly double _wiretime;

        public LaserArgument()
        {
            _power = 0;
            _frequency = 0;
            _width = 0;
            _duty = 0;
            _rise = 0;
            _fall = 0;
            _airIn = 0;
            _airOut = 0;
            _holdtime = 0;
            _wiretime = 0;
        }

        public LaserArgument(LaserWeldParameter lwp)
        {
            _power = lwp.LaserPower;
            _frequency = lwp.Frequency;
            _width = lwp.PulseWidth;
            _duty = lwp.DutyCycle;
            _rise = lwp.LaserRise;
            _fall = lwp.LaserFall;
            _airIn = lwp.AirIn;
            _airOut = lwp.AirOut;
            _holdtime = lwp.LaserHoldTime;
            _wiretime = lwp.WireTime;
        }

        public bool WholeEqual(LaserArgument lsa)
        {
            if (_power == lsa.Power && _frequency == lsa.Frequency && _width == lsa.Width
                && _duty == lsa.Duty && _rise == lsa.Rise && _fall == lsa.Fall && _airIn == lsa.AirIn
                && _airOut == lsa.AirOut && _holdtime == lsa.Holdtime && _wiretime == lsa.Wiretime)
                return true;
            else
                return false;
        }

        public int Power
        {
            get => _power;
        }

        public int Frequency
        {
            get => _frequency;
        }

        public int Duty
        {
            get => _duty;
        }

        public int Width
        {
            get => _width;
        }

        public double Rise
        {
            get => _rise;
        }

        public double Fall
        {
            get => _fall;
        }

        public double AirIn
        {
            get => _airIn;
        }

        public double AirOut
        {
            get => _airOut;
        }

        public double Holdtime
        {
            get => _holdtime;
        }

        public double Wiretime
        {
            get => _wiretime;
        }
    }
}
