using System.Windows;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// The message box for error and warning
    /// </summary>
    public class Werr
    {
        public Werr() { }

        public void WerrMessage(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void WaringMessage(string msg)
        {
            MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void TestMessage(string msg)
        {
            MessageBox.Show(msg, "Test", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class GiveMsg
    {
        public GiveMsg() { }

        public static void Show(string msg)
        {
            MessageBox.Show(msg, "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Show(int msg)
        {
            MessageBox.Show(msg.ToString(), "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Show(double msg)
        {
            MessageBox.Show(msg.ToString(), "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class Promption
    {
        public Promption() { }

        public static void Prompt(string msg, int rtn)
        {
            if (rtn != 0)
            {
                string sMsg = msg + " " + rtn.ToString();
                MessageBox.Show(sMsg, "Msg", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }
    }
}
