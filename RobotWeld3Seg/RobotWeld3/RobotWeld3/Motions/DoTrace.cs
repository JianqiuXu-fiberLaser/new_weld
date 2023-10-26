///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: new added class to display laser parameters.
// Ver. 3.0: All-in-one trace weld class.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.CaluWMS;
using RobotWeld3.AppModel;
using System.Collections.Generic;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// The basic class to run various trace.
    /// (1) Input: WMS List, and output: moving data.
    /// (2) Start to run.
    /// </summary>
    public class DoTrace
    {
        private readonly MotionBus? _mbus;
        private List<WeldMoveSection>? _wmslist;

        // 1st: wms number;
        // 2nd: with two ints, which are air-in number and air-out time
        //      in this seciton.
        private Dictionary<int, int[]>? _airDic;

        // 1st: wms number; 2nd: fall number in this section.
        private Dictionary<int, int>? _fallDic;

        // The kept vector and speed list with hardware direction.
        private List<List<Vector>>? _gps;
        private List<List<double>>? _sps;

        private List<int[]>? _ptlist;
        private List<double>? _splist;

        private List<int[]>? _lasSec;
        private List<double[]>? _lp;

        public DoTrace(MotionBus mbus)
        {
            _mbus = mbus;
        }

        /// <summary>
        /// Prepare the move and weld data.
        /// </summary>
        /// <param name="wms"></param>
        /// <returns> true: the process is correct </returns>
        public bool CopyHardware(in List<WeldMoveSection> wms)
        {
            _wmslist = wms;

            if (!CheckWms()) return false;
            PrepareData();
            CheckLimit();
            CheckWobble();

            if (_ptlist != null && _splist != null && _lasSec != null && _lp != null)
                WriteMsgFile.WriteFile(_ptlist, _splist, _lasSec, _lp);

            return true;
        }

        /// <summary>
        /// Put the parameters to Run method, and trigger the action.
        /// </summary>
        public void WorkProcess()
        {
            if (_mbus == null || _ptlist == null || _splist == null ||
                _lasSec == null || _lp == null)
                return;

            var runWeldAll = new RunWeldAll(_mbus);
            runWeldAll.PutParameter(_ptlist, _splist, _lasSec, _lp);
        }


        //-------------------------------------------------------------
        // Procedure to check and prepare the moving data
        //-------------------------------------------------------------

        /// <summary>
        /// Check the WmsList whether correct? (1) order; (2) continuous;
        /// (3) number; (4) velocity matching;
        /// </summary>
        /// <returns></returns>
        private bool CheckWms()
        {
            if (_wmslist == null) return false;

            for (int i = 0; i < _wmslist.Count - 1; i++)
            {
                if (_wmslist[i].WmsIndex != i)
                {
                    Assertion.AssertError("The wms order error.", 2001);
                    return false;
                }

                if (_wmslist[i].GetPosition().Count < 2)
                {
                    Assertion.AssertError("Point's number less than 2.", 2002);
                    return false;
                }

                var sp = _wmslist[i].Speed.Count;
                if (sp != _wmslist[i].GetPosition().Count)
                {
                    Assertion.AssertError("Velocity is mismatched.", 2003);
                    return false;
                }
            }

            _gps = Transfer.HardwarePosition(_wmslist);
            _sps = Transfer.HardwareSpeed(_wmslist);

            return true;
        }

        /// <summary>
        /// Check the wms list if they are out the limits
        /// </summary>
        /// <returns></returns>
        private bool CheckLimit()
        {
            if (_ptlist == null) return false;

            for (int i = 0; i < _ptlist.Count; i++)
            {
                var pt = _ptlist[i];
                if (pt[0] > MotionSpecification.XPosLimit)
                {
                    pt[0] = MotionSpecification.XPosLimit;
                    MainModel.AddInfo("X坐标超正极限");
                }
                else if (pt[0] < MotionSpecification.XNegLimit)
                {
                    pt[0] = MotionSpecification.XNegLimit;
                    MainModel.AddInfo("X坐标超负极限");
                }

                if (pt[1] > MotionSpecification.YPosLimit)
                {
                    pt[1] = MotionSpecification.YPosLimit;
                    MainModel.AddInfo("Y坐标超正限");
                }
                else if (pt[1] < MotionSpecification.YNegLimit)
                {
                    pt[1] = MotionSpecification.YNegLimit;
                    MainModel.AddInfo("Y坐标超负限");
                }

                if (pt[2] > MotionSpecification.ZPosLimit)
                {
                    pt[2] = MotionSpecification.ZPosLimit;
                    MainModel.AddInfo("Z坐标超正限");
                }
                else if (pt[2] < MotionSpecification.ZNegLimit)
                {
                    pt[2] = MotionSpecification.ZNegLimit;
                    MainModel.AddInfo("Z坐标超负限");
                }
            }

            return true;
        }

        /// <summary>
        /// Check and set wobble frquency
        /// </summary>
        /// <param name="pts"></param>
        private void CheckWobble()
        {
            if (_wmslist == null || _mbus == null) return;

            int ct = _wmslist.Count;
            for (int i = 0; i < ct; i++)
            {
                var wob = _wmslist[i].Wobble;
                if (wob != 0)
                {
                    double wobVoltage = 0.5 + 0.2722 * (wob / 1000.0 - 1.0);
                    _mbus.SetWobble(wobVoltage);
                    return;
                }
            }

            _mbus.SetWobble(0.0);
        }

        /// <summary>
        /// Transfer Wms to ptlist, which can be used directly by the motion card.
        /// _ptlist: point list.
        /// _splist: speed list.
        /// _lasSec: the segment number that need a laser actions. 
        ///          And the action number as follows:
        ///          (1) air in; (2) laser on >> power, rise; (3) change mode >> power,
        ///          rise, frequency, duty cycle; (4) air out; (5) laser off >> fall;
        /// _lp: laser parameter, depending on the value of _lasSec.
        /// </summary>
        private void PrepareData()
        {
            InsertAir();
            InsertFall();
            FilterPoints();
        }

        /// <summary>
        /// Insert Air-in and Air-out in the section, where no laser power.
        /// Air-in is triggered by the moving segement number.
        /// Air-out accounts with the time, so that when the segment time is
        /// enough, we just waiting a air time, then close it.
        /// </summary>
        private void InsertAir()
        {
            if (_wmslist == null || _gps == null || _sps == null) return;
            _airDic = new Dictionary<int, int[]>();

            // The first wms section
            var pw = _wmslist[0].LaserParameter.Power;
            if (pw != 0)
            {
                // Open air immediately, because the laser opened
                // from the start.
                _airDic.Add(0, new int[2] { 0, -1 });
            }
            else
            {
                var air = _wmslist[1].LaserParameter.Air;

                // Opening the air at the first
                // In accout total segment number.
                var gp = _gps[0];
                var gs = _sps[0];
                var si = Transfer.ToAirIn(air, ref gp, ref gs);
                _airDic.Add(0, new int[2] { si, -1 });
            }

            // Middle wms sections. Because the i = 1 wms, laser must be on,
            // the middle wms for air counted from i = 2.
            // If the wms <= 3, there is not middle air wms.
            for (int i = 2; i < _wmslist.Count - 1; i++)
            {
                var pwi = _wmslist[i].LaserParameter.Power;
                if (pwi == 0)
                {
                    var gp0 = _gps[i];
                    var gs0 = _sps[i];
                    var aOut = _wmslist[i - 1].LaserParameter.Air;
                    var aIn = _wmslist[i + 1].LaserParameter.Air;
                    (int sin, double sout) = Transfer.ToAirInOut(aIn, aOut, ref gp0, ref gs0);
                    _airDic.Add(i, new int[2] { sin, (int)sout });
                }
            }

            // The last wms section, close air always
            int c = _wmslist.Count;
            var aire = _wmslist[c - 1].LaserParameter.Air;
            _airDic.Add(c - 1, new int[2] { -1, (int)aire });

            //WriteMsgFile.WriteFile(_airDic);
        }

        /// <summary>
        /// Insert Laser fall inside the lasing section.
        /// Dic, 2nd int: laser off is determined by the segment number.
        /// </summary>
        private void InsertFall()
        {
            if (_wmslist == null || _gps == null || _sps == null) return;
            _fallDic = new Dictionary<int, int>();

            // Middle wms sections
            // When this wms is laser meanwhile the next wms is laser off.
            for (int i = 1; i < _wmslist.Count - 1; i++)
            {
                var p = _wmslist[i].LaserParameter;
                var p1 = _wmslist[i + 1].LaserParameter;
                if (p.Power != 0 && p1.Power == 0) 
                {
                    var gp0 = _gps[i];
                    var gs0 = _sps[i];
                    int startOff = Transfer.ToLaserOff(p.LaserFall, ref gp0, ref gs0);
                    _fallDic.Add(i, startOff);
                }
            }

            // The last wms should be consider seperately.
            // Usually, it has the last wms to back the prepared point, and
            // make its power to zero.
            var pn = _wmslist[^1].LaserParameter;
            if (pn.Power != 0)
            {
                var gp0 = _gps[^1];
                var gs0 = _sps[^1];
                int startOff = Transfer.ToLaserOff(pn.LaserFall, ref gp0, ref gs0);
                _fallDic.Add(_wmslist.Count - 1, startOff);
            }

            //WriteMsgFile.WriteFile(_fallDic);
        }

        /// <summary>
        /// Filter the first point and matching the speed.
        /// Add all laser segment marks to _lasSec and _lp.
        /// </summary>
        private void FilterPoints()
        {
            if (_airDic == null || _fallDic == null || _wmslist == null || _gps == null || _sps == null)
                return;

            _ptlist = new();
            _splist = new();
            _lasSec = new();
            _lp = new();

            int secNum = 0;
            // In the first wms, the first point should be kept.
            // Other wms, the first point is eliminated.
            for (int i = 0; i < _wmslist.Count; ++i)
            {
                var gp = _gps[i];
                var gs = _sps[i];

                // Add the position and speed.
                List<int[]> ptl;
                List<double> gsl;
                if (i == 0)
                {
                    ptl = Transfer.ToPosition(gp, 0);
                    gsl = Transfer.ToSpeed(gs, 0);
                    _ptlist.AddRange(ptl);
                    _splist.AddRange(gsl);
                }
                else
                {
                    ptl = Transfer.ToPosition(gp, i);
                    gsl = Transfer.ToSpeed(gs, i);
                    _ptlist.AddRange(ptl);
                    _splist.AddRange(gsl);
                }

                // At the begin of wms, switch on laser or change laser mode.
                // except the 1st wms.
                if (i > 0)
                {
                    var lpara0 = _wmslist[i - 1].LaserParameter;
                    var lpara = _wmslist[i].LaserParameter;
                    int lst;
                    double[] lp;

                    if (lpara0.Power == 0 && lpara.Power != 0)
                    {
                        (lst, lp) = Transfer.ToLaserOn(lpara0, lpara, gp, gs);
                        _lasSec.Add(new int[2] { secNum, lst });
                        _lp.Add(lp);
                    }
                }
                else
                {
                    // The first wms, if the laser is turn on at the start.
                    var lpara00 = _wmslist[i].LaserParameter;
                    int lst0;
                    double[] lp0;
                    if (lpara00.Power != 0)
                    {
                        (lst0, lp0) = Transfer.ToLaserOnStart(lpara00, gp, gs);
                        _lasSec.Add(new int[2] { secNum, lst0 });
                        _lp.Add(lp0);
                    }
                }

                // Add air in and air out
                if (_airDic.ContainsKey(i))
                {
                    // Air in, except the last wms
                    if (_airDic[i][0] != -1 && i < _wmslist.Count - 1)
                    {
                        _lasSec.Add(new int[2] { secNum + _airDic[i][0], (int)LaserAct.AirIn });
                        var airIn = _wmslist[i + 1].LaserParameter.Air;
                        _lp.Add(new double[] { airIn });
                    }

                    // Air out, except the 1st wms
                    if (_airDic[i][1] != -1 && i > 0)
                    {
                        _lasSec.Add(new int[2] { secNum + ptl.Count - 1, (int)LaserAct.AirOut });
                        var airOut = _wmslist[i - 1].LaserParameter.Air;
                        _lp.Add(new double[] { airOut });
                    }
                }

                // Add laser falls. -1 for no fall, switch off immediately.
                if (_fallDic.ContainsKey(i))
                {
                    if (_fallDic[i] != -1)
                    {
                        _lasSec.Add(new int[2] { secNum + _fallDic[i], (int)LaserAct.LaserOff });
                        var fall = _wmslist[i].LaserParameter.LaserFall;
                        _lp.Add(new double[] { fall });
                    }
                    else
                    {
                        _lasSec.Add(new int[2] { secNum + ptl.Count - 1, (int)LaserAct.LaserOff });
                        _lp.Add(new double[] { 0 });
                    }
                }

                // For the first wms, the first point is included.
                // but the first point is not real point to be arrived.
                // The action segement is accounted from the first point.
                // To the first point is not accounted.
                if (i == 0)
                    secNum += ptl.Count - 1;
                else
                    secNum += ptl.Count;
            }

            Transfer.SortSectionMarker(ref _lasSec, ref _lp);
            //WriteMsgFile.WriteFile(_lasSec, _lp);
        }
    }
}
