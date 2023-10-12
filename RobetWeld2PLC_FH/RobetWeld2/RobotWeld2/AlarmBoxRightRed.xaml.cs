using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotWeld2
{
    /// <summary>
    /// AlarmBoxRightRed.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmBoxRightRed : UserControl
    {
        public AlarmBoxRightRed()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IoValueRRedProperty =
    DependencyProperty.Register("IoValueRRed", typeof(bool), typeof(AlarmBoxRightRed));

        public bool IoValueRRed
        {
            get => (bool)GetValue(IoValueRRedProperty);
            set => SetValue(IoValueRRedProperty, value);
        }
    }
}
