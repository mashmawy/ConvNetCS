using ConvNetCS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeepQLearnExample
{
    public partial class rldemoWindow : Form
    {
        public rldemoWindow()
        {
            InitializeComponent();
        }

        World w;
        private void rldemoWindow_Load(object sender, EventArgs e)
        {
            w = new World();
            w.W = 700;
            w.H = 500;

            w.agents.Add(new Agent());
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.Clear(Color.White);

            GraphicsPath path = new GraphicsPath();

            // draw walls in environment
            for (int i = 4; i < 7; i++)
            {
                var q = w.walls[i];
                e.ClipRectangle.Offset((int)q.p1.X, (int)q.p1.Y);
                path.AddLine((int)q.p1.X, (int)q.p1.Y, (int)q.p2.X, (int)q.p2.Y);
            }
            Pen redPen = new Pen(Color.Black, 1);
            e.Graphics.DrawPath(redPen, path);


            GraphicsPath path2 = new GraphicsPath();
            // draw walls in environment
            for (int i = 7; i < 10; i++)
            {
                var q = w.walls[i];
                e.ClipRectangle.Offset((int)q.p1.X, (int)q.p1.Y);
                path2.AddLine((int)q.p1.X, (int)q.p1.Y, (int)q.p2.X, (int)q.p2.Y);
            }
            Pen redPen2 = new Pen(Color.Black, 1);
            e.Graphics.DrawPath(redPen2, path2);

            var r = Math.Floor(w.agents[0].brain.latest_reward * 200);
            if (r > 255) r = 255; if (r < 0) r = 0;

            for (int i = 0, n = w.agents.Count; i < n; i++)
            {
                var a = w.agents[i];


                Pen redPen3 = new Pen(Color.Blue, 4);
                e.Graphics.DrawArc(redPen3, (float)a.op.X, (float)a.op.Y,
                    (float)a.rad * 2, (float)a.rad * 2, (float)(0), (float)(360));


                for (int ei = 0, ne = a.eyes.Count; ei < ne; ei++)
                {
                    var e2 = a.eyes[ei];
                    var sr = e2.sensed_proximity;

                    GraphicsPath path3 = new GraphicsPath();
                    Color co = Color.Black;
                    // draw walls in environment
                    if (e2.sensed_type == 1) { co = Color.FromArgb(255, 150, 150); } // apples
                    if (e2.sensed_type == 2) { co = Color.FromArgb(150, 255, 150); } // poison

                    path3.AddLine((float)a.op.X, (float)a.op.Y,
                        (float)a.op.X + (float)(sr * Math.Sin(a.oangle + e2.angle)),
                        (float)a.op.Y + (float)(sr * Math.Cos(a.oangle + e2.angle)));

                    Pen redPen4 = new Pen(co, 1);
                    e.Graphics.DrawPath(redPen4, path3);

                }

            }


            for (int i = 0, n = w.items.Count; i < n; i++)
            {
                var it = w.items[i];
                Color co = Color.Black;
                // draw walls in environment
                if (it.type == 1.0) { co = Color.FromArgb(255, 150, 150); } // apples
                if (it.type == 2.0) { co = Color.FromArgb(150, 255, 150); } // poison

                Pen redPen3 = new Pen(co, 4);
                e.Graphics.DrawArc(redPen3, (float)it.p.X, (float)it.p.Y,
                    (float)it.rad * 2, (float)it.rad * 2, (float)(0), (float)(360));

            }
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            var num_inputs = 27; // 9 eyes, each sees 3 numbers (wall, green, red thing proximity)
            var num_actions = 5; // 5 possible angles agent can turn
            var temporal_window = 1; // amount of temporal memory. 0 = agent lives in-the-moment :)
            var network_size = num_inputs * temporal_window + num_actions * temporal_window + num_inputs;
            var brain = new Brain();



            Trainer trainer = new Trainer();

            brain.value_net = CreateNewNetwork();
            trainer.method = TrainingMethod.sgd;
            trainer.batch_size = 64;
            trainer.l2_decay = 0.01f;
            trainer.momentum = 0.0f;
            trainer.learning_rate = 0.001f;
            trainer.Net = brain.value_net;



            brain.tdtrainer = trainer;
            brain.temporal_window = temporal_window;
            brain.experience_size = 30000;
            brain.start_learn_threshold = 1000;
            brain.gamma = 0.7f;
            brain.learning_steps_total = 200000;
            brain.learning_steps_burnin = 3000;
            brain.epsilon_min = 0.05f;
            brain.epsilon_test_time = 0.05f;
            brain.num_actions = num_actions;
            brain.net_inputs = num_inputs;

            w.agents[0].brain = brain;

            Timer worldTimer = new Timer();
            worldTimer.Interval = 10;
            worldTimer.Tick += worldTimer_Tick;
            worldTimer.Start();
            label12.Text = DateTime.Now.ToShortTimeString();
        }
        void worldTimer_Tick(object sender, EventArgs e)
        {
            w.tick();

            pictureBox1.Refresh();



            label1.Text = w.agents[0].brain.average_loss_window.get_average().ToString();

            label2.Text = w.agents[0].brain.average_reward_window.get_average().ToString();


            label3.Text = w.agents[0].brain.experience.Count.ToString();

            label4.Text = w.agents[0].brain.age.ToString();

            label5.Text = w.agents[0].brain.epsilon.ToString();


            //t += 'exploration epsilon: ' + this.epsilon + '<br>';

        }

        private void StartButton_Click(object sender, EventArgs e)
        {

            w.agents[0].brain.learning = true;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {

            w.agents[0].brain.learning = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            w.agents[0].brain.learning = false;
            using (FileStream fs2 = new FileStream("BrainNetwork.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryFormatter bf2 = new BinaryFormatter();
                bf2.Serialize(fs2, w.agents[0].brain.value_net);
                fs2.Close();
            }
            w.agents[0].brain.learning = true;
        }

        private void LoadButtons_Click(object sender, EventArgs e)
        {
            w.agents[0].brain.learning = false;
            using (FileStream fs = new FileStream("BrainNetwork.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                var dd = bf.Deserialize(fs) as Network;
                w.agents[0].brain.value_net = dd;
                fs.Close();
            }
        }

        Random rand = new Random();
        private void GenerateItemsButton_Click(object sender, EventArgs e)
        {
            // set up food and poison
            w.items.Clear();
            w.items = new List<Item>();
            for (var k = 0; k < 30; k++)
            {
                var x = Util.RandF(20, w.W - 20);
                var y = Util.RandF(20, w.H - 20);
                var t = rand.Next(1, 3); // food or poison (1 and 2)
                var it = new Item(x, y, t);
                w.items.Add(it);
            }
        }
        private static Network CreateNewNetwork()
        {
            var num_inputs = 27; // 9 eyes, each sees 3 numbers (wall, green, red thing proximity)
            var num_actions = 5; // 5 possible angles agent can turn
            var temporal_window = 1; // amount of temporal memory. 0 = agent lives in-the-moment :)
            var network_size = num_inputs * temporal_window + num_actions * temporal_window + num_inputs;

            Network net = new Network();

            InputLayer il = new InputLayer();
            il.OutputWidth = 1;
            il.OutputHeight = 1;
            il.OutputDepth = network_size;
            net.Layers.Add(il);



            FullyConnLayer fc = new FullyConnLayer(50, il.OutputDepth, il.OutputWidth, il.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc);

            ReluLayer rl = new ReluLayer(fc.OutputDepth, fc.OutputWidth, fc.OutputHeight);
            net.Layers.Add(rl);
             

            FullyConnLayer fc2 = new FullyConnLayer(50, rl.OutputDepth, rl.OutputWidth, rl.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc2);

            ReluLayer rl2 = new ReluLayer(fc2.OutputDepth, fc2.OutputWidth, fc2.OutputHeight);
            net.Layers.Add(rl2);




            FullyConnLayer fc8 = new FullyConnLayer(5, rl2.OutputDepth, rl2.OutputWidth, rl2.OutputHeight, 0, 1, 0);
            net.Layers.Add(fc8);

            RegressionLayer sl = new RegressionLayer(fc8.OutputDepth, fc8.OutputWidth, fc8.OutputHeight);
            net.LossLayer = sl;
            return net;
        }

        //private static Network CreateNewNetwork()
        //{
        //    var num_inputs = 27; // 9 eyes, each sees 3 numbers (wall, green, red thing proximity)
        //    var num_actions = 5; // 5 possible angles agent can turn
        //    var temporal_window = 1; // amount of temporal memory. 0 = agent lives in-the-moment :)
        //    var network_size = num_inputs * temporal_window + num_actions * temporal_window + num_inputs;

        //    Network net = new Network();

        //    InputLayer il = new InputLayer();
        //    il.OutputWidth = 1;
        //    il.OutputHeight = 1;
        //    il.OutputDepth = network_size;
        //    net.Layers.Add(il);


        //    ConvLayer conv = new ConvLayer(16, 5, 5, il.OutputDepth, il.OutputWidth, il.OutputHeight, 1, 2, 0, 1, 0.1f);
        //    net.Layers.Add(conv);

        //    ReluLayer rlv = new ReluLayer(conv.OutputDepth, conv.OutputWidth, conv.OutputHeight);
        //    net.Layers.Add(rlv);

        //    MaxPoolLayer pl = new MaxPoolLayer(2, 2, rlv.OutputDepth, rlv.OutputWidth, rlv.OutputHeight, 2, 0, 0);
        //    net.Layers.Add(pl);

        //    FullyConnLayer fc = new FullyConnLayer(50, pl.OutputDepth, pl.OutputWidth, pl.OutputHeight, 0, 1, 0);
        //    net.Layers.Add(fc);

        //    ReluLayer rl = new ReluLayer(fc.OutputDepth, fc.OutputWidth, fc.OutputHeight);
        //    net.Layers.Add(rl);



        //    FullyConnLayer fc2 = new FullyConnLayer(50, rl.OutputDepth, rl.OutputWidth, rl.OutputHeight, 0, 1, 0);
        //    net.Layers.Add(fc2);

        //    ReluLayer rl2 = new ReluLayer(fc2.OutputDepth, fc2.OutputWidth, fc2.OutputHeight);
        //    net.Layers.Add(rl2);




        //    FullyConnLayer fc8 = new FullyConnLayer(5, rl2.OutputDepth, rl2.OutputWidth, rl2.OutputHeight, 0, 1, 0);
        //    net.Layers.Add(fc8);

        //    RegressionLayer sl = new RegressionLayer(fc8.OutputDepth, fc8.OutputWidth, fc8.OutputHeight);
        //    net.LossLayer = sl;
        //    return net;
        //}

    }
}
