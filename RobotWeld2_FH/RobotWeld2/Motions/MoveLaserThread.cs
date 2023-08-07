using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Threading;

namespace RobotWeld2.Motions
{
    public class MoveLaserThread
    {
        private List<WeldMoveSection>? _wms;
        private bool ActionFinish;

        // data for lasing
        private List<int>? _laserNodes;
        private List<int>? _laserNodeState;
        private List<LaserArgument>? _arguments;

        // move function
        private readonly MotionOperate _mo;
        public MoveLaserThread(MotionOperate mo)
        {
            _mo = mo;
        }

        public void PutWeldSectionData(List<WeldMoveSection> wms)
        {
            _wms = wms;
        }

        public void LaserDataAnalyse()
        {
            if (_wms == null) { return; }

            _laserNodes = new List<int>();
            _laserNodeState = new List<int>();
            _arguments = new List<LaserArgument>();

            int ptcount = 0;
            for (int i = 0; i < _wms.Count; i++)
            {
                RemarkLaserState(ptcount, _wms[i]);
                if (i == 0)
                {
                    ptcount = _wms[0].GetPointCount();
                }
                else
                {
                    ptcount += _wms[i].GetPointCount() - 1;
                }
            }
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
                    AdjustLaser(i);
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

        private void AdjustLaser(int nodeNum)
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
                _mo.AdjustLaserPower(_arguments[nodeNum], _arguments[nodeNum].Power);
            }
            else if (_laserNodeState[nodeNum] == 3)
            {
                // Switch off the laser.
                _mo.LaserOffNoFall();
            }
        }

        //
        // Extract laser states as the laserNodeState from weld-move-section data.
        // _laserNodes : section number
        // _laserNodesState: at this section number, how to adjust the laser
        // _argument: the laser argument at this section number.
        //
        private void RemarkLaserState(int wmsnum, WeldMoveSection iwms)
        {
            if (_laserNodes == null || _laserNodeState == null || _arguments == null)
            {
                return;
            }

            LaserArgument? oldArg = new();
            Dictionary<int, LaserArgument> argDic = iwms.Argument;
            foreach (var arg in argDic)
            {
                if (oldArg != null && !arg.Equals(oldArg))
                {
                    if (arg.Value.Frequency == oldArg.Frequency && arg.Value.Duty == arg.Value.Duty
                        && arg.Value.Power != oldArg.Power)
                    {
                        // just adjust power
                        _laserNodeState.Add(2);
                    }
                    else if (arg.Value.Power == 0)
                    {
                        // switch off the laser power
                        _laserNodeState.Add(3);
                    }
                    else
                    {
                        // reset the laser mode.
                        _laserNodeState.Add(1);
                    }

                    _arguments.Add(arg.Value);
                    _laserNodes.Add(wmsnum + arg.Key);
                    oldArg = arg.Value;
                }
            }
        }
    }
}
