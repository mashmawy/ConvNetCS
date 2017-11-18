using ConvNetCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayWithVGG
{
    public partial class VGG16Form : Form
    {
        public VGG16Form()
        {
            InitializeComponent();
        }
        List<string> Labels;
        Network net;
        delegate void ProgressDeleget(int progress);
        delegate void PredictionTimeDeleget(double seconds);
        delegate void DisplayResultsDeleget(Dictionary<int,float> results);
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        Thread predictionThread  ;
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            var resp = ofd.ShowDialog();
            if (resp == System.Windows.Forms.DialogResult.OK)
            {
                this.PredictionProgress.Value = 0;
                var x = LoadImage(ofd);
                predictionThread = new Thread(new ParameterizedThreadStart(Predict));
                predictionThread.Start(x);
            }
        }

        private void Predict(object param)
        {
            Vol x = param as Vol;
            var start = DateTime.Now;
            var p = net.Forward(x, true);

            var end = DateTime.Now;
            var time = end.Subtract(start).TotalSeconds;
            this.Invoke(new PredictionTimeDeleget(SetTime), time);
            var results = net.GetTop5Prediction();
            this.Invoke(new DisplayResultsDeleget(DisplayResults), results); 

           
        }

        private Vol LoadImage(OpenFileDialog ofd)
        {
            Image image = Bitmap.FromFile(ofd.FileName);
            ImageToPredict.Image = image;

            var x = new Vol(224, 224, 3, 0.0f);
            Bitmap bmp = ResizeImage(ImageToPredict.Image, 224, 224);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color clr = bmp.GetPixel(i, j); // Get the color of pixel at position 5,5
                    float red = clr.R - 123.68f;
                    float green = clr.G - 116.779f;
                    float blue = clr.B - 103.939f;

                    x.Set(i, j, 0, red);
                    x.Set(i, j, 1, green);
                    x.Set(i, j, 2, blue);
                }
            }
            return x;
        }

        private void DisplayFeaturesButton_Click(object sender, EventArgs e)
        {

            this.flowLayoutPanel1.Controls.Clear();


            DisplayFeatures();

        }
        private void DisplayFeatures()
        {
            var filters = net.Layers[this.LayersComboBox.SelectedIndex + 1].Output;
            for (int d = 0; d < filters.Depth; d++)
            {

                Bitmap bmap = new Bitmap(filters.Width, filters.Height);
                var min = 99999999.0f;
                var max = 0.0f;
                for (int x = 0; x < filters.Width; x++)
                {
                    for (int y = 0; y < filters.Height; y++)
                    {
                        var v = filters.Get(x, y, d);
                        if (v < min)
                        {
                            min = v;
                        }
                        if (v > max)
                        {
                            max = v;
                        }
                    }
                }
                for (int x = 0; x < filters.Width; x++)
                {
                    for (int y = 0; y < filters.Height; y++)
                    {
                        var r = ((filters.Get(x, y, d) - min) / (max - min)) * 255.0;
                        if (r<0.0 )
                        {
                            r = 0.0;
                        }
                        if (r> 255.0)
                        {
                            r = 255.0;
                        }
                        Color pix = Color.FromArgb((int)r, (int)r, (int)r);

                        bmap.SetPixel(x, y, pix);
                    }
                }
                PictureBox pb = new PictureBox();
                pb.Width = 50;
                pb.Height = 50;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Image = bmap;
                pb.Click += pb_Click;
                this.flowLayoutPanel1.Controls.Add(pb);

            }
        }

        void pb_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            ViewFeatureForm vff = new ViewFeatureForm();
            vff.SetImage(pb.Image);
            vff.ShowDialog(); 
        }

        private void openImageNetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            var resp = ofd.ShowDialog();
            if (resp == System.Windows.Forms.DialogResult.OK)
            {
                net = Network.Load(ofd.FileName);
                Labels = ConvNetCS.Models.VGG16.ImageNetLabels();

                BrowseButton.Enabled = true;
                net.ForwardLayer += net_ForwardLayer;
                this.PredictionProgress.Maximum = net.Layers.Count - 1;
            }
        }
        private void openPlaces365ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            var resp = ofd.ShowDialog();
            if (resp == System.Windows.Forms.DialogResult.OK)
            {
                net = Network.Load(ofd.FileName);
                Labels = ConvNetCS.Models.VGG16.Places365Labels();
                BrowseButton.Enabled = true;
                net.ForwardLayer += net_ForwardLayer;
                this.PredictionProgress.Maximum = net.Layers.Count - 1;
            }
        }
        int counter = 0;
        void net_ForwardLayer(object sender, EventArgs e)
        {
            counter++;
            this.Invoke(new ProgressDeleget(Progress), counter); 
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
      
        public void Progress(int progress)
        {
            this.PredictionProgress.Value = progress;
           
        }
        
        public void SetTime(double time)
        { 
            PredictionTimeLabel.Text = "Prediction Time : " + time.ToString().ToString() +  " Seconds";
           
        }
        
        private void DisplayResults(Dictionary<int,float> results)
        {
            PredictionListBox.Items.Clear(); 
            foreach (var item in results)
            {
                var label=  Labels[item.Key];
                PredictionListBox.Items.Add(((item.Value * Labels.Count)/100).ToString() + " " + label);
            }
            for (int i = 1; i < net.Layers.Count; i++)
            {
                var item = net.Layers[i];
                this.LayersComboBox.Items.Add("{" + i.ToString() + "}  " +
                    item.GetType().ToString());
            }
            DisplayFeaturesButton.Enabled = true;
        }

        private void VGG16Form_Load(object sender, EventArgs e)
        {

        }
    }
}
