using System;
using System.Linq;
using System.Threading;
using System.IO.Ports;
using RobotWeld.ViewModel;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// Deal with the PLC operation, special to XD5
    /// </summary>
    public class PlcProgram
    {
        private readonly object _lock = new();

        private bool _continue;
        private static SerialPort? _serialPort;

        private static int _messageIndex;
        private static int old_messageIndex = 0;

        private static readonly byte bStation = 0x01;
        private static readonly byte bErrNum = 0x19;
        private byte[] receive = new byte[10];
        private byte[] readAlarm = new byte[] {bStation, (byte)Operate.readCoil, 
            0x0C, 0x80, 0, bErrNum, 0x3E, 0xB8};

        private ExtraController? extracontroller;

        public PlcProgram() 
        {
            _serialPort = new SerialPort();
            extracontroller = new ExtraController();
        }

        public PlcProgram(ExtraController extc)
        {
            _serialPort = new SerialPort();
            if (extc != null)
            {
                extracontroller = extc;
            }
            else
            {
                extracontroller = new ExtraController();
            }
        }

        public static int MessageIndex
        {
            get { return _messageIndex; }
            set { _messageIndex = value; }
        }

        public enum Operate
        {
            readCoil = 0x01,
            writeCoil = 0x05,
            writeRegist = 0x06,
        }

        public void Read()
        {
            while (_continue)
            {
                try
                {
                    if (_serialPort != null)
                    {
                        lock (_lock)
                        {
                            _serialPort.Write(readAlarm, 0, readAlarm.Length);
                        }

                        Thread.Sleep(10);

                        lock (_lock)
                        {
                            _serialPort.Read(receive, 0, receive.Length);
                        }

                        ByteToMsgback();
                    }
                }
                catch (TimeoutException)
                {
                    _continue = false;
                    new Werr().WaringMessage("The Plc is off line.");
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private static string ? GetPortName()
        {
            string[] str = SerialPort.GetPortNames();
            if (str.Length > 0) 
            { 
                return str[0];
            }
            else
            {
                new Werr().WaringMessage("No serial port exist.");
                return null;
            }
        }

        public void Close()
        {
            _serialPort?.Close();
            _serialPort = null;
        }

        private void ByteToMsgback()
        {
            if (CheckSum() && (receive[2] == (byte)Operate.readCoil))
            {
                byte[] bRecv = receive.Skip(3).Take(2).ToArray();

                (bRecv[0], bRecv[1]) = (bRecv[1], bRecv[0]);
                int var  = BitConverter.ToInt32(bRecv, 0);

                for (int i=0; i<25; i++)
                {
                    if ((var & 0x01) == 1)
                    {
                        _messageIndex = i;
                        break;
                    }

                    var >>= 1;
                }
            }
            else
            {
                Array.Clear(receive);
                _messageIndex = -1;
            }

            if ((_messageIndex != old_messageIndex) && (_messageIndex > 0))
            {
                old_messageIndex = _messageIndex;

                extracontroller?.GetView(_messageIndex);
            }
        }

        private bool CheckSum()
        {
            int len = receive.Length;
            byte[] bRecv = receive.Skip(3).Take(len - 2).ToArray();
            CRC16modbus(bRecv, out byte bhi, out byte blo);

            if (bhi == receive[len - 1] && blo == receive[len - 2])
                return true;
            else
                return false;
        }

        // Write to M-type memory
        public void SentToPLC(MemType mty, int addr, bool sw)
        {
            if (!_continue) return;

            byte[] bData = new byte[8];
            int varAddr;

            if (mty == MemType.M)
                varAddr = addr;
            else
                return;

            byte[] bAddr = BitConverter.GetBytes(varAddr);

            bData[0] = bStation;
            bData[1] = (byte)Operate.writeCoil;
            bData[2] = bAddr[1];
            bData[3] = bAddr[0];
            bData[4] = 0xFF;
            bData[5] = 0x00;
            CRC16modbus(bData, out byte bhi, out byte blo);
            bData[6] = blo;
            bData[7] = bhi;

            lock (_lock)
            {
                _serialPort?.Write(bData, 0, bData.Length);
            }
        }

        // Write to HD-type memory
        public void SentToPLC(MemType mty, int addr, int data)
        {
            if (!_continue) return;

            byte[] bData = new byte[8];
            int varAddr;

            if (mty == MemType.HD)
                varAddr = 0XA080 + addr;
            else
                return;

            byte[] bAddr = BitConverter.GetBytes(varAddr);
            byte[] outData = BitConverter.GetBytes(data);

            bData[0] = bStation;
            bData[1] = (byte)Operate.writeRegist;
            bData[2] = bAddr[1];
            bData[3] = bAddr[0];
            bData[4] = outData[1];
            bData[5] = outData[0];
            CRC16modbus(bData, out byte bhi, out byte blo);
            bData[6] = blo;
            bData[7] = bhi;

            lock (_lock)
            {
                _serialPort?.Write(bData, 0, bData.Length);
            }
        }

        public void ReceiveFromPLC(MemType mty, int addr, bool sw)
        {
            // TODO: maybe
        }

        public void ReceiveFromPLC(MemType mty, int addr, int data)
        {
            // TODO: maybe
        }

        private static void CRC16modbus(byte[] bd, out byte bhi, out byte blo)
        {
            int len = bd.Length;
            if (len > 2)
            {
                ushort crc = 0xFFFF;

                for (int i = 0; i < len-2; i++)
                {
                    crc = (ushort)(crc ^ (bd[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }

                bhi = (byte)((crc & 0xFF00) >> 8);
                blo = (byte)(crc & 0x00FF);
            }
            else
            {
                bhi = 0; 
                blo = 0;
            }
        }

        private bool InitializePort()
        {
            if (_serialPort == null) return false;

            // Set the appropriate properties.
            string? var = GetPortName();
            if (var != null)
            {
                _serialPort.PortName = var;
                _serialPort.BaudRate = 19200;
                _serialPort.Parity = Parity.Even;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;

                // Set the read/write timeouts
                _serialPort.ReadTimeout = 300;
                _serialPort.WriteTimeout = 300;
            }
            else
            {
                _continue = false;
                return false;
            }

            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                    _continue = true;
                }
                catch (Exception ex)
                {
                    _continue = false;
                    new Werr().WaringMessage(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        public bool Connect()
        {
            if ( _serialPort == null )
            {
                _serialPort = new SerialPort();
                if (InitializePort())
                {
                    // Start a background thread
                    Thread readThread = new(Read);
                    readThread.Start();
                    readThread.IsBackground = true;
                }
                else
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The type of memory in PLC
    /// </summary>
    public enum MemType
    {
        M,
        HD
    }
}
