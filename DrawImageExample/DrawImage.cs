using ConvNetCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawImageExample
{
    public partial class DrawImage : Form
    {
        delegate void DrawImageFunction();
        
        public DrawImage()
        {
            InitializeComponent();
        }
        
        Bitmap CurrentImage = null;
        
        double[] image = null;
       
        private void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            CurrentImage = new Bitmap(ofd.FileName);
            pictureBox1.Image = CurrentImage;

            image = new double[CurrentImage.Width * CurrentImage.Height * 3];
            int c=0;
            for (int i = 0; i < CurrentImage.Width; i++)
            {
                for (int j = 0; j < CurrentImage.Height; j++)
                {
                    var pix = CurrentImage.GetPixel(i, j);

                    image[c] = Convert.ToDouble(pix.R);
                    c++;
                    image[c] = Convert.ToDouble(pix.G) ;
                    c++;
                    image[c] = Convert.ToDouble(pix.B) ;
                    c++;

                }
            }
            StartButton.Enabled = true;
           // image.Scale(0, 1);
        }
        Thread MainThread;
        private void StartButton_Click(object sender, EventArgs e)
        {
            LoadButton.Enabled = false;
            StopButton.Enabled = true;
            StartButton.Enabled = false;
            MainThread=  new Thread(new ThreadStart(TrainThread));
          MainThread.Start();
        }


        private void TrainThread()
        {
            var net = CreateNewNetwork();


            Trainer trainer = new Trainer();
            trainer.Net = net;
            trainer.method = TrainingMethod.sgd;
            trainer.batch_size = 5;
            trainer.l2_decay = 0.0;
            trainer.momentum = 0.9;
            trainer.learning_rate = 0.01;
            for (int i = 0; i < 10000; i++)
            {

                LearnStep(trainer);
                counter++;

                var W =Convert.ToDouble( CurrentImage.Width);
                var H = Convert.ToDouble( CurrentImage.Height) ;

                Bitmap outputImage = new Bitmap(CurrentImage.Width, CurrentImage.Height);


                var v = new Vol(1, 1, 2, 0);
                for (var x = 0; x < W; x++)
                {
                    v.W[0] = (x - (W / 2.0)) / W;
                    for (var y = 0; y < H; y++)
                    {
                        v.W[1] = (y - (H / 2.0)) / H;

                        var ix = ((W * y) + x) * 3;
                        var r = net.Forward(v, false);
                        var red = Math.Floor(255.0 * r.W[0]);
                        var green = Math.Floor(255.0 * r.W[1]);
                        var blue = Math.Floor(255.0 * r.W[2]);

                        if (red > 255)
                        {
                            red = 255;
                        }
                        if (green > 255)
                        {
                            green = 255;
                        }
                        if (blue > 255)
                        {
                            blue = 255;
                        }
                        if (red < 0)
                        {
                            red = 0;
                        }
                        if (green < 0)
                        {
                            green = 0;
                        }
                        if (blue < 0)
                        {
                            blue = 0;
                        }
                        Color color = Color.FromArgb(255, (int)red, (int)green, (int)blue);

                        outputImage.SetPixel(x, y, color);

                    }
                }
                this.Invoke(new DrawImageFunction(() =>
                {
                    outputImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                    pictureBox2.Image = outputImage;



                }));
            }

        }
        
        int counter = 0;
        private void LearnStep(Trainer trainer)
        {

            var batches_per_iteration = 100;
            var mod_skip_draw = 100;
            var smooth_loss = -1.0;
            var p = image;
            var W = Convert.ToDouble( CurrentImage.Width);
            var H = Convert.ToDouble( CurrentImage.Height);
            var loss = 0.0;
            var lossi = 0.0;
            var N = batches_per_iteration;

            Random random = new Random();
            var v = new Vol(1, 1, 2, 0);

            for (var iters = 0; iters < trainer.batch_size; iters++)
            {
                for (var i = 0; i < N; i++)
                {
                    // sample a coordinate
                    var x = random.Next(0, CurrentImage.Width-1);
                    var y = random.Next(0, CurrentImage.Height-1);
                    var ix = (int)((W * y) + x) * 3;


                    double[] r = new double[] { p[ix] / 255.0, p[ix + 1] / 255.0, p[ix + 2] / 255.0 }; // r g b



                    v.W[0] = (x - W / 2) / W;
                    v.W[1] = (y - H / 2) / H;
                    var stats = trainer.Train(v, r);
                    loss += stats.loss;
                    lossi += 1;
                }
            }
         //   label1.Text = loss.ToString();
            loss /= lossi;
            if (counter == 0) smooth_loss = loss;
            else smooth_loss = 0.99 * smooth_loss + 0.01 * loss;
            this.Invoke(new DrawImageFunction(() =>
            {

                ItLabel.Text = counter.ToString();
                LossLabel.Text = smooth_loss.ToString();


            }));
        }




        private static Network CreateNewNetwork()
        {

            Network net = new Network();

            InputLayer il = new InputLayer();
            il.OutputWidth = 1;
            il.OutputHeight = 1;
            il.OutputDepth = 2;
            net.Layers.Add(il);



            FullyConnLayer fc = new FullyConnLayer(50, il.OutputDepth, il.OutputWidth, il.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc);

            ReluLayer rl = new ReluLayer(fc.OutputDepth, fc.OutputWidth, fc.OutputHeight);
            net.Layers.Add(rl);




            FullyConnLayer fc2 = new FullyConnLayer(50, rl.OutputDepth, rl.OutputWidth, rl.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc2);

            ReluLayer rl2 = new ReluLayer(fc2.OutputDepth, fc2.OutputWidth, fc2.OutputHeight);
            net.Layers.Add(rl2);


             

            FullyConnLayer fc8 = new FullyConnLayer(3, rl2.OutputDepth, rl2.OutputWidth, rl2.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc8);

            RegressionLayer sl = new RegressionLayer(fc8.OutputDepth, fc8.OutputWidth, fc8.OutputHeight);
            net.LossLayer = sl;
            return net;
        }

        class TrainThreadParams
        {
            public Network net;
                public Trainer trainer;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            LoadButton.Enabled = true;
            StopButton.Enabled = false;
            MainThread.Abort();
        }

        private void DrawImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MainThread!=null)
            {
                MainThread.Abort();
            }
        }

      
    }
}
