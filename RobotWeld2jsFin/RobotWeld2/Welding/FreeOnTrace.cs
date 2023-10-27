using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Motions;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RobotWeld2.Welding
{
    public class FreeOnTrace
    {
        private MotionOperate _mo;
        private WorkPackage? _workPackage;
        private FreeCurveModel? freeCurveModel;

        private List<List<int[]>>? _moveSeg;
        private List<List<double>>? _splist;
        private List<int>? _type;
        private List<int>? _laserSeg;
        private List<int>? _laserState;
        private List<LaserArgument>? _laserArgs;

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
            if (_moveSeg == null || _type == null || _laserSeg == null
                || _laserState == null || _laserArgs == null || _splist == null)
                return;

            RunWeldXYZ runWeldXYZ = new(_mo);
            runWeldXYZ.PutParameter(_moveSeg, _type, _splist, _laserSeg, _laserState, _laserArgs);
        }

        //
        // ANALYSIS : the points data 
        //
        private void PrepareData()
        {
            if (_workPackage == null) return;

            PointListData points = FilterSamePoint(_workPackage.GetPointList());
            AssembleSeam(points, out List<int[]> ptlist);
            AssembleMoveSegment(ptlist, in points);
            MarkLaserSegment(in points);
            CheckWobble(points);

/*            if (_laserState != null && _laserSeg != null)
                WriteMsgFile.WriteFile(_laserSeg, _laserState);*/

            //if (_moveSeg != null) WriteMsgFile.WriteFile(_moveSeg);
        }

        //
        // Assemble the point data list to the real trace data array
        // ignore the repeated point.
        //
        private PointListData FilterSamePoint(PointListData ptt)
        {
            var point = new PointListData();

            int[] oldxyz = ptt.GetXYZ(0);
            int oldangle = ptt.GetAngle(0);

            for (int i = 1; i < ptt.GetCount(); i++)
            {
                int[] xyz = ptt.GetXYZ(i);
                int angle = ptt.GetAngle(i);

                if (xyz[0] != oldxyz[0] || xyz[1] != oldxyz[1] || xyz[2] != oldxyz[2] || angle != oldangle)
                {
                    point.Add(ptt.GetPoint(i - 1));
                }
                oldxyz = xyz;
            }

            // the last point
            int ct = ptt.GetCount();
            point.Add(ptt.GetPoint(ct - 1));
            return point;
        }

        //
        // assemble to bare data which related the mechanic specification.
        //
        private static void AssembleSeam(in PointListData pts, out List<int[]> ptlist)
        {
            ptlist = new();

            for (int i = 0; i < pts.GetCount(); i++)
            {
                int[] xyz = pts.GetXYZ(i);
                int angle = pts.GetAngle(i);
                int[] xyza = new int[4] { MotionSpecification.XDirection * xyz[0],
                    MotionSpecification.YDirection * xyz[1],
                    MotionSpecification.ZDirection * xyz[2], angle };

                ptlist.Add(xyza);
            }
        }

        //
        // find and abstract moving segments
        //
        private void AssembleMoveSegment(in List<int[]> ptlist, in PointListData pts)
        {
            _moveSeg = new List<List<int[]>>();
            _splist = new List<List<double>>();
            _type = new List<int>();

            int ct = ptlist.Count;
            int ipt = 0;
            while (ipt < ct)
            {
                int vstep;
                if (pts.GetLineType(ipt) == 0)
                {
                    List<int[]> lineseg = LinePointGroup(ipt, ct, ptlist, pts);
                    _moveSeg.Add(lineseg);
                    _type.Add(0);
                    vstep = lineseg.Count;
                }
                else
                {
                    List<int[]> Arcseg = ArcPointGroup(ipt, ct, ptlist, pts);
                    _moveSeg.Add(Arcseg);
                    _type.Add(1);
                    vstep = Arcseg.Count;
                }

                ipt += vstep;
            }
        }

        //
        // Check and set wobble frquency
        //
        private void CheckWobble(in PointListData pts)
        {
            int ct = pts.GetCount();
            for (int i = 0; i < ct; i++)
            {
                var wob = pts.GetLaserWeldParameter(i).WobbleSpeed;
                if (wob != 0)
                {
                    double wobVoltage = 0.5 + 0.2722 * (wob / 1000.0 - 1.0);
                    _mo.SetWobble(wobVoltage);
                }
            }
        }

        //
        // Line Point Group
        //
        private List<int[]> LinePointGroup(int i, int ct, in List<int[]> ptlist, in PointListData pts)
        {
            if (_splist == null || _moveSeg == null || _type == null || _splist == null)
                return new List<int[]>();

            var lineseg = new List<int[]>();

            int j;
            if (i < ct - 1)
            {
                j = i + 1;
                while (pts.GetLineType(j) == 0)
                {
                    if (j >= ct - 1) break;
                    j++;
                }
            }
            else j = ct - 1;

            var sp1 = new List<double>();
            for (int k = i; k <= j; k++)
            {
                lineseg.Add(ptlist[k]);
                var lp = pts.GetLaserWeldParameter(k);
                if (lp.LaserPower == 0) sp1.Add(lp.LeapSpeed);
                else sp1.Add(lp.WeldSpeed);
            }
            _splist.Add(sp1);

            return lineseg;
        }

        //
        // Arc point Group
        //
        private List<int[]> ArcPointGroup(int i, int ct, in List<int[]> ptlist, in PointListData pts)
        {
            if (_splist == null || _moveSeg == null || _type == null || _splist == null)
                return new List<int[]>();
            var arcseg = new List<int[]>();

            // no matter what suitation, add line section always in the first arc,
            // when it is the first arc from the first point.
            if (i == 0)
            {
                var preSeg = new List<int[]>()
                {
                    ptlist[1],
                };

                var lp2 = pts.GetLaserWeldParameter(0);
                _moveSeg.Add(preSeg);

                if (lp2.LaserPower == 0)
                    _splist.Add(new List<double> { lp2.LeapSpeed });
                else
                    _splist.Add(new List<double> { lp2.WeldSpeed });

                _type.Add(0);
            }

            int j = i + 1;
            while (pts.GetLineType(j) == 1)
            {
                j++;
                if (j >= ct) break;
            }

            // in the arc segment, the line point is not included.
            if ((j - i) % 2 == 0) j--;
            else j -= 2;

            var sp = new List<double>();
            for (int k = i; k <= j; k++)
            {
                arcseg.Add(ptlist[k]);
                sp.Add(pts.GetLaserWeldParameter(k).WeldSpeed);
            }
            _splist.Add(sp);

            return arcseg;
        }

        private void MarkLaserSegment(in PointListData pts)
        {
            if (_moveSeg == null || _type == null) return;

            _laserSeg = new List<int>();
            _laserState = new List<int>();
            _laserArgs = new List<LaserArgument>();

            int iseg = 0;
            int ipt = 0;
            for (int i = 0; i < _moveSeg.Count; i++)
            {
                if (_type[i] == 1)
                {
                    MarkArcLaser(pts, i, ref iseg, ref ipt);
                }
                else
                {
                    MarkLineLaser(pts, i, ref iseg, ref ipt);
                }
            }

            // force the end point to switch off laser.
            var elasarg = new LaserArgument(pts.GetLaserWeldParameter(0));
            _laserSeg.Add(iseg + 1);
            _laserState.Add((int)LaserAct.LasOff);
            _laserArgs.Add(elasarg);
        }

        private void MarkLineLaser(in PointListData pts, int i, ref int iseg, ref int ipt)
        {
            if (_moveSeg == null || _type == null || _laserSeg == null
                || _laserArgs == null || _laserState == null) return;

            for (int j = 0; j < _moveSeg[i].Count; j++)
            {
                var nlasarg = new LaserArgument(pts.GetLaserWeldParameter(ipt));

                int cn = _laserSeg.Count;
                if ((cn == 0 && nlasarg.Power != 0) || cn > 0)
                {
                    _laserSeg.Add(iseg);
                    _laserArgs.Add(nlasarg);

                    if (_laserState.Count == 0)
                        _laserState.Add((int)LaserAct.SetModeOn);
                    else if (nlasarg.Power != 0)
                        _laserState.Add((int)LaserAct.AdjPow);
                    else
                        _laserState.Add((int)LaserAct.LasOff);
                }
                iseg++;
                ipt++;
            }

            ipt--;
            iseg--;
        }

        private void MarkArcLaser(in PointListData pts, int i, ref int iseg, ref int ipt)
        {
            if (_moveSeg == null || _type == null || _laserSeg == null
                || _laserArgs == null || _laserState == null) return;

            ipt += _moveSeg[i].Count;
            iseg += _moveSeg[i].Count / 2;

            // at the arc end point
            var nlasarg = new LaserArgument(pts.GetLaserWeldParameter(ipt - 1));
            _laserSeg.Add(iseg);
            _laserArgs.Add(nlasarg);

            if (nlasarg.Power != 0)
                _laserState.Add((int)LaserAct.AdjPow);
            else
                _laserState.Add((int)LaserAct.LasOff);

            // to the next point
            iseg++;
        }
    }
}
