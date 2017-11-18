using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS.Models
{
    public static class VGG16
    {
         
        public static Network CreateVGG16Network(int imageWidth, int imageHeight, int LabelsCount)
        {

            Network net = new Network();

            InputLayer il = new InputLayer();
            il.OutputWidth = imageWidth;
            il.OutputHeight = imageHeight;
            il.OutputDepth = 3;
            net.Layers.Add(il);

            ConvLayer conv1_1 = new ConvLayer(64, 3, 3, 3, imageWidth, imageHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv1_1);
 
            ReluLayer rl1 = new ReluLayer(conv1_1.OutputDepth, conv1_1.OutputWidth, conv1_1.OutputHeight);
            net.Layers.Add(rl1);


            ConvLayer conv1_2 = new ConvLayer(64, 3, 3, rl1.OutputDepth, rl1.OutputWidth, rl1.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv1_2);

            ReluLayer rl2 = new ReluLayer(conv1_2.OutputDepth, conv1_2.OutputWidth, conv1_2.OutputHeight);
            net.Layers.Add(rl2);

            MaxPoolLayer pl1 = new MaxPoolLayer(2, 2, rl2.OutputDepth, rl2.OutputWidth, rl2.OutputHeight, 2, 0);
            net.Layers.Add(pl1);


            ConvLayer conv2_1 = new ConvLayer(128, 3, 3, pl1.OutputDepth, pl1.OutputWidth, pl1.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv2_1);
          
            ReluLayer rl3 = new ReluLayer(conv2_1.OutputDepth, conv2_1.OutputWidth, conv2_1.OutputHeight);
            net.Layers.Add(rl3);

            ConvLayer conv2_2 = new ConvLayer(128, 3, 3, rl3.OutputDepth, rl3.OutputWidth, rl3.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv2_2);
          
            ReluLayer rl4 = new ReluLayer(conv2_2.OutputDepth, conv2_2.OutputWidth, conv2_2.OutputHeight);
            net.Layers.Add(rl4);

            MaxPoolLayer pl2 = new MaxPoolLayer(2, 2, rl4.OutputDepth, rl4.OutputWidth, rl4.OutputHeight, 2, 0);
            net.Layers.Add(pl2);
             
            ConvLayer conv3_1 = new ConvLayer(256, 3, 3, pl2.OutputDepth, pl2.OutputWidth, pl2.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv3_1);
             
            ReluLayer rl5 = new ReluLayer(conv3_1.OutputDepth, conv3_1.OutputWidth, conv3_1.OutputHeight);
            net.Layers.Add(rl5);

            ConvLayer conv3_2 = new ConvLayer(256, 3, 3, rl5.OutputDepth, rl5.OutputWidth, rl5.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv3_2);
            
            ReluLayer rl6 = new ReluLayer(conv3_2.OutputDepth, conv3_2.OutputWidth, conv3_2.OutputHeight);
            net.Layers.Add(rl6);


            ConvLayer conv3_3 = new ConvLayer(256, 3, 3, rl6.OutputDepth, rl6.OutputWidth, rl6.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv3_3);
          
            ReluLayer rl7 = new ReluLayer(conv3_3.OutputDepth, conv3_3.OutputWidth, conv3_3.OutputHeight);
            net.Layers.Add(rl7);

            MaxPoolLayer pl3 = new MaxPoolLayer(2, 2, rl7.OutputDepth, rl7.OutputWidth, rl7.OutputHeight, 2, 0);
            net.Layers.Add(pl3);

            ConvLayer conv4_1 = new ConvLayer(512, 3, 3, pl3.OutputDepth, pl3.OutputWidth, pl3.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv4_1);
     
            ReluLayer rl8 = new ReluLayer(conv4_1.OutputDepth, conv4_1.OutputWidth, conv4_1.OutputHeight);
            net.Layers.Add(rl8);

            ConvLayer conv4_2 = new ConvLayer(512, 3, 3, rl8.OutputDepth, rl8.OutputWidth, rl8.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv4_2);
            
            ReluLayer rl9 = new ReluLayer(conv4_2.OutputDepth, conv4_2.OutputWidth, conv4_2.OutputHeight);
            net.Layers.Add(rl9);


            ConvLayer conv4_3 = new ConvLayer(512, 3, 3, rl9.OutputDepth, rl9.OutputWidth, rl9.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv4_3);
           
            ReluLayer rl10 = new ReluLayer(conv4_3.OutputDepth, conv4_3.OutputWidth, conv4_3.OutputHeight);
            net.Layers.Add(rl10);

            MaxPoolLayer pl4 = new MaxPoolLayer(2, 2, rl10.OutputDepth, rl10.OutputWidth, rl10.OutputHeight, 2, 0);
            net.Layers.Add(pl4);

            ConvLayer conv5_1 = new ConvLayer(512, 3, 3, pl4.OutputDepth, pl4.OutputWidth, pl4.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv5_1);
             
            ReluLayer rl11 = new ReluLayer(conv5_1.OutputDepth, conv5_1.OutputWidth, conv5_1.OutputHeight);
            net.Layers.Add(rl11);

            ConvLayer conv5_2 = new ConvLayer(512, 3, 3, rl11.OutputDepth, rl11.OutputWidth, rl11.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv5_2);
            
            ReluLayer rl12 = new ReluLayer(conv5_2.OutputDepth, conv5_2.OutputWidth, conv5_2.OutputHeight);
            net.Layers.Add(rl12);


            ConvLayer conv5_3 = new ConvLayer(512, 3, 3, rl12.OutputDepth, rl12.OutputWidth, rl12.OutputHeight, 1, 1, 0, 1, 0.1f);
            net.Layers.Add(conv5_3);
             
            ReluLayer rl13 = new ReluLayer(conv5_3.OutputDepth, conv5_3.OutputWidth, conv5_3.OutputHeight);
            net.Layers.Add(rl13);

            MaxPoolLayer pl5 = new MaxPoolLayer(2, 2, rl13.OutputDepth, rl13.OutputWidth, rl13.OutputHeight, 2, 0);
            net.Layers.Add(pl5);

            FullyConnLayer fc = new FullyConnLayer(4096, pl5.OutputDepth,
                pl5.OutputWidth, pl5.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc);
             
            ReluLayer rl14 = new ReluLayer(fc.OutputDepth, fc.OutputWidth, fc.OutputHeight);
            net.Layers.Add(rl14);
            DropoutLayer d = new DropoutLayer(rl14.OutputDepth, rl14.OutputWidth, rl14.OutputHeight, 0.5f);
            net.Layers.Add(d);

            FullyConnLayer fc2 = new FullyConnLayer(4096, d.OutputDepth, d.OutputWidth, d.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc2);
            
            ReluLayer rl15 = new ReluLayer(fc2.OutputDepth, fc2.OutputWidth, fc2.OutputHeight);
            net.Layers.Add(rl15);
            DropoutLayer d2 = new DropoutLayer(rl15.OutputDepth, rl15.OutputWidth, rl15.OutputHeight, 0.5f);
            net.Layers.Add(d2);


            FullyConnLayer fc3 = new FullyConnLayer(LabelsCount, d2.OutputDepth, d2.OutputWidth, d2.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc3);
          
            SoftmaxLayer sl = new SoftmaxLayer(fc3.OutputDepth, fc3.OutputWidth, fc3.OutputHeight);
            net.LossLayer = sl;

            return net;

        }

        public static List<string> Places365Labels()
        {
            List<string> res = new List<string>();

            using (FileStream fslabels = new FileStream("Models/Places365Labels.txt", FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fslabels);

                while (sr.EndOfStream == false)
                {
                    var line = sr.ReadLine();
                    res.Add(line);

                }
            }

            return res;

        }
        public static List<string> ImageNetLabels()
        {
            List<string> res = new List<string>();

            using (FileStream fslabels = new FileStream("Models/ImageNetLabels.txt", FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fslabels);

                while (sr.EndOfStream == false)
                {
                    var line = sr.ReadLine();
                    res.Add(line);

                }
            }

            return res;

        }

    }
}
