///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) new point definition with laser paremeter
//
///////////////////////////////////////////////////////////////////////

using RobotWeld2.AppModel;
using System.Windows;
using System.Windows.Controls;

namespace RobotWeld2
{
    /// <summary>
    /// ToggleButtonPoint.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleButtonPoint : UserControl
    {
        private DaemonModel? _dmModel;
        public ToggleButtonPoint()
        {
            InitializeComponent();
        }

        private void TraceDirButton_Checked(object sender, RoutedEventArgs e)
        {
            _dmModel ??= MainWindow.GetMainModel();
            _dmModel?.ChangeTraceDir();
        }

        private void TraceDirButton_UnChecked(object sender, RoutedEventArgs e)
        {
            _dmModel ??= MainWindow.GetMainModel();
            _dmModel?.ChangeTraceDir();
        }
    }
}
