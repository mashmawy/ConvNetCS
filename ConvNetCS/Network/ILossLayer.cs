using System;
using System.Collections.Generic;
namespace ConvNetCS
{
    public interface ILossLayer
    {
        float Backward(float[] y);
        float Backward(int y);
        Vol Forward(Vol V, bool is_training);
        List<ParamsAndGrads> getParamsAndGrads();
        Vol Output { get; set; }
    }
}
