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
using RobotWeld3.AppModel;
using RobotWeld3.Motions;
using RobotWeld3.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld3.View
{
    /// <summary>
    /// SuperUser.xaml 的交互逻辑
    /// </summary>
    public partial class SuperUser : Window
    {
        private readonly SuperUserViewModel? _viewModel;
        private readonly SuperUserModel? _model;

        public SuperUser(int timestamp)
        {
            _viewModel = new SuperUserViewModel();
            _model = new SuperUserModel(_viewModel);
            _model.ReadCoefficient(timestamp);

            InitializeComponent();
            DataContext = _viewModel;

            ButtonAddHandle();
        }

        public void PutMotionBus(MotionBus ms)
        {
            _model?.PutMotionBus(ms);
        }

        /// <summary>
        /// Add button handle to the command when the button be click.
        /// </summary>
        private void ButtonAddHandle()
        {
            //-- Feed button --
            FeedButton.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Feed_MouseLeftButtonDown), true);
            FeedButton.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Feed_MouseLeftButtonUp), true);

            BackButton.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.Back_MouseLeftButtonDown), true);
            BackButton.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.Back_MouseLeftButtonUp), true);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _model?.SaveCoefficient();
            this.Close();
        }

        private void Feed_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void Feed_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                _model?.FeedIn();
            }
        }

        private void Feed_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                _model?.FeedStop();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // do nothing
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.CaptureMouse();
                _model?.BackIn();
            }
        }

        private void Back_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                element.ReleaseMouseCapture();
                _model?.BackStop();
            }
        }

        private void FeedSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            FeedSpeed.SelectAll();
        }

        private void BackSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            BackSpeed.SelectAll();
        }

        private void RefeedLength_GotFocus(object sender, RoutedEventArgs e)
        {
            RefeedLength.SelectAll();
        }

        private void BackLength_GotFocus(object sender, RoutedEventArgs e)
        {
            BackLength.SelectAll();
        }

        private void FeedSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void BackSpeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void RefeedLength_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void BackLength_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void FeedSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void BackSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void RefeedLength_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void BackLength_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void ProtectAir_GotFocus(object sender, RoutedEventArgs e)
        {
            ProtectAir.SelectAll();
        }

        private void RiseEdge_GotFocus(object sender, RoutedEventArgs e)
        {
            RiseEdge.SelectAll();
        }

        private void FallEdge_GotFocus(object sender, RoutedEventArgs e)
        {
            FallEdge.SelectAll();
        }

        private void ProtectAir_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void RiseEdge_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void FallEdge_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void ProtectAir_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void RiseEdge_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void FallEdge_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
