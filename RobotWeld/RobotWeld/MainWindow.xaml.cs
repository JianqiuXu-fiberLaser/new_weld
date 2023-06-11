using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using RobotWeld.AlgorithmsBase;
using RobotWeld.ViewModel;
using RobotWeld.Welding;
using RobotWeld.GetTrace;

namespace RobotWeld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// (1) Commands in the Menu 
    /// (2) Animation in the left screen and video displayed in rigth screen
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ParameterViewModel? mainViewModel;
        private static DaemonFile? daemonFile;
        private static ExtraController? extraController;

        private static readonly MotionType motionType = new();
        private static readonly TraceType traceType = new();
        private static readonly SingleMotion singleMotion = new();

        private static readonly List<GetTrace.Point> points = new();
        private static int pointIndex = 0;
        private static int keyState = 0;

        // private static Thread? thread;

        public MainWindow()
        {
            mainViewModel = new ParameterViewModel();

            //
            // read default configure fiel and restore the parameters,
            // the data of curves and PLC configuration.
            //
            daemonFile = new DaemonFile();
            daemonFile.SetParameter(mainViewModel);
            // daemonFile.GetPoints(points);

            //
            // connect to the PLC and pass the viewModel to the PLC,
            // so that mainViewModel can refrash the view from the
            // status of PLC.
            //
            extraController = new ExtraController();
            extraController.SetParameter(mainViewModel);

            InitializeComponent();
            this.DataContext = mainViewModel;

            //
            // connect to the motion card and run the background
            // thread
            //
/*            thread = new(MotionCard)
            {
                Name = nameof(MotionCard)
            };
            thread.Start();
            thread.IsBackground = true;*/
        }

        //
        // -- Communicate with and manuplate the motion card.
        // pointIndex : the index of point to be treated
        // KeyState : the button state for the current point
        //
        public static void MotionCard()
        {
            RunTrace runTrace = new();

            while (true)
            {
                // if there is an error in motion card.
                if (runTrace.CardSignal == -1) 
                {
                    runTrace.CloseCard();
                    return; 
                }

                if (runTrace.TraceChanged(points, traceType, motionType,
                    singleMotion, ref pointIndex, ref keyState))
                {
                    ViewPlot();
                }

                Thread.Sleep(5);    // sleep 5 ms between each task cycle.
            }
        }

        #region view plots
        //
        //-- plot the trace when it is changed.
        // Depends on the user choice, display difference typies of plot
        //
        private static void ViewPlot()
        {
            if (motionType.motiontype == MotionType.EmotionType.RUN_TRACE)
            {
                switch (traceType.tracetype)
                {
                    case TraceType.EtraceType.INPUT:
                        break;
                    case TraceType.EtraceType.VANE_WHEEL:
                        break;
                    default: break;
                }
            }
            else if (motionType.motiontype == MotionType.EmotionType.TAKE_TRACE) 
            {
                switch (traceType.tracetype)
                {
                    case TraceType.EtraceType.INPUT:
                        break;
                    case TraceType.EtraceType.VANE_WHEEL:
                        break;
                    default: break;
                }
            }
            else
            { 
                // nothing here
            }
        }

        #endregion

        #region windows operation
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
                daemonFile?.NewFile();
                points.Clear();

                if (mainViewModel is not null)
                    daemonFile?.SetParameter(mainViewModel);
            }

            if (e.Command == ApplicationCommands.Close)
            {
                daemonFile?.Close();
                daemonFile = null;
            }

            if (e.Command == ApplicationCommands.SaveAs)
            {
                daemonFile?.SaveDialog();
            }

            if (e.Command == ApplicationCommands.Open)
            {
                daemonFile?.OpenDialog();
            }

            if (e.Command == ParameterViewModel.RunCommand)
            {
                daemonFile?.SaveTrace(points);
                motionType.motiontype = MotionType.EmotionType.RUN_TRACE;
            }

            if (e.Command == ParameterViewModel.ImportCommand)
            {
                // 导入DXF文件
            }

            //
            // Open a selection UI window which set the PLC parameter.
            // Restore and save the parameter when open and close the
            // window.
            //
            if (e.Command == ParameterViewModel.VaneWheelCommand)
            {
                if (daemonFile is not null) 
                {
                    VaneWheel vaneWheel = new(daemonFile);
                    vaneWheel.ShowDialog();
                }

                motionType.motiontype = MotionType.EmotionType.TAKE_TRACE;
                traceType.tracetype = TraceType.EtraceType.VANE_WHEEL;
            }

            if (e.Command == ParameterViewModel.IntersectCommand)
            {
                //motionId = MotionType.TAKE_TRACE;
            }

            if (e.Command == ParameterViewModel.TopTraceCommand)
            {
                //motionId = MotionType.TAKE_TRACE;
            }

            if (e.Command == ParameterViewModel.StageTraceCommand)
            {
                //motionId = MotionType.TAKE_TRACE;
            }

            if (e.Command == ParameterViewModel.SpiralCommand)
            {
                //motionId = MotionType.TAKE_TRACE;
            }

            if (e.Command == ParameterViewModel.InputTraceCommand) 
            {
                //traceType = 1;
                //motionId = MotionType.TAKE_TRACE;
            }

            if (e.Command == ParameterViewModel.AutoTraceCommand)
            {
                // TODO: calculate the trace with optical vision
            }

            if (e.Command == ParameterViewModel.PresetParameterCommand)
            {
                if (daemonFile is not null)
                {
                    SelectMaterial selectMaterial = new(daemonFile);
                    selectMaterial.ShowDialog();
                }
            }

            if (e.Command == ParameterViewModel.UserParameterCommand)
            {
                if (daemonFile is not null)
                {
                    InputLaserParameter inputLaserParameter = new(daemonFile);
                    inputLaserParameter.ShowDialog();
                }
            }

            if (e.Command == ParameterViewModel.AutoParameterCommand)
            {
                // TODO:give the weld parameter from calculation
            }

            if (e.Command == ParameterViewModel.LaserDecodeCommand)
            {
                // decode the laser expire date
                //viewModel?.LaserDecode();
            }

            if (e.Command == ParameterViewModel.LaserVersionCommand)
            {
                // show the laser factory information
                //viewModel?.ShowFactory();
            }

            if (e.Command == ParameterViewModel.GetHelpCommand)
            {
            }

            if (e.Command == ParameterViewModel.ShowAboutCommand)
            {
                var dialog = new About();
                dialog.ShowDialog();
            }

            e.Handled = true;
        }

        //-- the response for buttons --
        private void LaserButton_Click(object sender, RoutedEventArgs e)
        {
            //runTrace.LaserButton();
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            //runTrace.LineButton();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            //runTrace.OkButton();
        }

        //
        // Action:
        // 1) Close the motion card and exit the thread
        // 2) Close the daemon file.
        // 3) disconect the PLC and exit the thread
        //
        private void WindowClose_Click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            motionType.motiontype = MotionType.EmotionType.EXIT_MOTION;

            daemonFile?.Close();
            daemonFile = null;
            extraController?.Close();

            e.Cancel = false;
        }
        #endregion

        #region PLC communication
        //
        // -- actions for three mechanism in two work position
        //
        private void LeftClip_Checked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.LEFT_TOPPING);
        }

        private void LeftClip_Unchecked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.LEFT_TOPPING);
        }

        private void LeftLocate_Checked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.LEFT_LOCATE);
        }

        private void LeftLocate_Unchecked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.LEFT_LOCATE);
        }

        private void LeftTop_Checked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.LEFT_LOAD);
        }

        private void LeftTop_Unchecked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.LEFT_LOAD);
        }

        private void RightClip_Checked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.RIGHT_TOPPING);
        }

        private void RightClip_Unchecked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.RIGHT_TOPPING);
        }

        private void RightLocate_Checked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.RIGHT_LOCATE);
        }

        private void RightLocate_Unchecked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.RIGHT_LOCATE);
        }

        private void RightTop_Checked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.RIGHT_LOCATE);
        }

        private void RightTop_Unchecked(object sender, RoutedEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.RIGHT_LOAD);
        }

        //
        // -- four accelerate button, each with two directions,
        // and each direction responds for two actions.
        // Total 16 functions.
        //
        private void C2Right_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.C2_POS);
        }

        private void C2Right_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.C2_POS);
        }

        private void C2Left_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.C2_NEG);
        }

        private void C2Left_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.C2_NEG);
        }

        private void Y2Right_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.Y2_POS);
        }

        private void Y2Right_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.Y2_POS);
        }

        private void Y2Left_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.C2_NEG);
        }

        private void Y2Left_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.C2_NEG);
        }

        private void C1Right_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.C1_POS);
        }

        private void C1Right_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.C1_POS);
        }

        private void C1Left_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.C1_NEG);
        }

        private void C1Left_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.C1_NEG);
        }

        private void Y1Right_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.Y1_POS);
        }

        private void Y1Right_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.Y1_POS);
        }

        private void Y1Left_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOn(ActionIndex.Y1_NEG);
        }

        private void Y1Left_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            extraController?.TurnOff(ActionIndex.Y1_NEG);
        }

        private void C1Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int var;
            if (mainViewModel is not null)
            {
                var = mainViewModel.C1Speed;
            }
            else
                var = 0;

            extraController?.SendPlcData(SpeedAddress.C1_SPD, var);
        }

        private void Y1Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int var;
            if (mainViewModel is not null)
            {
                var = mainViewModel.Y1Speed;
            }
            else
                var = 0;

            extraController?.SendPlcData(SpeedAddress.Y1_SPD, var);
        }

        private void Y2Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int var;
            if (mainViewModel is not null)
            {
                var = mainViewModel.Y2Speed;
            }
            else
                var = 0;

            extraController?.SendPlcData(SpeedAddress.Y2_SPD, var);
        }

        private void C2Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int var;
            if (mainViewModel is not null)
            {
                var = mainViewModel.C1Speed;
            }
            else
                var = 0;

            extraController?.SendPlcData(SpeedAddress.C2_SPD, var);
        }
        #endregion
    }
}
