using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.ViewModel;

namespace RobotWeld2
{
    /// <summary>
    /// PassWord.xaml 的交互逻辑
    /// </summary>
    public partial class PassWord : Window
    {
        private readonly PassWordViewModel? viewModel;
        private Encryption? encryption;

        public PassWord()
        {
            viewModel = new PassWordViewModel();
            InitializeComponent();
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Get entry of Encryption class
        /// </summary>
        /// <param name="ecyp"></param>
        public void GetEncryption(Encryption encp)
        {
            encryption = encp;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InWord_GotFocus(object sender, RoutedEventArgs e)
        {
            InWord.SelectAll();
        }

        private void InWord_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void InWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (encryption != null && viewModel != null)
                {
                    if (encryption.VerifyPassword(viewModel))
                        Close();
                    else
                        viewModel.Prompting = "密码错误";
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}
