using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
{
    /// <summary>
    /// Interact with set vane Wheel parameter
    /// </summary>
    public class VaneWheelViewModel : INotifyPropertyChanged
    {
        private double y01Velocity;
        private double c01Velocity;
        private double y01Position;

        private double[] c0Position = new double[7];
        private double[] c1Position = new double[7];

        private double y11Velocity;
        private double c11Velocity;
        private double y11Position;

        private int vaneNumber;
        private int vaneIndex;

        public VaneWheelViewModel() { }

        //
        //---- Properties ----
        // 0 : Left work station
        //
        public double Y01Position
        {
            get { return y01Position; }
            set 
            { 
                y01Position = value;
                OnPropertyChanged(nameof(Y01Position));
            }
        }

        public double Y01Velocity
        {
            get { return y01Velocity; }
            set
            {
                y01Velocity = value;
                OnPropertyChanged(nameof(Y01Velocity));
            }
        }

        public double C01Velocity
        {
            get { return c01Velocity; }
            set
            {
                c01Velocity = value;
                OnPropertyChanged(nameof(C01Velocity));
            }
        }

        public double C01Position
        {
            get { return c0Position[0]; }
            set 
            {
                c0Position[0] = value; 
                OnPropertyChanged(nameof(C01Position));
            }
        }

        public double C02Position
        {
            get { return c0Position[1]; }
            set
            {
                c0Position[1] = value;
                OnPropertyChanged(nameof(C02Position));
            }
        }

        public double C03Position
        {
            get { return c0Position[2]; }
            set
            {
                c0Position[2] = value;
                OnPropertyChanged(nameof(C03Position));
            }
        }

        public double C04Position
        {
            get { return c0Position[3]; }
            set
            {
                c0Position[3] = value;
                OnPropertyChanged(nameof(C04Position));
            }
        }

        public double C05Position
        {
            get { return c0Position[4]; }
            set
            {
                c0Position[4] = value;
                OnPropertyChanged(nameof(C05Position));
            }
        }

        public double C06Position
        {
            get { return c0Position[5]; }
            set
            {
                c0Position[5] = value;
                OnPropertyChanged(nameof(C06Position));
            }
        }

        public double C07Position
        {
            get { return c0Position[6]; }
            set
            {
                c0Position[6] = value;
                OnPropertyChanged(nameof(C07Position));
            }
        }

        //------------------------- 1: Right work station ---------------------------------//
        //
        // 1 : Right work station
        //
        public double C11Velocity
        {
            get { return c11Velocity; }
            set
            {
                c11Velocity = value;
                OnPropertyChanged(nameof(C11Velocity));
            }
        }

        public double Y11Position
        {
            get { return y11Position; }
            set 
            { 
                y11Position = value;
                OnPropertyChanged(nameof(Y11Position));
            }
        }

        public double Y11Velocity
        {
            get { return y11Velocity; }
            set
            {
                y11Velocity = value;
                OnPropertyChanged(nameof(Y11Velocity));
            }
        }

        public double C11Position
        {
            get { return c1Position[0]; }
            set 
            { 
                c1Position[0] = value;
                OnPropertyChanged(nameof(C11Position));
            }
        }

        public double C12Position
        {
            get { return c1Position[1]; }
            set
            {
                c1Position[1] = value;
                OnPropertyChanged(nameof(C12Position));
            }
        }

        public double C13Position
        {
            get { return c1Position[2]; }
            set
            {
                c1Position[2] = value;
                OnPropertyChanged(nameof(C13Position));
            }
        }

        public double C14Position
        {
            get { return c1Position[3]; }
            set
            {
                c1Position[3] = value;
                OnPropertyChanged(nameof(C14Position));
            }
        }

        public double C15Position
        {
            get { return c1Position[4]; }
            set
            {
                c1Position[4] = value;
                OnPropertyChanged(nameof(C15Position));
            }
        }

        public double C16Position
        {
            get { return c1Position[5]; }
            set
            {
                c1Position[5] = value;
                OnPropertyChanged(nameof(C16Position));
            }
        }

        public double C17Position
        {
            get { return c1Position[6]; }
            set
            {
                c1Position[6] = value;
                OnPropertyChanged(nameof(C17Position));
            }
        }

        //
        // Vena specification
        //
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
