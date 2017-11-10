using ConvNetCS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetExample
{
    class Program
    {
        public static float[][] MatrixCreate(int rows, int cols)
        {
            // do error checking here
            float[][] result = new float[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new float[cols];
            return result;
        }
        public static Tuple<float[][], float[]> cifarTrainingData(int trainigCount = 50000)
        {

            float[][] X = MatrixCreate(trainigCount, 3072);
            float[] Y = new float[trainigCount];

            byte[] samples = new byte[30730000];
            using (FileStream fileStreamTrainPatterns = System.IO.File.Open("data_batch_1.bin",
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStreamTrainPatterns.Read(samples, 0, 30730000);
            }
            for (int index = 0; index < 10000; index++)
            {
                Y[index] = samples[3073 * index];
                float[] image = new float[3072];
                for (int i = 0; i < 3072; i++)
                {
                    image[i] = samples[((3073 * index) + 1) + i];
                }

                X[index] = image;
            }


            samples = new byte[30730000];
            using (FileStream fileStreamTrainPatterns = System.IO.File.Open("data_batch_2.bin",
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStreamTrainPatterns.Read(samples, 0, 30730000);
            }
            for (int index = 0; index < 10000; index++)
            {
                Y[index + 10000] = samples[3073 * index];
                float[] image = new float[3072];
                for (int i = 0; i < 3072; i++)
                {
                    image[i] = samples[((3073 * index) + 1) + i];
                }

                X[index + 10000] = image;
            }


            samples = new byte[30730000];
            using (FileStream fileStreamTrainPatterns = System.IO.File.Open("data_batch_3.bin",
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStreamTrainPatterns.Read(samples, 0, 30730000);
            }
            for (int index = 0; index < 10000; index++)
            {
                Y[index + 20000] = samples[3073 * index];
                float[] image = new float[3072];
                for (int i = 0; i < 3072; i++)
                {
                    image[i] = samples[((3073 * index) + 1) + i];
                }

                X[index + 20000] = image;
            }


            samples = new byte[30730000];
            using (FileStream fileStreamTrainPatterns = System.IO.File.Open("data_batch_4.bin",
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStreamTrainPatterns.Read(samples, 0, 30730000);
            }
            for (int index = 0; index < 10000; index++)
            {
                Y[index + 30000] = samples[3073 * index];
                float[] image = new float[3072];
                for (int i = 0; i < 3072; i++)
                {
                    image[i] = samples[((3073 * index) + 1) + i];
                }

                X[index + 30000] = image;
            }



            samples = new byte[30730000];
            using (FileStream fileStreamTrainPatterns = System.IO.File.Open("data_batch_5.bin",
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStreamTrainPatterns.Read(samples, 0, 30730000);
            }
            for (int index = 0; index < 10000; index++)
            {
                Y[index + 40000] = samples[3073 * index];
                float[] image = new float[3072];
                for (int i = 0; i < 3072; i++)
                {
                    image[i] = samples[((3073 * index) + 1) + i];
                }

                X[index + 40000] = image;
            }
            return new Tuple<float[][], float[]>(X, Y);

        }
        public static Tuple<float[][], float[]> cifarTestData(int trainigCount = 10000)
        {

            float[][] X = MatrixCreate(trainigCount, 3072);
            float[] Y = new float[trainigCount];

            byte[] samples = new byte[30730000];
            using (FileStream fileStreamTrainPatterns = System.IO.File.Open("test_batch.bin",
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStreamTrainPatterns.Read(samples, 0, 30730000);
            }
            for (int index = 0; index < 10000; index++)
            {
                Y[index] = samples[3073 * index];
                float[] image = new float[3072];
                for (int i = 0; i < 3072; i++)
                {
                    image[i] = samples[((3073 * index) + 1) + i];
                }

                X[index] = image;
            }


            return new Tuple<float[][], float[]>(X, Y);

        }

        private static Network CreateNewNetwork()
        {

            Network net = new Network();

            InputLayer il = new InputLayer();
            il.OutputWidth = 32;
            il.OutputHeight = 32;
            il.OutputDepth = 3;
            net.Layers.Add(il);

            ConvLayer conv = new ConvLayer(16, 5, 5, 3, 32, 32, 1, 2, 0, 1, 0.1f);
            net.Layers.Add(conv);

            ReluLayer rl = new ReluLayer(conv.OutputDepth, conv.OutputWidth, conv.OutputHeight);
            net.Layers.Add(rl);

            MaxPoolLayer pl = new MaxPoolLayer(2, 2, rl.OutputDepth, rl.OutputWidth, rl.OutputHeight, 2, 0);
            net.Layers.Add(pl);


            ConvLayer conv2 = new ConvLayer(20, 5, 5, pl.OutputDepth, pl.OutputWidth, pl.OutputHeight, 1, 2, 0, 1, 0.1f);
            net.Layers.Add(conv2);

            ReluLayer rl2 = new ReluLayer(conv2.OutputDepth, conv2.OutputWidth, conv2.OutputHeight);
            net.Layers.Add(rl2);

            MaxPoolLayer pl2 = new MaxPoolLayer(2, 2, rl2.OutputDepth, rl2.OutputWidth, rl2.OutputHeight, 2, 0);
            net.Layers.Add(pl2);


            ConvLayer conv3 = new ConvLayer(20, 5, 5, pl2.OutputDepth, pl2.OutputWidth, pl2.OutputHeight, 1, 2, 0, 1, 0.1f);
            net.Layers.Add(conv3);

            ReluLayer rl3 = new ReluLayer(conv3.OutputDepth, conv3.OutputWidth, conv3.OutputHeight);
            net.Layers.Add(rl3);

            MaxPoolLayer pl3 = new MaxPoolLayer(2, 2, rl3.OutputDepth, rl3.OutputWidth, rl3.OutputHeight, 2, 0);
            net.Layers.Add(pl3);

            FullyConnLayer fc = new FullyConnLayer(10, pl3.OutputDepth, pl3.OutputWidth, pl3.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc);

            SoftmaxLayer sl = new SoftmaxLayer(fc.OutputDepth, fc.OutputWidth, fc.OutputHeight);
            net.LossLayer = sl;
            return net;

        }


        private static Random randpermRand = new Random(1);
        private static int[] randperm(int count)
        {
            int[] res = new int[count];

            for (int i = 0; i < count; i++)
            {
                res[i] = randpermRand.Next(0, count - 1);

            }

            return res;
        }

        private static void Train()
        {
            var load = cifarTrainingData();
            var dd = new DataToSave() { Labels = load.Item2, Data = load.Item1 };

            string[] classes_txt = { "airplane", "car", "bird", "cat", "deer", "dog", "frog", "horse", "ship", "truck" };

            var image_dimension = 32;
            var image_channels = 3;
            var random_flip = true;
            var random_position = true;

            Network net = CreateNewNetwork();
            Trainer trainer = new Trainer();
            trainer.Net = net;
            trainer.method = TrainingMethod.adagrad;
            trainer.batch_size = 200;
            trainer.l2_decay = 0.0001f;
            trainer.momentum = 0.9f;
            trainer.learning_rate = 0.01f;
            Random random = new Random();
            var data = dd.Data;
            var rp = randperm(data.Length);
            float[] norm = new float[] {123.68f, 116.779f,103.939f};
            for (int ep = 0; ep < 40; ep++)
            {
                int all = 0;
                int good = 0;
                for (int we = 0; we < data.Length; we++)
                {
                    int i = rp[we];
                    var y = (int)dd.Labels[i];

                    var p = data[i];
                    var x = new Vol(32, 32, 3, 0.0f);
                    var W = image_dimension * image_dimension;
                    
                    var ii = 0;
                     

                    for (var dc = 0; dc < image_channels; dc++)
                    {
                        for (var xc = 0; xc < image_dimension; xc++)
                        {
                            for (var yc = 0; yc < image_dimension; yc++)
                            {
                                x.Set(yc, xc, dc, p[ii] - norm[dc]);
                                ii++;
                            }
                        }
                    }

                    if (random_position)
                    {
                        var dx = Math.Floor(random.NextDouble() * 5 - 2);
                        var dy = Math.Floor(random.NextDouble() * 5 - 2);
                        x = Vol.Augment(x, image_dimension, (int)dx, (int)dy, false); //maybe change position
                    }

                    if (random_flip)
                    {
                        x = Vol.Augment(x, image_dimension, 0, 0, random.NextDouble() < 0.5); //maybe flip horizontally
                    }

                    // train on it with network
                    var stats = trainer.Train(x, y);
                    var lossx = stats.cost_loss;
                    var lossw = stats.l2_decay_loss;

                    // keep track of stats such as the average training error and loss
                    var yhat = net.GetPrediction();
                    var train_acc = yhat == y ? 1 : 0;
                    all++;
                    good += train_acc;

                    var acc = (float)good * 100.0 / (float)all;
                    Console.WriteLine("It :{2}     ,  Loss :{0}   , Acc:{1}", lossx, acc, we);

                }
                FileStream fs2 = new FileStream("network.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter bf2 = new BinaryFormatter();
                bf2.Serialize(fs2, net);
                fs2.Close();
            }

            Console.Read();
        }

        private static void Test()
        {

            var load = cifarTestData();
            var dd = new DataToSave() { Labels = load.Item2, Data = load.Item1 };

            string[] classes_txt = { "airplane", "car", "bird", "cat", "deer", "dog", "frog", "horse", "ship", "truck" };

            var image_dimension = 32;
            var image_channels = 3;
            var random_flip = true;
            var random_position = true;

            FileStream fs = new FileStream("network.dat", FileMode.Open, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            var net = bf.Deserialize(fs) as Network;
            fs.Close();
            Trainer trainer = new Trainer();
            trainer.Net = net;
            trainer.method = TrainingMethod.adadelta;
            trainer.batch_size = 4;
            trainer.l2_decay = 0;
            trainer.momentum = 0.99f;
            trainer.learning_rate = 0.01f;
            Random random = new Random();
            var data = dd.Data;
            var rp = randperm(data.Length);

            int all = 0;
            int good = 0;
            for (int we = 0; we < data.Length; we++)
            {
                int i = we;
                var y = (int)dd.Labels[i];

                var p = data[i];
                var x = new Vol(32, 32, 3, 0.0f);
                var W = image_dimension * image_dimension; 
                var ii = 0;
                for (var dc = 0; dc < image_channels; dc++)
                {
                    for (var xc = 0; xc < image_dimension; xc++)
                    {
                        for (var yc = 0; yc < image_dimension; yc++)
                        {
                            x.Set(yc, xc, dc, p[ii] / 255.0f - 0.5f);//  ;
                            ii++;
                        }
                    }
                }

                if (random_position)
                {
                    var dx = Math.Floor(random.NextDouble() * 5 - 2);
                    var dy = Math.Floor(random.NextDouble() * 5 - 2);
                    x = Vol.Augment(x, image_dimension, (int)dx, (int)dy, false); //maybe change position
                }

                if (random_flip)
                {
                    x = Vol.Augment(x, image_dimension, 0, 0, random.NextDouble() < 0.5); //maybe flip horizontally
                }

                // train on it with network
                var stats = net.Forward(x, false);


                // keep track of stats such as the average training error and loss
                var yhat = net.GetPrediction();
                var train_acc = yhat == y ? 1 : 0;
                all++;
                good += train_acc;

                var acc = (float)good * 100.0 / (float)all;
                Console.WriteLine("It :{2}     ,  Loss :{0}   , Acc:{1}", 0, acc, we);


            }

            Console.Read();
        }
         
        static void Main(string[] args)
        {

            Train();



          
        }
 
         
    }
}

