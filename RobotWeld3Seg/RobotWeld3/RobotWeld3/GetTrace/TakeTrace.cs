///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AppModel;
using RobotWeld3.Motions;

namespace RobotWeld3.GetTrace
{
    /// <summary>
    /// Chose a point and add it to the trace
    /// </summary>
    public class TakeTrace
    {
        private MainModel?_mainModel;
        private readonly MotionBus _mbus;

        //
        // NEW: -- methods
        //

        /// <summary>
        /// Constract and put the motionBus in.
        /// </summary>
        /// <param name="mbus"></param>
        public TakeTrace(MotionBus mbus)
        {
            _mbus = mbus;
        }

        /// <summary>
        /// Open hand wheel.
        /// </summary>
        internal void OpenHandwheel()
        {
            MotionBus.InitialHandwheel();
            _mbus.SetActionMethod(this);
            _mbus.RunHandWheel();
        }

        /// <summary>
        /// Stop HandWheel thread.
        /// </summary>
        internal void StopHandwheel()
        {
            HandOperate.SetHandThread(false);
        }

        /// <summary>
        /// Read XYZ from the motion card.
        /// </summary>
        /// <param name="ptos"></param>
        public void ReadXYZ(ref int[] ptos)
        {
            _mbus.ReadCoordinate(out ptos);
        }

        /// <summary>
        /// Display XYZ coordinate in the main Windows.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="a"></param>
        public void DisplayXYZ(int x, int y, int z, int a)
        {
            _mainModel = MainWindow.GetMainModel();
            _mainModel.SetXYZ(x, y, z, a);
        }

        //
        // END: -- new methods end
        //
    }
}
