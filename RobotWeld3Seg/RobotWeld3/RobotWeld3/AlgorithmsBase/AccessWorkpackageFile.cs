///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Simple code indicates the point's file
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.GetTrace;
using RobotWeld3.Motions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Read and write workpackage file.
    /// </summary>
    internal class AccessWorkpackageFile
    {
        private const string recFile = "./WorkPackageRecord.xml";

        internal AccessWorkpackageFile() { }

        /// <summary>
        /// Open the default workpackage file.
        /// </summary>
        internal static WorkPackage Open()
        {
            return Open(recFile);
        }

        /// <summary>
        /// Open the file with a given file name
        /// </summary>
        /// <param name="filename"> file name </param>
        /// <returns> WorkPackage </returns>
        [STAThread]
        internal static WorkPackage Open(string filename)
        {
            var wk = new WorkPackage();
            var doc = new XmlDocument();

            try
            {
                doc.Load(filename);
                if (doc.DocumentElement == null) throw new IOException();

                XmlElement root = doc.DocumentElement;
                XmlNode? em = root.SelectSingleNode("project");

                if (em != null) GetFileDic(em, ref wk);
            }
            catch (IOException)
            {
                Assertion.AssertError("装载记录文件错误", 01);
            }
            catch (Exception ex1)
            {
                Assertion.AssertError(ex1.Message, 02);
            }
            finally
            {
                // do nothing
            }
            return wk;
        }

        /// <summary>
        /// Create new workpackage file, with new time stamp.
        /// </summary>
        /// <returns></returns>
        internal static WorkPackage NewFile()
        {
            var wk = new WorkPackage();
            wk.TimeStamp = CTimeStamp.CreateTimeStamp();
            return wk;
        }

        /// <summary>
        /// Update workpackage class when chose different trace type.
        /// </summary>
        /// <param name="wk"></param>
        internal static void UpdateWorkPackage(ref WorkPackage wk)
        {
            wk.ClearPointList();
            var str = CTraceType.GetName(wk.Tracetype);
            var itc = wk.TrCode[str];
            AccessTraceFile.ReadTraceFile(itc, ref wk);
        }

        /// <summary>
        /// Respond: [Save as]
        /// Save the file with a given file name and trace code
        /// </summary>
        /// <param name="traceCode"> trace code </param>
        /// <param name="filename"> file name </param>
        [STAThread]
        internal static void Save(string filename, in WorkPackage wk)
        {
            var sdict = new Dictionary<string, int>();

            foreach (var dtr in wk.TrCode)
            {
                var itc = dtr.Value;
                string sourceFile = "./Storage/" + itc.ToString() + ".trc";
            
                var ntc = GenerateCode();
                string destinateFile = "./Worktime/" + ntc.ToString() + ".trc";

                try
                {
                    File.Copy(sourceFile, destinateFile, true);
                    sdict.Add(dtr.Key, ntc);
                }
                catch (IOException ex)
                {
                    Assertion.AssertError(ex.Message, 03);
                }
            }

            SaveasXml(filename, sdict, wk.Tracetype);
        }

        /// <summary>
        /// Save current trace type in the workpackage file.
        /// </summary>
        /// <param name="typ"></param>
        [STAThread]
        internal static void FreshCurrent(Tracetype typ)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load(recFile);
                if (doc.DocumentElement == null) throw new IOException();
                XmlElement root = doc.DocumentElement;
                XmlNode? em = root.SelectSingleNode("project");
                if (em != null)
                {
                    var current = em.SelectSingleNode("current");
                    if (current != null)
                    {
                        var s = CTraceType.GetName(typ);
                        current.InnerText = s;
                    }
                }

                doc.Save(recFile);
            }
            catch(IOException)
            {
                Assertion.AssertError("工作文件损坏", 05);
            }
        }

        /// <summary>
        /// Generate a new file code depends on the time
        /// </summary>
        /// <returns></returns>
        private static int GenerateCode()
        {
            TimeSpan tSpan = DateTime.Now - new DateTime(2023, 9, 20, 23, 36, 11);
            return tSpan.Seconds;
        }

        /// <summary>
        /// write the save-as-record file to disk
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sd"></param>
        /// <param name="typ"></param>
        [STAThread]
        private static void SaveasXml(string filename, Dictionary<string, int> sd, Tracetype typ)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<Projects></Projects>");
            if (doc.DocumentElement is null) return;

            XmlElement root = doc.DocumentElement;

            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.InsertBefore(decl, root);

            XmlComment comment = doc.CreateComment("The record file for the sun flower welding system");
            doc.InsertBefore(comment, root);

            // the file node
            XmlElement project = doc.CreateElement("project");
            root.InsertAfter(project, root.FirstChild);

            foreach (var idt in sd)
            {
                AddXmlElement(doc, project, idt.Key, idt.Value.ToString());
            }

            var tsr = CTraceType.GetName(typ);
            AddXmlElement(doc, project, "current", tsr);

            // write to disk
            try
            {
                doc.Save(filename);
            }
            catch (IOException ex)
            {
                Assertion.AssertError(ex.Message, 04);
            }
        }

        /// <summary>
        /// Add Element under the project
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="em"></param>
        /// <param name="ename"></param>
        /// <param name="einner"></param>
        private static void AddXmlElement(XmlDocument doc, XmlElement em, string ename, string einner)
        {
            XmlElement t = doc.CreateElement(ename);
            t.InnerText = einner;
            em.AppendChild(t);
        }

        /// <summary>
        /// get the material data from the record file
        /// </summary>
        /// <param name="em"></param>
        /// <param name="wk"></param>
        private static void GetFileDic(XmlNode em, ref WorkPackage wk)
        {
            wk.TrCode = new Dictionary<string, int>();
            var tl = CTraceType.GetTracetypeList();
            foreach (var t in tl)
            {
                var tname = CTraceType.GetName(t);
                XmlNode? xn = em.SelectSingleNode(tname);

                if (xn != null)
                {
                    var tc = Convert.ToInt32(xn.InnerText);
                    wk.TrCode.Add(tname, tc);
                }
            }

            XmlNode? xm = em.SelectSingleNode("current");
            if (xm != null)
            {
                int trc = wk.TrCode[xm.InnerText];
                AccessTraceFile.ReadTraceFile(trc, ref wk);
            }
        }
    }
}
