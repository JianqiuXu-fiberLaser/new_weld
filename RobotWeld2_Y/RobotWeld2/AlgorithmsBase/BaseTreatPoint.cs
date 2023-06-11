using RobotWeld2.GetTrace;
using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.IO;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Basic function to save and load the point file
    /// </summary>
    public class BaseTreatPoint
    {

        private readonly DaemonFile dmf;
        public BaseTreatPoint(DaemonFile dmf)
        {
            this.dmf = dmf;
        }

        public void GetPoints(List<Point> points)
        {
            // only the old file to be read
            try
            {
                string rfilename = "./Storage/" + dmf.TraceIndex.ToString() + ".trc";
                if (File.Exists(rfilename))
                {
                    string[] lines = File.ReadAllLines(rfilename);
                    if (lines.Length > 1)
                        StringParse(lines, points);
                }
                else
                {
                    Point point = new();
                }
            }
            catch (Exception ex)
            {
                new Werr().WerrMessage(ex.ToString());
            }
        }

        public void SavePoints(List<Point> points)
        {
            StreamWriter sw = new(new FileStream("./Storage/" + dmf.TraceIndex.ToString() +
                ".trc", FileMode.Create));

            sw.WriteLine("code," + dmf.TraceCode.ToString());

            if (points != null)
            {
                int ptNumber = points.Count;
                for (short i = 0; i < ptNumber; i++)
                {
                    sw.WriteLine(points[i].ToString());
                }
            }

            sw.Close();
        }
        //
        // analysis the string for the point
        //
        private void StringParse(string[] strs, List<Point> pts)
        {
            for (short i = 1; i < strs.Length; i++)
            {
                string[] var = strs[i].Split(',');
                if (var.Length == 6)
                {
                    double x = Convert.ToDouble(var[0]);
                    double y = Convert.ToDouble(var[1]);
                    double z = Convert.ToDouble(var[2]);
                    Vector vector = new(x, y, z);

                    int lt = Convert.ToInt32(var[3]);
                    int ls = Convert.ToInt32(var[4]);
                    int pp = Convert.ToInt32(var[5]);
                    Point point = new(lt, ls, pp, vector);

                    pts.Add(point);
                }
            }
        }
    }
}
