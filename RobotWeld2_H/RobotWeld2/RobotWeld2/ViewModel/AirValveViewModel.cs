using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Welding;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
{
    /// <summary>
    /// The view model for Air Valve user Control
    /// </summary>
    public class AirValveViewModel : INotifyPropertyChanged
    {
        private int _laserPower;
        private string? _materialType;
        private int _materialIndex;
        private double _sheetThickness;
        private double _wireDiameter;

        private int _y1Speed;
        private int _y2Speed;
        private int _c1Speed;
        private int _c2Speed;

        private string _errMsg = string.Empty;

        private Color _lreset; 
        private Color _rreset;

        public AirValveViewModel()
        {
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

        public int LaserPower
        {
            get { return _laserPower; }
            set
            {
                _laserPower = value;
                OnPropertyChanged(nameof(LaserPower));
            }
        }

        public string MaterialType
        {
            get { return _materialType ?? "碳钢"; }
        }

        public int MaterialIndex
        {
            get { return _materialIndex; }
            set
            {
                _materialIndex = value;
                MaterialList malist = new();
                _materialType = malist.GetName(_materialIndex);
                OnPropertyChanged(nameof(MaterialType));
            }
        }

        public double SheetThickness
        {
            get { return _sheetThickness; }
            set
            {
                _sheetThickness = value;
                OnPropertyChanged(nameof(SheetThickness));
            }
        }

        public double WireDiameter
        {
            get { return _wireDiameter; }
            set
            {
                _wireDiameter = value;
                OnPropertyChanged(nameof(WireDiameter));
            }
        }

        public int Y1Speed
        {
            get { return _y1Speed; }
            set
            {
                _y1Speed = value; OnPropertyChanged(nameof(Y1Speed));
            }
        }

        public int Y2Speed
        {
            get { return _y2Speed; }
            set
            {
                _y2Speed = value; OnPropertyChanged(nameof(Y2Speed));
            }
        }

        public int C2Speed
        {
            get { return _c2Speed; }
            set
            {
                _c2Speed = value; OnPropertyChanged(nameof(C2Speed));
            }
        }

        public int C1Speed
        {
            get { return _c1Speed; }
            set
            {
                _c1Speed = value; OnPropertyChanged(nameof(C1Speed));
            }
        }

        public string ErrMsg
        {
            get { return _errMsg ?? "工作准备中"; }
            set
            {
                _errMsg = value;
                OnPropertyChanged(nameof(ErrMsg));
            }
        }

        public Color Lreset
        {
            get { return _lreset; }
            set { _lreset = value; OnPropertyChanged(); }
        }

        public Color Rreset
        {
            get { return _rreset; }
            set { _rreset = value; OnPropertyChanged(); }
        }
    }
}
