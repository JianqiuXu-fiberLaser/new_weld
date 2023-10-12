
using RobotWeld2.AppModel;
using System.Windows;
using System.Windows.Controls;

namespace RobotWeld2
{
    /// <summary>
    /// PointList.xaml 的交互逻辑
    /// </summary>
    public partial class PointList : UserControl
    {
        private readonly DaemonModel? _dmModel;

        public PointList()
        {
            _dmModel = MainWindow.GetMainModel();
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _dmModel?.SelectionOk();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _dmModel?.CancelChoose();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            _dmModel?.AddPoint();
        }
    }
}
