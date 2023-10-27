using RobotWeld2.AlgorithmsBase;
using System;
using System.IO;

namespace RobotWeld2.Motions
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

        private static int _xHomeDir;
        private static int _yHomeDir;
        private static int _zHomeDir;

        private double _xmm;
        private double _ymm;
        private double _zmm;
        private double _oneCycle;

        public MotionSpecification() { }

        public void ReadParameter()
        {
            int i = 0;
            string[] lines = new string[5];
            try
            {
                FileStream afile = new(cfgFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new(afile);

                string? line = sr.ReadLine();
                while (line != null && i < 5)
                {
                    lines[i] = line;
                    i++;
                    line = sr.ReadLine();
                }

                sr.Close();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 501);
            }
            finally
            {
                // X axis
                string[] svar = lines[1].Split(',');
                _xDirection = Convert.ToInt32(svar[1]);
                _xHomeDir = Convert.ToInt32(svar[2]);
                _xmm = Convert.ToDouble(svar[3]);
                MotionOperate.Xmillimeter = _xmm;

                // y axis
                svar = lines[2].Split(',');
                _yDirection = Convert.ToInt32(svar[1]);
                _yHomeDir = Convert.ToInt32(svar[2]);
                _ymm = Convert.ToDouble(svar[3]);
                MotionOperate.Ymillimeter = _ymm;

                // z axis
                svar = lines[3].Split(',');
                _zDirection = Convert.ToInt32(svar[1]);
                _zHomeDir = Convert.ToInt32(svar[2]);
                _zmm = Convert.ToDouble(svar[3]);
                MotionOperate.Zmillimeter = _zmm;

                // A axis
                svar = lines[4].Split(",");
                _oneCycle = Convert.ToInt32(svar[3]);
                MotionOperate.OneCycle = _oneCycle;
            }
        }

        public static int XDirection
        {
            get => _xDirection;
            internal set => _xDirection = value;
        }

        public static int YDirection
        {
            get => _yDirection;
            internal set => _yDirection = value;
        }

        public static int ZDirection
        {
            get => _zDirection;
            internal set => _zDirection = value;
        }

        public static int XHomeDir
        {
            get => _xHomeDir;
            internal set => _xHomeDir = value;
        }

        public static int YHomeDir
        {
            get => _yHomeDir;
            internal set => _yHomeDir = value;
        }

        public static int ZHomeDir
        {
            get => _zHomeDir;
            internal set => _zHomeDir = value;
        }
    }
}
