using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using GC.Frame.Motion.Privt;

using RobotWeld.AlgorithmsBase;
using RobotWeld.Welding;

namespace RobotWeld.GetTrace
{
    /// <summary>
    /// Run trace when receive the start signal
    /// </summary>
    public class RunTrace
    {
        // constants
        private const short LASER_CHANNEL = 0;
        private const int POWER_CODE = 32767;

        // welding parameter
        private double weldSpeed;
        private readonly double synAcc = 0.5;
        private int LaserPower;

        private bool hasRise;
        private double LaserRise;

        private bool hasFall;
        private double LaserFall;

        // motion card parameters
        private int cardSignal;
        private int oldCardSignal;
        private short oldAxisIndex = 0;
        private short oldRatio = 5;

        private ushort devHandle = 0;
        private ushort pcrdHandle;
        private readonly ushort[] axisHandle = new ushort[4];

        #region Construction
        public RunTrace()
        {
            if (!ConfigCard()) return;
            ConfigCoordinate();
        }

        // Configure the motion card
        private bool ConfigCard()
        {
            // open motion card and load the configure file.
            short rtn = CNMCLib20.NMC_DevOpen(0, ref devHandle);
            ShowError("Open card", rtn);

            if (rtn == 0)
            {
                rtn = CNMCLib20.NMC_LoadConfigFromFile(devHandle,
                    Encoding.Default.GetBytes("./GCN400.cfg"));
                ShowError("Load card", rtn);

                for (short i = 0; i < 3; i++)
                {
                    rtn = CNMCLib20.NMC_MtOpen(devHandle, i, ref axisHandle[i]);
                    ShowError("open axis", rtn);
                    rtn = CNMCLib20.NMC_MtSetSvOn(axisHandle[i]);
                    ShowError("set axis", rtn);
                }
                return true;
            }
            else
            {
                cardSignal = -1;
                return false;
            }
        }

        // Configure the coordinate
        private void ConfigCoordinate()
        {
            // open the coordinate
            short rtn = CNMCLib20.NMC_CrdOpen(devHandle, ref pcrdHandle);
            ShowError("open coordinate", rtn);

            // configure the coordinate
            CNMCLib20.TCrdConfig crd = new(true);
            crd.axCnts = 3;
            crd.pAxArray[0] = 0;
            crd.pAxArray[1] = 1;
            crd.pAxArray[2] = 2;
            crd.port[0] = 0;
            crd.port[1] = 0;
            crd.port[2] = 0;
            rtn = CNMCLib20.NMC_CrdConfig(pcrdHandle, ref crd);
            ShowError("configure coordinate", rtn);

            // configure moving parameters
            CNMCLib20.TCrdPara crdPara = new CNMCLib20.TCrdPara(true);
            crdPara.orgFlag = 1;
            crdPara.offset[0] = 0;
            crdPara.offset[1] = 0;
            crdPara.offset[2] = 0;
            crdPara.synAccMax = 50;
            crdPara.synVelMax = 800;
            rtn = CNMCLib20.NMC_CrdSetPara(pcrdHandle, ref crdPara);
            ShowError("moving parameter", rtn);

            for (short i=0; i<3; i++)
            {
                rtn = CNMCLib20.NMC_MtSetPrfCoe(axisHandle[i], 1);
                ShowError("Set Profile coefficent", i);
            }

            // clear the command buffer
            rtn = CNMCLib20.NMC_CrdBufClr(pcrdHandle);
            ShowError("clear buffer", rtn);
        }
        #endregion

        //-- properties
        public int CardSignal
        {
            get { return cardSignal; }
            set { cardSignal = value; }
        }

