using System.Windows;
using System.Windows.Input;
using RobotWeld.ViewModel;

namespace RobotWeld
{
    /// <summary>
    /// SelectMaterial.xaml 的交互逻辑
    /// </summary>
    
    public partial class SelectMaterial : Window
    {
        private int _materialIndex;

        public SelectMaterial()
        {
            InitializeComponent();
            SelectMaterialViewModel viewModel = new SelectMaterialViewModel();
            this.DataContext = viewModel;
            viewModel.FileIndex = MaterialIndex;
        }

        // The keyboard actions
        private void SheetThickness_GotFocus(object sender, RoutedEventArgs e)
        {
            SheetThickness.SelectAll();
        }

        private void SheetThickness_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void WireDiameter_GotFocus(object sender, RoutedEventArgs e)
        {
            WireDiameter.SelectAll();
        }

        private void WireDiameter_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // save the material parameter, it they are changed.

            this.Close();
        }

        public int MaterialIndex
        {
            get { return _materialIndex; }
            set { _materialIndex = value; }
        }        
    }
}
