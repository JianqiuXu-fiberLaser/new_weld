///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// treat the file about the maximum power
    /// </summary>
    public class MaxPowerFile
    {
        private const string mFile = "./Storage/maxpower.dat";

        public MaxPowerFile() { }

        [STAThread]
        public static int GetMaxPower()
        {
            try
            {
                FileStream afile = new(mFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new(afile);

                string? line = sr.ReadLine();

                sr.Close();
                if (line != null)
                    return Convert.ToInt32(line);
                else
                    return 0;
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 701);
                return 0;
            }
        }

        [STAThread]
        public static void PutMaxPower(int maxPower)
        {
            try
            {
                FileStream afile = new(mFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter sw = new(afile);

                sw.WriteLine(maxPower);
                sw.Close();
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 702);
                return;
            }
        }
    }
}
