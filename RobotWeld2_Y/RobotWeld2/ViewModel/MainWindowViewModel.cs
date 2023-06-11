using RobotWeld2.Welding;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RobotWeld2.ViewModel
{
    /// <summary>
    /// the parameter for display in the main windows.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _laserPower;
        private int _frequency;
        private int _pulse;
        private int _duty;
        private int _rise;
        private int _fall;
        private int _hold;
        private int _wire;
        private int _in;
        private int _out;
        private int _weld;
        private int _wobble;
        private int _leapSpeed;

        private string? _materialType;
        private int _materialIndex;
        private double _sheetThickness;
        private double _wireDiameter;

        private bool _linetype;
        private bool _laseronoff;
        private bool _inputPowerOk;

        private bool _pmFlag;
        private string _pmText = string.Empty;

        private bool _leftResetFlag;
        private bool _rightResetFlag;

        //
        // the plot and the point being chose
        //
        private int _x;
        private int _y;
        private int _z;
        private int _pointInfo;

        private string _errMsg = string.Empty;
        private string _pointList = string.Empty;

        //
        // pass the file information to the class of WeldFileAccess,
        // which deal with the record file.
        //
        public MainWindowViewModel() { }

        //
        //-- Response for the ICommand Events in the MainWindow --
        // required by INotifPropertyChanged interface, regist in the first
        //
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //
        //-- Define the RoutedCommand for the menu operation in MainWindows --
        //
        public static RoutedCommand RunCommand = new("Run", typeof(MainWindowViewModel));
        public static RoutedCommand ImportCommand = new("Import", typeof(MainWindowViewModel));

        public static RoutedCommand InputTraceCommand = new("InputTrace", typeof(MainWindowViewModel));
        public static RoutedCommand VaneWheelCommand = new("VaneWheel", typeof(MainWindowViewModel));
        public static RoutedCommand IntersectCommand = new("Intersect", typeof(MainWindowViewModel));
        public static RoutedCommand TopTraceCommand = new("TopTrace", typeof(MainWindowViewModel));
        public static RoutedCommand StageTraceCommand = new("StageTrace", typeof(MainWindowViewModel));
        public static RoutedCommand SpiralCommand = new("Spiral", typeof(MainWindowViewModel));
        public static RoutedCommand AutoTraceCommand = new("AutoTrace", typeof(MainWindowViewModel));

        public static RoutedCommand PresetParameterCommand = new("PresetParameter", typeof(MainWindowViewModel));
        public static RoutedCommand UserParameterCommand = new("UserParameter", typeof(MainWindowViewModel));
        public static RoutedCommand AutoParameterCommand = new("AutoParameter", typeof(MainWindowViewModel));
        public static RoutedCommand LaserDecodeCommand = new("LaserDecode", typeof(MainWindowViewModel));
        public static RoutedCommand LaserVersionCommand = new("LaserVersion", typeof(MainWindowViewModel));

        public static RoutedCommand SetupCommand = new("Setup", typeof(MainWindowViewModel));
        public static RoutedCommand GetHelpCommand = new("GetHelp", typeof(MainWindowViewModel));
        public static RoutedCommand ShowAboutCommand = new("ShowAbout", typeof(MainWindowViewModel));
        public static RoutedCommand RunVaneWheelCommand = new("RunVaneWheel", typeof(MainWindowViewModel));
        public static RoutedCommand LaserTypeCommand = new("LaserType", typeof(MainWindowViewModel));

        //-- The dependency properties to display in the MainWindow --
        public int LaserPower
        {
            get { return _laserPower; }
            set
            {
                _laserPower = value;
                OnPropertyChanged(nameof(LaserPower));
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

        public int PulseWidth
        {
            get { return _pulse; }
            set
            {
                _pulse = value;
                OnPropertyChanged(nameof(PulseWidth));
            }
        }

        public int DutyCycle
        {
            get { return _duty; }
            set
            {
                _duty = value;
                OnPropertyChanged(nameof(DutyCycle));
            }
        }

        public int LaserRise
        {
            get { return _rise; }
            set
            {
                _rise = value;
                OnPropertyChanged(nameof(LaserRise));
            }
        }

        public int LaserFall
        {
            get { return _fall; }
            set
            {
                _fall = value;
                OnPropertyChanged(nameof(LaserFall));
            }
        }

        public int LaserHoldtime
        {
            get { return _hold; }
            set
            {
                _hold = value;
                OnPropertyChanged(nameof(LaserHoldtime));
            }
        }

        public int WireTime
        {
            get { return _wire; }
            set
            {
                _wire = value;
                OnPropertyChanged(nameof(WireTime));
            }
        }

        public int AirIn
        {
            get { return _in; }
            set
            {
                _in = value;
                OnPropertyChanged(nameof(AirIn));
            }
        }

        public int AirOut
        {
            get { return _out; }
            set
            {
                _out = value;
                OnPropertyChanged(nameof(AirOut));
            }
        }

        public int WeldSpeed
        {
            get { return _weld; }
            set
            {
                _weld = value;
                OnPropertyChanged(nameof(WeldSpeed));
            }
        }

        public int WobbleSpeed
        {
            get { return _wobble; }
            set
            {
                _wobble = value;
                OnPropertyChanged(nameof(WobbleSpeed));
            }
        }

        public int LeapSpeed
        {
            get
            {
                if (_leapSpeed >= 0 && _leapSpeed <= 500)
                    return _leapSpeed;
                else
                    return 0;
            }
            set
            {
                if (value >= 0 && value <= 500)
                {
                    _leapSpeed = value;
                }
                else
                {
                    _leapSpeed = 0;
                }
                OnPropertyChanged(nameof(LeapSpeed));
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

        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value; OnPropertyChanged();
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value; OnPropertyChanged();
            }
        }

        public int Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value; OnPropertyChanged();
            }
        }

        public int PointInfo
        {
            get { return _pointInfo; }
            set { _pointInfo = value; OnPropertyChanged(); }
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

        public bool LineType
        {
            get { return _linetype; }
            set { _linetype = value; OnPropertyChanged(); }
        }

        public bool LaserOnOff
        {
            get { return _laseronoff; }
            set { _laseronoff = value; OnPropertyChanged(); }
        }

        public bool InputPowerOk
        {
            get { return _inputPowerOk; }
            set { _inputPowerOk = value; OnPropertyChanged(); }
        }

        public string PointList
        {
            get { return _pointList; }
            set { _pointList = value; OnPropertyChanged(); }
        }

        public bool PmFlag
        {
            get { return _pmFlag; }
            set { _pmFlag = value; OnPropertyChanged(); }
        }

        public string PmText
        {
            get { return _pmText; }
            set { _pmText = value; OnPropertyChanged(); }
        }

        public bool LeftResetFlag
        {
            get { return _leftResetFlag; }
            set { _leftResetFlag = value; OnPropertyChanged(); }
        }

        public bool RightResetFlag
        {
            get { return _rightResetFlag; }
            set { _rightResetFlag = value; OnPropertyChanged(); }
        }
    }
}
