using RobotWeld2;
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
        private static MainWindowViewModel? mainViewModel;
        private static DaemonFile? daemonFile;
        private static DaemonModel? dmModel;
        private string keepString = string.Empty;

        public MainWindow()
        {
            //
            // read default configure fiel and restore the parameters,
            // the data of curves and PLC configuration.
            //
            mainViewModel = new MainWindowViewModel();
            daemonFile = new DaemonFile(mainViewModel);
            daemonFile.SetParameter();
            InitializeComponent();
            this.DataContext = mainViewModel;

            dmModel = new DaemonModel(daemonFile, mainViewModel);
            dmModel.SetupParameter();
            // dmModel.Initial();
            daemonFile.AddMsg("初始化完成");

            ClickLightButton.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonDown), true);
            ClickLightButton.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonUp), true);
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
                    if (mainViewModel != null && daemonFile != null && dmModel != null)
                    {
                        daemonFile.NewFile();
                        dmModel.NewFile();
                        daemonFile.SetParameter();
                    }
                }
            }

            if (e.Command == ApplicationCommands.Close)
            {
                MotionOperate.StopAllThread();
                daemonFile?.Close();
                Close();
            }

            if (e.Command == ApplicationCommands.SaveAs)
            {
                dmModel?.SaveTrace();
                daemonFile?.SaveDialog();
            }

            //
            // Open the saved file, which the 20201218.wdf is the factory file
            //
            if (e.Command == ApplicationCommands.Open)
            {
                if (daemonFile != null && dmModel != null)
                {
                    daemonFile.OpenDialog();
                    daemonFile.SetParameter();
                    dmModel.OpenFile();
                }
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
                if (daemonFile != null && dmModel != null)
                {
                    if (daemonFile.TraceType != Tracetype.VANE_WHEEL)
                    {
                        daemonFile.AddMsg("轨迹错误");
                    }
                    else
                    {
                        //dmModel.RunTrace(Tracetype.VANE_WHEEL);
                        dmModel.WeldTrace(Tracetype.VANE_WHEEL);
                    }
                }
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
                if (daemonFile != null && dmModel != null)
                {
                    VaneWheel vaneWheel = new(daemonFile);
                    vaneWheel.ShowDialog();
                    daemonFile.SetInputPower(false);
                    dmModel.TakePoints();
                }
            }

            if (e.Command == MainWindowViewModel.IntersectCommand)
            {
            }

            if (e.Command == MainWindowViewModel.TopTraceCommand)
            {
            }

            if (e.Command == MainWindowViewModel.StageTraceCommand)
            {
            }

            if (e.Command == MainWindowViewModel.SpiralCommand)
            {
            }

            if (e.Command == MainWindowViewModel.InputTraceCommand)
            {
            }

            if (e.Command == MainWindowViewModel.AutoTraceCommand)
            {
                // TODO: calculate the trace with optical vision
            }

            if (e.Command == MainWindowViewModel.PresetParameterCommand)
            {
                if (daemonFile is not null)
                {
                    SelectMaterial selectMaterial = new(daemonFile);
                    selectMaterial.ShowDialog();
                }
            }

            if (e.Command == MainWindowViewModel.UserParameterCommand)
            {
                if (daemonFile is not null)
                {
                    InputLaserParameter inputLaserParameter = new(daemonFile);
                    inputLaserParameter.ShowDialog();
                }

                if ((daemonFile is not null) && (mainViewModel is not null))
                    daemonFile.SetParameter();
            }

            if (e.Command == MainWindowViewModel.AutoParameterCommand)
            {
                // TODO:give the weld parameter from calculation
            }

            if (e.Command == MainWindowViewModel.LaserTypeCommand)
            {
                if (daemonFile != null)
                {
                    var dialog = new LaserType(daemonFile);
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
            daemonFile?.Close();
            e.Cancel = false;
        }


        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            dmModel?.SelectionOk();
        }

        private void WholeResetButton_Click_1(object sender, RoutedEventArgs e)
        {
            dmModel?.ResetCard();
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
            dmModel?.JogLightOn();
        }

        private void ClickLightButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dmModel?.JogLightOff();
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

        private void PointInfo_GotFocus(object sender, RoutedEventArgs e)
        {
            PointInfo.SelectAll();
        }

        private void PointInfo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void PointInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    dmModel?.GotoSingleStep();
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            dmModel?.CancelChoose();
        }
    }
}
