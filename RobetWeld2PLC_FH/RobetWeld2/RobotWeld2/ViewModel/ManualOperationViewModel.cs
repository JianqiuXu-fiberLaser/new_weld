using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RobotWeld2.ViewModel
{
    public class ManualOperationViewModel : INotifyPropertyChanged
    {
        private int _actMemory;
        private bool _actMemoryValue;
        private int _hdMemory;
        private int _hdMemoryValue;

        public ManualOperationViewModel() { }

        //
        //-- Response for the ICommand Events in the MainWindow --
        // required by INotifPropertyChanged interface, regist in the first
        //
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int ActMemory
        {
            get { return _actMemory; }
            set { _actMemory = value; OnPropertyChanged(); }
        }

        public bool ActMemoryValue
        {
            get { return _actMemoryValue; }
            set { _actMemoryValue = value; OnPropertyChanged(); }
        }

        public int HdMemory
        {
            get { return _hdMemory; }
            set { _hdMemory = value; OnPropertyChanged(); }
        }

        public int HdMemoryValue
        {
            get { return _hdMemoryValue; }
            set { _hdMemoryValue = value; OnPropertyChanged(); }
        }
    }
}
