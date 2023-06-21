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
        private bool clampDirection;

        public IntersectModel() { }

        public void GetParameter(WorkPackage workPackage)
        {
            this.workPackage = workPackage;
            BasicWorkFile bwf = new();
            bwf.GetIntersectParameter(workPackage, out int IntclampDirection, out upperRadius, out lowerRadius);

            if (IntclampDirection == 0)
            {
                clampDirection = false;
            }
            else if (IntclampDirection  == 1)
            {
                clampDirection = true;
            }
        }

        public void SetParameter(IntersectViewModel viewModel)
        {
            viewModel.ClampDirection = clampDirection;
            viewModel.UpperRadius = upperRadius;
            viewModel.LowerRadius = lowerRadius;
        }

        public void PutParameter(IntersectViewModel viewModel)
        {
            clampDirection = viewModel.ClampDirection;
            upperRadius = viewModel.UpperRadius;
            lowerRadius = viewModel.LowerRadius;

            if (workPackage != null)
            {
                int IntclampDirection;
                if (clampDirection)
                {
                    IntclampDirection = 1;
                }
                else
                {
                    IntclampDirection = 0;
                }

                BasicWorkFile bwf = new();
                bwf.SaveIntersectParameter(workPackage, IntclampDirection, upperRadius, lowerRadius);
            }
        }

        public void SetClampDirection(IntersectViewModel viewModel)
        {
            clampDirection = viewModel.ClampDirection;
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

        public bool ClampDirection
        {
            get => clampDirection;
            set { clampDirection = value; }
        }
    }
}
