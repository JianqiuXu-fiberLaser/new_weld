using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using RobotWeld2.Welding;
using System.Collections.Generic;
using System.Threading;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// The intermedia layer between the master and the PLC
    /// Collecting and pretreat data
    /// Managing the communication
    /// </summary>
    public class ExtraController
    {
        private static PlcLibrary? plc;
        private bool _scanAlarm;
        private bool _scanIO;

        //
        // the ratio to the real speed of servo
        // Y axis and C axis
        //
        private const int _yspdRatio = 1;
        private const int _cspdRatio = 1;

        //
        // Alarm Message
        //
        private string _message = string.Empty;
        private string _oldMessage = string.Empty;

        private DaemonFile? dmFile;
        private MainWindowViewModel? _mainViewModel;
        private PlcIoViewModel? plcViewModel;
        private AirValveViewModel? airValveViewModel;
        private ManualOperationViewModel? manualViewModel;

        public ExtraController()
        {
            plc ??= new PlcLibrary();
        }

        public ExtraController(DaemonFile dmFile, MainWindowViewModel mvm)
        {
            plc ??= new PlcLibrary();
            _mainViewModel = mvm;
            this.dmFile = dmFile;
        }

        public ExtraController(AirValveViewModel vvm)
        {
            plc ??= new PlcLibrary();
            airValveViewModel = vvm;
        }

        public ExtraController(PlcIoViewModel vvm)
        {
            plc ??= new PlcLibrary();
            plcViewModel = vvm;
        }

        public ExtraController(ManualOperationViewModel movm)
        {
            plc ??= new PlcLibrary();
            manualViewModel = movm;
        }

        public bool GetState(ActionIndex ati)
        {
            int addr = (int)ati;

            bool? var = plc?.GetFromPLC(MemType.M, addr);

            if (var.HasValue && var.Value == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Show the value in the HD-type memory
        /// </summary>
        /// <param name="addr"> the address </param>
        public void ShowHdValue(int addr)
        {
            int? var = plc?.ReceiveFromPLC(MemType.HD, addr);

            if ((var.HasValue) && (manualViewModel is not null))
            {
                manualViewModel.HdMemoryValue = var.Value;
            }
        }

        /// <summary>
        /// Show the value in the M-type memory
        /// </summary>
        /// <param name="ati"> the address </param>
        public void ShowMValue(int ati)
        {
            bool? var = plc?.GetFromPLC(MemType.M, ati);

            if ((var.HasValue) && (manualViewModel is not null))
            {
                manualViewModel.ActMemoryValue = var.Value;
            }
        }

        /// <summary>
        /// Switch on / off to scan the alarm
        /// </summary>
        /// <param name="sw"> the switch </param>
        public void SetScanAlarm(bool sw)
        {
            this._scanAlarm = sw;
        }

        /// <summary>
        /// Switch on / off to scan the IO
        /// </summary>
        /// <param name="sw"> the switch</param>
        public void SetScanIO(bool sw)
        {
            this._scanIO = sw;
        }

        /// <summary>
        /// Turn on the M-type address, and reset it after 10 ms 
        /// </summary>
        /// <param name="ati"> the address of the M-type memory </param>
        public void SelfResetTurnOn(ActionIndex ati)
        {
            if (plc == null)
            {
                plc = new PlcLibrary();
                ConnectPLC();
            }

            Thread t = new(() =>
            {
                TurnOn(ati);
                Thread.Sleep(10);
                TurnOff(ati);
            })
            { IsBackground = true, };
            t.Start();
        }

        /// <summary>
        /// Connect to PLC when start the PLC works
        /// </summary>
        /// <param name="idx"></param>
        public void ConnectPLC(int idx = 0)
        {
            if (idx == 0)
            {
                Thread OpenThread = new(OpenPLC)
                {
                    Name = nameof(OpenPLC),
                    IsBackground = true,
                };
                OpenThread.Start();
            }
        }

        /// <summary>
        /// Start a Thread, which scan special IO of the PLC and
        /// indicate their states in the mainViewModel, IO info page.
        /// </summary>
        public void ScanPlcIo()
        {
            Thread ScanIoThread = new(GetPlcIo)
            {
                Name = nameof(ScanPlcIo),
                IsBackground = true,
            };
            ScanIoThread.Start();
        }

        /// <summary>
        /// Start a Thread, which scan Alarm of PLC and
        /// write the error message in the main viewModel, Air valve page.
        /// </summary>
        public void ScanAlarm()
        {
            Thread AlarmThread = new(GetPlcAlarm)
            {
                Name = nameof(ScanAlarm),
                IsBackground = true,
            };
            AlarmThread.Start();
        }

        //
        // Program to get PLC alarm state, and throw a exception message
        // to the main View Model if it exist.
        // Only new message been thrown, and old one be ignored.
        //
        private void GetPlcAlarm()
        {
            while (_scanAlarm)
            {
                bool[]? var = plc?.ReadAlarm();
                if (_mainViewModel != null && var != null && var.Length == 25)
                {
                    for (int i = 0; i < var.Length - 1; i++)
                    {
                        if (var[i] != false)
                        {
                            _message = PlcError.ErrDict[i];
                            break;
                        }
                    }

                    if (_message != _oldMessage)
                    {
                        _message = _oldMessage;
                        AddErrMsg(_message);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Program to get PLC IO states and translate to main viewModel
        /// </summary>
        private void GetPlcIo()
        {
            while (_scanIO)
            {
                bool[]? var = plc?.ScanIo();
                if (plcViewModel != null && var != null && var.Length == 78)
                {
                    plcViewModel.IsAlarmX1 = var[0];
                    plcViewModel.IsAlarmX2 = var[1];
                    plcViewModel.IsAlarmX3 = var[2];
                    plcViewModel.IsAlarmX4 = var[3];
                    plcViewModel.IsAlarmX5 = var[4];
                    plcViewModel.IsAlarmX6 = var[5];
                    plcViewModel.IsAlarmX7 = var[6];
                    plcViewModel.IsAlarmX8 = var[7];
                    plcViewModel.IsAlarmX9 = var[8];
                    plcViewModel.IsAlarmX10 = var[9];
                    plcViewModel.IsAlarmX11 = var[10];
                    plcViewModel.IsAlarmX12 = var[11];
                    plcViewModel.IsAlarmX13 = var[12];
                    plcViewModel.IsAlarmX14 = var[13];
                    plcViewModel.IsAlarmX15 = var[14];
                    plcViewModel.IsAlarmX16 = var[15];
                    plcViewModel.IsAlarmX17 = var[16];
                    plcViewModel.IsAlarmX18 = var[17];
                    plcViewModel.IsAlarmX19 = var[18];
                    plcViewModel.IsAlarmX20 = var[19];
                    plcViewModel.IsAlarmX21 = var[20];
                    plcViewModel.IsAlarmX22 = var[21];
                    plcViewModel.IsAlarmX23 = var[22];
                    plcViewModel.IsAlarmX24 = var[23];
                    plcViewModel.IsAlarmX25 = var[24];
                    plcViewModel.IsAlarmX26 = var[25];
                    plcViewModel.IsAlarmX27 = var[26];
                    plcViewModel.IsAlarmX28 = var[27];
                    plcViewModel.IsAlarmX29 = var[28];
                    plcViewModel.IsAlarmX30 = var[29];
                    plcViewModel.IsAlarmX31 = var[30];
                    plcViewModel.IsAlarmX32 = var[31];
                    plcViewModel.IsAlarmX33 = var[32];
                    plcViewModel.IsAlarmX34 = var[33];
                    plcViewModel.IsAlarmX35 = var[34];
                    plcViewModel.IsAlarmX36 = var[35];
                    plcViewModel.IsAlarmX37 = var[36];
                    plcViewModel.IsAlarmX38 = var[37];
                    plcViewModel.IsAlarmX39 = var[38];
                    plcViewModel.IsAlarmX40 = var[39];
                    plcViewModel.IsAlarmX41 = var[40];

                    plcViewModel.IsAlarmY1 = var[41];
                    plcViewModel.IsAlarmY2 = var[42];
                    plcViewModel.IsAlarmY3 = var[43];
                    plcViewModel.IsAlarmY4 = var[44];
                    plcViewModel.IsAlarmY5 = var[45];
                    plcViewModel.IsAlarmY6 = var[46];
                    plcViewModel.IsAlarmY7 = var[47];
                    plcViewModel.IsAlarmY8 = var[48];
                    plcViewModel.IsAlarmY9 = var[49];
                    plcViewModel.IsAlarmY10 = var[50];
                    plcViewModel.IsAlarmY11 = var[51];
                    plcViewModel.IsAlarmY12 = var[52];
                    plcViewModel.IsAlarmY13 = var[53];
                    plcViewModel.IsAlarmY14 = var[54];
                    plcViewModel.IsAlarmY15 = var[55];
                    plcViewModel.IsAlarmY16 = var[56];
                    plcViewModel.IsAlarmY17 = var[57];
                    plcViewModel.IsAlarmY18 = var[58];
                    plcViewModel.IsAlarmY19 = var[59];
                    plcViewModel.IsAlarmY20 = var[60];
                    plcViewModel.IsAlarmY21 = var[61];
                    plcViewModel.IsAlarmY22 = var[62];
                    plcViewModel.IsAlarmY23 = var[63];
                    plcViewModel.IsAlarmY24 = var[64];
                    plcViewModel.IsAlarmY25 = var[65];
                    plcViewModel.IsAlarmY26 = var[66];
                    plcViewModel.IsAlarmY27 = var[67];
                    plcViewModel.IsAlarmY28 = var[68];
                    plcViewModel.IsAlarmY29 = var[69];
                    plcViewModel.IsAlarmY30 = var[70];
                    plcViewModel.IsAlarmY31 = var[71];
                    plcViewModel.IsAlarmY32 = var[72];
                    plcViewModel.IsAlarmY33 = var[73];
                    plcViewModel.IsAlarmY34 = var[74];
                    plcViewModel.IsAlarmY35 = var[75];
                    plcViewModel.IsAlarmY36 = var[76];
                    plcViewModel.IsAlarmY37 = var[77];
                }
                Thread.Sleep(100);
            }
        }

        //
        // Task: Connect to PLC through TCP/IP
        //
        private void OpenPLC()
        {
            bool? ret = plc?.Open();

            if (ret != null)
            {
                if (_mainViewModel is not null)
                {
                    dmFile?.AddMsg("PLC 连接成功");
                }
            }
            else
            {
                if (_mainViewModel is not null)
                    dmFile?.AddMsg("PLC 连接失败");
            }
        }

        //
        // Add an error message to the message list.
        // Check the list length of the maximum of 3.
        // If the length is larger than 3, the earlier string should be deleted.
        //
        private void AddErrMsg(string strMsg)
        {
            if (_mainViewModel != null)
            {
                string[] varString = _mainViewModel.ErrMsg.Split("\n");
                List<string> msglist = new(varString) { strMsg };

                if (msglist.Count > 4)
                    msglist.RemoveAt(0);

                string[] var2String = msglist.ToArray();
                _mainViewModel.ErrMsg = string.Join("\n", var2String);
            }
        }

        /// <summary>
        /// setup the HD data of PLC by checking the PLC state in the first.
        /// </summary>
        /// <param name="addr"> the HD address of PLC to story the data </param>
        /// <param name="data"> the data </param>
        public void SendPlcData(SpeedAddress addr, int data)
        {
            if (plc == null)
            {
                plc = new PlcLibrary();
                ConnectPLC();
            }

            BareSendPlcData(addr, data);
        }

        //
        // Setup the HD data of PLC without check PLC state
        //
        private void BareSendPlcData(SpeedAddress addr, int data)
        {
            int var;
            if (addr == SpeedAddress.Y1_SPD || addr == SpeedAddress.Y2_SPD)
                var = data * _yspdRatio;
            else
                var = data * _cspdRatio;

            plc?.SentToPLC(MemType.HD, (int)addr, var);
        }

        /// <summary>
        /// Turn on the switch M memory of PLC
        /// </summary>
        /// <param name="ati"> The address of M-type Memory </param>
        public void TurnOn(ActionIndex ati)
        {
            plc?.SentToPLC(MemType.M, (int)ati, true);
        }

        /// <summary>
        /// Turn off the switch M memory of PLC
        /// </summary>
        /// <param name="ati"> The type of memory </param>
        public void TurnOff(ActionIndex ati)
        {
            plc?.SentToPLC(MemType.M, (int)ati, false);
        }

        /// <summary>
        /// Disconnect the PLC, dispose the TCP/IP and close the thread.
        /// </summary>
        public void Close()
        {
            plc?.Close();
        }

        public void CheckPreparedState(ActionIndex ati)
        {
            if (ati != ActionIndex.LEFT_RESET && ati != ActionIndex.RIGHT_RESET)
            {
                return;
            }

            ChkPre ck = new(this);
            ck.SetParameter((int)ati);
            Thread cthread = new(ck.CheckPstate)
            { IsBackground = true };
            cthread.Start();
        }

        protected void SetReady(int ati)
        {
            if (ati == (int)ActionIndex.LEFT_RESET)
            {
                if (airValveViewModel != null)
                    airValveViewModel.Lreset = System.Windows.Media.Colors.DarkGreen;

                SelfResetTurnOn(ActionIndex.LEFT_READY);
            }

            if (ati == (int)ActionIndex.RIGHT_RESET)
            {
                if (airValveViewModel != null)
                    airValveViewModel.Rreset = System.Windows.Media.Colors.DarkGreen;

                SelfResetTurnOn(ActionIndex.RIGHT_READY);
            }

        }

        class ChkPre : ExtraController
        {
            private AirValveViewModel? AirValveViewModel;
            private int ati;
            private int resetAddr;
            public ChkPre(ExtraController exc)
            {
                this.airValveViewModel = exc.airValveViewModel;
            }

            public void SetParameter(int ati)
            {
                this.ati = ati;
                if (ati == (int)ActionIndex.LEFT_RESET)
                {
                    resetAddr = 2036;
                }
                else if (ati == (int)ActionIndex.RIGHT_RESET)
                {
                    resetAddr = 2536;
                }
            }

            public void CheckPstate()
            {
                PlcLibrary _plc = new();
                int chktime = 0;
                while (true)
                {
                    chktime++;
                    if (_plc.GetFromPLC(MemType.M, resetAddr))
                    {
                        SetReady(ati);
                        break;
                    }
                    else if (chktime >= 10)
                    {
                        break;
                    }
                    Thread.Sleep(500);
                }
            }
        }
    }

    /// <summary>
    /// The address of M-type memory for special Action
    /// </summary>
    public enum ActionIndex
    {
        YRAllORG = 2070,
        YRORG = 1300,
        YRJOGP = 1303,
        YRJOGN = 1304,
        YRWELD = 1351,
        YRINORG = 1350,
        CRORG = 1310,
        CRJOGP = 1313,
        CRJOGN = 1314,
        CR1_POSIT = 1360,
        CR2_POSIT = 1361,
        CR3_POSIT = 1362,
        CR4_POSIT = 1363,
        CR5_POSIT = 1364,
        CR6_POSIT = 1365,
        CR7_POSIT = 1366,
        CR0_POSIT = 1369,

        Y1AllORG = 2070,
        Y1ORG = 1300,
        Y1JOGP = 1303,
        Y1JOGN = 1304,
        Y1WELD = 1351,
        Y1INORG = 1350,
        C1ORG = 1310,
        C1JOGP = 1313,
        C1JOGN = 1314,
        C1_POSIT = 1360,
        C2_POSIT = 1361,
        C3_POSIT = 1362,
        C4_POSIT = 1363,
        C5_POSIT = 1364,
        C6_POSIT = 1365,
        C7_POSIT = 1366,
        C0_POSIT = 1369,

        // Button for working preparation
        LEFT_READY = 2072,
        RIGHT_READY = 2572,
        LEFT_RESET = 2070,
        RIGHT_RESET = 2570,
        AUTO_MODE = 200,
        MANUAL_MODE = 201,

        // Button in the air valve page
        LEFT_TOPPING = 1302,
        LEFT_TOPPING_2 = 1301,
        LEFT_LOCATE = 1306,
        LEFT_LOCATE_2 = 1305,
        LEFT_LOAD = 1308,
        LEFT_LOAD_2 = 1307,

        LTOP_STATE = 274, LBOTTOM_STATE = 275,
        RTOP_STATE = 374, RBOTTOM_STATE = 375,

        LLOCATE_STATE = 272, LBACK_STATE = 273,
        RLOCATE_STATE = 372, RBACK_STATE = 373,

        LLOAD_STATE = 270, LUNLO_STATE = 271,
        RLOAD_STATE = 370, RUNLO_STATE = 371,

        RIGHT_TOPPING = 1322,
        RIGHT_TOPPING_2 = 1321,
        RIGHT_LOCATE = 1326,
        RIGHT_LOCATE_2 = 1325,
        RIGHT_LOAD = 1328,
        RIGHT_LOAD_2 = 1327,

        Y1_POS = 1303,
        Y1_NEG = 1304,
        C1_POS = 1313,
        C1_NEG = 1314,
        Y2_POS = 1323,
        Y2_NEG = 1324,
        C2_POS = 1333,
        C2_NEG = 1334,

        // Button in the setup page
        Y01_CLICK = 1351,
        Y11_CLICK = 1381,
        C01_CLICK = 1369,
        C02_CLICK = 1360,
        C03_CLICK = 1361,
        C04_CLICK = 1362,
        C05_CLICK = 1363,
        C06_CLICK = 1364,
        C07_CLICK = 1365,
        C11_CLICK = 1399,
        C12_CLICK = 1390,
        C13_CLICK = 1391,
        C14_CLICK = 1392,
        C15_CLICK = 1393,
        C16_CLICK = 1394,
        C17_CLICK = 1395,
        O01_CLICK = 1350,
        O02_CLICK = 1380
    }

    public enum SpeedAddress
    {
        Y1_SPD = 100,
        C1_SPD = 200,
        Y2_SPD = 300,
        C2_SPD = 400,
        V061_POSI = 602,
        V062_POSI = 620,
        V063_POSI = 622,
        V064_POSI = 624,
        V065_POSI = 626,
        V066_POSI = 628,
        V071_POSI = 600,
        V072_POSI = 640,
        V073_POSI = 642,
        V074_POSI = 644,
        V075_POSI = 646,
        V076_POSI = 648,
        V077_POSI = 650,
        V161_POSI = 802,
        V162_POSI = 820,
        V163_POSI = 822,
        V164_POSI = 824,
        V165_POSI = 826,
        V166_POSI = 828,
        V171_POSI = 800,
        V172_POSI = 840,
        V173_POSI = 842,
        V174_POSI = 844,
        V175_POSI = 846,
        V176_POSI = 848,
        V177_POSI = 850,
        Y01_POSI = 520,
        Y11_POSI = 720,
        C01_SPD = 610,
        Y01_SPD = 526,
        C11_SPD = 810,
        Y11_SPD = 726,
        VANE_NUM = 10,
    }

    /// <summary>
    /// Treat the error information when PLC throw alarm
    /// </summary>
    public class PlcError
    {
        private static Dictionary<int, string> _errDict = new()
        {
            {0, "工作正常" },
            {1, "1#压紧气缸到动点位超时报警"},
            {2, "1#压紧气缸到原点位超时报警"},
            {3, "1#定位气缸到动点位超时报警"},
            {4, "1#定位气缸到原点位超时报警"},
            {5, "1#顶料气缸到动点位超时报警"},
            {6, "1#顶料气缸到原点位超时报警"},
            {7, "1#Y1轴回原超时报警"},
            {8, "1#C1轴回原超时报警"},
            {9, "2#压紧气缸到动点位超时报警"},
            {10, "2#压紧气缸到原点位超时报警"},
            {11, "2#定位气缸到动点位超时报警"},
            {12, "2#定位气缸到原点位超时报警"},
            {13, "2#顶料气缸到动点位超时报警"},
            {14, "2#顶料气缸到原点位超时报警"},
            {15, "2#Y1轴回原超时报警"},
            {16, "2#C1轴回原超时报警"},
            {17, "激光报警"},
            {18, "气压报警"},
            {19, "急停报警"},
            {20, "1#急停SW"},
            {21, "2#急停SW"},
            {22, "Y1伺服报警"},
            {23, "C1伺服报警"},
            {24, "Y2伺服报警"},
            {25, "C2伺服报警"}
        };

        public PlcError() { }

        public static Dictionary<int, string> ErrDict
        {
            get { return _errDict; }
        }
    }
}
