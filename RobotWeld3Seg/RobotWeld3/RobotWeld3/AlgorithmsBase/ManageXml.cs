///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System.Xml;

namespace RobotWeld3.AlgorithmsBase
{
    /// <summary>
    /// Basic method to handle Xml element
    /// </summary>

    internal class ManageXml
    {
        //
        // Add Xml Element to the Xml file node
        //
        internal static void AddXmlElement(XmlDocument doc, XmlElement em, string ename, int index)
        {
            XmlElement xi = doc.CreateElement(ename);
            xi.InnerText = index.ToString();
            em.AppendChild(xi);
        }
    }
}
