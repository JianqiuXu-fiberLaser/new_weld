///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: inherit interface of ITraceParameter
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
    /// InputSpiral.xaml 的交互逻辑
    /// </summary>
    public partial class InputSpiral : Window
    {
        private readonly SpiralViewModel _viewModel;
        private SpiralCurveModel _spiralModel;

        public InputSpiral()
        {
            _viewModel = new SpiralViewModel();
            _spiralModel = new SpiralCurveModel();
            InitializeComponent();
            DataContext = _viewModel;
        }

        /// <summary>
        /// Transfer the workpackage to the model
        /// </summary>
        /// <param name="wk"> workpackage </param>
        internal void PutWorkPackage(WorkPackage wk)
        {
            _spiralModel.PutWorkPackage(wk);
            _spiralModel.SetPitch(_viewModel);
        }

        private void Pitch_GotFocus(object sender, RoutedEventArgs e)
        {
            Pitch.SelectAll();
        }

        private void Pitch_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) return; 

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Pitch_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsIntNumber(e.Key)) e.Handled = false;
            else e.Handled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _spiralModel?.FromViewModel(_viewModel.Pitch);
            this.Close();
        }
    }
}
