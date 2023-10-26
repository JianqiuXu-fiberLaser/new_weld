///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Clearify view interface.
//
///////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RobotWeld3.ViewModel
{
    /// <summary>
    /// the parameter for display in the main windows.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        //
        // the plot and the point being chose
        //
        private double _x;
        private double _y;
        private double _z;
        private double _a;

        private static int _workCount = 0;
        private static string _errMsg = string.Empty;
        private static bool _pmFlag;

        //
        // Trace plot view
        //
        private object? _selectedTracePlot;
        
        public object? SelectedTracePlot
        {
            get { return _selectedTracePlot; }
            set 
            { 
                _selectedTracePlot = value; 
                OnPropertyChanged(nameof(SelectedTracePlot)); 
            }
        }

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

        public static event PropertyChangedEventHandler? StaticPropertyChanged;

        protected static void StaticOnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
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

        public static RoutedCommand SuperUserCommand = new("SuperUser", typeof(MainWindowViewModel));
        public static RoutedCommand LaserDecodeCommand = new("LaserDecode", typeof(MainWindowViewModel));
        public static RoutedCommand CalibrateMachineCommand = new("CalibrateMachie", typeof(MainWindowViewModel));
        public static RoutedCommand LaserVersionCommand = new("LaserVersion", typeof(MainWindowViewModel));

        public static RoutedCommand SetupCommand = new("Setup", typeof(MainWindowViewModel));
        public static RoutedCommand GetHelpCommand = new("GetHelp", typeof(MainWindowViewModel));
        public static RoutedCommand ShowAboutCommand = new("ShowAbout", typeof(MainWindowViewModel));
        public static RoutedCommand LaserTypeCommand = new("LaserType", typeof(MainWindowViewModel));

        //-- The dependency properties to display in the MainWindow --

        public double X
        {
            get { return _x; }
            set
            { _x = value; OnPropertyChanged(); }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; OnPropertyChanged();}
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; OnPropertyChanged(); }
        }

        public double A
        {
            get { return _a; }
            set { _a = value; OnPropertyChanged(); }
        }

        public static string ErrMsg
        {
            get { return _errMsg ?? "工作准备中"; }
            set
            {
                _errMsg = value;
                StaticOnPropertyChanged(nameof(ErrMsg));
            }
        }

        public static int WorkCount
        {
            get => _workCount;
            set { _workCount = value; StaticOnPropertyChanged(nameof(WorkCount));}
        }

        public bool PmFlag
        {
            get => _pmFlag;
            set { _pmFlag = value; OnPropertyChanged();}
        }
    }
}
