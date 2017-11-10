using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ConvNetCS
{
    [Serializable]
    public class FullyConnLayer : ILayer
    {
        public int OutputDepth { get; set; }
        public float L1_Decay_Mul { get; set; }
        public float L2_Decay_Mul { get; set; }
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; }
        public int num_inputs { get; set; }
        public Vol Biases { get; set; }
        public Vol[] Filters { get; set; } 
        public Vol input { get; set; }
        public Vol Output { get; set; }

        public FullyConnLayer(int num_neurons, int inputDepth,
     int inputWidth, int inputHeight )
        {
            Init(num_neurons, inputDepth, inputWidth, inputHeight,
                0.0f, 1.0f, 0.0f);

        }

        public FullyConnLayer(int num_neurons, int inputDepth,
            int inputWidth, int inputHeight,
              float l1_decay, float l2_decay, float bais_pref)
        {
            Init(num_neurons, inputDepth, inputWidth, inputHeight, l1_decay, l2_decay, bais_pref);

        }

        private void Init(int num_neurons, int inputDepth, int inputWidth, int inputHeight, float l1_decay, float l2_decay, float bais_pref)
        {
            this.OutputDepth = num_neurons;

            this.InputHeight = inputHeight;
            this.InputWidth = inputWidth;
            this.InputDepth = inputDepth;

            this.L1_Decay_Mul = l1_decay;
            this.L2_Decay_Mul = l2_decay;

            this.num_inputs = inputWidth * inputHeight * inputDepth;
            this.OutputWidth = 1;
            this.OutputHeight = 1;


            var bais = bais_pref;
            this.Filters = new Vol[this.OutputDepth];
            for (int i = 0; i < this.OutputDepth; i++)
            {
                this.Filters[i] = (new Vol(1, 1, this.num_inputs));
            }

            this.Biases = new Vol(1, 1, this.OutputDepth, bais);
        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.input = V;
            var A = new Vol(1, 1, this.OutputDepth, 0.0f);
            var Vw = V.W;
            Product(A, Vw);
            this.Output = A;
            return this.Output;
        }

        private void Product(Vol A, float[] Vw)
        {


            var source = Enumerable.Range(0, this.OutputDepth);
            var pquery = from num in source.AsParallel()
                         select num;
            pquery.ForAll((i) => MulChannel(A, Vw, i));


        }

        private void MulChannel(Vol A, float[] Vw, int i)
        {
            var a = 0.0f;
            var wi = this.Filters[i].W;
            for (var d = 0; d < this.num_inputs; d++)
            {
                a += Vw[d] * wi[d];  
            }
            a += this.Biases.W[i];
            A.W[i] = a;
        }

        public void Backward()
        {
            var V = this.input;
            V.DW = new float[V.W.Length]; // zero out the gradient in input Vol

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
                l1_decay_mul = 0.0f,
                l2_decay_mul = 0.0f
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
