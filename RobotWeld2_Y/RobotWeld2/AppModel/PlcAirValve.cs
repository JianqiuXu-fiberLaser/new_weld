using RobotWeld2.AlgorithmsBase;
using RobotWeld2.ViewModel;
using System.Threading;

namespace RobotWeld2.AppModel
{
    public class PlcAirValve
    {
        private AirValveViewModel? viewModel;
        private static ExtraController? extraController;

        public PlcAirValve(AirValveViewModel vvm)
        {
            this.viewModel = vvm;
            extraController = new ExtraController(viewModel);
            extraController.ConnectPLC();
        }

        public void SendToPlc(SpeedAddress addr, int data)
        {
            extraController?.SendPlcData(addr, data);
        }

        public void TurnOnPlc(ActionIndex ati)
        {
            extraController?.SelfResetTurnOn(ActionIndex.AUTO_MODE);
            Thread.Sleep(20);
            extraController?.SelfResetTurnOn(ati);
            Thread.Sleep(20);
            extraController?.CheckPreparedState(ati);
        }

        public void TurnOn(ActionIndex ati)
        {
            extraController?.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);
            extraController?.TurnOn(ati);
        }

        public void TurnOff(ActionIndex ati)
        {
            extraController?.TurnOff(ati);
        }
    }
}