        //---- program ---- 
        // scan the information from the handwheel
        // and record the changes to the pointer list.
        // pointIndex : when run, represent the point in welding
        //            : when set, represent the point to be set
        // keyState : button state in setting; when run, it is 0
        public bool TraceChanged(List<Point> points,
            TraceType traceType,
            MotionType motionType,
            SingleMotion singleMotion,
            ref int pointIndex,
            ref int keyState)
        {
            if (singleMotion.singlemotion != SingleMotion.EsingleMotion.NONE)
            {
                if (singleMotion.singlemotion == SingleMotion.EsingleMotion.RESET)
                {
                    ResetCard();
                }
                else
                {
                    SingleStep(singleMotion);
                }
            }
            else if (motionType.motiontype != MotionType.EmotionType.NO_MOTION)
            {
                if (motionType.motiontype == MotionType.EmotionType.RUN_TRACE)
                {
                    keyState = 0;    // in run scene, the key is forbiden.);
                    if (traceType.tracetype == TraceType.EtraceType.VANE_WHEEL)
                    {
                        InAction_VaneWheel(points);
                    }
                }
                else if (motionType.motiontype == MotionType.EmotionType.SINGLE_TRACE)
                {
                    // TODO: 单步运动
                }
                else
                {
                    SetPoint(points, ref pointIndex, ref keyState);
                }
                return true;
            }
            else if (motionType.motiontype != MotionType.EmotionType.EXIT_MOTION)
            {
                CloseCard();
                cardSignal = -1;
            }
            else
            {
                // Just Waiting
            }

            // in other scenes, reploting is not need
            return false;
        }

        //---- the properties ----
        public double WeldSpeed
        {
            get { return weldSpeed; }
            set { weldSpeed = value; }
        }

        #region Set point coordinate
        // Get coordinate of pionter and add/change to the list
        private void SetPoint(List<Point> points,
            ref int pointIndex,
            ref int keyState)
        {
            short rtn;

            // initial the handwheel
            rtn = CNMCLib20.NMC_ClrHandWheel(devHandle);
            ShowError("open handwheel", rtn);

            rtn = CNMCLib20.NMC_SetHandWheelInput(devHandle, 256);
            ShowError("set handwheel", rtn);

            rtn = CNMCLib20.NMC_GetDIGroup(devHandle, out int isignal, 0);
            if (rtn == 0)
            {
                HandWheelParse(isignal, points, ref pointIndex, ref keyState);
            }
            else
            {
                CNMCLib20.NMC_ClrHandWheel(devHandle);
                ShowError("get handwheel", rtn);
            }
        }

        // analysis the information from handwheel
        private void HandWheelParse(
            int isignal,
            List<Point> points,
            ref int pointIndex,
            ref int keyState)
        {
            //-- set the handWheel by the key selection --
            short axisIndex = oldAxisIndex;
            short ratio = oldRatio;

            // Choice of the hand axis
            // IN 22-bit for X axis
            // IN 24-bit for Y axis
            // IN 26-bit for Z axis
            for (short i = 0; i <= 4; i += 2)
            {
                if ((isignal & (0x400_0000 >> i)) != 0)
                {
                    oldAxisIndex = axisIndex = (short)(i / 2);
                    break;
                }
            }

            // Choice of the ratio
            // IN 23-bit for 1x times
            // IN 25-bit for 5x times
            // IN 27-bit for 10x times
            for (short i = 0; i < 4; i += 2)
            {
                if ((isignal & (0x800_0000 >> i)) != 0)
                {
                    oldRatio = ratio = (short)(i * 5);
                    break;
                }
            }

            CNMCLib20.NMC_SetHandWheel(devHandle, axisIndex, ratio, 1, 10);

            //-- Set the key state or save the data at the key pressing --
            // IN 16-bit for K1; confirm the point coordinate.
            if ((isignal & (0x1_0000)) != 0)
            {
                int var0 = keyState & 0b10;
                int var1 = keyState & 0b01;
                int[] pos = { 0 };
                for (short i = 0; i < 3; i++)
                {
                    CNMCLib20.NMC_MtGetCmdPos(axisHandle[i], ref pos[i]);
                }
                Vector vector = new(pos[0], pos[1], pos[2]);
                Point pts = new(var0, var1, vector);

                if (pointIndex <= points.Count)
                {
                    // modify the existing point
                    points[pointIndex] = pts;
                    pointIndex++;
                }
                else if (pointIndex == points.Count + 1)
                {
                    // Add a new point
                    if (points[pointIndex - 1].vector.X == pos[0] &&
                        points[pointIndex - 1].vector.Y == pos[1] &&
                        points[pointIndex - 1].vector.Z == pos[2])
                    {
                        points.Add(pts);
                        pointIndex++;
                    }
                    else
                    {
                        // circulate the points index
                        pointIndex = 0;
                    }
                }
            }

            // IN 17-bit for K2; revise the 2nd bit of the key_state
            if ((isignal & (0x2_0000)) != 0)
            {
                int var0 = ~(keyState & 0b10);
                int var1 = keyState & 0b01;

                keyState = var0 | var1;
            }

            // IN 18-bit for K3; revise the 1st bit of the key_state
            if ((isignal & (0x4_0000)) != 0)
            {
                int var0 = ~(keyState & 0b01);
                int var1 = keyState & 0b10;

                keyState = var0 | var1;
            }
        }
        #endregion

