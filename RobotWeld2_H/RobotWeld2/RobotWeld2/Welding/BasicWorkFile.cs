using RobotWeld2.AlgorithmsBase;
using RobotWeld2.GetTrace;
using RobotWeld2.Motions;
using System;
using System.Xml;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Read and write work package to disk
    /// </summary>
    public class BasicWorkFile
    {
        private const string recFiles = "./WorkPackageRecord.xml";
        private WorkPackage? workPackage;
        private string current = string.Empty;

        public BasicWorkFile() { }

        public int GetMaxPower()
        {
            MaxPowerFile mpf = new();
            return mpf.GetMaxPower();
        }

        public void GetWorkPackage(WorkPackage workPackage)
        {
            this.workPackage = workPackage;
            OpenFile(recFiles);
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
                if (workPackage != null)
                {
                    awpf.OpenPackageFile(workPackage, typeIndex, current);
                }
            }
        }

        //
        // translate trace type to string Name
        //
        private Tracetype GetTraceType(string stype)
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
        /// Write workPackage data to the disk
        /// </summary>
        /// <param name="fileName"></param>
        private void WriteFile(string fileName)
        {
            if (workPackage == null) { return; }

            string stype = BackString(workPackage.TraceType);
            AccessWorkPackFile awpf = new();
            awpf.WriteIndexFile(workPackage, fileName, stype);
        }

        private string BackString(Tracetype type)
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
            DateTime dateTime2020 = new(2020, 6, 15, 0, 0, 0);

            DateTime DateNow = DateTime.Now;
            TimeSpan timespan = (DateNow - dateTime2020);
            workPackage.Clear();
            workPackage.TypeIndex = (int)timespan.TotalSeconds;
        }

        public void Save(WorkPackage workPackage)
        {
            this.workPackage = workPackage;
            WriteFile(recFiles);
        }

        public void Save(WorkPackage workPackage, string filename)
        {
            this.workPackage = workPackage;
            WriteFile(filename);
        }

        public void Open(WorkPackage workPackage, string filename)
        {
            this.workPackage = workPackage;
            OpenFile(filename);
        }

        /// <summary>
        /// Get and put intersect parameter
        /// </summary>
        /// <param name="workPackage"></param>
        /// <param name="upperRadius"></param>
        /// <param name="lowerRadius"></param>
        public void GetIntersectParameter(WorkPackage workPackage, out int clampDirection, out double upperRadius, out double lowerRadius)
        {
            clampDirection = 0;
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
                int typeIndex = ReadFileIndex(em, workPackage, "intersect");
                IntersectFile intersectFile = new();
                intersectFile.GetParameter(typeIndex, out clampDirection, out upperRadius, out lowerRadius);
            }
        }

        public void SaveIntersectParameter(WorkPackage workPackage, int clampDirection, double upperRadius, double lowerRadius)
        {
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
                int typeIndex = ReadFileIndex(em, workPackage, "intersect");
                IntersectFile intersectFile = new();
                intersectFile.PutParameter(typeIndex, clampDirection, upperRadius, lowerRadius);
            }
        }

        //
        // Read file index, then go to trace file
        //
        private int ReadFileIndex(XmlNode em, WorkPackage workPackage, string traceName)
        {
            if (workPackage == null) return 0;

            foreach (XmlNode xn in em.ChildNodes)
            {
                if (traceName != string.Empty && xn.Name == traceName)
                {
                    string svar = xn.InnerText;
                    int typeIndex = Convert.ToInt32(svar);
                    return workPackage.TypeIndex = typeIndex;
                }
            }

            return 0;
        }

        //
        // Read file index, then go to trace file
        //
        private int ReadFileIndex(XmlNode em)
        {
            if (workPackage == null) return 0;

            foreach (XmlNode xn in em.ChildNodes)
            {
                if (xn.Name == "current")
                {
                    current = xn.InnerText;
                    workPackage.TraceType = GetTraceType(current);
                }
            }

            foreach (XmlNode xn in em.ChildNodes)
            {
                if (current != string.Empty && xn.Name == current)
                {
                    string svar = xn.InnerText;
                    int typeIndex = Convert.ToInt32(svar);
                    return workPackage.TypeIndex = typeIndex;
                }
            }

            return 0;
        }
    }
}
