using System.Windows;
using System.Windows.Input;
using RobotWeld.ViewModel;

namespace RobotWeld
{
    /// <summary>
    /// InputLaserParameter.xaml 的交互逻辑
    /// </summary>
    public partial class InputLaserParameter : Window
    {
        private InputLaserParameterViewModel? _laserParameterView;
        public InputLaserParameter()
        {
            _laserParameterView = new InputLaserParameterViewModel();
            InitializeComponent();
        }

        // The keyboard actions
        private void Frequency_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void Frequency_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void PulseWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            DutyCycle.SelectAll();
        }

        private void PulseWidth_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void DutyCycle_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void DutyCycle_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void LaserPower_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void LaserPower_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void AirIn_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void AirIn_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void LaserRise_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void LaserRise_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void LaserHoldtime_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void LaserHoldtime_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void WireTime_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void WireTime_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void LaserFall_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void LaserFall_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void AirOut_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void AirOut_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void WeldSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void WeldSpeed_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void WobbleSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void WobbleSpeed_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
