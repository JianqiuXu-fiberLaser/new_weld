using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RobotWeld.AlgorithmsBase;
using RobotWeld.ViewModel;
using RobotWeld.Weldding;

namespace RobotWeld
{
    /// <summary>
    /// SelectMaterial.xaml 的交互逻辑
    /// </summary>
    
    public partial class SelectMaterial : Window
    {
        private SelectMaterialViewModel viewModel;
        private MaterialList materialList;

        public delegate void SMsendfile(int fileIndex);
        public SMsendfile? smSendfile;
        public int filematerialIndex;

        // load the material file from the project file
        public SelectMaterial()
        {
            InitializeComponent();

            // display the material list control in the material selection
            viewModel = new SelectMaterialViewModel();
            materialList = new MaterialList();

            WeldMaterial.ItemsSource = materialList;
            WeldMaterial.DisplayMemberPath = "Name";
            WeldMaterial.SelectedValuePath = "Index";
            WeldMaterial.SelectedIndex = viewModel.MaterialType;
        }

        public void GetMaterialFile(int fileMaterial)
        {
            viewModel?.GetfileInfo(fileMaterial);
        }

        public void GetFileName(int fileMaterialIndex)
        {
            filematerialIndex = fileMaterialIndex;
        }

        //-- The keyboard actions --
        private void SheetThickness_GotFocus(object sender, RoutedEventArgs e)
        {
            SheetThickness.SelectAll();
        }

        private void SheetThickness_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void WireDiameter_GotFocus(object sender, RoutedEventArgs e)
        {
            WireDiameter.SelectAll();
        }

        private void WireDiameter_PreviewMouseLeftButtonDown(object sender,
            MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        // save the selection
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // grab the file index
            if (viewModel != null)
            {
                viewModel.sendfile = GetFileName;

                // save the material parameter, if they are changed.
                viewModel.SetFileMaterial(true);
            }

            // delivary the file index to main windows
            smSendfile?.Invoke(filematerialIndex);
            this.Close();
        }

        // discard the selection
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // keep the old material parameters
            viewModel?.SetFileMaterial(false);
            this.Close();
        }

        // when the selection is changed
        private void WeldMaterial_Changed(object sender, SelectionChangedEventArgs e)
        {
            object var = (sender as ComboBox).SelectedValue;
            if (viewModel != null)
                viewModel.MaterialName = (var != null) ? var.ToString() : "";
        }

        // limit to input digit numbers only
        private void SheetThickness_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
           }
        }

        private void WireDiameter_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
