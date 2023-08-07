using Modbus.Message;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using System;
using System.IO;
using System.Threading;
using static System.Math;

namespace RobotWeld2.AppModel
{
    /// <summary>
    /// The data model to calibrate the machine
    /// </summary>
    public class CalibrateMachineModel
    {
        private const string cfgFile = "./GCN400.cfg";
        private const string mspf = "./RobotWeld.cfg";

        private CalibrateMachineViewModel? _viewModel;
        private MotionOperate? _mo;
        private readonly int[] _Zstate;
        private int _zdir;
        private int _zhomedir;

        private readonly int[] _Ystate;
        private int _ydir;
        private int _yhomedir;

        private readonly int[] _Xstate;
        private int _xdir;
        private int _xhomedir;

        private double _progressBaseline;
        private bool _updateProgress;

        public CalibrateMachineModel()
        {
            _Zstate = new int[3];
            _Ystate = new int[3];
            _Xstate = new int[3];
        }

        public void GetMotionOperate(MotionOperate mo)
        {
            _mo = mo;
        }

        public void ReadCfgFile(CalibrateMachineViewModel vm)
        {
            _viewModel = vm;

            int i = 0;
            string[] lines = new string[635];

            // read from the file
            try
            {
                FileStream afile = new(cfgFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new(afile);

                string? line = sr.ReadLine();
                while (line != null)
                {
                    lines[i] = line;
                    if (i++ >= 635) break;
                    line = sr.ReadLine();
                }

                sr.Close();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 300);
            }
            finally
            {
                // For the X axis
                string[] svar = lines[5].Split('=');
                if (Convert.ToInt32(svar[1]) == 0)
                {
                    _viewModel.XDirection = 1;
                }
                else if (Convert.ToInt32(svar[1]) == 2)
                {
                    _viewModel.XDirection = -1;
                }

                svar = lines[9].Split('=');
                _viewModel.XNegLimit = Convert.ToInt32(svar[1]);

                svar = lines[10].Split('=');
                _viewModel.XPosLimit = Convert.ToInt32(svar[1]);

                // For the Y axis
                svar = lines[36].Split('=');
                if (Convert.ToInt32(svar[1]) == 0)
                {
                    _viewModel.YDirection = 1;
                }
                else if (Convert.ToInt32(svar[1]) == 2)
                {
                    _viewModel.YDirection = -1;
                }

                svar = lines[40].Split('=');
                _viewModel.YNegLimit = Convert.ToInt32(svar[1]);

                svar = lines[41].Split('=');
                _viewModel.YPosLimit = Convert.ToInt32(svar[1]);

                // For the Z axis
                svar = lines[67].Split('=');
                if (Convert.ToInt32(svar[1]) == 0)
                {
                    _viewModel.ZDirection = 1;
                }
                else if (Convert.ToInt32(svar[1]) == 2)
                {
                    _viewModel.ZDirection = -1;
                }

                svar = lines[71].Split('=');
                _viewModel.ZNegLimit = Convert.ToInt32(svar[1]);

                svar = lines[72].Split('=');
                _viewModel.ZPosLimit = Convert.ToInt32(svar[1]);
            }

            //-- To read pulse number for each axis
            try
            {
                FileStream mfile = new(mspf, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new(mfile);

                string? line = sr.ReadLine();
                line = sr.ReadLine();
                if (line != null)
                {
                    string[] svar = line.Split(',');
                    _viewModel.XPulse = Convert.ToInt32(svar[3]);
                }

                line = sr.ReadLine();
                if (line != null)
                {
                    string[] svar = line.Split(',');
                    _viewModel.YPulse = Convert.ToInt32(svar[3]);
                }

                line = sr.ReadLine();
                if (line != null)
                {
                    string[] svar = line.Split(',');
                    _viewModel.ZPulse = Convert.ToInt32(svar[3]);
                }

                line = sr.ReadLine();
                if (line != null)
                {
                    string[] svar = line.Split(',');
                    _viewModel.APulse = Convert.ToInt32(svar[3]);
                }

                sr.Close();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 304);
            }
            finally
            {
                // do nothing
            }
        }

        public void CalibrateMachine()
        {
            if (_mo == null) return;

            CalibrateMachineOperate cmo = new(_mo, this);
            cmo.CalibrateMachine();

            // The thread to display the progress bar.
            Thread pthread = new(ProgressThread)
            {
                IsBackground = true
            };
            pthread.Start();
        }

        private void ProgressThread()
        {
            if (_viewModel == null) return;

            double timesecond = 0;
            double progressScale = 0;
            _progressBaseline = 0;
            _updateProgress = false;

            while (progressScale < 100)
            {
                progressScale = _progressBaseline + 33.3 * (1.0 - System.Math.Exp(-timesecond));
                _viewModel.ProgressScale = progressScale;

                Thread.Sleep(500);

                if (_updateProgress)
                {
                    timesecond = 0;
                    _updateProgress = false;
                }
                else
                {
                    timesecond += 0.01;
                }
            }
        }

