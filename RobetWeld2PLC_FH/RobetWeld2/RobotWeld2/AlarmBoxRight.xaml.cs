using System.Windows;
using System.Windows.Controls;

namespace RobotWeld2
{
    /// <summary>
    /// AlarmBoxRight.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmBoxRight : UserControl
    {
        public AlarmBoxRight()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IoValueRProperty = 
            DependencyProperty.Register("IoValueR", typeof(bool), typeof(AlarmBoxRight));

        public bool IoValueR
        {
            get => (bool)GetValue(IoValueRProperty);
            set => SetValue(IoValueRProperty, value);
        }
    }
}
