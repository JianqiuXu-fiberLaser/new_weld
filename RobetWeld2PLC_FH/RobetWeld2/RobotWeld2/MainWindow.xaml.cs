///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using RobotWeld2.GetTrace;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;

namespace RobotWeld2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindowViewModel? _ViewModel;
        private static DaemonModel? _dmModel;

        public MainWindow()
        {
            //
            // read default configure fiel and restore the parameters,
            // the data of curves and PLC configuration.
            //
            _ViewModel = new MainWindowViewModel();
            _dmModel = new DaemonModel(_ViewModel);
            _dmModel.SetupParameter();

            // setup the view of main window
            _dmModel.SetParameter();
            InitializeComponent();
            this.DataContext = _ViewModel;

            _dmModel.AddMsg("初始化完成");

            ClickLightButton.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonDown), true);
            ClickLightButton.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonUp), true);
        }

        public static DaemonModel? GetMainModel()
        {
            return _dmModel;
        }

        //
        // Enable all menu items.
        //
        private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        //
        // Executed program for each menu item.
        //
        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.New)
            {
                if (e.Command == ApplicationCommands.New)
                {
                    _dmModel?.NewFile();
                }
            }

            if (e.Command == ApplicationCommands.Close)
            {
                MotionOperate.StopAllThread();
                ChkPre.StopChkThread();
                _dmModel?.Close();
                Close();
            }

            if (e.Command == ApplicationCommands.SaveAs)
            {
                _dmModel?.SaveTrace();
                _dmModel?.SaveDialog();
            }

            //
            // Open the saved file, which the 20201218.wdf is the factory file
            //
            if (e.Command == ApplicationCommands.Open)
            {
                _dmModel?.OpenFile();
            }

            if (e.Command == MainWindowViewModel.RunCommand)
            {
                // Only for special project
            }

            //
            // Run Vane Wheel welding process
            //
            if (e.Command == MainWindowViewModel.RunVaneWheelCommand)
            {
                _dmModel?.WeldTrace(Tracetype.VANE_WHEEL);
            }

            if (e.Command == MainWindowViewModel.ImportCommand)
            {
                // 导入DXF文件
            }

            //
            // Open a selection UI window which set the PLC parameter.
            // Restore and save the parameter when open and close the
            // window.
            //
            if (e.Command == MainWindowViewModel.VaneWheelCommand)
            {
                _dmModel?.TakePoints();
            }

            if (e.Command == MainWindowViewModel.IntersectCommand)
            {
                // ToDo: for other version
            }

            if (e.Command == MainWindowViewModel.TopTraceCommand)
            {
                // do nothing
            }

            if (e.Command == MainWindowViewModel.StageTraceCommand)
            {
                // do nothing
            }

            if (e.Command == MainWindowViewModel.SpiralCommand)
            {
                // do nothing
            }

            if (e.Command == MainWindowViewModel.InputTraceCommand)
            {
                // do nothing
            }

            if (e.Command == MainWindowViewModel.AutoTraceCommand)
            {
                // TODO: calculate the trace with optical vision
            }

            if (e.Command == MainWindowViewModel.PresetParameterCommand)
            {
                if (_dmModel != null)
                {
                    var selectMaterial = new SelectMaterial(_dmModel);
                    selectMaterial.ShowDialog();
                }
            }

            if (e.Command == MainWindowViewModel.UserParameterCommand)
            {
                if (_dmModel is not null)
                {
                    var inputLaserParameter = new InputLaserParameter(_dmModel);
                    inputLaserParameter.ShowDialog();
                }

                _dmModel?.SetParameter();
                MotionOperate.StopAllThread();
                _dmModel?.PreparedState(false);
            }

            if (e.Command == MainWindowViewModel.AutoParameterCommand)
            {
                // TODO:give the weld parameter from calculation
            }

            if (e.Command == MainWindowViewModel.LaserTypeCommand)
            {
                if (_dmModel != null)
                {
                    var dialog = new LaserType();
                    dialog.ShowDialog();
                }
            }

            if (e.Command == MainWindowViewModel.LaserDecodeCommand)
            {
                // decode the laser expire date
            }

            if (e.Command == MainWindowViewModel.LaserVersionCommand)
            {
                // show the laser factory information
            }

            //
            // Setup the password
            //
            if (e.Command == MainWindowViewModel.SetupCommand)
            {
                var dialog = new Setup();
                dialog.ShowDialog();
            }

            if (e.Command == MainWindowViewModel.GetHelpCommand)
            {
                // Todo: Web based help.
            }

            if (e.Command == MainWindowViewModel.ShowAboutCommand)
            {
                var dialog = new About();
                dialog.ShowDialog();
            }
            e.Handled = true;
        }

        //---- the response for buttons ----
        //
        // Action:
        // 1) Close the motion card and exit the thread
        // 2) Close the daemon file.
        // 3) disconect the PLC and exit the thread
        //
        private void WindowClose_Click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MotionOperate.StopAllThread();
            ChkPre.StopChkThread();
            _dmModel?.Close();
            e.Cancel = false;
        }

        private void WholeResetButton_Click_1(object sender, RoutedEventArgs e)
        {
            _dmModel?.ResetCard();
        }

        private void ClickLightButton_Click(object sender, RoutedEventArgs e)
        {
            // Do nothing, just trigger the mouse event
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? tabItemString = string.Empty;
            if (sender != null)
            {
                if (sender is TabControl tabCtrl)
                {
                    if (tabCtrl.SelectedItem is TabItem tabItem)
                        tabItemString = tabItem.Header.ToString();
                }
            }

            switch (tabItemString)
            {
                case "AirValve":
                    break;
                case "IOinfo":
                    break;
                case "ManualOperation":
                    break;
                default:
                    return;
            }
        }

        private void ClickLightButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dmModel?.JogLightOn();
        }

        private void ClickLightButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dmModel?.JogLightOff();
        }

        //
        // If the input character in TextBox is number, return true.
        //
        private static bool IsIntNumber(Key key)
        {
            if (key == Key.Enter || key == Key.Back ||
                (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9))
            {
                return true;
            }
            else { return false; }
        }

        private void TraceInfo_GotFocus(object sender, RoutedEventArgs e)
        {
            PointInfo.SelectAll();
        }

        private void TraceInfo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) return;

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void TraceInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter) _dmModel?.ShowTraceInfo();
                e.Handled = false;
            }
            else e.Handled = true;
        }

        private void PointInfo_GotFocus(object sender, RoutedEventArgs e)
        {
            PointInfo.SelectAll();
        }

        private void PointInfo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) return;

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void PointInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter) _dmModel?.GotoSingleStep();
                e.Handled = false;
            }
            else e.Handled = true;
        }

        private void TraceCount_GotFocus(object sender, RoutedEventArgs e)
        {
            TraceCount.SelectAll();
        }

        private void TraceCount_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) return;

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void TraceCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter) _dmModel?.SetTraceCount();
                e.Handled = false;
            }
            else e.Handled = true;
        }
    }
}
