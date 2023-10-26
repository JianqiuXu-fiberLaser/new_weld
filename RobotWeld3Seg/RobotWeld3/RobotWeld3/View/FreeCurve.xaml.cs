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
using RobotWeld3.AppModel;
using RobotWeld3.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace RobotWeld3.View
{
    /// <summary>
    /// FreeCurve.xaml 的交互逻辑
    /// </summary>
    public partial class FreeCurve : UserControl
    {
        private readonly FreeCurveViewModel viewModel;
        private readonly FreeCurveModel _model;

        public FreeCurve()
        {
            viewModel = new FreeCurveViewModel();
            _model = new FreeCurveModel();

            InitializeComponent();
            DataContext = viewModel;
        }

        /// <summary>
        /// Establist the plot of List, with new vector list.
        /// </summary>
        /// <param name="workPackage"></param>
        internal void EstablishList(List<DisPoint> ptl)
        {
            _model.PutVisualPoint(ptl);
            List<PathFigureCollection> vlp = _model.CreatePlot();
            FreeTraceData.Figures = vlp[0];
            FreeTraceLaserData.Figures = vlp[1];
        }
    }
}
