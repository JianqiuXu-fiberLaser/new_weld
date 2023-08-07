using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;

namespace RobotWeld2.AppModel
{
    /// <summary>
    /// The model for intersect weld
    /// In this model, the data are included because only two data being used.
    /// </summary>
    public class IntersectModel
    {
        private WorkPackage? workPackage;
        private double upperRadius;
        private double lowerRadius;
        private int instalStyle;

        public IntersectModel() { }

        public void GetParameter(WorkPackage workPackage)
        {
            this.workPackage = workPackage;
            BasicWorkFile bwf = new();
            bwf.GetIntersectParameter(workPackage, out instalStyle, out upperRadius, out lowerRadius);
        }

        public void SetParameter(IntersectViewModel viewModel)
        {
            viewModel.InstalStyleValue = this.instalStyle;
            viewModel.UpperRadius = upperRadius;
            viewModel.LowerRadius = lowerRadius;
        }

        public void PutParameter(IntersectViewModel viewModel)
        {
            instalStyle = viewModel.InstalStyleValue;
            upperRadius = viewModel.UpperRadius;
            lowerRadius = viewModel.LowerRadius;

            if (workPackage != null)
            {
                BasicWorkFile bwf = new();
                bwf.SaveIntersectParameter(workPackage, instalStyle, upperRadius, lowerRadius);
            }
        }

        public double UpperRadius
        {
            get { return upperRadius; }
            set { upperRadius = value; }
        }

        public double LowerRadius
        {
            get { return lowerRadius; }
            set { lowerRadius = value; }
        }

        public int InstalStyle
        {
            get => instalStyle;
            set { instalStyle = value; }
        }
    }
}
