using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;

namespace RobotWeld2.AppModel
{
    public class SpiralCurveModel
    {
        private WorkPackage? workPackage;
        private double _pitch;

        public SpiralCurveModel() { }

        public double Pitch
        {
            get { return this._pitch; }
            set { this._pitch = value; }
        }

        public void SetParameter(SpiralViewModel svm)
        {
           svm.Pitch = this._pitch;
        }

        public void SaveParameter(SpiralViewModel svm)
        { 
            this._pitch = svm.Pitch;

            if (workPackage != null)
            {
                BasicWorkFile bwf = new();
                bwf.SaveSpiralParameter(workPackage, _pitch);
            }
        }

        public void GetParameter(WorkPackage workPackage)
        {
            this.workPackage = workPackage;
            BasicWorkFile bwf = new();
            bwf.GetSpiralParameter(workPackage, out _pitch);
        }
    }
}
