using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld.ViewModel;
using RobotWeld.Welding;

namespace RobotWeld
{
    /// <summary>
    /// SelectMaterial.xaml 的交互逻辑
    /// </summary>
    
    public partial class SelectMaterial : Window
    {
        // the object to be get from the Parameter viewModel
        private readonly DaemonFile? daemonFile;
        private readonly SelectViewModel viewModel;

        // load the material file from the project file
        public SelectMaterial(DaemonFile dmFile)
        {
            viewModel = new SelectViewModel();
            daemonFile = dmFile;
            InitializeComponent();

            viewModel.MaterialIndex = daemonFile.MaterialIndex;
            viewModel.SheetThickness = daemonFile.SheetThickness;
            viewModel.WireDiameter = daemonFile.WireDiameter;

            this.DataContext = viewModel;
        }

        //-- The keyboard actions --
        private void SheetThickness_GotFocus(object sender, RoutedEventArgs e)
        {
            SheetThickness.SelectAll();
        }

        private void SheetThickness_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WireDiameter_GotFocus(object sender, RoutedEventArgs e)
        {
            WireDiameter.SelectAll();
        }

        private void WireDiameter_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        // save the selection
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Write the material information
            if (daemonFile is not null)
            {
                daemonFile.MaterialIndex = viewModel.MaterialIndex;
                daemonFile.SheetThickness = viewModel.SheetThickness;
                daemonFile.WireDiameter = viewModel.WireDiameter;
            }
            this.Close();
        }

        // discard the selection
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // keep the old material parameters
            this.Close();
        }

        // when the selection of comboBox is changed
        private void WeldMaterial_Changed(object sender, SelectionChangedEventArgs e)
        {
            // TODO
        }

        // limit to input digit numbers only
        private void SheetThickness_KeyDown(object sender, KeyEventArgs e)
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

        private void WireDiameter_KeyDown(object sender, KeyEventArgs e)
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
    }
}
