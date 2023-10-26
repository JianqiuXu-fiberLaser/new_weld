///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: revise the point list to match single workpackage file.
//           (1) using laserDisplayParameter for display on UI only.
// Ver. 3.0: (1) the construct has not input parameter, that losing
//               the connection between differernt modules.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using RobotWeld3.ViewModel;
using RobotWeld3.AlgorithmsBase;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld3.Motions;

namespace RobotWeld3.View
{
    /// <summary>
    /// InputLaserParameter.xaml 的交互逻辑
    /// </summary>
    public partial class InputLaserParameter : Window
    {
        private readonly InputViewModel _viewModel;
        private readonly InputLaserParameterModel _inputModel;

        internal InputLaserParameter()
        {
            _viewModel = new InputViewModel();
            _inputModel = new InputLaserParameterModel();
            InitializeComponent();
            DataContext = _viewModel;
        }

        internal void PutPointModel(PointListModel ptm, int ipd)
        {
            _inputModel.PutPointModel(ptm, ipd);
            _inputModel.SetLaserParameter(_viewModel);
        }

        //-- The keyboard actions --
        private void Frequency_GotFocus(object sender, RoutedEventArgs e)
        {
            Frequency.SelectAll();
        }

        private void Frequency_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void DutyCycle_GotFocus(object sender, RoutedEventArgs e)
        {
            DutyCycle.SelectAll();
        }

        private void DutyCycle_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void LaserPower_GotFocus(object sender, RoutedEventArgs e)
        {
            LaserPower.SelectAll();
        }

        private void LaserPower_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Speed_GotFocus(object sender, RoutedEventArgs e)
        {
            Speed.SelectAll();
        }

        private void Speed_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WobbleSpeed_GotFocus(object sender, RoutedEventArgs e)
        {
            WobbleSpeed.SelectAll();
        }

        private void WobbleSpeed_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Frequency_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void LaserPower_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void DutyCycle_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void WobbleSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _inputModel?.PutLaserParameter(_viewModel);
            this.Close();
        }

        private void Thickness_GotFocus(object sender, RoutedEventArgs e)
        {
            Thickness.SelectAll();
        }

        private void Thickness_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Thickness_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key)) e.Handled = false;
            else e.Handled = true;
        }

        private void WireDiameter_GotFocus(object sender, RoutedEventArgs e)
        {
            WireDiameter.SelectAll();
        }

        private void WireDiameter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WireDiameter_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key)) e.Handled = false;
            else e.Handled = true;
        }

        private void WeldMaterial_Changed(object sender, SelectionChangedEventArgs e)
        {
            // ToDo: when the material has changed.
        }

        private void Speed_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsIntNumber(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
