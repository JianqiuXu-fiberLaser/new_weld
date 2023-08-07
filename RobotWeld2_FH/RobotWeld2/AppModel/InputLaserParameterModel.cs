using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Windows;

namespace RobotWeld2.AppModel
{
    /// <summary>
    /// Application model for input the laser parameter.
    /// </summary>
    public class InputLaserParameterModel
    {
        private readonly WorkPackage _workPackage;
        private readonly int _ptindex;

        public InputLaserParameterModel(WorkPackage workPackage, int ptindex)
        {
            _workPackage = workPackage;
            _ptindex = ptindex;
        }

        public void SetLaserParameter(InputViewModel viewModel)
        {
            if (_workPackage == null) { return; }

            LaserWeldParameter lp;
            PointListData pts = _workPackage.GetPointList();

            if (_ptindex > pts.GetCount())
            {
                lp = new LaserWeldParameter();
            }
            else
            {
                lp = pts.GetLaserWeldParameter(_ptindex);
            }
  
            viewModel.LaserPower = lp.LaserPower;
            viewModel.Frequency = lp.Frequency;
            viewModel.LaserPulseWidth = lp.PulseWidth;
            viewModel.LaserDutyCycle = lp.DutyCycle;
            viewModel.LaserRise = (int)lp.LaserRise;
            viewModel.LaserFall = (int)lp.LaserFall;
            viewModel.LaserHoldtime = (int)lp.LaserHoldTime;
            viewModel.WireTime = (int)lp.WireTime;
            viewModel.AirIn = (int)lp.AirIn;
            viewModel.AirOut = (int)lp.AirOut;
            viewModel.WeldSpeed = lp.WeldSpeed;
            viewModel.WobbleSpeed = lp.WobbleSpeed;
            viewModel.LeapSpeed = lp.LeapSpeed;
        }

        public void PutLaserParameter(InputViewModel viewModel)
        {
            PointListData pts = _workPackage.GetPointList(); ;
            LaserWeldParameter oldlp = pts.GetLaserWeldParameter(_ptindex);

            if (oldlp.LaserPower != viewModel.LaserPower || oldlp.Frequency != viewModel.Frequency ||
                oldlp.DutyCycle != viewModel.LaserDutyCycle || oldlp.LaserRise != viewModel.LaserRise ||
                oldlp.LaserFall != viewModel.LaserFall || oldlp.LaserHoldTime != viewModel.LaserHoldtime ||
                oldlp.WireTime != viewModel.WireTime || oldlp.AirIn != viewModel.AirIn ||
                oldlp.AirOut != viewModel.AirOut || oldlp.WeldSpeed != viewModel.WeldSpeed ||
                oldlp.WobbleSpeed != viewModel.WobbleSpeed || oldlp.LeapSpeed != viewModel.LeapSpeed)
            {
                int paraindex = oldlp.ParaIndex + 1;
                LaserWeldParameter lp = new(paraindex)
                {
                    LaserPower = viewModel.LaserPower,
                    Frequency = viewModel.Frequency,
                    PulseWidth = viewModel.LaserPulseWidth,
                    DutyCycle = viewModel.LaserDutyCycle,
                    LaserRise = viewModel.LaserRise,
                    LaserFall = viewModel.LaserFall,
                    LaserHoldTime = viewModel.LaserHoldtime,
                    WireTime = viewModel.WireTime,
                    AirIn = viewModel.AirIn,
                    AirOut = viewModel.AirOut,
                    WeldSpeed = viewModel.WeldSpeed,
                    WobbleSpeed = viewModel.WobbleSpeed,
                    LeapSpeed = viewModel.LeapSpeed
                };

                pts.ModifyLaserWeldParameter(lp, _ptindex);
            }

        }
    }
}
