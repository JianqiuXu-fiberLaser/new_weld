///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld3.ViewModel
{
    public class PassWordViewModel : INotifyPropertyChanged
    {
        private string _inputPassword = string.Empty;
        private string prompting = string.Empty;

        public PassWordViewModel() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string InputPassword
        { 
            get { return _inputPassword; } 
            set 
            {  
                _inputPassword = value;
                OnPropertyChanged(nameof(InputPassword));
            }
        }

        public string Prompting
        {
            get { return prompting; }
            set 
            { 
                prompting = value; 
                OnPropertyChanged(nameof(Prompting));
            }
        }
    }
}