        public void FinishAxis(int[] axisValue, int axisIndex)
        {
            if (_viewModel == null) { return; }

            switch (axisIndex)
            {
                case 0:    // X axis
                    // the signs of limits are checked in thread
                    _viewModel.XPosLimit = _Xstate[0] = Abs(axisValue[0]);
                    _viewModel.XNegLimit = _Xstate[1] = -Abs(axisValue[1]);

                    if (axisValue[0] > 0)
                    {
                        // axisValue[0] save the data for positive limit
                        // if it > 0, that, the positive limit in the positive pulse direction
                        // The pulse is in the normal direction.
                        _viewModel.XDirection = _xdir = 1;
                    }
                    else
                    {
                        // axisValue[0] save the data for positive limit
                        // if it < 0, that, the negative limit in the positive pulse direction
                        // The pulse is in the revise direction.
                        _viewModel.XDirection = _xdir = -1;
                    }

                    if (Abs(axisValue[0]) >= Abs(axisValue[1]))
                    {
                        // the homeback should be in the revise direction.
                        _xhomedir = 0;
                    }
                    else
                    {
                        // the homeback should be in the normal direction.
                        _xhomedir = 1;
                    }

                    _Xstate[2] = axisValue[2];

                    _progressBaseline = 99.9;
                    _updateProgress = true;
                    break;
                case 1:    // Y axis
                    // the signs of limits are checked in thread
                    _viewModel.YPosLimit = _Ystate[0] = Abs(axisValue[0]);
                    _viewModel.YNegLimit = _Ystate[1] = -Abs(axisValue[1]);

                    if (axisValue[0] > 0)
                    {
                        // axisValue[0] save the data for positive limit
                        // if it > 0, that, the positive limit in the positive pulse direction
                        // The pulse is in the normal direction.
                        _viewModel.YDirection = _ydir = 1;
                    }
                    else
                    {
                        // axisValue[0] save the data for positive limit
                        // if it < 0, that, the negative limit in the positive pulse direction
                        // The pulse is in the revise direction.
                        _viewModel.YDirection = _ydir = -1;
                    }

                    if (Abs(axisValue[0]) >= Abs(axisValue[1]))
                    {
                        // the homeback should be in the revise direction.
                        _yhomedir = 0;
                    }
                    else
                    {
                        // the homeback should be in the normal direction.
                        _yhomedir = 1;
                    }

                    _Ystate[2] = axisValue[2];

                    _progressBaseline = 66.6;
                    _updateProgress = true;
                    break;
                case 2:    // Z axis
                    // the signs of limits are checked in thread
                    _viewModel.ZPosLimit = _Zstate[0] = Abs(axisValue[0]);
                    _viewModel.ZNegLimit = _Zstate[1] = -Abs(axisValue[1]);

                    if (axisValue[0] > 0)
                    {
                        // axisValue[0] save the data for positive limit
                        // if it > 0, that, the positive limit in the positive pulse direction
                        // The pulse is in the normal direction.
                        _viewModel.ZDirection = 1;
                        _zdir = 1;
                    }
                    else
                    {
                        // axisValue[0] save the data for positive limit
                        // if it < 0, that, the negative limit in the positive pulse direction
                        // The pulse is in the revise direction.
                        _viewModel.ZDirection = _zdir = -1;
                    }

                    if (Abs(axisValue[0]) >= Abs(axisValue[1]))
                    {
                        // the homeback should be in the revise direction.
                        _zhomedir = 0;
                    }
                    else
                    {
                        // the homeback should be in the normal direction.
                        _zhomedir = 1;
                    }

                    _Zstate[2] = axisValue[2];

                    _progressBaseline = 33.3;
                    _updateProgress = true;
                    break;
                default:
                    break;
            }
        }

        [STAThread]
        public void ConfirmResult()
        {
            int i = 0;
            int lineNum = 0;
            string[] lines = new string[635];

            // read from the file
            try
            {
                FileStream afile = new(cfgFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new(afile);

                string? line = sr.ReadLine();
                while (line != null)
                {
                    lines[i] = line;
                    if (i++ >= 635) break; ;
                    line = sr.ReadLine();
                }
                lineNum = i;

                sr.Close();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 301);
            }
            finally
            {
                // Do nothing
            }

            // For the X axis
            if (_xdir == -1)
            {
                lines[5] = "stepMode=2";
            }
            lines[9] = "softLmtNeg=" + _Xstate[1].ToString();
            lines[10] = "softLmtPos=" + _Xstate[0].ToString();

            if (_xhomedir == 1)
            {
                lines[18] = "hm_dir=1";
            }

            // For the Y axis
            if (_ydir == -1)
            {
                lines[36] = "stepMode=2";
            }
            lines[40] = "softLmtNeg=" + _Ystate[1].ToString();
            lines[41] = "softLmtPos=" + _Ystate[0].ToString();

            if (_yhomedir == 1)
            {
                lines[49] = "hm_dir=1";
            }

            // For the Z axis
            if (_zdir == -1)
            {
                lines[67] = "stepMode=2";
            }
            lines[71] = "softLmtNeg=" + _Zstate[1].ToString();
            lines[72] = "softLmtPos=" + _Zstate[0].ToString();

            if (_zhomedir == 1)
            {
                lines[80] = "hm_dir=1";
            }

            // write to file
            try
            {
                FileStream afile = new(cfgFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter sw = new(afile);

                for (int j = 0; j < lineNum; j++)
                {
                    sw.WriteLine(lines[j]);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 302);
            }
            finally
            {
                // Do nothing
            }

            try
            {
                FileStream mfile = new(mspf, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter sw = new(mfile);

                if (_viewModel == null)
                {
                    throw (new Exception());
                }

                int xsign = _xhomedir == 0 ? 1 : -1;
                int ysign = _yhomedir == 0 ? 1 : -1;
                int zsign = _zhomedir == 0 ? 1 : -1;

                sw.WriteLine("[Motion Sepcifications]");
                sw.WriteLine("x," + xsign + "," + _xhomedir + "," + _viewModel.XPulse);
                sw.WriteLine("y," + ysign + "," + _yhomedir + "," + _viewModel.YPulse);
                sw.WriteLine("z," + zsign + "," + _zhomedir + "," + _viewModel.ZPulse);
                sw.WriteLine("a," + 1 + "," + 1 + "," + _viewModel.APulse);
                sw.Close();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 303);
            }
            finally
            {
                // Do nothing
            }
        }
    }
}
