using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace RobotWeld2.Welding
{
    public class StageOnTrace
    {
        private readonly MotionOperate _mo;
        private WorkPackage? workPackage;
        private StageCurveModel? stageCurveModel;
        private List<WeldMoveSection>? _wms;

        public StageOnTrace(MotionOperate mo)
        {
            this._mo = mo;
        }

        public void CopyHardware(WorkPackage wp, StageCurveModel stm)
        {
            this.workPackage = wp;
            this.stageCurveModel = stm;
            PrepareData();
        }

        /// <summary>
        /// The standard weld process
        /// </summary>
        public void WorkProcess()
        {
            if (_wms == null) { return; }

            RunWeldHybrid runWeldhybrid = new(_mo);
            runWeldhybrid.PutParameter(_wms);
        }

        //
        // ANALYSIS : the points data 
        //
        private void PrepareData()
        {
            if (workPackage == null) return;

            PointListData points = workPackage.GetPointList();
            AssembleSeam(points, out List<int[]> ptlist, out List<int[]> arcList, out PointListData points2);
            TransferToWms(points2, ptlist, arcList);
        }

        //
        // Assemble the point list to the real trace
        // arcList: the start ptIndex and the end ptIndex of arc.
        // pts2: the newer (extened) pointListData
        //
        private static void AssembleSeam(in PointListData pts, out List<int[]> ptlist, out List<int[]> arcList,
            out PointListData pts2)
        {
            ptlist = new();
            arcList = new();
            pts2 = new(pts);

            int stptIndex = 0;
            int endptIndex = 0;
            int ptIndex = 0;

            // The start point.
            // It assume be a straight line. Moving and rotating directly to the start.
            int[] xyz0 = pts.GetXYZ(0);
            int angle = pts.GetAngle(0);
            int oldAngle = angle;
            ptlist.Add(new int[] { xyz0[0], xyz0[1], xyz0[2], angle });

            for (int i = 1; i < pts.GetCount(); i++)
            {
                angle = pts.GetAngle(i);

                if (angle == oldAngle)
                {
                    SameAngle(in pts, ref stptIndex, ref endptIndex, ref ptIndex, i, angle, ref ptlist, ref arcList);
                }
                else
                {
                    DifferentAngle(in pts, ref stptIndex, ref endptIndex, ref ptIndex, i, angle, ref oldAngle,
                        ref ptlist, ref arcList, ref pts2);
                }
            }
        }

        //
        // If the angel is not changed, keeping the xyz coordinate only,
        // but we need to record the arc nodes.
        //
        private static void SameAngle(in PointListData pts, ref int stptIndex, ref int endptIndex,
            ref int ptIndex, int i, int angle, ref List<int[]> ptlist, ref List<int[]> arcList)
        {
            if (pts.GetLineType(i) == 1 && pts.GetLineType(i - 1) == 0)
            {
                // start of arc
                stptIndex = ptIndex;
            }
            else if (pts.GetLineType(i) == 0 && pts.GetLineType(i - 1) == 1)
            {
                // end of arc
                endptIndex = ptIndex;
                int[] ptrange = new int[2] { stptIndex, endptIndex };
                arcList.Add(ptrange);
            }

            int[] xyz = pts.GetXYZ(i);
            ptIndex++;
            ptlist.Add(new int[] { xyz[0], xyz[1], xyz[2], angle });
        }

        //
        // If the plane trace is a straight line, moving XYZ at first, then rotating to
        // the new angle. 
        // If the plane trace is an arc, then moving all arcs before rotating.
        // In this case, it inclues the condition where the point is
        // the last point of arc.
        //
        private static void DifferentAngle(in PointListData pts, ref int stptIndex, ref int endptIndex,
            ref int ptIndex, int i, int angle, ref int oldAngle, ref List<int[]> ptlist, ref List<int[]> arcList,
            ref PointListData pts2)
        {
            if (pts.GetLineType(i) == 0 && pts.GetLineType(i - 1) == 0)
            {
                // straight line, one point splits to two same points,
                // but this two points in the ptlist are different.
                // One for moving, another for rotating.
                int[] xyz = pts.GetXYZ(i);

                ptIndex++;
                ptlist.Add(new int[] { xyz[0], xyz[1], xyz[2], oldAngle });

                ptIndex++;
                ptlist.Add(new int[] { xyz[0], xyz[1], xyz[2], angle });

                // Add the rotate point, and smooth the rotating.
                // Note: ptindex should add one at first before add the point.

                int lt = pts.GetPoint(i).LineType;
                Vector vc = pts.GetVector(i);
                int a = pts.GetAngle(i);
                LaserWeldParameter lwp = pts.GetLaserWeldParameter(i);
                double wp = 0.5 * lwp.WeldSpeed;
                double lp = 0.5 * lwp.LeapSpeed;
                Point addpoint = new(lt, vc, a, lwp);
                addpoint.SetSpeed(MotionOperate.SmoothCoefficient * wp, MotionOperate.SmoothCoefficient * lp);
                pts2.Insert(ptIndex, addpoint);

                oldAngle = angle;
            }
            else if (pts.GetLineType(i) == 0 && pts.GetLineType(i - 1) == 1)
            {
                // end of arc
                endptIndex = ptIndex;
                int[] ptrange = new int[2] { stptIndex, endptIndex };
                arcList.Add(ptrange);

                int[] xyz = pts.GetXYZ(i);
                angle = pts.GetAngle(i);

                ptlist.Add(new int[] { xyz[0], xyz[1], xyz[2], angle });
                oldAngle = angle;
                ptIndex++;
            }
            // the points all are arc's points.
            else
            {
                // start of arc, we record the start point index.
                if (pts.GetLineType(i - 1) == 0)
                {
                    stptIndex = ptIndex;
                }

                // or middle of arc
                int[] xyz = pts.GetXYZ(i);
                ptlist.Add(new int[] { xyz[0], xyz[1], xyz[2], oldAngle });
                ptIndex++;
            }
        }

        //
        // Transfer point list to weld-move-section data
        //
        private void TransferToWms(in PointListData points, in List<int[]> ptlist, in List<int[]> arcList)
        {
            _wms ??= new();

            StartWms(in points, in ptlist);
            // There are all straight lines.
            if (arcList.Count == 0)
            {
                AllLineSection(in points, in ptlist);
            }
            else
            {
                ArcSection(in points, in ptlist, in arcList);
            }

            EndWms(in points, in ptlist);
        }

        //
        // Start weld-move-section
        //
        //
        private void StartWms(in PointListData points, in List<int[]> ptlist)
        {
            if (_wms == null) return;

            // From preparation to the start point
            List<int[]> sectpts = ptlist.GetRange(0, 2);
            LaserWeldParameter lwp = points.GetLaserWeldParameter(0);
            List<double> speed = new() { lwp.LeapSpeed, lwp.LeapSpeed };
            LaserArgument lag = new(lwp);
            Dictionary<int, LaserArgument> args = new()
            {
                { 0, lag }
            };

            WeldMoveSection iwms = new(sectpts, speed)
            {
                Argument = args,
                MoveType = 0,
            };
            _wms.Add(iwms);
        }

        //
        // End weld-move-section
        //
        private void EndWms(in PointListData points, in List<int[]> ptlist)
        {
            if (_wms == null) return;

            // Back section
            int end = ptlist.Count - 1;
            LaserWeldParameter lwp = points.GetLaserWeldParameter(0);
            List<double> speed = new() { lwp.LeapSpeed, 0.5 * MotionOperate.SmoothCoefficient * lwp.LeapSpeed, lwp.LeapSpeed };
            LaserArgument lag = new(lwp);
            Dictionary<int, LaserArgument> args = new()
            {
                { 0, lag }
            };

            List<int[]> sectpts2 = new();
            int[] p = ptlist[end];
            sectpts2.Add(p);
            sectpts2.Add(new int[] { p[0], p[1], p[2], ptlist[0][3] });
            sectpts2.Add(new int[] { ptlist[0][0], ptlist[0][1], ptlist[0][2], ptlist[0][3] });
            WeldMoveSection iwms2 = new(sectpts2, speed)
            {
                Argument = args,
                MoveType = 0,
            };
            _wms.Add(iwms2);
        }

        // All section are lines. Only one main wms section, but for different laser arguments.
        // Plus two sections: the start and the end.
        //
        private void AllLineSection(in PointListData points, in List<int[]> ptlist)
        {
            if (_wms == null) return;

            // Main section
            int end = ptlist.Count - 1;
            List<int[]> sectpts0 = ptlist.GetRange(1, end);
            List<double> speed0 = DataAnalysis.FilterWms(points, out Dictionary<int, LaserArgument> args0, 1, end + 1);
            WeldMoveSection iwms1 = new(sectpts0, speed0)
            {
                Argument = args0,
                MoveType = 0,
            };

            _wms.Add(iwms1);
        }

        //
        // There are arc points
        //
        private void ArcSection(in PointListData points, in List<int[]> ptlist, in List<int[]> arcList)
        {
            if (_wms == null) return;

            // each arcList has two secton, one is straight lines which before the arcList,
            // another is arc which among the arcList
            for (int i = 0; i < arcList.Count; i++)
            {
                int ptIndex;
                // for the first arcList, the straight lines are from 1.
                if (i == 0) ptIndex = 1;
                // for the other arcList, the straight lines are from the end of i-1 arc.
                else ptIndex = arcList[i - 1][1];

                int end = arcList[i][0] + 1;
                List<int[]> linepts = ptlist.GetRange(ptIndex, end - ptIndex);
                List<double> speed0 = DataAnalysis.FilterWms(points, out Dictionary<int, LaserArgument> args0, ptIndex, end + 1);
                WeldMoveSection iwms0 = new(linepts, speed0)
                {
                    Argument = args0,
                    MoveType = 0,
                };

                _wms.Add(iwms0);

                int ptIndex2 = arcList[i][0];
                int end2 = arcList[i][1] + 1;
                List<int[]> arcpts = ptlist.GetRange(ptIndex2, end - ptIndex2);
                List<double> speed1 = DataAnalysis.FilterWms(points, out Dictionary<int, LaserArgument> args1, ptIndex2, end2 + 1);
                WeldMoveSection iwms1 = new(arcpts, speed1)
                {
                    Argument = args1,
                    MoveType = 1,
                };

                _wms.Add(iwms1);
            }
        }
    }
}
