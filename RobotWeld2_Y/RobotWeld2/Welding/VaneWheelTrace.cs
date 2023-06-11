using RobotWeld2.AlgorithmsBase;
using RobotWeld2.Motions;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation.Provider;

namespace RobotWeld2.Welding
{
    /// <summary>
    /// Welding along the trace in VaneWheel trace Type
    /// </summary>
    public class VaneWheelTrace
    {
        //
        // welding parameter
        //
        private List<int[]>? pots1;
        private List<int[]>? pots2;
        private List<LaserParameter>? lplist;
        private MoveAction? _ma;
        private DaemonFile? dmFile;

        private nowState? nws;

        private static bool ActionAbort;

        public VaneWheelTrace(MoveAction ma)
        {
            _ma = ma;
            pots1 = new List<int[]>();
            pots2 = new List<int[]>();
            lplist = new List<LaserParameter>();
        }

        //
        // NEW:---- methods from HERE
        //
        private MotionOperate? _mo;
        public VaneWheelTrace() { }
        private List<int[]>? potsLeft;
        private List<int[]>? potsRight;
        private int lpLeft;
        private int lpRight;
        private double WeldSpeed;
        private double moveSpeed;

        private bool EchoState;

        public void CopyHardware(MotionOperate mo, DaemonFile dmFile, LaserParameter lp,
            double WeldSpeed, double moveSpeed)
        {
            this._mo = mo;
            this.dmFile = dmFile;
            _mo.InitialLaser();
            _mo.SetupLaserParameter(in lp);
            this.WeldSpeed = WeldSpeed;
            this.moveSpeed = moveSpeed;
        }

        public void WorkProcess(List<Point> points)
        {
            potsLeft = new List<int[]>();
            potsRight = new List<int[]>();
            PrepareData(points, potsLeft, out lpLeft, potsRight, out lpRight);

            _mo?.SetActionMethod(this);
            CheckPosition();

            int[] bitAddr = new int[4] { 12, 13, 14, 15 };
            // Loop thread to scan IO and action 
            _mo?.ScanAction(bitAddr);
        }

        public void Action(int[] iActNum)
        {
            //GiveMsg.Show(nws.ToString());
            if (nws == nowState.RESET_POS || nws == nowState.LEFT_FINISH || nws == nowState.RIGHT_FINISH)
            {
                if (iActNum[0] == 1)
                {
                    FlyWeld_Left();
                }
                else if (iActNum[2] == 1)
                {
                    FlyWeld_Right();
                }
            }
            else if (nws == nowState.LEFT_WELD)
            {
                if (iActNum[0] == 1 && iActNum[1] == 1)
                {
                    Weld_Left_NoEcho();
                }
                if (iActNum[0] == 1)
                {
                    Weld_Left();
                }
                else if (iActNum[1] == 1)
                {
                    Echo_Left();
                }
            }
            else if (nws == nowState.RIGHT_WELD)
            {
                if (iActNum[2] == 1 && iActNum[3] == 1)
                {
                    Weld_Right_NoEcho();
                }
                else if (iActNum[2] == 1)
                {
                    Weld_Right();
                }
                else if (iActNum[3] == 1)
                {
                    Echo_Right();
                }
            }
        }

        public void FinAction(int iAct)
        {
            switch (iAct)
            {
                case 8:    // Weld finished
                    DoWeldFin();
                    break;
                case 9:    // fly finished
                    DoFlyFin();
                    break;
                case 10:    // fly weld finishe
                    DoFlyWeldFin();
                    break;
                default:
                    break;
            }
        }

        public void DoBit12()
        {
            if (nws == nowState.LEFT_WELD || nws == nowState.LEFT_FINISH)
            {
                nws = nowState.LEFT_WELD;
                dmFile?.SetInputPower(lpLeft);

                Weld_Left();
                Thread.Sleep(10);
            }
            else if (nws == nowState.RESET_POS || nws == nowState.RIGHT_FINISH)
            {
                nws = nowState.LEFT_WELD;
                dmFile?.SetInputPower(lpLeft);

                FlyWeld_Left();
                Thread.Sleep(10);
            }
        }

        public void DoBit13()
        {
            if (nws == nowState.LEFT_WELD)
            {
                nws = nowState.LEFT_FINISH;
                CloseAir();
                Echo_Left();
            }
        }

