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
    public class IntersectViewModel : INotifyPropertyChanged
    {
        private double _upperRadius;
        private double _lowerRadius;
        private bool _clampDirection;
        private int _installStyle;

        public IntersectViewModel() { }
       
        public double LowerRadius
        {
            get { return _lowerRadius; }
            set
            {
                _lowerRadius = value;
                OnPropertyChanged(nameof(LowerRadius));
            }
        }

        public double UpperRadius
        {
            get { return _upperRadius; }
            set
            {
                _upperRadius = value;
                OnPropertyChanged(nameof(UpperRadius));
            }
        }

        public bool ClampDirection
        {
            get { return _clampDirection; }
            set { _clampDirection = value; OnPropertyChanged(); }
        }

        public int InstalStyleValue
        {
            get { return _installStyle; }
            set
            {
                _installStyle = value;
                OnPropertyChanged(nameof(InstalStyleValue));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
