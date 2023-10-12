using RobotWeld2.Welding;
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
using System.Windows.Shapes;

namespace RobotWeld2
{
    /// <summary>
    /// Intersect.xaml 的交互逻辑
    /// </summary>
    public partial class Intersect : Window
    {
        private DaemonFile dmFile;

        public Intersect(DaemonFile dmFile)
        {
            this.dmFile = dmFile;
            InitializeComponent();
        }

        private void VerticalTube_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void VerticalTube_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void VerticalTube_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void HorizonalTube_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void HorizonalTube_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void HorizonalTube_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
