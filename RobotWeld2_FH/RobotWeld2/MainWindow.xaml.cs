using RobotWeld2.AppModel;
using RobotWeld2.GetTrace;
using RobotWeld2.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel mainViewModel;
        private static MainModel? mainModel;

        public MainWindow()
        {
            //
            // read default configure fiel and restore the parameters,
            // the data of curves and PLC configuration.
            //
            mainViewModel = new MainWindowViewModel();
            mainModel = new(mainViewModel);
            MainModel.ConfigMachine();
            mainModel.ReadRecord();

            InitializeComponent();
            this.DataContext = mainViewModel;

            ClickLightButton.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonDown), true);
            ClickLightButton.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonUp), true);
        }

        public static MainModel? GetMainModel()
        {
            return mainModel;
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
                mainModel?.NewFile();
            }

            if (e.Command == ApplicationCommands.Close)
            {
                Close();
            }

            if (e.Command == ApplicationCommands.SaveAs)
            {
                mainModel?.SaveDialog();
            }

            //
            // Open the saved file, which the 20201218.wdf is the factory file
            //
            if (e.Command == ApplicationCommands.Open)
            {
                mainModel?.OpenDialog();
            }

            if (e.Command == MainWindowViewModel.RunCommand)
            {
                mainModel?.WeldTrace();
            }

            //
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
                mainModel?.ConfigTrace(Tracetype.VANE_WHEEL);
            }

            if (e.Command == MainWindowViewModel.IntersectCommand)
            {
                mainModel?.ConfigTrace(Tracetype.INTERSECT);
            }

            if (e.Command == MainWindowViewModel.CalibrateMachineCommand)
            {
                mainModel?.CalibrateMachine();
            }

            if (e.Command == MainWindowViewModel.StageTraceCommand)
            {
                mainModel?.ConfigTrace(Tracetype.STAGE_TRACE);
            }

            if (e.Command == MainWindowViewModel.SpiralCommand)
            {
                mainModel?.ConfigTrace(Tracetype.SPIRAL);
            }

            if (e.Command == MainWindowViewModel.InputTraceCommand)
            {
                mainModel?.ConfigTrace(Tracetype.FREE_TRACE);
            }

            if (e.Command == MainWindowViewModel.AutoTraceCommand)
            {
                // TODO: calculate the trace with optical vision
            }

            if (e.Command == MainWindowViewModel.PresetParameterCommand)
            {
                mainModel?.SetupMaterial();
            }

            if (e.Command == MainWindowViewModel.UserParameterCommand)
            {
                mainModel?.Userparameter();
            }

            if (e.Command == MainWindowViewModel.AutoParameterCommand)
            {
                // TODO:give the weld parameter from calculation
            }

            if (e.Command == MainWindowViewModel.LaserTypeCommand)
            {
                mainModel?.SetupLaserType();
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
            mainModel?.Close();
            e.Cancel = false;
        }

        private void WholeResetButton_Click_1(object sender, RoutedEventArgs e)
        {
            mainModel?.ResetCard();
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
            mainModel?.JogLightOn();
        }

        private void ClickLightButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mainModel?.JogLightOff();
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
                    mainModel?.GotoSingleStep();
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
