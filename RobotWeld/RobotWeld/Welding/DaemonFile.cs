using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml;
using Microsoft.Win32;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Shapes;
using System.Text;

using RobotWeld.AlgorithmsBase;
using RobotWeld.ViewModel;
using RobotWeld.GetTrace;

namespace RobotWeld.Welding
{
    /// <summary>
    /// The Daemon process for the data record
    /// </summary>
    public class DaemonFile
    {
        private int _fileIndex;
        private int _traceIndex;
        private int _traceCode;

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
        private int _weld;
        private int _wobble;

        private int _materialIndex;
        private double _sheetThickness;
        private double _wireDiameter;

        private double _x;
        private double _y;
        private double _z;

        private const string recFiles = "./weldingRecord.xml";

        public DaemonFile() 
        {
            WeldFileAccess fileAccess = new();
            fileAccess.WeldLoad(this, recFiles);
        }
        
        #region Properties
        //-- Properties --
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
            set { _out = value;}
        }

        public int WeldSpeed
        {
            get { return _weld; }
            set { _weld = value; }
        }

        public int WobbleSpeed
        {
            get { return _wobble; }
            set { _wobble = value; }
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

        public double X
        { get { return _x; } set {  _x = value; } }

        public double Y
        { get { return _y; } set { _y = value; } }

        public double Z
        { get { return _z; } set { _z = value; } }

        #endregion

        #region operate data
        //---- the program to operate data ----
        // set the value for parameter viewModel
        public void SetParameter(ParameterViewModel mvm)
        {
            mvm.MaterialIndex = _materialIndex;
            mvm.WireDiameter = _wireDiameter;
            mvm.SheetThickness = _sheetThickness;
            mvm.LaserPower = _laserPower;
            mvm.Frequency = _frequency;
            mvm.PulseWidth = _pulse;
            mvm.DutyCycle = _duty;
            mvm.LaserRise = _rise;
            mvm.LaserFall = _fall;
            mvm.LaserHoldtime = _hold;
            mvm.WireTime = _wire;
            mvm.AirIn = _in;
            mvm.AirOut = _out;
            mvm.WeldSpeed = _weld;
            mvm.WobbleSpeed = _wobble;

            mvm.X = _x;
            mvm.Y = _y;
            mvm.Z = _z;
        }

        // create new engineering project
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

        // analysis the string for the pointer
        private void StringParse(string[] strs, List<Point> pts)
        {
            for (short i=1; i<=strs.Length; i++)
            {
                string[] var = strs[i].Split(',');
                double x = Convert.ToDouble(var[0]);
                double y = Convert.ToDouble(var[1]);
                double z = Convert.ToDouble(var[2]);
                Vector vector = new(x, y, z);

                int lt = Convert.ToInt32(var[3]);
                int ls = Convert.ToInt32(var[4]);
                Point pointer = new(lt, ls, vector);

                pts.Add(pointer);
            }
        }
        #endregion

        #region File treatment
        //---- the operation of files ----
        private static void AddXmlElement(XmlDocument doc, XmlElement em, string name, string fvalue)
        {
            XmlElement var = doc.CreateElement(name);
            var.InnerText = fvalue;
            em.AppendChild(var);
        }

        // get the pointer list
        [STAThread]
        public void GetPoints(List<Point> points)
        {
            if (_traceCode == 0) return;

            // only the old file to be read
            try
            {
                string rfilename = ("./Storage/" + _traceIndex.ToString() + ".trc");
                string[] lines = File.ReadAllLines(rfilename);
                if (lines.Length > 0)
                    StringParse(lines, points);
            }
            catch (Exception ex) 
            { 
                new Werr().WerrMessage(ex.ToString());
            }
        }

        [STAThread]
        public void SaveTrace(List<Point> points)
        {
            _traceCode++;

            FileStream wfile = new FileStream("./Storage/" +
               _traceIndex.ToString() + ".trc", FileMode.Create);
            StreamWriter sw = new StreamWriter(wfile);

            sw.WriteLine(_traceCode.ToString());

            int ptNumber = points.Count;
            for (short i = 0; i < ptNumber; i++)
            {
                sw.WriteLine(points[i].ToString());
            }

            sw.Close();
        }

        [STAThread]
        public void OpenDialog()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Weld Data files (*.wfd)|*.wfd",
                Title = "Open Weld Data file",
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = openFileDialog.ShowDialog();

            if (rst == true)
            {
                string filename = openFileDialog.FileName;
                WeldFileAccess fileAccess = new();
                fileAccess.OpenFile(this, filename);
            }
        }

        // close the main window and keep the scene
        public void Close()
        {
            Save(recFiles);
        }


        // save the enginnering project
        [STAThread]
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
                Save(filename);
            }
        }

        // save the detail stuff to the file
        [STAThread]
        private void Save(string filename)
        {
            // only the new trace is code = 0
            _traceCode++;

            XmlDocument doc = new();
            doc.LoadXml("<Projects></Projects>");
            if (doc.DocumentElement is null) { return; }

            try
            {
                XmlElement root = doc.DocumentElement;

                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.InsertBefore(decl, root);

                XmlComment comment = doc.CreateComment(
                    "The record file for the sun flower welding system");
                doc.InsertBefore(comment, root);

                // the file node
                XmlElement project = doc.CreateElement("project");
                root.InsertAfter(project, root.FirstChild);
                AddXmlElement(doc, project, "fileindex", _fileIndex.ToString());

                // the trace node
                XmlElement trace = doc.CreateElement("trace");
                project.AppendChild(trace);
                AddXmlElement(doc, trace, "index", _traceIndex.ToString());
                AddXmlElement(doc, trace, "code", _traceCode.ToString());

                // the material node
                XmlElement material = doc.CreateElement("material");
                project.AppendChild(material);
                AddXmlElement(doc, material, "type", _materialIndex.ToString());
                AddXmlElement(doc, material, "thick", _sheetThickness.ToString());
                AddXmlElement(doc, material, "diameter", _wireDiameter.ToString());

                // the laser node
                XmlElement laser = doc.CreateElement("laser");
                project.AppendChild(laser);
                AddXmlElement(doc, laser, "power", _laserPower.ToString());
                AddXmlElement(doc, laser, "frequency", _frequency.ToString());
                AddXmlElement(doc, laser, "pulse", _pulse.ToString());
                AddXmlElement(doc, laser, "duty", _duty.ToString());
                AddXmlElement(doc, laser, "rise", _rise.ToString());
                AddXmlElement(doc, laser, "fall", _fall.ToString());
                AddXmlElement(doc, laser, "hold", _hold.ToString());
                AddXmlElement(doc, laser, "wire", _wire.ToString());
                AddXmlElement(doc, laser, "in", _in.ToString());
                AddXmlElement(doc, laser, "out", _out.ToString());
                AddXmlElement(doc, laser, "weld", _weld.ToString());
                AddXmlElement(doc, laser, "wobble", _wobble.ToString());

                doc.Save(filename);
            }
            catch (IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
        }

        #endregion
    }
}
