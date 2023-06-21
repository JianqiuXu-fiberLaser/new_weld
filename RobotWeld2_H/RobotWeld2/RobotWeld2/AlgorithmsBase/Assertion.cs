namespace RobotWeld2.AlgorithmsBase
{
    public class Assertion
    {
        public Assertion() { }

        /// <summary>
        /// Show error information to customer
        /// </summary>
        /// <param name="name"> the error name </param>
        /// <param name="rtn"> the error code of motion card </param>
        public static void AssertError(string name, short rtn)
        {
            string msg = string.Format("错误代码 No.{0},\n", rtn) + name;

            if (rtn != 0)
                new Werr().WerrMessage(msg);
        }

        public static void AssertError(string name, int rtn)
        {
            string msg = string.Format("错误代码 No.{0},\n", rtn) + name;

            if (rtn != 0)
                new Werr().WerrMessage(msg);
        }
    }
}
