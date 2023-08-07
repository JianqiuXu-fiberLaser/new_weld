using System.Linq.Expressions;

namespace RobotWeld2.GetTrace
{
    /// <summary>
    /// The type of trac to run
    /// </summary>
    public enum Tracetype
    {
        NONE = 0,
        FREE_TRACE,
        VANE_WHEEL,
        INTERSECT,
        TOP_TRACE,
        STAGE_TRACE,
        SPIRAL
    }
}
