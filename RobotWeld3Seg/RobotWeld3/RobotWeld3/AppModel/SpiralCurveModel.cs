///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: inherit interface of ITraceParameter.
// Ver. 3.0: Original function to pass parameter.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.Motions;
using RobotWeld3.ViewModel;

namespace RobotWeld3.AppModel
{
    public class SpiralCurveModel
    {
        private double _pitch;

        public SpiralCurveModel() { }

        /// <summary>
        /// Put the pitch to the model
        /// </summary>
        /// <param name="wk"></param>
        internal void PutWorkPackage(WorkPackage wk)
        {
            var pml = wk.GetParameter();
            if (pml != null && pml.Count > 0) _pitch = pml[0];
        }

        /// <summary>
        /// Get pitch from view model
        /// </summary>
        /// <param name="pitich"></param>
        public void FromViewModel(double pitch)
        {
            _pitch = pitch;
        }

        /// <summary>
        /// Set pitch to view model
        /// </summary>
        /// <param name="vm"></param>
        public void SetPitch(SpiralViewModel vm)
        {
            vm.Pitch = _pitch;
        }
    }
}
