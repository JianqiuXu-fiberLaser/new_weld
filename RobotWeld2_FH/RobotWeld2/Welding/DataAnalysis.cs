using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using System.Collections.Generic;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Basic function for data analysis
    /// </summary>
    public class DataAnalysis
    {
        public DataAnalysis() { }

        /// <summary>
        /// filter the same laser argument in the WMS section by the welding states.
        /// and put different laser speed for the WMS.
        /// </summary>
        /// <param name="pts"></param>
        /// <param name="args"> out laser arguments </param>
        /// <param name="start"></param>
        /// <param name="end"> the speed in the efficient laser argument </param>
        /// <returns></returns>
        public static List<double> FilterWms(in PointListData pts, out Dictionary<int, LaserArgument> args, int start, int end)
        {
            List<double> speed = new();
            args = new();
            LaserArgument oldarg = new();

            // add the newer laser argument to the dictionary
            for (int i = start; i < end; i++)
            {
                LaserWeldParameter lwp = pts.GetLaserWeldParameter(i);
                LaserArgument iarg = new(lwp);

                if (!iarg.WholeEqual(oldarg))
                {
                    args.Add(i - start, iarg);
                    oldarg = iarg;
                }

                // Search the laser state for the laser on/off in this section.
                // Every point has its speed.
                if (iarg.Power != 0)
                    speed.Add(pts.GetLaserWeldParameter(i).WeldSpeed);
                else
                    speed.Add(pts.GetLaserWeldParameter(i).LeapSpeed);
            }

            return speed;
        }
    }
}
