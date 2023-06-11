using RobotWeld2.GetTrace;
using RobotWeld2.Welding;
using System;
using System.IO;
using System.IO.Packaging;
using System.Xml;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Basic function to treat file
    /// </summary>
    public class BaseTreatFile
    {
        private readonly DaemonFile dmf;

        public BaseTreatFile(DaemonFile dmf)
        {
            this.dmf = dmf;
        }

        //
        // open the record file
        //
        [STAThread]
        public void Open(string filename)
        {
            XmlElement root;
            try
            {
                XmlDocument doc = new();
                doc.Load(filename);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch
            {
                Assertion.AssertError("装载记录文件错误", 0);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                ScanChildNode(em);
            }
        }

        // Scan each child nodes to get data
        private void ScanChildNode(XmlNode em)
        {
            foreach (XmlNode xn in em.ChildNodes)
            {
                if (xn.Name == "fileindex")
                {
                    GetFileData(xn, out int _fileIndex);
                    if (dmf is not null)
                        dmf.FileIndex = _fileIndex;
                }
                else if (xn.Name == "trace")
                {
                    int[] ivar = new int[2];
                    Tracetype tracetype = new();
                    GetTraceData(xn, ivar, ref tracetype);

                    if (dmf is not null)
                    {
                        dmf.TraceIndex = ivar[0];
                        dmf.TraceCode = ivar[1];
                        dmf.TraceType = tracetype;
                    }
                }
                else if (xn.Name == "material")
                {
                    double[] dvar = new double[3];
                    GetMaterialData(xn, dvar);

                    if (dmf is not null)
                    {
                        dmf.MaterialIndex = (int)dvar[0];
                        dmf.SheetThickness = dvar[1];
                        dmf.WireDiameter = dvar[2];
                    }
                }
                else if (xn.Name == "laser")
                {
                    int[] ivar = new int[14];
                    GetLaserData(xn, ivar);

                    if (dmf is not null)
                    {
                        dmf.LaserPower = ivar[0];
                        dmf.Frequency = ivar[1];
                        dmf.PulseWidth = ivar[2];
                        dmf.DutyCycle = ivar[3];
                        dmf.LaserRise = ivar[4];
                        dmf.LaserFall = ivar[5];
                        dmf.LaserHoldtime = ivar[6];
                        dmf.WireTime = ivar[7];
                        dmf.AirIn = ivar[8];
                        dmf.AirOut = ivar[9];
                        dmf.WeldSpeed = ivar[10];
                        dmf.WobbleSpeed = ivar[11];
                        dmf.LeapSpeed = ivar[12];
                        dmf.MaxPower = ivar[13];
                    }
                }
            }
        }

        //
        // get the material data from the record file
        //
        private static void GetFileData(XmlNode xn, out int ivar)
        {
            string svar = xn.InnerText;
            ivar = Convert.ToInt32(svar);
        }

        //
        // get the trace information
        //
        private static void GetTraceData(XmlNode xn, int[] ivar, ref Tracetype tracetype)
        {
            string svar = RetrieveAttribute(xn, "index");
            ivar[0] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "code");
            ivar[1] = Convert.ToInt32(svar);

            string svar2 = RetrieveAttribute(xn, "tracetype");
            if (svar2.Equals("van"))
            {
                tracetype = Tracetype.VANE_WHEEL;
            }
            else
            {
                tracetype = Tracetype.NONE;
            }
        }

        //
        // get the material infromation
        //
        private static void GetMaterialData(XmlNode xn, double[] dvar)
        {
            string svar = RetrieveAttribute(xn, "type");
            dvar[0] = Convert.ToDouble(svar);

            svar = RetrieveAttribute(xn, "thick");
            dvar[1] = Convert.ToDouble(svar);

            svar = RetrieveAttribute(xn, "diameter");
            dvar[2] = Convert.ToDouble(svar);
        }

        //
        // get the laser data from the record file
        //
        private static void GetLaserData(XmlNode xn, int[] dvar)
        {
            string svar = RetrieveAttribute(xn, "power");
            dvar[0] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "frequency");
            dvar[1] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "pulse");
            dvar[2] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "duty");
            dvar[3] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "rise");
            dvar[4] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "fall");
            dvar[5] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "hold");
            dvar[6] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "wire");
            dvar[7] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "in");
            dvar[8] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "out");
            dvar[9] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "weld");
            dvar[10] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "wobble");
            dvar[11] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "leap");
            dvar[12] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "maxpower");
            dvar[13] = Convert.ToInt32(svar);
        }

        //
        // get attribute from the nodes
        //
        private static string RetrieveAttribute(XmlNode nd, string name)
        {
            XmlNode? var = nd.SelectSingleNode(name);
            if (var is not null)
            {
                return var.InnerText;
            }
            else
            {
                new Werr().WerrMessage(name + "记录文件不完整!");
                return "";
            }
        }

        /// <summary>
        /// save the data detail to the file
        /// </summary>
        /// <param name="filename"> File name to be save </param>
        [STAThread]
        public void Save(string filename)
        {
            // only the new trace is code = 0
            // trace code is the version sub-number
            // Aftet each saving, it increase one. Just record save times.
            dmf.TraceCode++;

            XmlDocument doc = new();
            doc.LoadXml("<Projects></Projects>");
            if (doc.DocumentElement is null) { return; }

            try
            {
                XmlElement root = doc.DocumentElement;

                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.InsertBefore(decl, root);

                XmlComment comment = doc.CreateComment(
                    "The record file for the sun flower welding system");
                doc.InsertBefore(comment, root);

                // the file node
                XmlElement project = doc.CreateElement("project");
                root.InsertAfter(project, root.FirstChild);
                AddXmlElement(doc, project, "fileindex", dmf.FileIndex.ToString());

                // the trace node
                XmlElement trace = doc.CreateElement("trace");
                project.AppendChild(trace);
                AddXmlElement(doc, trace, "index", dmf.TraceIndex.ToString());
                AddXmlElement(doc, trace, "code", dmf.TraceCode.ToString());

                string svar = string.Empty;
                Translate(dmf.TraceType, out svar);
                AddXmlElement(doc, trace, "tracetype", svar);

                // the material node
                XmlElement material = doc.CreateElement("material");
                project.AppendChild(material);
                AddXmlElement(doc, material, "type", dmf.MaterialIndex.ToString());
                AddXmlElement(doc, material, "thick", dmf.SheetThickness.ToString());
                AddXmlElement(doc, material, "diameter", dmf.WireDiameter.ToString());

                // the laser node
                XmlElement laser = doc.CreateElement("laser");
                project.AppendChild(laser);
                AddXmlElement(doc, laser, "power", dmf.LaserPower.ToString());
                AddXmlElement(doc, laser, "frequency", dmf.Frequency.ToString());
                AddXmlElement(doc, laser, "pulse", dmf.PulseWidth.ToString());
                AddXmlElement(doc, laser, "duty", dmf.DutyCycle.ToString());
                AddXmlElement(doc, laser, "rise", dmf.LaserRise.ToString());
                AddXmlElement(doc, laser, "fall", dmf.LaserFall.ToString());
                AddXmlElement(doc, laser, "hold", dmf.LaserHoldtime.ToString());
                AddXmlElement(doc, laser, "wire", dmf.WireTime.ToString());
                AddXmlElement(doc, laser, "in", dmf.AirIn.ToString());
                AddXmlElement(doc, laser, "out", dmf.AirOut.ToString());
                AddXmlElement(doc, laser, "weld", dmf.WeldSpeed.ToString());
                AddXmlElement(doc, laser, "wobble", dmf.WobbleSpeed.ToString());
                AddXmlElement(doc, laser, "leap", dmf.LeapSpeed.ToString());
                AddXmlElement(doc, laser, "maxpower", dmf.MaxPower.ToString());

                doc.Save(filename);
            }
            catch (IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
            }
        }

        //
        // Translate trace type to file suffixes
        //
        private void Translate(Tracetype traceType, out string svar)
        {
            switch (traceType)
            {
                case Tracetype.VANE_WHEEL:
                    svar = "van";
                    break;
                default:
                    svar = "none";
                    break;
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
