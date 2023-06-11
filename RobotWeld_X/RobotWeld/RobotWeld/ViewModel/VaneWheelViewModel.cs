using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld.ViewModel
{
    /// <summary>
    /// Interact with set vane Wheel parameter
    /// </summary>
    public class VaneWheelViewModel : INotifyPropertyChanged
    {
        private double y1Position;
        private double y1Velocity;
        private double c1Position; 
        private double c1Velocity;

        private double y2Position;
        private double y2Velocity;
        private double c2Position;
        private double c2Velocity;

        private int vaneNumber;
        private int vaneIndex;

        public VaneWheelViewModel() { }

        //---- Properties ----
        public double Y1Position
        {
            get { return y1Position; }
            set 
            { 
                y1Position = value;
                OnPropertyChanged(nameof(Y1Position));
            }
        }

        public double Y1Velocity
        {
            get { return y1Velocity; }
            set
            {
                y1Velocity = value;
                OnPropertyChanged(nameof(Y1Velocity));
            }
        }

        public double C1Position
        {
            get { return c1Position; }
            set 
            { 
                c1Position = value; 
                OnPropertyChanged(nameof(C1Position));
            }
        }

        public double C1Velocity
        {
            get { return c1Velocity; }
            set
            {
                c1Velocity = value;
                OnPropertyChanged(nameof(C1Velocity));
            }
        }

        public double Y2Position
        {
            get { return y2Position; }
            set 
            { 
                y2Position = value;
                OnPropertyChanged(nameof(Y2Position));
            }
        }

        public double Y2Velocity
        {
            get { return y2Velocity; }
            set
            {
                y2Velocity = value;
                OnPropertyChanged(nameof(Y2Velocity));
            }
        }

        public double C2Position
        {
            get { return c2Position; }
            set 
            { 
                c2Position = value;
                OnPropertyChanged(nameof(C2Position));
            }
        }

        public double C2Velocity
        {
            get { return c2Velocity; }
            set
            {
                c2Velocity = value;
                OnPropertyChanged(nameof(C2Velocity));
            }
        }

        public int VaneNumber
        {
            get { return vaneNumber; }
            set 
            { 
                if (value == 7 || value == 6)
                    vaneNumber = value; 
                else 
                    vaneNumber = 7;

                OnPropertyChanged(nameof(VaneNumber));
            }
        }

        public int VaneIndex
        {
            get { return vaneIndex; }
            set { vaneIndex = value; OnPropertyChanged(nameof(VaneIndex)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class VaneTypeList : ObservableCollection<VaneType>
    {
        public VaneTypeList() : base()
        {
            this.Add(new VaneType(1, "15T"));
            this.Add(new VaneType(2, "20T"));
        }
    }

    public class VaneType
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public VaneType(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}
