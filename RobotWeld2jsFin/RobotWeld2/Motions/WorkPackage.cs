using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Welding;

namespace RobotWeld2.Motions
{
    /// <summary>
    /// All data for one welding works. 
    /// </summary>
    public class WorkPackage
    {
        private Tracetype tracetype;
        private int typeIndex;
        private MetalMaterial metalMaterial;
        private int materialIndex;
        private PointListData pointList;

        public WorkPackage()
        {
            metalMaterial = new MetalMaterial();
            pointList = new PointListData();
        }

        public void AddPoint(Point point)
        {
            pointList.Add(point);
        }

        public void Clear()
        {
            metalMaterial = new();
            pointList.Clear();
        }

        public Tracetype TraceType
        {
            get { return tracetype; }
            set { tracetype = value; }
        }

        public int TypeIndex
        {
            get { return typeIndex; }
            set { typeIndex = value; }
        }

        public MetalMaterial MetalMaterial
        {
            get { return metalMaterial; }
            set { metalMaterial = value; }
        }

        public int MaterialIndex
        {
            get { return materialIndex; }
            set { materialIndex = value; }
        }

        /// <summary>
        /// Get the ref of PointListData
        /// </summary>
        /// <returns> pointList </returns>
        public PointListData GetPointList()
        {
            return pointList;
        }

        public void SetPointList(PointListData cypoints)
        {
            this.pointList.Clear();
            for (int i = 0; i < cypoints.GetCount(); i++)
            {
                this.pointList.Add(cypoints.GetPoint(i));
            }
        }

        public void ClearPointList()
        {
            this.pointList.Clear();
        }
    }
}
