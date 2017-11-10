using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class ReluLayer:ILayer
    {
        public int OutputDepth { get; set; }
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; } 
        public int num_inputs { get; set; }
        public Vol Biases { get; set; }
        public List<Vol> Filters { get; set; }
        public float[] es { get; set; }
        public Vol input { get; set; }
        public Vol Output { get; set; }

        public ReluLayer(int inputDepth, int inputWidth, int inputHeight)
        {
            this.OutputDepth = inputDepth; 
            this.OutputWidth = inputWidth;
            this.OutputHeight = inputHeight; 
            this.InputHeight = inputHeight;
            this.InputWidth = inputWidth;
            this.InputDepth = inputDepth; 
        }

        public Vol Forward(Vol V, bool is_training)
        {
            this.input = V;
            var V2 = V.Clone();
            var N = V.W.Length;
            var V2w = V2.W;
            for (var i = 0; i < N; i++)
            {
                if (V2w[i] < 0) V2w[i] = 0; // threshold at 0
            }
            this.Output = V2;
            return this.Output;
        }

        public void Backward()
        {
            var V = this.input; // we need to set dw of this
            var V2 = this.Output;
            var N = V.W.Length;
            V.DW = new float[N]; // zero out gradient wrt data
            for (var i = 0; i < N; i++)
            {
                if (V2.W[i] <= 0) V.DW[i] = 0; // threshold
                else V.DW[i] = V2.DW[i];
            }
        }

        public List<ParamsAndGrads> getParamsAndGrads()
        {
            List<ParamsAndGrads> response = new List<ParamsAndGrads>();


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
