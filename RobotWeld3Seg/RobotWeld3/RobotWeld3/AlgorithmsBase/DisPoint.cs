///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 2.2: (1) new laser weld parameter from the server or local
// Ver. 3.0: (1) add 2 xml file to store points' information and using
//               5 data structures to represent moving, laser and
//               material specifications.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.Motions;
using RobotWeld3.Welding;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Display point
    /// </summary>
    internal class DisPoint
    {
        private int _materialIndex;
        private double _thick;
        private double _wire;

        private Vector _vector;
        private int _linetype;
        private LaserDisplayParameter _parameter;

        public DisPoint()
        {
            _parameter ??= new LaserDisplayParameter();
            _vector ??= new Vector();
        }

        public DisPoint(DisPoint dp)
        {
            _parameter = new LaserDisplayParameter(dp.Parameter);
            _vector ??= new Vector(dp.Vector);
            _wire = dp.Wire;
            _thick = dp.Thick;
            _linetype = dp.Linetype;
        }

        public DisPoint(int p, int f, int d, int m, double t, double w,
            double sp, double wb, Vector vc, int l)
        {
            _parameter ??= new();
            _parameter.Power = p;
            _parameter.Frequency = f;
            _parameter.Duty = d;
            _materialIndex = m;
            _parameter.Speed = sp;
            _parameter.Wobble = wb;
            _wire = w;
            _thick = t;
            _vector = new Vector(vc);
            _linetype = l;
        }

        public DisPoint (LaserDisplayParameter lp, Vector vc, int l)
        {
            _parameter = lp;
            _vector = vc;
            _linetype = l;
        }

        public void ModifyLaser(LaserDisplayParameter lp)
        {
            _parameter = lp;
        }

        public int Power
        {
            get => _parameter.Power;
            set => _parameter.Power = value;
        }

        public int Frequency
        {
            get => _parameter.Frequency;
            set => _parameter.Frequency = value;
        }

        public int Duty
        {
            get => _parameter.Duty;
            set => _parameter.Duty = value;
        }

        public int MaterialIndex
        {
            get => _materialIndex;
            set => _materialIndex = value;
        }

        public double Thick
        {
            get => _thick;
            set => _thick = value;
        }

        public double Wire
        {
            get => _wire;
            set => _wire = value;
        }

        public double Speed
        {
            get => _parameter.Speed;
            set => _parameter.Speed = value;
        }

        public double Wobble
        {
            get => _parameter.Wobble;
            set => _parameter.Wobble = value;
        }

        public LaserDisplayParameter Parameter
        {
            get => _parameter ?? new();
            set => _parameter = value;
        }

        public Vector Vector
        {
            get => _vector ?? new();
            set => _vector = value;
        }

        public int Linetype
        {
            get => _linetype;
            set => _linetype = value;
        }

        /// <summary>
        /// The X position in mm.
        /// </summary>
        public double XCoordinate
        {
            get => _vector.X / MotionOperate.Xmillimeter;
            set => _vector.X = value * MotionOperate.Xmillimeter;
        }

        /// <summary>
        /// The Y position in mm.
        /// </summary>
        public double YCoordinate
        {
            get => _vector.Y / MotionOperate.Ymillimeter;
            set => _vector.Y = value * MotionOperate.Ymillimeter;
        }

        /// <summary>
        /// The Z position in mm.
        /// </summary>
        public double ZCoordinate
        {
            get => _vector.Z / MotionOperate.Zmillimeter;
            set => _vector.Z = value * MotionOperate.Zmillimeter;
        }

        /// <summary>
        /// The rotate angle in degree.
        /// </summary>
        public double ACoordinate
        {
            get => _vector.A * 360.0 / MotionOperate.OneCycle;
            set => _vector.A = value * MotionOperate.OneCycle / 360.0;
        }
    }
}
