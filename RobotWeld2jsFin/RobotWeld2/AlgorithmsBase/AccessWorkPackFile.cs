using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RobotWeld2.AlgorithmsBase
{
    /// <summary>
    /// Read and write WorkPackage file
    /// </summary>
    public class AccessWorkPackFile
    {
        private LaserWeldParameter _laserWeldParameter;

        public AccessWorkPackFile()
        {
            _laserWeldParameter = new LaserWeldParameter();
        }

        /// <summary>
        /// Open the work package file
        /// </summary>
        /// <param name="workPackage"></param>
        /// <param name="typeIndex"></param>
        /// <param name="current"></param>
        [STAThread]
        public void OpenPackageFile(WorkPackage workPackage, int typeIndex, string current)
        {
            if (workPackage == null) { return; }

            // only the old file to be read
            string rfname = "./Storage/" + typeIndex.ToString() + current + ".trc";
            try
            {
                FileStream afile = new(rfname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new(afile);
                if (File.Exists(rfname))
                {
                    List<string> lines = new();
                    string? line = sr.ReadLine();
                    while (line != null)
                    {
                        lines.Add(line);
                        line = sr.ReadLine();
                    }

                    if (lines.Count >= 2)
                    {
                        string[] svar = lines[0].Split(',');
                        if (svar[1] != current)
                        {
                            Assertion.AssertError("装载轨迹文件错误", 1);
                            return;
                        }

                        svar = lines[1].Split(",");
                        if (workPackage != null && !string.IsNullOrEmpty(svar[1]))
                        {
                            workPackage.MaterialIndex = Convert.ToInt32(svar[1]);
                            AccessMaterialFile accessMaterialFile = new();
                            accessMaterialFile.ReadMaterialFile(workPackage, svar[1]);
                            PointFileParse(workPackage, lines);
                        }
                    }
                    else
                    {
                        throw new Exception("轨迹文件为空");
                    }
                }
                else
                {
                    throw new Exception("轨迹文件不存在");
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                Assertion.AssertError(ex.Message, 201);
            }
        }

        private void PointFileParse(WorkPackage? workPackage, List<string> lines)
        {
            if (workPackage == null) { return; }

            string[] svar;
            if (lines.Count <= 2)
            {
                Vector vector = new();
                Point pt = new(0, vector, 0, new LaserWeldParameter());
                workPackage.AddPoint(pt);
                return;
            }

            int oldLpIndex = -1;
            for (short i = 2; i < lines.Count; i++)
            {
                svar = lines[i].Split(',');
                if (svar == null || svar.Length < 6 || !CheckStringNull(svar))
                {
                    continue;
                }

                double x = Convert.ToDouble(svar[0]);
                double y = Convert.ToDouble(svar[1]);
                double z = Convert.ToDouble(svar[2]);
                double a = Convert.ToDouble(svar[3]);
                Vector vector = new(x, y, z);

                int lt = Convert.ToInt32(svar[4]);
                int lpIndex = Convert.ToInt32(svar[5]);

                if (lpIndex == 0)
                {
                    Point pt = new(lt, vector, (int)a, new LaserWeldParameter(0));
                    workPackage.AddPoint(pt);
                }
                else if (lpIndex != oldLpIndex)
                {
                    OpenLaserWeldParameterFile(lpIndex);
                    Point pt = new(lt, vector, (int)a, _laserWeldParameter);
                    workPackage.AddPoint(pt);
                    oldLpIndex = lpIndex;
                }
                else
                {
                    // if the index is the same, the parameter is referent to the same address.
                    Point pt = new(lt, vector, (int)a)
                    {
                        Parameter = _laserWeldParameter
                    };
                    workPackage.AddPoint(pt);
                }
            }
        }

        private static bool CheckStringNull(string[] st)
        {
            for (int i = 0; i < st.Length; i++)
            {
                if (string.IsNullOrEmpty(st[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private void OpenLaserWeldParameterFile(int lpIndex)
        {
            string rfname = "./Storage/" + lpIndex.ToString() + ".lwp";
            XmlElement root;

            try
            {
                XmlDocument doc = new();
                // only the old file to be read
                if (File.Exists(rfname))
                {
                    doc.Load(rfname);
                    if (doc.DocumentElement == null)
                    {
                        _laserWeldParameter = new LaserWeldParameter();
                        return;
                    }
                    root = doc.DocumentElement;
                }
                else
                {
                    throw new Exception();
                }

                XmlNode? em = root.SelectSingleNode("laser");
                if (em is not null)
                {
                    LaserFileParse(lpIndex, em);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                Assertion.AssertError("装载激光文件错误", 1);
                _laserWeldParameter = new LaserWeldParameter(0);
                return;
            }
        }

        private void LaserFileParse(int lpIndex, XmlNode em)
        {
            _laserWeldParameter = new() { ParaIndex = lpIndex };

            string svar;
            foreach (XmlNode xn in em.ChildNodes)
            {
                if (xn.Name == "power")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.LaserPower = Convert.ToInt32(svar);
                }
                if (xn.Name == "frequency")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.Frequency = Convert.ToInt32(svar);
                }
                if (xn.Name == "pulse")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.PulseWidth = Convert.ToInt32(svar);
                }
                if (xn.Name == "duty")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.DutyCycle = Convert.ToInt32(svar);
                }
                if (xn.Name == "rise")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.LaserRise = Convert.ToInt32(svar);
                }
                if (xn.Name == "fall")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.LaserFall = Convert.ToInt32(svar);
                }
                if (xn.Name == "hold")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.LaserHoldTime = Convert.ToInt32(svar);
                }
                if (xn.Name == "airin")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.AirIn = Convert.ToInt32(svar);
                }
                if (xn.Name == "airout")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.AirIn = Convert.ToInt32(svar);
                }
                if (xn.Name == "wiretime")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.WireTime = Convert.ToInt32(svar);
                }
                if (xn.Name == "weldspeed")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.WeldSpeed = Convert.ToInt32(svar);
                }
                if (xn.Name == "wobble")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.WobbleSpeed = Convert.ToInt32(svar);
                }
                if (xn.Name == "leap")
                {
                    svar = xn.InnerText;
                    _laserWeldParameter.LeapSpeed = Convert.ToInt32(svar);
                }
            }
        }

        /// <summary>
        /// Change the current index or create a new package file.
        /// Then, write work Package to the disk. 
        /// </summary>
        /// <param name="workPackage"> work package </param>
        /// <param name="fileName"> File name of workpackage </param>
        /// <param name="stype"> string for the trace type file </param>
        public void WriteIndexFile(WorkPackage workPackage, string fileName, string stype)
        {
            if (File.Exists(fileName))
            {
                ChangeFileIndex(workPackage, fileName, stype);
            }
            else
            {
                CreateFileIndex(workPackage, fileName, stype);
            }
            WritePackageFile(workPackage, stype);
        }

        [STAThread]
        private static void ChangeFileIndex(WorkPackage workPackage, string fileName, string stype)
        {
            XmlDocument doc = new();
            XmlElement root;

            try
            {
                doc.Load(fileName);
                if (doc.DocumentElement == null) { return; }
                root = doc.DocumentElement;

                XmlNode? em = root.SelectSingleNode("project");
                if (em is not null)
                {
                    foreach (XmlNode xn in em.ChildNodes)
                    {
                        if (xn.Name == stype)
                            xn.InnerText = workPackage.TypeIndex.ToString();

                        if (xn.Name == "current")
                            xn.InnerText = stype;
                    }
                }
            }
            catch
            {
                Assertion.AssertError("记录文件错误", 1);
            }

            doc.Save(fileName);
        }

        [STAThread]
        private static void CreateFileIndex(WorkPackage workPackage, string fileName, string stype)
        {
            XmlDocument doc = new();
            XmlElement root;

            doc.LoadXml("<Projects></Projects>");
            if (doc.DocumentElement is null) { return; }

            try
            {
                root = doc.DocumentElement;

                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.InsertBefore(decl, root);

                XmlComment comment = doc.CreateComment("The record file for the sun flower welding system");
                doc.InsertBefore(comment, root);

                // the file node
                XmlElement project = doc.CreateElement("project");
                root.InsertAfter(project, root.FirstChild);

                // except the current type, others are initialized to index of 000
                foreach (string svar in _nodeName)
                {
                    if (svar == stype)
                    {
                        AddXmlElement(doc, project, stype, workPackage.TypeIndex.ToString());
                    }
                    else
                    {
                        AddXmlElement(doc, project, svar, "000");
                    }
                }

                XmlNode? em = root.SelectSingleNode("project");
                if (em is not null)
                {
                    foreach (XmlNode xn in em.ChildNodes)
                    {
                        if (xn.Name == stype)
                            xn.InnerText = workPackage.TypeIndex.ToString();

                        if (xn.Name == "current")
                            xn.InnerText = stype;
                    }
                }

            }
            catch (IOException ex)
            {
                Assertion.AssertError(ex.Message, 102);
            }

            doc.Save(fileName);
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

        private static readonly List<string> _nodeName = new()
        {
            "vane",
            "intersect",
            "stage",
            "spiral",
            "free",
            "current",
        };

        //
        // Write package information to the ".trc" file
        //
        [STAThread]
        private static void WritePackageFile(WorkPackage workPackage, string stype)
        {
            if (workPackage == null) { return; }

            int typeIndex = workPackage.TypeIndex;
            int materialFileIndex = workPackage.MaterialIndex;

            string fileName = "./Storage/" + typeIndex.ToString() + stype + ".trc";
            try
            {
                FileStream afile = new(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter sw = new(afile);

                sw.WriteLine("Type," + stype);
                sw.WriteLine("material," + materialFileIndex.ToString());

                if (workPackage != null)
                {
                    AccessMaterialFile accessMaterialFile = new();
                    accessMaterialFile.WriteMaterialFile(workPackage);
                }

                PointListData pt;
                if (workPackage != null)
                    pt = workPackage.GetPointList();
                else
                    pt = new PointListData();

                if (pt == null)
                {
                    sw.Close();
                    throw (new Exception());
                }

                int oldParaIndex = 0;
                for (short i = 0; i < pt.GetCount(); i++)
                {
                    Point ptvar = pt.GetPoint(i);
                    if (ptvar != null)
                    {
                        int x = (int)ptvar.vector.X;
                        int y = (int)ptvar.vector.Y;
                        int z = (int)ptvar.vector.Z;
                        int A = (int)ptvar.A;


                        sw.WriteLine(x.ToString() + "," + y.ToString() + "," + z.ToString() + ","
                            + A.ToString() + "," + ptvar.LineType + "," +
                            ptvar.Parameter.ParaIndex.ToString());


                        if (ptvar.Parameter.ParaIndex != oldParaIndex)
                        {
                            WriteLaserWeldParameterFile(ptvar.Parameter.ParaIndex, ptvar);
                            oldParaIndex = ptvar.Parameter.ParaIndex;
                        }
                    }
                }
                sw.Close();
            }
            catch
            {
                Assertion.AssertError("工作文件写错误", 202);
                return;
            }
        }

        [STAThread]
        private static void WriteLaserWeldParameterFile(int paraIndex, Point pt)
        {
            XmlDocument doc = new();
            doc.LoadXml("<Lasers></Lasers>");

            if (doc.DocumentElement == null) { return; }
            XmlElement root = doc.DocumentElement;

            XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", "");
            doc.InsertBefore(decl, root);

            XmlComment comment = doc.CreateComment("The record file for the sun flower welding system");
            doc.InsertBefore(comment, root);

            // the file node
            XmlElement laser = doc.CreateElement("laser");
            root.InsertAfter(laser, root.FirstChild);
            AddXmlElement(doc, laser, "power", pt.Parameter.LaserPower.ToString());
            AddXmlElement(doc, laser, "frequency", pt.Parameter.Frequency.ToString());
            AddXmlElement(doc, laser, "pulse", pt.Parameter.PulseWidth.ToString());
            AddXmlElement(doc, laser, "duty", pt.Parameter.DutyCycle.ToString());
            AddXmlElement(doc, laser, "rise", pt.Parameter.LaserRise.ToString());
            AddXmlElement(doc, laser, "fall", pt.Parameter.LaserFall.ToString());
            AddXmlElement(doc, laser, "hold", pt.Parameter.LaserHoldTime.ToString());
            AddXmlElement(doc, laser, "airin", pt.Parameter.AirIn.ToString());
            AddXmlElement(doc, laser, "airout", pt.Parameter.AirOut.ToString());
            AddXmlElement(doc, laser, "wiretime", pt.Parameter.WireTime.ToString());
            AddXmlElement(doc, laser, "weldspeed", pt.Parameter.WeldSpeed.ToString());
            AddXmlElement(doc, laser, "wobble", pt.Parameter.WobbleSpeed.ToString());
            AddXmlElement(doc, laser, "leap", pt.Parameter.LeapSpeed.ToString());

            string rfname = "./Storage/" + paraIndex.ToString() + ".lwp";
            doc.Save(rfname);
        }
    }
}
