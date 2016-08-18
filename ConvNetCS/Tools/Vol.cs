using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    // Vol is the basic building block of all data in a net.
    // it is essentially just a 3D volume of numbers, with a
    // width (sx), height (sy), and depth (depth).
    // it is used to hold data for all filters, all volumes,
    // all weights, and also stores all gradients w.r.t. 
    // the data. c is optionally a value to initialize the volume
    // with. If c is missing, fills the Vol with random numbers.
        [Serializable]
    public class Vol
    {
        public int SX { get; set; }
        public int SY { get; set; }
        public int Depth { get; set; }
        public double[] W { get; set; }
        public double[] DW { get; set; }
        public double[] step_cache { get; set; }

        public Vol(double[] sx)
        {
            this.SX = 1;
            this.SY = 1;
            this.Depth = sx.Length;
            this.W = new double[this.Depth];
            this.DW = new double[this.Depth];
            this.step_cache = new double[this.Depth];
            for (int i = 0; i < this.Depth; i++)
            {
                this.W[i] = sx[i];
            }
        }

        public Vol(int sx, int sy, int depth,bool normal=false)
        {
            this.SX = sx;
            this.SY = sy;
            this.Depth = depth;
            var n = sx * sy * depth;
            this.W = new double[n];
            this.DW = new double[n];
            this.step_cache = new double[n];

            var scale = Math.Sqrt(1.0 / (sx * sy * depth));
            if (normal)
            {
                scale = 0.08;
            }
            for (int i = 0; i < n; i++)
            {
                this.W[i] = Util.Randn(0.0, scale);  
            }

        }

        public Vol(int sx, int sy, int depth,double c)
        {
            this.SX = sx;
            this.SY = sy;
            this.Depth = depth;
            var n = sx * sy * depth;
            this.W = new double[n];
            this.DW = new double[n];

            this.step_cache = new double[n];
           // var scale = Math.Sqrt(1.0 / (sx * sy * depth));
            for (int i = 0; i < n; i++)
            {
                this.W[i] = c;
            }

        }

        public double Get(int x, int y, int d)
        {
            var ix = ((this.SX * y) + x) * this.Depth + d;
            return this.W[ix];
        }

        public void Set(int x, int y, int d,double v)
        {
            var ix = ((this.SX * y) + x) * this.Depth + d;
              this.W[ix]=v;
        }

        public void Add(int x, int y, int d, double v)
        {
            var ix = ((this.SX * y) + x) * this.Depth + d;
            this.W[ix] += v;
        }

        public double Get_Grad(int x, int y, int d)
        {
            var ix = ((this.SX * y) + x) * this.Depth + d;
            return this.DW[ix];
        }

        public void Set_Grad(int x, int y, int d, double v)
        {
            var ix = ((this.SX * y) + x) * this.Depth + d;
            this.DW[ix] = v;
        }

        public void Add_Grad(int x, int y, int d, double v)
        {
            var ix = ((this.SX * y) + x) * this.Depth + d;
            this.DW[ix] += v;
        }

        public Vol CloneAndZero()
        {
            return new Vol(this.SX, this.SY, this.Depth, 0.0);

        }

        public Vol Clone()
        {
            var v= new Vol(this.SX, this.SY, this.Depth, 0.0);
            var n = this.W.Length;
            for (int i = 0; i < n; i++)
            {
                v.W[i] = this.W[i];
            }
            return v;
        }

        public void AddFrom(Vol V)
        {
            for (int k = 0; k < this.W.Length; k++)
            {
                this.W[k] += V.W[k];
            }
        }

        public void AddFromScaled(Vol V,double a)
        {
            for (int k = 0; k < this.W.Length; k++)
            {
                this.W[k] +=a* V.W[k];
            }
        }

        public void SetConst(double a)
        {
            for (int k = 0; k < this.W.Length; k++)
            {
                this.W[k]  = a  ;
            }
        }

        public static Vol Augment(Vol V, int corp, int dx, int dy, bool fliplr)
        {
            //if (dx==0)
            //{
            //    dx = Util.Randi(0, V.SX - corp);
            //}
            //if (dy == 0)
            //{
            //    dy = Util.Randi(0, V.SY- corp);
            //}


            Vol W;
            if (corp!= V.SX || dx !=0 || dy!=0)
            {
                W = new Vol(corp, corp, V.Depth, 0.0);
                for (int x = 0; x < corp; x++)
                {
                    for (int y = 0; y < corp; y++)
                    {
                        if (x + dx < 0 || x + dx >= V.SX || y + dy < 0 || y + dy >= V.SY) continue;
                        for (int d = 0; d < V.Depth; d++)
                        {
                            W.Set(x, y, d, V.Get(x + dx, y + dy, d));
                        }
                    }
                }
            }
            else
            {
                W = V;
            }

            if (fliplr)
            {
                var W2 = W.CloneAndZero();

                for (int x = 0; x < W.SX; x++)
                {
                    for (int y = 0; y < W.SY; y++)
                    {
                        for (int d = 0; d < W.Depth; d++)
                        {
                            W2.Set(x, y, d, W.Get(W.SX - x - 1, y, d));

                        }
                    }
                }
                W = W2;
            }


            return W;
        }

        public static  Vol Augment(Vol V, int corp)
        {
            int dx; int dy; bool fliplr = false;

            dx = (int)Util.Randi(0, V.SX - corp);


            dy = (int)Util.Randi(0, V.SY - corp); 
            Vol W;
            if (corp != V.SX || dx != 0 || dy != 0)
            {
                W = new Vol(corp, corp, V.Depth, 0.0);
                for (int x = 0; x < corp; x++)
                {
                    for (int y = 0; y < corp; y++)
                    {
                        if (x + dx < 0 || x + dx >= V.SX || y + dy < 0 || y + dy >= V.SY) continue;
                        for (int d = 0; d < V.Depth; d++)
                        {
                            W.Set(x, y, d, V.Get(x + dx, y + dy, d));
                        }
                    }
                }
            }
            else
            {
                W = V;
            }

            if (fliplr)
            {
                var W2 = W.CloneAndZero();

                for (int x = 0; x < W.SX; x++)
                {
                    for (int y = 0; y < W.SY; y++)
                    {
                        for (int d = 0; d < W.Depth; d++)
                        {
                            W2.Set(x, y, d, W.Get(W.SX - x - 1, y, d));

                        }
                    }
                }
                W = W2;
            }


            return W;
        }

        public static  Vol Image_To_Vol(double[] img,int width,int hight, bool convert_grayscale)
        {
            var p = img;
            var w = width;
            var h = hight;
            double[] pv = new double[img.Length];
            for (var i = 0; i < p.Length; i++)
            {
                pv[i]=(p[i] / 255.0 - 0.5); // normalize image pixels to [-0.5, 0.5]
            }
            var x = new Vol(w, h, 4, 0.0);
            x.W = pv;

            if (convert_grayscale)
            {
                var x1 = new Vol(w, h, 1, 0.0);
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        x1.Set(i, j, 0, x.Get(i, j, 0));
                    }
                }
                x = x1;
            }


            return x;
        }
    }




        #region ToRemove


        public enum ConvType
        {
            Full, Same, Valid
        }

        public static class Matrix
        {
            public static double[][] Korn(double[][] A, double[][] B)
            {
                double[][] rotate = Matrix.MatrixCreate(A.Length * B.Length, A[0].Length * B[0].Length);
                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        var element = A[i][j];
                        var newInd = i * B.Length;
                        var newIndj = j * B[0].Length;
                        for (int ii = newInd; ii < B.Length + newInd; ii++)
                        {
                            for (int jj = newIndj; jj < B[0].Length + newIndj; jj++)
                            {
                                rotate[ii][jj] = A[i][j] * B[ii - newInd][jj - newIndj];
                            }
                        }

                    }
                }
                return rotate;
            }
            public static double[] Scale(this double[] arr, double min, double max)
            {
                double m = (max - min) / (arr.Max() - arr.Min());
                double c = min - arr.Min() * m;
                var newarr = new double[arr.Length];
                for (int i = 0; i < newarr.Length; i++)
                    newarr[i] = m * arr[i] + c;
                return newarr;
            }


            private static double[][] repmat(this double[] b1, int m)
            {
                double[][] r = new double[m][];
                for (int i = 0; i < m; i++)
                {
                    r[i] = b1;
                }


                return r.Transpose();
            }
            public static double[][] Diagonal(this double[] A)
            {

                double[][] means = new double[A.Length][];
                for (int i = 0; i < A.Length; i++)
                {

                    means[i] = new double[A.Length];

                    means[i][i] = A[i];


                }

                return means;

            }
            public static double[][] Reshape(this double[] A, int x, int y)
            {

                double[][] means = MatrixCreate(x, y);

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        means[i][j] = A[i * x + j];
                    }
                }

                return means;

            }
            public static double[] Diagonal(this double[][] A)
            {

                double[] means = new double[A.Length];
                for (int i = 0; i < A.Length; i++)
                {

                    means[i] = A[i][i];

                }

                return means;

            }

            public static double Max(this double[][] A)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i].Max();

                }
                return C.Max();

            }
            public static double[][] Sqrt(this double[][] A)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = Math.Sqrt(A[i][j]);
                    }
                }
                return C;

            }
            public static double[][] Abs(this double[][] A)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = Math.Abs(A[i][j]);
                    }
                }
                return C;

            }
            public static double[] Sqrt(this double[] A)
            {

                double[] C = new double[A.Length];
                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = Math.Sqrt(A[i]);

                }
                return C;

            }
            public static double Variance(this double[] A)
            {


                double average = A.Average();
                double sumOfSquaresOfDifferences = A.Select(val => (val - average) * (val - average)).Sum();
                double sd = Math.Sqrt(sumOfSquaresOfDifferences / A.Length);
                return sd * sd;

            }
            public static double[] Mean(this double[][] A)
            {

                double[] means = new double[A.Length];
                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        means[i] += A[i][j];
                    }
                }
                for (int i = 0; i < means.Length; i++)
                {
                    means[i] /= A.Length;
                }
                return means;

            }
            public static double[] Mean(this double[][] A, int dim)
            {
                if (dim == 0)
                {
                    return A.Mean();
                }
                else
                {

                    double[] means = new double[A[0].Length];
                    for (int i = 0; i < A.Length; i++)
                    {
                        for (int j = 0; j < A[0].Length; j++)
                        {
                            means[j] += A[i][j];
                        }
                    }
                    for (int i = 0; i < means.Length; i++)
                    {
                        means[i] /= A[0].Length;
                    }
                    return means;

                }
            }

            public static double[] ElementwisePower(this double[] A, int pow)
            {

                double[] res = new double[A.Length];
                for (int i = 0; i < A.Length; i++)
                {
                    res[i] = Math.Pow(A[i], pow);
                }
                return res;

            }

            public static double Mean(this double[] A)
            {


                double average = A.Average();
                return average;

            }
            public static double StandardDeviation(this double[] A)
            {


                double average = A.Average();
                double sumOfSquaresOfDifferences = A.Select(val => (val - average) * (val - average)).Sum();
                double sd = Math.Sqrt(sumOfSquaresOfDifferences / A.Length);
                return sd;

            }
            public static double StandardDeviation(this double[] A, double average)
            {
                double sumOfSquaresOfDifferences = A.Select(val => (val - average) * (val - average)).Sum();
                double sd = Math.Sqrt(sumOfSquaresOfDifferences / A.Length);
                return sd;

            }
            public static double[] Sum(this double[][] A, int c)
            {

                double[] C;
                if (c == 0)
                {
                    C = new double[A[0].Length];

                    for (int i = 0; i < A.Length; i++)
                    {
                        for (int j = 0; j < A[0].Length; j++)
                        {
                            C[j] += A[i][j];
                        }
                    }
                    return C;
                }
                else
                {

                    C = new double[A.Length];
                    for (int i = 0; i < A.Length; i++)
                    {
                        for (int j = 0; j < A[0].Length; j++)
                        {
                            C[i] += A[i][j];
                        }
                    }
                    return C;
                }


            }
            public static double[] Sum(this double[][] A)
            {

                double[] C;


                C = new double[A.Length];
                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i] += A[i][j];
                    }
                }
                return C;



            }
            public static double[][] Log(this double[][] A)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = Math.Log(A[i][j]);
                    }
                }
                return C;

            }
            public static double[][] Exp(this double[][] A)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = Math.Exp(A[i][j]);
                    }
                }
                return C;

            }
            public static double[][] Divide(this double c, double[][] A)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = c / A[i][j];
                    }
                }
                return C;

            }
            public static double[][] Subtract(this double c, double[][] A)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = c - A[i][j];
                    }
                }
                return C;

            }
            public static double[] Divide(this double c, double[] A)
            {
                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {
                    C[i] = c / A[i];

                }
                return C;

            }
            public static double[][] ElementwiseDivide(this double[][] A, double[] b, int dimension = 0, bool inPlace = false)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        if (dimension == 0)
                        {
                            C[i][j] = A[i][j] / b[i];

                        }
                        else
                        {

                            C[i][j] = A[i][j] / b[j];
                        }

                    }
                }

                return C;
            }
            public static double[][] ElementwiseMultiply(this double[][] A, double[][] c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] * c[i][j];
                    }
                }
                return C;

            }
            public static double[][] ElementwiseDivide(this double[][] A, double[][] c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] / c[i][j];
                    }
                }
                return C;

            }
            public static double[][] Add(this double[][] A, double c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] + c;
                    }
                }
                return C;

            }

            public static double[][] Divide(this double[][] A, double c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] / c;
                    }
                }
                return C;

            }
            public static double[][] Subtract(this double[][] A, double[][] c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] - c[i][j];
                    }
                }
                return C;

            }
            public static double[][] Subtract(this double[][] A, double c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] - c;
                    }
                }
                return C;

            }
            public static double[][] Subtract(this double[][] A, double[] c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);
                if (c.Length == A[0].Length)
                {
                    for (int i = 0; i < A.Length; i++)
                    {
                        for (int j = 0; j < A[0].Length; j++)
                        {
                            C[i][j] = A[i][j] - c[j];
                        }
                    }

                }
                else
                {

                    for (int i = 0; i < A.Length; i++)
                    {
                        for (int j = 0; j < A[0].Length; j++)
                        {
                            C[i][j] = A[i][j] - c[i];
                        }
                    }
                }
                return C;

            }
            public static double[][] Add(this double[][] A, double[] c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] + c[j];
                    }
                }
                return C;

            }
            public static double[] Subtract(this double[] A, double c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] - c;

                }
                return C;

            }
            public static double[][] Transpose(this double[][] A)
            {

                double[][] C = MatrixCreate(A[0].Length, A.Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[j][i] = A[i][j];
                    }
                }
                return C;

            }
            public static double[] Subtract(this double c, double[] A)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] - c;

                }
                return C;

            }
            public static double[] Subtract(this double[] A, double[] c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] - c[i];

                }
                return C;

            }
            public static double[] Add(this double[] A, double[] c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] + c[i];

                }
                return C;

            }
            public static double[] Add(this double[] A, double c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] + c;

                }
                return C;

            }
            public static double[] Divide(this double[] A, double c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] / c;

                }
                return C;

            }

            public static double[] ElementwiseDivide(this double[] A, double[] c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] / c[i];

                }
                return C;

            }

            public static double[][] Multiply(this double[][] A, double[][] B)
            {

                double[][] C = MatrixCreate(A.Length, B[0].Length);

                var source = Enumerable.Range(0, A.Length);
                var pquery = from num in source.AsParallel()
                             select num;
                pquery.ForAll((e) => MultiplyKernel(A, B, C, e));
                return C;

            }
            public static double[][] Multiply(this double[][] A, double c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] * c;
                    }
                }
                return C;

            }
            public static double[][] Multiply(this double c, double[][] A)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] * c;
                    }
                }
                return C;
            }
            public static double[] Multiply(this double c, double[] A)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] * c;

                }
                return C;

            }
            public static double[][] Pow(this double[][] A, int d)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = Math.Pow(A[i][j], d);
                    }
                }
                return C;

            }
            public static double[] Pow(this double[] A, int d)
            {

                double[] C = new double[A.Length];
                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = Math.Pow(A[i], d);

                }
                return C;

            }
            public static double[] Multiply(this double[] A, double c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i] * c;

                }
                return C;

            }
            static void MultiplyKernel(double[][] A, double[][] B, double[][] C, int i)
            {
                double[] iRowA = A[i];
                double[] iRowC = C[i];



                for (int k = 0; k < A[0].Length; k++)
                {

                    double[] kRowB = B[k];
                    double ikA = iRowA[k];
                    for (int j = 0; j < B[0].Length; j++)
                    {
                        iRowC[j] += ikA * kRowB[j];
                    }
                }
            }

            public static double[][] Conv2(this double[][] inputSignal, double[][] kernel, ConvType type = ConvType.Full)
            {

                int kCols = kernel[0].Length;
                int kRows = kernel.Length;
                int kCenterX = 0;
                int kCenterY = 0;

                int rows = 0; int cols = 0;

                switch (type)
                {
                    case ConvType.Full:
                        var colst = Math.Max(inputSignal[0].Length + kernel[0].Length - 1, inputSignal[0].Length);
                        cols = Math.Max(colst, kernel[0].Length);


                        var rowst = Math.Max(inputSignal.Length + kernel.Length - 1, inputSignal.Length);
                        rows = Math.Max(rowst, kernel.Length);
                        kCenterX = inputSignal[0].Length - kernel[0].Length;
                        kCenterY = inputSignal.Length - kernel.Length;
                        break;
                    case ConvType.Same:
                        cols = inputSignal[0].Length;
                        rows = inputSignal.Length;
                        kCenterX = kCols / 2;
                        kCenterY = kRows / 2;
                        break;
                    case ConvType.Valid:
                        cols = inputSignal[0].Length - Math.Max(kernel[0].Length - 1, 0);
                        rows = inputSignal.Length - Math.Max(kernel.Length - 1, 0);


                        kCenterX = 0;
                        kCenterY = 0;


                        break;
                    default:
                        break;
                }
                double[][] y = Matrix.MatrixCreate(rows, cols);


                var source = Enumerable.Range(0, rows);
                var pquery = from num in source.AsParallel()
                             select num;
                pquery.ForAll((e) => Conv2Kernel(inputSignal, kernel, y, e, cols, rows, kCols, kRows, kCenterX, kCenterY, type));
                return y;

            }
            static void Conv2Kernel(double[][] inputSignal, double[][] kernel, double[][] y, int i, int cols, int rows,
                int kCols, int kRows, int kCenterX, int kCenterY, ConvType type)
            {
                int ti = i;
                for (int j = 0; j < cols; ++j)          // columns
                {
                    int tj = j;
                    double sum = 0;
                    for (int m = 0; m < kRows; ++m)     // kernel rows
                    {
                        int mm = kRows - 1 - m;      // row index of flipped kernel 
                        for (int n = 0; n < kCols; ++n) // kernel columns
                        {
                            int nn = kCols - 1 - n;  // column index of flipped kernel 
                            // index of input signal, used for checking boundary
                            int ii = i + (m - kCenterY);//+ m;
                            int jj = j + (n - kCenterX);//+ n;
                            // ignore input samples which are out of bound

                            if (ii >= 0 && ii < inputSignal.Length && jj >= 0 && jj < inputSignal[ii].Length)
                            {

                                sum += inputSignal[ii][jj] * kernel[mm][nn];



                            }


                        }
                    }

                    y[i][j] = sum;
                }
            }

            public static double[][] Add(this double[][] A, double[][] c)
            {

                double[][] C = MatrixCreate(A.Length, A[0].Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        C[i][j] = A[i][j] + c[i][j];
                    }
                }
                return C;

            }

            private static Random rand = new Random(); //reuse this if you are generating many

            public static double[][] RandomN(int rows, int cols, double min, double max)
            {
                // do error checking here
                double[][] result = new double[rows][];
                for (int i = 0; i < rows; ++i)
                {
                    result[i] = new double[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        double mean = 0;
                        double stdDev = Math.Pow(10, -4);
                        double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
                        double u2 = rand.NextDouble();
                        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                        double randNormal =
                                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)


                        result[i][j] = randNormal;
                    }
                }
                return result;
            }

            public static double[][] MatrixCreate(int rows, int cols)
            {
                // do error checking here
                double[][] result = new double[rows][];
                for (int i = 0; i < rows; ++i)
                    result[i] = new double[cols];
                return result;
            }
            static Random rng = new Random(3);
            public static double[][] Random(int rows, int cols, double min, double max)
            {
                // do error checking here
                double[][] result = new double[rows][];
                for (int i = 0; i < rows; ++i)
                {
                    result[i] = new double[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        result[i][j] = min + (rng.NextDouble() * (max - min));
                    }
                }
                return result;
            }

            public static double Norm2(this double[][] A)
            {
                double norm = 0.0;
                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < A[0].Length; j++)
                    {
                        norm += A[i][j] * A[i][j];
                    }
                }
                return (double)Math.Sqrt(norm);

            }

            public static double Norm2(this double[] A)
            {
                double norm = 0.0;
                for (int i = 0; i < A.Length; i++)
                {

                    norm += A[i] * A[i];

                }
                return (double)Math.Sqrt(norm);

            }


            public static double[][] GetColumns(this double[][] A, int[] c)
            {

                double[][] C = MatrixCreate(A.Length, c.Length);

                for (int i = 0; i < A.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        C[i][j] = A[i][c[j]];
                    }
                }
                return C;

            }



            public static double[] GetColumn(this double[][] A, int c)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < A.Length; i++)
                {

                    C[i] = A[i][c];

                }
                return C;

            }

            public static double[] GetRow(this double[][] A, int c)
            {

                double[] C = new double[A[0].Length];

                for (int i = 0; i < C.Length; i++)
                {

                    C[i] = A[c][i];

                }
                return C;

            }
            public static double[] Abs(this double[] A)
            {

                double[] C = new double[A.Length];

                for (int i = 0; i < C.Length; i++)
                {

                    C[i] = Math.Abs(A[i]);

                }
                return C;

            }


            public static double[] Ones(int n)
            {
                double[] res = new double[n];
                for (int i = 0; i < n; i++)
                {
                    res[i] = 1;
                }
                return res;
            }
            public static double[][] Ones(int n, int m)
            {
                double[][] res = MatrixCreate(n, m);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        res[i][j] = 1;
                    }
                }
                return res;
            }


            public static double[] Zeros(int n)
            {
                double[] res = new double[n];
                for (int i = 0; i < n; i++)
                {
                    res[i] = 0;
                }
                return res;
            }
            public static double[][] Zeros(int n, int m)
            {
                double[][] res = MatrixCreate(n, m);
                //for (int i = 0; i < n; i++)
                //{
                //    for (int j = 0; j < m; j++)
                //    {
                //        res[i][j] = 0;
                //    }
                //}
                return res;
            }



            public static double Get(this double[][] A, int x, int y)
            {
                return A[x][y];
            }

            public static void Set(this double[][] A, int x, int y, double val)
            {
                A[x][y] = val;
            }



            public static double[][] Rot90(this double[][] start)
            {
                double[][] rotate = Matrix.MatrixCreate(start[0].Length, start.Length);
                for (int i = 0; i < start.Length; i++)
                {
                    int ith = start[0].Length - 1;
                    for (int j = 0; j < start[0].Length; j++)
                    {
                        rotate[ith][i] = start[i][j];
                        ith--;
                    }
                }
                return rotate;
            }
            public static double[][] Rot90(this double[][] start, int num)
            {
                double[][] res = start;
                for (int i = 0; i < num; i++)
                {
                    res = Rot90(res);
                }
                return res;
            }






        }
        #endregion

    }
