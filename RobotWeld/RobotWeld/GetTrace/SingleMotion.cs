namespace RobotWeld.GetTrace
{
    /// <summary>
    /// the information to single step action
    /// </summary>
    public class SingleMotion
    {
        public enum EsingleMotion
        {
            NONE,
            RESET,
            GOTO_STEP
        };

        public EsingleMotion singlemotion;
    }
}
