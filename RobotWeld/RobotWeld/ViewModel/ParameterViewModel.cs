using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.Generic;

using RobotWeld.AlgorithmsBase;
using RobotWeld.Welding;
using RobotWeld.GetTrace;

namespace RobotWeld.ViewModel
{
    /// <summary>
    /// the parameter for display in the main windows.
    /// </summary>
    public class ParameterViewModel : INotifyPropertyChanged
    {
        #region Laser parameters
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

        private string? _materialType;
        private int _materialIndex;
        private double _sheetThickness;
        private double _wireDiameter;
        #endregion

        //
        // the plot and the point being chose
        //
        public Vector vector;
        private string? _pointerInfo;

        //
        // PLC parameters
        //
        private bool _isAlarm1;
        private bool _isAlarm2;

        private List<string> msglist = new();
        private string? _errorMsg;
        private int _y1Speed;
        private int _y2Speed;
        private int _c1Speed;
        private int _c2Speed;

        //
        // the states of buttons in the bottm of canvas.
        //
        //private int _keyStates_left;
        //private int _keyStates_right;

        //
        // pass the file information to the class of WeldFileAccess,
        // which deal with the record file.
        //
        public ParameterViewModel()
        {
            vector = new Vector();
            IsAlarmY1 = true;
            IsAlarmY2 = false;
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

        #region RoutedCommand
        //
        //-- Define the RoutedCommand in MainWindows --
        //
        public static RoutedCommand RunCommand = new("Run", typeof(ParameterViewModel));
        public static RoutedCommand ImportCommand = new("Import", typeof(ParameterViewModel));

        public static RoutedCommand InputTraceCommand = new("InputTrace", typeof(ParameterViewModel));
        public static RoutedCommand VaneWheelCommand = new("VaneWheel", typeof(ParameterViewModel));
        public static RoutedCommand IntersectCommand = new("Intersect", typeof(ParameterViewModel));
        public static RoutedCommand TopTraceCommand = new("TopTrace", typeof(ParameterViewModel));
        public static RoutedCommand StageTraceCommand = new("StageTrace", typeof(ParameterViewModel));
        public static RoutedCommand SpiralCommand = new("Spiral", typeof(ParameterViewModel));
        public static RoutedCommand AutoTraceCommand = new("AutoTrace", typeof(ParameterViewModel));

        public static RoutedCommand PresetParameterCommand = new("PresetParameter", typeof(ParameterViewModel));
        public static RoutedCommand UserParameterCommand = new("UserParameter", typeof(ParameterViewModel));
        public static RoutedCommand AutoParameterCommand = new("AutoParameter", typeof(ParameterViewModel));
        public static RoutedCommand LaserDecodeCommand = new("LaserDecode", typeof(ParameterViewModel));
        public static RoutedCommand LaserVersionCommand = new("LaserVersion", typeof(ParameterViewModel));

        public static RoutedCommand GetHelpCommand = new("GetHelp", typeof(ParameterViewModel));
        public static RoutedCommand ShowAboutCommand = new("ShowAbout", typeof(ParameterViewModel));
        #endregion

        #region properties
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

        public double X
        {
            get => vector.X;
            set { vector.X = value; OnPropertyChanged(); }
        }

        public double Y
        {
            get => vector.Y;
            set { vector.Y = value; OnPropertyChanged(); }
        }

        public double Z
        {
            get => vector.Z;
            set { vector.Z = value; OnPropertyChanged(); }
        }

        public string PointerInfo
        {
            get { return _pointerInfo ?? "选点提示"; }
            set { _pointerInfo = value; OnPropertyChanged(); }
        }

        public int KeyStates_left { get; set; }

        public int KeyStates_right { get; set; }

        //
        // PLC properties
        //
        public bool IsAlarmY1
        {
            get { return _isAlarm1; }
            set { _isAlarm1 = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY2
        {
            get { return _isAlarm2; }
            set { _isAlarm2 = value; OnPropertyChanged(); }
        }

        public string ErrorMsg
        {
            get 
            { 
                if (_errorMsg == null)
                {
                    _errorMsg = "工作正常";
                }
                return _errorMsg; 
            }
            set 
            {
                _errorMsg = value; 
                OnPropertyChanged(); 
            }
        }

        public int Y1Speed
        {
            get { return _y1Speed; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    _y1Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        public int C1Speed
        {
            get { return _c1Speed; }
            set 
            {
                if (value >= 0 && value <= 100)
                {
                    _c1Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        public int C2Speed
        {
            get { return _c2Speed; }
            set 
            {
                if (value >= 0 && value <= 100)
                {
                    _c2Speed = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Y2Speed
        {
            get { return _y2Speed; }
            set 
            {
                if (value >= 0 && value <= 100)
                {
                    _y2Speed = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region plot the curves
        // plot the trace plots in the left region of screen.
        public void ModifyTrace(List<Point> points, TraceType traceType)
        {
            switch (traceType.tracetype)
            {
                case TraceType.EtraceType.INPUT:
                    PlotInput(points); 
                    break;
                case TraceType.EtraceType.VANE_WHEEL:
                    PlotVaneWheel(points); 
                    break;
                default:
                    break;
            }
        }

        private void PlotInput(List<Point> points) { }
        private void PlotVaneWheel(List<Point> points) { }

        #endregion
    }
}
