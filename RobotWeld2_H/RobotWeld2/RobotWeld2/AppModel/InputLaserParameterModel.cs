using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotWeld2.AppModel
{
    /// <summary>
    /// Application model for input the laser parameter.
    /// </summary>
    public class InputLaserParameterModel
    {
        private WorkPackage workPackage;
        private int firstPt;

        public InputLaserParameterModel(WorkPackage workPackage)
        {
            this.workPackage = workPackage;
        }

        public void SetLaserParameter(InputViewModel viewModel)
        {
            if (workPackage == null) { return; }

            LaserWeldParameter lp;
            PointListData pts;
            pts = workPackage.GetPointList();

            int i = 0;
            do
            {
                lp = pts.GetPoint(i).Parameter;
                i++;
            }
            while ( i< pts.GetCount() && lp.LaserPower == 0);

            firstPt = i-1;
            viewModel.MaxPower = workPackage.GetMaxPower();

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
            viewModel.WeldSpeed = (int)lp.WeldSpeed;
            viewModel.WobbleSpeed = (int)lp.WobbleSpeed;
            viewModel.LeapSpeed = (int)lp.LeapSpeed;
        }

        public void PutLaserParameter(InputViewModel viewModel)
        {
            LaserWeldParameter lp;
            PointListData pts;
            if (workPackage != null)
            {
                pts = workPackage.GetPointList();
                lp = pts.GetPoint(firstPt).Parameter;
            }
            else
            {
                lp = new();
            }

            lp.LaserPower = viewModel.LaserPower;
            lp.Frequency = viewModel.Frequency;
            lp.PulseWidth = viewModel.LaserPulseWidth;
            lp.DutyCycle = viewModel.LaserDutyCycle;
            lp.LaserRise = viewModel.LaserRise;
            lp.LaserFall = viewModel.LaserFall;
            lp.LaserHoldTime = viewModel.LaserHoldtime;
            lp.WireTime = viewModel.WireTime;
            lp.AirIn = viewModel.AirIn;
            lp.AirOut = viewModel.AirOut;
            lp.WeldSpeed = viewModel.WeldSpeed;
            lp.WobbleSpeed = viewModel.WobbleSpeed;
            lp.LeapSpeed = viewModel.LeapSpeed;
        }
    }
}
