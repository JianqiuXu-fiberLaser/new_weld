using RobotWeld2.Motions;
using System.IO;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Globalization;

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

        public static void Show(string[] msg)
        {
            string smsg = string.Empty;
            for (int i = 0; i < msg.Length; i++)
            {
                smsg += msg[i] + "\n";
            }
            MessageBox.Show(smsg.ToString(), "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
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

    public class WriteMsgFile
    {
        public WriteMsgFile() { }

        public static void WriteFile(List<WeldMoveSection> wms)
        {
            string mFile = "./DebugMachine.log";
            var afile = new FileStream(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            var sw = new StreamWriter(afile);
            try
            {

                for (int i = 0; i < wms.Count; i++)
                {
                    sw.WriteLine("--" + i + "--");
                    List<int[]> c = wms[i].GetPosition();
                    List<double> d = wms[i].GetSpeedList();
                    for (int j = 0; j < wms[i].GetPointCount(); j++)
                    {
                        string istr = $"{c[j][0]},{c[j][1]},{c[j][2]},{c[j][3]}" + "," + $"{d[j]}";
                        sw.WriteLine(istr);
                    }

                    string dstr = string.Empty;
                    var lg = wms[i].Argument;
                    foreach (var id in lg)
                    {
                        dstr += id.Key + "," + id.Value.Power + "\n";
                    }
                    sw.WriteLine(dstr);
                }
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

        public static void WriteFile(List<int[]> ptlist)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {

                for (int i = 0; i < ptlist.Count; i++)
                {
                    sw.WriteLine($"{ptlist[i][0]},{ptlist[i][1]},{ptlist[i][2]},{ptlist[i][3]}");
                }
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

        public static void WriteFile(List<double> slist)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {

                for (int i = 0; i < slist.Count; i++)
                {
                    sw.WriteLine(slist[i]);
                }
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

        public static void WriteFile(List<int> slist)
        {
            string mFile = "./DebugMachine.log";
            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {

                for (int i = 0; i < slist.Count; i++)
                {
                    sw.WriteLine(slist[i]);
                }

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

        public static void WriteFile(List<List<int[]>> move)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < move.Count; i++)
                {
                    string str = string.Empty;
                    var pt = move[i];
                    for (int j = 0; j < pt.Count; j++)
                    {
                        str += pt[j][0] + "," + pt[j][1] + "," + pt[j][2] + "," + pt[j][3] + "\n";
                    }
                    sw.WriteLine(str);
                }
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

        public static void WriteFile(List<List<double>> splist)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < splist.Count; i++)
                {
                    string str = string.Empty;
                    var pt = splist[i];
                    for (int j = 0; j < pt.Count; j++)
                    {
                        str += pt[j] + "\n";
                    }
                    sw.WriteLine(str);
                }
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

        public static void WriteFile(List<int> lseg, List<int> lstate)
        {
            string mFile = "./DebugMachine.log";

            FileStream afile = new(mFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new(afile);
            try
            {
                for (int i = 0; i < lseg.Count; i++)
                {
                    string str = string.Empty;
                    str += lseg[i] + "," + lstate[i];
                    sw.WriteLine(str);
                }
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
    }
}
