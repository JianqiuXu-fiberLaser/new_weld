using System.Windows;
using System.Windows.Input;
using RobotWeld.ViewModel;

namespace RobotWeld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// (1) Commands in the Menu 
    /// (2) Animation in the left screen and video displayed in rigth screen
    /// </summary>
    public partial class MainWindow : Window
    {
        public ParameterViewModel ? viewModel;

        public MainWindow()
        {
            InitializeComponent();
            ParameterViewModel viewModel = new ParameterViewModel();

            this.Loaded += new RoutedEventHandler(MainWindow_Load);
        }

        private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.New)
                e.CanExecute = true;
            if (e.Command == ApplicationCommands.Open)
                e.CanExecute = true;
            if (e.Command == ApplicationCommands.Close)
                e.CanExecute = true;
            if (e.Command == ApplicationCommands.SaveAs)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.DownloadCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.ImportCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.PresetTraceCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.AutoTraceCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.PresetParameterCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.UserParameterCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.AutoParameterCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.LaserDecodeCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.LaserVersionCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.GetHelpCommand)
                e.CanExecute = true;
            if (e.Command == ParameterViewModel.ShowAboutCommand)
                e.CanExecute = true;
            e.Handled = true;
        }

        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.New)
            {
                viewModel ??= new(); 
                viewModel.New();

            }
            if (e.Command == ApplicationCommands.Close)
            {
                viewModel?.Close();
                viewModel = null;
                Close();
            }
            if (e.Command == ApplicationCommands.SaveAs)
            {
                viewModel ??= new();
                viewModel?.SaveDialog();
            }
            if (e.Command == ApplicationCommands.Open)
            {
                viewModel ??= new();
                viewModel?.Open();
            }
            if (e.Command == ParameterViewModel.DownloadCommand)
            {
                viewModel ??= new ParameterViewModel();
                viewModel.DownloadSave();

            }
            if (e.Command == ParameterViewModel.ImportCommand)
            {
                // 导入DXF文件
            }
            if (e.Command == ParameterViewModel.PresetTraceCommand)
            {
/*                var dialog = new ChoiceTraceTyep();
                dialog.ShowDialog();*/
            }
            if (e.Command == ParameterViewModel.AutoTraceCommand)
            {
                // TODO: calculate the trace with optical vision
            }
            if (e.Command == ParameterViewModel.PresetParameterCommand)
            {
                viewModel?.SelectMaterialDialog();
            }
            if (e.Command == ParameterViewModel.UserParameterCommand)
            {
/*                var dialog = new InputLaserParameter();
                dialog.ShowDialog();*/
            }
            if (e.Command == ParameterViewModel.AutoParameterCommand)
            {
                // TODO:give the weld parameter from calculation
            }
            if (e.Command == ParameterViewModel.LaserDecodeCommand)
            {
                // decode the laser expire date
                //viewModel?.LaserDecode();
            }
            if (e.Command == ParameterViewModel.LaserVersionCommand)
            {
                // show the laser factory information
                //viewModel?.ShowFactory();
            }
            if (e.Command == ParameterViewModel.GetHelpCommand)
            {
            }
            if (e.Command == ParameterViewModel.ShowAboutCommand)
            {
                var dialog = new About();
                dialog.ShowDialog();
            }

            e.Handled = true;
        }

        private void LaserButton_Click(object sender, RoutedEventArgs e)
        {
            //viewModel?.LaserButton();
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            //viewModel?.LineButton();
        }

        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            // the first loading of MainWindow
            // loads the record file and check the binary file in the motion Card
            viewModel ??= new();
            viewModel.WeldLoad();
        }

        private void WindowClose_Click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel?.Close();
            viewModel = null;
            e.Cancel = false;
        }
    }
}