        public void DoBit14()
        {
            if (nws == nowState.RIGHT_WELD || nws == nowState.RIGHT_FINISH)
            {
                nws = nowState.RIGHT_WELD;
                dmFile?.SetInputPower(lpRight);

                Weld_Right();
                Thread.Sleep(10);
            }
            else if (nws == nowState.RESET_POS || nws == nowState.LEFT_FINISH)
            {
                nws = nowState.RIGHT_WELD;
                dmFile?.SetInputPower(lpRight);

                FlyWeld_Right();
                Thread.Sleep(10);
            }
        }

        public void DoBit15()
        {
            if (nws == nowState.RIGHT_WELD)
            {
                nws = nowState.RIGHT_FINISH;
                CloseAir();
                Echo_Right();
            }
        }

        public void DoWeldFin()
        {
            if (!EchoState)
            {
                if (nws == nowState.LEFT_WELD)
                {
                    Echo_Left();
                }
                if (nws == nowState.RIGHT_WELD)
                {
                    Echo_Right();
                }

                return;
            }

            if (nws == nowState.LEFT_WELD)
            {
                _mo?.EchoBit(13);
                MotionOperate.SectionFinish();
            }
            if (nws == nowState.RIGHT_WELD)
            {
                _mo?.EchoBit(15);
                MotionOperate.SectionFinish();
            }
        }

        public void DoFlyWeldFin()
        {
            if (nws == nowState.LEFT_WELD)
            {
                _mo?.EchoBit(13);
                MotionOperate.SectionFinish();
            }
            if (nws == nowState.RIGHT_WELD)
            {
                _mo?.EchoBit(15);
                MotionOperate.SectionFinish();
            }
        }

        public void DoFlyFin()
        {
            if (nws == nowState.LEFT_WELD)
            {
                _mo?.EchoBit(13);
                nws = nowState.LEFT_FINISH;
                MotionOperate.SectionFinish();
                CloseAir();
            }
            if (nws == nowState.RIGHT_WELD)
            {
                _mo?.EchoBit(15);
                nws = nowState.RIGHT_FINISH;
                MotionOperate.SectionFinish();
                CloseAir();
            }
        }

        private void Weld_Left()
        {
            _mo?.EchoBit(12);
            nws = nowState.LEFT_WELD;
            EchoState = true;
            OpenAir();
            if (potsLeft != null)
            {
                List<int[]> ptlist = new List<int[]>();
                for (int i = 1; i < potsLeft.Count; i++)
                {
                    ptlist.Add(potsLeft[i]);
                }
                _mo?.RunWeld(lpLeft, ptlist, WeldSpeed);
            }
        }

        private void Weld_Left_NoEcho()
        {
            _mo?.EchoBit(12);
            nws = nowState.LEFT_WELD;
            EchoState = false;
            OpenAir();
            if (potsLeft != null)
            {
                List<int[]> ptlist = new List<int[]>();
                for (int i = 1; i < potsLeft.Count; i++)
                {
                    ptlist.Add(potsLeft[i]);
                }
                _mo?.RunWeld(lpLeft, ptlist, WeldSpeed);
            }
        }

        private void Weld_Right()
        {
            _mo?.EchoBit(14);
            nws = nowState.RIGHT_WELD;
            EchoState = true;
            OpenAir();
            if (potsRight != null)
            {
                List<int[]> ptlist = new List<int[]>();
                for (int i = 1; i < potsRight.Count; i++)
                    ptlist.Add(potsRight[i]);
                {
                }
                _mo?.RunWeld(lpRight, ptlist, WeldSpeed);
            }
        }

        private void Weld_Right_NoEcho()
        {
            _mo?.EchoBit(14);
            nws = nowState.RIGHT_WELD;
            EchoState = false;
            OpenAir();
            if (potsRight != null)
            {
                List<int[]> ptlist = new List<int[]>();
                for (int i = 1; i < potsRight.Count; i++)
                    ptlist.Add(potsRight[i]);
                {
                }
                _mo?.RunWeld(lpRight, ptlist, WeldSpeed);
            }
        }

        private void FlyWeld_Left()
        {
            _mo?.EchoBit(12);
            nws = nowState.LEFT_WELD;
            if (potsLeft != null)
            {
                List<int[]> ptlist = new List<int[]>();
                for (int i = 1; i < potsLeft.Count; i++)
                {
                    ptlist.Add(potsLeft[i]);
                }
                _mo?.RunFlyWeld(lpLeft, ptlist, WeldSpeed, potsLeft[0], moveSpeed);
            }
        }

