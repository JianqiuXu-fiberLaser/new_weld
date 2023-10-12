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

using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Basic function to save and load the point file
    /// </summary>
    public class BaseTreatPoint
    {
        public BaseTreatPoint() { }

        /// <summary>
        /// Get Point information from disk
        /// </summary>
        /// <param name="traceinde"> the index of trace </param>
        /// <param name="points"> dictionary of point </param>
        [STAThread]
        internal int GetPoints(int traceIndex, ref Dictionary<int, List<Point>> points)
        {
            int tc = 7;
            string filename = "./Storage/" + traceIndex.ToString() + ".trc";
            var doc = new XmlDocument();

            // only the old file to be read
            try
            {
                doc.Load(filename);
                if (doc.DocumentElement == null)
                    throw new IOException("坐标文件错误");

                XmlElement root = doc.DocumentElement;
                tc = GetXmlRoot(root, ref points);
                // if there an exception, then return default value of 7.
                if (tc == 0) tc = 7;
            }
            catch (IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
            catch (Exception ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
            finally
            {
                // do nothing
            }
            return tc;
        }

        /// <summary>
        /// Save points information to disk
        /// </summary>
        /// <param name="traceIndex"></param>
        /// <param name="points"></param>
        [STAThread]
        internal void SavePoints(int traceIndex, in Dictionary<int, List<Point>> points)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<Trace></Trace>");
            if (doc.DocumentElement == null) return;

            XmlElement root = doc.DocumentElement;
            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.InsertBefore(decl, root);

            XmlComment comment = doc.CreateComment("The record file for the sun flower welding system");
            doc.InsertBefore(comment, root);

            // the trace node, the first node
            XmlElement count = doc.CreateElement("Count");
            root.InsertAfter(count, root.FirstChild);

            XmlElement trace = doc.CreateElement("trace");
            root.InsertAfter(trace, count);

            AddXmlElement(doc, trace, "type", "vane");

            XmlElement material = doc.CreateElement("material");
            trace.AppendChild(material);
            AddXmlBranch(doc, material);

            try
            {
                for (int i = 0; i <= 14; i++)
                {
                    XmlElement subtrace = doc.CreateElement("subtrace");
                    trace.AppendChild(subtrace);
                    AddXmlSubtrace(doc, subtrace, i, points);
                }
            }
            catch (Exception ex)
            {
                new Werr().WerrMessage(ex.Message);
            }

            try
            {
                string filename = "./Storage/" + traceIndex.ToString() + ".trc";
                doc.Save(filename);
            }
            catch (IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
        }

        //
        // Add Xml Element to the Xml file node
        //
        private static void AddXmlElement(XmlDocument doc, XmlElement em, string name, string fvalue)
        {
            XmlElement var = doc.CreateElement(name);
            var.InnerText = fvalue;
            em.AppendChild(var);
        }

        //
        // Add xml branch to material
        //
        private static void AddXmlBranch(XmlDocument doc, XmlElement em)
        {
            XmlElement xe = doc.CreateElement("mtype");
            xe.InnerText = "1";
            em.AppendChild(xe);

            xe = doc.CreateElement("thickness");
            xe.InnerText = "0";
            em.AppendChild(xe);

            xe = doc.CreateElement("wire");
            xe.InnerText = "0";
            em.AppendChild(xe);
        }

        //
        // Add xml points to the subtrace
        // iNum: the order
        //
        private static void AddXmlSubtrace(XmlDocument doc, XmlElement em, int iNum,
            Dictionary<int, List<Point>> points)
        {
            int ii;
            XmlElement xe = doc.CreateElement("order");
            if (iNum <= 7)
            {
                ii = iNum;
                xe.InnerText = iNum.ToString();
            }
            else
            {
                ii = 7 - iNum;
                xe.InnerText = (7 - iNum).ToString();
            }

            em.AppendChild(xe);

            var pts = points[ii];

            if (pts == null) return;
            int c = pts.Count;
            for (int i = 0; i < c; i++)
            {
                XmlElement xpt = doc.CreateElement("point");
                em.AppendChild(xpt);
                if (pts != null) AddXmlPoint(doc, xpt, ii, i, pts);
            }
        }

        //
        // Add xml points to the point of the subtrace
        // iNum: the order
        // pNum: the point number
        //
        private static void AddXmlPoint(XmlDocument doc, XmlElement em, int iNum, int pNum, List<Point> pts)
        {
            XmlElement xnm = doc.CreateElement("number");
            if (iNum == 0) xnm.InnerText = pNum.ToString();
            else xnm.InnerText = (pNum + 1).ToString();

            em.AppendChild(xnm);

            XmlElement xlt = doc.CreateElement("linetype");
            if (iNum == 0) xlt.InnerText = "0";
            else xlt.InnerText = "1";

            em.AppendChild(xlt);

            XmlElement xco = doc.CreateElement("coordinate");
            em.AppendChild(xco);
            AddXmlCoordinate(doc, xco, pNum, pts);

            XmlElement xla = doc.CreateElement("laser");
            em.AppendChild(xla);
            AddXmlLaser(doc, xla, pNum, pts);
        }

        //
        // Add xml point's coordinate
        //
        private static void AddXmlCoordinate(XmlDocument doc, XmlElement em, int iNum, List<Point> pts)
        {
            XmlElement xx = doc.CreateElement("x");
            xx.InnerText = ((int)pts[iNum].vector.X).ToString();
            em.AppendChild(xx);

            XmlElement xy = doc.CreateElement("y");
            xy.InnerText = ((int)pts[iNum].vector.Y).ToString();
            em.AppendChild(xy);

            XmlElement xz = doc.CreateElement("z");
            xz.InnerText = ((int)pts[iNum].vector.Z).ToString();
            em.AppendChild(xz);
        }

        //
        // Add xml point's laser parameters
        //
        private static void AddXmlLaser(XmlDocument doc, XmlElement em, int iNum, List<Point> pts)
        {
            XmlElement xp = doc.CreateElement("power");
            xp.InnerText = pts[iNum].LaserPointPower.ToString();
            em.AppendChild(xp);

            var lp = pts[iNum].GetLaserParameter();

            XmlElement xf = doc.CreateElement("frequency");
            xf.InnerText = "0";
            em.AppendChild(xf);

            XmlElement xl = doc.CreateElement("pulse");
            xl.InnerText = "0";
            em.AppendChild(xl);

            XmlElement xd = doc.CreateElement("duty");
            xd.InnerText = "0";
            em.AppendChild(xd);

            XmlElement xw = doc.CreateElement("wobble");
            xw.InnerText = "0";
            em.AppendChild(xw);

            XmlElement xs = doc.CreateElement("weldspeed");
            xs.InnerText = lp.WeldSpeed.ToString();
            em.AppendChild(xs);

            XmlElement xe = doc.CreateElement("leap");
            xe.InnerText = lp.Leap.ToString();
            em.AppendChild(xe);
        }

        //
        //---- Get functions ----
        //

        //
        // Get childNotes from root
        //
        private static int GetXmlRoot(XmlElement rt, ref Dictionary<int, List<Point>> pts)
        {
            int tc = 0;
            XmlNode? et = rt.SelectSingleNode("count");
            if (et != null)
            {
                tc = Convert.ToInt32(et.InnerText);
            }

            XmlNode? em = rt.SelectSingleNode("trace");
            if (em == null) return 0;

            foreach (XmlNode node in em.ChildNodes)
            {
                if (node.Name == "subtrace")
                    GetXmlSubtrace(node, ref pts);
            }

            return tc;
        }

        //
        // Get points and order from subtrace branch
        //
        private static void GetXmlSubtrace(XmlNode nd, ref Dictionary<int, List<Point>> pts)
        {
            XmlNode? od = nd.SelectSingleNode("order");
            if (od == null) return;

            var ikey = Convert.ToInt32(od.InnerText);
            var pvalue = GetXmlList(ikey, nd);

            if (pvalue.Count > 0) pts.Add(ikey, pvalue);
        }

        //
        // Get point list
        // ik: the order
        //
        private static List<Point> GetXmlList(int ik, XmlNode nd)
        {
            var retList = new List<Point>();
            var pdic = new Dictionary<int, Point>();

            XmlNodeList? pdlist = nd.SelectNodes("point");
            if (pdlist == null) return retList;

            foreach (XmlNode pd in pdlist)
            {
                int iNum = GetXmlPoint(pd, out Point pt);
                pdic.Add(iNum, pt);
            }

            SortDic(ik, in pdic, ref retList);
            return retList;
        }

        //
        // Get point dictionary, which may be disorder
        //
        private static int GetXmlPoint(in XmlNode pd, out Point pt)
        {
            int ret;

            XmlNode? xnum = pd.SelectSingleNode("number");
            if (xnum == null) ret = 0;
            else ret = Convert.ToInt32(xnum.InnerText);

            int lt;
            XmlNode? xline = pd.SelectSingleNode("linetype");
            if (xline == null) lt = 0;
            else lt = Convert.ToInt32(xline.InnerText);

            XmlNode? xcoord = pd.SelectSingleNode("coordinate");
            var vc = new Vector();
            if (xcoord != null) GetXmlCoordinate(xcoord, ref vc);

            XmlNode? xlaser = pd.SelectSingleNode("laser");
            var lp = new LaserParameter();
            if (xlaser != null) GetXmlLaser(xlaser, ref lp);

            int ls;
            if (lp.Power != 0) ls = 1;
            else ls = 0;

            pt = new Point(lt, ls, lp, vc);
            return ret;
        }

        //
        // Get Coordinate of each point
        //
        private static void GetXmlCoordinate(XmlNode xcoord, ref Vector vc)
        {
            XmlNode? x = xcoord.SelectSingleNode("x");
            XmlNode? y = xcoord.SelectSingleNode("y");
            XmlNode? z = xcoord.SelectSingleNode("z");

            if (x != null && y != null && z != null)
            {
                vc.X = Convert.ToInt32(x.InnerText);
                vc.Y = Convert.ToInt32(y.InnerText);
                vc.Z = Convert.ToInt32(z.InnerText);
            }
        }

        //
        // Get Laser parameter of each point
        //
        private static void GetXmlLaser(XmlNode xlaser, ref LaserParameter lp)
        {
            XmlNode? p = xlaser.SelectSingleNode("power");
            XmlNode? f = xlaser.SelectSingleNode("frequency");
            XmlNode? u = xlaser.SelectSingleNode("pulse");
            XmlNode? d = xlaser.SelectSingleNode("duty");
            XmlNode? w = xlaser.SelectSingleNode("wobble");
            XmlNode? s = xlaser.SelectSingleNode("weldspeed");
            XmlNode? l = xlaser.SelectSingleNode("leap");

            if (p != null && f != null && u != null
                && d != null && w != null && s != null && l != null)
            {
                lp.Power = Convert.ToInt32(p.InnerText);
                lp.Frequency = Convert.ToInt32(f.InnerText);
                lp.Pulse = Convert.ToInt32(u.InnerText);
                lp.Duty = Convert.ToInt32(d.InnerText);
                lp.Wobble = Convert.ToInt32(w.InnerText);
                lp.WeldSpeed = Convert.ToInt32(s.InnerText);
                lp.Leap = Convert.ToInt32(l.InnerText);
            }
        }

        //
        // Sort the point dictionary in the ascending order
        // ik: the order
        //
        private static void SortDic(int ik, in Dictionary<int, Point> pdic, ref List<Point> ptlist)
        {
            int len;
            try
            {
                if (ik == 0)
                {
                    if (pdic.Count < 3) throw new Exception("轨迹文件缺少原点");
                    len = 3;
                }
                else
                {
                    if (pdic.Count < 7) throw new Exception("轨迹文件缺少点位");
                    len = 7;
                }

                var pa = new Point[len];
                foreach (var p in pdic)
                {
                    if (len == 3) pa[p.Key] = p.Value;
                    else pa[p.Key - 1] = p.Value;
                }

                ptlist = pa.ToList();
            }
            catch (Exception ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
            finally
            {
                // do nothing
            }
        }
    }
}
