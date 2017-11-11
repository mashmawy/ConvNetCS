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

        private static void Train()
        {
            var load = Tools.cifarTrainingData();
            var dd = new DataToSave() { Labels = load.Item2, Data = load.Item1 };

            string[] classes_txt = { "airplane", "car", "bird", "cat", "deer", "dog", "frog", "horse", "ship", "truck" };

            var image_dimension = 32;
            var image_channels = 3;
            var random_flip = true;
            var random_position = true;

            Network net = Tools.CreateNewNetwork();
            Trainer trainer = new Trainer();
            trainer.Net = net;
            trainer.method = TrainingMethod.adagrad;
            trainer.batch_size = 200;
            trainer.l2_decay = 0.0001f;
            trainer.momentum = 0.9f;
            trainer.learning_rate = 0.001f;
            Random random = new Random();
            var data = dd.Data;
            var rp = Tools.randperm(data.Length);
            float[] norm = new float[] { 123.68f, 116.779f, 103.939f };
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

            var load = Tools.cifarTestData();
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
            var rp = Tools.randperm(data.Length);

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

