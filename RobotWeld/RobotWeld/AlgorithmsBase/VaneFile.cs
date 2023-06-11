using RobotWeld.ViewModel;
using System;
using System.IO;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// Treat the file of Vane Wheel
    /// </summary>
    public class VaneFile
    {
        private int vType;
        private int vaneNumber;

        private double y1Position;
        private double y1Velocity;

        private double y2Position;
        private double y2Velocity;

        private double c1Position;
        private double c1Velocity;

        private double c2Position;
        private double c2Velocity;

        public VaneFile() { }

        [STAThread]
        public void SetViewParameter(int fileIndex, VaneWheelViewModel vvm)
        {
            string sFileName = "./Storage/" + fileIndex.ToString() + "van";
            String[] lines = File.ReadAllLines(sFileName);

            if (lines.Length < 11)
            {
                vType = Convert.ToInt32(lines[1]);
                vaneNumber = Convert.ToInt32(lines[2]);
                y1Position = Convert.ToDouble(lines[3]);
                y1Velocity = Convert.ToDouble(lines[4]);
                c1Position = Convert.ToDouble(lines[5]);
                c1Velocity = Convert.ToDouble(lines[6]);
                y2Position = Convert.ToDouble(lines[7]);
                y2Velocity = Convert.ToDouble(lines[8]);
                c2Position = Convert.ToDouble(lines[9]);
                c2Velocity = Convert.ToDouble(lines[10]);
            }
            else
            {
                vType = 0;
                vaneNumber = 0;
                y1Position = 0;
                y1Velocity = 0;
                c1Position = 0;
                c1Velocity = 0;
                y2Position = 0;
                y2Velocity = 0;
                c2Position = 0;
                c2Velocity = 0;
            }

            vvm.Y1Position = y1Position;
            vvm.Y1Velocity = y1Velocity;

            vvm.C1Position = c1Position;
            vvm.C1Velocity = c1Velocity;

            vvm.Y2Position = y2Position;
            vvm.Y2Velocity = y2Velocity;

            vvm.C2Position = c2Position;
            vvm.C2Velocity = c2Velocity;
        }

        public void CloseParameter(int fileIndex, VaneWheelViewModel vvm) 
        { 
        }
    }
}
