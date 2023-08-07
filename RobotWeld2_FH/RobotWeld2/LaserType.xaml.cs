using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// LaserType.xaml 的交互逻辑
    /// </summary>
    public partial class LaserType : Window
    {
        // the object to be get from the Parameter viewModel
        private readonly LaserTypeViewModel viewModel;

        public LaserType()
        {
            viewModel = new LaserTypeViewModel();
            BasicWorkFile bwf = new();
            viewModel.MaxLaserPower = bwf.GetMaxPower();

            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void LaserTypeSelected_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            // Do nothing in this version.
        }

        private void MaxLaserPower_GotFocus(object sender, RoutedEventArgs e)
        {
            MaxLaserPower.SelectAll();
        }

        private void MaxLaserPower_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MotionOperate.SetMaxPower(viewModel.MaxLaserPower);
            BasicWorkFile bwf = new();
            bwf.PutMaxPower(viewModel.MaxLaserPower);
            Close();
        }

        private void MaxLaserPower_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                if (e.Key == Key.Enter)
                {
                    BasicWorkFile bwf = new();
                    bwf.PutMaxPower(viewModel.MaxLaserPower);
                    MotionOperate.SetMaxPower((int)viewModel.MaxLaserPower);
                    Close();
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
