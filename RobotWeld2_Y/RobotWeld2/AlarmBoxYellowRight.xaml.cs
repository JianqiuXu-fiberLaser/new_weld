using System.Windows;
using System.Windows.Controls;

namespace RobotWeld2
{
    /// <summary>
    /// AlarmBoxYellowRight.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmBoxYellowRight : UserControl
    {
        public AlarmBoxYellowRight()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IoValueRYellowProperty = 
            DependencyProperty.Register("IoValueRYellow", typeof(bool), typeof(AlarmBoxYellowRight));

        public bool IoValueRYellow
        {
            get => (bool)GetValue(IoValueRYellowProperty);
            set => SetValue(IoValueRYellowProperty, value);
        }
    }
}
