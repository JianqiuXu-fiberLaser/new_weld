///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using RobotWeld3.ViewModel;
using RobotWeld3.Welding;
using System.IO;

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// Model for SuperUser page.
    /// </summary>
    internal class SuperUserModel
    {
        private readonly SuperUserViewModel? _viewModel;
        private MotionBus? _mbus;
        private int _timestamp;

        public SuperUserModel(SuperUserViewModel vm)
        {
            _viewModel = vm;
        }

        internal void PutMotionBus(MotionBus ms)
        {
            _mbus = ms;
        }

        /// <summary>
        /// Read coefficient from disk
        /// </summary>
        internal void ReadCoefficient(int timestamp)
        {
            _timestamp = timestamp;

            string fname = "./Storage/" + _timestamp.ToString() + ".wms";
            string dname = "./Worktime/" + "default.pam";

            var coeff = new LaserCoefficient();
            if (File.Exists(fname))
                AccessParameterFile.Read(fname, ref coeff);
            else
                AccessParameterFile.Read(dname, ref coeff);

            if (_viewModel == null) return;

            _viewModel.ProtectAir = coeff.AirCoef;
            _viewModel.RiseEdge = coeff.RiseCoef;
            _viewModel.FallEdge = coeff.FallCoef;
            _viewModel.BackLength = coeff.Wireback;
            _viewModel.FeedSpeed = coeff.WireSpeed;
            _viewModel.BackSpeed = coeff.WithdrawSpeed;
            _viewModel.RefeedLength = coeff.RefeedLength;
        }

        /// <summary>
        /// Save laser coeffcients.
        /// </summary>
        internal void SaveCoefficient()
        {
            if (_viewModel == null) return;
            string fname = "./Storage/" + _timestamp.ToString() + ".wms";

            var coeff = new LaserCoefficient()
            {
                AirCoef = _viewModel.ProtectAir,
                RiseCoef = _viewModel.RiseEdge,
                FallCoef = _viewModel.FallEdge,
                Wireback = _viewModel.BackLength,
                WireSpeed = _viewModel.FeedSpeed,
                WithdrawSpeed = _viewModel.BackSpeed,
                RefeedLength = _viewModel.RefeedLength,
            };

            AccessParameterFile.Save(fname, ref coeff);
        }

        /// <summary>
        /// Feed wire in when mouse left button be pressed down
        /// </summary>
        internal void FeedIn()
        {
            _mbus?.FeedIn();
        }

        /// <summary>
        /// Stop feed wire when the mouse left button be released.
        /// </summary>
        internal void FeedStop() 
        {
            _mbus?.FeedStop();
        }

        /// <summary>
        /// Withdraw wire in when mouse left button be pressed down
        /// </summary>
        internal void BackIn()
        {
            _mbus?.Withdraw();
        }

        /// <summary>
        /// Stop withdraw wire when the mouse left button be released.
        /// </summary>
        internal void BackStop() 
        {
            _mbus?.WithdrawStop();
        }
    }
}
