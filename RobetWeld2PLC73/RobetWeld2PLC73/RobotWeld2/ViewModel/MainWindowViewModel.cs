///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AlgorithmsBase;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RobotWeld2.ViewModel
{
    /// <summary>
    /// the parameter for display in the main windows.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _laserPower;

        private bool _laseronoff;

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
        private int _traceInfo;
        private bool _traceDir;
        private int _traceCount;

        private string _errMsg = string.Empty;
        private ObservableCollection<Point>? _pointList;
        private int _ptIndex;

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
            get => _laserPower;
            set
            {
                _laserPower = value;
                OnPropertyChanged(nameof(LaserPower));
            }
        }

        public int X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        public int Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }

        public int Z
        {
            get => _z;
            set { _z = value; OnPropertyChanged(); }
        }

        public int TraceInfo
        {
            get => _traceInfo;
            set { _traceInfo = value; OnPropertyChanged();}
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

        public bool LaserOnOff
        {
            get => _laseronoff;
            set { _laseronoff = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Point> PointList
        {
            get => _pointList ??= new ObservableCollection<Point>();
            set { _pointList = value; OnPropertyChanged(); }
        }

        public bool PmFlag
        {
            get => _pmFlag;
            set { _pmFlag = value; OnPropertyChanged(); }
        }

        public string PmText
        {
            get => _pmText;
            set { _pmText = value; OnPropertyChanged(); }
        }

        public bool LeftResetFlag
        {
            get => _leftResetFlag;
            set { _leftResetFlag = value; OnPropertyChanged(); }
        }

        public bool RightResetFlag
        {
            get => _rightResetFlag;
            set { _rightResetFlag = value; OnPropertyChanged(); }
        }

        public bool TraceDir
        {
            get => _traceDir; 
            set { _traceDir = value; OnPropertyChanged(); }
        }

        public int PtIndex
        {
            get => _ptIndex;
            set { _ptIndex = value; OnPropertyChanged(); }
        }

        public int TraceCount
        {
            get => _traceCount;
            set { _traceCount = value; OnPropertyChanged(); }
        }
    }
}
