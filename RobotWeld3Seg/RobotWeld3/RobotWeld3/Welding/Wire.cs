///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Simple code indicates the point's file
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RobotWeld3.Welding
{
    /// <summary>
    /// Wire specification data structure.
    /// </summary>
    public class Wire
    {
        private int _materialIndex;
        private double _diameter;

        public Wire(int mi = 0, double di = 0)
        {
            _materialIndex = mi;
            _diameter = di;
        }

        public int MaterialIndex
        {
            get => _materialIndex;
            set => _materialIndex = value;
        }

        public double Diameter
        {
            get => _diameter;
            set => _diameter = value;
        }
    }
}
