///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver. 3.0: new class to treat wms list file.
//
///////////////////////////////////////////////////////////////////////

using RobotWeld3.CaluWMS;
using RobotWeld3.Motions;
using RobotWeld3.Welding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Read and write the WMS file.
    /// </summary>
    internal class AccessWmsFile
    {
        public AccessWmsFile() { }

        /// <summary>
        /// Read WMS file
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="timeStamp"> timestampe </param>
        /// <param name="wms"> wms list </param>
        /// <returns> if timestampe is not the same, return false </returns>
        internal static bool Read(string fname, int timestamp, ref List<WeldMoveSection> wms)
        {
            var doc = new XmlDocument();

            try
            {
                doc.Load(fname);
                if (doc.DocumentElement == null) throw new IOException();

                XmlElement root = doc.DocumentElement;
                XmlNode? wmslist = root.SelectSingleNode("wmslist");
                if (wmslist == null) return false;

                var time = wmslist.SelectSingleNode("time");

                if (time != null)
                {
                    var ts = Convert.ToInt32(time.InnerText);
                    if (ts != timestamp) return false;
                }

                var wmsl = wmslist.SelectNodes("wms");
                if (wmsl == null) return false;

                foreach (XmlNode w in wmsl)
                    GetNodeWms(w, ref wms);
            }
            catch (IOException)
            {
                return false;
            }
            catch (Exception ex1)
            {
                Assertion.AssertError(ex1.Message, 02);
            }
            finally
            {
                // do nothing
            }

            return true;
        }

        /// <summary>
        /// Save WMS to file with a given trace Code.
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="timeStamp"> timestampe </param>
        /// <param name="wms"> wms list </param>
        [STAThread]
        internal static void Save(string fname, int timestamp, in List<WeldMoveSection> wms)
        {
            var doc = new XmlDocument();
            doc.LoadXml("<wmsbase></wmsbase>");

            if (doc.DocumentElement is null) return;
            XmlElement root = doc.DocumentElement;

            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.InsertBefore(decl, root);

            XmlComment comment = doc.CreateComment("The weldsection file for the sun flower welding system");
            doc.InsertBefore(comment, root);

            // the time node
            XmlElement wmsl = doc.CreateElement("wmslist");
            root.InsertAfter(wmsl, root.FirstChild);

            XmlElement time = doc.CreateElement("time");
            time.InnerText = timestamp.ToString();
            wmsl.AppendChild(time);

            for (int i = 0; i < wms.Count; i++)
            {
                var ew = doc.CreateElement("wms");
                wmsl.AppendChild(ew);
                AddXmlWms(doc, ew, wms[i]);
            }

            // write to disk
            try
            {
                doc.Save(fname);
            }
            catch (IOException ex)
            {
                Assertion.AssertError(ex.Message, 204);
            }
        }

        //-------------------------------------------------------------
        // wms element to XmlDocument
        //-------------------------------------------------------------

        /// <summary>
        /// add each wms element
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="em"></param>
        /// <param name="wmsi"></param>
        private static void AddXmlWms(XmlDocument doc, XmlElement em, WeldMoveSection wmsi)
        {
            var t = doc.CreateElement("order");
            t.InnerText = wmsi.WmsIndex.ToString();
            em.AppendChild(t);

            var pts = wmsi.GetPosition();
            AddXmlVector(doc, em, pts);

            var spt = wmsi.Speed;
            AddXmlSpeed(doc, em, spt);

            var lps = wmsi.LaserParameter;
            AddXmlLaser(doc, em, lps);

            var mts = wmsi.Material;
            AddXmlMaterial(doc, em, mts);

            var wis = wmsi.Wire;
            AddXmlWire(doc, em, wis);

            t = doc.CreateElement("wobble");
            t.InnerText = wmsi.Wobble.ToString();
            em.AppendChild(t);
        }

        private static void AddXmlVector(XmlDocument doc, XmlElement em, List<Vector> vc)
        {
            var et = doc.CreateElement("vector");
            em.AppendChild(et);

            for (int i = 0; i < vc.Count; i++)
            {
                var t = doc.CreateElement("point");
                t.InnerText = vc[i].X.ToString() + "," + vc[i].Y.ToString() + "," + vc[i].Z.ToString() + "," + vc[i].A.ToString();
                var attr = doc.CreateAttribute("id");
                attr.Value = i.ToString();
                t.SetAttributeNode(attr);
                et.AppendChild(t);
            }
        }

        private static void AddXmlSpeed(XmlDocument doc, XmlElement em, List<double> spt)
        {
            var et = doc.CreateElement("velocity");
            em.AppendChild(et);

            for (int i = 0; i < spt.Count; i++)
            {
                var t = doc.CreateElement("speed");
                t.InnerText = spt[i].ToString();
                var attr = doc.CreateAttribute("id");
                attr.Value = i.ToString();
                t.SetAttributeNode(attr);
                et.AppendChild(t);
            }
        }

        private static void AddXmlLaser(XmlDocument doc, XmlElement em, LaserParameter lps)
        {
            var et = doc.CreateElement("laser");
            em.AppendChild(et);

            var t = doc.CreateElement("power");
            t.InnerText = lps.Power.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("frequency");
            t.InnerText = lps.Frequency.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("duty");
            t.InnerText = lps.DutyCycle.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("rise");
            t.InnerText = lps.LaserRise.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("fall");
            t.InnerText = lps.LaserFall.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("air");
            t.InnerText = lps.Air.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("hold");
            t.InnerText = lps.LaserHoldtime.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("wiretime");
            t.InnerText = lps.WireTime.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("wirespeed");
            t.InnerText = lps.WireSpeed.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("wirelength");
            t.InnerText = lps.WireLength.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("wireback");
            t.InnerText = lps.WireBack.ToString();
            et.AppendChild(t);
        }

        private static void AddXmlMaterial(XmlDocument doc, XmlElement em, MetalMaterial mts)
        {
            var et = doc.CreateElement("material");
            em.AppendChild(et);

            var t = doc.CreateElement("mindex");
            t.InnerText = mts.MaterialType.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("thick");
            t.InnerText = mts.Thickness.ToString();
            et.AppendChild(t);
        }

        private static void AddXmlWire(XmlDocument doc, XmlElement em, Wire wis)
        {
            var et = doc.CreateElement("wire");
            em.AppendChild(et);

            var t = doc.CreateElement("windex");
            t.InnerText = wis.MaterialIndex.ToString();
            et.AppendChild(t);

            t = doc.CreateElement("diameter");
            t.InnerText = wis.Diameter.ToString();
            et.AppendChild(t);
        }

        //-------------------------------------------------------------
        // get wms element from XmlDocument
        //-------------------------------------------------------------

        private static void GetNodeWms(XmlNode em, ref List<WeldMoveSection> wms)
        {
            int order = 0;
            var t = em.SelectSingleNode("order");
            if (t != null) order = Convert.ToInt32(t.InnerText);

            var vc = new List<Vector>();
            t = em.SelectSingleNode("vector");
            if (t != null) GetXmlVector(t, ref vc);

            var spl = new List<double>();
            t = em.SelectSingleNode("velocity");
            if (t != null) GetXmlVelocity(t, ref spl);

            var lp = new LaserParameter();
            t = em.SelectSingleNode("laser");
            if (t != null) GetXmlLaser(t, ref lp);

            var mt = new MetalMaterial();
            t = em.SelectSingleNode("material");
            if (t != null) GetXmlMaterial(t, ref mt);

            var wi = new Wire();
            t = em.SelectSingleNode("wire");
            if (t != null) GetXmlWire(t, ref wi);

            double wo = 0;
            t = em.SelectSingleNode("wobble");
            if (t != null) wo = Convert.ToDouble(t.InnerText);

            var wmi = new WeldMoveSection(vc)
            {
                WmsIndex = order,
                Speed = spl,
                LaserParameter = lp,
                Material = mt,
                Wire = wi,
                Wobble = wo,
            };

            wms.Add(wmi);
        }

        private static void GetXmlVector(XmlNode en, ref List<Vector> vc)
        {
            var t = en.SelectNodes("point");
            if (t == null) return;

            foreach (XmlNode ti in t)
            {
                var s = ti.InnerText.Split(",");
                if (s.Length >= 4)
                {
                    var x = Convert.ToDouble(s[0]);
                    var y = Convert.ToDouble(s[1]);
                    var z = Convert.ToDouble(s[2]);
                    var a = Convert.ToDouble(s[3]);
                    vc.Add(new Vector(x, y, z, a));
                }
            }
        }

        private static void GetXmlVelocity(XmlNode en, ref List<double> spl)
        {
            var t = en.SelectNodes("speed");
            if (t == null) return;

            foreach (XmlNode ti in t)
            {
                var s = Convert.ToDouble(ti.InnerText);
                spl.Add(s);
            }
        }

        private static void GetXmlLaser(XmlNode en, ref LaserParameter lp)
        {
            var t = en.SelectSingleNode("power");
            if (t != null) lp.Power = Convert.ToInt32(t.InnerText);

            t = en.SelectSingleNode("frequcy");
            if (t != null) lp.Frequency = Convert.ToInt32(t.InnerText);

            t = en.SelectSingleNode("duty");
            if (t != null) lp.DutyCycle = Convert.ToInt32(t.InnerText);

            t = en.SelectSingleNode("rise");
            if (t != null) lp.LaserRise = Convert.ToDouble(t.InnerText);

            t = en.SelectSingleNode("fall");
            if (t != null) lp.LaserFall = Convert.ToDouble(t.InnerText);

            t = en.SelectSingleNode("air");
            if (t != null) lp.Air = Convert.ToDouble(t.InnerText);

            t = en.SelectSingleNode("hold");
            if (t != null) lp.LaserHoldtime = Convert.ToDouble(t.InnerText);

            t = en.SelectSingleNode("wiretime");
            if (t != null) lp.WireTime = Convert.ToDouble(t.InnerText);

            t = en.SelectSingleNode("wirespeed");
            if (t != null) lp.WireSpeed = Convert.ToDouble(t.InnerText);

            t = en.SelectSingleNode("wirelength");
            if (t != null) lp.WireLength = Convert.ToDouble(t.InnerText);

            t = en.SelectSingleNode("wireback");
            if (t != null) lp.WireBack = Convert.ToDouble(t.InnerText);
        }

        private static void GetXmlMaterial(XmlNode en, ref MetalMaterial mt)
        {
            var t = en.SelectSingleNode("mindex");
            if (t != null) mt.MaterialType = Convert.ToInt32(t.InnerText);

            t = en.SelectSingleNode("thick");
            if (t != null) mt.Thickness = Convert.ToDouble(t.InnerText);
        }

        private static void GetXmlWire(XmlNode en, ref Wire wi)
        {
            var t = en.SelectSingleNode("wirelength");
            if (t != null) wi.MaterialIndex = Convert.ToInt32(t.InnerText);

            t = en.SelectSingleNode("diameter");
            if (t != null) wi.Diameter = Convert.ToDouble(t.InnerText);
        }
    }
}
