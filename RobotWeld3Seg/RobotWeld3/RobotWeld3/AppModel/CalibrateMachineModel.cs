///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) new configu file in XML style.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using RobotWeld3.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using static System.Math;

namespace RobotWeld3.AppModel
{
    /// <summary>
    /// The data model to calibrate the machine
    /// </summary>
    public class CalibrateMachineModel
    {
        private const string cfgFile = "./GCN400.cfg";
        private const string mspf = "./RobotWeld.cfg";

        private CalibrateMachineViewModel? _viewModel;
        private MotionBus? _mbus;

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

        /// <summary>
        /// Calibrate machine hardware.
        /// </summary>
        public CalibrateMachineModel()
        {
            _Zstate = new int[3];
            _Ystate = new int[3];
            _Xstate = new int[3];
        }

        /// <summary>
        /// Get motionbus reference
        /// </summary>
        /// <param name="mbus"></param>
        internal void GetMotionOperate(MotionBus mbus)
        {
            _mbus = mbus;
        }

        /// <summary>
        /// Read the config file
        /// </summary>
        /// <param name="vm"></param>
        public void ReadCfgFile(CalibrateMachineViewModel vm)
        {
            _viewModel = vm;

            int i = 0;
            string[] lines = new string[635];

            // read from the file
            FileStream afile0 = new(cfgFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr0 = new(afile0);
            try
            {
                string? line = sr0.ReadLine();
                while (line != null)
                {
                    lines[i] = line;
                    if (i++ >= 635) break;
                    line = sr0.ReadLine();
                }

                AnalysisConfigFile(lines);
                MachineSpecificationFile();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 300);
            }
            finally
            {
                sr0.Close();
            }
        }


        /// <summary>
        /// Analyze the file of GCN400.CFG, giving the direction and limits of the machine.
        /// </summary>
        /// <param name="lines"> lines being read </param>
        private void AnalysisConfigFile(string[] lines)
        {
            if (_viewModel == null) return;

            // For the X axis
            string[] svar = lines[5].Split('=');
            if (Convert.ToInt32(svar[1]) == 0)
                _viewModel.XDirection = 1;
            else if (Convert.ToInt32(svar[1]) == 2)
                _viewModel.XDirection = -1;

            svar = lines[9].Split('=');
            _viewModel.XNegLimit = Convert.ToInt32(svar[1]);

            svar = lines[10].Split('=');
            _viewModel.XPosLimit = Convert.ToInt32(svar[1]);

            // For the Y axis
            svar = lines[36].Split('=');
            if (Convert.ToInt32(svar[1]) == 0)
                _viewModel.YDirection = 1;
            else if (Convert.ToInt32(svar[1]) == 2)
                _viewModel.YDirection = -1;

            svar = lines[40].Split('=');
            _viewModel.YNegLimit = Convert.ToInt32(svar[1]);

            svar = lines[41].Split('=');
            _viewModel.YPosLimit = Convert.ToInt32(svar[1]);

            // For the Z axis
            svar = lines[67].Split('=');
            if (Convert.ToInt32(svar[1]) == 0)
                _viewModel.ZDirection = 1;
            else if (Convert.ToInt32(svar[1]) == 2)
                _viewModel.ZDirection = -1;

            svar = lines[71].Split('=');
            _viewModel.ZNegLimit = Convert.ToInt32(svar[1]);

            svar = lines[72].Split('=');
            _viewModel.ZPosLimit = Convert.ToInt32(svar[1]);
        }

