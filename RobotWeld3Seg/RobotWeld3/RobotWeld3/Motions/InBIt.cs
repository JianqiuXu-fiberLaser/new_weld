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

namespace RobotWeld3.Motions
{
    /// <summary>
    /// Input port map of handwheel and IO ports
    /// </summary>
    public enum InBit
    {
        Aaxis = 21,    // BIT 21
        Xaxis = 22,    // BIT 22
        Yaxis = 24,    // BIT 24
        Zaxis = 26,    // BIT 26
        RX1 = 23,
        RX10 = 25,
        RX100 = 27,
        Bit12 = 12,
        Bit13 = 13,
        Bit14 = 14,
        Bit15 = 15,
        Reset = 0x7FFF_FFFF,    // reset all bits
    }
}
