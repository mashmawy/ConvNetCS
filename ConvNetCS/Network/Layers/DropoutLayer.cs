using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class DropoutLayer : ILayer
    {
        public double drop_prob { get; set; }  
        public Vol in_Act { get; set; } 
        public bool[] dropped { get; set; }


        public DropoutLayer(int in_depth, int in_sx, int in_sy, double drop_prob )
        {
            this.drop_prob = drop_prob;
             

            this.InputDepth = in_depth;
            this.InputWidth = in_sx;
            this.InputHeight = in_sy;
            this.OutputWidth = in_sx;
            this.OutputHeight = in_sy;
            this.OutputDepth = in_depth;

            this.dropped = new bool[this.OutputWidth * this.OutputHeight * this.OutputDepth];
        }
        public void Backward()
        {
            var V = this.in_Act; // we need to set dw of this
            var chain_grad = this.Output;
            var N = V.W.Length;
            V.DW = new double[N]; // zero out gradient wrt data
            for (var i = 0; i < N; i++)
            {
                if (!(this.dropped[i]))
                {
                    V.DW[i] = chain_grad.DW[i]; // copy over the gradient
                }
            }
        }
        Random r = new Random();
        public Vol Forward(Vol V, bool is_training)
        {
            this.in_Act = V;
            var V2 = V.Clone();
            var N = V.W.Length;
            if (is_training)
            {
                // do dropout
                for (var i = 0; i < N; i++)
                {
                    if (r.NextDouble() < this.drop_prob) { V2.W[i] = 0; this.dropped[i] = true; } // drop!
                    else { this.dropped[i] = false; }
                }
            }
            else
            {
                // scale the activations during prediction
                for (var i = 0; i < N; i++) { V2.W[i] *= this.drop_prob; }
            }
            this.Output = V2;
            return this.Output; // dummy identity function for now
        }

        public List<ParamsAndGrads> getParamsAndGrads()
        {

            List<ParamsAndGrads> response = new List<ParamsAndGrads>();


            return response;
        }

        public Vol Output
        {
            get;
            set;
        }

        public int InputDepth
        {
            get;
            set;
        }

        public int InputWidth
        {
            get;
            set;
        }

        public int InputHeight
        {
            get;
            set;
        }

        public int OutputDepth
        {
            get;
            set;
        }

        public int OutputWidth
        {
            get;
            set;
        }

        public int OutputHeight
        {
            get;
            set;
        }
    }
}
