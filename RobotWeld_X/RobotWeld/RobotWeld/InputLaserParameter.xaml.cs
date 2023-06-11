using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld.ViewModel;
using RobotWeld.Welding;

namespace RobotWeld
{
    /// <summary>
    /// InputLaserParameter.xaml 的交互逻辑
    /// </summary>
    public partial class InputLaserParameter : Window
    {
        private readonly DaemonFile daemonFile;
        private readonly InputViewModel viewModel;

        public InputLaserParameter(DaemonFile dmFile)
        {
            viewModel = new InputViewModel();
            daemonFile = dmFile;
            
            InitializeComponent();
            viewModel.LaserPower = daemonFile.LaserPower;
            viewModel.Frequency = daemonFile.Frequency;
            viewModel.LaserPulseWidth = daemonFile.PulseWidth;
            viewModel.LaserDutyCycle = daemonFile.DutyCycle;
            viewModel.LaserRise = daemonFile.LaserRise;
            viewModel.LaserFall = daemonFile.LaserFall;
            viewModel.LaserHoldtime = daemonFile.LaserHoldtime;
            viewModel.WireTime = daemonFile.WireTime;
            viewModel.AirIn = daemonFile.AirIn;
            viewModel.AirOut = daemonFile.AirOut;
            viewModel.WeldSpeed = daemonFile.WeldSpeed;
            viewModel.WobbleSpeed = daemonFile.WobbleSpeed;

            this.DataContext = viewModel;
        }

        //-- The keyboard actions --
        private void Frequency_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void Frequency_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void PulseWidth_GotFocus(object sender, RoutedEventArgs e)
        {
            PulseWidth.SelectAll();
        }

        private void PulseWidth_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void DutyCycle_GotFocus(object sender, RoutedEventArgs e)
        {
            DutyCycle.SelectAll();
        }

        private void DutyCycle_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LaserPower_GotFocus(object sender, RoutedEventArgs e)
        {
            LaserPower.SelectAll();
        }

        private void LaserPower_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void AirIn_GotFocus(object sender, RoutedEventArgs e)
        {
            AirIn.SelectAll();
        }

        private void AirIn_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LaserRise_GotFocus(object sender, RoutedEventArgs e)
        {
            LaserRise.SelectAll();
        }

        private void LaserRise_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LaserHoldtime_GotFocus(object sender, RoutedEventArgs e)
        {
            LaserHoldtime.SelectAll();
        }

        private void LaserHoldtime_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WireTime_GotFocus(object sender, RoutedEventArgs e)
        {
            WireTime.SelectAll();
        }

        private void WireTime_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LaserFall_GotFocus(object sender, RoutedEventArgs e)
        {
            LaserFall.SelectAll();
        }

        private void LaserFall_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void AirOut_GotFocus(object sender, RoutedEventArgs e)
        {
            AirOut.SelectAll();
        }

        private void AirOut_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WeldSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            WeldSpeed.SelectAll();
        }

        private void WeldSpeed_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WobbleSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            WobbleSpeed.SelectAll();
        }

        private void WobbleSpeed_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Frequency_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void LaserPower_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void PulseWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void DutyCycle_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void AirIn_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void LaserRise_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void LaserHoldtime_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void WireTime_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void LaserFall_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void AirOut_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void WobbleSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void WeldSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
