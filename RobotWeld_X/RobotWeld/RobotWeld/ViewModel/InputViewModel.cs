using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld.ViewModel
{
    /// <summary>
    /// the view model for input laser parameter
    /// </summary>
    public class InputViewModel : INotifyPropertyChanged
    {
        private const int _MAXPOWER = 1500;
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
        private int _weldSpeed;
        private int _wobbleSpeed;

        public InputViewModel() { }

        public int LaserPower
        {
            get { return _laserPower; }
            set 
            {
                if (value <= _MAXPOWER && value >= 0)
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

        public int WeldSpeed
        {
            get { return _weldSpeed; }
            set
            {
                _weldSpeed = value;
                OnPropertyChanged(nameof(WeldSpeed));
            }
        }

        public int WobbleSpeed
        {
            get { return _wobbleSpeed; }
            set
            {
                _wobbleSpeed = value;
                OnPropertyChanged(nameof(WobbleSpeed));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
