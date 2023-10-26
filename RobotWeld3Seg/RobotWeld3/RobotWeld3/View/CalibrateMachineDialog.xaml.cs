///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using RobotWeld3.Motions;
using RobotWeld3.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld3.View
{
    /// <summary>
    /// CalibrateMachineDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CalibrateMachineDialog : Window
    {
        private readonly CalibrateMachineViewModel _viewModel;
        private readonly CalibrateMachineModel _model;

        public CalibrateMachineDialog()
        {
            _viewModel = new CalibrateMachineViewModel();
            _model = new CalibrateMachineModel();
            _model.ReadCfgFile(_viewModel);

            InitializeComponent();
            this.DataContext = _viewModel;
        }

        internal void PutMotionOperate(MotionBus mbus)
        {
            _model.GetMotionOperate(mbus);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _model?.CalibrateMachine();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _model?.ConfirmResult();
            this.Close();
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

        private void XPulse_GotFocus(object sender, RoutedEventArgs e)
        {
            XPulse.SelectAll();
        }

        private void XPulse_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void XPulse_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void YPulse_GotFocus(object sender, RoutedEventArgs e)
        {
            YPulse.SelectAll();
        }

        private void YPulse_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void YPulse_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void ZPulse_GotFocus(object sender, RoutedEventArgs e)
        {
            ZPulse.SelectAll();
        }

        private void ZPulse_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Zpulse_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void APulse_GotFocus(object sender, RoutedEventArgs e)
        {
            APulse.SelectAll();
        }

        private void APulse_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void APulse_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void PedalTrigger_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void ProtectedAir_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void PedalTrigger_GotFocus(object sender, RoutedEventArgs e)
        {
            PedalTrigger.SelectAll();
        }

        private void ProtectedAir_GotFocus(object sender, RoutedEventArgs e)
        {
            ProtectedAir.SelectAll();
        }

        private void BurninButton_Click(object sender, RoutedEventArgs e)
        {
            _model?.BurninMachine(true);
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            _model?.BurninMachine(false);
        }

        private void AaxisCheck(object sender, RoutedEventArgs e)
        {
            _viewModel.AaxisState = 1;
            _viewModel.AaxisBool = true;
        }

        private void AaxisUnchecked(object sender, RoutedEventArgs e)
        {
            _viewModel.AaxisState = 0;
            _viewModel.AaxisBool = false;
        }

        private void PedalTrigger_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void ProtectedAir_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LaserEnable_GotFocus(object sender, RoutedEventArgs e)
        {
            LaserEnable.SelectAll();
        }

        private void Wobble_GotFocus(object sender, RoutedEventArgs e)
        {
            Wobble.SelectAll();
        }

        private void FeedWire_GotFocus(object sender, RoutedEventArgs e)
        {
            FeedWire.SelectAll();
        }

        private void Withdraw_GotFocus(object sender, RoutedEventArgs e)
        {
            Withdraw.SelectAll();
        }

        private void WireDac_GotFocus(object sender, RoutedEventArgs e)
        {
            WireDac.SelectAll();
        }

        private void LaserEnable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Wobble_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void FeedWire_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Withdraw_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WireDac_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LaserEnable_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void Wobble_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void FeedWire_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void Withdraw_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void WireDac_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
