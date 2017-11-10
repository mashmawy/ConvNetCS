using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    public class Window
    {
        public Queue<float> V { get; set; }
        public int Size { get; set; }
        public int minsize { get; set; }
        public float sum { get; set; }
        public Window()
        {
            this.V = new Queue<float>();
            this.Size = 100;
            this.minsize = 20;
            this.sum = 0;
        }
        public Window(int size, int minsize)
        {
            this.V = new Queue<float>();
            this.Size = size;
            this.minsize = minsize;
            this.sum = 0;
        }


        public void Add(float x)
        {
            this.V.Enqueue(x);
            sum += x;
            if (this.V.Count > this.Size)
            {
                var xold = this.V.Dequeue();
                this.sum -= xold;
            }
        }
        public float get_average()
        {
            if (this.V.Count < this.minsize) return -1;
            else return this.sum / (float)this.V.Count;
        }

        public void reset()
        {
            this.V = new Queue<float>(); 
            this.sum = 0;
        }
    }
}
