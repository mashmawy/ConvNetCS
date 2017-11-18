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
        public int KernelWidth { get; set; }// filter size. Should be odd if possible, it's cleaner.
        public int KernelHeight { get; set; }
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

        public Vol input { get; set; }
        public Vol Output { get; set; }
        public MaxPoolLayer(int kernelWidth, int kernelHeight, int inputDepth,
            int inputWidth, int inputHeight )
        {
            Init(kernelWidth, kernelHeight, inputDepth,
                inputWidth, inputHeight, 2, 0);

        }

        public MaxPoolLayer(int kernelWidth, int kernelHeight, int inputDepth, int inputWidth, int inputHeight,
            int stride, int pad)
        {
            Init(kernelWidth, kernelHeight, inputDepth, inputWidth, inputHeight, stride, pad);

        }

        private void Init(int kernelWidth, int kernelHeight, int inputDepth, int inputWidth, int inputHeight, int stride, int pad)
        {
            this.OutputDepth = inputDepth;
            this.KernelWidth = kernelWidth;
            this.InputDepth = inputDepth;
            this.InputWidth = inputWidth;
            this.InputHeight = inputHeight;
            this.KernelHeight = kernelHeight;
            this.Stride = stride; //or 2
            this.Pad = pad;//0

            this.OutputWidth = ((this.InputWidth + (this.Pad * 2) - this.KernelWidth) / this.Stride) + 1;
            this.OutputHeight = ((this.InputHeight + (this.Pad * 2) - this.KernelHeight) / this.Stride) + 1;

            this.Switchx = new int[this.OutputWidth * this.OutputHeight * this.OutputDepth];
            this.Switchy = new int[this.OutputWidth * this.OutputHeight * this.OutputDepth];
        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.input = V;

            var A = new Vol(this.OutputWidth, this.OutputHeight, this.OutputDepth, 0.0f);
             Conv(V, is_training, A);
            
            this.Output = A;
            return this.Output;
        }

        private void Conv(Vol V, bool is_training, Vol A)
        {
            var n = 0; // a counter for switches
            for (int d = 0; d < this.OutputDepth; d++)
            {
                for (var ax = 0; ax < this.OutputWidth; ax++)
                {
                    var x = -this.Pad;
                    x += (this.Stride * ax);
                    for (var ay = 0; ay < this.OutputHeight;  ay++)
                    {
                        var y = -this.Pad; 
                        y += (this.Stride * ay); 
                        // convolve centered at this particular location
                        float a = -99999; // hopefully small enough ;\
                        var winx = -1; var winy = -1;
                        for (var fx = 0; fx < this.KernelWidth; fx++)
                        {
                            for (var fy = 0; fy < this.KernelHeight; fy++)
                            {
                                var oy = y + fy;
                                var ox = x + fx;
                                if (oy >= 0 && oy < V.Height && ox >= 0 && ox < V.Width)
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
            //var source = Enumerable.Range(0, this.OutputDepth);
            //var pquery = from num in source.AsParallel()
            //             select num;
          //  pquery.ForAll((d) => );
           
        }

       

        public void Backward()
        {
            // pooling layers have no parameters, so simply compute 
            // gradient wrt data here
            var V = this.input;
            V.DW = new float[V.W.Length]; // zero out gradient wrt data
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
