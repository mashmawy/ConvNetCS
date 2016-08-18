using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class SVMLayer : ILossLayer
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

        public SVMLayer(int in_depth, int in_sx, int in_sy)
        {
            this.num_inputs = in_sx * in_sy * in_depth;
            this.Out_Depth = this.num_inputs;

            this.out_sx = 1;
            this.out_sy = 1;



        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.in_Act = V;
            this.Output = V; // nothing to do, output raw scores
            return V;
        }

        public double Backward(int y)
        {
            // compute and accumulate gradient wrt weights and bias of this layer
            var x = this.in_Act;
            x.DW = new double[x.W.Length]; // zero out the gradient of input Vol

            // we're using structured loss here, which means that the score
            // of the ground truth should be higher than the score of any other 
            // class, by a margin
            var yscore = x.W[y]; // score of ground truth
            var margin = 1.0;
            var loss = 0.0;
            for (var i = 0; i < this.Out_Depth; i++)
            {
                if (y == i) { continue; }
                var ydiff = -yscore + x.W[i] + margin;
                if (ydiff > 0)
                {
                    // violating dimension, apply loss
                    x.DW[i] += 1;
                    x.DW[y] -= 1;
                    loss += ydiff;
                }
            }

            return loss;
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
