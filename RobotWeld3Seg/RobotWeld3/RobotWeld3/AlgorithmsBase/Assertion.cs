///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
// 
// Ver. 3.0: (1) new static Assertion method
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.AlgorithmsBase
{
    public static class Assertion
    {
        /// <summary>
        /// Show error information to customer
        /// </summary>
        /// <param name="name"> the error name </param>
        /// <param name="rtn"> the error code of motion card </param>
        public static void AssertError(string name, short rtn)
        {
            string msg = string.Format("错误代码 No.{0},\n", rtn) + name;

            if (rtn != 0)
                Werr.WerrMessage(msg);
        }

        public static void AssertError(string name, int rtn)
        {
            string msg = string.Format("错误代码 No.{0},\n", rtn) + name;

            if (rtn != 0)
                Werr.WerrMessage(msg);
        }
    }
}
