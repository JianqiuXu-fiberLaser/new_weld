using System.Collections.ObjectModel;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// The laser type list
    /// </summary>
    public class LaserTypeList : ObservableCollection<LaserTypePair>
    {
        public LaserTypeList() : base()
        {
            this.Add(new LaserTypePair(0, "创鑫"));
            this.Add(new LaserTypePair(1, "锐科"));
            this.Add(new LaserTypePair(2, "杰普特"));
            this.Add(new LaserTypePair(3, "凯普林"));
            this.Add(new LaserTypePair(4, "IPG"));
        }

        public string GetName(int index)
        {
            if (index >= 0 && index < this.Count)
                return this[index].Name;
            throw new System.IndexOutOfRangeException();
        }
    }

    /// <summary>
    /// Define the name-index pair for Laser types.
    /// </summary>
    public class LaserTypePair
    {
        private string _name = string.Empty;
        private int _index;

        public LaserTypePair(int index, string name)
        {
            _index = index;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
    }
}
