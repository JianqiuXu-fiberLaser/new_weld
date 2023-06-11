using RobotWeld.Welding;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;

namespace RobotWeld.AlgorithmsBase
{
    /// <summary>
    /// the class to load and open the recording file.
    /// </summary>
    public class WeldFileAccess
    {
        private DaemonFile? dmFile;

        public WeldFileAccess() { }

         // load the record file at the start of the weldding system.
        public void WeldLoad(DaemonFile dmt, string filename)
        {
            dmFile = dmt;
            Open(filename);
        }

        public void OpenFile(DaemonFile dmt, string filename)
        {
            dmFile = dmt;
            Open(filename);
        }

        // Scan each child nodes to get data
        private void ScanChildNode(XmlNode em)
        {
            foreach (XmlNode xn in em.ChildNodes)
            {
                if (xn.Name == "fileindex")
                {
                    GetFileData(xn, out int _fileIndex);
                    if (dmFile is not null)
                        dmFile.FileIndex = _fileIndex;
                }
                else if (xn.Name == "trace")
                {
                    int[] ivar = new int[2];
                    GetTraceData(xn, ivar);

                    if (dmFile is not null)
                    {
                        dmFile.TraceIndex = ivar[0];
                        dmFile.TraceCode = ivar[1];
                    }
                }
                else if (xn.Name == "material")
                {
                    double[] dvar = new double[3];
                    GetMaterialData(xn, dvar);

                    if (dmFile is not null)
                    {
                        dmFile.MaterialIndex = (int)dvar[0];
                        dmFile.SheetThickness = dvar[1];
                        dmFile.WireDiameter = dvar[2];
                    }
                }
                else if (xn.Name == "laser")
                {
                    int[] ivar = new int[12];
                    GetLaserData(xn, ivar);

                    if (dmFile is not null)
                    {
                        dmFile.LaserPower = ivar[0];
                        dmFile.Frequency = ivar[1];
                        dmFile.PulseWidth = ivar[2];
                        dmFile.DutyCycle = ivar[3];
                        dmFile.LaserRise = ivar[4];
                        dmFile.LaserFall = ivar[5];
                        dmFile.LaserHoldtime = ivar[6];
                        dmFile.WireTime = ivar[7];
                        dmFile.AirIn = ivar[8];
                        dmFile.AirOut = ivar[9];
                        dmFile.WeldSpeed = ivar[10];
                        dmFile.WobbleSpeed = ivar[11];
                    }
                }
            }
        }

        // get the material data from the record file
        private static void GetFileData(XmlNode xn, out int ivar) 
        {
            string svar = xn.InnerText;
            ivar = Convert.ToInt32(svar);
        }

        private static void GetTraceData(XmlNode xn, int[] ivar)
        {
            string svar = RetrieveAttribute(xn, "index");
            ivar[0] = Convert.ToInt32(svar);

            svar = RetrieveAttribute(xn, "code");
            ivar[1] = Convert.ToInt32(svar);
        }

        private static void GetMaterialData(XmlNode xn, double[] dvar)
        {
            string svar = RetrieveAttribute(xn, "type");
            dvar[0] = Convert.ToDouble(svar);

            svar = RetrieveAttribute(xn, "thick");
            dvar[1] = Convert.ToDouble(svar);

            svar = RetrieveAttribute(xn, "diameter");
            dvar[2] = Convert.ToDouble(svar);
        }

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
        }

        // get attribute from the nodes
        private static string RetrieveAttribute(XmlNode nd, string name)
        { 
            XmlNode? var = nd.SelectSingleNode(name);
            if (var is not null)
            {
                return var.InnerText;
            }
            else
            {
                new Werr().WerrMessage(name + "Record file has an error!");
                return "";
            }
        }

        // open the record file
        [STAThread]
        private void Open(string filename)
        {
            XmlElement root;
            try
            {
                XmlDocument doc = new();
                doc.Load(filename);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;
            }
            catch (IOException ex)
            {
                new Werr().WerrMessage(ex.Message);
                return;
            }

            XmlNode? em = root.SelectSingleNode("project");
            if (em is not null)
            {
                ScanChildNode(em);
            }
        }
    }
}
