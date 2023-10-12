using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Welding;
using RobotWeld2.ViewModel;

namespace RobotWeld2.AppModel
{
    public class PlcIoModel
    {
        private PlcIoViewModel viewModel;
        private static ExtraController? extraController;

        public PlcIoModel(PlcIoViewModel vvm)
        {
            this.viewModel = vvm;
            extraController = new(viewModel);
            extraController.ConnectPLC();
            extraController.SetScanIO(true);
            extraController.ScanPlcIo();
        }
    }
}
