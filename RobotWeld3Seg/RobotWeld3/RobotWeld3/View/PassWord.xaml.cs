///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld3.View
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
