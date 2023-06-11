using Microsoft.Win32;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using RobotWeld2.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// The Daemon process for the data record
    /// </summary>
    public class DaemonFile
    {
        private int _fileIndex;
        private int _traceIndex;
        private int _traceCode;
        private Tracetype _traceType;

        private int _maxPower;
        private int _laserPower;
        private int _frequency;
        private int _pulse;
        private int _duty;
        private int _rise;
        private int _fall;
        private int _hold;
        private int _wire;
        private int _in;
        private int _out;
        private double _weld;
        private double _wobble;
        private int _leap;

        private LaserParameter _laserParameter;

        private int _materialIndex;
        private double _sheetThickness;
        private double _wireDiameter;

        private int _x;
        private int _y;
        private int _z;

        private bool _lineType;
        private bool _laserOnOff;

        private const string recFiles = "./weldingRecord.xml";
        private readonly MainWindowViewModel _viewModel;

        private int _pointIndex;
        private string _errMsg = string.Empty;
        private static string _openMsg = string.Empty;

        private bool _pmFlag;

        public DaemonFile(MainWindowViewModel viewModel)
        {
            BaseTreatFile baseTreatFile = new(this);
            baseTreatFile.Open(recFiles);
            _viewModel = viewModel;
            _laserParameter = new LaserParameter(_maxPower);
        }

        public bool LineType
        {
            get { return _lineType; }
            set { _lineType = value; }
        }

        public bool LaserOnOff
        {
            get { return _laserOnOff; }
            set { _laserOnOff = value; }
        }

        public int PointIndex
        {
            get { return _pointIndex; }
            set { _pointIndex = value; }
        }

        public int FileIndex
        {
            get { return _fileIndex; }
            set { _fileIndex = value; }
        }

        public int TraceIndex
        {
            get { return _traceIndex; }
            set { _traceIndex = value; }
        }

        public int TraceCode
        {
            get { return _traceCode; }
            set { _traceCode = value; }
        }

        public Tracetype TraceType
        {
            get { return _traceType; }
            set { _traceType = value; }
        }

        public int MaxPower
        {
            get { return _maxPower; }
            set { _maxPower = value; }
        }

        public int LaserPower
        {
            get { return _laserPower; }
            set { _laserPower = value; }
        }

        public int Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        public int PulseWidth
        {
            get { return _pulse; }
            set { _pulse = value; }
        }

        public int DutyCycle
        {
            get { return _duty; }
            set { _duty = value; }
        }

        public int LaserRise
        {
            get { return _rise; }
            set { _rise = value; }
        }

        public int LaserFall
        {
            get { return _fall; }
            set { _fall = value; }
        }

        public int LaserHoldtime
        {
            get { return _hold; }
            set { _hold = value; }
        }

        public int WireTime
        {
            get { return _wire; }
            set { _wire = value; }
        }

        public int AirIn
        {
            get { return _in; }
            set { _in = value; }
        }

        public int AirOut
        {
            get { return _out; }
            set { _out = value; }
        }

        public double WeldSpeed
        {
            get { return _weld; }
            set { _weld = value; }
        }

        public double WobbleSpeed
        {
            get { return _wobble; }
            set { _wobble = value; }
        }

        public int LeapSpeed
        {
            get { return _leap; }
            set { _leap = value; }
        }
        public int MaterialIndex
        {
            get { return _materialIndex; }
            set { _materialIndex = value; }
        }

        public double WireDiameter
        {
            get { return _wireDiameter; }
            set { _wireDiameter = value; }
        }

        public double SheetThickness
        {
            get { return _sheetThickness; }
            set { _sheetThickness = value; }
        }

        public bool PmFlag
        {
            get { return _pmFlag; }
            set { _pmFlag = value; }
        }

        public int X
        { get { return _x; } set { _x = value; } }

        public int Y
        { get { return _y; } set { _y = value; } }

        public int Z
        { get { return _z; } set { _z = value; } }

        /// <summary>
        /// ---- the program to operate data ----
        /// set the value for parameter viewModel
        /// </summary>
        /// <param name="mvm"> the Parameter view model </param>
        public void SetParameter()
        {
            _viewModel.MaterialIndex = _materialIndex;
            _viewModel.WireDiameter = _wireDiameter;
            _viewModel.SheetThickness = _sheetThickness;
            _viewModel.LaserPower = _laserPower;
            _viewModel.Frequency = _frequency;
            _viewModel.PulseWidth = _pulse;
            _viewModel.DutyCycle = _duty;
            _viewModel.LaserRise = _rise;
            _viewModel.LaserFall = _fall;
            _viewModel.LaserHoldtime = _hold;
            _viewModel.WireTime = _wire;
            _viewModel.AirIn = _in;
            _viewModel.AirOut = _out;
            _viewModel.WeldSpeed = (int)_weld;
            _viewModel.WobbleSpeed = (int)_wobble;
            _viewModel.LeapSpeed = _leap;

            _viewModel.X = _x;
            _viewModel.Y = _y;
            _viewModel.Z = _z;
            _viewModel.InputPowerOk = true;
        }

        /// <summary>
        /// Get the laser parameter from daemon file.
        /// </summary>
        /// <param name="ivm"> the viewMode of Input laser parameter </param>
        public void GetLaserParameter(InputViewModel ivm)
        {
            ivm.MaxPower = _maxPower;
            ivm.LaserPower = _laserPower;
            ivm.Frequency = _frequency;
            ivm.LaserPulseWidth = _pulse;
            ivm.LaserDutyCycle = _duty;
            ivm.LaserRise = (_rise > 0) ? _rise : 0;
            ivm.LaserFall = (_fall > 0) ? _fall : 0;
            ivm.LaserHoldtime = (_hold > 0) ? _hold : 0;
            ivm.WireTime = _wire;
            ivm.AirIn = _in;
            ivm.AirOut = _out;
            ivm.WeldSpeed = (int)_weld;
            ivm.WobbleSpeed = (int)_wobble;
            ivm.LeapSpeed = _leap;
        }

        /// <summary>
        /// Set the laser parameter to the daemon file.
        /// </summary>
        /// <param name="ivm"> the viewMode of Input laser parameter </param>
        public void SetLaserParameter(InputViewModel ivm)
        {
            _laserPower = ivm.LaserPower;
            _frequency = ivm.Frequency;
            _pulse = ivm.LaserPulseWidth;
            _duty = ivm.LaserDutyCycle;
            _rise = ivm.LaserRise;
            _fall = ivm.LaserFall;
            _hold = ivm.LaserHoldtime;
            _wire = ivm.WireTime;
            _in = ivm.AirIn;
            _out = ivm.AirOut;
            _weld = ivm.WeldSpeed;
            _wobble = ivm.WobbleSpeed;
            _leap = ivm.LeapSpeed;
        }

        /// <summary>
        /// create a new engineering project
        /// </summary>
        public void NewFile()
        {
            DateTime dateTime2020 = new(2020, 1, 1, 0, 0, 0);
            DateTime dataTime2041 = new(2020, 4, 1, 0, 0, 0);

            DateTime DateNow = DateTime.Now;
            TimeSpan timespan = (DateNow - dateTime2020);
            _fileIndex = (int)timespan.TotalSeconds;

            timespan = (DateNow - dataTime2041);
            _traceIndex = (int)timespan.TotalSeconds;
            _traceCode = 0;
            _traceType = Tracetype.NONE;

            LaserPower = 0;
            Frequency = 0;
            PulseWidth = 0;
            DutyCycle = 0;
            LaserRise = 0;
            LaserFall = 0;
            LaserHoldtime = 0;
            WireTime = 0;
            AirIn = 0;
            AirOut = 0;
            WeldSpeed = 0;
            WobbleSpeed = 0;

            MaterialIndex = 0;
            SheetThickness = 0;
            WireDiameter = 0;

            X = 0; Y = 0; Z = 0;
        }

        //---- the operation of files ----

        /// <summary>
        /// Open the file through the winfrom dialog
        /// </summary>
        public void OpenDialog()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Weld Data files (*.wfd)|*.wfd",
                Title = "打开焊接文件",
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = openFileDialog.ShowDialog();

            if (rst == true)
            {
                string filename = openFileDialog.FileName;
                BaseTreatFile baseTreatFile = new(this);
                baseTreatFile.Open(filename);
            }
        }

        /// <summary>
        /// close the main window and keep the scene
        /// </summary>
        public void Close()
        {
            BaseTreatFile baseTreatFile = new(this);
            baseTreatFile.Save(recFiles);
        }


        /// <summary>
        /// save the enginnering project
        /// </summary>
        public void SaveDialog()
        {

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".wfd",
                Filter = "WeldData documents (.wfd)|*.wfd",
                AddExtension = true,
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = dialog.ShowDialog();
            if (rst == true)
            {
                string filename = dialog.FileName;
                BaseTreatFile baseTreatFile = new(this);
                baseTreatFile.Save(filename);
            }
        }

        public void ChangeLinetype()
        {
            if (_viewModel.LineType)
            {
                _viewModel.LineType = false;
            }
            else
            {
                _viewModel.LineType = true;
            }
        }

        public void ChangeLaseronoff()
        {
            if (_viewModel.LaserOnOff)
            {
                _viewModel.LaserOnOff = false;
                _laserOnOff = false;
            }
            else
            {
                _viewModel.LaserOnOff = true;
                _laserOnOff = true;
            }

        }

        public void SetSelectionOK()
        {
            // forbid to input laser in main window
            _viewModel.InputPowerOk = true;
            _pointIndex = 0;
        }

        public void SetInputPower(bool value)
        {
            _viewModel.InputPowerOk = value;
        }

        public void SetInputPower(int value)
        {
            _viewModel.LaserPower = value;
        }

        public void SetXYZ(int x, int y, int z)
        {
            _viewModel.X = x;
            _viewModel.Y = y;
            _viewModel.Z = z;
        }

        /// <summary>
        /// Add an error message to the message box
        /// </summary>
        /// <param name="msg"></param>
        public void AddMsg(string msg)
        {
            _errMsg += msg + "\n";
            _viewModel.ErrMsg = _errMsg;
        }

        public void FreshMsg()
        {
            _errMsg += _openMsg + "\n";
            _viewModel.ErrMsg = _errMsg;
            _openMsg = string.Empty;
        }

        public void AddMsg(int x, int y, int z)
        {
            string lnstring = string.Empty;
            string lsstring = string.Empty;

            if (_viewModel.LineType)
                lnstring = "直线";
            else
                lnstring = "圆弧";

            if (_viewModel.LaserOnOff)
                lsstring = "激光开";
            else
                lsstring = "激光关";

            _errMsg += (x.ToString() + "," + y.ToString() + "," + z.ToString() + "," +
                lnstring + "," + lsstring + "," + _viewModel.LaserPower.ToString() + "\n");
            _viewModel.ErrMsg = _errMsg;
        }

        public static void AddInfo(string msg)
        {
            _openMsg += msg;
        }

        public LaserParameter laserParameter
        {
            get
            {
                _laserParameter ??= new LaserParameter(_maxPower);
                _laserParameter.MaxPower = _maxPower;
                _laserParameter.LaserPower = _laserPower;
                _laserParameter.Frequency = _frequency;
                _laserParameter.PulseWidth = _pulse;
                _laserParameter.DutyCycle = _duty;
                _laserParameter.LaserRise = _rise;
                _laserParameter.LaserFall = _fall;
                _laserParameter.LaserHoldtime = _hold;
                _laserParameter.AirIn = _in;
                _laserParameter.AirOut = _out;
                _laserParameter.WireLength = 0;
                _laserParameter.WireTime = 0;
                _laserParameter.WireBack = 0;
                _laserParameter.WireSpeed = 0;

                return _laserParameter;
            }
            set { _laserParameter = value; }
        }

        public void DisplyPointList(List<Point> pointList)
        {
            string pstring = string.Format("{0,-8}\t{1,-20}{2,-20}{3,-18}{4,5}{5,8}{6,7}\n\n", "序号", "X", "Y", "Z", "线型", "开关状态", "功率");
            for (int i = 0; i < pointList.Count; i++)
            {
                pstring += string.Format("{0,-8}\t", i) + pointList[i].ToScreen();
            }

            _viewModel.PointList = pstring;
        }

        public void DisplyPointList(List<Point> pointList, int ptIndex)
        {
            string pstring = string.Format("{0,-8}\t{1,-20}{2,-20}{3,-18}{4,5}{5,8}{6,7}\n\n", "序号", "X", "Y", "Z", "线型", "开关状态", "功率");
            for (int i = 0; i < pointList.Count; i++)
            {
                if (i == ptIndex)
                {
                    pstring += string.Format("{0,-8}\t", Convert.ToString("\u25A1")) + pointList[i].ToScreen();
                }
                else
                {
                    pstring += string.Format("{0,-8}\t", i) + pointList[i].ToScreen();
                }
            }

            _viewModel.PointList = pstring;
        }

        public void PreparedState(bool bState)
        {
            if (!bState)
            {
                _viewModel.PmText = "待机....";
                _viewModel.PmFlag = false;
            }
            else
            {
                _viewModel.PmText = "准备运行";
                _viewModel.PmFlag = true;
            }
        }
    }
}
