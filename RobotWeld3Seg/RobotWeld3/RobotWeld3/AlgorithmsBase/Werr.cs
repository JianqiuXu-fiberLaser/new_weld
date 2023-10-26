///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: new added class to display laser parameters.
// Ver. 3.0: Analysis method for Wms
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.CaluWMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// The message box for error and warning
    /// </summary>
    public static class Werr
    {
        public static void WerrMessage(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void WaringMessage(string msg)
        {
            MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void TestMessage(string msg)
        {
            MessageBox.Show(msg, "Test", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public static class GiveMsg
    {
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

        public static void Show(string msg, double d)
        {
            string smsg = string.Empty;
            smsg += msg + d.ToString();
            MessageBox.Show(smsg.ToString(), "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public static class Promption
    {
        public static void Prompt(string msg, int rtn)
        {
            if (rtn != 0)
            {
                string sMsg = msg + " " + rtn.ToString();
                MessageBox.Show(sMsg, "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    /// <summary>
    /// Write log file to the disk, various overloads.
    /// </summary>
    public static class WriteMsgFile
    {
        /// <summary>
        /// Write log file to disk
        /// </summary>
        /// <param name="slist"> data to be write </param>
        public static void WriteFile(List<WeldMoveSection> wms)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < wms.Count; i++)
                {
                    var w = wms[i];
                    sw.WriteLine("-- wms: No. " + w.WmsIndex.ToString() + ", Power = " + w.GetLaserPower().ToString());

                    var vc = w.GetPosition();
                    var spl = w.Speed;
                    if (spl.Count != vc.Count) throw new IOException();
                    for (int j = 0; j < vc.Count; j++)
                    {
                        sw.WriteLine(string.Format("{0:F3}", vc[j].X) + "," + string.Format("{0:F3}", vc[j].Y) + "," + string.Format("{0:F3}", vc[j].Z)
                            + "," + string.Format("{0:F3}", vc[j].A));
                        sw.WriteLine(string.Format("{0:F3}", spl[j]));
                    }
                }
            }
            catch (IOException)
            {
                Assertion.AssertError("位置与速度失配", 1012);
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1011);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk. List(int[])
        /// </summary>
        /// <param name="slist"> data to be write </param>
        public static void WriteFile(List<int[]> ptlist)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < ptlist.Count; i++)
                    sw.WriteLine($"{ptlist[i][0]},{ptlist[i][1]},{ptlist[i][2]},{ptlist[i][3]}");
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1021);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk. 
        /// position and speed: List(int[]) and List(double)
        /// </summary>
        /// <param name="slist"> data to be write </param>
        public static void WriteFile(List<int[]> ptlist, List<double> sp)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                if (sp.Count != ptlist.Count) throw new IOException();

                for (int i = 0; i < ptlist.Count; i++)
                {
                    sw.WriteLine($"{ptlist[i][0]},{ptlist[i][1]},{ptlist[i][2]},{ptlist[i][3]}");
                    sw.WriteLine(string.Format("{0:F3}", sp[i]));
                }
            }
            catch (IOException)
            {
                Assertion.AssertError("array is not match", 1013);
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1012);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk
        /// </summary>
        /// <param name="slist"> data to be write </param>
        public static void WriteFile(List<double> slist)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < slist.Count; i++)
                    sw.WriteLine(slist[i]);
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1020);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, List (double[]) 
        /// </summary>
        /// <param name="slist"> data to be write </param>
        public static void WriteFile(List<double[]> slist)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < slist.Count; i++)
                {
                    //sw.WriteLine("line: " + i.ToString());
                    //for (int j = 0; j < slist[i].Length; j++)
                    sw.WriteLine(slist[i][0].ToString() + "," + slist[i][1].ToString());
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1013);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk
        /// </summary>
        /// <param name="slist"> data to be write </param>
        public static void WriteFile(List<int> slist)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < slist.Count; i++)
                    sw.WriteLine(slist[i]);
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1014);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, for PointListData
        /// </summary>
        /// <param name="slist"> data to be write </param>
        internal static void WriteFile(PointListData ptl)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < ptl.Count; i++)
                {
                    var vc = ptl[i].Vector;
                    sw.WriteLine(vc.X.ToString() + "," + vc.Y.ToString() + "," + vc.Z.ToString());
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1019);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, for air Dictionary
        /// </summary>
        /// <param name="slist"> data to be write </param>
        internal static void WriteFile(Dictionary<int, int[]> aird)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                foreach (var kvp in aird)
                {
                    sw.WriteLine(kvp.Key + "," + kvp.Value[0] + "," + kvp.Value[1]);
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1015);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, for air Dictionary
        /// </summary>
        /// <param name="slist"> data to be write </param>
        internal static void WriteFile(Dictionary<int, int> falld)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                foreach (var kvp in falld)
                {
                    sw.WriteLine(kvp.Key + "," + kvp.Value);
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1016);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, for lasSec and laser parameter
        /// </summary>
        /// <param name="slist"> data to be write </param>
        internal static void WriteFile(List<int[]> lasSec, List<double[]> lp)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < lasSec.Count; i++)
                {
                    sw.WriteLine("las," + lasSec[i][0] + "," + lasSec[i][1]);
                    string lpst = string.Empty;
                    for (int j = 0; j < lp[i].Length; j++)
                    {
                        lpst += lp[i][j].ToString() + ",";
                    }
                    sw.WriteLine(lpst);
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1017);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, all moving parameter
        /// </summary>
        /// <param name="ptlist"> data to be write </param>
        /// <param name="splist"></param>
        /// <param name="lasSec"></param>
        /// <param name="lp"></param>
        internal static void WriteFile(List<int[]> ptlist, List<double> splist, List<int[]> lasSec, List<double[]> lp)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                int j = 0;
                for (int i = 0; i < ptlist.Count; i++)
                {
                    string astr = ptlist[i][0].ToString() + "," + ptlist[i][1].ToString() + "," + ptlist[i][2].ToString() + "," + ptlist[i][3].ToString() + ",";
                    astr += "[" + string.Format("{0:F3}", splist[i]) + "]";

                    if (j < lasSec.Count && i == lasSec[j][0])
                    {
                        // laser on/off state
                        astr += "[" + lasSec[j][1].ToString() + "][";

                        // laser parameter
                        if (lp[j].Length == 1)
                            astr += string.Format("{0:F2}", lp[j][0]) + "]";
                        else
                        {
                            for (int k = 0; k < lp[j].Length - 1; k++)
                                astr += string.Format("{0:F2}", lp[j][k]) + ",";
                            astr += string.Format("{0:F2}", lp[j][^1]) + "]";
                        }
                        j++;
                    }
                    sw.WriteLine(astr);
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1018);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, all moving parameter
        /// </summary>
        /// <param name="ptlist"> data to be write </param>
        /// <param name="splist"></param>
        /// <param name="lasSec"></param>
        /// <param name="lp"></param>
        internal static void WriteFile(List<List<Vector>> gps)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int j = 0; j < gps.Count; j++)
                {
                    sw.WriteLine($"--{j}--");
                    var vc = gps[j];
                    for (int i = 0; i < vc.Count; i++)
                    {
                        string astr = vc[i].X + "," + vc[i].Y + "," + vc[i].Z + "," + vc[i].A;
                        sw.WriteLine(astr);
                    }
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1019);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, all moving parameter
        /// </summary>
        /// <param name="ptlist"> data to be write </param>
        /// <param name="splist"></param>
        /// <param name="lasSec"></param>
        /// <param name="lp"></param>
        internal static void WriteFile(List<List<double>> sps)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int j = 0; j < sps.Count; j++)
                {
                    sw.WriteLine($"--{j}--");
                    var vc = sps[j];
                    for (int i = 0; i < vc.Count; i++)
                    {
                        string astr = vc[i].ToString();
                        sw.WriteLine(astr);
                    }
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1020);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, plane Vector list
        /// </summary>
        /// <param name="pvl"> data to be write </param>
        internal static void WriteFile(List<Vector> pvl)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int j = 0; j < pvl.Count; j++)
                {
                    string astr = string.Format("{0:F0}", pvl[j].X) + "," + string.Format("{0:F0}", pvl[j].Y + "," 
                        + string.Format("{0:F0}", pvl[j].Z) + "," + string.Format("{0:F0}", pvl[j].A));
                    sw.WriteLine(astr);
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1031);
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Write log file to disk, plane Vector list
        /// </summary>
        /// <param name="pvl"> data to be write </param>
        internal static void WriteFile(List<PlaneVector> pvl)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int j = 0; j < pvl.Count; j++)
                {
                    string astr = string.Format("{0:F0}", pvl[j].X) + "," + string.Format("{0:F0}", pvl[j].Y);
                    sw.WriteLine(astr);
                }
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 1032);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}
