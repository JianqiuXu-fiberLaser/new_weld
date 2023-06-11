namespace RobotWeld.GetTrace
{
    /// <summary>
    /// The type of trace
    /// </summary>
    public class TraceType
    {
        public enum EtraceType
        {
            NONE = 0,
            INPUT,
            VANE_WHEEL,
            INTERSECT,
            TOP_TRACE,
            STAGE_TRACE,
            SPIRAL
        };

        public EtraceType tracetype;
    }
}
