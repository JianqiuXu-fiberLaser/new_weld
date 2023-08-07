using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// read and write material file
    /// </summary>
    public class AccessMaterialFile
    {
        public AccessMaterialFile() { }

        //
        // Read material file
        //
        public void ReadMaterialFile(WorkPackage workPackage, string materialFile)
        {
            XmlElement root;
            // only the old file to be read
            try
            {
                XmlDocument doc = new();
                string rfname = "./Storage/" + materialFile + ".mat";
                if (File.Exists(rfname))
                {
                    doc.Load(rfname);
                    if (doc.DocumentElement == null) { return; }
                    root = doc.DocumentElement;
                }
                else
                {
                    throw new Exception();
                }

                XmlNode? em = root.SelectSingleNode("material");
                if (em is not null)
                {
                    MaterialFileParse(workPackage, em);
                }
            }
            catch
            {
                Assertion.AssertError("装载材料文件错误", 1);
                return;
            }
        }

        private void MaterialFileParse(WorkPackage workPackage, XmlNode em)
        {
            MetalMaterial metalMaterial = new();

            foreach (XmlNode xn in em.ChildNodes)
            {
                if (xn.Name == "type")
                {
                    metalMaterial.MaterialType = Convert.ToInt32(xn.InnerText);
                }
                if (xn.Name == "thickness")
                {
                    metalMaterial.Thickness = Convert.ToDouble(xn.InnerText);
                }
                if (xn.Name == "wire")
                {
                    metalMaterial.WireDiameter = Convert.ToDouble(xn.InnerText);
                }
            }

            if (workPackage != null)
            {
                workPackage.MetalMaterial = metalMaterial;
            }
        }

        /// <summary>
        /// Write material file to the disk
        /// </summary>
        /// <param name="metalMaterial"></param>
        [STAThread]
        public void WriteMaterialFile(WorkPackage workPackage)
        {
            MetalMaterial metalMaterial = workPackage.MetalMaterial;

            try
            {
                XmlDocument doc = new();
                doc.LoadXml("<Material></Material>");

                if (doc.DocumentElement == null) { return; }
                XmlElement root = doc.DocumentElement;

                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.InsertBefore(decl, root);

                XmlComment comment = doc.CreateComment("The record file for the sun flower welding system");
                doc.InsertBefore(comment, root);

                // the file node
                XmlElement material = doc.CreateElement("material");
                root.InsertAfter(material, root.FirstChild);
                AddXmlElement(doc, material, "type", metalMaterial.MaterialType.ToString());
                AddXmlElement(doc, material, "thickness", metalMaterial.Thickness.ToString());
                AddXmlElement(doc, material, "wire", metalMaterial.WireDiameter.ToString());

                if (workPackage != null)
                {
                    string rfname = "./Storage/" + workPackage.MaterialIndex.ToString() + ".mat";
                    doc.Save(rfname);
                }
            }
            catch
            {
                Assertion.AssertError("写材料文件错误", 1);
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
    }
}
