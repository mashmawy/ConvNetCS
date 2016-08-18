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
        public static double V_Val { get; set; }

        private static Random random = new Random(3);
        public static double GaussRandom()
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
            V_Val = v * c;
            Return_V = true;
            return u * c;
        }

        public static double RandF(double a, double b)
        {

            return random.NextDouble() * (b - a) + a;
        
        }

        public static double Randi(double a, double b)
        { 
            return Math.Floor(  random.NextDouble() * (b - a) + a); 
        }
        public static double Randn(double mu, double std)
        {
            return mu + GaussRandom() * std;
        }

        public static bool ArrContains(double[] arr, double elt)
        {
            return arr.Contains(elt);
        }
        public static double[] ArrUnique(double[] arr)
        {
            List<double> d = new List<double>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (!d.Contains(arr[i]))
                {
                    d.Add(arr[i]);
                }
            }
            return d.ToArray();
        }

        public static maxminResult maxmin(double[] w)
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


        public static double[] RandPerm(int n)
        {
            int i = n, j = 0;
            double temp = 0.0;
            double[] array =new double[n];
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

        public static double WeightedSample(double[] lst, double[] probs)
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
        public double maxi;
        public double maxv;
        public double mini;
        public double minv;
        public double dv;

    }
    [Serializable]
    public class DataToSave
    {
        public double[][] Data { get; set; }
        public double[] Labels { get; set; }

    }
}
