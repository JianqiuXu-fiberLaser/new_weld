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
    /// AlarmBoxRed.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmBoxRed : UserControl
    {
        public AlarmBoxRed()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IoValueRedProperty =
    DependencyProperty.Register("IoValueRed", typeof(bool), typeof(AlarmBoxRed));

        public bool IoValueRed
        {
            get => (bool)GetValue(IoValueRedProperty);
            set => SetValue(IoValueRedProperty, value);
        }
    }
}
