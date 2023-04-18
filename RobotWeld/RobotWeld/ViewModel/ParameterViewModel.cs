using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using RobotWeld.AlgorithmsBase;
using RobotWeld;
using System;

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
        public WeldFileAccess weldFileAccess;

        // the states of buttons in the bottm of canvas.
        //private int _keyStates_left;
        //private int _keyStates_middle;
        //private int _keyStates_right;

        // pass the file information to the class of WeldFileAccess,
        // which deal with the record file.
        public ParameterViewModel()
        {
            vector = new Vector(0, 0, 0);
            weldFileAccess = new WeldFileAccess();
        }

        // get the file name/Index from the dialog Box
        public void GetFileMaterial(int fileIndex)
        {
            weldFileAccess.FileMaterial = fileIndex;
        }

        // show the select material dialog
        public void SelectMaterialDialog()
        {
            SelectMaterial dialog = new();
            dialog.GetMaterialFile(weldFileAccess.FileMaterial);
            dialog.smSendfile = GetFileMaterial;
            dialog.ShowDialog();
        }

        // load the record file at the startup of Main Window
        public void WeldLoad()
        {
            weldFileAccess.WeldLoad();
        }

        // Open the customer saved file
        [STAThread]
        public void Open()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Weld Data files (*.wfd)|*.wfd",
                Title = "Open Weld Data file",
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };
            
            bool ? rst = openFileDialog.ShowDialog();

            if (rst == true)
            {
                string filename = openFileDialog.FileName;
                weldFileAccess.Open(filename);
            }
        }

        // save the file information when download,
        // and compare the file in machine with the save information
        // at startup of the Main Window.
        public void DownloadSave()
        {
            weldFileAccess.DownloadSave();
        }

        // The diaglog for the SaveAs Command
        public void SaveDialog()
        {
            SaveFileDialog dialog = new()
            {
                DefaultExt = ".wfd",
                Filter = "WeldData documents (.wfd)|*.wfd",
                AddExtension = true,
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool ? rst = dialog.ShowDialog();
            
            if (rst == true) 
            {
                string filename = dialog.FileName;
                weldFileAccess.SaveDialog(filename);
            }
        }

        // save the worktime each close the program
        public void Close()
        {
            weldFileAccess.Close();
        }

        // create new project
        public void New()
        {
            weldFileAccess.New();
        }

        //-- Define the RoutedCommand in MainWindows --
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

        //-- The properties to display in the MainWindow --
        public int LaserPower
        {
            get { return _laserPower; }
            set
            {
                _laserPower = value;
                OnPropertyChanged();
            }
        }

        public string MaterialType
        {
            get { return _materialType; }
            set
            {
                _materialType = value;
                OnPropertyChanged();
            }
        }

        public double MaterialThick
        {
            get { return _materialThick; }
            set
            {
                _materialThick = value;
                OnPropertyChanged();
            }
        }

        public double WireDiameter
        {
            get { return _wireDiameter; }
            set
            {
                _wireDiameter = value;
                OnPropertyChanged();
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

        public string Prompting
        {
            get { return _prompting; }
            set { _prompting = value; OnPropertyChanged(); }
        }

        public int KeyStates_left { get; set; }

        public int KeyStates_right { get; set; }

        //-- Response for the ICommand Events in the MainWindow --
        // required by INotifPropertyChanged interface
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
