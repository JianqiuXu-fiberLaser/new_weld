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
    /// Read and write intersect file
    /// </summary>
    public class IntersectFile
    {
        public IntersectFile() { }

        public void GetParameter(int typeIndex, out int instalStyle, out double upperRadius, out double lowerRadius)
        {
            instalStyle = 0;
            upperRadius = 0;
            lowerRadius = 0;
            // only the old file to be read
            string rfname = "./Storage/" + typeIndex.ToString() + ".int";
            try
            {
                using FileStream afile = new FileStream(rfname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(afile);
                if (File.Exists(rfname))
                {
                    string? line = sr.ReadLine();
                    if (line != null)
                    {
                        instalStyle = Convert.ToInt32(line);
                    }

                    line = sr.ReadLine();
                    if (line != null)
                    {
                        upperRadius = Convert.ToDouble(line);
                    }

                    line = sr.ReadLine();
                    if (line != null)
                    {
                        lowerRadius = Convert.ToDouble(line);
                    }
                    sr.Close();
                }
                else
                {
                    throw new Exception("半径文件不存在");
                }
            }
            catch
            {
                Assertion.AssertError("半径文件错误", 1);
            }
        }

        public void PutParameter(int typeIndex, int clampDirection, double upperRadius, double lowerRadius)
        {
            string rfname = "./Storage/" + typeIndex.ToString() + ".int";
            try
            {
                FileStream afile = new FileStream(rfname, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(afile);
                sw.WriteLine(clampDirection);
                sw.WriteLine(upperRadius);
                sw.WriteLine(lowerRadius);
                sw.Close();
            }
            catch
            {
                Assertion.AssertError("半径文件保存错误", 1);
            }
        }
    }
}
