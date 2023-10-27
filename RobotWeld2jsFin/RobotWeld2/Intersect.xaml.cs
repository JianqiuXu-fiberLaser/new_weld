using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// Intersect.xaml 的交互逻辑
    /// </summary>
    public partial class Intersect : Window
    {
        private readonly IntersectViewModel viewModel;
        private readonly IntersectModel intersectModel;

        public Intersect(IntersectModel intersectModel)
        {
            viewModel = new IntersectViewModel();
            this.intersectModel = intersectModel;
            intersectModel.SetParameter(viewModel);

            InitializeComponent();
            this.DataContext = viewModel;
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

        private void VerticalTube_GotFocus(object sender, RoutedEventArgs e)
        {
            VerticalTube.SelectAll();
        }

        private void VerticalTube_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void VerticalTube_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void HorizonalTube_GotFocus(object sender, RoutedEventArgs e)
        {
            HorizonalTube.SelectAll();
        }

        private void HorizonalTube_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void HorizonalTube_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
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
            intersectModel.PutParameter(viewModel);
            this.Close();
        }
    }
}
