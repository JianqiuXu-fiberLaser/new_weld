///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: (1) Primary welding model to determine laser paremeter.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.Welding;
using System;
using System.IO;
using System.Xml;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Read and write the coefficient file for laser parameter calculation.
    /// </summary>
    internal class AccessParameterFile
    {
        public AccessParameterFile() { }

        /// <summary>
        /// read laser coefficients
        /// </summary>
        /// <param name="fn"></param>
        internal static void Read(string fn, ref LaserCoefficient lc)
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load(fn);
                if (doc.DocumentElement == null) throw new IOException();
                XmlElement root = doc.DocumentElement;
                XmlNode? param = root.SelectSingleNode("parameter");
                if (param == null) return;

                GetXmlCoeff(param, ref lc);

            }
            catch (IOException)
            {
                return;
            }
            catch (Exception ex1)
            {
                Assertion.AssertError(ex1.Message, 3002);
            }
            finally
            {
                // do nothing
            }
        }

        /// <summary>
        /// Save laser coefficient to disk or upload to web server.
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="lc"></param>
        internal static void Save(string fn, ref LaserCoefficient lc)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<Parameter></Parameter>");

            if (doc.DocumentElement is null) return;
            XmlElement root = doc.DocumentElement;

            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.InsertBefore(decl, root);

            XmlComment comment = doc.CreateComment("The parameter file for the sun flower welding system");
            doc.InsertBefore(comment, root);

            // the time node
            XmlElement em = doc.CreateElement("parameter");
            root.InsertAfter(em, root.FirstChild);

            var t = doc.CreateElement("air");
            t.InnerText = lc.AirCoef.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("rise");
            t.InnerText = lc.RiseCoef.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("fall");
            t.InnerText = lc.FallCoef.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("wireback");
            t.InnerText = lc.Wireback.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("refeed");
            t.InnerText = lc.RefeedLength.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("wirespeed");
            t.InnerText = lc.WireSpeed.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("withdrawspeed");
            t.InnerText = lc.WithdrawSpeed.ToString();
            em.AppendChild(t);

            // write to disk
            try
            {
                doc.Save(fn);
            }
            catch (IOException ex)
            {
                Assertion.AssertError(ex.Message, 204);
            }
        }

        /// <summary>
        /// Get laser coefficients
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="param"></param>
        /// <param name="lc"></param>
        private static void GetXmlCoeff(XmlNode param, ref LaserCoefficient lc)
        {
            var t = param.SelectSingleNode("air");
            if (t != null) lc.AirCoef = Convert.ToDouble(t.InnerText);

            t = param.SelectSingleNode("rise");
            if (t != null) lc.RiseCoef = Convert.ToDouble(t.InnerText);

            t = param.SelectSingleNode("fall");
            if (t != null) lc.FallCoef = Convert.ToDouble(t.InnerText);

            t = param.SelectSingleNode("wireback");
            if (t != null) lc.Wireback = Convert.ToDouble(t.InnerText);

            t = param.SelectSingleNode("refeed");
            if (t != null) lc.RefeedLength = Convert.ToDouble(t.InnerText);

            t = param.SelectSingleNode("wirespeed");
            if (t != null) lc.WireSpeed = Convert.ToDouble(t.InnerText);

            t = param.SelectSingleNode("withdrawspeed");
            if (t != null) lc.WithdrawSpeed = Convert.ToDouble(t.InnerText);
        }
    }
}
