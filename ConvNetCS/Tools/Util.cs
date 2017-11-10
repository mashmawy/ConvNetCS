using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    [Serializable]
    public static class Util
    {

        public static bool Return_V { get; set; }
        public static float V_Val { get; set; }

        private static Random random = new Random(3);
        public static float GaussRandom()
        {
            if (Return_V)
            {
                Return_V = false;
                return V_Val;
            }
            var u = 2 * random.NextDouble() - 1;
            var v = 2 * random.NextDouble() - 1;
            var r = (u * u) + (v * v);

            if (r == 0 || r > 1) return GaussRandom();
            var c = Math.Sqrt(-2 * Math.Log(r) / r);
            V_Val =(float) (v * c);
            Return_V = true;
            return (float)(u * c);
        }

        public static float RandF(float a, float b)
        {

            return (float) random.NextDouble() * (b - a) + a;
        
        }

        public static float Randi(float a, float b)
        {
            return (float) Math.Floor(random.NextDouble() * (b - a) + a); 
        }
        public static float Randn(float mu, float std)
        {
            return mu + GaussRandom() * std;
        }

        public static bool ArrContains(float[] arr, float elt)
        {
            return arr.Contains(elt);
        }
        public static float[] ArrUnique(float[] arr)
        {
            List<float> d = new List<float>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (!d.Contains(arr[i]))
                {
                    d.Add(arr[i]);
                }
            }
            return d.ToArray();
        }

        public static maxminResult maxmin(float[] w)
        {
            var maxv = w[0];
            var minv = w[0];
            var maxi = 0;
            var mini = 0;
            var n = w.Length;
            for (int i = 0; i < n; i++)
            {
                if (w[i] > maxv) { maxv = w[i]; maxi = i; }
                if (w[i] < minv) { minv = w[i]; mini = i; } 
            }
            return  new maxminResult(){maxi= maxi, maxv= maxv, mini= mini, minv= minv, dv=maxv-minv};
        }


        public static float[] RandPerm(int n)
        {
            int i = n, j = 0;
            float temp = 0.0f;
            float[] array =new float[n];
            for (int q = 0; q < n; q++)
			{
                array[q] = q;
            }
            while (i > 0)
            {
                i--;
                j =(int) Math.Floor( random.NextDouble() * (i + 1));
                temp = array[i];
                array[i] = array[j];
                array[j] = temp;

            }


            return array;
        }

        public static float WeightedSample(float[] lst, float[] probs)
        {
            var p = RandF(0, 1);
            var cumprob = 0.0;
            for (int k = 0; k < lst.Length; k++)
            {
                cumprob += probs[k];
                if (p<cumprob)
                {
                    return lst[k];
                }
            }
            return -1;
        }

    }
    public struct maxminResult
    {
        public float maxi;
        public float maxv;
        public float mini;
        public float minv;
        public float dv;

    }
    [Serializable]
    public class DataToSave
    {
        public float[][] Data { get; set; }
        public float[] Labels { get; set; }

    }
}
