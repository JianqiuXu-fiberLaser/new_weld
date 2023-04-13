using System.Windows;
using System.Windows.Input;
using RobotWeld.AlgorithmsBase;
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
        public WeldFileAccess ? fileAccess;

        public MainWindow()
        {
            InitializeComponent();
            fileAccess = new WeldFileAccess();
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
                fileAccess ??= new(); 
                fileAccess.New();

            }
            if (e.Command == ApplicationCommands.Close)
            {
                fileAccess?.Close();
                this.Close();
            }
            if (e.Command == ApplicationCommands.SaveAs)
            {
                fileAccess?.SaveDialog();
            }
            if (e.Command == ApplicationCommands.Open)
            {
                /*var fileAccess = new FileAccess();
                fileAccess.Open();*/
            }
            if (e.Command == ParameterViewModel.DownloadCommand)
            {
                fileAccess ??= new WeldFileAccess();
                fileAccess.DownloadSave();

            }
            if (e.Command == ParameterViewModel.ImportCommand)
            {
                // 导入DXF文件
            }
            if (e.Command == ParameterViewModel.PresetTraceCommand)
            {

            }
            if (e.Command == ParameterViewModel.AutoTraceCommand)
            {
                // TODO: calculate the trace with optical vision
            }
            if (e.Command == ParameterViewModel.PresetParameterCommand)
            {
                var dialog = new SelectMaterial();

                if(fileAccess != null)
                    dialog.MaterialIndex = fileAccess.FileMaterial;

                dialog.ShowDialog();
            }
            if (e.Command == ParameterViewModel.UserParameterCommand)
            {
                var dialog = new InputLaserParameter();
                dialog.ShowDialog();
            }
            if (e.Command == ParameterViewModel.AutoParameterCommand)
            {
                // TODO: give the weld parameter from calculation
            }
            if (e.Command == ParameterViewModel.LaserDecodeCommand)
            {
                // TODO: decode the laser expire date
            }
            if (e.Command == ParameterViewModel.LaserVersionCommand)
            {
                // TODO: show the laser factory information
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
            // TODO:
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO:
        }

        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            // the first loading of MainWindow
            // loads the record file and check the binary file in the motion Card
            fileAccess ??= new();
            fileAccess.WeldLoad();
        }
    }
}
