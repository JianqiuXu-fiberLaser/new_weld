using RobotWeld.AlgorithmsBase;
using System.IO;
using System;

namespace RobotWeld.Welding
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

        private int _oldMaterialType;
        private double _oldSheetThickness;
        private double _oldWireDiameter;

        public MetalMaterial() { }

        public void ReadFileInfo()
        {
            string line;
            try
            {
                string fname = "./Stuff/" + FileNumber.ToString() + ".ma";
                FileStream rfile = new(fname, FileMode.OpenOrCreate);
                StreamReader sr = new(rfile);

                line = sr.ReadLine() ?? "1";
                MaterialType = Convert.ToInt32(line);
                _oldMaterialType = MaterialType;

                line = sr.ReadLine() ?? "0";
                Thickness = Convert.ToDouble(line);
                _oldSheetThickness = Thickness;

                line = sr.ReadLine() ?? "0";
                WireDiameter = Convert.ToDouble(line);
                _oldWireDiameter = WireDiameter;

                sr.Close();
            }
            catch (Exception ex)
            {
                new Werr().WaringMessage(ex.Message);
            }
        }

        public void WriteFileInfo()
        {
            try
            {
                DateTime dateTime2020 = new(2020, 2, 14, 0, 0, 0);
                DateTime DateNow = DateTime.Now;
                TimeSpan timespan = (DateNow - dateTime2020);
                FileNumber = (int)timespan.TotalSeconds;

                FileStream afile = new("./Stuff/" +
                    FileNumber.ToString() + ".ma", FileMode.Create);
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

        public void DiscardFileInfo()
        {
            MaterialType = _oldMaterialType;
            Thickness = _oldSheetThickness;
            WireDiameter = _oldWireDiameter;
        }

        //-- get and set properties --
        public int FileNumber
        { 
            get { return _fileNumber; } 
            set { _fileNumber = value; }
        }

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
