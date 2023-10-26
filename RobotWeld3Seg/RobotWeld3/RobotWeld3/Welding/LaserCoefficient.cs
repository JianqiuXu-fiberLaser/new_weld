///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Primary welding model to determine laser paremeter.
//
///////////////////////////////////////////////////////////////////////

namespace RobotWeld3.Welding
{
    /// <summary>
    /// The coefficients to calculate laser parameters.
    /// </summary>
    public class LaserCoefficient
    {
        private double _airCoef;
        private double _riseCoef;
        private double _fallCoef;

        private double _wireback;
        private double _refeedLength;
        private double _wireSpeed;
        private double _withdrawSpeed;

        public LaserCoefficient() { }

        /// <summary>
        /// Coefficient for air-in and out.
        /// </summary>
        public double AirCoef
        {
            get => _airCoef;
            set => _airCoef = value;
        }

        /// <summary>
        /// Coefficient for laser rise edge.
        /// </summary>
        public double RiseCoef
        {
            get => _riseCoef;
            set => _riseCoef = value;
        }

        /// <summary>
        /// Coefficient for laser fall edge.
        /// </summary>
        public double FallCoef
        {
            get => _fallCoef;
            set => _fallCoef = value;
        }

        /// <summary>
        /// withdraw length when stop the laser.
        /// </summary>
        public double Wireback
        { 
            get => _wireback;
            set => _wireback = value;
        }

        public double RefeedLength
        {
            get => _refeedLength;
            set => _refeedLength = value;
        }

        /// <summary>
        /// Feed speed of wire
        /// </summary>
        public double WireSpeed
        {
            get => _wireSpeed;
            set => _wireSpeed = value;
        }

        /// <summary>
        /// Withdraw speed of wire
        /// </summary>
        public double WithdrawSpeed
        {
            get => _withdrawSpeed;
            set => _withdrawSpeed = value;
        }
    }
}
