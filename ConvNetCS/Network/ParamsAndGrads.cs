using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class ParamsAndGrads
    {
        public double[] Params { get; set; }
        public double[] Grads { get; set; }
        public double l1_decay_mul { get; set; }
        public double l2_decay_mul { get; set; }

    }
}
