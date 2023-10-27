using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
{
    public class IntersectViewModel : INotifyPropertyChanged
    {
        private double _upperRadius;
        private double _lowerRadius;
        private bool _clampDirection;
        private int _instalStyle;

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
            get { return _instalStyle; }
            set
            {
                _instalStyle = value;
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
