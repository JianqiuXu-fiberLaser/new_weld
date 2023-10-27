using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using System.Collections.Generic;
using static System.Math;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Weld the trace of spiral
    /// </summary>
    public class SpiralOnTrace
    {
        private MotionOperate _mo;
        private WorkPackage? _workPackage;
        private SpiralCurveModel? spiralCurveModel;
        private List<WeldMoveSection>? _wms;

        private double _pitch;

        public SpiralOnTrace(MotionOperate mo)
        {
            this._mo = mo;
        }

        public void CopyHardware(WorkPackage wp, SpiralCurveModel scm)
        {

            _workPackage = wp;
            this.spiralCurveModel = scm;
            _pitch = scm.Pitch;
            PrepareData();

        }

        /// <summary>
        /// The standard weld process
        /// </summary>
        public void WorkProcess()
        {
            if (_wms == null) { return; }

            RunWeld4D runWeld4D = new(_mo);
            runWeld4D.CPutParameter(_wms);
        }

        //
        // ANALYSIS : the points data 
        //
        private void PrepareData()
        {
            if (_workPackage == null) return;

            PointListData points = _workPackage.GetPointList();
            // at least three points
            if (points.GetCount() < 3) return;

            int angle = SpiralSeam(points);
            AssembleSeam(points, angle, out List<int[]> ptlist);
            TransferToWms(ptlist, points);
        }

        //
        // Calculate stage seam to seperate the move section to XYZ and A axis.
        //
        private int SpiralSeam(in PointListData pts)
        {
            // we used only the second and third points to calculate the rotated angle.
            int[] xyz0 = pts.GetXYZ(1);
            int[] xyz1 = pts.GetXYZ(2);

            double distance = Sqrt(Pow(xyz1[0] - xyz0[0], 2) + Pow(xyz1[1] - xyz0[1], 2) + Pow(xyz1[2] - xyz0[2], 2));
            int totalAngle = (int)(MotionOperate.OneCycle * distance / (_pitch* MotionOperate.Xmillimeter));

            return totalAngle;
        }

        //
        // Assemble the point list to the real trace
        //
        private static void AssembleSeam(in PointListData pts, in int angle, out List<int[]> ptlist)
        {
            ptlist = new();

            int[] xyz0 = pts.GetXYZ(0);
            int[] xyz1 = pts.GetXYZ(1);
            int[] xyz2 = pts.GetXYZ(2);

            int angle0 = pts.GetAngle(0);
            int angle1 = pts.GetAngle(1);
            int angle2 = angle1 + angle;

            // total 4 points
            ptlist.Add(new int[] { xyz0[0], xyz0[1], xyz0[2], angle0 });
            ptlist.Add(new int[] { xyz1[0], xyz1[1], xyz1[2], angle1 });
            ptlist.Add(new int[] { xyz2[0], xyz2[1], xyz2[2], angle2 });
            ptlist.Add(new int[] { xyz0[0], xyz0[1], xyz0[2], angle0 });
        }

        //
        // Transfer point list to weld-move-section data
        //
        private void TransferToWms(in List<int[]> ptlist, in PointListData points)
        {
            _wms ??= new();

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

            // the spiral seam
            List<int[]> sectpts1 = ptlist.GetRange(1, 2);
            LaserWeldParameter lwp1 = points.GetLaserWeldParameter(1);
            List<double> speed2 = new(){ lwp1.WeldSpeed, lwp1.WeldSpeed };
            LaserArgument lag1 = new(lwp1);
            Dictionary<int, LaserArgument> args1 = new()
            {
                { 0, lag1 }
            };
            WeldMoveSection iwms1 = new(sectpts1, speed2)
            {
                Argument = args1,
                MoveType = 0,
            };
            _wms.Add(iwms1);

            // the back trace
            List<int[]> sectpts2 = ptlist.GetRange(2, 2);
            WeldMoveSection iwms2 = new(sectpts2, speed)
            {
                Argument = args,
                MoveType = 0,
            };
            _wms.Add(iwms2);
        }
    }
}
