///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld3.ViewModel
{
    /// <summary>
    /// View Model for SuperUser page.
    /// </summary>
    internal class SuperUserViewModel : INotifyPropertyChanged
    {
        private double _air;
        private double _rise;
        private double _fall;
        private double _speed;
        private double _backSpeed;
        private double _back;
        private double _refeed;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double ProtectAir
        {
            get { return _air; }
            set { _air = value; OnPropertyChanged(); }
        }

        public double RiseEdge
        {
            get { return _rise; }
            set { _rise = value; OnPropertyChanged(); }
        }

        public double FallEdge
        {
            get { return _fall; }
            set { _fall = value; OnPropertyChanged(); }
        }

        public double FeedSpeed
        {
            get { return _speed; }
            set { _speed = value; OnPropertyChanged(); }
        }

        public double BackSpeed
        {
            get { return _backSpeed; }
            set { _backSpeed = value; OnPropertyChanged(); }
        }

        public double BackLength
        {
            get { return _back; }
            set { _back = value; OnPropertyChanged(); }
        }

        public double RefeedLength
        {
            get { return _refeed; }
            set { _refeed= value; OnPropertyChanged(); }
        }
    }
}
