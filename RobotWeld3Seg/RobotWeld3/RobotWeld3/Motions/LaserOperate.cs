///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.Threading;

namespace RobotWeld3.Motions
{
    /// <summary>
    /// Thread and methods to operate the laser
    /// </summary>
    internal class LaserOperate
    {
        private int _laserPower;
        private int _frequency;
        private int _duty;
        private int _rise;
        private int _fall;

        private readonly int[] _riseMillis;
        private readonly int[] _fallMillis;
        private static int _maxPower;
        private readonly double[]? SubPowerPercent;

        public LaserOperate()
        {
            _riseMillis = new int[4];
            _fallMillis = new int[4];
            SubPowerPercent = new double[5];
        }

        public static int MaxPower
        {
            get => _maxPower;
            set => _maxPower = value;
        }

        /// <summary>
        /// Set laser parameter to work.
        /// </summary>
        /// <param name="laserPower"> When laser power == 0, </param>
        /// <param name="frequency"></param>
        /// <param name="duty"></param>
        /// <param name="r"> rise </param>
        /// <param name="f"> fall </param>
        internal void SetParameter(int laserPower, int frequency = 0, int duty = 0, int r = 0, int f = 0)
        {
            if (laserPower == 0) return;

            _laserPower = laserPower;
            _frequency = frequency;
            _duty = duty;
            // initial laser driver
            if (_frequency == 0 || _duty == 0)
            {
                LaserDriver.InitialLaser();
                _rise = r;
                _fall = f;
                if (_maxPower > 0) CalSubPower(_laserPower);
            }
            else
            {
                _rise = 0;
                _fall = 0;
                LaserDriver.InitialLaser(_frequency, _duty);
            }
        }

        /// <summary>
        /// Change laser power, rise and fall, not change laser mode.
        /// </summary>
        /// <param name="laserPower"></param>
        /// <param name="r"> rise </param>
        /// <param name="f"> fall </param>
        internal void ChangePower(int laserPower, int r = 0, int f = 0)
        {
            if (laserPower == 0) return;

            _laserPower = laserPower;
            _rise = r;
            _fall = f;
            if (_maxPower > 0) CalSubPower(_laserPower);
        }

        /// <summary>
        /// Setup laser rise and fall edge
        /// </summary>
        /// <param name="r"> rise time </param>
        /// <param name="f"> fall time </param>
        internal void SetEdge(int r = 0, int f = 0)
        {
            if (_laserPower == 0) return;
            _rise = r;
            _fall = f;
            if (_maxPower > 0) CalSubPower(_laserPower);
        }
        //
        // Calculate sub power percentage.
        //
        private void CalSubPower(int lpow)
        {
            if (SubPowerPercent == null || _maxPower == 0) return;

            for (int i = 0; i < 5; i++)
            {
                SubPowerPercent[i] = lpow * 0.2 * (i + 1) / _maxPower;
            }

            for (int j = 0; j < 4; j++)
            {
                _riseMillis[j] = (int)(_rise * 0.25);
                _fallMillis[j] = (int)(_fall * 0.25);
            }
        }

        /// <summary>
        /// Set analog voltage from axis servo
        /// </summary>
        /// <param name="wobble"> wobble voltage </param>
        public void SetWobble(double wobble)
        {
            LaserDriver.SetAnalog(MotionSpecification.WobbleDac - 1, wobble);
        }

        /// <summary>
        /// Switch on laser slowly
        /// </summary>
        internal void SlowLaserOn()
        {
            if (SubPowerPercent == null) return;
            LaserDriver.EnableSW();

            if (_rise > 20)
            {
                for (int i = 0; i < 4; i++)
                {
                    LaserDriver.LaserOn(SubPowerPercent[i]);
                    Thread.Sleep(_riseMillis[i]);
                }
            }

            LaserDriver.LaserOn(SubPowerPercent[4]);
        }

        /// <summary>
        /// Switch off laser slowly
        /// </summary>
        internal void SlowLaserOff()
        {
            if (SubPowerPercent == null) return;

            if (_fall > 20)
            {
                for (int i = 0; i < 4; i++)
                {
                    LaserDriver.LaserOn(SubPowerPercent[3 - i]);
                    Thread.Sleep(_fallMillis[i]);
                }
            }

            LaserDriver.LaserOff();
            LaserDriver.DisableSW();
        }
    }
}
