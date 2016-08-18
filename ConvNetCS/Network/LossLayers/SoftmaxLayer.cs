using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class SoftmaxLayer :  ILossLayer
    {
        public int Out_Depth { get; set; }
        public int out_sx { get; set; }
        public int out_sy { get; set; }
        public int SX { get; set; }
        public int SY { get; set; }
        public int num_inputs { get; set; }
        public Vol Biases { get; set; }
        public List<Vol> Filters { get; set; }
        public double[] es { get; set; }
        public Vol in_Act { get; set; }
        public Vol Output { get; set; }

        public SoftmaxLayer(int in_depth, int in_sx, int in_sy)
        {
            this.num_inputs = in_sx * in_sy * in_depth;
            this.Out_Depth = this.num_inputs;

            this.out_sx = 1;
            this.out_sy = 1;



        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.in_Act = V;

            var A = new Vol(1, 1, this.Out_Depth, 0.0);

            // compute max activation
            var ass = V.W;
            var amax = V.W[0];
            for (var i = 1; i < this.Out_Depth; i++)
            {
                if (ass[i] > amax) amax = ass[i];
            }

            // compute exponentials (carefully to not blow up)
            var es = new double[this.Out_Depth];
            var esum = 0.0;
            for (var i = 0; i < this.Out_Depth; i++)
            {
                var e = Math.Exp(ass[i] - amax);
                esum += e;
                es[i] = e;
            }

            // normalize and output to sum to one
            for (var i = 0; i < this.Out_Depth; i++)
            {
                es[i] /= esum;
                A.W[i] = es[i];
            }

            this.es = es; // save these for backprop
            this.Output = A;
            return this.Output;
        }

        public double Backward(int y)
        {
            // compute and accumulate gradient wrt weights and bias of this layer
            var x = this.in_Act;
            x.DW = new double[x.W.Length]; // zero out the gradient of input Vol

            for (var i = 0; i < this.Out_Depth; i++)
            {
                var indicator = i == y ? 1.0 : 0.0;
                var mul = -(indicator - this.es[i]);
                x.DW[i] = mul;
            }

            // loss is the class negative log likelihood
            return -Math.Log(this.es[y]);
        }

        public List<ParamsAndGrads> getParamsAndGrads()
        {
            List<ParamsAndGrads> response = new List<ParamsAndGrads>();


            return response;
        }


        public double Backward(double[] y)
        {
            throw new NotImplementedException();
        }
    }
}
