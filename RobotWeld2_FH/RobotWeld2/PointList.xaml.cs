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
        private readonly MainModel? _mainModel;

        public PointList()
        {
            _mainModel = MainWindow.GetMainModel();
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainModel?.CancelChoose();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _mainModel?.OkChoose();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _mainModel?.DeletePoint();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            _mainModel?.ChangePoint();
        }
    }
}
