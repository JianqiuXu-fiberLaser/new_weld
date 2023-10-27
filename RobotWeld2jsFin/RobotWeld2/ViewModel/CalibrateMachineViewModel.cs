using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RobotWeld2.ViewModel
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

        private double progressScale;

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
    }
}
