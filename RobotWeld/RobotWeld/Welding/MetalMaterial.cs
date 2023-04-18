namespace RobotWeld.Weldding
{
    /// <summary>
    /// the core parameter of materail to weldding, for display and calculation
    /// </summary>

    public class MetalMaterial
    {
        private int _materialType = 0;
        private double _sheetThickness = 0;
        private double _wireDiameter = 0;
        private double _weldSpeed = 0;
        private double _wobbleSpeed = 0;

        public MetalMaterial() 
        { 
            _materialType = 0;
            _sheetThickness = 0;
            _wireDiameter = 0;
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
