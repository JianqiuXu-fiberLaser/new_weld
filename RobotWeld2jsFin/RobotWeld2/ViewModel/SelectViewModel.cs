using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
{
    public class SelectViewModel : INotifyPropertyChanged
    {
        private string _materialName = string.Empty;
        private int _materialIndex;
        private double _wireDiameter;
        private double _sheetThickness;

        public SelectViewModel() { }

        public int MaterialIndex
        {
            get { return _materialIndex; }
            set 
            { 
                _materialIndex = value;
                OnPropertyChanged(nameof(MaterialIndex));
            }
        }

        public string MaterialName
        {
            get { return _materialName; }
            set
            {
                _materialName = value;
                OnPropertyChanged(nameof(MaterialName));
            }
        }

        public double WireDiameter
        { 
            get { return _wireDiameter; } 
            set
            { 
                if (_wireDiameter != value)
                {
                    _wireDiameter = value;
                    OnPropertyChanged(nameof(WireDiameter));
                }
            }
        }

        public double SheetThickness
        {
            get { return _sheetThickness; }
            set
            {
                if (_sheetThickness != value)
                {
                    _sheetThickness = value;
                    OnPropertyChanged(nameof(SheetThickness));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
