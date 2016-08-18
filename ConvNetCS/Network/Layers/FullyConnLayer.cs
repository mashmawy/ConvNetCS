using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class FullyConnLayer : ILayer
    {
        public int OutputDepth { get; set; }
        public double L1_Decay_Mul { get; set; }
        public double L2_Decay_Mul { get; set; }
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; }
        public int num_inputs { get; set; }
        public Vol Biases { get; set; }
        public List<Vol> Filters { get; set; }
        public int SX { get; set; }
        public int SY { get; set; }
        public Vol in_Act { get; set; }
        public Vol Output { get; set; }

        public FullyConnLayer(int num_neurons, int in_depth, int in_sx, int in_sy,
              double l1_decay, double l2_decay, double bais_pref)
        {
            this.OutputDepth = num_neurons;

            this.InputHeight = in_sy;
            this.InputWidth = in_sx;
            this.InputDepth = in_depth;

            this.L1_Decay_Mul = l1_decay;//0.0
            this.L2_Decay_Mul = l2_decay;//1.0

            this.num_inputs = in_sx * in_sy * in_depth;
            this.OutputWidth = 1;
            this.OutputHeight = 1;


            var bais = bais_pref;//0.0;
            this.Filters = new List<Vol>();
            for (int i = 0; i < this.OutputDepth; i++)
            {
                this.Filters.Add(new Vol(1, 1, this.num_inputs));

            }
            this.Biases = new Vol(1, 1, this.OutputDepth, bais);
        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.in_Act = V;
            var A = new Vol(1, 1, this.OutputDepth, 0.0);
            var Vw = V.W;
            for (var i = 0; i < this.OutputDepth; i++)
            {
                var a = 0.0;
                var wi = this.Filters[i].W;
                for (var d = 0; d < this.num_inputs; d++)
                {
                    a += Vw[d] * wi[d]; // for efficiency use Vols directly for now
                }
                a += this.Biases.W[i];
                A.W[i] = a;
            }
            this.Output = A;
            return this.Output;
        }

        public void Backward()
        {
            var V = this.in_Act;
            V.DW = new double[V.W.Length]; // zero out the gradient in input Vol

            // compute gradient wrt weights and data
            for (var i = 0; i < this.OutputDepth; i++)
            {
                var tfi = this.Filters[i];
                var chain_grad = this.Output.DW[i];
                for (var d = 0; d < this.num_inputs; d++)
                {
                    V.DW[d] += tfi.W[d] * chain_grad; // grad wrt input data
                    tfi.DW[d] += V.W[d] * chain_grad; // grad wrt params
                }
                this.Biases.DW[i] += chain_grad;
            }
        }

        public List<ParamsAndGrads> getParamsAndGrads()
        {
            List<ParamsAndGrads> response = new List<ParamsAndGrads>();

            for (var i = 0; i < this.OutputDepth; i++)
            {
                response.Add(new ParamsAndGrads()
                {
                    Params = this.Filters[i].W,
                    Grads = this.Filters[i].DW,
                    l2_decay_mul = this.L2_Decay_Mul,
                    l1_decay_mul = this.L1_Decay_Mul
                });
            }
            response.Add(new ParamsAndGrads()
            {
                Params = this.Biases.W,
                Grads = this.Biases.DW,
                l1_decay_mul = 0.0,
                l2_decay_mul = 0.0
            });


            return response;
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
    }
}
