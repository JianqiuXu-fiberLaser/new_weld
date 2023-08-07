using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System.Collections.Generic;

namespace RobotWeld2.GetTrace
{
    /// <summary>
    /// Chose a point and add it to the trace
    /// </summary>
    public class TakeTrace
    {
        private MainModel? mainModel;
        private readonly MotionOperate _mo;

        //
        // NEW: -- methods
        //
        public TakeTrace(MotionOperate mo)
        {
            this._mo = mo;
        }

        public void OpenHandwheel(MainModel mainModel)
        {
            this.mainModel = mainModel;
            _mo.InitialHandwheel();
            _mo.SetActionMethod(this);
            _mo.RunHandWheel();
        }


        public void ReadXYZ(ref int[] ptos)
        {
            _mo?.ReadCoordinate(out ptos);
        }


        public void DisplayXYZ(int x, int y, int z, int a)
        {
            mainModel?.SetXYZ(x, y, z, a);
        }

        //
        // END: -- new methods end
        //
    }
}
