using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public class InputLayer :  ILayer 
    {
        public int OutputDepth { get; set; }
        public int OutputWidth { get; set; }
        public int OutputHeight { get; set; }
        public int SX { get; set; } 
        public int SY { get; set; }
        public int num_inputs { get; set; }
        public Vol Biases { get; set; }
        public List<Vol> Filters { get; set; }

        public Vol in_Act { get; set; }
        public Vol Output { get; set; }

        public InputLayer( )
        {
           
             
             
        }

        public   Vol Forward(Vol V, bool is_training)
        {
            this.in_Act = V;
            var A = new Vol(1, 1, this.OutputDepth, 0.0);
          
            this.Output = V;
            return this.Output;
        }

        public void Backward()
        {
            
        }

        public   List<ParamsAndGrads> getParamsAndGrads()
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
