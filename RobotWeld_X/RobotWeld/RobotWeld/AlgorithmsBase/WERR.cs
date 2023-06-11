using System.Windows;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// The message box for error and warning
    /// </summary>
    public class Werr
    {
        public Werr() { }
        
        public void WerrMessage(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }

        public void WaringMessage(string msg)
        {
            MessageBox.Show(msg, "Warning", MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public void TestMessage(string msg)
        {
            MessageBox.Show(msg, "Test", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