        /// <summary>
        /// disply machine's specifications which comes from RobotWeld.CFG
        /// </summary>
        private void MachineSpecificationFile()
        {
            if (_viewModel == null) return;

            // X aixs
            _viewModel.XPulse = (int)MotionOperate.Xmillimeter;
            // Y axis
            _viewModel.YPulse = (int)MotionOperate.Ymillimeter;
            // z axis
            _viewModel.ZPulse = (int)MotionOperate.Zmillimeter;

            // A axis
            _viewModel.APulse = (int)MotionOperate.OneCycle;
            if (MotionSpecification.AaxisState == 1)
                _viewModel.AaxisBool = true;
            else
                _viewModel.AaxisBool = false;

            // feet padel IO port.
            _viewModel.PedalTrigger = MotionSpecification.PedalTrigger;
            // air trigger IO port
            _viewModel.ProtectedAir = MotionSpecification.ProtectedAir;
            // laser enable
            _viewModel.LaserEnable = MotionSpecification.LaserEnable;
            // Wobble dac output
            _viewModel.Wobble = MotionSpecification.WobbleDac;
            // Feed in wire, direction port. 
            _viewModel.FeedWire = MotionSpecification.FeedWire;
            // withdraw the wire, direction port.
            _viewModel.Withdraw = MotionSpecification.Withdraw;
            // Drive the wire servo, DAC
            _viewModel.WireDac = MotionSpecification.WireDac;
        }

        /// <summary>
        /// Run calibration program.
        /// </summary>
        public void CalibrateMachine()
        {
            if (_mbus == null) return;

            var cmo = new CalibrateMachineOperate(_mbus, this);
            cmo.CalibrateMachine();

            // The thread to display the progress bar.
            Thread pthread = new(ProgressThread)
            {
                IsBackground = true
            };
            pthread.Start();
        }

        /// <summary>
        /// thread for progress bar.
        /// </summary>
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

