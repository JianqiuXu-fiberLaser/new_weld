///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Enhancable material speciications
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.Welding
{
    /// <summary>
    /// the core parameter of materail to weldding, for stored and calculated.
    /// </summary>
    public class MetalMaterial
    {
        private int _materialIndex;
        private double _sheetThickness;

        public MetalMaterial(int mi = 0, double tk = 0)
        {
            _materialIndex = mi;
            _sheetThickness = tk;
        }


        //-- get and set properties --
        public int MaterialType
        {
            get => _materialIndex;
            set => _materialIndex = value;
        }

        public double Thickness
        {
            get => _sheetThickness;
            set => _sheetThickness = value;
        }
    }
}
