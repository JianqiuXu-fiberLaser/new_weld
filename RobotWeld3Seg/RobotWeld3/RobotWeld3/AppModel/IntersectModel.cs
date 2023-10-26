///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) new laser weld parameter from the server or local
// Ver. 3.0: (1) add 2 xml file to store points' information and using
//               5 data structures to represent moving, laser and
//               material specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.Motions;
using RobotWeld3.ViewModel;
using System.Collections.Generic;

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// The model for intersect weld
    /// In this model, the data are included because only two data being used.
    /// </summary>
    public class IntersectModel
    {
        private double upperRadius;
        private double lowerRadius;
        private int instalStyle;

        public IntersectModel() { }

        internal void PutWorkPackage(WorkPackage wk)
        {
            var pmlist = wk.GetParameter();
            if (pmlist == null || pmlist.Count < 3) return;

            upperRadius = pmlist[0];
            lowerRadius = pmlist[1];
            instalStyle = (int)pmlist[2];
        }

        internal void SetIntersectParameter(IntersectViewModel _viewModel)
        {
            _viewModel.UpperRadius = upperRadius;
            _viewModel.LowerRadius = lowerRadius;
            _viewModel.InstalStyleValue = instalStyle;
        }

        public void FromViewModel(List<double> pml)
        {
            if (pml != null && pml.Count > 3)
            {
                upperRadius = pml[0];
                lowerRadius = pml[1];
                instalStyle = (int)pml[2];
            }
        }
    }
}
