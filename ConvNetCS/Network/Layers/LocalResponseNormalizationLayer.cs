using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class LocalResponseNormalizationLayer : ILayer
    {
        public float k { get; set; }
        public float n { get; set; }
        public float alpha { get; set; }
        public float beta { get; set; }
        public Vol input { get; set; }
        public Vol S_cache_ { get; set; }


        public LocalResponseNormalizationLayer(int inputDepth, int inputWidth, 
            int inputHeight, float k, float n, float alpha, float beta)
        {
            this.k = k;
            this.n = n;
            if (n%2 ==0)
            {
                throw new Exception("n must be odd in lrn");
            }
            this.alpha = alpha;
            this.beta = beta;

            this.InputDepth = inputDepth;
            this.InputWidth = inputWidth;
            this.InputHeight = inputHeight;
            this.OutputWidth = inputWidth;
            this.OutputHeight = inputHeight;
            this.OutputDepth = inputDepth;
        }
        public void Backward()
        {
            // evaluate gradient wrt data
            var V = this.input; // we need to set dw of this
            V.DW = new float[V.W.Length]; // zero out gradient wrt data
            var A = this.Output; // computed in forward pass 

            var n2 = Math.Floor(this.n / 2);
            DLRN(V, (float)n2);
        }

        private void DLRN(Vol V, float n2)
        {

            var source = Enumerable.Range(0, V.Width);
            var pquery = from num in source.AsParallel()
                         select num;
            pquery.ForAll((x) => DLRNforWidth(V, n2, x));
            
        }

        private void DLRNforWidth(Vol V, float n2, int x)
        {
            for (var y = 0; y < V.Height; y++)
            {
                for (var i = 0; i < V.Depth; i++)
                {

                    var chain_grad = this.Output.Get_Grad(x, y, i);
                    var S = this.S_cache_.Get(x, y, i);
                    var SB = Math.Pow(S, this.beta);
                    var SB2 = SB * SB;

                    // normalize in a window of size n
                    for (var j = Math.Max(0, i - n2); j <= Math.Min(i + n2, V.Depth - 1); j++)
                    {
                        var aj = V.Get(x, y, (int)j);
                        var g = -aj * this.beta * Math.Pow(S, this.beta - 1) * this.alpha / this.n * 2 * aj;
                        if (j == i) g += SB;
                        g /= SB2;
                        g *= chain_grad;
                        V.Add_Grad(x, y, (int)j, (float) g);
                    }

                }
            }
        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.input = V;

            var A = V.CloneAndZero();
            this.S_cache_ = V.CloneAndZero();
            var n2 = Math.Floor(this.n / 2);
            LRN(V, A, (float) n2);

            this.Output = A;
            return this.Output; // dummy identity function for now

        }

        private void LRN(Vol V, Vol A, float n2)
        {
            var source = Enumerable.Range(0, V.Width);
            var pquery = from num in source.AsParallel()
                         select num;
            pquery.ForAll((x) => LRNforWidth(V, A, n2, x));
           
        }

        private void LRNforWidth(Vol V, Vol A, float n2, int x)
        {
            for (var y = 0; y < V.Height; y++)
            {
                for (var i = 0; i < V.Depth; i++)
                {

                    var ai = V.Get(x, y, i);

                    // normalize in a window of size n
                    var den = 0.0;
                    for (var j = Math.Max(0, i - n2); j <= Math.Min(i + n2, V.Depth - 1); j++)
                    {
                        var aa = V.Get(x, y, (int)j);
                        den += aa * aa;
                    }
                    den *= this.alpha / this.n;
                    den += this.k;
                    this.S_cache_.Set(x, y, i,  (float) den); // will be useful for backprop
                    den = Math.Pow(den, this.beta);
                    A.Set(x, y, i, (float) ai / (float) den);
                }
            }
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
