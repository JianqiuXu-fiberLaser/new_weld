using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RobotWeld.LaserOperation;
using RobotWeld.Weldding;
using RobotWeld.AlgorithmsBase;

namespace RobotWeld.ViewModel
{
    /// <summary>
    /// The View data of InputLaserParameter dialog for laser paremeters
    /// </summary>
    
    [Serializable]
    public class InputLaserParameterViewModel : INotifyPropertyChanged
    {
        private LaserParameter _laserParameterView;
        private MetalMaterial _metalMaterial;

        public InputLaserParameterViewModel()
        {

            _laserParameterView = new LaserParameter();
            _metalMaterial = new MetalMaterial();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int LaserFrequecy
        {
            get { return _laserParameterView.Frequency; }
            set
            {
                _laserParameterView.Frequency = value;
                OnPropertyChanged();
            }
        }

        public int LaserPulseWidth
        {
            get { return _laserParameterView.PulseWidth; }
            set
            {
                _laserParameterView.PulseWidth = value;
                OnPropertyChanged();
            }
        }

        public int LaserDutyCycle
        {
            get { return _laserParameterView.DutyCycle; }
            set
            {
                _laserParameterView.DutyCycle = value;
                OnPropertyChanged();
            }
        }

        public double LaserPower
        {
            get { return _laserParameterView.LaserPower; }
            set
            {
                _laserParameterView.LaserPower = value;
                OnPropertyChanged();
            }
        }

        public double AirIn
        {
            get { return _laserParameterView.AirIn; }
            set
            {
                _laserParameterView.AirIn = value;
                OnPropertyChanged();
            }
        }

        public double AirOut
        {
            get { return _laserParameterView.AirOut; }
            set
            {
                _laserParameterView.AirOut = value;
                OnPropertyChanged();
            }
        }

        public double LaserFall
        {
            get { return _laserParameterView.LaserFall; }
            set
            {
                _laserParameterView.LaserFall = value;
                OnPropertyChanged();
            }
        }

        public double LaserRise
        {
            get { return _laserParameterView.LaserRise; }
            set
            {
                _laserParameterView.LaserRise = value;
                OnPropertyChanged();
            }
        }

        public double LaserHoldtime
        {
            get { return _laserParameterView.LaserHoldtime; }
            set
            {
                _laserParameterView.LaserHoldtime = value;
                OnPropertyChanged();
            }
        }

        public double WireTime
        {
            get { return _laserParameterView.WireTime; }
            set
            {
                _laserParameterView.WireTime = value;
                OnPropertyChanged();
            }
        }

        // weld parameters
        public double WeldSpeed
        {
            get { return _metalMaterial.WeldSpeed; }
            set
            {
                _metalMaterial.WeldSpeed = value;
                OnPropertyChanged();
            }
        }

        public double WobbleSpeed
        {
            get { return _metalMaterial.WobbleSpeed; }
            set
            {
                _metalMaterial.WobbleSpeed = value;
                OnPropertyChanged();
            }
        }
    }
}
