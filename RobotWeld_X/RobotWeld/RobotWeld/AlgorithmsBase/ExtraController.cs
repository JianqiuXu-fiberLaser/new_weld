using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private PlcProgram? plc;
        private bool _cnt;

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
            plc = new PlcProgram(this);

            if (plc.Connect())
                _cnt = true;
            else
                _cnt = false;
        }

        public void GetView(int msg)
        {
            if (_mainViewModel is not null)
            {
                _mainViewModel.ErrorMsg = ErrDict[msg];
            }
        }

        public void SetParameter(ParameterViewModel mvm)
        {
            _mainViewModel = mvm;
        }

        public void TurnOn(ActionIndex ati)
        {
            if (_cnt)
                plc?.SentToPLC(MemType.M, (int)ati, true);
        }

        public void TurnOff(ActionIndex ati)
        {
            if (_cnt)
                plc?.SentToPLC(MemType.M, (int)ati, false);
        }

        public void Close()
        {
            if (_cnt)
                plc?.Close();
        }
    }

    public enum ActionIndex
    {
        LEFT_TOPPING = 1302,
        LEFT_LOCATE = 1301,
        LEFT_LOAD = 1305,
        RIGHT_TOPPING = 1306,
        RIGHT_LOCATE = 1307,
        RIGHT_LOAD = 1308,
    }
}
