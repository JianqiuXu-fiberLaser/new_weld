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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace RobotWeld3.ViewModel
{
    /// <summary>
    /// The view model for the PointList User Control.
    /// </summary>

    internal class PointListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DisPoint>? _pointList;
        private int _ptIndex;

        internal PointListViewModel() { }

        public static RoutedCommand DeletePointCommand = new("DeletePoint", typeof(PointListViewModel));
        public static RoutedCommand AddPointCommand = new("AddPoint", typeof(PointListViewModel));
        public static RoutedCommand ChangeLineCommand = new("ChangeLine", typeof(PointListViewModel));
        public static RoutedCommand ChangeArcCommand = new("ChangeArc", typeof(PointListViewModel));

        //
        //-- Response for the ICommand Events in the MainWindow --
        // required by INotifPropertyChanged interface, regist in the first
        //
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<DisPoint> PointList
        {
            get { return _pointList ?? new ObservableCollection<DisPoint>(); }
            set
            {
                _pointList = value;
                OnPropertyChanged(nameof(PointList));
            }
        }

        public int PtIndex
        {
            get { return _ptIndex; }
            set { _ptIndex = value; OnPropertyChanged(); }
        }
    }
}
