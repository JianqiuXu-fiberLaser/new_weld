using RobotWeld2.Motions;
using RobotWeld2.Welding;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Printing.IndexedProperties;
using System.Threading;

namespace RobotWeld2.GetTrace
{
    public class SingleStep
    {
        private readonly MotionOperate _mo;
        private double moveSpeed;

        public SingleStep(MotionOperate mo)
        {
            _mo = mo;
        }

        public void SetupParameter(double moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }

        public void GotoPosition(int[] ptxyz)
        {
            List<int[]> xyza = new();

            int x = MotionSpecification.XDirection * ptxyz[0];
            int y = MotionSpecification.YDirection * ptxyz[1];
            int z = MotionSpecification.XDirection * ptxyz[2];
            int a = ptxyz[3];

            xyza.Add(new int[] { x, y, z, a });
            List<double> speed = new()
            {
                this.moveSpeed
            };

            new Thread(_ => _mo.Move4D(xyza, speed, 0)).Start();
        }
    }
}