        #region other command
        // reset the motor to the orignal pointer
        private void ResetCard()
        {
            // TODO: Reset action
        }

        // close the motion card
        public void CloseCard()
        {
            short rtn = CNMCLib20.NMC_CrdClose(ref pcrdHandle);
            ShowError("Close coordinate", rtn);

            rtn = CNMCLib20.NMC_DevClose(ref devHandle);
            ShowError("Close card", rtn);
        }

        // operation the card in single step
        public void SingleStep(SingleMotion singlemotion)
        {
            // TODO: single Stop moving
        }
        #endregion

        #region moving the motors according to the IO input
        // read PLC output IO
        public void InAction_VaneWheel(List<Point> points)
        {
            List<Vector> vect1 = new List<Vector>();
            List<Vector> vect2 = new List<Vector>();

            AnalyseList_VaneWheel(points, vect1, vect2);

            short rtn = CNMCLib20.NMC_GetDIGroup(devHandle, out int isignal, 0);
            if (rtn == 0)
            {
                ParseIOinput_VaneWheel(isignal, vect1, vect2);
            }
            else
            {
                cardSignal = -1;
                ShowError("hand Action", rtn);
            }
        }

        // Analyse the point list to work vectors
        private void AnalyseList_VaneWheel(
            List<Point> points, 
            List<Vector> vect1, 
            List<Vector> vect2)
        {
            int firstNumber;
            int secondNumber;

            // Collect points in the first welding seam.
            for (int i = 0; i < points.Count; i++)
            {
                vect1.Add(points[i].vector);
                if (points[i].LineType == 0) break;
            }

            firstNumber = vect1.Count;

            // the list shoud be odd number
            if ((firstNumber % 2) == 0)
            {
                int last = vect1.Count - 1;

                double midptX = (vect1[last].X - vect1[last - 1].X) / 2;
                double midptY = (vect1[last].Y - vect1[last - 1].Y) / 2;
                double midptZ = (vect1[last].Z - vect1[last - 1].Z) / 2;

                vect1.Insert(last, new Vector((int)midptX, (int)midptY, (int)midptZ));
            }

            // Collect points in the second welding seam.
            for (int i = firstNumber + 1; i < points.Count; i++)
            {
                vect2.Add(points[i].vector);
            }
            secondNumber = vect2.Count;

            if (secondNumber % 2 == 0)
            {
                int last = secondNumber - 1;
                double midptX = (vect2[last].X - vect2[last - 1].X) / 2;
                double midptY = (vect2[last].Y - vect2[last - 1].Y) / 2;
                double midptZ = (vect2[last].Z - vect2[last - 1].Z) / 2;

                vect2.Insert(last, new Vector((int)midptX, (int)midptY, (int)midptZ));
            }
        }

