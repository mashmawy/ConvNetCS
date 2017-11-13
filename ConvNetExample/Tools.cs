using ConvNetCS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetExample
{
    public static class Tools
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
            if (!File.Exists("data_batch_1.bin"))
            {
                Console.WriteLine("Kindly Download the dataset first : https://www.cs.toronto.edu/~kriz/cifar.html");
                throw new FileNotFoundException("Kindly Download the dataset first : https://www.cs.toronto.edu/~kriz/cifar.html");
            }
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
            if (!File.Exists("test_batch.bin"))
            {
                Console.WriteLine("Kindly Download the testset first : https://www.cs.toronto.edu/~kriz/cifar.html");
                throw new FileNotFoundException("Kindly Download the testset first : https://www.cs.toronto.edu/~kriz/cifar.html");
            }
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

        public static Network CreateNewNetwork()
        {

            Network net = new Network();

            InputLayer il = new InputLayer();
            il.OutputWidth = 32;
            il.OutputHeight = 32;
            il.OutputDepth = 3;
            net.Layers.Add(il);

            ConvLayer conv = new ConvLayer(16, 3, 3, 3, 32, 32, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv);

            ReluLayer rl = new ReluLayer(conv.OutputDepth, conv.OutputWidth, conv.OutputHeight);
            net.Layers.Add(rl);
              
            ConvLayer conv2 = new ConvLayer(20, 3, 3, rl.OutputDepth, rl.OutputWidth, rl.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv2);

            ReluLayer rl2 = new ReluLayer(conv2.OutputDepth, conv2.OutputWidth, conv2.OutputHeight);
            net.Layers.Add(rl2);

            MaxPoolLayer pl2 = new MaxPoolLayer(2, 2, rl2.OutputDepth, rl2.OutputWidth, rl2.OutputHeight, 2, 0);
            net.Layers.Add(pl2);


            ConvLayer conv3 = new ConvLayer(20, 3, 3, pl2.OutputDepth, pl2.OutputWidth, pl2.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv3);

            ReluLayer rl3 = new ReluLayer(conv3.OutputDepth, conv3.OutputWidth, conv3.OutputHeight);
            net.Layers.Add(rl3);

            ConvLayer conv4 = new ConvLayer(20, 3, 3, rl3.OutputDepth, rl3.OutputWidth, rl3.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv4);

            ReluLayer rl4 = new ReluLayer(conv4.OutputDepth, conv4.OutputWidth, conv4.OutputHeight);
            net.Layers.Add(rl4);

            MaxPoolLayer pl3 = new MaxPoolLayer(2, 2, rl4.OutputDepth, rl4.OutputWidth, rl4.OutputHeight, 2, 0);
            net.Layers.Add(pl3);

            FullyConnLayer fc = new FullyConnLayer(256, pl3.OutputDepth, pl3.OutputWidth, pl3.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc);


            FullyConnLayer fc2 = new FullyConnLayer(10, fc.OutputDepth, fc.OutputWidth, fc.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc2);

            SoftmaxLayer sl = new SoftmaxLayer(fc2.OutputDepth, fc2.OutputWidth, fc2.OutputHeight);
            net.LossLayer = sl;
            return net;

        }
        public static Random randpermRand = new Random(1);
        public static int[] randperm(int count)
        {
            int[] res = new int[count];

            for (int i = 0; i < count; i++)
            {
                res[i] = randpermRand.Next(0, count - 1);

            }

            return res;
        }

    }
}
