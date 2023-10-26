///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: New clas to transfer wms to point array. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using System;
using System.Collections.Generic;
using static System.Math;

namespace RobotWeld3.CaluWMS
{
    /// <summary>
    /// Basic class to transfer wms to motion parameters
    /// </summary>
    internal class Transfer
    {
        internal Transfer() { }

        /// <summary>
        /// Transfer the position to work array, except the first wms (iNum == 0), 
        ///   the first is elimated.
        /// </summary>
        /// <param name="vc"> vector </param>
        /// <param name="iNum"> the number of wms </param>
        /// <returns> work array List(int[]) </returns>
        internal static List<int[]> ToPosition(List<Vector> vc, int iNum)
        {
            int start;
            if (iNum == 0) start = 0;
            else start = 1;

            // the vector list is at least two points.
            var pl = new List<int[]>();
            for (int i = start; i < vc.Count; i++)
            {
                int ax = (int)vc[i].X;
                int ay = (int)vc[i].Y;
                int az = (int)vc[i].Z;
                int aa = (int)vc[i].A;
                var vi = new int[4] { ax, ay, az, aa };
                pl.Add(vi);
            }

            return pl;
        }

        /// <summary>
        /// Transfer the speed to work speed, and the last is elimated. 
        /// it is equivalent to shift back by one sit.
        /// </summary>
        /// <param name="sp"> speed </param>
        /// <param name="iNum"> the number of wms. 0: the first;
        ///                     1: the middle wms; other: end wms.
        /// </param>
        /// <returns> work speed List(int[]) </returns>
        internal static List<double> ToSpeed(List<double> sp, int iNum)
        {
            var spl = new List<double>();

            //
            // because in the first wms, we need to open the air, then
            // the speed of last point is used with the speed of the first
            // point in the second wms.
            //
            // The second wms has one more speed to give the first wms.
            //
            if (iNum == 0)
                for (int i = 0; i < sp.Count; i++) spl.Add(sp[i]);
            else if (iNum == 1)
                for (int i = 1; i < sp.Count; i++) spl.Add(sp[i]);
            else
                for (int i = 1; i < sp.Count; i++) spl.Add(sp[i]);

            return spl;
        }

        /// <summary>
        /// Calculate a leading segment number to open air in the first wms.
        /// Using interpolating of the vector at the end of this wms.
        /// </summary>
        /// <param name="air"> time for air in </param>
        /// <param name="vc"> vector </param>
        /// <param name="sp"> speed </param>
        /// <returns> segment number, -1: not to operate air </returns>
        internal static int ToAirIn(double air, ref List<Vector> vc, ref List<double> sp)
        {
            var t = RunningTime(in vc, in sp);
            if (t < air) return 0;

            // normal procedure to open air, where the time is enough.
            (int sNum, double DeltaTime) = MoveTimeToLast(air, in vc, in sp);
            InterPolateSpeed(sNum, DeltaTime, ref vc, ref sp);

            // Because the first point will elimate in the filter process, the added
            // point is exactly at its sit.
            // Because the first wms, the first point is kept, the added point
            // is the next to its sit.
            return sNum + 1;
        }

        /// <summary>
        /// Calculate a reserved segment for laser fall.
        /// Automatically elimate the fall time when the weld trace is too short.
        /// </summary>
        /// <param name="fall"> fall time </param>
        /// <param name="vc"> vector </param>
        /// <param name="sp"> speed </param>
        /// <returns> segment number. -1 has not slow fall. </returns>
        internal static int ToLaserOff(double fall, ref List<Vector> vc, ref List<double> sp)
        {
            int lst;
            var t = RunningTime(in vc, in sp);

            // the laser too short or the fall to short, then the fall is ignored.
            if (t < 3 * fall || fall <= 10)
            {
                lst = -1;
            }
            else
            {
                (lst, var residualTime) = MoveTimeToLast(fall, in vc, in sp);

                // no new point insert.
                if (residualTime == 0)
                    lst--;
                else
                    InterPolateSpeed(lst, residualTime, ref vc, ref sp);
            }

            // because the first point will elimate in the filter process, so the add point
            //   is exactly at the sit.
            return lst;
        }

        /// <summary>
        /// Calculate laser parameters:
        /// (1) air in; (2) laser on >> power, rise; (3) change mode >> power,
        /// rise, frequency, duty cycle; (4) air out; (5) laser off >> fall;
        /// </summary>
        /// <param name="lpara0"> the previous wms parameter </param>
        /// <param name="lpara"> this wms parameter </param>
        /// <returns> laser state and laser parameter array </returns>
        internal static (int lst, double[] lp) ToLaserOn(LaserParameter lpara0, LaserParameter lpara, List<Vector> vc, List<double> sp)
        {
            int lst;
            double[] lp;
            if (lpara.Frequency == lpara0.Frequency && lpara.DutyCycle == lpara0.DutyCycle)
            {
                lst = (int)LaserAct.LaserOn;
                lp = new double[2];
            }
            else
            {
                lst = (int)LaserAct.ChangeMode;
                lp = new double[4];
                lp[2] = lpara.Frequency;
                lp[3] = lpara.DutyCycle;
            }

            lp[0] = lpara.Power;
            lp[1] = CheckRise(lpara.LaserRise, in vc, in sp);

            return (lst, lp);
        }