        /// <summary>
        /// The callback when find an axis after scanning the whole moving space.
        /// </summary>
        /// <param name="axisValue"></param>
        /// <param name="axisIndex"></param>
        public void FinishAxis(int[] axisValue, int axisIndex)
        {
            if (_viewModel == null) return;

            switch (axisIndex)
            {
                // the signs of limits are checked in thread
                case 0:    // X axis
                    _viewModel.XPosLimit = _Xstate[0] = Abs(axisValue[0]);
                    _viewModel.XNegLimit = _Xstate[1] = -Abs(axisValue[1]);

                    // axisValue[0] save the data for positive limit
                    // if it > 0, that, the positive limit in the positive pulse direction
                    // The pulse is in the normal direction.
                    // else if
                    // it < 0, that, the negative limit in the positive pulse direction
                    // The pulse is in the revise direction.
                    if (axisValue[0] > 0)
                        _viewModel.XDirection = _xdir = 1;
                    else
                        _viewModel.XDirection = _xdir = -1;

                    // the homeback should be in the revise direction.
                    // else if
                    // the homeback should be in the normal direction.
                    if (Abs(axisValue[0]) >= Abs(axisValue[1]))
                        _xhomedir = 0;
                    else
                        _xhomedir = 1;

                    _Xstate[2] = axisValue[2];

                    _progressBaseline = 99.9;
                    _updateProgress = true;
                    break;
                case 1:    // Y axis
                    _viewModel.YPosLimit = _Ystate[0] = Abs(axisValue[0]);
                    _viewModel.YNegLimit = _Ystate[1] = -Abs(axisValue[1]);

                    // axisValue[0] save the data for positive limit
                    // if it > 0, that, the positive limit in the positive pulse direction
                    // The pulse is in the normal direction.
                    // else if
                    // it < 0, that, the negative limit in the positive pulse direction
                    // The pulse is in the revise direction.
                    if (axisValue[0] > 0)
                        _viewModel.YDirection = _ydir = 1;
                    else
                        _viewModel.YDirection = _ydir = -1;

                    // the homeback should be in the revise direction.
                    // else if
                    // the homeback should be in the normal direction.
                    if (Abs(axisValue[0]) >= Abs(axisValue[1]))
                        _yhomedir = 0;
                    else
                        _yhomedir = 1;

                    _Ystate[2] = axisValue[2];

                    _progressBaseline = 66.6;
                    _updateProgress = true;
                    break;
                case 2:    // Z axis
                    _viewModel.ZPosLimit = _Zstate[0] = Abs(axisValue[0]);
                    _viewModel.ZNegLimit = _Zstate[1] = -Abs(axisValue[1]);

                    // axisValue[0] save the data for positive limit
                    // if it > 0, that, the positive limit in the positive pulse direction
                    // The pulse is in the normal direction.
                    // else if
                    // it < 0, that, the negative limit in the positive pulse direction
                    // The pulse is in the revise direction.
                    if (axisValue[0] > 0)
                        _viewModel.ZDirection = _zdir = 1;
                    else
                        _viewModel.ZDirection = _zdir = -1;

                    // the homeback should be in the revise direction.
                    // else if
                    // the homeback should be in the normal direction.
                    if (Abs(axisValue[0]) >= Abs(axisValue[1]))
                        _zhomedir = 0;
                    else
                        _zhomedir = 1;

                    _Zstate[2] = axisValue[2];

                    _progressBaseline = 33.3;
                    _updateProgress = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Burn machine, while coming with python script.
        /// </summary>
        /// <param name="sw"> start or stop burning </param>
        public void BurninMachine(bool sw)
        {
        }

        /// <summary>
        /// Save calibration results to the config file.
        /// </summary>
        [STAThread]
        public void ConfirmResult()
        {
            int i = 0;
            string[] lines = new string[635];

            // read from the file
            FileStream afile1 = new(cfgFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamReader sr1 = new(afile1);
            try
            {
                string? line = sr1.ReadLine();
                while (line != null)
                {
                    lines[i] = line;
                    if (i++ >= 635) break; ;
                    line = sr1.ReadLine();
                }

                // For the X axis
                if (_xdir == -1) lines[5] = "stepMode=2";

                lines[9] = "softLmtNeg=" + _Xstate[1].ToString();
                lines[10] = "softLmtPos=" + _Xstate[0].ToString();

                if (_xhomedir == 1) lines[18] = "hm_dir=1";

                // For the Y axis
                if (_ydir == -1) lines[36] = "stepMode=2";

                lines[40] = "softLmtNeg=" + _Ystate[1].ToString();
                lines[41] = "softLmtPos=" + _Ystate[0].ToString();

                if (_yhomedir == 1) lines[49] = "hm_dir=1";

                // For the Z axis
                if (_zdir == -1) lines[67] = "stepMode=2";

                lines[71] = "softLmtNeg=" + _Zstate[1].ToString();
                lines[72] = "softLmtPos=" + _Zstate[0].ToString();

                if (_zhomedir == 1) lines[80] = "hm_dir=1";

                // write back
                WriteBackConfigFile(lines, i);
                WriteSpecificationFile();
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 301);
            }
            finally
            {
                sr1.Close();
            }
        }

        /// <summary>
        /// Write back parameters to the config file.
        /// </summary>
        /// <param name="lines"> parameter lines </param>
        /// <param name="lineNum"> lines count </param>
        private static void WriteBackConfigFile(string[] lines, int lineNum)
        {
            FileStream afile2 = new(cfgFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw2 = new(afile2);
            try
            {
                for (int j = 0; j < lineNum; j++)
                {
                    sw2.WriteLine(lines[j]);
                }
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 302);
            }
            finally
            {
                sw2.Close();
            }
        }

        /// <summary>
        /// write bakc specification file. 
        /// In this case, the machine should be restarted after calibration. 
        /// </summary>
        [STAThread]
        private void WriteSpecificationFile()
        {
            var doc = new XmlDocument();

            doc.LoadXml("<Machine></Machine>");
            if (doc.DocumentElement == null) return;
            XmlElement root = doc.DocumentElement;

            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.InsertBefore(decl, root);

            XmlComment comment = doc.CreateComment("The specification file for the sun flower welding system");
            doc.InsertBefore(comment, root);

            XmlElement machine = doc.CreateElement("machine");
            root.InsertAfter(machine, root.FirstChild);

            CreateSpecificationFile(doc, machine);

            try
            {
                doc.Save(mspf);
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 303);
            }
        }

        /// <summary>
        /// Create the specification file
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="em"></param>
        private void CreateSpecificationFile(XmlDocument doc, XmlElement em)
        {
            if (_viewModel == null) return;

            var sname = new string[4] { "x", "y", "z", "a" };
            var paras = new List<int[]>();

            // X axis
            int xsign = _xhomedir == 0 ? 1 : -1;
            paras.Add(new int[] { _viewModel.XDirection, xsign, _viewModel.XPulse, _Xstate[1], _Xstate[0] });

            // Y axis
            int ysign = _yhomedir == 0 ? 1 : -1;
            paras.Add(new int[] { _viewModel.YDirection, ysign, _viewModel.YPulse, _Ystate[1], _Ystate[0] });

            // Z axis
            int zsign = _zhomedir == 0 ? 1 : -1;
            paras.Add(new int[] { _viewModel.ZDirection, zsign, _viewModel.ZPulse, _Zstate[1], _Zstate[0] });

            // A axis
            int astate;
            if (_viewModel.AaxisBool) astate = 1;
            else astate = 0;

            paras.Add(new int[] { _viewModel.XDirection, astate, _viewModel.APulse, 0, 0 });

            for (int i = 0; i < 4; i++)
                AddXmlXaxis(doc, em, sname[i], paras[i]);

            // IO ports
            var trigger = doc.CreateElement("trigger");
            trigger.InnerText = _viewModel.PedalTrigger.ToString();
            em.AppendChild(trigger);

            var air = doc.CreateElement("air");
            air.InnerText = _viewModel.ProtectedAir.ToString();
            em.AppendChild(air);

            var wobble = doc.CreateElement("wobble");
            wobble.InnerText = _viewModel.Wobble.ToString();
            em.AppendChild(wobble);

            var enable = doc.CreateElement("enable");
            enable.InnerText = _viewModel.LaserEnable.ToString();
            em.AppendChild(enable);

            var wireswitch = doc.CreateElement("wireswitch");
            wireswitch.InnerText = _viewModel.FeedWire.ToString() + "," + _viewModel.Withdraw.ToString();
            em.AppendChild(wireswitch);

            var wiredac = doc.CreateElement("wiredac");
            wiredac.InnerText = _viewModel.WireDac.ToString();
            em.AppendChild(wiredac);
        }

        /// <summary>
        /// Add axis parameters to the axis block
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="em"></param>
        /// <param name="name"></param>
        /// <param name="paras"></param>
        private void AddXmlXaxis(XmlDocument doc, XmlElement em, string name, int[] paras)
        {
            var axis = doc.CreateElement("axis");
            var attr = doc.CreateAttribute("name");
            attr.Value = name;
            axis.SetAttributeNode(attr);
            em.AppendChild(axis);

            var dir = doc.CreateElement("direction");
            dir.InnerText = paras[0].ToString();
            axis.AppendChild(dir);

            if (name == "a")
            {
                var state = doc.CreateElement("state");
                state.InnerText = paras[1].ToString();
                axis.AppendChild(state);
            }
            else
            {
                var home = doc.CreateElement("home");
                home.InnerText = paras[1].ToString();
                axis.AppendChild(home);
            }

            var pitch = doc.CreateElement("pitch");
            pitch.InnerText = paras[2].ToString();
            axis.AppendChild(pitch);

            var pos = doc.CreateElement("plimit");
            pos.InnerText = paras[3].ToString();
            axis.AppendChild(pos);

            var neg = doc.CreateElement("nlimit");
            neg.InnerText = paras[4].ToString();
            axis.AppendChild(neg);
        }
    }
}
