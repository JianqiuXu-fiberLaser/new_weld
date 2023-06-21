using System.Windows;
using System.Windows.Controls;

namespace RobotWeld2
{
    /// <summary>
    /// AlarmBox.xaml 的交互逻辑
    /// The box to indicate the state of infromation of PLC in the special bit.
    /// </summary>
    public partial class AlarmBox : UserControl
    {
        public AlarmBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IoValueProperty = 
            DependencyProperty.Register("IoValue", typeof(bool), typeof(AlarmBox));   

        public bool IoValue
        {
            get => (bool)GetValue(IoValueProperty); 
            set => SetValue(IoValueProperty, value);
        }
    }
}
