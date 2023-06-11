using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RobotWeld2
{
    /// <summary>
    /// ManualOperation.xaml 的交互逻辑
    /// </summary>
    public partial class ManualOperation : UserControl
    {
        private ManualOperationViewModel viewModel;
        private ManualOperationModel manualOperationModel;

        public ManualOperation()
        {
            viewModel = new ManualOperationViewModel();
            manualOperationModel = new ManualOperationModel(viewModel);
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void LeftClip_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.LeftClip();
        }

        private void LeftLocation_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.LeftLocation();
        }

        private void RightClip_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.RightClip();
        }

        private void RightLocation_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.RightLocation();
        }

        private void SWaddrees_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void SWaddrees_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void SWaddrees_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void LeftJack_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.LeftJack();
        }

        private void RightJack_Click(object sender, RoutedEventArgs e)
        {
            manualOperationModel.RightJack();
        }
    }
}
