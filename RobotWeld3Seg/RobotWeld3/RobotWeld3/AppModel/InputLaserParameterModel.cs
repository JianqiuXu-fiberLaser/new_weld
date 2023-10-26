///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: revise the point list to match single workpackage file.
//           (1) using laserDisplayParameter for display on UI only.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.ViewModel;
using RobotWeld3.Welding;
using System.Collections.ObjectModel;

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// Application model for input the laser parameter.
    /// </summary>
    internal class InputLaserParameterModel
    {
        private PointListModel? _ptModel;
        private int _ptindex;

        internal InputLaserParameterModel() { }

        internal void PutPointModel(PointListModel ptm, int pd)
        {
            _ptModel = ptm;
            _ptindex = pd;
        }

        /// <summary>
        /// Set laer parameter to view model.
        /// </summary>
        /// <param name="viewModel"></param>
        internal void SetLaserParameter(InputViewModel viewModel)
        {
            if (_ptModel == null) return; 

            LaserDisplayParameter lp;
            ObservableCollection<DisPoint> pts = _ptModel.GetPointList();

            if (_ptindex >= pts.Count)
            {
                lp = new LaserDisplayParameter();
            }
            else
            {
                lp = pts[_ptindex].Parameter;
            }

            viewModel.LaserPower = lp.Power;
            viewModel.Frequency = lp.Frequency;
            viewModel.LaserDutyCycle = lp.Duty;
            viewModel.Speed = lp.Speed;
            viewModel.WobbleSpeed = lp.Wobble;
        }

        /// <summary>
        /// Put in the laser parameter from vier model.
        /// </summary>
        /// <param name="viewModel"></param>
        internal void PutLaserParameter(InputViewModel viewModel)
        {
            if (_ptModel == null) return;

            var pts = _ptModel.GetPointList();
            var oldlp = pts[_ptindex].Parameter;

            if (oldlp.Power != viewModel.LaserPower || oldlp.Frequency != viewModel.Frequency ||
                oldlp.Duty != viewModel.LaserDutyCycle || oldlp.Speed != viewModel.Speed ||
                oldlp.Wobble != viewModel.WobbleSpeed)
            {
                LaserDisplayParameter lp = new()
                {
                    Power = viewModel.LaserPower,
                    Frequency = viewModel.Frequency,
                    Duty = viewModel.LaserDutyCycle,
                    Speed = viewModel.Speed,
                    Wobble = viewModel.WobbleSpeed,
                };

                pts[_ptindex].ModifyLaser(lp);
            }
        }
    }
}
