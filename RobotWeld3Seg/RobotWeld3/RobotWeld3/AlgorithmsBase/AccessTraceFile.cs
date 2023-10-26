///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: New class to access trace point file.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.Motions;
using System;
using System.IO;
using System.Xml;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Read and write Trace specification file.
    /// </summary>
    internal class AccessTraceFile
    {
        public AccessTraceFile() { }

        /// <summary>
        /// Read Trace point data from file.
        /// </summary>
        /// <param name="iCode"></param>
        /// <param name="wk"></param>
        [STAThread]
        internal static void ReadTraceFile(int iCode, ref WorkPackage wk)
        {
            string filename = "./Storage/" + iCode.ToString() + ".trc";
            var doc = new XmlDocument();

            // only the old file to be read
            try
            {
                doc.Load(filename);
                if (doc.DocumentElement == null) throw new IOException();

                XmlElement root = doc.DocumentElement;

                var atf = new GetPointXml(wk);
                atf.GetXmlRoot(root);
            }
            catch (IOException)
            {
                Assertion.AssertError("坐标文件错误", 10);
            }
            catch (Exception ex)
            {
                Werr.WerrMessage(ex.Message);
            }
            finally
            {
                // do nothing
            }
        }

        /// <summary>
        /// Save the trace data to the file
        /// </summary>
        /// <param name="icode"></param>
        /// <param name="wk"></param>
        [STAThread]
        internal static void Save(int icode, in WorkPackage wk)
        {
            if (icode == 0 && wk == null) return;

            string filename = "./Storage/" + icode.ToString() + ".trc";
            var doc = new XmlDocument();

            doc.LoadXml("<Trace></Trace>");

            if (doc.DocumentElement is null) return;
            XmlElement root = doc.DocumentElement;

            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.InsertBefore(decl, root);

            XmlComment comment = doc.CreateComment("The record file for the sun flower welding system");
            doc.InsertBefore(comment, root);

            XmlElement trace = doc.CreateElement("trace");
            root.InsertAfter(trace, root.FirstChild);

            var stf = new SavePointXml(wk);
            stf.SaveXmlRoot(doc, trace);

            try
            {
                doc.Save(filename);
            }
            catch (IOException ex)
            {
                Assertion.AssertError(ex.Message, 11);
            }
        }
    }
}
