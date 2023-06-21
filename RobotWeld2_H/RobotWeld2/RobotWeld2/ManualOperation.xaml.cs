using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// ManualOperation.xaml 的交互逻辑
    /// </summary>
    public partial class ManualOperation : UserControl
    {
        private ManualOperationViewModel viewModel;
        private ManualOperationModel manualOperationModel;

        public ManualOperation()
        {
            viewModel = new ManualOperationViewModel();
            manualOperationModel = new ManualOperationModel(viewModel);
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

        private void LeftClip_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.LeftClip();
        }

        private void LeftLocation_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.LeftLocation();
        }

        private void RightClip_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.RightClip();
        }

        private void RightLocation_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.RightLocation();
        }

        private void SWaddress_GotFocus(object sender, RoutedEventArgs e)
        {
            SWaddress.SelectAll();
        }

        private void SWaddress_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void SWaddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    manualOperationModel.ReadMValue();
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void LeftJack_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.LeftJack();
        }

        private void RightJack_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.RightJack();
        }
    }
}
