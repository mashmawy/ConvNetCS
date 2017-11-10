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
        public float[] Params { get; set; }
        public float[] Grads { get; set; }
        public float l1_decay_mul { get; set; }
        public float l2_decay_mul { get; set; }

    }
}
