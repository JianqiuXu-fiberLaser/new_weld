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
            vFile = new VaneFile();
            daemonFile = dmFile;

            vFile.SetViewParameter(daemonFile.TraceIndex, viewModel);

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

        private void Y1Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y1Position_GotFocus(object sender, RoutedEventArgs e)
        {
            Y1Position.SelectAll();
        }

        private void Y1Position_KeyDown(object sender, KeyEventArgs e)
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

        private void Y1Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            Y1Velocity.SelectAll();
        }

        private void Y1Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y1Velocity_KeyDown(object sender, KeyEventArgs e)
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

        private void C1Position_GotFocus(object sender, RoutedEventArgs e)
        {
            C1Position.SelectAll();
        }

        private void C1Position_KeyDown(object sender, KeyEventArgs e)
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

        private void C1Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C1Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C1Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            C1Velocity.SelectAll();
        }

        private void C1Velocity_KeyDown(object sender, KeyEventArgs e)
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

        private void Y2Position_GotFocus(object sender, RoutedEventArgs e)
        {
            Y2Position.SelectAll();
        }

        private void Y2Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y2Position_KeyDown(object sender, KeyEventArgs e)
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

        private void Y2Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            Y2Velocity.SelectAll();
        }

        private void Y2Velocity_KeyDown(object sender, KeyEventArgs e)
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

        private void C2Position_GotFocus(object sender, RoutedEventArgs e)
        {
            C2Position.SelectAll();
        }

        private void C2Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C2Position_KeyDown(object sender, KeyEventArgs e)
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

        private void C2Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            C2Velocity.SelectAll();
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
    }
}
