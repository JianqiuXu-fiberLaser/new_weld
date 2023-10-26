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

using RobotWeld3.AlgorithmsBase;
using RobotWeld3.Motions;
using System.IO;

namespace RobotWeld3.Welding
{
    /// <summary>
    /// Welding model to calculate the laser parameters in Wms.
    /// </summary>
    internal class WeldingModel
    {
        private LaserCoefficient? _coeff;

        public WeldingModel() { }

        /// <summary>
        /// Read calcualate coefficients from the disk or the web server.
        /// </summary>
        internal void ReadCoefficient(int timestamp)
        {
            string fname = "./Worktime/" + "default.pam";
            string tname = "./Worktime/" + timestamp.ToString() + ".pam";
            _coeff = new LaserCoefficient();

            if (File.Exists(tname))
                AccessParameterFile.Read(tname, ref _coeff);
            else
                AccessParameterFile.Read(fname, ref _coeff);
        }

        /// <summary>
        /// calculate rise time
        /// </summary>
        /// <returns></returns>
        internal double CalculateRise(MetalMaterial mi, Wire wi, DisPoint dp)
        {
            // When it has wire, no rise needed.
            // no power, no rise
            if (wi.Diameter != 0 || dp.Power == 0) return 0.0;

            if (_coeff == null) return 0.0;
            double rise = _coeff.RiseCoef * MotionOperate.Xmillimeter / dp.Speed;
            return rise;
        }

        /// <summary>
        /// Calcualte fall time
        /// </summary>
        /// <returns></returns>
        internal double CalculateFall(MetalMaterial mi, Wire wi, DisPoint dp)
        {
            // When it has wire, no fall needed.
            // no power, no fall
            if (wi.Diameter != 0 || dp.Power == 0) return 0.0;

            if (_coeff == null) return 0.0;
            double fall = _coeff.FallCoef * MotionOperate.Xmillimeter / dp.Speed;
            return fall;
        }

        /// <summary>
        /// Calcualte time for air in and out.
        /// </summary>
        /// <returns></returns>
        internal double CalculateAir(MetalMaterial mi, Wire wi, DisPoint dp)
        {
            if (_coeff == null) return 500;
            else return _coeff.AirCoef * 100;
        }

        /// <summary>
        /// Calculate feed, withdraw, and re-feed wire parameters.
        /// </summary>
        /// <param name="mi"> Metal Material of base </param>
        /// <param name="wi"> wire parameter </param>
        /// <param name="dp"> display point </param>
        /// <returns> tuple (backlength, feed speed, re-feed length, burning-time) </returns>
        internal (double back, double speed, double length, double time) CalculateWire(MetalMaterial mi, Wire wi, DisPoint dp)
        {
            // without wire, no need to consider.
            // no power, stop wire always.
            if (wi.Diameter == 0 || dp.Power == 0) return (0, 0, 0, 0);

            if (_coeff == null) return (0, 0, 0, 0);
            double back = _coeff.Wireback * 2.0 * wi.Diameter;
            double speed = _coeff.WireSpeed * dp.Speed * 1.5;
            double length = 1.1 * 2.0 * wi.Diameter * _coeff.RefeedLength;
            
            double time = back / _coeff.WithdrawSpeed;

            return (back, speed, length, time);
        }
    }
}
