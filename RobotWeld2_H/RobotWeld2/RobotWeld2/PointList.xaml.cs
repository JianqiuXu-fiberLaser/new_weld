using RobotWeld2.AppModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// PointList.xaml 的交互逻辑
    /// </summary>
    public partial class PointList : UserControl
    {
        private readonly MainModel? mainModel;

        public PointList()
        {
            this.mainModel = MainWindow.mainModel;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            mainModel?.CancelChoose();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            mainModel?.OkChoose();
        }

        private static bool IsNumber(Key key)
        {
            if (key == Key.Enter || key == Key.Back || key == Key.OemPeriod || key == Key.Decimal ||
                (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9))
            {
                return true;
            }
            else { return false; }
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

        private void ListLaserPower_GotFocus(object sender, RoutedEventArgs e)
        {
            ListLaserPower.SelectAll();
        }

        private void ListLaserPower_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void ListLaserPower_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void ListFrequency_GotFocus(object sender, RoutedEventArgs e)
        {
            ListFrequency.SelectAll();
        }

        private void ListFrequency_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void ListFrequency_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void ListWeldSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            ListWeldSpeed.SelectAll();
        }

        private void ListWeldSpeed_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void ListWeldSpeed_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void ListPulse_GotFocus(object sender, RoutedEventArgs e)
        {
            ListPulse.SelectAll();
        }

        private void ListPulse_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void ListPulse_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
