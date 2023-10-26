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
    /// View model for Laser Type page.
    /// </summary>
    public class LaserTypeViewModel : INotifyPropertyChanged
    {
        private int _laserTypeIndex;
        private int _maxLaserPower;

        public LaserTypeViewModel() { }

        public int LaserTypeIndex
        {
            get { return _laserTypeIndex; }
            set { _laserTypeIndex = value; OnPropertyChanged(); }
        }

        public int MaxLaserPower
        {
            get { return _maxLaserPower; }
            set { _maxLaserPower = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
