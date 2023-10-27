using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Analysis workpackage file for backup all sub file
    /// </summary>
    public class AnalyseFile
    {
        private readonly List<int> laserNameList;
        private readonly List<int> materialNameList;

        public AnalyseFile()
        {
            laserNameList = new();
            materialNameList = new();
        }

        [STAThread]
        public void AnalyzeWorkFile(string filename)
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
                Assertion.AssertError("装载记录文件错误", 501);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em != null)
            {
                foreach (XmlNode xn in em.ChildNodes)
                {
                    if (xn.Name == "current") continue;

                    string TraceTypeName = xn.Name;
                    string TraceIndexName = xn.InnerText;
                    string sourceFile = "./Storage/" + TraceIndexName + TraceTypeName + ".trc";
                    try
                    {
                        SpecificFileCopy(TraceTypeName, TraceIndexName);
                        SearchMaterialParameter(sourceFile);
                        SearchLaserParameter(sourceFile);

                    }
                    catch (Exception ex)
                    {
                        Promption.Prompt(ex.Message, 502);
                    }
                }

                MoveMaterialFile();
                MoveLaserFile();
            }
        }

        [STAThread]
        private void SpecificFileCopy(string typename, string typeindex)
        {
            string sourceFile = "./Storage/" + typeindex + typename + ".trc";
            string destinateFile = "./Worktime/" + typeindex + typename + ".trc";
            string sourceIndex = "./Storage/" + typeindex;
            string destinateIndex = "./Worktime/" + typeindex;
            try
            {
                File.Copy(sourceFile, destinateFile, true);
                if (typename == "vane")
                    File.Copy(sourceIndex + ".van", destinateIndex + ".van", true);

                if (typename == "intersect")
                    File.Copy(sourceIndex + ".int", destinateIndex + ".int", true);

                if (typename == "spiral")
                    File.Copy(sourceIndex + ".spl", destinateIndex + ".spl", true);
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 507);
            }
        }

        [STAThread]
        private void SearchMaterialParameter(string sourceFile)
        {
            try
            {
                IEnumerable<string> line = File.ReadLines(sourceFile);
                string[] svar;
                if (line.Count() >= 2)
                {
                    svar = line.ElementAt(1).Split(',');
                }
                else
                {
                    return;
                }

                if (svar.Length == 2)
                {
                    int temp = Convert.ToInt32(svar[1]);
                    if (materialNameList.Count == 0 || !materialNameList.Contains(temp))
                    {
                        materialNameList.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 503);
            }
        }

        [STAThread]
        private void SearchLaserParameter(string sourceFile)
        {
            try
            {
                FileStream afile = new(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new(afile);

                if (!File.Exists(sourceFile)) return;

                string? line = sr.ReadLine();
                while (line != null)
                {
                    string[] svar = line.Split(',');
                    if (svar.Length == 7)
                    {
                        int temp = Convert.ToInt32(svar[6]);
                        if (temp != 0 && (laserNameList.Count == 0 || !laserNameList.Contains(temp)))
                        {
                            laserNameList.Add(temp);
                        }
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 504);
            }
        }

        [STAThread]
        private void MoveMaterialFile()
        {
            if (materialNameList.Count == 0) { return; }
            foreach (int i in materialNameList)
            {
                string materialfile = "./Storage/" + i.ToString() + ".mat";
                string destinationFile = "./Worktime/" + i.ToString() + ".mat";
                try
                {
                    File.Copy(materialfile, destinationFile, true);
                }
                catch (Exception ex)
                {
                    Promption.Prompt(ex.Message, 505);
                }
            }
        }

        [STAThread]
        private void MoveLaserFile()
        {
            if (laserNameList.Count == 0) { return; }
            foreach (int i in laserNameList)
            {
                string laserfile = "./Storage/" + i.ToString() + ".lwp";
                string destinationFile = "./Worktime/" + i.ToString() + ".lwp";
                try
                {
                    File.Copy(laserfile, destinationFile, true);
                }
                catch (Exception ex)
                {
                    Promption.Prompt(ex.Message, 506);
                }
            }
        }

        [STAThread]
        public void RestoreWorkFile(string filename)
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
                Assertion.AssertError("装载记录文件错误", 501);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em != null)
            {
                foreach (XmlNode xn in em.ChildNodes)
                {
                    if (xn.Name == "current") continue;

                    string TraceTypeName = xn.Name;
                    string TraceIndexName = xn.InnerText;
                    string sourceFile = "./Worktime/" + TraceIndexName + TraceTypeName + ".trc";
                    try
                    {
                        SpecificFileRestore(TraceTypeName, TraceIndexName);
                        SearchMaterialParameter(sourceFile);
                        SearchLaserParameter(sourceFile);

                    }
                    catch (Exception ex)
                    {
                        Promption.Prompt(ex.Message, 502);
                    }
                }

                RestoreMaterialFile();
                RestoreLaserFile();
            }
        }

        [STAThread]
        private void SpecificFileRestore(string typename, string typeindex)
        {
            string sourceFile = "./Worktime/" + typeindex + typename + ".trc";
            string destinateFile = "./Storage/" + typeindex + typename + ".trc";
            string sourceIndex = "./Worktime/" + typeindex;
            string destinateIndex = "./Storage/" + typeindex;
            try
            {
                File.Copy(sourceFile, destinateFile, true);
                if (typename == "vane")
                    File.Copy(sourceIndex + ".van", destinateIndex + ".van", true);

                if (typename == "intersect")
                    File.Copy(sourceIndex + ".int", destinateIndex + ".int", true);

                if (typename == "spiral")
                    File.Copy(sourceIndex + ".spl", destinateIndex + ".spl", true);
            }
            catch (Exception ex)
            {
                Promption.Prompt(ex.Message, 508);
            }
        }

        [STAThread]
        private void RestoreMaterialFile()
        {
            if (materialNameList.Count == 0) { return; }
            foreach (int i in materialNameList)
            {
                string materialfile = "./Worktime/" + i.ToString() + ".mat";
                string destinationFile = "./Storage/" + i.ToString() + ".mat";
                try
                {
                    File.Copy(materialfile, destinationFile, true);
                }
                catch (Exception ex)
                {
                    Promption.Prompt(ex.Message, 509);
                }
            }
        }

        [STAThread]
        private void RestoreLaserFile()
        {
            if (laserNameList.Count == 0) { return; }
            foreach (int i in laserNameList)
            {
                string laserfile = "./Worktime/" + i.ToString() + ".lwp";
                string destinationFile = "./Storage/" + i.ToString() + ".lwp";
                try
                {
                    File.Copy(laserfile, destinationFile, true);
                }
                catch (Exception ex)
                {
                    Promption.Prompt(ex.Message, 510);
                }
            }
        }
    }
}
