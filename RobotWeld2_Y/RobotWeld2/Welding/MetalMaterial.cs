using RobotWeld2.AlgorithmsBase;
using System.IO;
using System;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// the core parameter of materail to weldding, for display and calculation
    /// </summary>

    public class MetalMaterial
    {
        private int _fileNumber;
        private int _materialType;
        private double _sheetThickness;
        private double _wireDiameter;
        private double _weldSpeed;
        private double _wobbleSpeed;

        public MetalMaterial()
        {
            _fileNumber = 0;
            _materialType = 0;
            _sheetThickness = 0;
            _wireDiameter = 0;
            _weldSpeed = 0;
            _wobbleSpeed = 0;
        }

        public MetalMaterial(DaemonFile dmf)
        {
            _fileNumber = dmf.FileIndex;
            _materialType = dmf.MaterialIndex;
            _sheetThickness = dmf.SheetThickness;
            _wireDiameter = dmf.WireDiameter;
            _weldSpeed = dmf.WeldSpeed;
            _wobbleSpeed = dmf.WobbleSpeed;
        }

        /// <summary>
        /// read material parameters from file
        /// </summary>
        public void ReadFileInfo()
        {
            string line;
            try
            {
                string fname = "./Stuff/" + _fileNumber.ToString() + ".mat";
                FileStream rfile = new(fname, FileMode.OpenOrCreate);
                StreamReader sr = new(rfile);

                line = sr.ReadLine() ?? "1";
                MaterialType = Convert.ToInt32(line);

                line = sr.ReadLine() ?? "0";
                Thickness = Convert.ToDouble(line);

                line = sr.ReadLine() ?? "0";
                WireDiameter = Convert.ToDouble(line);

                sr.Close();
            }
            catch (Exception ex)
            {
                new Werr().WaringMessage(ex.Message);
            }
        }

        /// <summary>
        /// Write the material parameter to the filew
        /// </summary>
        public void WriteFileInfo()
        {
            try
            {
                DateTime dateTime2020 = new(2020, 2, 14, 0, 0, 0);
                DateTime DateNow = DateTime.Now;
                TimeSpan timespan = (DateNow - dateTime2020);
                _fileNumber = (int)timespan.TotalSeconds;

                FileStream afile = new("./Stuff/" +
                    _fileNumber.ToString() + ".mat", FileMode.Create);
                StreamWriter sw = new(afile);

                sw.WriteLine(MaterialType.ToString());
                sw.WriteLine(Thickness.ToString());
                sw.WriteLine(WireDiameter.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                new Werr().WaringMessage(ex.Message);
            }
        }

        //-- get and set properties --
        public int MaterialType
        {
            get { return _materialType; }
            set { _materialType = value; }
        }

        public double Thickness
        {
            get { return _sheetThickness; }
            set { _sheetThickness = value; }
        }

        public double WireDiameter
        {
            get { return _wireDiameter; }
            set { _wireDiameter = value; }
        }

        public double WeldSpeed
        {
            get { return _weldSpeed; }
            set
            {
                if (value < 0) value = 0;
                _weldSpeed = value;
            }
        }

        public double WobbleSpeed
        {
            get { return _wobbleSpeed; }
            set
            {
                if (value < 0 && value > 10000) value = 0;
                _wobbleSpeed = value;
            }
        }
    }
}
