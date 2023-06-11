using System;
using System.Net.Sockets;
using Modbus.Device;
using System.Net;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Deal with the PLC operation, special to XD5E
    /// ModBus protocol base on NModbus Library, for RTU, TCP and UDP frame
    /// </summary>
    public class PlcLibrary
    {
        private readonly object _lock = new();

        private static readonly int alarmAddr = 3200;
        private static readonly int ErrNum = 25;

        private static readonly int InputAddress = 0x5000;
        private static readonly int AddiInputAddress = 0x5100;
        private static readonly int OutputAddress = 0x6000;
        private static readonly int AddiOutputAddress = 0x6100;

        private static readonly int[] IoNum = new int[4] { 33, 8, 26, 11 };
        private static readonly int totalIoNum = 78;

        private bool[] bScan = new bool[totalIoNum];

        public PlcLibrary() { }

        //
        // Note: this method is special for allocating the alarm memory address
        //
        public bool[]? ReadAlarm()
        {
            return Readcoils(alarmAddr, ErrNum);
        }

        public bool[] ScanIo()
        {
            bool[]? var;
            var = Readcoils(InputAddress, IoNum[0]);
            if (var != null)
            {
                for (int i = 0; i < IoNum[0]; i++)
                    bScan[i] = var[i];
            }

            var = Readcoils(AddiInputAddress, IoNum[1]);
            if (var != null)
            {
                for (int i = 0; i < IoNum[1]; i++)
                {
                    int addi = IoNum[0];
                    bScan[addi+i] = var[i];
                }
            }

            var = Readcoils(OutputAddress, IoNum[2]);
            if (var != null)
            {
                for (int i = 0; i < IoNum[2]; i++)
                {
                    int addi = IoNum[0] + IoNum[1];
                    bScan[addi + i] = var[i];
                }
            }

            var = Readcoils(AddiOutputAddress, IoNum[3]);
            if (var != null)
            {
                for (int i = 0; i < IoNum[3]; i++)
                {
                    int addi = IoNum[0] + IoNum[1] + IoNum[2];
                    bScan[addi + i] = var[i];
                }
            }

            return bScan;
        }

        public void SentToPLC(MemType mty, int addr, bool sw)
        {
            if (mty == MemType.M)
            {
                int varAddr = addr;
                lock (_lock)
                {
                    WriteCoil(varAddr, sw);
                }
            }
        }

        public void SentToPLC(MemType mty, int addr, int data)
        {
            if (mty == MemType.HD)
            {
                int varAddr = 0XA080 + addr;
                lock (_lock)
                {
                    WriteReg(varAddr, data);
                }
            }
        }

        /// <summary>
        /// Get data from M type memory
        /// </summary>
        /// <param name="mty"> MemType </param>
        /// <param name="addr"> addr </param>
        /// <returns></returns>
        public bool GetFromPLC(MemType mty, int addr)
        {
            bool ret = true;
            if (mty == MemType.M)
            {
                lock (_lock)
                {
                    bool[]? vars = Readcoils(addr, 1);
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
                int varAddr = 0XA080 + addr;
                lock (_lock)
                {
                    ushort[]? vars = ReadKeepReg(varAddr, 1);
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

        //--------------------- ModBus/TCP Protocal ------------------------//
        private TcpClient? tcpClient;
        private ModbusIpMaster? master;

        private bool Connect(string ip)
        {
            int port = 502;    // the ModBus/TCP
                               // port is 502

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
            catch (Exception) { }
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
