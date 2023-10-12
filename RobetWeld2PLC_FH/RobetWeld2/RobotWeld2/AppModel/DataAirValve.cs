using RobotWeld2.ViewModel;

namespace RobotWeld2.AppModel
{
    /// <summary>
    /// The class to transform data between air valve and main window
    /// </summary>
    public static class DataAirValve
    {
        public static AirValveViewModel? airValveViewModel { get; set; }

        public static void TakeAirValueViewModel(AirValveViewModel vvm)
        {
            airValveViewModel = vvm;
        }
    }
}
