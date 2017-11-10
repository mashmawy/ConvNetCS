using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    // An agent is in state0 and does action0
    // environment then assigns reward0 and provides new state, state1
    // Experience nodes store all this information, which is used in the
    // Q-learning update step
    public class Experience
    {
        public float[] state0 { get; set; }
        public float action0 { get; set; }
        public float reward0 { get; set; }
        public float[] state1 { get; set; }

        public Experience()
        {
        }
        public Experience(float[] state0, float action0, float reward0, float[] state1)
        {
            this.state0 = state0;
            this.action0 = action0;
            this.reward0 = reward0;
            this.state1 = state1;
        }

    }
}
