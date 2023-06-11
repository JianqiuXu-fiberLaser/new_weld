using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotWeld.Welding
{
    /// <summary>
    /// The laser parameters to configure the motion card
    /// </summary>
    public class LaserParameter
    {
        private int _maxPower;
        private int _power;
        private int _onDelay;
        private int _offDelay;
        private long _frequency;
        private double _puslewidth;
        private double _rise;
        private double _fall;

        public LaserParameter() { }

        public int MaxPower 
        { 
            get { return _maxPower;} 
            set { _maxPower = value; }
        }

        public int Power
        {
            get { return _power;}
            set { _power = value; }
        }

        public int OnDelay
        {
            get { return _onDelay;}
            set { _onDelay = value; }
        }

        public int OffDelay
        {
            get { return _offDelay; }
            set { _offDelay = value; }
        }

        public long Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        public double Pulsewidth
        {
            get { return _puslewidth;}
            set { _puslewidth = value; }
        }

        public double Rise
        {
            get { return _rise; }
            set { _rise = value; }
        }

        public double Fall
        {
            get { return _fall; }
            set { _fall = value; }
        }

        public int Mode
        { get; set; }
    }
}
