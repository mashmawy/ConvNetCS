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
        public float[] es { get; set; }
        public Vol input { get; set; }
        public Vol Output { get; set; }

        public SoftmaxLayer(int inputDepth, int inputWidth, int inputHeight)
        {
            this.num_inputs = inputWidth * inputHeight * inputDepth;
            this.Out_Depth = this.num_inputs;

            this.out_sx = 1;
            this.out_sy = 1;



        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.input = V;

            var A = new Vol(1, 1, this.Out_Depth, 0.0f);

            // compute max activation
            var ass = V.W;
            var amax = V.W[0];
            for (var i = 1; i < ass.Length; i++)
            {
                if (ass[i] > amax) amax = ass[i];
            }

            // compute exponentials (carefully to not blow up)
            var es = new float[this.Out_Depth];
            var esum = 0.0;
            for (var i = 0; i < this.Out_Depth; i++)
            {
                var e =(float) Math.Exp(ass[i] - amax);
                esum += e;
                es[i] = e;
            }

            // normalize and output to sum to one
            for (var i = 0; i < this.Out_Depth; i++)
            {
                es[i] /= (float)esum;
                A.W[i] = es[i];
            }

            this.es = es; // save these for backprop
            this.Output = A;
            return this.Output;
        }

        public float Backward(int y)
        {
            // compute and accumulate gradient wrt weights and bias of this layer
            var x = this.input;
            x.DW = new float[x.W.Length]; // zero out the gradient of input Vol

            for (var i = 0; i < this.Out_Depth; i++)
            {
                var indicator = i == y ? 1.0 : 0.0;
                var mul =(float) -(indicator - this.es[i]);
                x.DW[i] = mul;
            }

            // loss is the class negative log likelihood
            return (float) -Math.Log(this.es[y]);
        }

        public List<ParamsAndGrads> getParamsAndGrads()
        {
            List<ParamsAndGrads> response = new List<ParamsAndGrads>();


            return response;
        }


        public float Backward(float[] y)
        {
            throw new NotImplementedException();
        }
    }
}
