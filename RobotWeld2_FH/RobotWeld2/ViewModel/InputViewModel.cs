using System.ComponentModel;
using System.Runtime.CompilerServices;
using RobotWeld2.Motions;

namespace RobotWeld2.ViewModel
{
    /// <summary>
    /// the view model for input laser parameter
    /// </summary>
    public class InputViewModel : INotifyPropertyChanged
    {
        private int _laserPower;
        private int _frequency;
        private int _pulse;
        private int _duty;
        private int _rise;
        private int _fall;
        private int _hold;
        private int _wiretime;
        private int _airin;
        private int _airout;
        private double _weldSpeed;
        private double _wobbleSpeed;
        private double _leapSpeed;
        private int _weldNum;

        public InputViewModel() { }

        public int LaserPower
        {
            get { return _laserPower; }
            set
            {
                int maxpower = MotionOperate.GetMaxPower();
                if (value <=  maxpower && value >= 0)
                {
                    _laserPower = value;
                    OnPropertyChanged(nameof(LaserPower));
                }
            }
        }

        public int Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                OnPropertyChanged(nameof(Frequency));
            }
        }

        public int LaserPulseWidth
        {
            get { return _pulse; }
            set
            {
                _pulse = value;
                OnPropertyChanged(nameof(LaserPulseWidth));
            }
        }

        public int LaserDutyCycle
        {
            get { return _duty; }
            set
            {
                _duty = value;
                OnPropertyChanged(nameof(LaserDutyCycle));
            }
        }

        public int LaserRise
        {
            get { return _rise; }
            set
            {
                _rise = value;
                OnPropertyChanged(nameof(LaserRise));
            }
        }

        public int LaserFall
        {
            get { return _fall; }
            set
            {
                _fall = value;
                OnPropertyChanged(nameof(LaserFall));
            }
        }

        public int LaserHoldtime
        {
            get { return _hold; }
            set
            {
                _hold = value;
                OnPropertyChanged(nameof(LaserHoldtime));
            }
        }

        public int WireTime
        {
            get { return _wiretime; }
            set
            {
                _wiretime = value;
                OnPropertyChanged(nameof(WireTime));
            }
        }

        public int AirIn
        {
            get { return _airin; }
            set
            {
                _airin = value;
                OnPropertyChanged(nameof(AirIn));
            }
        }

        public int AirOut
        {
            get { return _airout; }
            set
            {
                _airout = value;
                OnPropertyChanged(nameof(AirOut));
            }
        }

        public double WeldSpeed
        {
            get { return _weldSpeed; }
            set
            {
                _weldSpeed = value;
                OnPropertyChanged(nameof(WeldSpeed));
            }
        }

        public double WobbleSpeed
        {
            get { return _wobbleSpeed; }
            set
            {
                _wobbleSpeed = value;
                OnPropertyChanged(nameof(WobbleSpeed));
            }
        }

        public double LeapSpeed
        {
            get
            {
                if (_leapSpeed >= 0 && _leapSpeed <= 500)
                    return _leapSpeed;
                else
                    return 0;
            }
            set
            {
                if (value >= 0 && value <= 500)
                {
                    _leapSpeed = value;
                }
                else
                {
                    _leapSpeed = 0;
                }
                OnPropertyChanged(nameof(LeapSpeed));
            }
        }

        public int WeldNum
        {
            get { return _weldNum; }
            set
            {
                _weldNum = value;
                OnPropertyChanged(nameof(WeldNum));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
