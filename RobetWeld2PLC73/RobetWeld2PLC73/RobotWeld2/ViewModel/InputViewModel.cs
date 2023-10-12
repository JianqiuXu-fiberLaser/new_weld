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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
{
    /// <summary>
    /// the view model for input laser parameter
    /// </summary>
    public class InputViewModel : INotifyPropertyChanged
    {
        private int _maxPower;
        private int _laserPower;
        private int _frequency;
        private int _pulse;
        private int _duty;
        private int _weldSpeed;
        private int _wobbleSpeed;
        private int _leapSpeed;

        public InputViewModel() { }

        public int MaxPower
        {
            get => _maxPower;
            set => _maxPower = value;
        }

        public int LaserPower
        {
            get { return _laserPower; }
            set 
            {
                if (value <= _maxPower && value >= 0)
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

        public int LeapSpeed
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
