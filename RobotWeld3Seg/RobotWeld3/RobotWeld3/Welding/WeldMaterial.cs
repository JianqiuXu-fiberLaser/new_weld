///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Simple code indicates the point's file
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.Welding
{
    /// <summary>
    /// The material used in welding, includes base and wire material
    /// </summary>
    public class WeldMaterial
    {
        private int _materialType;
        private double _sheetThickness;
        private double _wireDiameter;
        private double _wireSpeed;
        private double _preLength;
        private double _backLength;

        public WeldMaterial() { }

        public int MaterialType 
        { 
            get { return _materialType; } 
            set { _materialType = value; } 
        }

        public double SheetThickness
        {
            get => _sheetThickness; 
            set { _sheetThickness = value; }
        }

        public double WireDiameter
        {
            get => _wireDiameter;
            set { _wireDiameter = value; }
        }

        public double WireSpeed
        {
            get => _wireSpeed;
            set { _wireSpeed = value; }
        }

        public double PreLength
        {
            get => _preLength; set 
            { _preLength = value; }
        }

        public double BackLength
        {
            get => _backLength;
            set { _backLength = value; }
        }
    }
}
