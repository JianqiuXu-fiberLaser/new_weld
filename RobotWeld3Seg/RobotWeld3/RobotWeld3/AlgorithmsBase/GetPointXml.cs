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
using System;
using System.Collections.Generic;
using System.Xml;

namespace RobotWeld3.AlgorithmsBase
{
    internal class GetPointXml
    {
        private readonly WorkPackage _wk;
        private double _wire;
        private int _material;
        private double _thick;

        public GetPointXml(WorkPackage wk)
        {
            _wk = wk;
        }

        /// <summary>
        /// Read subtrace information from Xml doc.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="wk"></param>
        internal void GetXmlRoot(XmlElement rt)
        {
            XmlNode? em = rt.SelectSingleNode("trace");
            if (em == null) return;

            foreach (XmlNode node in em.ChildNodes)
            {
                if (node.Name == "type") GetXmlTracetype(node);
                if (node.Name == "time") GetXmlTime(node);
                if (node.Name == "parameter") GetXmlParameter(node);
                if (node.Name == "material") GetXmlMaterial(node);
                if (node.Name == "wire") GetXmlWire(node);
                if (node.Name == "subtrace") GetXmlSubtrace(node);
            }
        }

        /// <summary>
        /// Get trace type
        /// </summary>
        /// <param name="nd"></param>
        private void GetXmlTracetype(XmlNode nd)
        {
            _wk.Tracetype = CTraceType.GetTrace(nd.InnerText);
        }

        private void GetXmlTime(XmlNode nd)
        {
            _wk.TimeStamp = Convert.ToInt32(nd.InnerText);
        }

        private void GetXmlParameter(XmlNode nd)
        {
            var svar = nd.InnerText.Split(",");
            var pml = new List<double>();

            for (int i = 0; i < svar.Length; i++)
            {
                var s = Convert.ToDouble(svar[i]);
                pml.Add(s);
            }

            _wk.SetParameter(pml);
        }

        private void GetXmlMaterial(XmlNode nd)
        {
            var en1 = nd.SelectSingleNode("mtype");
            if (en1 != null) _material = Convert.ToInt32(en1.InnerText);

            var en2 = nd.SelectSingleNode("thickness");
            if (en2 != null) _thick = Convert.ToDouble(en2.InnerText);

            _wk.Material = new MetalMaterial()
            {
                MaterialType = _material,
                Thickness = _thick
            };
        }

        private void GetXmlWire(XmlNode nd)
        {
            _wire = Convert.ToDouble(nd.InnerText);

            _wk.Wire = new Wire()
            {
                Diameter = _wire,
            };
        }

        private void GetXmlSubtrace(XmlNode nd)
        {
            // Here, we read the first subtrace only.
            // For more subtraces, will use script to assembly them.
            var en = nd.SelectSingleNode("order");
            if (en != null) _wk.Order = Convert.ToInt32(en.InnerText);

            var dic = new Dictionary<int, DisPoint>();
            var eds = nd.SelectNodes("point");
            if (eds == null) return;
            foreach (XmlNode ed in eds)
            {
                GetTracePoint(ed, ref dic);
            }

            var p = ManagePointList.SortPointDic(in dic);
            _wk.SetPointList(p);
        }

        private void GetTracePoint(XmlNode nd, ref Dictionary<int, DisPoint> dic)
        {
            int rk = 0;
            XmlNode? xnum = nd.SelectSingleNode("number");
            if (xnum != null) rk = Convert.ToInt32(xnum.InnerText);

            var vc = new Vector();
            XmlNode? xcoord = nd.SelectSingleNode("coordinate");
            if (xcoord != null) vc = GetCoordinate(xcoord);

            int lt = 0;
            XmlNode? xltype = nd.SelectSingleNode("ltype");
            if (xltype != null) lt = Convert.ToInt32(xltype.InnerText);

            int p = 0, f = 0, u = 0;
            XmlNode? xlaser = nd.SelectSingleNode("laser");
            if (xlaser != null) (p, f, u) = GetLaser(xlaser);

            int wb = 0;
            XmlNode? xwb = nd.SelectSingleNode("wobble");
            if (xwb != null) wb = Convert.ToInt32(xwb.InnerText);

            double sp = 0;
            XmlNode? xsp = nd.SelectSingleNode("speed");
            if (xsp != null) sp = Convert.ToInt32(xsp.InnerText);

            var dp = new DisPoint(p, f, u, _material, _thick, _wire, sp, wb, vc, lt);
            dic.Add(rk, dp);
        }

        private static Vector GetCoordinate(XmlNode xcoord)
        {
            var c = xcoord.InnerText;
            var s = c.Split(',');

            var x = new double[4] { 0, 0, 0, 0 };
            int it = (s.Length < 4) ? s.Length : 4;

            for (int i = 0; i < it; i++)
                x[i] = Convert.ToDouble(s[i]);

            return new Vector(x[0], x[1], x[2], x[3]);
        }

        private static (int p, int f, int u) GetLaser(XmlNode la)
        {
            int p;
            XmlNode? xp = la.SelectSingleNode("power");
            if (xp == null) p = 0;
            else p = Convert.ToInt32(xp.InnerText);

            int f;
            XmlNode? xf = la.SelectSingleNode("frequency");
            if (xf == null) f = 0;
            else f = Convert.ToInt32(xf.InnerText);

            int d;
            XmlNode? xd = la.SelectSingleNode("duty");
            if (xd == null) d = 0;
            else d = Convert.ToInt32(xd.InnerText);

            return (p, f, d);
        }
    }

}
