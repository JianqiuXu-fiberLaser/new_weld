///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Clarified Interface, eliminate PLC UI
//           (2) connect point list with main model in main window
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using RobotWeld3.GetTrace;
using RobotWeld3.View;
using RobotWeld3.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel mainViewModel;
        private static MainModel? mainModel;
        private static PointListModel? _pointListModel;

        public MainWindow()
        {
            //
            // read default configure fiel and restore the parameters,
            // the data of curves and PLC configuration.
            //
            mainViewModel = new MainWindowViewModel();
            mainModel = new();
            _pointListModel = new PointListModel();
            mainModel.PutViewModel(mainViewModel);
            mainModel.Startup();
            mainModel.PutPointListModel(_pointListModel);

            InitializeComponent();
            DataContext = mainViewModel;

            ClickLightButton.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonDown), true);
            ClickLightButton.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.ClickLightButton_MouseLeftButtonUp), true);
        }

        /// <summary>
        /// Get reference of Main model.
        /// </summary>
        /// <returns></returns>
        internal static MainModel GetMainModel()
        {
            mainModel ??= new MainModel();
            return mainModel;
        }

        /// <summary>
        /// Get reference of PointListModel.
        /// </summary>
        /// <returns></returns>
        internal static PointListModel GetPointListModel()
        {
            _pointListModel ??= new PointListModel();
            return _pointListModel;
        }

        /// <summary>
        /// Enable all menu items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Executed program for each menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.New)
            {
                mainModel?.NewFile();
            }

            if (e.Command == ApplicationCommands.Close)
            {
                Close();
                mainModel?.CloseWindow();
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
                TabControl.SelectedIndex = 0;
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
            if (e.Command == MainWindowViewModel.IntersectCommand)
            {
                mainModel?.ConfigTrace(Tracetype.INTERSECT);
                TabControl.SelectedIndex = 1;
            }

            if (e.Command == MainWindowViewModel.StageTraceCommand)
            {
                mainModel?.ConfigTrace(Tracetype.STAGE_TRACE);
                TabControl.SelectedIndex = 1;
            }

            if (e.Command == MainWindowViewModel.SpiralCommand)
            {
                mainModel?.ConfigTrace(Tracetype.SPIRAL);
                TabControl.SelectedIndex = 1;
            }

            if (e.Command == MainWindowViewModel.InputTraceCommand)
            {
                mainModel?.ConfigTrace(Tracetype.FREE_TRACE);
                TabControl.SelectedIndex = 1;
            }

            if (e.Command == MainWindowViewModel.AutoTraceCommand)
            {
                // TODO: calculate the trace with optical vision
            }

            if (e.Command == MainWindowViewModel.LaserTypeCommand)
            {
                mainModel?.SetupLaserType();
            }

            if (e.Command == MainWindowViewModel.LaserDecodeCommand)
            {
                // decode the laser expire date
            }

            if (e.Command == MainWindowViewModel.SuperUserCommand)
            {
                // setup weld parameter beyond defalut setting.
                mainModel?.SuperUserSettings();
            }

            if (e.Command == MainWindowViewModel.CalibrateMachineCommand)
            {
                mainModel?.CalibrateMachine();
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

        /// <summary>
        /// Action:
        /// 1) Close the motion card and exit the thread
        /// 2) Close the daemon file.
        /// 3) disconect the PLC and exit the thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClose_Click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            mainModel?.CloseWindow();
        }

        /// <summary>
        /// Reset four axes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WholeResetButton_Click_1(object sender, RoutedEventArgs e)
        {
            mainModel?.ResetCard();
        }

        /// <summary>
        /// Light button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickLightButton_Click(object sender, RoutedEventArgs e)
        {
            // Do nothing, just trigger the mouse event
        }

        /// <summary>
        /// Methods when chosing various tabItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControl.SelectedIndex == 0)
                mainModel?.SelectItemVideo();
            else if (TabControl.SelectedIndex == 1)
                mainModel?.SelectItemPoint();
        }

        /// <summary>
        /// Jog light on/off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickLightButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainModel?.JogLightOn();
        }

        /// <summary>
        /// Jog light on/off 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickLightButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mainModel?.JogLightOff();
        }
    }
}

