using RobotWeld.AlgorithmsBase;
using RobotWeld.ViewModel;
using RobotWeld.Welding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld
{
    /// <summary>
    /// VaneWheel.xaml 的交互逻辑
    /// </summary>
    public partial class VaneWheel : Window
    {
        private readonly DaemonFile daemonFile;
        private readonly VaneWheelViewModel viewModel;
        private readonly VaneFile? vFile;

        public VaneWheel(DaemonFile dmFile)
        {
            viewModel = new VaneWheelViewModel();
            // vFile = new VaneFile();
            daemonFile = dmFile;

            // vFile.SetViewParameter(daemonFile.TraceIndex, viewModel);

            InitializeComponent();

            this.DataContext = viewModel;
        }

        #region button function
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // keep the old vane parameters
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Write the vane wheel information
/*            if (daemonFile is not null)
            {
                daemonFile.MaterialIndex = viewModel.MaterialIndex;
                daemonFile.SheetThickness = viewModel.SheetThickness;
                daemonFile.WireDiameter = viewModel.WireDiameter;
            }*/
            this.Close();
        }

        private void VaneNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            VaneNumber.SelectAll();
        }

        private void VaneNumber_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void VaneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                    e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Y01Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y01Position_GotFocus(object sender, RoutedEventArgs e)
        {
            Y01Position.SelectAll();
        }

        private void Y01Position_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || 
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Y01Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            Y01Velocity.SelectAll();
        }

        private void Y01Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y01Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || 
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void C01Position_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || 
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void C01Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C01Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C01Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Y11Position_GotFocus(object sender, RoutedEventArgs e)
        {
            Y11Position.SelectAll();
        }

        private void Y11Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y11Position_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || 
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Y11Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            Y11Velocity.SelectAll();
        }

        private void Y11Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || 
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void C02Position_GotFocus(object sender, RoutedEventArgs e)
        {
            C02Position.SelectAll();
        }

        private void C02Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C02Position_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || 
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }


        private void C2Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C2Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || 
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Y2Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void VaneType_Changed(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion

        private void C01Velocity_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C01Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C03Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C03Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C03Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C04Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C04Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C04Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C05Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C05Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C05Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C06Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C06Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C06Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C07Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C07Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C07Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Y11Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C11Velocity_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C11Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C11Velocity_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C11Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C11Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C11Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C12Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C12Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C12Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C13Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C13Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C13Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C14Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C14Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C4Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C15Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C15Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C15Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C16Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C16Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C16Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void C7Position_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void C17Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void C17Position_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Y01_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C02_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C01_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C03_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C04_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C05_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C06_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C07_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Y11_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C11_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C12_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C13_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C14_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C15_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C16_Click(object sender, RoutedEventArgs e)
        {

        }

        private void C17_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
