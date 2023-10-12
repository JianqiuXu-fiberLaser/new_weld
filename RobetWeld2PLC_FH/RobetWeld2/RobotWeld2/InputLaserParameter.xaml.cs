///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld2.ViewModel;
using RobotWeld2.AppModel;

namespace RobotWeld2
{
    /// <summary>
    /// InputLaserParameter.xaml 的交互逻辑
    /// </summary>
    public partial class InputLaserParameter : Window
    {
        private readonly DaemonModel _dmModel;
        private readonly InputViewModel viewModel;

        public InputLaserParameter(DaemonModel dm)
        {
            viewModel = new InputViewModel();
            _dmModel = dm;
            _dmModel.GetLaserParameter(viewModel);

            InitializeComponent();
            this.DataContext = viewModel;
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
            _dmModel.SetLaserParameter(viewModel);
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
    }
}
