///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.IO;

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

        public static void Show(string msg, double db)
        {
            MessageBox.Show(msg + db.ToString(), "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class Promption
    {
        public Promption() { }

        public static void Prompt(string msg, int rtn)
        {
            if (rtn != 0)
                MessageBox.Show(msg, "Msg", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool Prompt(string msg)
        {
            var ret = MessageBox.Show(msg, "Note", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (ret == MessageBoxResult.OK)
                return true;
            else
                return false;
        }
    }

    internal class WriteLog
    {
        public WriteLog() { }

        internal static void WrtLog(List<int[]> li)
        {
            var sw = new StreamWriter(".\\rlog.txt");

            try
            {
                for (int i = 0; i < li.Count; i++)
                {
                    sw.WriteLine(li[i][0].ToString() + "," + li[i][1].ToString() + "," + li[i][2].ToString());
                }
            }
            catch(Exception ex) 
            {
                new Werr().WaringMessage(ex.Message);
            }
            finally
            {
                sw.Close();
            }
        }

        internal static void WrtLog(List<int> li)
        {
            var sw = new StreamWriter(".\\rlog.txt");

            try
            {
                for (int i = 0; i < li.Count; i++)
                {
                    sw.WriteLine(li[i].ToString());
                }
            }
            catch (Exception ex)
            {
                new Werr().WaringMessage(ex.Message);
            }
            finally
            {
                sw.Close();
            }
        }
    }
}
