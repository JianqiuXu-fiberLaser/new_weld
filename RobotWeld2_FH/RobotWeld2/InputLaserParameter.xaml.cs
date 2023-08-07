using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// InputLaserParameter.xaml 的交互逻辑
    /// </summary>
    public partial class InputLaserParameter : Window
    {
        private readonly InputViewModel _viewModel;
        private readonly InputLaserParameterModel? _inputLaserParameterModel;

        public InputLaserParameter(InputLaserParameterModel inputLaserParameterModel)
        {
            _viewModel = new InputViewModel();
            _inputLaserParameterModel = inputLaserParameterModel;
            inputLaserParameterModel.SetLaserParameter(_viewModel);

            InitializeComponent();
            this.DataContext = _viewModel;
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
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    if (_viewModel.Frequency != 0 && _viewModel.LaserPulseWidth != 0)
                    {
                        int dutyVar = (int)(100.0 * _viewModel.LaserPulseWidth / _viewModel.Frequency);
                        if (dutyVar >= 0 && dutyVar <= 100)
                        {
                            _viewModel.LaserDutyCycle = dutyVar;
                        }
                        else
                        {
                            _viewModel.LaserDutyCycle = 0;
                        }
                    }
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void LaserPower_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    if (_viewModel.Frequency != 0 && _viewModel.LaserPulseWidth != 0)
                    {
                        int dutyVar = (int)(100.0 * _viewModel.LaserPulseWidth / _viewModel.Frequency);
                        if (dutyVar >= 0 && dutyVar <= 100)
                        {
                            _viewModel.LaserDutyCycle = dutyVar;
                        }
                        else
                        {
                            _viewModel.LaserDutyCycle = 0;
                        }
                    }
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void DutyCycle_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    if (_viewModel.LaserDutyCycle >= 0 && _viewModel.LaserDutyCycle <= 100)
                    {
                        if (_viewModel.Frequency != 0)
                        {
                            _viewModel.LaserPulseWidth = (int)(_viewModel.LaserDutyCycle * _viewModel.Frequency / 100.0);
                        }
                    }
                    else
                    {
                        _viewModel.LaserDutyCycle = 0;
                    }
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void AirIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
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
            if (IsIntNumber(e.Key))
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
            _inputLaserParameterModel?.PutLaserParameter(_viewModel);
            this.Close();
        }

        private void LeapSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            LeapSpeed.SelectAll();
        }

        private void LeapSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LeapSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void WeldNum_GotFocus(object sender, RoutedEventArgs e)
        {
            WeldingNum.SelectAll();
        }

        private void WeldNum_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WeldNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