        // analyse IO input and start the action
        private void ParseIOinput_VaneWheel(int isignal, 
            List<Vector> vect1,
            List<Vector> vect2) 
        {
            // IN 12-bit and 13-bit for #1 laser and moving
            // IN 14-bit and 15-bit for #2 laser and moving
            // Note : the bit count from 0
            for (ushort i = 4; i > 0; i--)
            {
                if ((isignal & (0x8000 >> i)) != 0)
                {
                    cardSignal = i + 1;
                    if (cardSignal != oldCardSignal)
                    {
                        switch (cardSignal)
                        {
                            case 1:
                                LaserCurve(vect1);
                                break;
                            case 2:
                                MovePosition(vect2);
                                break;
                            case 3:
                                LaserCurve(vect2);
                                break;
                            case 4:
                                MovePosition(vect1);
                                break;
                            default:
                                break;
                        }
                        oldCardSignal = cardSignal;
                    }
                    break;
                }
            }
        }

        // the real action due to IO input
        // welding the first part
        private void LaserCurve(List<Vector> vect)
        {
            //-- interpolate the circle
            int segNo = 0;
            int[] pt0 = { (int)vect[0].X, (int)vect[0].Y, (int)vect[0].Z };

            // go to the start point
            CNMCLib20.NMC_CrdLineXYZEx(pcrdHandle, segNo, 7, pt0, 0, weldSpeed, synAcc, 0);

            // push the interpolate command into the buffer
            for (short i=1; i < vect.Count; i+=2)
            {
                int[] midpos = { (int)vect[i].X, (int)vect[i].Y, (int)vect[i].Z };
                int[] tgpos = { (int)vect[i + 1].X, (int)vect[i + 1].Y, (int)vect[i + 1].Z };
                CNMCLib20.NMC_CrdArc3DEx(pcrdHandle, ++segNo, midpos, tgpos, 0, weldSpeed, synAcc, 0);
            }

            // Start to action
            CNMCLib20.NMC_CrdEndMtn(pcrdHandle);
            short rtn = CNMCLib20.NMC_CrdStartMtn(pcrdHandle);
            ShowError("Action Run", rtn);
            if (rtn != 0) { return; }

            // during the action procesure.
            long firstTime = PassTime();
            short crdState = 0;
            while(true)
            {
                LaserSwitchOn(firstTime); 
                CNMCLib20.NMC_CrdGetSts(pcrdHandle, ref crdState);

                // trigger the slow falling
                LaserSlowFall(vect);

                // when the moving is stop, jump the loop
                if ((crdState & CNMCLib20.BIT_AXIS_BUSY) == 0) 
                {
                    LaserSwitchOff();
                    break; 
                }
                Thread.Sleep(1);
            }
        }

        // moving to the left position
        private void MovePosition(List<Vector> vect)
        {
            int[] pt0 = { (int)vect[0].X, (int)vect[0].Y, (int)vect[0].Z };

            // go to the start point
            CNMCLib20.NMC_CrdLineXYZEx(pcrdHandle, 0, 7, pt0, 0, weldSpeed, synAcc, 0);

            CNMCLib20.NMC_CrdEndMtn(pcrdHandle);
            short rtn = CNMCLib20.NMC_CrdStartMtn(pcrdHandle);
            ShowError("Action Run", rtn);

            short crdState = 0;
            while (true)
            {
                CNMCLib20.NMC_CrdGetSts(pcrdHandle, ref crdState);
                if ((crdState & CNMCLib20.BIT_AXIS_BUSY) == 0)
                {
                    LaserSwitchOff();
                    break;
                }
                Thread.Sleep(1);
            }
        }
        #endregion

