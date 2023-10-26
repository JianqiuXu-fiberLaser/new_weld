///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver.3.0: Base class for enum Tracetype.
//
///////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// Base Class for enum of Laser actions
    /// </summary>
    internal class CLaserAct
    {
        public CLaserAct() { }
    }

    /// <summary>
    /// _lasSec: the segment number that need a laser actions. 
    ///          (1) air in; (2) laser on >> power, rise; (3) change mode >> power,
    ///          rise, frequency, duty cycle; (4) air out; (5) laser off >> fall;
    /// _lp: laser parameter, depending on the value of _lasSec.
    /// </summary>
    public enum LaserAct
    {
        AirIn = 1,
        LaserOn = 2,
        ChangeMode = 3,
        AirOut = 4,
        LaserOff = 5,
    }
}
