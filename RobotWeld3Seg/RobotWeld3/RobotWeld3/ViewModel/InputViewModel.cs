///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver 2.2: revise the point list to match single workpackage file.
//          (1) using laserDisplayParameter for display on UI only.
//
///////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Runtime.CompilerServices;
using RobotWeld3.Motions;

namespace RobotWeld3.ViewModel
{
    /// <summary>
    /// the view model for input laser parameter
    /// </summary>
    public class InputViewModel : INotifyPropertyChanged
    {
        private int _laserPower;
        private int _frequency;
        private int _duty;
        
        private double _thickness;
        private double _wireDiameter;

        private double _speed;
        private double _wobbleSpeed;

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

        public int LaserDutyCycle
        {
            get { return _duty; }
            set
            {
                _duty = value;
                OnPropertyChanged(nameof(LaserDutyCycle));
            }
        }

        public double Thickness
        {
            get => _thickness;
            set
            {
                _thickness = value;
                OnPropertyChanged(nameof(Thickness));
            }
        }

        public double WireDiameter
        {
            get => _wireDiameter;
            set
            {
                _wireDiameter = value;
                OnPropertyChanged(nameof(WireDiameter));
            }
        }

        public double Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                OnPropertyChanged(nameof(Speed));
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

        //
        //-- Response for the ICommand Events in the MainWindow --
        // required by INotifPropertyChanged interface, regist in the first
        //
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
