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
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public float[] W { get; set; }
        [NonSerialized]
        public float[] DW;
        [NonSerialized]
        public float[] step_cache;
         
        public Vol(float[] sx)
        {
            this.Width = 1;
            this.Height = 1;
            this.Depth = sx.Length;
            this.W = new float[this.Depth];
            this.DW = new float[this.Depth];
            this.step_cache = new float[this.Depth];
            for (int i = 0; i < this.Depth; i++)
            {
                this.W[i] = sx[i];
            }
        }

      
        public Vol(int sx, int sy, int depth,bool normal=false)
        {
            this.Width = sx;
            this.Height = sy;
            this.Depth = depth;
            var n = sx * sy * depth;
            this.W = new float[n];
            this.DW = new float[n];
            this.step_cache = new float[n];

            var scale = (float)Math.Sqrt(1.0 / (sx * sy * depth));
            if (normal)
            {
                scale = 0.08f;
            }
            for (int i = 0; i < n; i++)
            {
                this.W[i] = Util.Randn(0.0f, scale);  
            }

        }

        public Vol(int sx, int sy, int depth,float c)
        {
            this.Width = sx;
            this.Height = sy;
            this.Depth = depth;
            var n = sx * sy * depth;
            this.W = new float[n];
            this.DW = new float[n];

            this.step_cache = new float[n]; 
            for (int i = 0; i < n; i++)
            {
                this.W[i] = c;
            }

        }

        public float Get(int x, int y, int d)
        {
            var ix = ((this.Width * y) + x) * this.Depth + d;
            return this.W[ix];
        }

        public void Set(int x, int y, int d,float v)
        {
            var ix = ((this.Width * y) + x) * this.Depth + d;
              this.W[ix]=v;
        }

        public void Add(int x, int y, int d, float v)
        {
            var ix = ((this.Width * y) + x) * this.Depth + d;
            this.W[ix] += v;
        }

        public float Get_Grad(int x, int y, int d)
        {
            var ix = ((this.Width * y) + x) * this.Depth + d;
            return this.DW[ix];
        }

        public void Set_Grad(int x, int y, int d, float v)
        {
            var ix = ((this.Width * y) + x) * this.Depth + d;
            this.DW[ix] = v;
        }

        public void Add_Grad(int x, int y, int d, float v)
        {
            var ix = ((this.Width * y) + x) * this.Depth + d;
            this.DW[ix] += v;
        }

        public Vol CloneAndZero()
        {
            return new Vol(this.Width, this.Height, this.Depth, 0.0f);

        }

        public Vol Clone()
        {
            var v= new Vol(this.Width, this.Height, this.Depth, 0.0f);
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

        public void AddFromScaled(Vol V,float a)
        {
            for (int k = 0; k < this.W.Length; k++)
            {
                this.W[k] +=a* V.W[k];
            }
        }

        public void SetConst(float a)
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
            if (corp!= V.Width || dx !=0 || dy!=0)
            {
                W = new Vol(corp, corp, V.Depth, 0.0f);
                for (int x = 0; x < corp; x++)
                {
                    for (int y = 0; y < corp; y++)
                    {
                        if (x + dx < 0 || x + dx >= V.Width || y + dy < 0 || y + dy >= V.Height) continue;
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

                for (int x = 0; x < W.Width; x++)
                {
                    for (int y = 0; y < W.Height; y++)
                    {
                        for (int d = 0; d < W.Depth; d++)
                        {
                            W2.Set(x, y, d, W.Get(W.Width - x - 1, y, d));

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

            dx = (int)Util.Randi(0, V.Width - corp);


            dy = (int)Util.Randi(0, V.Height - corp); 
            Vol W;
            if (corp != V.Width || dx != 0 || dy != 0)
            {
                W = new Vol(corp, corp, V.Depth, 0.0f);
                for (int x = 0; x < corp; x++)
                {
                    for (int y = 0; y < corp; y++)
                    {
                        if (x + dx < 0 || x + dx >= V.Width || y + dy < 0 || y + dy >= V.Height) continue;
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

                for (int x = 0; x < W.Width; x++)
                {
                    for (int y = 0; y < W.Height; y++)
                    {
                        for (int d = 0; d < W.Depth; d++)
                        {
                            W2.Set(x, y, d, W.Get(W.Width - x - 1, y, d));

                        }
                    }
                }
                W = W2;
            }


            return W;
        }

        public static  Vol Image_To_Vol(float[] img,int width,int hight, bool convert_grayscale)
        {
            var p = img;
            var w = width;
            var h = hight;
            float[] pv = new float[img.Length];
            for (var i = 0; i < p.Length; i++)
            {
                pv[i]=(p[i] / 255.0f - 0.5f); // normalize image pixels to [-0.5, 0.5]
            }
            var x = new Vol(w, h, 4, 0.0f);
            x.W = pv;

            if (convert_grayscale)
            {
                var x1 = new Vol(w, h, 1, 0.0f);
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



     
    }
