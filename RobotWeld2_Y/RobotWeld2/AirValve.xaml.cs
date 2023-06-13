using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// AirValve.xaml 的交互逻辑
    /// </summary>
    public partial class AirValve : UserControl
    {
        private AirValveViewModel viewModel;
        private PlcAirValve plcAirValve;

        public AirValve()
        {
            viewModel = new AirValveViewModel();
            plcAirValve = new(viewModel);
            
            InitializeComponent();

            GridC1.DataContext = viewModel;
            GridC2.DataContext = viewModel;
            GridY1.DataContext = viewModel;
            GridY2.DataContext = viewModel;

            ButtonAddHandle();
        }

        private void ButtonAddHandle()
        {
            //-- Y1 --
            Y1Dwon.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Y1Dwon_MouseLeftButtonDown), true);
            Y1Dwon.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Y1Dwon_MouseLeftButtonUp), true);

            Y1Up.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Y1Up_MouseLeftButtonDown), true);
            Y1Up.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Y1Up_MouseLeftButtonUp), true);

            //-- Y2 --
            Y2Dwon.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Y2Dwon_MouseLeftButtonDown), true);
            Y2Dwon.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Y2Dwon_MouseLeftButtonUp), true);

            Y2Up.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Y2Up_MouseLeftButtonDown), true);
            Y2Up.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Y2Up_MouseLeftButtonUp), true);

            //-- C1 --
            C1Dwon.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.C1Dwon_MouseLeftButtonDown), true);
            C1Dwon.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.C1Dwon_MouseLeftButtonUp), true);

            C1Up.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.C1Up_MouseLeftButtonDown), true);
            C1Up.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.C1Up_MouseLeftButtonUp), true);

            //-- C2 --
            C2Dwon.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.C2Dwon_MouseLeftButtonDown), true);
            C2Dwon.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.C2Dwon_MouseLeftButtonUp), true);

            C2Up.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.C2Up_MouseLeftButtonDown), true);
            C2Up.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.C2Up_MouseLeftButtonUp), true);
        }
        
        private void LeftReset_Click(object sender, RoutedEventArgs e)
        {
            plcAirValve.TurnOnPlc(ActionIndex.LEFT_RESET);
        }

        private void RightReset_Click(object sender, RoutedEventArgs e)
        {
            plcAirValve.TurnOnPlc(ActionIndex.RIGHT_RESET);
        }

        private void C1Up_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void C1Dwon_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void C2Up_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void C2Dwon_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void Y1Dwon_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void Y2Dwon_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void Y2Up_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void C1Dwon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve?.TurnOn(ActionIndex.C1_POS);
            }
        }

        private void C1Dwon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve?.TurnOff(ActionIndex.C1_POS);
            }
        }

        private void C1Up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve?.TurnOn(ActionIndex.C1_NEG);
            }
        }

        private void C1Up_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve?.TurnOff(ActionIndex.C1_NEG);
            }
        }

        private void C2Up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve?.TurnOn(ActionIndex.C2_NEG);
            }
        }

        private void C2Up_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve.TurnOff(ActionIndex.C2_NEG);
            }
        }

        private void C2Dwon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve?.TurnOn(ActionIndex.C2_POS);
            }
        }

        private void C2Dwon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve.TurnOff(ActionIndex.C2_POS);
            }
        }

        private void Y1Dwon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve.TurnOn(ActionIndex.Y1_NEG);
            }
        }

        private void Y1Dwon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve.TurnOff(ActionIndex.Y1_NEG);
            }
        }

        private void Y2Dwon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve.TurnOn(ActionIndex.Y2_NEG);
            }
        }

        private void Y2Dwon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve.TurnOff(ActionIndex.Y2_NEG);
            }
        }

        private void Y1Up_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void Y1Up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve.TurnOn(ActionIndex.Y1_POS);
            }
        }

        private void Y1Up_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve.TurnOff(ActionIndex.Y1_POS);
            }
        }

        private void Y2Up_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                plcAirValve.TurnOn(ActionIndex.Y2_POS);
            }
        }

        private void Y2Up_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                plcAirValve.TurnOff(ActionIndex.Y2_POS);
            }
        }

        private void Y2Slider_ThumbDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (viewModel is not null)
            {
                int var = 100 * viewModel.Y2Speed;
                plcAirValve.SendToPlc(SpeedAddress.Y2_SPD, var);
            }
        }

        private void Y1Slider_ThumbDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (viewModel is not null)
            {
                int var = 100 * viewModel.Y1Speed;
                plcAirValve.SendToPlc(SpeedAddress.Y1_SPD, var);
            }
        }

        private void C2Slider_ThumbDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (viewModel is not null)
            {
                int var = 100 * viewModel.C2Speed;
                plcAirValve.SendToPlc(SpeedAddress.C2_SPD, var);
            }
        }

        private void C1Slider_ThumbDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            if (viewModel is not null)
            {
                int var = 100 * viewModel.C1Speed;
                plcAirValve.SendToPlc(SpeedAddress.C1_SPD, var);
            }
        }
    }
}
