///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) integral the function of visual point list. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.AppModel;
using RobotWeld3.GetTrace;
using RobotWeld3.Motions;
using RobotWeld3.ViewModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld3.View
{
    /// <summary>
    /// PointList.xaml 的交互逻辑
    /// </summary>
    public partial class PointList : UserControl
    {
        private PointListViewModel? _viewModel;
        private static PointListModel? _ptModel;
        private static MainModel? _mw;
        private readonly TakeTrace? _takeTrace;
        private readonly MotionBus? _mbus;

        public PointList()
        {
            _viewModel = new PointListViewModel();
            _ptModel = MainWindow.GetPointListModel();
            _mw = MainWindow.GetMainModel();

            _ptModel.PutViewModel(in _viewModel);
            var wk = _mw.GetWorkPackage();
            _ptModel.PutWorkPackage(in wk);
            _mbus = _mw.GetMotionBus();
            _takeTrace = _mw.GetTakeTrace();

            InitializeComponent();
            pointListBox.ItemsSource = _viewModel.PointList;
            DataContext = _viewModel;
        }

        private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Executed command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // delete point
            if (e.Command == PointListViewModel.DeletePointCommand)
            {
                _ptModel?.DeletePoint();
            }

            // Add point with the current position after this point
            if (e.Command == PointListViewModel.AddPointCommand)
            {
                if (_takeTrace != null) _ptModel?.AddPoint(_takeTrace);
            }

            // change the line type to line
            if (e.Command == PointListViewModel.ChangeLineCommand)
            {
                _ptModel?.ChangeLine();
            }

            // change the line type to arc
            if (e.Command == PointListViewModel.ChangeArcCommand)
            {
                _ptModel?.ChangeArc();
            }
        }

        /// <summary>
        /// when chose to cancel button, then back to old point list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _ptModel?.CancelChoose();
        }

        /// <summary>
        /// when chose the OK buttton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _ptModel?.OkChoose();
        }

        /// <summary>
        /// when chose the change button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_takeTrace != null) _ptModel?.ChangePoint(_takeTrace);
        }

        /// <summary>
        /// When chose goto the position button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArriveButton_Click(object sender, RoutedEventArgs e)
        {
            MotionBus.StopActionThread();
            if (_mbus != null)
            {
                Thread.Sleep(5);
                _ptModel?.GotoSingleStep(_mbus);
            }
        }

        /// <summary>
        /// When double click the point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListViewItemDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_ptModel == null || _viewModel == null) return;

            int i = _viewModel.PtIndex;
            var ilp = new InputLaserParameter();
            ilp.PutPointModel(_ptModel, i);
            ilp.ShowDialog();

            _ptModel.RefreshParameter(i);
        }
    }
}
