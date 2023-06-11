using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RobotWeld2
{
    /// <summary>
    /// VaneWheel.xaml 的交互逻辑
    /// </summary>
    public partial class VaneWheel : Window
    {
        private const double YRatio = 500.0;
        private const double CRatio = 100.0;
        private const double YSpeedRatio = 500.0;
        private const double CSpeedRatio = 100.0;

        private readonly DaemonFile daemonFile;
        private readonly VaneWheelViewModel viewModel;
        private ExtraController? extraController;
        private readonly VaneFile vFile;
        private Encryption encryption;

        private bool passwordOk;

        public VaneWheel(DaemonFile dmFile)
        {
            this.daemonFile = dmFile;
            this.vFile = new VaneFile(daemonFile);
            this.viewModel = new VaneWheelViewModel();
            vFile.GetData(viewModel);

            InitializeComponent();
            this.DataContext = viewModel;

            this.encryption = new Encryption();

            ExtraController extraController = new();
            extraController.ConnectPLC();
            Thread.Sleep(100);    // waiting for connection.
            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // keep the old vane parameters
            Close();
        }

        //
        // Save the PLC data the trace file
        //
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Write the vane wheel information
            daemonFile.TraceType = Tracetype.VANE_WHEEL;
            vFile.Write(viewModel);

            Close();
        }

        //
        // If the input character in TextBox is number, return true.
        //
        private static bool IsNumber(Key key)
        {
            if (key == Key.Enter || key == Key.Back || key == Key.OemPeriod || key == Key.Decimal ||
                (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9))
            {
                return true;
            }
            else { return false; }
        }

        //
        // If the input character in TextBox is number, return true.
        //
        private static bool IsIntNumber(Key key)
        {
            if (key == Key.Enter || key == Key.Back ||
                (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9))
            {
                return true;
            }
            else { return false; }
        }

        //
        // Convert the distance to the PLC step
        //
        private static void ConvertToYStep(double iNum, out int YStep)
        {
            YStep = (int)(iNum * YRatio);
        }

        //
        // Convert the angle to the PLC step
        //
        private static void ConvertToCStep(double iNum, out int CStep)
        {
            CStep = (int)(iNum * CRatio);
        }

        //
        // Convert the distance to the PLC step
        //
        private static void ConvertToYSpeed(double iNum, out int YSpeed)
        {
            YSpeed = (int)(iNum * YSpeedRatio);
        }

        //
        // Convert the angle to the PLC step
        //
        private static void ConvertToCSpeed(double iNum, out int CSpeed)
        {
            CSpeed = (int)(iNum * CSpeedRatio);
        }

        //----------------------------- Vane Number -----------------------------------//
        //
        // Text block : Vane Number
        //
        private void VaneNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                VaneNumber.SelectAll();
            }
        }

        private void VaneNumber_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void VaneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsIntNumber(e.Key))
            {
                extraController ??= new ExtraController();
                if (viewModel.VaneNumber == 6)
                    extraController.SendPlcData(SpeedAddress.VANE_NUM, 1);
                else if (viewModel.VaneNumber == 7)
                    extraController.SendPlcData(SpeedAddress.VANE_NUM, 2);

                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //----------------------------- 01 Left -----------------------------------//
        //
        // Y01 Velocity
        //
        private void Y01Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                Y01Velocity.SelectAll();
            }
        }

        private void Y01Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y01Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToYSpeed(var, out int Speed);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Speed);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // Y11 Position
        //
        private void Y01Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y01Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                Y01Position.SelectAll();
            }
        }

        private void Y01Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToYStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C01 Velocity
        //
        private void C01Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C01Velocity.SelectAll();
            }
        }

        private void C01Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C01Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCSpeed(var, out int Speed);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Speed);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C01
        //
        private void C01Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C01Position.SelectAll();
            }
        }

        private void C01Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C01Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController ??= new ExtraController();
                    extraController.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C02
        //
        private void C02Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C02Position.SelectAll();
            }
        }

        private void C02Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C02Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C03
        //
        private void C03Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C03Position.SelectAll();
            }
        }

        private void C03Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C03Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C04
        //
        private void C04Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C04Position.SelectAll();
            }
        }

        private void C04Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C04Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C05
        //
        private void C05Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C05Position.SelectAll();
            }
        }

        private void C05Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C05Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C06
        //
        private void C06Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C06Position.SelectAll();
            }
        }

        private void C06Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C06Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C07 Text block
        //
        private void C07Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C07Position.SelectAll();
            }
        }

        private void C07Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C07Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //----------------------------- 11 Right -----------------------------------//
        //
        // Y11 Velocity
        //
        private void Y11Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                Y11Velocity.SelectAll();
            }
        }

        private void Y11Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToYSpeed(var, out int Speed);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Speed);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Y11Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        //
        // Y11 Text block
        //
        private void Y11Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                Y11Position.SelectAll();
            }
        }

        private void Y11Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void Y11Position_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.OemPeriod ||
                e.Key == Key.Enter)
            {
                int var = Convert.ToInt32(viewModel.Y11Position);
                extraController?.SendPlcData(SpeedAddress.Y01_POSI, var);
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C11 Velocity
        //
        private void C11Velocity_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C11Position.SelectAll();
            }
        }

        private void C11Velocity_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C11Velocity_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCSpeed(var, out int Speed);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Speed);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C11 Text block
        //
        private void C11Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C11Position.SelectAll();
            }
        }

        private void C11Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C11Position_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.OemPeriod ||
                e.Key == Key.Enter)
            {
                if (e.Key == Key.Enter)
                {
                    int var = Convert.ToInt32(viewModel.C11Position);
                    if (viewModel.VaneNumber == 6)
                        extraController?.SendPlcData(SpeedAddress.V161_POSI, var);
                    else if (viewModel.VaneNumber == 7)
                        extraController?.SendPlcData(SpeedAddress.V171_POSI, var);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C12 Text block
        //
        private void C12Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C12Position.SelectAll();
            }
        }

        private void C12Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C12Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C13 Text block
        //
        private void C13Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C13Position.SelectAll();
            }
        }

        private void C13Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C13Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C14 Text block
        //
        private void C14Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C14Position.SelectAll();
            }
        }

        private void C14Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C14Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C15 text block
        //
        private void C15Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C15Position.SelectAll();
            }
        }

        private void C15Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C15Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void C16Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C16Position.SelectAll();
            }
        }

        //
        // C16 text block
        //
        private void C16Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C16Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController?.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //
        // C17 text block
        //
        private void C17Position_GotFocus(object sender, RoutedEventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
            else
            {
                C17Position.SelectAll();
            }
        }

        private void C17Position_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as Control;
            if (control != null) { return; }

            Keyboard.Focus(control);
            e.Handled = true;
        }

        private void C17Position_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNumber(e.Key))
            {
                if (e.Key == Key.Enter)
                {
                    double var = Convert.ToDouble(viewModel.Y01Velocity);
                    ConvertToCStep(var, out int Step);
                    extraController ??= new ExtraController();
                    extraController.SendPlcData(SpeedAddress.Y01_SPD, Step);
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //----------------------------- Click buttons -----------------------------------//
        //
        // -- Repond to Click button that the PLC driver to the given positions
        //
        private void Y01_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.Y01_CLICK);
        }
        private void C01_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C01_CLICK);
        }

        private void C02_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C02_CLICK);
        }

        private void C03_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C03_CLICK);
        }

        private void C04_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C04_CLICK);
        }

        private void C05_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C05_CLICK);
        }

        private void C06_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C06_CLICK);
        }

        private void C07_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C07_CLICK);
        }

        private void Y11_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.Y11_CLICK);
        }

        private void C11_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C11_CLICK);
        }

        private void C12_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C12_CLICK);
        }

        private void C13_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C13_CLICK);
        }

        private void C14_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C14_CLICK);
        }

        private void C15_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C15_CLICK);
        }

        private void C16_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C16_CLICK);
        }

        private void C17_Click(object sender, RoutedEventArgs e)
        {
            extraController ??= new ExtraController();
            extraController.SelfResetTurnOn(ActionIndex.C17_CLICK);
        }

        //----------------------------- Password when modify -----------------------------------//
        private void VaneType_DropDownOpened(object sender, EventArgs e)
        {
            if (encryption != null)
            {
                passwordOk = encryption.PasswordOk;
            }
            else
            {
                encryption = new Encryption();
                passwordOk = encryption.PasswordOk;
            }

            if (!passwordOk)
            {
                PassWord psw = new();
                psw.GetEncryption(encryption);
                psw.ShowDialog();
            }
        }
    }
}
