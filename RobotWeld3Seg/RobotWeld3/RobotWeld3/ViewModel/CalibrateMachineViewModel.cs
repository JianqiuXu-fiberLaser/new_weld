///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld3.ViewModel
{
    public class CalibrateMachineViewModel : INotifyPropertyChanged
    {
        private int zNegLimit;
        private int zPosLimit;
        private int zDirection;
        private int _zPulse;

        private int yNegLimit;
        private int yPosLimit;
        private int yDirection;
        private int _yPulse;

        private int xNegLimit;
        private int xPosLimit;
        private int xDirection;
        private int _xPulse;
        private int _aPulse;

        private int _pedalTrigger;
        private int _protectedAir;
        private int _laserEnable;
        private int _wobble;
        private int _feedWire;
        private int _withdraw;
        private int _wireDac;

        private double progressScale;
        private int _aaxisState;
        private bool _aaxisBool;

        public CalibrateMachineViewModel() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int ZNegLimit
        {
            get { return zNegLimit; }
            set
            {
                zNegLimit = value;
                OnPropertyChanged();
            }
        }

        public int ZPosLimit
        {
            get { return zPosLimit; }
            set
            {
                zPosLimit = value;
                OnPropertyChanged();
            }
        }

        public int ZDirection
        {
            get { return zDirection; }
            set
            {
                zDirection = value;
                OnPropertyChanged();
            }
        }

        public int ZPulse
        {
            get { return _zPulse; }
            set
            {
                _zPulse = value;
                OnPropertyChanged();
            }
        }

        public int YNegLimit
        {
            get { return yNegLimit; }
            set
            {
                yNegLimit = value;
                OnPropertyChanged();
            }
        }

        public int YPosLimit
        {
            get { return yPosLimit; }
            set
            {
                yPosLimit = value;
                OnPropertyChanged();
            }
        }

        public int YDirection
        {
            get { return yDirection; }
            set
            {
                yDirection = value;
                OnPropertyChanged();
            }
        }

        public int YPulse
        {
            get { return _yPulse; }
            set
            {
                _yPulse = value;
                OnPropertyChanged();
            }
        }

        public int XNegLimit
        {
            get { return xNegLimit; }
            set
            {
                xNegLimit = value;
                OnPropertyChanged();
            }
        }

        public int XPosLimit
        {
            get { return xPosLimit; }
            set
            {
                xPosLimit = value;
                OnPropertyChanged();
            }
        }

        public int XDirection
        {
            get { return xDirection; }
            set
            {
                xDirection = value;
                OnPropertyChanged();
            }
        }

        public int XPulse
        {
            get { return _xPulse; }
            set
            {
                _xPulse = value;
                OnPropertyChanged();
            }
        }

        public int APulse
        {
            get { return _aPulse; }
            set
            {
                _aPulse = value;
                OnPropertyChanged();
            }
        }

        public double ProgressScale
        {
            get => progressScale;
            set
            {
                progressScale = value;
                OnPropertyChanged();
            }
        }

        public int AaxisState
        {
            get => _aaxisState;
            set
            {
                _aaxisState = value;
                OnPropertyChanged();
            }
        }

        public int PedalTrigger
        {
            get => _pedalTrigger;
            set
            {
                _pedalTrigger = value;
                OnPropertyChanged();
            }
        }

        public int ProtectedAir
        {
            get => _protectedAir;
            set
            {
                _protectedAir = value;
                OnPropertyChanged();
            }
        }

        public bool AaxisBool
        {
            get => _aaxisBool;
            set
            {
                _aaxisBool = value;
                OnPropertyChanged();
            }
        }

        public int LaserEnable
        {
            get => _laserEnable;
            set
            {
                _laserEnable = value;
                OnPropertyChanged();
            }
        }

        public int Wobble
        {
            get => _wobble;
            set
            {
                _wobble = value;
                OnPropertyChanged();
            }
        }

        public int FeedWire
        {
            get => _feedWire;
            set
            {
                _feedWire = value;
                OnPropertyChanged();
            }
        }

        public int Withdraw
        {
            get => _withdraw;
            set
            {
                _withdraw = value;
                OnPropertyChanged();
            }
        }

        public int WireDac
        {
            get => _wireDac;
            set
            {
                _wireDac = value;
                OnPropertyChanged();
            }
        }
    }
}
