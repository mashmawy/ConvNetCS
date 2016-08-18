using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvNetCS
{
    // A Brain object does all the magic.
    // over time it receives some inputs and some rewards
    // and its job is to set the outputs to maximize the expected reward
    public class Brain
    {

        // in number of time steps, of temporal memory
        // the ACTUAL input to the net will be (x,a) temporal_window times, and followed by current x
        // so to have no information from previous time step going into value function, set to 0.
        public int temporal_window { get; set; }

        // size of experience replay memory
        public int experience_size { get; set; }

        // number of examples in experience replay memory before we begin learning
        public int start_learn_threshold { get; set; }

        // gamma is a crucial parameter that controls how much plan-ahead the agent does. In [0,1]
        public double gamma { get; set; }

        // number of steps we will learn for
        public int learning_steps_total { get; set; }

        // how many steps of the above to perform only random actions (in the beginning)?
        public int learning_steps_burnin { get; set; }

        // what epsilon value do we bottom out on? 0.0 => purely deterministic policy at end
        public double epsilon_min { get; set; }

        // what epsilon to use at test time? (i.e. when learning is disabled)
        public double epsilon_test_time { get; set; }

        // advanced feature. Sometimes a random action should be biased towards some values
        // for example in flappy bird, we may want to choose to not flap more often
        // this better sum to 1 by the way, and be of length this.num_actions
        public double[] random_action_distribution { get; set; }

        // states that go into neural net to predict optimal action look as
        // x0,a0,x1,a1,x2,a2,...xt
        // this variable controls the size of that temporal window. Actions are
        // encoded as 1-of-k hot vectors
        public int net_inputs { get; set; }

        public int num_states { get; set; }
        public int num_actions { get; set; }
        public int window_size { get; set; }


        public List<double[]> state_window { get; set; }
        public List<double> action_window { get; set; }
        public List<double> reward_window { get; set; }
        public List<double[]> net_window { get; set; }

        public Network value_net { get; set; }
        public Trainer tdtrainer { get; set; }

        public List<Experience> experience { get; set; }

        public int age { get; set; }
        public int forward_passes { get; set; }
        // controls exploration exploitation tradeoff. Should be annealed over time
        public double epsilon { get; set; }
        public double latest_reward { get; set; }
        public double[] last_input_array { get; set; }
        public Window average_reward_window { get; set; }
        public Window average_loss_window { get; set; }
        public bool learning { get; set; }
        public Brain()
        {


            this.temporal_window = 1;
            this.experience_size = 30000;
            this.start_learn_threshold = (int)Math.Floor(Math.Min(this.experience_size * 0.1, 1000));
            this.gamma = 0.8;

            this.learning_steps_total = 100000;

            // how many steps of the above to perform only random actions (in the beginning)?
            this.learning_steps_burnin = 3000;
            this.epsilon_min = 0.05;
            this.epsilon_test_time = 0.01;

            this.random_action_distribution = new double[0];


            // states that go into neural net to predict optimal action look as
            // x0,a0,x1,a1,x2,a2,...xt
            // this variable controls the size of that temporal window. Actions are
            // encoded as 1-of-k hot vectors
            this.net_inputs = num_states * this.temporal_window + num_actions * this.temporal_window + num_states;
            this.num_states = num_states;
            this.num_actions = num_actions;
            this.window_size = Math.Max(this.temporal_window, 2); // must be at least 2, but if we want more context even more


            this.state_window = new List<double[]>(this.window_size);


            this.action_window = new List<double>(this.window_size);

            this.reward_window = new List<double>(this.window_size);

            this.net_window = new List<double[]>(this.window_size);
            for (int i = 0; i < this.window_size; i++)
            {
                this.net_window.Add(new double[2]);
                this.state_window.Add(new double[2]);
                this.action_window.Add(0);
                this.reward_window.Add(0);
            }




            this.experience = new List<Experience>();

            // various housekeeping variables
            this.age = 0; // incremented every backward()
            this.forward_passes = 0; // incremented every forward()
            this.epsilon = 1.0; // controls exploration exploitation tradeoff. Should be annealed over time
            this.latest_reward = 0;
            this.average_reward_window = new Window(1000, 10);
            this.average_loss_window = new Window(1000, 10);
            this.learning = true;
            this.last_input_array = new double[0];
        }



        public double random_action()
        {
            // a bit of a helper function. It returns a random action
            // we are abstracting this away because in future we may want to 
            // do more sophisticated things. For example some actions could be more
            // or less likely at "rest"/default state.
            if (this.random_action_distribution.Length == 0)
            {
                return Util.Randi(0, this.num_actions);
            }
            else
            {
                // okay, lets do some fancier sampling:
                var p = Util.RandF(0, 1.0);
                var cumprob = 0.0;
                for (var k = 0; k < this.num_actions; k++)
                {
                    cumprob += this.random_action_distribution[k];
                    if (p < cumprob) { return k; }
                }
            }
            return 0;
        }

        public Policy policy(double[] s)
        {
            // compute the value of doing any action in this state
            // and return the argmax action and its value
            var svol = new Vol(1, 1, this.net_inputs);
            svol.W = s;
            var action_values = this.value_net.Forward(svol, false);
            var maxk = 0;
            var maxval = action_values.W[0];
            for (var k = 1; k < this.num_actions; k++)
            {
                if (action_values.W[k] > maxval) { maxk = k; maxval = action_values.W[k]; }
            }
            return new Policy() { action = maxk, value = maxval };
        }

        public double[] getNetInput(double[] xt)
        {
            // return s = (x,a,x,a,x,a,xt) state vector. 
            // It's a concatenation of last window_size (x,a) pairs and current state x
            List<double> w = new List<double>();
            w.AddRange(xt); // start with current state
            // and now go backwards and append states and actions from history temporal_window times
            var n = this.window_size;
            for (var k = 0; k < this.temporal_window; k++)
            {
                // state
                w.AddRange(this.state_window[n - 1 - k]);
                // action, encoded as 1-of-k indicator vector. We scale it up a bit because
                // we dont want weight regularization to undervalue this information, as it only exists once
                var action1ofk = new double[this.num_actions];
                for (var q = 0; q < this.num_actions; q++) action1ofk[q] = 0.0;
                action1ofk[(int)this.action_window[n - 1 - k]] = 1.0 * this.num_states;
                w.AddRange(action1ofk);
            }
            return w.ToArray();
        }

        public double forward(double[] input_array)
        {
            // compute forward (behavior) pass given the input neuron signals from body
            this.forward_passes += 1;
            this.last_input_array = input_array; // back this up

            double[] net_input;
            // create network input
            double action;
            if (this.forward_passes > this.temporal_window)
            {
                // we have enough to actually do something reasonable
                net_input = this.getNetInput(input_array);
                if (this.learning)
                {
                    // compute epsilon for the epsilon-greedy policy
                    this.epsilon = Math.Min(1.0, Math.Max(this.epsilon_min, 1.0 - (double)((double)this.age - (double)this.learning_steps_burnin) / ((double)this.learning_steps_total - (double)this.learning_steps_burnin)));
                }
                else
                {
                    this.epsilon = this.epsilon_test_time; // use test-time value
                }
                var rf = Util.RandF(0, 1);
                if (rf < this.epsilon)
                {
                    // choose a random action with epsilon probability
                    action = this.random_action();
                }
                else
                {
                    // otherwise use our policy to make decision
                    var maxact = this.policy(net_input);
                    action = maxact.action;
                }
            }
            else
            {
                // pathological case that happens first few iterations 
                // before we accumulate window_size inputs
                net_input = new double[0];
                action = this.random_action();
            }

            // remember the state and action we took for backward pass

            this.net_window.RemoveAt(0);
            this.net_window.Add(net_input);
            this.state_window.RemoveAt(0);
            this.state_window.Add(input_array);
            this.action_window.RemoveAt(0);
            this.action_window.Add(action);

            return action;
        }
        public void backward(double reward)
        {
            this.latest_reward = reward;
            this.average_reward_window.Add(reward);


            this.reward_window.RemoveAt(0);
            this.reward_window.Add(reward);

            if (!this.learning) { return; }

            // various book-keeping
            this.age += 1;

            // it is time t+1 and we have to store (s_t, a_t, r_t, s_{t+1}) as new experience
            // (given that an appropriate number of state measurements already exist, of course)
            if (this.forward_passes > this.temporal_window + 1)
            {
                var e = new Experience();
                var n = this.window_size;
                e.state0 = this.net_window[n - 2];
                e.action0 = this.action_window[n - 2];
                e.reward0 = this.reward_window[n - 2];
                e.state1 = this.net_window[n - 1];
                if (this.experience.Count < this.experience_size)
                {
                    this.experience.Add(e);
                }
                else
                {
                    // replace. finite memory!
                    var ri = Util.Randi(0, this.experience_size);
                    this.experience[(int)ri] = e;
                }
            }

            // learn based on experience, once we have some samples to go on
            // this is where the magic happens...
            if (this.experience.Count > this.start_learn_threshold)
            {
                var avcost = 0.0;
                for (var k = 0; k < this.tdtrainer.batch_size; k++)
                {
                    var re = Util.Randi(0, this.experience.Count);
                    var e = this.experience[(int)re];
                    var x = new Vol(1, 1, this.net_inputs);
                    x.W = e.state0;
                    var maxact = this.policy(e.state1);
                    var r = e.reward0 + this.gamma * maxact.value;
                    ClassOutput ystruct = new ClassOutput() { dim = (int)e.action0, val = r };
                    var loss = this.tdtrainer.Train(x, ystruct);
                    avcost += loss.loss;
                }
                avcost = avcost / this.tdtrainer.batch_size;
                this.average_loss_window.Add(avcost);
            }
        }
    }

    public class Policy
    {
        public int action { get; set; }
        public double value { get; set; }
    }
}