        /// <summary>
        /// if the first wms is lased, then make it lase.
        /// </summary>
        /// <param name="lpara"></param>
        /// <param name="vc"></param>
        /// <param name="sp"></param>
        /// <returns> (laser state and laser parameters </returns>
        internal static (int lst, double[] lp) ToLaserOnStart(LaserParameter lpara, List<Vector> vc, List<double> sp)
        {
            int lst;
            double[] lp;
            if (lpara.Frequency == 0 && lpara.DutyCycle == 0)
            {
                lst = (int)LaserAct.LaserOn;
                lp = new double[2];
            }
            else
            {
                lst = (int)LaserAct.ChangeMode;
                lp = new double[4];
                lp[2] = lpara.Frequency;
                lp[3] = lpara.DutyCycle;
            }

            lp[0] = lpara.Power;
            lp[1] = CheckRise(lpara.LaserRise, in vc, in sp);

            return (lst, lp);
        }

        /// <summary>
        /// Calculate time to close and open air in the middle wms section.
        /// When the wms has not enough delay time, keep open the air,
        /// i.e., time = -1. 
        /// </summary>
        /// <param name="aIn"> air out </param>
        /// <param name="aOut"> air in </param>
        /// <param name="vc"> vector of wms </param>
        /// <param name="sp"> speed of wms </param>
        /// <returns> air number and time; -1: not operate air </returns>
        internal static (int sin, double sout) ToAirInOut(double aIn, double aOut, ref List<Vector> vc, ref List<double> sp)
        {
            var t = RunningTime(in vc, in sp);
            if (t < aIn + aOut + 100) return (-1, -1);

            double sout = aOut;
            int sin;

            // normal procedure to open air, where the time is enough.
            (sin, double DeltaTime) = MoveTimeToLast(aIn, in vc, in sp);
            InterPolateSpeed(sin, DeltaTime, ref vc, ref sp);

            // because the first point will elimate in the filter process, so the add point
            //   is exactly at the sit.
            return (sin, sout);
        }

        /// <summary>
        /// calculate the residual time at the node of the original vector, speed,
        ///  and the segment just before the node.
        /// </summary>
        /// <param name="fall"></param>
        /// <param name="vc"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        private static (int sNum, double residualTime) MoveTimeToLast(double fall, in List<Vector> vc, in List<double> sp)
        {
            // Here vc.Count >= 2
            int sNum = vc.Count - 1;
            double residualTime = fall;

            // Here, the residual and cost time all recorded in unit of ms.
            // The speed is also in the unit of pulse/ms.
            double costTime = fall;
            while (residualTime > 0 && sNum > 0)
            {
                costTime = CrdInterpolation.DistanceTwoVector4D(vc[sNum - 1], vc[sNum]) / sp[sNum];
                residualTime -= costTime;
                sNum--;
            }

            // now, the residual is negative
            double DeltaTime = costTime + residualTime;

            return (sNum, DeltaTime);
        }

        /// <summary>
        /// Interpolate the vector between two vectors.
        /// </summary>
        /// <param name="sNum"></param>
        /// <param name="rt"> residual time </param>
        /// <param name="vc"></param>
        /// <param name="sp"></param>
        private static void InterPolateSpeed(int sNum, double rt, ref List<Vector> vc, ref List<double> sp)
        {
            // the distance farther over the start, it need not to consider further.
            if (sNum < 0 || rt == 0 || sNum >= vc.Count - 1) return;

            // because the move to the end with the speed of the next section, but the costTime is
            // calculated with the speed of the section, the time should be re-scaled with
            // different speed.
            var costTime = CrdInterpolation.DistanceTwoVector4D(vc[sNum], vc[sNum + 1]) / sp[sNum];
            double mst = Abs(costTime - Abs(rt));
            double rtdist = mst * sp[sNum];
            var vr = CrdInterpolation.CalculateResidualDistance(vc[sNum], vc[sNum + 1], rtdist);
            vc.Insert(sNum + 1, vr);
            sp.Insert(sNum + 1, sp[sNum]);
        }

        /// <summary>
        /// Too short weld to give rise.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="vc"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        private static double CheckRise(double r, in List<Vector> vc, in List<double> sp)
        {
            double realRise;
            var t = RunningTime(in vc, in sp);

            if (t > 3 * r) realRise = r;
            else realRise = 0;

            return realRise;
        }

