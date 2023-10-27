using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static GC.Frame.Motion.Privt.CNMCLib20;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Dirct write and read to motion card, but wrapped with lock function
    ///  to avoid conflection on the motion card.
    /// </summary>
    public class MotionCard
    {    }
}
