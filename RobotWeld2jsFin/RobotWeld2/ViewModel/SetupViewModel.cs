using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotWeld2.ViewModel
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