        /// <summary>
        /// time to run over the whole wms.
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        private static double RunningTime(in List<Vector> vc, in List<double> sp)
        {
            double costTime = 0;
            for (int i = 0; i < vc.Count - 1; i++)
            {
                costTime += CrdInterpolation.DistanceTwoVector4D(vc[i], vc[i + 1]) / sp[i];
            }

            return costTime;
        }


        /// <summary>
        /// Sort the laser Section list ordered by the secNum. Correspondingly,
        /// the laser paremeter changes its sequence.
        /// When there are the same segment number, it is combinate them into one
        /// section in the way that the first number (tens bit) represent the 
        /// first laser section and the second number (single bit) represent
        /// the second laser section.
        /// For the corresponding lp (laser parameters), it is extend the int array.
        /// </summary>
        /// <param name="ls"> lasSec </param>
        /// <param name="lp"> laser parameters </param>
        internal static void SortSectionMarker(ref List<int[]> ls, ref List<double[]> lp)
        {
            // Sort the laser section with the exchanged method.
            bool itChanged = true;
            while (itChanged)
            {
                itChanged = false;
                for (int i = 0; i < ls.Count - 1; i++)
                {
                    if (ls[i][0] > ls[i + 1][0])
                    {
                        Swap(i, ref ls, ref lp);
                        itChanged = true;
                    }
                }
            }

            // Find laser sections which have the same section number.
            var rj = new List<int>();
            for (int i = 0; i < ls.Count - 1; i++)
            {
                if (ls[i][0] == ls[i + 1][0]) rj.Add(i);
            }

            // Merge the same laser section and elimited no-used laser section.
            // Because the removing operation will change the index in the laser
            // section, so it is need operate from the end of the laser section.
            for (int i = rj.Count - 1; i >= 0; i--)
            {
                var j = rj[i];
                // Replace lp;
                int cn = lp[j].Length + lp[j + 1].Length;
                var pj = lp[j].Length;
                var np = new double[cn];
                for (int k = 0; k < lp[j].Length; k++)
                    np[k] = lp[j][k];

                for (int l = 0; l < lp[j + 1].Length; l++)
                    np[pj + l] = lp[j + l][l];

                lp.RemoveAt(j);
                lp.Insert(j, np);
                lp.RemoveAt(j + 1);

                // Replace ls
                int ns = ls[j][1] * 10 + ls[j + 1][1];
                ls[j][1] = ns;
                ls.RemoveAt(j + 1);
            }
        }

        /// <summary>
        /// Swap the ith with (i+1)th element in the list of lasSec and lasPara.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ls"></param>
        /// <param name="lp"></param>
        private static void Swap(int i, ref List<int[]> ls, ref List<double[]> lp)
        {
            int[] ltemp = ls[i];
            double[] rtemp = lp[i];

            ls[i] = ls[i + 1];
            lp[i] = lp[i + 1];

            ls[i + 1] = ltemp;
            lp[i + 1] = rtemp;
        }

        /// <summary>
        /// Convert the coordinate to the hardware position.
        /// </summary>
        /// <param name="wmslist"></param>
        /// <returns> The list of position list </returns>
        internal static List<List<Vector>> HardwarePosition(List<WeldMoveSection> wmslist)
        {
            var vclist = new List<List<Vector>>();

            for (int j = 0; j < wmslist.Count; j++)
            {
                var vc = new List<Vector>();
                var p = wmslist[j].GetPosition();
                for (int i = 0; i < p.Count; i++)
                {
                    double ax = MotionSpecification.XDirection * p[i].X;
                    double ay = MotionSpecification.YDirection * p[i].Y;
                    double az = MotionSpecification.ZDirection * p[i].Z;
                    double aa = p[i].A;

                    vc.Add(new Vector(ax, ay, az, aa));
                }
                vclist.Add(vc);
            }

            return vclist;
        }

        /// <summary>
        /// Convert the coordinate speed to the hardware speed.
        /// Meamwhile, check the speed limit.
        /// </summary>
        /// <param name="wmslist"> wms list </param>
        /// <returns> the list of speed list </returns>
        internal static List<List<double>> HardwareSpeed(List<WeldMoveSection> wmslist)
        {
            var splist = new List<List<double>>();

            for (int j = 0; j < wmslist.Count; j++)
            {
                var sp = new List<double>();
                for (int i = 0; i < wmslist[j].Speed.Count; i++)
                {
                    // Todo: restrict the speed to given value. 166 ~ 10 m/min.
                    if (wmslist[j].Speed[i] < 167)
                        sp.Add(wmslist[j].Speed[i]);
                    else
                        sp.Add(166);
                }
                splist.Add(sp);
            }

            return splist;
        }
    }
}
