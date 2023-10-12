///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) each trace has its trace data.
//           (2) read trace information from PLC.
//
///////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Xml;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Basic function to treat file
    /// </summary>
    public class BaseTreatFile
    {
        private const string recFile = "./weldingRecord.xml";

        public BaseTreatFile() { }

        public int Open()
        {
            return Open(recFile);
        }

        //
        // open the record file and return traceIndex
        //
        [STAThread]
        public int Open(string filename)
        {
            int ret = 0;
            try
            {
                var doc = new XmlDocument();
                doc.Load(filename);
                if (doc.DocumentElement == null) throw new IOException();

                XmlElement root = doc.DocumentElement;
                XmlNode? em = root.SelectSingleNode("project");
                if (em != null) GetFileData(em, out ret);
            }
            catch
            {
                Assertion.AssertError("装载记录文件错误", 0);
            }
            finally
            {
                // do nothing
            }
            return ret;
        }

        public void Save(int traceIndex)
        {
            Save(traceIndex, recFile);
        }

        /// <summary>
        /// Save trace information into disk
        /// </summary>
        /// <param name="traceIndex"></param>
        /// <param name="filename"></param>
        [STAThread]
        public void Save(int traceIndex, string filename)
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
            AddXmlElement(doc, project, traceIndex);

            try
            {
                doc.Save(filename);
            }
            catch (IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
        }

        //
        // get the material data from the record file
        //
        private static void GetFileData(XmlNode em, out int index)
        {
            XmlNode? xn = em.SelectSingleNode("index");
            if (xn != null)
            {
                index = Convert.ToInt32(xn.InnerText); 
            }
            else index = 0;
        }

        //
        // Add Xml Element to the Xml file node
        //
        private static void AddXmlElement(XmlDocument doc, XmlElement em, int index)
        {
            XmlElement t = doc.CreateElement("tracetype");
            t.InnerText = "vane";
            em.AppendChild(t);

            XmlElement xi = doc.CreateElement("index");
            xi.InnerText = index.ToString();
            em.AppendChild(xi);
        }
    }
}
