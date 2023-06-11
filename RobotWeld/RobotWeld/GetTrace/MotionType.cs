namespace RobotWeld.GetTrace
{
    public class MotionType
    {
        /// <summary>
        /// the action type for motion card
        /// </summary>
        public enum EmotionType
        {
            NO_MOTION = 0,
            RUN_TRACE,
            TAKE_TRACE,
            SINGLE_TRACE,
            EXIT_MOTION = -1
        };

        public EmotionType motiontype;
    }
}