        #region Laser operation
        // Configure the Laser
        public void ConfigLaser(LaserParameter lp)
        {
            LaserPower = lp.Power * POWER_CODE / 100;

            // set laser operation mode
            if (lp.Mode == 3)
                CNMCLib20.NMC_LaserSetMode(devHandle, CNMCLib20.SHIO_OUTPUT_MODE, LASER_CHANNEL);
            else if (lp.Mode == 2)
                CNMCLib20.NMC_LaserSetMode(devHandle, CNMCLib20.TIME_ARRAY_OUTPUT_MODE, LASER_CHANNEL);
            else
            {
                CNMCLib20.NMC_LaserSetMode(devHandle, CNMCLib20.BASIC_OUTPUT_MODE, LASER_CHANNEL);
                CNMCLib20.NMC_LaserSetParam(devHandle, lp.OnDelay, lp.OffDelay, 0, lp.MaxPower, 0, LASER_CHANNEL);
                
                // CW or QCW operation
                if (lp.Frequency == 0)
                {
                    CNMCLib20.NMC_LaserSetOutputType(devHandle, CNMCLib20.LASER_DA, 0, 0, LASER_CHANNEL);
                    
                    if (lp.Rise != 0)
                    {
                        hasRise = true;
                        LaserRise = lp.Rise;
                    }

                    if (lp.Fall != 0)
                    {
                        hasFall = true;
                        LaserFall = lp.Fall;
                    }
                }
                else
                {
                    // TODO: 频率输出
                }

            }
        }

        private long PassTime()
        {
            TimeSpan tSpan = DateTime.Now - new DateTime(2023, 4, 1, 0, 0, 0); ;
            return tSpan.Milliseconds;
        }

        private void LaserSwitchOn(long firstTime)
        {
            if (hasRise)
            {
                long passTime = PassTime() - firstTime;

                if ((passTime >= 0) && (passTime <= LaserRise * 0.25))
                {
                    CNMCLib20.NMC_LaserOnOff(devHandle, 1, LASER_CHANNEL);
                    CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.2), LASER_CHANNEL);
                }
                else if ((passTime >= LaserRise * 0.25) && (passTime < LaserRise * 0.5))
                {
                    CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.4), LASER_CHANNEL);
                }
                else if ((passTime >= LaserRise * 0.5) && (passTime < LaserRise*0.75))
                {
                    CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.6), LASER_CHANNEL);
                }
                else if ((passTime >= LaserRise * 0.75) && (passTime < LaserRise))
                {
                    CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.8), LASER_CHANNEL);
                }
                else if (passTime >= LaserRise)
                {
                    CNMCLib20.NMC_LaserSetPower(devHandle, LaserPower, LASER_CHANNEL);
                }
            }
            else
            {
                CNMCLib20.NMC_LaserOnOff(devHandle, 1, LASER_CHANNEL);
                CNMCLib20.NMC_LaserSetPower(devHandle, LaserPower, LASER_CHANNEL);
            }
        }

        // slow decreasing the laser power at the end of action.
        private void LaserSlowFall(List<Vector> vect)
        {
            if (!hasFall) { return; }

            double pLen = 0;
            CNMCLib20.NMC_CrdGetBufLeftLength(devHandle, ref pLen);

            double leftTime = pLen / WeldSpeed + WeldSpeed / synAcc;
            if (leftTime < LaserFall)
            {
                CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.8), LASER_CHANNEL);
            }
            else if ((leftTime <= LaserFall * 0.75) && (leftTime > LaserFall * 0.5))
            {
                CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.6), LASER_CHANNEL);
            }
            else if ((leftTime <= LaserFall * 0.5) &&  (leftTime > LaserFall * 0.25))
            {
                CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.4), LASER_CHANNEL);
            }
            else
            {
                // the last laser power is hold at 20%
                CNMCLib20.NMC_LaserSetPower(devHandle, (int)(LaserPower * 0.2), LASER_CHANNEL);
            }
        }

        private void LaserSwitchOff()
        {
            CNMCLib20.NMC_LaserSetPower(devHandle, 0, LASER_CHANNEL);
            CNMCLib20.NMC_LaserOnOff(devHandle, 0, LASER_CHANNEL);
        }
        #endregion

        private static void ShowError(string name, short rtn)
        {
            string msg = string.Format("The card has an error of {0} \n", rtn) + name;

            if (rtn != 0) 
                new Werr().WerrMessage(msg);
        }
    }
}
