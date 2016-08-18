using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class MaxPoolLayer : ILayer
    {
        public int OutputDepth { get; set; }
        public int SX { get; set; }// filter size. Should be odd if possible, it's cleaner.
        public int SY { get; set; }
        public int InputDepth { get; set; }
        public int InputWidth { get; set; }
        public int InputHeight { get; set; }
        public int Stride { get; set; }
        public int Pad { get; set; }
        public int[] Switchx { get; set; }
        public int[] Switchy { get; set; }
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; }
        public Vol Biases { get; set; }
        public List<Vol> Filters { get; set; }

        public Vol in_Act { get; set; }
        public Vol Output { get; set; }

        public MaxPoolLayer(int sx, int sy, int in_depth, int in_sx, int in_sy,
            int stride, int pad, double bais_pref)
        {
            this.OutputDepth = in_depth;
            this.SX = sx;
            this.InputDepth = in_depth;
            this.InputWidth = in_sx;
            this.InputHeight = in_sy;
            this.SY = sy;
            this.Stride = stride; //or 2
            this.Pad = pad;//0

            this.OutputWidth = (this.InputWidth + this.Pad * 2 - this.SX) / this.Stride + 1;
            this.OutputHeight = (this.InputHeight + this.Pad * 2 - this.SY) / this.Stride + 1;

            this.Switchx = new int[this.OutputWidth * this.OutputHeight * this.OutputDepth];
            this.Switchy = new int[this.OutputWidth * this.OutputHeight * this.OutputDepth];

        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.in_Act = V;

            var A = new Vol(this.OutputWidth, this.OutputHeight, this.OutputDepth, 0.0);

            var n = 0; // a counter for switches
            for (var d = 0; d < this.OutputDepth; d++)
            {
                var x = -this.Pad;
                var y = -this.Pad;
                for (var ax = 0; ax < this.OutputWidth; x += this.Stride, ax++)
                {
                    y = -this.Pad;
                    for (var ay = 0; ay < this.OutputHeight; y += this.Stride, ay++)
                    {

                        // convolve centered at this particular location
                        double a = -99999; // hopefully small enough ;\
                        var winx = -1; var winy = -1;
                        for (var fx = 0; fx < this.SX; fx++)
                        {
                            for (var fy = 0; fy < this.SY; fy++)
                            {
                                var oy = y + fy;
                                var ox = x + fx;
                                if (oy >= 0 && oy < V.SY && ox >= 0 && ox < V.SX)
                                {
                                    var v = V.Get(ox, oy, d);
                                    // perform max pooling and store pointers to where
                                    // the max came from. This will speed up backprop 
                                    // and can help make nice visualizations in future
                                    if (v > a) { a = v; winx = ox; winy = oy; }
                                }
                            }
                        }
                        this.Switchx[n] = winx;
                        this.Switchy[n] = winy;
                        n++;
                        A.Set(ax, ay, d, a);
                    }
                }
            }
            this.Output = A;
            return this.Output;
        }

        public void Backward()
        {
            // pooling layers have no parameters, so simply compute 
            // gradient wrt data here
            var V = this.in_Act;
            V.DW = new double[V.W.Length]; // zero out gradient wrt data
            var A = this.Output; // computed in forward pass 

            var n = 0;
            for (var d = 0; d < this.OutputDepth; d++)
            {
                var x = -this.Pad;
                var y = -this.Pad;
                for (var ax = 0; ax < this.OutputWidth; x += this.Stride, ax++)
                {
                    y = -this.Pad;
                    for (var ay = 0; ay < this.OutputHeight; y += this.Stride, ay++)
                    {

                        var chain_grad = this.Output.Get_Grad(ax, ay, d);
                        V.Add_Grad(this.Switchx[n], this.Switchy[n], d, chain_grad);
                        n++;

                    }
                }
            }
        }

        public List<ParamsAndGrads> getParamsAndGrads()
        {
            List<ParamsAndGrads> response = new List<ParamsAndGrads>();


            return response;
        }

    }
}
