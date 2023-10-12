///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld2.ViewModel;
using RobotWeld2.AppModel;

namespace RobotWeld2
{
    /// <summary>
    /// SelectMaterial.xaml 的交互逻辑
    /// </summary>
    
    public partial class SelectMaterial : Window
    {
        // the object to be get from the Parameter viewModel
        private readonly DaemonModel _dmModel;
        private readonly SelectViewModel viewModel;

        // load the material file from the project file
        public SelectMaterial(DaemonModel dm)
        {
            viewModel = new SelectViewModel();
            _dmModel = dm;

            InitializeComponent();

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
            // Do nothing in this version.
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
