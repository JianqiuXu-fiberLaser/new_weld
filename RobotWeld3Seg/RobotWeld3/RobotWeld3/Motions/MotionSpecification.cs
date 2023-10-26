///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) new Xml style file to record machie specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.AlgorithmsBase;
using System;
using System.IO;
using System.Xml;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// The specification of Machine and motion card
    /// </summary>
    public class MotionSpecification
    {
        private const string cfgFile = "./RobotWeld.cfg";

        private static int _xDirection;
        private static int _yDirection;
        private static int _zDirection;
        private static int _aDirection;

        private static int _xHomeDir;
        private static int _yHomeDir;
        private static int _zHomeDir;

        private static double _xmm;
        private static double _ymm;
        private static double _zmm;
        private static double _oneCycle;

        private static int _xneglimit;
        private static int _xposlimit;

        private static int _yneglimit;
        private static int _yposlimit;

        private static int _zneglimit;
        private static int _zposlimit;

        private static int _aaxisState;

        private static int _pedalTrigger;
        private static int _protectedAir;

        private static int _wireDirPos;
        private static int _wireDirNeg;
        private static int _wireDac;

        private static int _laserEnable;
        private static int _wobbleDac;

        public MotionSpecification() { }

        /// <summary>
        /// Read RobotWeld.cfg to obtain the machine sepecifications.
        /// </summary>
        public static void ReadParameter()
        {
            var doc = new XmlDocument();

            try
            {
                doc.Load(cfgFile);
                if (doc.DocumentElement == null) throw new IOException();
                XmlElement root = doc.DocumentElement;
                XmlNode? machine = root.SelectSingleNode("machine");
                if (machine == null) return;

                var trigger = machine.SelectSingleNode("trigger");
                if (trigger != null) _pedalTrigger = Convert.ToInt32(trigger.InnerText);

                var air = machine.SelectSingleNode("air");
                if (air != null) _protectedAir = Convert.ToInt32(air.InnerText);

                var wobble = machine.SelectSingleNode("wobble");
                if (wobble != null) _wobbleDac = Convert.ToInt32(wobble.InnerText);

                var enable = machine.SelectSingleNode("enable");
                if (enable != null) _laserEnable = Convert.ToInt32(enable.InnerText);

                var wireswitch = machine.SelectSingleNode("wireswitch");
                if (wireswitch != null)
                {
                    var w = wireswitch.InnerText;
                    var s = w.Split(",");
                    _wireDirPos = Convert.ToInt32(s[0]);
                    _wireDirNeg = Convert.ToInt32(s[1]);
                }

                var wiredac = machine.SelectSingleNode("wiredac");
                if (wiredac != null) _wireDac= Convert.ToInt32(wiredac.InnerText);

                var axes = machine.SelectNodes("axis");
                if (axes != null)
                {
                    foreach (XmlNode ax in axes) GetAxisXml(ax);
                }
            }
            catch (IOException)
            {
                Assertion.AssertError("设备配置文件不存在", 501);
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 502);
            }
            finally
            {
                // do nothing
            }
        }

        /// <summary>
        /// Analysis each axis specifications
        /// </summary>
        /// <param name="ax"></param>
        private static void GetAxisXml(XmlNode ax)
        {
            if (ax == null || ax.Attributes == null) return;
            var id = ax.Attributes["name"];

            int d = 0;
            int h = 0;
            int p = 0;
            int pl = 0;
            int nl = 0;
            int s = 0;

            var direction = ax.SelectSingleNode("direction");
            if (direction != null) d = Convert.ToInt32(direction.InnerText);

            var home = ax.SelectSingleNode("home");
            if (home != null) h = Convert.ToInt32(home.InnerText);

            var pitch = ax.SelectSingleNode("pitch");
            if (pitch != null) p = Convert.ToInt32(pitch.InnerText);

            var plimit = ax.SelectSingleNode("plimit");
            if (plimit != null) pl = Convert.ToInt32(plimit.InnerText);

            var nlimit = ax.SelectSingleNode("nlimit");
            if (nlimit != null) nl = Convert.ToInt32(nlimit.InnerText);

            var state = ax.SelectSingleNode("state");
            if (state != null) s = Convert.ToInt32(state.InnerText);

            SetParameters(id, d, h, p, pl, nl, s);
        }

        /// <summary>
        /// Set specification parameter
        /// </summary>
        /// <param name="id"> axis id </param>
        /// <param name="d"> direction </param>
        /// <param name="h"> home direction </param>
        /// <param name="p"> pitch = pulse / mm </param>
        /// <param name="pl"> positive limit </param>
        /// <param name="nl"> negative limit </param>
        /// <param name="s"> A axis state </param>
        private static void SetParameters(XmlAttribute? id, int d, int h, int p, int pl, int nl, int s)
        {
            try
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                switch (id.Value)
                {
                    case "x":
                        _xDirection = d;
                        _xHomeDir = h;
                        _xmm = p;
                        MotionOperate.Xmillimeter = _xmm;
                        _xposlimit = pl;
                        _xneglimit = nl;
                        break;
                    case "y":
                        _yDirection = d;
                        _yHomeDir = h;
                        _ymm = p;
                        MotionOperate.Ymillimeter = _ymm;
                        _yposlimit = pl;
                        _yneglimit = nl;
                        break;
                    case "z":
                        _zDirection = d;
                        _zHomeDir = h;
                        _zmm = p;
                        MotionOperate.Zmillimeter = _zmm;
                        _zposlimit = pl;
                        _zneglimit = nl;
                        break;
                    case "a":
                        _aDirection = d;
                        _aaxisState = s;
                        _oneCycle = p;
                        MotionOperate.OneCycle = _oneCycle;
                        break;
                    default:
                        break;
                }

            }
            catch (ArgumentNullException ex)
            {
                Assertion.AssertError(ex.Message, 503);
            }
            finally
            {
                // do nothing
            }
        }

        public static int XDirection
        {
            get => _xDirection;
        }

        public static int YDirection
        {
            get => _yDirection;
        }

        public static int ZDirection
        {
            get => _zDirection;
        }

        public static int ADirection
        {
            get => _aDirection;
        }

        public static int XHomeDir
        {
            get => _xHomeDir;
        }

        public static int YHomeDir
        {
            get => _yHomeDir;
        }

        public static int ZHomeDir
        {
            get => _zHomeDir;
            internal set => _zHomeDir = value;
        }

        public static int XNegLimit
        {
            get => _xneglimit;
        }

        public static int XPosLimit
        {
            get => _xposlimit;
        }

        public static int YNegLimit
        {
            get => _yneglimit;
        }

        public static int YPosLimit
        {
            get => _yposlimit;
        }

        public static int ZNegLimit
        {
            get => _zneglimit;
        }

        public static int ZPosLimit
        {
            get => _zposlimit;
        }

        public static int AaxisState
        {
            get => _aaxisState;
        }

        public static int PedalTrigger
        {
            get => _pedalTrigger;
        }

        public static int ProtectedAir
        {
            get => _protectedAir;
        }

        public static int LaserEnable
        {
            get => _laserEnable;
        }

        public static int WobbleDac
        {
            get => _wobbleDac;
        }

        public static int FeedWire
        {
            get => _wireDirPos;
        }

        public static int Withdraw
        {
            get => _wireDirNeg;
        }

        public static int WireDac
        {
            get => _wireDac;
        }
    }
}
