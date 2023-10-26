///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.GetTrace;
using RobotWeld3.Motions;
using RobotWeld3.Welding;
using System.Xml;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Save trace point into Xml file.
    /// </summary>
    internal class SavePointXml
    {
        private readonly WorkPackage _wk;

        /// <summary>
        /// Save trace point into Xml file.
        /// </summary>
        /// <param name="wk"></param>
        internal SavePointXml(WorkPackage wk)
        {
            _wk = wk;
        }

        /// <summary>
        /// Save from Root
        /// </summary>
        /// <param name="doc"> XmlDocument </param>
        internal void SaveXmlRoot(XmlDocument doc, XmlElement trace)
        {
            AddXmlTracetype(doc, trace);
            AddXmlTime(doc, trace);
            AddXmlParameter(doc, trace);
            AddXmlMaterial(doc, trace);
            AddXmlWire(doc, trace);
            AddXmlSubtrace(doc, trace);
        }

        /// <summary>
        /// Create Trace type
        /// </summary>
        /// <param name="doc"></param>
        private void AddXmlTracetype(XmlDocument doc, XmlElement trace)
        {
            var t = doc.CreateElement("type");
            t.InnerText = CTraceType.GetName(_wk.Tracetype);
            trace.AppendChild(t);
        }

        /// <summary>
        /// Create time stamp
        /// </summary>
        /// <param name="doc"></param>
        private void AddXmlTime(XmlDocument doc, XmlElement trace)
        {
            var t = doc.CreateElement("time");
            t.InnerText = _wk.TimeStamp.ToString();
            trace.AppendChild(t);
        }

        /// <summary>
        /// Create parameter
        /// </summary>
        /// <param name="doc"></param>
        private void AddXmlParameter(XmlDocument doc, XmlElement trace)
        {
            var t = doc.CreateElement("parameter");
            var p = _wk.GetParameter();
            int pn = p.Count;
            string str = string.Empty;
            for (int i = 0; i < pn - 1; i++)
            {
                str += p[i].ToString() + ",";
            }
            str += p[^1].ToString();
            t.InnerText = str;
            trace.AppendChild(t);
        }

        /// <summary>
        /// Create Material parameter Xml block
        /// </summary>
        /// <param name="doc"></param>
        private void AddXmlMaterial(XmlDocument doc, XmlElement trace) 
        {
            var t = doc.CreateElement("material");
            trace.AppendChild(t);

            var m = _wk.Material;
            var tt = doc.CreateElement("mtype");
            tt.InnerText = m.MaterialType.ToString();
            t.AppendChild(tt);

            tt = doc.CreateElement("thickness");
            tt.InnerText = m.Thickness.ToString();
            t.AppendChild(tt);
        }

        /// <summary>
        /// Create Wire block
        /// </summary>
        /// <param name="doc"></param>
        private void AddXmlWire(XmlDocument doc, XmlElement trace) 
        {
            var w = _wk.Wire;
            var t = doc.CreateElement("wire");
            t.InnerText = w.Diameter.ToString();
            trace.AppendChild(t);
        }

        /// <summary>
        /// Create subtrace block for point list
        /// </summary>
        /// <param name="doc"></param>
        private void AddXmlSubtrace(XmlDocument doc, XmlElement trace) 
        {
            var pts = _wk.PointList;

            var em = doc.CreateElement("subtrace");
            trace.AppendChild(em);

            var t = doc.CreateElement("number");
            t.InnerText = "0";
            em.AppendChild(t);

            for (int i = 0; i < pts.Count; i++)
            {
                AddXmlPoint(doc, em, i, pts[i]);
            }
        }

        /// <summary>
        /// Save each point
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="em"></param>
        /// <param name="i"> number </param>
        /// <param name="pt"> display point </param>
        private void AddXmlPoint(XmlDocument doc, XmlElement em, int i, DisPoint pt)
        {
            var tm = doc.CreateElement("point");
            em.AppendChild(tm);

            var t = doc.CreateElement("number");
            t.InnerText = i.ToString();
            tm.AppendChild(t);

            t = doc.CreateElement("coordinate");
            t.InnerText = pt.Vector.X.ToString() + "," + pt.Vector.Y.ToString() + "," 
                + pt.Vector.Z.ToString() + "," + pt.Vector.A.ToString();
            tm.AppendChild(t);

            t = doc.CreateElement("ltype");
            t.InnerText = pt.Linetype.ToString();
            tm.AppendChild(t);

            var emt = doc.CreateElement("laser");
            tm.AppendChild(emt);

            var lp = pt.Parameter;
            AddXmlLaser(doc, emt, lp);

            t = doc.CreateElement("wobble");
            t.InnerText = pt.Wobble.ToString();
            tm.AppendChild(t);

            t = doc.CreateElement("speed");
            t.InnerText = pt.Speed.ToString();
            tm.AppendChild(t);
        }

        /// <summary>
        /// Create laser display parameter
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="em"></param>
        /// <param name="lp"></param>
        private void AddXmlLaser(XmlDocument doc, XmlElement em, LaserDisplayParameter lp)
        {
            var t = doc.CreateElement("power");
            t.InnerText = lp.Power.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("frequency");
            t.InnerText = lp.Frequency.ToString();
            em.AppendChild(t);

            t = doc.CreateElement("duty");
            t.InnerText = lp.Duty.ToString();
            em.AppendChild(t);
        }
    }
}
