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

using RobotWeld3.AlgorithmsBase;
using System.Collections.Generic;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Basic class template for WMS resolves of various traces.
    /// </summary>
    internal abstract class BasicWms
    {
        public PointListData? _ptList;
        public List<double>? _pml; 

        public BasicWms() { }

        public void PutParameter(PointListData ptList, List<double> pml)
        {
            _ptList = ptList;
            _pml = pml;
        }

        public abstract void BackWms(int icode, int timeStamp, ref List<WeldMoveSection> wms);
    }
}
