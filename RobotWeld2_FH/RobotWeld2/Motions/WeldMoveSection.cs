using System.Collections.Generic;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// Move section for welding in one action
    /// </summary>
    public class WeldMoveSection
    {
        private readonly List<int[]> _position;
        private readonly List<double> _speed;
        private Dictionary<int, LaserArgument>? _laserArgument;
        private int _moveType;

        public WeldMoveSection(List<int[]> secpts, List<double> sp)
        {
            _position = secpts;
            _speed = sp;
        }

        public double Speed
        {
            get => _speed[0];
        }

        public Dictionary<int, LaserArgument> Argument
        {
            get
            {
                if (_laserArgument == null)
                    return new Dictionary<int, LaserArgument>();
                else
                    return _laserArgument;
            }

            set { _laserArgument = value; }
        }

        public int MoveType
        {
            get => _moveType;
            set { _moveType = value; }
        }

        public int[] GetFirstPosition()
        {
            return _position[0];
        }

        public int GetPointCount()
        {
            return _position.Count;
        }

        public int GetLaserPower()
        {
            if (_laserArgument != null)
            {
                return _laserArgument[0].Power;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Get the laser argument of Dictionary
        /// </summary>
        /// <param name="index"> the key: the index in this section </param>
        /// <returns> the argument </returns>
        public LaserArgument? GetParameter(int index)
        {
            if (_laserArgument != null && _laserArgument.ContainsKey(index))
            {
                return _laserArgument[index];
            }
            else
            {
                return null;
            }
        }

        public List<int[]> GetPosition()
        {
            return _position;
        }

        public double GetSpeed(int index)
        {
            if (_speed != null && index < _speed.Count)
            {
                return _speed[index];
            }
            else
            {
                return 0;
            }
        }

        public List<double> GetSpeedList()
        {
            return _speed;
        }
    }
}