        private void FlyWeld_Right()
        {
            _mo?.EchoBit(14);
            nws = nowState.RIGHT_WELD;
            if (potsRight != null)
            {
                List<int[]> ptlist = new List<int[]>();
                for (int i = 1; i < potsRight.Count; i++)
                {
                    ptlist.Add(potsRight[i]);
                }
                _mo?.RunFlyWeld(lpRight, ptlist, WeldSpeed, potsRight[0], moveSpeed);
            }
        }

        private void Echo_Left()
        {
            //nws = nowState.LEFT_FINISH;
            if (potsLeft != null)
            {
                _mo?.RunArrive(potsLeft[0], 0.8 * moveSpeed);
            }
        }

        private void Echo_Right()
        {
            nws = nowState.RIGHT_FINISH;
            if (potsRight != null)
                _mo?.RunArrive(potsRight[0], 0.8 * moveSpeed);
        }

        private void CloseAir()
        {
            _mo?.TurnOnBit(8);
        }

        private void OpenAir()
        {
            _mo?.TurnOffBit(8);
        }

        private void PrepareData(List<Point> ptlist, List<int[]> potsLeft, out int lpLeft,
            List<int[]> potsRight, out int lpRight)
        {
            int LeftCount = 0;
            int RightCount = 0;

            if (ptlist != null)
            {
                // ignore the prepared situation in the first and last points
                for (int i = 1; i < ptlist.Count - 1; i++)
                {
                    if (ptlist[i].LaserState == 0)
                    {
                        LeftCount = i;
                        RightCount = ptlist.Count - LeftCount - 2;
                        break;
                    }
                }

                lpLeft = ptlist[1].LaserPointPower;
                for (int i = 0; i <= LeftCount; i++)
                {
                    int[] ptxyz = new int[3];
                    ptxyz[0] = (int)ptlist[i].vector.X;
                    ptxyz[1] = (int)ptlist[i].vector.Y;
                    ptxyz[2] = (int)ptlist[i].vector.Z;
                    potsLeft.Add(ptxyz);
                }

                lpRight = ptlist[LeftCount + 1].LaserPointPower;
                for (int i = 0; i <= RightCount; i++)
                {
                    int[] ptxyz = new int[3];
                    ptxyz[0] = (int)ptlist[LeftCount + i + 1].vector.X;
                    ptxyz[1] = (int)ptlist[LeftCount + i + 1].vector.Y;
                    ptxyz[2] = (int)ptlist[LeftCount + i + 1].vector.Z;
                    potsRight.Add(ptxyz);
                }
            }
            else
            {
                lpLeft = 0;
                lpRight = 0;
            }
        }

        //
        // END -- the new method end
        //

        /// <summary>
        /// axis runs along the trace and weld it.
        /// This is thread function
        /// </summary>
        public void Run()
        {
            ActionAbort = false;
            while (true)
            {
                // NMC_GetDIGroup(devHandle, out int signal, 0);
                int signal = 0;
                _ma?.ReadIo(out signal);
                ParseInputIO(signal);
                if (ActionAbort) break;
                Thread.Sleep(10);
            }
        }

        public void FormSeam(List<Point> points)
        {
            AnalyzePoints analyzePoints = new();
            if (pots1 != null && pots2 != null && lplist != null)
                analyzePoints.FormingSeam(points, pots1, pots2, lplist);
        }

        public void UsingDaemon(DaemonFile daemonFile)
        {
            dmFile = daemonFile;
        }

        public bool CheckPosition()
        {
            int[] pPosArray = new int[3];
            _ma?.ReadPosition(ref pPosArray);
            /*            short rtn = NMC_CrdGetPrfPos(pcrdHandle, 1, pPosArray);
                        Assertion.AssertError("读取位置错误", rtn);*/

            if (pots1 != null && pots2 != null)
            {
                if (pPosArray[0] == pots1[0][0])
                {
                    nws = nowState.LEFT_FINISH;
                    return true;
                }
                else if (pPosArray[0] == pots2[0][0])
                {
                    nws = nowState.RIGHT_FINISH;
                    return true;
                }
            }

            nws = nowState.RESET_POS;
            return false;
        }

        public static void AbortThread()
        {
            ActionAbort = true;
        }

        //
        // Working ordered by the IO input
        //
        private void ParseInputIO(int signal)
        {
            //GiveMsg.Show(nws.ToString());
            switch (nws)
            {
                case nowState.LEFT_WELD:
                    DoLeft(signal);
                    break;
                case nowState.LEFT_FINISH:
                    StandLeft(signal);
                    break;
                case nowState.RIGHT_WELD:
                    DoRight(signal);
                    break;
                case nowState.RIGHT_FINISH:
                    StandRight(signal);
                    break;
                case nowState.RESET_POS:
                    FromReset(signal);
                    break;
                default:
                    break;
            }
        }

