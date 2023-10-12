using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using RobotWeld2.AppModel;
using System.Windows.Controls;

namespace RobotWeld2
{
    /// <summary>
    /// PlcIo.xaml 的交互逻辑
    /// </summary>
    public partial class PlcIo : UserControl
    {
        private PlcIoViewModel? viewModel;
        private PlcIoModel? plcIoModel;

        public PlcIo()
        {
            viewModel = new PlcIoViewModel();
            plcIoModel = new(viewModel);

            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
