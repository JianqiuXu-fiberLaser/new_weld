using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
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

        public void PutMotionOperate(MotionOperate mo)
        {
            _model.GetMotionOperate(mo);
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
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
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
