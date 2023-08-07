using RobotWeld2.AlgorithmsBase;
using RobotWeld2.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// Setup.xaml 的交互逻辑
    /// </summary>
    public partial class Setup : Window
    {
        private readonly string passfile = "./Storage/passfile.dat";
        private readonly SetupViewModel? viewModel;

        public Setup()
        {
            viewModel = new SetupViewModel();

            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel is not null)
            {
                if (viewModel.InputPassword1 == viewModel.InputPassword2)
                {
                    FileStream wfile = new(passfile, FileMode.Create);
                    StreamWriter sw = new(wfile);
                    sw.WriteLine(viewModel.InputPassword1);
                    sw.Close();
                    this.Close();
                }
                else
                {
                    new Werr().WaringMessage("两次输入不相符");
                }
            }
        }

        private void InWord1_GotFocus(object sender, RoutedEventArgs e)
        {
            InWord1.SelectAll();
        }

        private void InWord1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void InWord1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void InWord2_GotFocus(object sender, RoutedEventArgs e)
        {
            InWord2.SelectAll();
        }

        private void InWord2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void InWord2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled= false;
            }
        }
    }
}
