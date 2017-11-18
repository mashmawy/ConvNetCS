using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    // This file contains all layers that do dot products with input,
    // but usually in a different connectivity pattern and weight sharing
    // schemes: 
    // - FullyConn is fully connected dot products 
    // - ConvLayer does convolutions (so weight sharing spatially)
    // putting them together in one file because they are very similar
    [Serializable]
    public class ConvLayer : ILayer
    {
        public int OutputDepth { get; set; }
        public int FilterWidth { get; set; }// filter size. Should be odd if possible, it's cleaner.
        public int FilterHeight { get; set; }
        public int InputDepth { get; set; }
        public int InputWidth { get; set; }
        public int InputHeight { get; set; }
        public int Stride { get; set; }
        public int Pad { get; set; }
        public float L1_Decay_Mul { get; set; }
        public float L2_Decay_Mul { get; set; }
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; }
        public Vol Biases { get; set; }
        public Vol[] Filters { get; set; }

        public Vol Input { get; set; }
        public Vol Output { get; set; }

        public ConvLayer(int filters, int filtersize, int inputDepth, int inputSize )
        {
            Init(filters, filtersize, filtersize,
                inputDepth, inputSize, inputSize, 1, 2, 0, 1, 0.1f);
        }

        public ConvLayer(int filters, int filterWidth, int filterHeight, int inputDepth, int inputHeight,
            int inputWidth)
        {
            Init(filters, filterWidth, filterHeight,
                inputDepth, inputHeight, inputWidth, 1, 2, 0, 1, 0.1f);
        }

        public ConvLayer(int filters, int filterWidth, int filterHeight, int inputDepth,
            int inputHeight, int inputWidth,
            int stride, int pad, float l1_decay, float l2_decay, float bais_pref)
        {
            Init(filters, filterWidth, filterHeight, 
                inputDepth, inputHeight, inputWidth, stride, pad, l1_decay, l2_decay, bais_pref);

        }

        private void Init(int filters, int filterWidth, int filterHeight, int inputDepth, int inputHeight, int inputWidth, int stride, int pad, float l1_decay, float l2_decay, float bais_pref)
        {
            this.OutputDepth = filters;
            this.FilterWidth = filterWidth;
            this.InputDepth = inputDepth;
            this.InputWidth = inputWidth;
            this.InputHeight = inputHeight;
            this.FilterHeight = filterHeight;
            this.Stride = stride;
            this.Pad = pad;
            this.L1_Decay_Mul = l1_decay;
            this.L2_Decay_Mul = l2_decay;

            this.OutputWidth = ((this.InputWidth + (this.Pad * 2) - this.FilterWidth) / this.Stride) + 1;
            this.OutputHeight = ((this.InputHeight + (this.Pad * 2) - this.FilterHeight) / this.Stride) + 1;

            var bais = bais_pref;
            this.Filters = new Vol[filters];
            for (int i = 0; i < filters; i++)
            {
                this.Filters[i] = (new Vol(this.FilterWidth, this.FilterHeight, this.InputDepth));
            }

            this.Biases = new Vol(1, 1, this.OutputDepth, bais);
        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.Input = V;
            var tempOutput = new Vol(this.OutputWidth | 0, this.OutputHeight | 0, this.OutputDepth | 0, 0.0f);

            var inputWidth = V.Width | 0;
            var inputHeight = V.Height | 0;
            var xy_stride = this.Stride | 0; 
            for (int d = 0; d < this.OutputDepth; d++)
            {
                ConvFilter(V, tempOutput, inputWidth, inputHeight, xy_stride, d);
            }  
            this.Output = tempOutput;
            return this.Output;
        }
 
        private void ConvFilter(Vol V, Vol tempOutput, int inputWidth, int inputHeight, int xy_stride, int d)
        {
            var source = Enumerable.Range(0, this.OutputHeight);
            var pquery = from num in source.AsParallel()
                         select num;
            pquery.ForAll((ay) => ConvOverRows(V, tempOutput, inputWidth, inputHeight, xy_stride, d,  ay));

        }

        private void ConvOverRows(Vol V, Vol tempOutput, int inputWidth, int inputHeight, int xy_stride, int d, int ay)
        {
            var y = (-this.Pad | 0) + (xy_stride * ay);
            var f = this.Filters[d];
            for (var ax = 0; ax < this.OutputWidth; ax++) // for each out width
            {
                var x = (-this.Pad | 0)+ (xy_stride * ax);  
                // convolve centered at this particular location
                var a = 0.0;
                for (var fy = 0; fy < f.Height; fy++) // for each element in the filter height
                {
                    var oy = y+ fy; // coordinates in the original input array coordinates
                    for (var fx = 0; fx < f.Width; fx++) // for each element in the filter width
                    {
                        //x is current width element of the output
                        //fx is the current width element of the filter
                        var ox = x + fx;
                        if (oy >= 0 && oy < inputHeight && ox >= 0 && ox < inputWidth)
                        {
                            for (var fd = 0; fd < f.Depth; fd++) // for each filter depth or input depth
                            {
                                // multiply filter pixel by image pixel and add (shared weight filter)
                                // avoid function call overhead (x2) for efficiency, compromise modularity :(
                                //filter (fx,fy,fd) *
                                //input (ox,oy,fd)
                                a += f.W[((f.Width * fy) + fx) * f.Depth + fd] *
                                    V.W[((inputWidth * oy) + ox) * V.Depth + fd];
                            }
                        }
                    }
                }
                a += this.Biases.W[d];
                
                tempOutput.Set(ax, ay, d, (float)a);
            }
        }
         
        public void Backward()
        {
            var V = this.Input;
            V.DW = new float[V.W.Length];  
            var inputWidth = V.Width | 0;
            var inputHeight = V.Height | 0;
            var xy_stride = this.Stride | 0;

            for (var d = 0; d < this.OutputDepth; d++)
            {
                var f = this.Filters[d];
                var x = -this.Pad | 0;
                var y = -this.Pad | 0;
                for (var ay = 0; ay < this.OutputHeight; y += xy_stride, ay++)
                {  // xy_stride
                    x = -this.Pad | 0;
                    for (var ax = 0; ax < this.OutputWidth; x += xy_stride, ax++)
                    {  // xy_stride

                        // convolve centered at this particular location
                        var chain_grad = this.Output.Get_Grad(ax, ay, d); // gradient from above, from chain rule
                        for (var fy = 0; fy < f.Height; fy++)
                        {
                            var oy = y + fy; // coordinates in the original input array coordinates
                            for (var fx = 0; fx < f.Width; fx++)
                            {
                                var ox = x + fx;
                                if (oy >= 0 && oy < inputHeight && ox >= 0 && ox < inputWidth) // check if oy not <0 or oy > input height
                                {
                                    for (var fd = 0; fd < f.Depth; fd++)
                                    {
                                        // avoid function call overhead (x2) for efficiency, compromise modularity :(
                                        //V(ox,oy,fd)
                                        //f(fx,fy,fd)
                                        var ix1 = ((inputWidth * oy) + ox) * V.Depth + fd;
                                        var ix2 = ((f.Width * fy) + fx) * f.Depth + fd;
                                        f.DW[ix2] += V.W[ix1] * chain_grad;
                                        V.DW[ix1] += f.W[ix2] * chain_grad;
                                    }
                                }
                            }
                        }
                        this.Biases.DW[d] += chain_grad;
                    }
                }
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

    }


}
