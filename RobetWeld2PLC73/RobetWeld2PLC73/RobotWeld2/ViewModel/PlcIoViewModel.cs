using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
{
    public class PlcIoViewModel : INotifyPropertyChanged
    {
        //
        // PLC parameters
        //
        private bool[] _isAlarmX;
        private bool[] _isAlarmY; 

        public PlcIoViewModel()
        {
            _isAlarmX = new bool[41];
            _isAlarmY = new bool[37];
        }

        //
        //-- Response for the ICommand Events in the MainWindow --
        // required by INotifPropertyChanged interface, regist in the first
        //
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //
        // PLC properties
        //
        public bool IsAlarmX1
        {
            get { return _isAlarmX[0]; }
            set { _isAlarmX[0] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX2
        {
            get { return _isAlarmX[1]; }
            set { _isAlarmX[1] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX3
        {
            get { return _isAlarmX[2]; }
            set { _isAlarmX[2] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX4
        {
            get { return _isAlarmX[3]; }
            set { _isAlarmX[3] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX5
        {
            get { return _isAlarmX[4]; }
            set { _isAlarmX[4] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX6
        {
            get { return _isAlarmX[5]; }
            set { _isAlarmX[5] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX7
        {
            get { return _isAlarmX[0]; }
            set { _isAlarmX[0] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX8
        {
            get { return _isAlarmX[7]; }
            set { _isAlarmX[7] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX9
        {
            get { return _isAlarmX[8]; }
            set { _isAlarmX[8] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX10
        {
            get { return _isAlarmX[9]; }
            set { _isAlarmX[9] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX11
        {
            get { return _isAlarmX[10]; }
            set { _isAlarmX[10] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX12
        {
            get { return _isAlarmX[11]; }
            set { _isAlarmX[11] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX13
        {
            get { return _isAlarmX[12]; }
            set { _isAlarmX[12] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX14
        {
            get { return _isAlarmX[13]; }
            set { _isAlarmX[13] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX15
        {
            get { return _isAlarmX[14]; }
            set { _isAlarmX[14] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX16
        {
            get { return _isAlarmX[15]; }
            set { _isAlarmX[15] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX17
        {
            get { return _isAlarmX[16]; }
            set { _isAlarmX[16] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX18
        {
            get { return _isAlarmX[17]; }
            set { _isAlarmX[17] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX19
        {
            get { return _isAlarmX[18]; }
            set { _isAlarmX[18] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX20
        {
            get { return _isAlarmX[19]; }
            set { _isAlarmX[19] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX21
        {
            get { return _isAlarmX[20]; }
            set { _isAlarmX[20] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX22
        {
            get { return _isAlarmX[21]; }
            set { _isAlarmX[21] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX23
        {
            get { return _isAlarmX[22]; }
            set { _isAlarmX[22] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX24
        {
            get { return _isAlarmX[23]; }
            set { _isAlarmX[23] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX25
        {
            get { return _isAlarmX[24]; }
            set { _isAlarmX[24] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX26
        {
            get { return _isAlarmX[25]; }
            set { _isAlarmX[25] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX27
        {
            get { return _isAlarmX[26]; }
            set { _isAlarmX[26] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX28
        {
            get { return _isAlarmX[27]; }
            set { _isAlarmX[27] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX29
        {
            get { return _isAlarmX[28]; }
            set { _isAlarmX[28] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX30
        {
            get { return _isAlarmX[29]; }
            set { _isAlarmX[29] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX31
        {
            get { return _isAlarmX[30]; }
            set { _isAlarmX[30] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX32
        {
            get { return _isAlarmX[31]; }
            set { _isAlarmX[31] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX33
        {
            get { return _isAlarmX[32]; }
            set { _isAlarmX[32] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX34
        {
            get { return _isAlarmX[33]; }
            set { _isAlarmX[33] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX35
        {
            get { return _isAlarmX[34]; }
            set { _isAlarmX[34] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX36
        {
            get { return _isAlarmX[35]; }
            set { _isAlarmX[35] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX37
        {
            get { return _isAlarmX[36]; }
            set { _isAlarmX[36] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX38
        {
            get { return _isAlarmX[37]; }
            set { _isAlarmX[37] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX39
        {
            get { return _isAlarmX[38]; }
            set { _isAlarmX[38] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX40
        {
            get { return _isAlarmX[39]; }
            set { _isAlarmX[39] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmX41
        {
            get { return _isAlarmX[40]; }
            set { _isAlarmX[40] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY1
        {
            get { return _isAlarmY[0]; }
            set { _isAlarmY[0] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY2
        {
            get { return _isAlarmY[1]; }
            set { _isAlarmY[1] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY3
        {
            get { return _isAlarmY[2]; }
            set { _isAlarmY[2] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY4
        {
            get { return _isAlarmY[3]; }
            set { _isAlarmY[3] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY5
        {
            get { return _isAlarmY[5]; }
            set { _isAlarmY[5] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY6
        {
            get { return _isAlarmY[0]; }
            set { _isAlarmY[0] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY7
        {
            get { return _isAlarmY[6]; }
            set { _isAlarmY[6] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY8
        {
            get { return _isAlarmY[7]; }
            set { _isAlarmY[7] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY9
        {
            get { return _isAlarmY[8]; }
            set { _isAlarmY[8] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY10
        {
            get { return _isAlarmY[9]; }
            set { _isAlarmY[9] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY11
        {
            get { return _isAlarmY[10]; }
            set { _isAlarmY[10] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY12
        {
            get { return _isAlarmY[12]; }
            set { _isAlarmY[12] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY13
        {
            get { return _isAlarmY[13]; }
            set { _isAlarmY[13] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY14
        {
            get { return _isAlarmY[14]; }
            set { _isAlarmY[14] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY15
        {
            get { return _isAlarmY[15]; }
            set { _isAlarmY[15] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY16
        {
            get { return _isAlarmY[16]; }
            set { _isAlarmY[16] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY17
        {
            get { return _isAlarmY[17]; }
            set { _isAlarmY[17] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY18
        {
            get { return _isAlarmY[18]; }
            set { _isAlarmY[18] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY19
        {
            get { return _isAlarmY[18]; }
            set { _isAlarmY[18] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY20
        {
            get { return _isAlarmY[19]; }
            set { _isAlarmY[19] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY21
        {
            get { return _isAlarmY[20]; }
            set { _isAlarmY[20] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY22
        {
            get { return _isAlarmY[21]; }
            set { _isAlarmY[21] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY23
        {
            get { return _isAlarmY[22]; }
            set { _isAlarmY[22] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY24
        {
            get { return _isAlarmY[23]; }
            set { _isAlarmY[23] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY25
        {
            get { return _isAlarmY[24]; }
            set { _isAlarmY[24] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY26
        {
            get { return _isAlarmY[25]; }
            set { _isAlarmY[25] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY27
        {
            get { return _isAlarmY[26]; }
            set { _isAlarmY[26] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY28
        {
            get { return _isAlarmY[27]; }
            set { _isAlarmY[27] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY29
        {
            get { return _isAlarmY[28]; }
            set { _isAlarmY[28] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY30
        {
            get { return _isAlarmY[29]; }
            set { _isAlarmY[29] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY31
        {
            get { return _isAlarmY[30]; }
            set { _isAlarmY[30] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY32
        {
            get { return _isAlarmY[31]; }
            set { _isAlarmY[31] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY33
        {
            get { return _isAlarmY[32]; }
            set { _isAlarmY[32] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY34
        {
            get { return _isAlarmY[33]; }
            set { _isAlarmY[33] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY35
        {
            get { return _isAlarmY[34]; }
            set { _isAlarmY[34] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY36
        {
            get { return _isAlarmY[35]; }
            set { _isAlarmY[35] = value; OnPropertyChanged(); }
        }

        public bool IsAlarmY37
        {
            get { return _isAlarmY[36]; }
            set { _isAlarmY[36] = value; OnPropertyChanged(); }
        }
    }
}
