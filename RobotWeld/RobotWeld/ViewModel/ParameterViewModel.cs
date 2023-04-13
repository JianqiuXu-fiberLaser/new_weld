using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RobotWeld.AlgorithmsBase;

namespace RobotWeld.ViewModel
{
    /// <summary>
    /// the parameter for display in the main windows.
    /// </summary>
    public class ParameterViewModel : INotifyPropertyChanged
    {
        private int _laserPower = 0;
        private string _materialType = "碳钢";
        private double _materialThick = 0;
        private double _wireDiameter = 0;
        private string _prompting = "提示";

        public Vector vector;

        // the states of buttons in the bottm of canvas.
        private int _keyStates_left;
        private int _keyStates_middle;
        private int _keyStates_right;

        public ParameterViewModel()
        {
            vector = new Vector(0, 0, 0);
        }

        public void Load()
        {
            // 程序启动时，装载。
        }

        // Define the RoutedCommand in MainWindows
        public static RoutedCommand DownloadCommand = 
            new RoutedCommand("Download", typeof(ParameterViewModel));
        public static RoutedCommand ImportCommand =
            new RoutedCommand("Import", typeof(ParameterViewModel));
        public static RoutedCommand PresetTraceCommand = 
            new RoutedCommand("PresetTrace", typeof(ParameterViewModel));
        public static RoutedCommand AutoTraceCommand = 
            new RoutedCommand("AutoTrace", typeof(ParameterViewModel));
        public static RoutedCommand PresetParameterCommand = 
            new RoutedCommand("PresetParameter", typeof(ParameterViewModel));
        public static RoutedCommand UserParameterCommand = 
            new RoutedCommand("UserParameter", typeof(ParameterViewModel));
        public static RoutedCommand AutoParameterCommand = 
            new RoutedCommand("AutoParameter", typeof(ParameterViewModel));
        public static RoutedCommand LaserDecodeCommand = 
            new RoutedCommand("LaserDecode", typeof(ParameterViewModel));
        public static RoutedCommand LaserVersionCommand = 
            new RoutedCommand("LaserVersion", typeof(ParameterViewModel));
        public static RoutedCommand GetHelpCommand = 
            new RoutedCommand("GetHelp", typeof(ParameterViewModel));
        public static RoutedCommand ShowAboutCommand = 
            new RoutedCommand("ShowAbout", typeof(ParameterViewModel));

        //-- The properties to display in the MainWindow
        public int LaserPower
        {
            set
            {
                _laserPower = value;
                OnPropertyChanged();
            }
        }

        public string MaterialType
        {
            set
            {
                _materialType = value;
                OnPropertyChanged();
            }
        }

        public double MaterialThick
        {
            set
            {
                _materialThick = value;
                OnPropertyChanged();
            }
        }

        public double WireDiameter
        {
            set
            {
                _wireDiameter = value;
                OnPropertyChanged();
            }
        }

        public double X
        {
            set { vector.X = value; OnPropertyChanged(); }
        }

        public double Y
        {
            set { vector.Y = value; OnPropertyChanged(); }
        }

        public double Z
        {
            set { vector.Z = value; OnPropertyChanged(); }
        }

        public string Prompting
        {
            set { _prompting = value; OnPropertyChanged(); }
        }

        public int KeyStates_left { get; set; }

        public int KeyStates_right { get; set; }

        //-- Response for the ICommand Events in the MainWindow
        // required by INotifPropertyChanged interface
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
