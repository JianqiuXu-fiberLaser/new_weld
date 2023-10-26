///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.Collections.ObjectModel;

namespace RobotWeld3.Welding
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

        public string GetName(int index)
        {  
            if (index >=0 && index < this.Count)
                return this[index].Name;
            throw new System.IndexOutOfRangeException();
        }
    }

    /// <summary>
    /// Define the name-index pair for material types.
    /// </summary>
    public class MaterialTypePair
    {
        private string _name = string.Empty;
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
