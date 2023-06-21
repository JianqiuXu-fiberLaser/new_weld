using System;
using System.IO;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// treat the file about the maximum power
    /// </summary>
    public class MaxPowerFile
    {
        private const string mFile = "./Storage/" + "maxpower" + ".dat";

        public MaxPowerFile() { }

        [STAThread]
        public int GetMaxPower()
        {
            try
            {
                FileStream afile = new FileStream(mFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(afile);

                string? line = sr.ReadLine();

                if (line != null)
                {
                    return Convert.ToInt32(line);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1);
                return 0;
            }
        }

        [STAThread]
        public void PutMaxPower(int maxPower)
        {
            try
            {
                FileStream afile = new FileStream(mFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamWriter sw = new(afile);

                sw.WriteLine(maxPower);
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1);
                return;
            }
        }
    }
}
