using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// InputSpiral.xaml 的交互逻辑
    /// </summary>
    public partial class InputSpiral : Window
    {
        private readonly SpiralViewModel viewModel;
        private readonly SpiralCurveModel spiralModel;
        public InputSpiral(SpiralCurveModel scm)
        {
            viewModel = new SpiralViewModel();
            this.spiralModel = scm;
            spiralModel.SetParameter(viewModel);

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

        private void Pitch_GotFocus(object sender, RoutedEventArgs e)
        {
            Pitch.SelectAll();
        }

        private void Pitch_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Pitch_KeyDown(object sender, KeyEventArgs e)
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
            spiralModel.SaveParameter(viewModel);
            this.Close();
        }
    }
}
