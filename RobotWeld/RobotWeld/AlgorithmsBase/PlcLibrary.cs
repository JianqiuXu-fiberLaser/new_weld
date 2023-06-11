using System;
using System.Net.Sockets;
using Modbus.Device;
using System.Net;
using System.Threading;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// Deal with the PLC operation, special to XD5E
    /// ModBus protocol base on NModbus Library, for RTU, TCP and UDP frame
    /// </summary>
    public class PlcLibrary
    {
        private readonly object _lock = new();
        private static int _messageIndex;
        private static int old_messageIndex = 0;

        private static readonly int alarmAddr = 3200;
        private static readonly int ErrNum = 25;

        private bool[]? bAlarm = new bool[ErrNum];
        private ExtraController? extracontroller;

        private bool plcIsDone;

        #region Construct
        public PlcLibrary() 
        {
            extracontroller = new ExtraController();
        }

        public PlcLibrary(ExtraController extc)
        {
            if (extc is not null)
            {
                extracontroller = extc;
            }
            else
            {
                extracontroller = new ExtraController();
            }
            
        }
        #endregion

        #region properties
        public static int MessageIndex
        {
            get { return _messageIndex; }
            set { _messageIndex = value; }
        }
        
        public bool PlcIsDone
        {
            get { return plcIsDone; } 
            set { plcIsDone = value;}
        }
        #endregion

        #region public methods
        public void ReadAlarm()
        {
            while (plcIsDone)
            {
                lock(_lock)
                {
                    bAlarm = Readcoils(alarmAddr, ErrNum);
                }
                Thread.Sleep(1000);
            }
        }

        public void SentToPLC(MemType mty, int addr, bool sw)
        {
            if (mty == MemType.M)
            {
                int varAddr = addr;
                lock(_lock)
                {
                    WriteCoil(varAddr, sw);
                }
            }
            else
            { 
                return;
            }
        }

        public void SentToPLC(MemType mty, int addr, int data)
        {
            if (mty == MemType.HD)
            { 
                int varAddr = 0XA080 + addr;
                lock(_lock)
                {
                    WriteReg(varAddr, data);
                }
            }
            else
            {
                return;
            }
        }

        public bool GetFromPLC(MemType mty, int addr)
        {
            bool ret = true;
            if (mty == MemType.M)
            {
                lock (_lock)
                {
                    bool[]?  vars = Readcoils(addr, 1);
                    if (vars != null)
                    {
                        ret = vars[0];
                    }
                }
            }

            return ret;
        }

        public int ReceiveFromPLC(MemType mty, int addr)
        {
            int ret = 0;
            if (mty == MemType.HD)
            {
                lock (_lock)
                {
                    ushort[]? vars = ReadKeepReg(addr, 1);
                    if (vars != null)
                    {
                        ret = Convert.ToInt32(vars[0]);
                    }
                }
            }
            return ret;
        }

        public bool Open()
        {
            string ip = "192.168.6.6";
            return Connect(ip);
        }

        public void Close()
        {
            Disconnect();
            master?.Dispose();
        }

        public void StartPLcThread()
        {
            if (master is not null)
            {
                plcIsDone = true;

                // Start a background thread
                Thread readThread = new(ReadAlarm);
                readThread.Start();
                readThread.IsBackground = false;
            }
        }
        #endregion

        #region Modbus/TCP method
        private TcpClient? tcpClient;
        private ModbusIpMaster? master;

        private bool Connect(string ip)
        {
            int port = 502;    // the ModBus port is 502

            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(ip), port);
                master = ModbusIpMaster.CreateIp(tcpClient);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool Disconnect()
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        // iAddress: register address
        // iLength: data length to be read
        private ushort[]? ReadKeepReg(int iAddress, int iLength)
        {
            try
            {
                if (master is not null)
                {
                    ushort[] des = master.ReadHoldingRegisters(Convert.ToUInt16(iAddress), 
                        Convert.ToUInt16(iLength));
                    return des;
                }
            }
            catch (Exception) { }
            return null;
        }

        private bool[]? Readcoils(int iAddress, int iLength)
        {
            try
            {
                if (master is not null)
                {
                    bool[] des = master.ReadCoils(Convert.ToUInt16(iAddress),
                        Convert.ToUInt16(iLength));
                    return des;
                }
            }
            catch(Exception) { }
            return null;
        }

        private void WriteCoil(int iAddress, bool value)
        {
            master?.WriteSingleCoil(Convert.ToUInt16(iAddress), value);
        }

        private void WriteReg(int iAddress, int value)
        {
            master?.WriteSingleRegister(Convert.ToUInt16(iAddress),
                Convert.ToUInt16(value));
        }
        #endregion
    }

    /// <summary>
    /// Memory type of PLC
    /// </summary>
    public enum MemType
    {
        M,
        HM,
        D,
        HD
    }
}