        private void FromReset(int signal)
        {
            if ((signal & (int)ComBit.Bit12) == 0)
            {
                FastMoveToLeft();
                WeldLeft();
                Thread.Sleep(50);
                nws = nowState.LEFT_WELD;
            }
            else if ((signal & (int)ComBit.Bit14) == 0)
            {
                FastMoveToRight();
                WeldRight();
                Thread.Sleep(50);
                nws = nowState.RIGHT_WELD;
            }
        }

        //
        // Four working state
        //
        private void DoLeft(int signal)
        {
            if ((signal & (int)ComBit.Bit12) == 0)
            {
                WeldLeft();
                Thread.Sleep(50);
                nws = nowState.LEFT_WELD;
            }
            else if ((signal & (int)ComBit.Bit13) == 0)
            {
                EchoLeft();
                nws = nowState.LEFT_FINISH;
            }
        }

        private void StandLeft(int signal)
        {
            _ma?.CloseAir();
            if ((signal & (int)ComBit.Bit12) == 0)
            {
                WeldLeft();
                Thread.Sleep(50);
                nws = nowState.LEFT_WELD;
            }
            else if ((signal & (int)ComBit.Bit14) == 0)
            {
                FastMoveToRight();
                WeldRight();
                Thread.Sleep(50);
                nws = nowState.RIGHT_WELD;
            }
        }

        private void DoRight(int signal)
        {
            if ((signal & (int)ComBit.Bit14) == 0)
            {
                WeldRight();
                Thread.Sleep(50);
                nws = nowState.RIGHT_WELD;
            }
            else if ((signal & (int)ComBit.Bit15) == 0)
            {
                EchoRight();
                nws = nowState.RIGHT_FINISH;
            }
        }

        private void StandRight(int signal)
        {
            _ma?.CloseAir();
            if ((signal & (int)ComBit.Bit14) == 0)
            {
                WeldRight();
                Thread.Sleep(50);
                nws = nowState.RIGHT_WELD;
            }
            else if ((signal & (int)ComBit.Bit12) == 0)
            {
                FastMoveToLeft();
                WeldLeft();
                Thread.Sleep(50);
                nws = nowState.LEFT_WELD;
            }
        }

        private void FastMoveToRight()
        {
            if (pots2 != null)
                _ma?.LeapFrog(pots2[0]);
        }

        private void FastMoveToLeft()
        {
            if (pots1 != null)
                _ma?.LeapFrog(pots1[0]);
        }

        private void WeldLeft()
        {
            _ma?.SendBit(12);

            // setup special laser power for each section
            if (lplist != null && lplist.Count > 0)
            {
                _ma?.SetupLaserPower(lplist[0].LaserPower);
                dmFile?.SetInputPower(lplist[0].LaserPower);
            }

            if (pots1 != null)
            {
                int ptcount = pots1.Count;
                List<int[]> workpts = new();
                for (int i = 1; i < ptcount - 1; i++)
                {
                    workpts.Add(pots1[i]);
                }

                _ma?.WeldInCard(workpts, 13);
            }
            /*            Thread.Sleep(100);
                        _ma.SendBit(13);*/
        }


        private void EchoLeft()
        {
            _ma?.CloseAir();
            _ma?.SendBit(13);
            if (pots1 != null)
                _ma?.LeapFrog(pots1[^1]);
        }

        private void WeldRight()
        {
            //NMC_SetDOBit(devHandle, 14, 0);
            _ma?.SendBit(14);

            // setup special laser power for each section
            if (lplist != null && lplist.Count > 0)
            {
                _ma?.SetupLaserPower(lplist[1].LaserPower);
                dmFile?.SetInputPower(lplist[1].LaserPower);
            }

            if (pots2 != null)
            {
                int ptcount = pots2.Count;
                List<int[]> workpts = new();
                for (int i = 1; i < ptcount - 1; i++)
                {
                    workpts.Add(pots2[i]);
                }

                _ma?.WeldInCard(workpts, 15);
            }
            /*            Thread.Sleep(100);
                        _ma.SendBit(15);*/
        }

        private void EchoRight()
        {
            _ma?.CloseAir();
            _ma?.SendBit(15);
            if (pots2 != null)
                _ma?.LeapFrog(pots2[^1]);
        }

        //
        // constants: work statement
        //
        public enum nowState
        {
            LEFT_WELD,
            LEFT_FINISH,
            RIGHT_WELD,
            RIGHT_FINISH,
            RESET_POS,
        }
    }
}
