using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
{
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
