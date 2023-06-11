using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using RobotWeld.ViewModel;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// The intermedia layer between the master and the PLC
    /// Collecting and pretreat data
    /// Managing the communication
    /// </summary>
    public class ExtraController
    {
        private readonly PlcLibrary? plc;
        private bool _continue;
        
        //
        // the ratio to the real speed of servo
        // Y axis and C axis
        //
        private readonly int _yspdRatio = 1;
        private readonly int _cspdRatio = 1;

        private Thread? OpenThread; 

        private ParameterViewModel? _mainViewModel;
        public Dictionary<int, string> ErrDict = new Dictionary<int, string>()
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

        public ExtraController()
        {
            plc = new PlcLibrary(this);
            OpenThread = new Thread(OpenPLC)
            {
                Name = nameof(OpenPLC),
                IsBackground = true,
            };
            OpenThread.Start();
        }

        //
        // Connect to PLC through TCP/IP
        //
        private void OpenPLC()
        {
            bool ret = false;
            if (plc is not null)
            {
                AddErrMsg("PLC 连接中...");
                ret = plc.Open();
            }

            if (ret)
            {
                _continue = true;
                if (_mainViewModel is not null)
                    this.AddErrMsg("PLC 连接成功");
            }
            else
            {
                _continue = false;
                if (_mainViewModel is not null)
                    this.AddErrMsg("PLC 连接失败");
            }
        }

        //
        // Add an error message to the message list.
        // Check the list length of the maximum of 3.
        // If the length is larger than 3, the earlier string should be deleted.
        //
        public void AddErrMsg(string strMsg)
        {
            if (_mainViewModel != null)
            {
                string[] varString = _mainViewModel.ErrorMsg.Split("\n");
                List<string> msglist = new List<string>(varString)
                {
                    strMsg
                };

                if (msglist.Count > 4) 
                {
                    msglist.RemoveAt(0);
                }

                string[] var2String = msglist.ToArray();
                _mainViewModel.ErrorMsg = string.Join("\n", var2String);
            }
        }

        public void GetView(int msg)
        {
            if (_mainViewModel is not null)
            {
                _mainViewModel.ErrorMsg = ErrDict[msg];
            }
        }

        //
        // setup the HD data of PLC
        //
        public void SendPlcData(SpeedAddress addr, int data)
        {
            int var;
            if (addr == SpeedAddress.Y1_SPD || addr == SpeedAddress.Y2_SPD)
            {
                var = data * _yspdRatio;
            }
            else
            {
                var = data * _cspdRatio;
            }

            plc?.SentToPLC(MemType.HD, (int)addr, var);
        }

        //
        // get the reference of MainWindow ViewModel
        //
        public void SetParameter(ParameterViewModel mvm)
        {
            _mainViewModel = mvm;
        }

        //
        // Turn on the switch M memory of PLC
        //
        public void TurnOn(ActionIndex ati)
        {
            if (_continue)
                plc?.SentToPLC(MemType.M, (int)ati, true);
        }

        //
        // Turn off the switch M memory of PLC
        //
        public void TurnOff(ActionIndex ati)
        {
            if (_continue)
                plc?.SentToPLC(MemType.M, (int)ati, false);
        }

        //
        // Disconnect the PLC, dispose the TCP/IP and close the thread.
        //
        public void Close()
        {
            if (_continue)
                plc?.Close();
        }
    }

    #region PLC address of memory and switch 
    public enum ActionIndex
    {
        LEFT_TOPPING = 1302,
        LEFT_LOCATE = 1301,
        LEFT_LOAD = 1305,
        RIGHT_TOPPING = 1306,
        RIGHT_LOCATE = 1307,
        RIGHT_LOAD = 1308,
        Y1_POS = 1303,
        Y1_NEG = 1304,
        C1_POS = 1313,
        C1_NEG = 1314,
        Y2_POS = 1323,
        Y2_NEG = 1324,
        C2_POS = 1333,
        C2_NEG = 1334,
    }

    public enum SpeedAddress
    {
        Y1_SPD = 100,
        C1_SPD = 200,
        Y2_SPD = 300,
        C2_SPD = 400,
    }
    #endregion
}
