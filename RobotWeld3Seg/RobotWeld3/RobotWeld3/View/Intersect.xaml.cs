///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) new laser weld parameter from the server or local
// Ver. 3.0: (1) add 2 xml files to store points' information and using
//               4 data structures to represent moving, laser and
//               material specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using RobotWeld3.AlgorithmsBase;
using RobotWeld3.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld3.Motions;

namespace RobotWeld3.View
{
    /// <summary>
    /// Intersect.xaml 的交互逻辑
    /// </summary>
    public partial class Intersect : Window
    {
        private readonly IntersectViewModel _viewModel;
        private readonly IntersectModel _intersectModel;

        public Intersect()
        {
            _viewModel = new IntersectViewModel();
            _intersectModel = new IntersectModel();

            InitializeComponent();
            DataContext = _viewModel;
        }

        internal void PutWorkPackage(WorkPackage wk)
        {
            _intersectModel.PutWorkPackage(wk);
            _intersectModel.SetIntersectParameter(_viewModel);
        }

        private void VerticalTube_GotFocus(object sender, RoutedEventArgs e)
        {
            VerticalTube.SelectAll();
        }

        private void VerticalTube_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) return;

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void VerticalTube_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key)) e.Handled = false;
            else e.Handled = true;
        }

        private void HorizonalTube_GotFocus(object sender, RoutedEventArgs e)
        {
            HorizonalTube.SelectAll();
        }

        private void HorizonalTube_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) return;

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void HorizonalTube_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyInput.IsNumber(e.Key)) e.Handled = false;
            else e.Handled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var pmlist = new List<double>
            {
                _viewModel.InstalStyleValue,
                _viewModel.UpperRadius,
                _viewModel.LowerRadius,
            };

            _intersectModel.FromViewModel(pmlist);
            Close();
        }
    }
}
