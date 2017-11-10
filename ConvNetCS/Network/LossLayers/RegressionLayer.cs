using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class RegressionLayer : ILossLayer
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

        public RegressionLayer(int inputDepth, int inputWidth, int inputHeight)
        {
            this.num_inputs = inputWidth * inputHeight * inputDepth;
            this.Out_Depth = this.num_inputs;

            this.out_sx = 1;
            this.out_sy = 1;



        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.input = V;
            this.Output = V;
            return V; // identity function
        }

        public float Backward(float[] y)
        {
            var x = this.input;
            x.DW = new float[x.W.Length]; // zero out the gradient of input Vol
            var loss = 0.0f;
            for (var i = 0; i < this.Out_Depth; i++)
            {
                var dy = x.W[i] - y[i];
                x.DW[i] = dy;
                loss += 0.5f * dy * dy;
            }

            return loss;
        }
        public float Backward(int  y)
        {
            var x = this.input;
            x.DW = new float[x.W.Length]; // zero out the gradient of input Vol
            var loss = 0.0f;
            var dy = x.W[0] - y;
            x.DW[0] = dy;
            loss += 0.5f * dy * dy;

            return loss;
        }
        public float Backward(ClassOutput y)
        {
            var x = this.input;
            x.DW = new float[x.W.Length]; // zero out the gradient of input Vol
            var loss = 0.0f;
            var i = y.dim;
            var yi = y.val;
            var dy = x.W[i] - yi;
            x.DW[i] = dy;
            loss += 0.5f * dy * dy;

            return loss;
        }


        public List<ParamsAndGrads> getParamsAndGrads()
        {
            List<ParamsAndGrads> response = new List<ParamsAndGrads>();


            return response;
        }


       
    }


    public struct ClassOutput
    {
        public int dim; public float val;
    }
}
