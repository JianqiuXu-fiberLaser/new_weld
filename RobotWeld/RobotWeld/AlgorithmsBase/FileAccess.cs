using Microsoft.Win32;
using System;
using System.IO;
using System.Xml;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// the class to operate the file information
    /// </summary>
    public class WeldFileAccess
    {
        private int _fileIndex = 0;
        private int _workTime = 0;
        private int _fileTrace = 0;
        private int _fileMaterial = 0;
        private int _fileLaser = 0;
        private int _fileCode = 0;

        private const string recFiles = "./weldingRecord.xml";
        public XmlDocument ? doc;

        public WeldFileAccess() { }

        // create new enginning project
        public void New()
        {
            DateTime dateTime2020 = new DateTime(2020,1,1,0,0,0);
            DateTime DateNow = DateTime.Now;
            TimeSpan timespan = (DateNow - dateTime2020);
            _fileIndex = (int)timespan.TotalSeconds;

            _workTime = 0;
            _fileTrace = 0;
            _fileMaterial = 0;
            _fileLaser = 0;
            _fileCode = 0;
        }

        // save the file information when download.
        // the information is used for accouting the worktime.
        public void DownloadSave()
        {
            FileInfo downfile = new FileInfo("./Storage/" + 
                _fileIndex.ToString() + ".inf");
            if (downfile.Exists) return;
            else
            {
                try
                {
                    FileStream afile = new FileStream("./Storage/" +
                    _fileIndex.ToString() + ".inf", FileMode.Create);
                    StreamWriter sw = new StreamWriter(afile);

                    sw.WriteLine(_fileIndex.ToString());
                    sw.WriteLine(_fileTrace.ToString());
                    sw.WriteLine(_fileMaterial.ToString());
                    sw.WriteLine(_fileLaser.ToString()); 
                    sw.Close();
                }
                catch (Exception ex)
                {
                    new Werr().WaringMessage(ex.Message);
                    return;
                }
            }
        }

        // save the worktime when download
        private void SaveWorkTime()
        {
            try
            {
                // 增加搜索目录功率，如果有相同名称，就扩展文件
                // 否则就建新文件

                FileStream wfile = new FileStream("./Worktime/" +
                _fileIndex.ToString() + ".wkt", FileMode.Create);
                StreamWriter sw = new StreamWriter(wfile);

                sw.WriteLine(_workTime.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                new Werr().WaringMessage(ex.Message);
                return;
            }
        }

        // save the file information with the customer name
        private void Save(string filename)
        {
            doc = new XmlDocument();
            doc.LoadXml("<Projects></Projects>");
            if (doc.DocumentElement == null) { return; }

            try
            {
                XmlElement root = doc.DocumentElement;

                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.InsertBefore(decl, root);

                XmlComment comment = doc.CreateComment(
                    "The record file for the sun flower welding system");
                doc.InsertBefore(comment, root);

                XmlElement project = doc.CreateElement("project");
                root.InsertAfter(project, root.FirstChild);

                AddXmlElement(doc, project, "fileindex", _fileIndex.ToString());
                AddXmlElement(doc, project, "trace", _fileTrace.ToString());
                AddXmlElement(doc, project, "material", _fileMaterial.ToString());
                AddXmlElement(doc, project, "laser", _fileLaser.ToString());
                AddXmlElement(doc, project, "code", _fileCode.ToString());

                doc.Save(filename);
            }
            catch (IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
        }

        private void AddXmlElement(XmlDocument doc, XmlElement em, 
            string name, string fvalue)
        {
            XmlElement var = doc.CreateElement(name);
            var.InnerText = fvalue;
            em.AppendChild(var);
        }

        // save as using the dialog Box
        public void SaveDialog(string filename)
        {   
            Save(filename);
        }

        // save file before close the program.
        public void Close()
        {
            Save(recFiles);
            SaveWorkTime();
        }

        // get the file data from record file
        private void GetFileData(XmlNode em)
        {
            if (!RetrieveAttribute(em, "fileindex", out _fileIndex))
            {
                new Werr().WerrMessage("Record file has an error!");
                return;
            }

            if (!RetrieveAttribute(em, "trace", out _fileTrace))
            {
                new Werr().WerrMessage("Trace in the record file has an error!");
                return;
            }

            if (!RetrieveAttribute(em, "material", out _fileMaterial))
            {
                new Werr().WerrMessage("Material in the record file has an error!");
                return;
            }

            if (!RetrieveAttribute(em, "laser", out _fileLaser))
            {
                new Werr().WerrMessage("Laser in the record file has an error!");
                return;
            }

            if (!RetrieveAttribute(em, "code", out _fileCode))
            {
                new Werr().WerrMessage("Laser in the record file has an error!");
                return;
            }
        }

        private bool RetrieveAttribute(XmlNode nd, string name, out int fvalue)
        {
            bool rst;
            XmlNode? var = nd.SelectSingleNode(name);
            if (var != null)
            {
                rst = int.TryParse(var.InnerText, out fvalue);
            }
            else
            {
                fvalue = -1;
                rst = false;
            }

            return rst;
        }

        // load the record file at the start of the weldding system.
        public void WeldLoad()
        {
            Open(recFiles);
        }

        // open the record file
        public void Open(string filename)
        { 
            XmlElement root;
            try
            {
                doc = new XmlDocument();
                doc.Load(filename);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch(IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em != null)
            {
                this.GetFileData(em);
            }     
        }

        //-- get and set properties --
        public int FileIndex
        { 
            get { return _fileIndex; }
            set { _fileIndex = value; }
        }

        public int WorkTime
        {
            get { return _workTime; }
            set { _workTime = value; }
        }

        public int FileTrace
        { 
            get { return _fileTrace; }
            set { _fileTrace = value; }
        }

        public int FileMaterial
        { 
            get { return _fileMaterial; }
            set { _fileMaterial = value; }
        }

        public int FileLaser
        { 
            get { return _fileLaser; }
            set
            {
                _fileLaser = value;
            }
        }

        public int FileCode
        {
            get { return _fileCode; }
            set { _fileCode = value; }
        }
    }
}
