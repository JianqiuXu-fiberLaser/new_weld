using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using System;
using System.Xml;
using System.IO;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Read and write work package to disk
    /// </summary>
    public class BasicWorkFile
    {
        private const string recFiles = "./WorkPackageRecord.xml";
        private WorkPackage? _workPackage;
        private string _current = string.Empty;

        public BasicWorkFile() { }

        public int GetMaxPower()
        {
            return MaxPowerFile.GetMaxPower();
        }

        public void PutMaxPower(int maxPower)
        {
            MaxPowerFile.PutMaxPower(maxPower);
        }

        public void GetSpiralParameter(WorkPackage workPackage, out double pitch)
        {
            _workPackage = workPackage;
            pitch = 0;

            XmlElement root;
            XmlDocument doc = new();

            try
            {
                doc.Load(recFiles);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch
            {
                Assertion.AssertError("装载螺距文件错误", 1);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                int typeIndex = ReadFileIndex(em, "spiral");
                SpiralPitchFile spf = new();
                pitch = spf.GetSpiralParameter(typeIndex);
            }
        }

        public void SaveSpiralParameter(WorkPackage workPackage, double pitch)
        {
            _workPackage = workPackage;
            XmlElement root;
            XmlDocument doc = new();
            try
            {
                doc.Load(recFiles);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch
            {
                Assertion.AssertError("装载半径文件错误", 2);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                int typeIndex = ReadFileIndex(em, "spiral");
                SpiralPitchFile spf = new();
                spf.PutSpiralParameter(typeIndex, pitch);
            }
        }

        public void GetWorkPackage(WorkPackage workPackage)
        {
            _workPackage = workPackage;
            OpenFile(recFiles);
        }

        public void UpdateWorkPackage(WorkPackage workPackage)
        {
            _workPackage = workPackage;
            string traceName = BackString(_workPackage.TraceType);
            OpenFile(recFiles, traceName);
        }

        /// <summary>
        /// Open and read workPackage data from disk.
        /// </summary>
        /// <param name="filename"></param>
        [STAThread]
        private void OpenFile(string filename)
        {
            XmlElement root;
            try
            {
                XmlDocument doc = new();
                doc.Load(filename);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch
            {
                Assertion.AssertError("装载记录文件错误", 1);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                int typeIndex = ReadFileIndex(em);
                AccessWorkPackFile awpf = new();
                if (_workPackage != null && typeIndex != 0)
                {
                    awpf.OpenPackageFile(_workPackage, typeIndex, _current);
                }
            }
        }

        [STAThread]
        private void OpenFile(string filename, string traceName)
        {
            XmlElement root;
            try
            {
                XmlDocument doc = new();
                doc.Load(filename);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch
            {
                Assertion.AssertError("装载记录文件错误", 1);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                int typeIndex = ReadFileIndex(em, traceName);
                AccessWorkPackFile awpf = new();
                if (_workPackage != null)
                {
                    if (typeIndex != 0)
                    {
                        awpf.OpenPackageFile(_workPackage, typeIndex, traceName);
                    }
                    else
                    {
                        NewFile(_workPackage);
                    }
                }
            }
        }

        //
        // translate trace type to string Name
        //
        private static Tracetype GetTraceType(string stype)
        {
            var type = stype switch
            {
                "vane" => Tracetype.VANE_WHEEL,
                "intersect" => Tracetype.INTERSECT,
                "top" => Tracetype.TOP_TRACE,
                "stage" => Tracetype.STAGE_TRACE,
                "spiral" => Tracetype.SPIRAL,
                _ => Tracetype.FREE_TRACE,
            };
            return type;
        }

        /// <summary>
        /// Find the curent traceType name and keep it to write the current index.
        /// </summary>
        /// <param name="fileName"> record file name </param>
        private void WriteFile(string fileName)
        {
            if (_workPackage == null) { return; }

            string stype = BackString(_workPackage.TraceType);
            AccessWorkPackFile awpf = new();
            awpf.WriteIndexFile(_workPackage, fileName, stype);
        }

        private static void BackupFile(string fileName)
        {
            try
            {
                File.Copy(recFiles, fileName, true);
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 104);
            }

            AnalyseFile af = new(); 
            af.AnalyzeWorkFile(recFiles);
        }

        private static string BackString(Tracetype type)
        {
            var stype = type switch
            {
                Tracetype.VANE_WHEEL => "vane",
                Tracetype.INTERSECT => "intersect",
                Tracetype.TOP_TRACE => "top",
                Tracetype.STAGE_TRACE => "stage",
                Tracetype.SPIRAL => "spiral",
                Tracetype.FREE_TRACE => "free",
                _ => "free"
            };

            return stype;
        }

        public void NewFile(WorkPackage workPackage)
        {
            _workPackage = workPackage;
            DateTime dateTime2020 = new(2020, 6, 15, 0, 0, 0);

            DateTime DateNow = DateTime.Now;
            TimeSpan timespan = (DateNow - dateTime2020);
            _workPackage.Clear();
            _workPackage.TypeIndex = (int)timespan.TotalSeconds;
        }

        public void Save(WorkPackage workPackage)
        {
            _workPackage = workPackage;
            WriteFile(recFiles);
        }

        public void Save(WorkPackage workPackage, string filename)
        {
            _workPackage = workPackage;
            WriteFile(recFiles);
            BackupFile(filename);
        }

        public void Open(WorkPackage workPackage, string filename)
        {
            _workPackage = workPackage;
            AnalyseFile af = new();
            af.RestoreWorkFile(filename);
            OpenFile(filename);
        }

        /// <summary>
        /// Get and put intersect parameter
        /// </summary>
        /// <param name="workPackage"> workpackage </param>
        /// <param name="upperRadius"> Radius to intersect </param>
        /// <param name="lowerRadius"> Radius be intersected </param>
        public void GetIntersectParameter(WorkPackage workPackage, out int instalStyle, out double upperRadius, out double lowerRadius)
        {
            _workPackage = workPackage;
            instalStyle = 0;
            upperRadius = 0;
            lowerRadius = 0;

            XmlElement root;
            XmlDocument doc = new();
            try
            {
                doc.Load(recFiles);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch
            {
                Assertion.AssertError("装载半径文件错误", 1);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                int typeIndex = ReadFileIndex(em, "intersect");
                IntersectFile intersectFile = new();
                intersectFile.GetParameter(typeIndex, out instalStyle, out upperRadius, out lowerRadius);
            }
        }

        public void SaveIntersectParameter(WorkPackage workPackage, int clampDirection, double upperRadius, double lowerRadius)
        {
            _workPackage = workPackage;
            XmlElement root;
            XmlDocument doc = new();
            try
            {
                doc.Load(recFiles);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch
            {
                Assertion.AssertError("装载半径文件错误", 2);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                int typeIndex = ReadFileIndex(em, "intersect");
                IntersectFile intersectFile = new();
                intersectFile.PutParameter(typeIndex, clampDirection, upperRadius, lowerRadius);
            }
        }

        //
        // Read file index, then go to trace file
        //
        private int ReadFileIndex(XmlNode em, string traceName)
        {
            if (_workPackage == null) return 0;

            foreach (XmlNode xn in em.ChildNodes)
            {
                if (traceName != string.Empty && xn.Name == traceName)
                {
                    string svar = xn.InnerText;
                    int typeIndex = Convert.ToInt32(svar);
                    return _workPackage.TypeIndex = typeIndex;
                }
            }

            return 0;
        }

        //
        // Read file index, then go to trace file
        //
        private int ReadFileIndex(XmlNode em)
        {
            if (_workPackage == null) return 0;

            foreach (XmlNode xn in em.ChildNodes)
            {
                if (xn.Name == "current")
                {
                    _current = xn.InnerText;
                    _workPackage.TraceType = GetTraceType(_current);
                }
            }

            foreach (XmlNode xn in em.ChildNodes)
            {
                if (_current != string.Empty && xn.Name == _current)
                {
                    string svar = xn.InnerText;
                    int typeIndex = Convert.ToInt32(svar);
                    return _workPackage.TypeIndex = typeIndex;
                }
            }

            return 0;
        }
    }
}
