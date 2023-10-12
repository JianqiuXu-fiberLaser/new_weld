using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using System.Collections.Generic;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Analysis the point's list and seperate them to working array
    /// </summary>
    public class AnalyzePoints
    {
        /// <summary>
        /// special case for two arcs, as in the Vane Wheel.
        /// </summary>
        public AnalyzePoints() { }

        /// <summary>
        /// Forming the seam and save the data to pots1 and post2
        /// </summary>
        public void FormingSeam(List<Point> points, List<int[]> pots1, List<int[]> pots2, List<LaserParameter> lplist)
        {
            pots1.Clear();
            pots2.Clear();
            lplist.Clear();

            int LeftCount = 0;

            if (points != null) 
            {
                // ignore the prepared situation in the first and last points
                for (int i = 1; i < points.Count - 1; i++)
                {
                    if (points[i].LaserState == 0)
                    {
                        LeftCount = i;
                        break;
                    }
                }

                LaserParameter varLp1 = new(2000);
                varLp1.LaserPower = points[1].LaserPointPower;
                lplist.Add(varLp1);

                LaserParameter varLp2 = new(2000);
                varLp2.LaserPower = points[LeftCount + 1].LaserPointPower;
                lplist.Add(varLp2);

                for (int i = 0; i < LeftCount; i++)
                {
                    int[] ptxyz = new int[3];
                    ToIntArray(points[i], ptxyz);
                    pots1.Add(ptxyz);
                }
                
                for (int i = LeftCount; i < points.Count; i++)
                {
                    int[] ptxyz = new int[3];
                    ToIntArray(points[i], ptxyz);
                    pots2.Add(ptxyz);
                }
            }            
        }

        //
        // convert Point to Int[3] array
        //
        private void ToIntArray(Point pts, int[] ptxyz)
        {
            ptxyz[0] = (int)pts.vector.X;
            ptxyz[1] = (int)pts.vector.Y;
            ptxyz[2] = (int)pts.vector.Z;
        }
    }
}
