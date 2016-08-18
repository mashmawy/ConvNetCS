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
        public double k { get; set; }
        public double n { get; set; }
        public double alpha { get; set; }
        public double beta { get; set; }
        public Vol in_Act { get; set; }
        public Vol S_cache_ { get; set; }


        public LocalResponseNormalizationLayer(int in_depth, int in_sx, int in_sy, double k, double n, double alpha, double beta)
        {
            this.k = k;
            this.n = n;
            if (n%2 ==0)
            {
                throw new Exception("n must be odd in lrn");
            }
            this.alpha = alpha;
            this.beta = beta;

            this.InputDepth = in_depth;
            this.InputWidth = in_sx;
            this.InputHeight = in_sy;
            this.OutputWidth = in_sx;
            this.OutputHeight = in_sy;
            this.OutputDepth = in_depth;
        }
        public void Backward()
        {
            // evaluate gradient wrt data
            var V = this.in_Act; // we need to set dw of this
            V.DW = new double[V.W.Length]; // zero out gradient wrt data
            var A = this.Output; // computed in forward pass 

            var n2 = Math.Floor(this.n / 2);
            DLRN(V, n2);
        }

        private void DLRN(Vol V, double n2)
        {

            var source = Enumerable.Range(0, V.SX);
            var pquery = from num in source.AsParallel()
                         select num;
            pquery.ForAll((x) => DLRNforWidth(V, n2, x));
            
        }

        private void DLRNforWidth(Vol V, double n2, int x)
        {
            for (var y = 0; y < V.SY; y++)
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
                        V.Add_Grad(x, y, (int)j, g);
                    }

                }
            }
        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.in_Act = V;

            var A = V.CloneAndZero();
            this.S_cache_ = V.CloneAndZero();
            var n2 = Math.Floor(this.n / 2);
            LRN(V, A, n2);

            this.Output = A;
            return this.Output; // dummy identity function for now

        }

        private void LRN(Vol V, Vol A, double n2)
        {
            var source = Enumerable.Range(0, V.SX);
            var pquery = from num in source.AsParallel()
                         select num;
            pquery.ForAll((x) => LRNforWidth(V, A, n2, x));
           
        }

        private void LRNforWidth(Vol V, Vol A, double n2, int x)
        {
            for (var y = 0; y < V.SY; y++)
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
                    this.S_cache_.Set(x, y, i, den); // will be useful for backprop
                    den = Math.Pow(den, this.beta);
                    A.Set(x, y, i, ai / den);
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
