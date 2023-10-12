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

using RobotWeld2.ViewModel;
using RobotWeld2.AppModel;
using System;
using System.IO;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Treat the file of Vane Wheel
    /// </summary>
    public class VaneFile
    {
        private readonly int _fileIndex;

        public VaneFile(DaemonModel dm)
        {
            _fileIndex = dm.GetTraceIndex();
        }

        /// <summary>
        /// write the data the disk
        /// </summary>
        /// <param name="vvm"> the data in the view model </param>
        [STAThread]
        public void Write(VaneWheelViewModel vvm)
        {
            string vanefile = "./Storage/" + _fileIndex.ToString() + ".van";
            FileStream wfile = new(vanefile, FileMode.Create);
            StreamWriter sw = new(wfile);

            sw.WriteLine("VaneIndex," + vvm.VaneIndex.ToString());
            sw.WriteLine("VaneNumber," + vvm.VaneNumber.ToString());

            sw.WriteLine("Y01Velocity," + vvm.Y01Velocity.ToString());
            sw.WriteLine("C01Velocity," + vvm.C01Velocity.ToString());
            sw.WriteLine("Y01Position," + vvm.Y01Position.ToString());
            sw.WriteLine("C01Position," + vvm.C01Position.ToString());
            sw.WriteLine("C02Position," + vvm.C02Position.ToString());
            sw.WriteLine("C03Position," + vvm.C03Position.ToString());
            sw.WriteLine("C04Position," + vvm.C04Position.ToString());
            sw.WriteLine("C05Position," + vvm.C05Position.ToString());
            sw.WriteLine("C06Position," + vvm.C06Position.ToString());
            sw.WriteLine("C07Position," + vvm.C07Position.ToString());

            sw.WriteLine("Y11Velocity," + vvm.Y11Velocity.ToString());
            sw.WriteLine("C11Velocity," + vvm.C11Velocity.ToString());
            sw.WriteLine("Y11Position," + vvm.Y11Position.ToString());
            sw.WriteLine("C11Position," + vvm.C11Position.ToString());
            sw.WriteLine("C12Position," + vvm.C12Position.ToString());
            sw.WriteLine("C13Position," + vvm.C13Position.ToString());
            sw.WriteLine("C14Position," + vvm.C14Position.ToString());
            sw.WriteLine("C15Position," + vvm.C15Position.ToString());
            sw.WriteLine("C16Position," + vvm.C16Position.ToString());
            sw.WriteLine("C17Position," + vvm.C17Position.ToString());

            sw.Close();
        }

        /// <summary>
        /// Get the data from disk
        /// </summary>
        /// <param name="vvm"> the data in the view modal </param>
        [STAThread]
        public void GetData(VaneWheelViewModel vvm)
        {
            string sFileName = "./Storage/" + _fileIndex.ToString() + ".van";
            if (!File.Exists(sFileName))
            {
                GiveMsg.Show("叶轮数据文件不存在");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(sFileName);

                if (lines.Length == 22)
                {
                    string[] var = new string[22];
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] varLine = lines[i].Split(",");
                        var[i] = varLine[1];
                    }

                    vvm.VaneIndex = Convert.ToInt32(var[0]);
                    vvm.VaneNumber = Convert.ToInt32(var[1]);

                    vvm.Y01Velocity = Convert.ToDouble(var[2]) / 500.0;
                    vvm.C01Velocity = Convert.ToDouble(var[3]) / 100.0;
                    vvm.Y01Position = Convert.ToDouble(var[4]) / 500.0;
                    vvm.C01Position = Convert.ToDouble(var[5]) / 100.0;
                    vvm.C02Position = Convert.ToDouble(var[6]) / 100.0;
                    vvm.C03Position = Convert.ToDouble(var[7]) / 100.0;
                    vvm.C04Position = Convert.ToDouble(var[8]) / 100.0;
                    vvm.C05Position = Convert.ToDouble(var[9]) / 100.0;
                    vvm.C06Position = Convert.ToDouble(var[10]) / 100.0;
                    vvm.C07Position = Convert.ToDouble(var[11]) / 100.0;

                    vvm.Y11Velocity = Convert.ToDouble(var[12]) / 500.0;
                    vvm.C11Velocity = Convert.ToDouble(var[13]) / 100.0;
                    vvm.Y11Position = Convert.ToDouble(var[14]) / 500.0;
                    vvm.C11Position = Convert.ToDouble(var[15]) / 100.0;
                    vvm.C12Position = Convert.ToDouble(var[16]) / 100.0;
                    vvm.C13Position = Convert.ToDouble(var[17]) / 100.0;
                    vvm.C14Position = Convert.ToDouble(var[18]) / 100.0;
                    vvm.C15Position = Convert.ToDouble(var[19]) / 100.0;
                    vvm.C16Position = Convert.ToDouble(var[20]) / 100.0;
                    vvm.C17Position = Convert.ToDouble(var[21]) / 100.0;
                }
                else
                {
                    throw new Exception("the trace file has an error.");
                }
            }
            catch (Exception ex) 
            {
                GiveMsg.Show(ex.Message);
                vvm.VaneIndex = 0;
                vvm.VaneNumber = 0;

                vvm.Y01Velocity = 0;
                vvm.C01Velocity = 0;
                vvm.Y01Position = 0;
                vvm.C01Position = 0;
                vvm.C02Position = 0;
                vvm.C03Position = 0;
                vvm.C04Position = 0;
                vvm.C05Position = 0;
                vvm.C06Position = 0;
                vvm.C07Position = 0;

                vvm.Y11Velocity = 0;
                vvm.C11Velocity = 0;
                vvm.Y11Position = 0;
                vvm.C11Position = 0;
                vvm.C12Position = 0;
                vvm.C13Position = 0;
                vvm.C14Position = 0;
                vvm.C15Position = 0;
                vvm.C16Position = 0;
                vvm.C17Position = 0;
            }
        }
    }
}
