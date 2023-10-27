using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.Motions
{
    public class MoveLaserThread
    {
        private List<WeldMoveSection>? _wms;
        private bool ActionFinish;

        // data for lasing
        private List<int>? _laserSeg;
        private List<int>? _laserState;
        private List<LaserArgument>? _laserArgs;

        // move function
        private readonly MotionOperate _mo;
        public MoveLaserThread(MotionOperate mo)
        {
            _mo = mo;
        }

        public void PutWeldSectionData(List<int> laserSeg, List<int> laserState, List<LaserArgument> laserArgs)
        {
            _laserSeg = laserSeg;
            _laserState = laserState;
            _laserArgs = laserArgs;
        }

        /// <summary>
        /// To switch on/off the laser thread.
        /// </summary>
        /// <param name="state"></param>
        public void SetActionFinish(bool state)
        {
            this.ActionFinish = state;
        }

        public void LaserThread()
        {
            if (_laserSeg == null || _laserState == null || _laserArgs == null) { return; }

            int i = 0;
            int sectionAct;
            ActionFinish = false;
            while (!ActionFinish)
            {
                sectionAct = _mo.GetSectionNumber();

                // shift the laser section
                if (sectionAct > _laserSeg[i])
                {
                    int sta = _laserState[i];
                    AdjustLaser(sta, i);
                    i++;
                }

                if (i > _laserSeg.Count - 1)
                {
                    _mo.CloseAir();
                    break;
                }
            }

            // make sure the laser off at the last step.
            _mo.LaserOffNoFall();
            _mo.CloseAir();
        }

        private void AdjustLaser(int nodeNum, int iNum)
        {
            if (_laserArgs == null)
            {
                MainModel.AddInfo("焊接激光参数错误");
                return;
            }

            if (nodeNum == (int)LaserAct.SetModeOn)
            {
                // Switch on the laser in the first step or change the laser mode.
                _mo.SetupLaserParameter(_laserArgs[iNum]);
                if (_laserArgs[nodeNum].Frequency == 0)
                    _mo.SetLaserMode();
                else
                    _mo.SetLaserMode(_laserArgs[iNum].Frequency, _laserArgs[iNum].Duty);

                _mo.OpenAir();
                _mo.LaserOnNoRise();
            }
            else if (nodeNum == (int)LaserAct.AdjPow)
            {
                // change the laser power only
                _mo.OpenAir();
                // _mo.ResetPWM();
                _mo.AdjustLaserPower(_laserArgs[iNum].Power);
            }
            else if (nodeNum == (int)LaserAct.LasOff)
            {
                // Switch off the laser.
                _mo.LaserOffNoFall();
                _mo.CloseAir();
            }
        }

        //-------------------------------------------------------------
        // Old methods, used already
        //-------------------------------------------------------------
        // data for lasing
        private List<int>? _laserNodes;
        private List<int>? _laserNodeState;
        private List<LaserArgument>? _arguments;

        public void CLaserThread()
        {
            if (_laserNodes == null || _laserNodeState == null || _arguments == null)
            {
                MainModel.AddInfo("激光参数Thread错误");
                return;
            }

            int i = 0;
            int sectionAct;
            ActionFinish = false;
            while (!ActionFinish)
            {
                sectionAct = _mo.GetSectionNumber();

                // shift the laser section
                if (sectionAct >= _laserNodes[i])
                {
                    CAdjustLaser(i);
                    i++;
                }

                if (i > _laserNodes.Count - 1)
                {
                    break;
                }
            }

            // make sure the laser off at the last step.
            _mo.LaserOffNoFall();
            _mo.CloseAir();
        }

        private void CAdjustLaser(int nodeNum)
        {
            if (_laserNodes == null || _laserNodeState == null || _arguments == null)
            {
                MainModel.AddInfo("焊接激光参数错误");
                return;
            }

            if (_laserNodeState[nodeNum] == 1)
            {
                _mo.OpenAir();
                // Switch on the laser in the first step or change the laser mode.
                _mo.SetupLaserParameter(_arguments[nodeNum]);
                if (_arguments[nodeNum].Frequency == 0 || _arguments[nodeNum].Duty == 0 || _arguments[nodeNum].Duty == 100)
                {
                    _mo.SetLaserMode();
                }
                else
                {
                    _mo.SetLaserMode(_arguments[nodeNum].Frequency, _arguments[nodeNum].Duty);
                }

                _mo.LaserOnNoRise();
            }
            else if (_laserNodeState[nodeNum] == 2)
            {
                _mo.OpenAir();
                // change the laser power only
                _mo.AdjustLaserPower(_arguments[nodeNum].Power);
            }
            else if (_laserNodeState[nodeNum] == 3)
            {
                // Switch off the laser.
                _mo.LaserOffNoFall();
                _mo.CloseAir();
            }
        }

        public void CPutWeldSectionData(List<WeldMoveSection> wms)
        {
            _wms = wms;
        }

        public void CLaserDataAnalyse()
        {
            if (_wms == null) { return; }

            _laserNodes = new List<int>();
            _laserNodeState = new List<int>();
            _arguments = new List<LaserArgument>();

            int wmsStep = 0;
            for (int i = 0; i < _wms.Count; i++)
            {
                RemarkLaserState(_wms[i], wmsStep);

                if (i == 0)
                    wmsStep += _wms[i].GetPointCount();
                else
                    wmsStep += _wms[i].GetPointCount() - 1;
            }

            WriteMsgFile.WriteFile(_laserNodes, _laserNodeState);
        }

        //
        // Extract laser states as the laserNodeState from weld-move-section data.
        // _laserNodes : section number
        // _laserNodesState: at this section number, how to adjust the laser
        // _argument: the laser argument at this section number.
        //
        private void RemarkLaserState(WeldMoveSection iwms, int step)
        {
            bool hasOn = false;

            if (_laserNodes == null || _laserNodeState == null || _arguments == null)
            {
                return;
            }

            var oldArg = new LaserArgument();
            Dictionary<int, LaserArgument> argDic = iwms.Argument;
            foreach (var arg in argDic)
            {
                if (oldArg != null && !arg.Equals(oldArg))
                {
                    if (arg.Value.Frequency == oldArg.Frequency && arg.Value.Duty == arg.Value.Duty
                        && arg.Value.Power != oldArg.Power)
                    {
                        // Whatever, the first laser action should set laser mode
                        // or other section, we can just adjust power
                        if (!hasOn)
                        {
                            _laserNodeState.Add(1);
                            hasOn = true;
                        }
                        else
                            _laserNodeState.Add(2);
                    }
                    else if (arg.Value.Power == 0)
                    {
                        // switch off the laser power
                        _laserNodeState.Add(3);
                    }
                    else
                    {
                        // set the laser mode and switch on.
                        _laserNodeState.Add(1);
                    }

                    _arguments.Add(arg.Value);
                    _laserNodes.Add(arg.Key + step);
                    oldArg = arg.Value;
                }
            }
        }
    }
}
