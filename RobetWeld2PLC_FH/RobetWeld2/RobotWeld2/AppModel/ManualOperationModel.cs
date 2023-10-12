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

using RobotWeld2.AlgorithmsBase;
using RobotWeld2.ViewModel;
using System.Threading;

namespace RobotWeld2.AppModel
{
    public class ManualOperationModel
    {
        private ManualOperationViewModel viewModel;
        private static ExtraController? extraController;

        public ManualOperationModel(ManualOperationViewModel ovm)
        {
            this.viewModel = ovm;
            extraController = new ExtraController(viewModel);
            extraController.ConnectPLC();
        }

        public void LeftClip()
        {
            if (extraController == null) return;

            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.LTOP_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.LEFT_TOPPING);
            }
            else if (extraController.GetState(ActionIndex.LBOTTOM_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.LEFT_TOPPING_2);
            }
        }

        public void RightClip()
        {
            if (extraController == null) return;

            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.RTOP_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.RIGHT_TOPPING);
            }
            else if (extraController.GetState(ActionIndex.RBOTTOM_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.RIGHT_TOPPING_2);
            }
        }

        public void LeftLocation()
        {
            if (extraController == null) return;

            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.LLOCATE_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.LEFT_LOCATE);
            }
            else if (extraController.GetState(ActionIndex.LBACK_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.LEFT_LOCATE_2);
            }
        }

        public void RightLocation()
        {
            if (extraController == null) return;

            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.RLOCATE_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.RIGHT_LOCATE);
            }
            else if (extraController.GetState(ActionIndex.RBACK_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.RIGHT_LOCATE_2);
            }

        }

        public void LeftJack()
        {
            if (extraController == null) return;
            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.LLOAD_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.LEFT_LOAD);
            }
            else if (extraController.GetState(ActionIndex.LUNLO_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.LEFT_LOAD_2);
            }
        }

        public void RightJack()
        {
            if (extraController == null) return;
            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.RLOAD_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.RIGHT_LOAD);
            }
            else if (extraController.GetState(ActionIndex.RUNLO_STATE))
            {
                extraController?.SelfResetTurnOn(ActionIndex.RIGHT_LOAD_2);
            }
        }

        internal void LeftEnter()
        {
            if (extraController == null) return;
            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.LEFT_WELD_STATE))
                extraController?.SelfResetTurnOn(ActionIndex.YRINORG);
            else if (extraController.GetState(ActionIndex.LEFT_ORG_STATE))
                extraController?.SelfResetTurnOn(ActionIndex.Y01_CLICK);
        }

        public void RightEnter()
        {
            if (extraController == null) return;
            extraController.SelfResetTurnOn(ActionIndex.MANUAL_MODE);
            Thread.Sleep(20);

            if (extraController.GetState(ActionIndex.RIGHT_WELD_STATE))
                extraController?.SelfResetTurnOn(ActionIndex.O02_CLICK);
            else if (extraController.GetState(ActionIndex.RIGHT_ORG_STATE))
                extraController?.SelfResetTurnOn(ActionIndex.Y11_CLICK);
        }

        public void ReadMValue()
        {
            extraController?.ShowMValue(viewModel.ActMemory);
        }
    }
}