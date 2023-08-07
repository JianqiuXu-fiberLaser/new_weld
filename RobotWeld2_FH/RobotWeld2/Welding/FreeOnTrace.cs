using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace RobotWeld2.Welding
{
    public class FreeOnTrace
    {
        private MotionOperate _mo;
        private WorkPackage? _workPackage;
        private FreeCurveModel? freeCurveModel;
        private List<WeldMoveSection>? _wms;

        public FreeOnTrace(MotionOperate mo)
        {
            this._mo = mo;
        }

        public void CopyHardware(WorkPackage wp, FreeCurveModel fcm)
        {
            _workPackage = wp;
            this.freeCurveModel = fcm;
            PrepareData();
        }

        /// <summary>
        /// The standard weld process
        /// </summary>
        public void WorkProcess()
        {
            if (_wms == null) { return; }

            RunWeldXYZ runWeldXYZ = new(_mo);
            runWeldXYZ.PutParameter(_wms);
        }

        //
        // ANALYSIS : the points data 
        //
        private void PrepareData()
        {
            if (_workPackage == null) return;

            PointListData points = _workPackage.GetPointList();
            AssembleSeam(points, out List<int[]> ptlist);
            TransferToWms(ptlist, points);
        }

        //
        // Assemble the point data list to the real trace data array
        //
        private static void AssembleSeam(in PointListData pts, out List<int[]> ptlist)
        {
            ptlist = new();

            for (int i = 0; i < pts.GetCount(); i++)
            {
                int[] xyz = pts.GetXYZ(i);
                int angle = pts.GetAngle(i);
                int[] xyza = new int[4] { xyz[0], xyz[1], xyz[2], angle };
                ptlist.Add(xyza);
            }
        }

        //
        // Transfer point array list to weld-move-section data
        //
        private void TransferToWms(in List<int[]> ptlist, in PointListData pts)
        {
            _wms ??= new();

            StartWms(in pts, in ptlist);

            List<int[]> sectData = new();
            FindArcSection(in pts, ptlist.Count, sectData);
            SetLineSection(ptlist.Count, sectData);
            RecordSection(in ptlist, in pts, in sectData);

            EndWms(in pts, in ptlist);
        }

        private static void FindArcSection(in PointListData pts, int count, List<int[]> sd)
        {
            int i = 1;
            while (i < count)
            {
                if (pts.GetLineType(i) == 1)
                {
                    int start = i;
                    int j = i;
                    int lnum = 1;

                    // Find all arc points in this section
                    int sectLineType;
                    if (j < count - 1)
                    {
                        // the points is not the last one.
                        sectLineType = pts.GetLineType(j++);
                        while (sectLineType == 1)
                        {
                            lnum++;
                            i++;
                            if (j >= count - 1) break;
                            sectLineType = pts.GetLineType(j++);
                        }
                    }

                    // a good arc section, the number of points should be odd.
                    if (lnum % 2 == 0)
                    {
                        lnum--;
                        i++;    // this points should be step over
                    }

                    // for arc section, only arc points are kept.
                    int end = start + lnum;
                    sd.Add(new int[] { start, end, 1 });
                }
                i++;
            }
        }

        private static void SetLineSection(int count, List<int[]> sd)
        {
            int length = sd.Count;
            if (length == 0)
            {
                // there are no arc section
                int start = 1;
                int end = count;
                sd.Add(new int[] { start, end, 0 });
                return;
            }
            else if (length > 1)
            {
                // fill the space between middle sections,
                // when sections number larger than 2.
                for (int i = 0; i < length - 1; i++)
                {
                    int start = sd[i][1];
                    int end = sd[i + 1][0];
                    sd.Insert(i, new int[] { start - 1, end + 1, 0 });
                }
            }

            // the first section is line section
            if (sd[0][0] != 1)
            {
                int start = 1;
                int end = sd[0][0];
                sd.Insert(0, new int[] { start, end + 1, 0 });
            }

            length = sd.Count;
            // the last section is line section
            if (sd[length - 1][1] != count)
            {
                int start = sd[length - 1][1];
                int end = count;
                sd.Add(new int[] { start - 1, end, 0 });
            }
        }

        private void RecordSection(in List<int[]> ptlist, in PointListData pts, in List<int[]> sd)
        {
            if (_wms == null) return;

            for (int i = 0; i < sd.Count; i++)
            {
                int start = sd[i][0];
                int end = sd[i][1];
                int lt = sd[i][2];

                //GiveMsg.Show(start);
                //GiveMsg.Show(end);

                List<int[]> sectPts = ptlist.GetRange(start, end - start);
                List<double> speed = DataAnalysis.FilterWms(pts, out Dictionary<int, LaserArgument> args, start, end);
                WeldMoveSection iwms = new(sectPts, speed)
                {
                    MoveType = lt,
                    Argument = args,
                };

                _wms.Add(iwms);
            }
        }

        //
        // Start weld-move-section
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
            List<double> speed = new() { lwp.LeapSpeed, lwp.LeapSpeed };
            LaserArgument lag = new(lwp);
            Dictionary<int, LaserArgument> args = new()
            {
                { 0, lag }
            };

            List<int[]> sectpts2 = new()
            {
                ptlist[end],
                new int[] { ptlist[0][0], ptlist[0][1], ptlist[0][2], ptlist[0][3] }
            };
            WeldMoveSection iwms2 = new(sectpts2, speed)
            {
                Argument = args,
                MoveType = 0,
            };
            _wms.Add(iwms2);
        }
    }
}
