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

using Microsoft.Win32;
using RobotWeld2.AlgorithmsBase;
using RobotWeld2.AppModel;
using RobotWeld2.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// The Daemon process for the data record, include workpackage
    /// </summary>
    public class DaemonFile
    {
        private readonly MainWindowViewModel _viewModel;
        private Dictionary<int, List<Point>>? _points;

        private int _traceIndex;
        private static int _maxPower = 1500;

        private string _errMsg = string.Empty;
        private static string _openMsg = string.Empty;

        private bool _pmFlag;    // true : Prepare buttion has been checked.

        public DaemonFile(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public int TraceIndex
        {
            get => _traceIndex;
            set => _traceIndex = value;
        }

        public static int MaxPower
        {
            get => _maxPower;
            set => _maxPower = value;
        }

        // True : when prepared of run trace is checked
        public bool PmFlag
        {
            get { return _pmFlag; }
            set { _pmFlag = value; }
        }

        /// <summary>
        /// ---- the program to operate data ----
        /// set the value for parameter viewModel
        /// </summary>
        public void SetParameter()
        {
            var cp = _viewModel.PointList.ToList();
            if (cp != null && cp.Count > 1)
                _viewModel.LaserPower = cp[1].LaserPointPower;
            else
                _viewModel.LaserPower = 0;
        }

        /// <summary>
        /// Get the laser parameter from daemon file.
        /// </summary>
        /// <param name="ivm"> the viewMode of Input laser parameter </param>
        public void GetLaserParameter(InputViewModel ivm)
        {
            ivm.MaxPower = _maxPower;

            var cp = _viewModel.PointList.ToList();
            if (cp != null && cp.Count > 1)
            {
                ivm.LaserPower = cp[1].LaserPointPower;
                ivm.WeldSpeed = cp[1].WeldSpeed;
                ivm.WobbleSpeed = cp[1].Wobble;
                ivm.LeapSpeed = cp[1].Leap;
            }
            else
            {
                ivm.LaserPower = 0;
                ivm.WeldSpeed = 0;
                ivm.WobbleSpeed = 0;
                ivm.LeapSpeed = 0;
            }

            ivm.Frequency = 0;
            ivm.LaserPulseWidth = 0;
            ivm.LaserDutyCycle = 0;
        }

        /// <summary>
        /// Set the laser parameter to the daemon file.
        /// </summary>
        /// <param name="ivm"> the viewMode of Input laser parameter </param>
        public void SetLaserParameter(InputViewModel ivm)
        {
            // all point has the same laser parameters
            var cp = _viewModel.PointList.ToList();
            for (int i = 1; i < cp.Count; i++)
            {
                cp[i].LaserPointPower = ivm.LaserPower;
                cp[i].WeldSpeed = ivm.WeldSpeed;
                cp[i].Wobble = ivm.WobbleSpeed;
                cp[i].Leap = ivm.LeapSpeed;
            }
            _viewModel.PtIndex = 0;
            _viewModel.PtIndex = 1;
        }

        /// <summary>
        /// Get the laser point of click on
        /// </summary>
        /// <returns> the laser power </returns>
        public int GetPointPower()
        {
            int p = _viewModel.PtIndex;
            var cp = _viewModel.PointList.ToList();
            if (cp != null && cp.Count > p)
            {
                return cp[p].LaserPointPower;
            }

            return 0;
        }

        public List<Point>? GetCopyPoint()
        {
            if (_viewModel.PointList != null)
                return _viewModel.PointList.ToList();
            else
                return new List<Point>();
        }

        /// <summary>
        /// Open the file through the winfrom dialog
        /// </summary>
        public int OpenDialog()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Weld Data files (*.wfd)|*.wfd",
                Title = "打开焊接文件",
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = openFileDialog.ShowDialog();

            if (rst == true)
            {
                string filename = openFileDialog.FileName;
                BaseTreatFile baseTreatFile = new();
                return baseTreatFile.Open(filename);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// close the main window and keep the scene
        /// </summary>
        public void Close()
        {
            BaseTreatFile baseTreatFile = new();
            baseTreatFile.Save(_traceIndex);
        }

        /// <summary>
        /// save the enginnering project
        /// </summary>
        public void SaveDialog()
        {
            SaveFileDialog dialog = new()
            {
                DefaultExt = ".wfd",
                Filter = "WeldData documents (.wfd)|*.wfd",
                AddExtension = true,
                InitialDirectory = Directory.GetCurrentDirectory() + ".\\usr",
            };

            bool? rst = dialog.ShowDialog();
            if (rst == true)
            {
                string filename = dialog.FileName;
                BaseTreatFile baseTreatFile = new();
                baseTreatFile.Save(_traceIndex, filename);
            }
        }

        /// <summary>
        /// Add a new point by press the button on handpad
        /// </summary>
        /// <param name="x"> coordinate X </param>
        /// <param name="y"> coordinate Y </param>
        /// <param name="z"> coordinate Z </param>
        public void AddPoint(int x, int y, int z)
        {

            Vector vc = new(x, y, z);
            var cp = _viewModel.PointList.ToList();
            if (cp == null) return;

            if (CheckPoints(cp))
            {
                ChangePointValue(vc, cp);
            }
            else if (ExaminePoints())
            {
                if (_points != null)
                {
                    var Cdt = new CoordTransform();
                    Cdt.PutPointList(_points);
                    Cdt.CompleteTrace(_viewModel.PtIndex, vc, ref cp);
                }
            }

            RefrashPointList(cp);
        }

        //
        // Is the list of all points have the corrected values?
        // True: yes, all has the values.
        //
        private static bool CheckPoints(List<Point> cp)
        {
            for (int i = 0; i < cp.Count; i++)
            {
                if (cp[i].vector.X == 0 && cp[i].vector.Y == 0 && cp[i].vector.Z == 0)
                    return false;
            }
            return true;
        }

        //
        // The point list is completed and change the point value only
        //
        private void ChangePointValue(Vector vc, List<Point> cp)
        {
            int p = _viewModel.PtIndex;
            if (p > cp.Count) return;

            int linetype;
            int laserOnOff;

            if (p == 0) linetype = 0;
            else linetype = 1;

            if (cp[p].LaserPointPower == 0) laserOnOff = 0;
            else laserOnOff = 1;

            var lp = cp[p].GetLaserParameter();
            var onept = new Point(linetype, laserOnOff, lp, vc);

            cp[p] = onept;
        }

        //
        // Because the first trace must input by hand, we should check
        //     whether the first trace when it is a new workpiece? 
        // true: it is not the first trace.
        //
        private bool ExaminePoints()
        {
            bool ret = false;
            if (_points == null) return ret;

            // the list of No. Zero is for prepared points.
            // the trace counted from 1st. 
            for (int i = 1; i < _points.Count; i++)
            {
                var l = _points[i];
                if (l != null && l[1].vector.X != 0) 
                    return true;
            }

            return ret;
        }

        public void CancelChoose()
        {
            if (_points != null) DisplayPointList(_points);
        }

        public void SetInputPower(int value)
        {
            _viewModel.LaserPower = value;
        }

        /// <summary>
        /// Set the display XYZ coordinates to the machine positions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetXYZ(int x, int y, int z)
        {
            _viewModel.X = x;
            _viewModel.Y = y;
            _viewModel.Z = z;
        }

        /// <summary>
        /// Add an error message to the message box
        /// </summary>
        /// <param name="msg"></param>
        public void AddMsg(string msg)
        {
            _errMsg += msg + "\n";
            _viewModel.ErrMsg = _errMsg;
        }

        public void FreshMsg()
        {
            _errMsg += _openMsg + "\n";
            _viewModel.ErrMsg = _errMsg;
            _openMsg = string.Empty;
        }

        public static void AddInfo(string msg)
        {
            _openMsg += msg;
        }

        //
        // Display a new list of point, established by the intelligence calculation.
        //
        private void RefrashPointList(List<Point> cp)
        {
            if (_points == null || _points.Count == 0) return;
            _viewModel.PointList.Clear();

            if (!_viewModel.TraceDir)
            {
                // left side
                var po = new Point(_points[0][1]);
                po.SetNum(0);
                _viewModel.PointList.Add(po);
            }
            else
            {
                // right side
                var po = new Point(_points[0][2]);
                po.SetNum(0);
                _viewModel.PointList.Add(po);
            }

            // establish a new copy of point list
            for (int i = 0; i < cp.Count; i++)
            {
                var po = new Point(cp[i]);
                po.SetNum(i + 1);
                _viewModel.PointList.Add(po);
            }
        }

        public void DisplayPointList(Dictionary<int, List<Point>> points)
        {
            if (_viewModel.TraceInfo > 7 || _viewModel.TraceInfo < 0) return;

            _points = points;
            _viewModel.PointList.Clear();

            if (_viewModel.TraceInfo == 0) DisplayPointZero();
            else DisplayPointNormal();
        }

        private void DisplayPointZero()
        {
            if (_points == null || _points.Count == 0) return;

            for (int i = 0; i < _points[0].Count; i++)
            {
                var po = new Point(_points[0][i]);
                po.SetNum(i);
                _viewModel.PointList.Add(po);
            }
        }

        //
        // Display the point list of this point list.
        // Establish a new pointList with the given Trace direction
        //    and the Trace information (the point index).
        //
        private void DisplayPointNormal()
        {
            if (_points == null || _points.Count == 0) return;

            int itr;
            if (!_viewModel.TraceDir)
            {
                // left side
                itr = _viewModel.TraceInfo;
                var po = new Point(_points[0][1]);
                po.SetNum(0);
                _viewModel.PointList.Add(po);
            }
            else
            {
                // right side
                itr = 0 - _viewModel.TraceInfo;
                var po = new Point(_points[0][2]);
                po.SetNum(0);
                _viewModel.PointList.Add(po);
            }

            // establish a new copy of point list
            for (int i = 0; i < _points[itr].Count; i++)
            {
                var po = new Point(_points[itr][i]);
                po.SetNum(i + 1);
                _viewModel.PointList.Add(po);
            }
        }

        public void PreparedState(bool bState)
        {
            if (!bState)
            {
                _viewModel.PmText = "待机....";
                _viewModel.PmFlag = false;
            }
            else
            {
                _viewModel.PmText = "准备运行";
                _viewModel.PmFlag = true;
            }
        }

        /// <summary>
        /// Get coordinate of point indicated
        /// </summary>
        /// <returns> true: the point exist; false: add a new point </returns>
        public bool GetPointIndex(ref int[] ptxyz, out double speed)
        {
            if (_viewModel.PointList == null) goto END;

            int p = _viewModel.PtIndex;
            var cp = _viewModel.PointList.ToList();
            if (cp != null && cp.Count > p)
            {
                ptxyz[0] = (int)cp[p].vector.X;
                ptxyz[1] = (int)cp[p].vector.Y;
                ptxyz[2] = (int)cp[p].vector.Z;
                speed = cp[p].Leap;
                return true;
            }

        END:
            speed = 0;
            return false;
        }
    }
}
