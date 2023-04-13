using System.Collections.ObjectModel;

namespace RobotWeld.Weldding
{
    /// <summary>
    /// The list of material types
    /// </summary>
    public class MaterialList : ObservableCollection<MaterialTypePair>
    {
        public MaterialList() : base()
        {
            this.Add(new MaterialTypePair(0, "碳钢"));
            this.Add(new MaterialTypePair(1, "不锈钢"));
            this.Add(new MaterialTypePair(2, "铸铁"));
            this.Add(new MaterialTypePair(3, "铝合金"));
            this.Add(new MaterialTypePair(4, "黄铜"));
            this.Add(new MaterialTypePair(5, "镀锌板"));
        }
    }

    /// <summary>
    /// Define the name-index pair for material types.
    /// </summary>
    public class MaterialTypePair
    {
        private string _name = "";
        private int _index;

        public MaterialTypePair(int index, string name)
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
