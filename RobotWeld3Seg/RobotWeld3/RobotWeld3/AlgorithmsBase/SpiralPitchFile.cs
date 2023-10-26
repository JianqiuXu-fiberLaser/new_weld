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
    /// Read pitch of the spiral from the file
    /// </summary>
    public class SpiralPitchFile
    {
        public SpiralPitchFile() { }

        public double GetSpiralParameter(int typeIndex)
        {
            double pitch = 0;
            string rfname = "./Storage/" + typeIndex.ToString() + ".spl";
            if (File.Exists(rfname))
            {
                try
                {
                    using FileStream afile = new FileStream(rfname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(afile);
                    string? line = sr.ReadLine();
                    if (line != null)
                    {
                        pitch = Convert.ToDouble(line);
                    }
                    sr.Close();

                }
                catch
                {
                    Assertion.AssertError("螺距文件错误", 1);
                }
            }
            else
            {
                throw new Exception("新建螺距文件");
            }

            return pitch;
        }

        public void PutSpiralParameter(int typeIndex, double pitch)
        {
            string rfname = "./Storage/" + typeIndex.ToString() + ".spl";
            try
            {
                FileStream afile = new FileStream(rfname, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(afile);
                sw.WriteLine(pitch);
                sw.Close();
            }
            catch
            {
                Assertion.AssertError("螺距保存错误", 3);
            }
        }
    }
}
