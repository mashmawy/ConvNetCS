using System;
using System.Collections.Generic;
namespace ConvNetCS
{
    public interface ILayer
    {
        void Backward();
        Vol Forward(Vol V, bool is_training);
        List<ParamsAndGrads> getParamsAndGrads();
        Vol Output { get; set; }

        int InputDepth { get; set; }
        int InputWidth { get; set; }
        int InputHeight { get; set; } 

        int OutputDepth { get; set; }
        int OutputWidth { get; set; }
        int OutputHeight { get; set; }
    }
}
