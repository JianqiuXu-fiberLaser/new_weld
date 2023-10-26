///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld3.ViewModel
{
    public class SetupViewModel : INotifyPropertyChanged
    {
        private string _inputPassword1 = string.Empty;
        private string _inputPassword2 = string.Empty;

        public SetupViewModel() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string InputPassword1
        {
            get { return _inputPassword1; }
            set
            {
                _inputPassword1 = value;
                OnPropertyChanged(nameof(InputPassword1));
            }
        }

        public string InputPassword2
        {
            get { return _inputPassword2; }
            set
            {
                _inputPassword2 = value;
                OnPropertyChanged(nameof(InputPassword2));
            }
        }
    }
}
