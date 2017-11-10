using ConvNetCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepQLearnExample
{

    public class Vec
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vec(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float dist_from(Vec v)
        {
            return (float) Math.Sqrt(Math.Pow(this.X - v.X, 2)
                + Math.Pow(this.Y - v.Y, 2));

        }
        public float length()
        {
            return (float) Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));

        }

        public Vec add(Vec v)
        {
            return new Vec(this.X + v.X, this.Y + v.Y);
        }


        public Vec sub(Vec v)
        {
            return new Vec(this.X - v.X, this.Y - v.Y);
        }


        public Vec rotate(float a)
        {
            // CLOCKWISE
            return new Vec((float)this.X * (float)Math.Cos(a) + (float)this.Y * (float)Math.Sin(a),
                          (float)-this.X * (float)Math.Sin(a) + (float)this.Y * (float)Math.Cos(a));
        }

        public void scale(float s)
        {
            this.X *= s; this.Y *= s;
        }

        public void normalize()
        {
            var d = this.length(); this.scale(1.0f / d);
        }

    }

    // Wall is made up of two points
    public class Wall
    {
        public Vec p1 { get; set; }
        public Vec p2 { get; set; }
        public Wall(Vec p1, Vec p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }

    public class Item
    {
        public Vec p { get; set; }
        public float type { get; set; }
        public float rad { get; set; }
        public int age { get; set; }
        public bool cleanup_ { get; set; }
        public Item(float x, float y, float type)
        {
            this.p = new Vec(x, y); // position
            this.type = type;
            this.rad = 10; // default radius
            this.age = 0;
            this.cleanup_ = false;
        }
    }

    public class intersection
    {
        public float ua { get; set; }
        public float ub { get; set; }
        public Vec up { get; set; }

        public float type { get; set; }
    }


    public static class Helper
    {
        // line intersection helper function: does line segment (p1,p2) intersect segment (p3,p4) ?
        public static intersection line_intersect(Vec p1, Vec p2, Vec p3, Vec p4)
        {
            var denom = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
            if (denom == 0.0) { return null; } // parallel lines
            var ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denom;
            var ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denom;
            if (ua > 0.0 && ua < 1.0 && ub > 0.0 && ub < 1.0)
            {
                var up = new Vec(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
                return new intersection() { ua = ua, ub = ub, up = up }; // up is intersection point
            }
            return null;
        }

        public static intersection line_point_intersect(Vec p1, Vec p2, Vec p0, float rad)
        {
            var v = new Vec(p2.Y - p1.Y, -(p2.X - p1.X)); // perpendicular vector
            var d = Math.Abs((p2.X - p1.X) * (p1.Y - p0.Y) - (p1.X - p0.X) * (p2.Y - p1.Y));
            d = d / v.length();
            if (d > rad) { return null; }

            v.normalize();
            v.scale(d);
            var up = p0.add(v);
            float ua = 0;
            if (Math.Abs(p2.X - p1.X) > Math.Abs(p2.Y - p1.Y))
            {
                ua = (up.X - p1.X) / (p2.X - p1.X);
            }
            else
            {
                ua = (up.Y - p1.Y) / (p2.Y - p1.Y);
            }
            if (ua > 0.0 && ua < 1.0)
            {
                return new intersection() { ua = ua, up = up };
            }
            return null;
        }

        // World object contains many agents and walls and food and stuff
        public static void util_add_box(List<Wall> lst, float x, float y, float w, float h)
        {
            lst.Add(new Wall(new Vec(x, y), new Vec(x + w, y)));
            lst.Add(new Wall(new Vec(x + w, y), new Vec(x + w, y + h)));
            lst.Add(new Wall(new Vec(x + w, y + h), new Vec(x, y + h)));
            lst.Add(new Wall(new Vec(x, y + h), new Vec(x, y)));
        }




    }

    // Eye sensor has a maximum range and senses walls
    public class Eye
    {
        public float angle { get; set; }
        public int max_range { get; set; }
        public float sensed_proximity { get; set; }
        public float sensed_type { get; set; }

        public Eye(float angle)
        {
            this.angle = angle; // angle relative to agent its on
            this.max_range = 85;
            this.sensed_proximity = 85; // what the eye is seeing. will be set in world.tick()
            this.sensed_type = -1; // what does the eye see?
        }
    }


    // A single agent
    public class Agent
    {
        public Vec p { get; set; }
        public Vec op { get; set; }
        public float angle { get; set; }
        public List<float[]> actions { get; set; }

        public float rad { get; set; }
        public List<Eye> eyes { get; set; }
        public Brain brain { get; set; }
        public float reward_bonus { get; set; }
        public float digestion_signal { get; set; }

        public float rot1 { get; set; }
        public float rot2 { get; set; }
        public float actionix { get; set; }


        public int prevactionix { get; set; }

        public Agent()
        {

            // positional information
            this.p = new Vec(50, 50);
            this.op = this.p; // old position
            this.angle = 0; // direction facing

            this.actions = new List<float[]>();
            this.actions.Add(new float[] { 1, 1 });
            this.actions.Add(new float[] { 0.8f, 1 });
            this.actions.Add(new float[] { 1, 0.8f });
            this.actions.Add(new float[] { 0.5f, 0 });
            this.actions.Add(new float[] { 0, 0.5f });

            // properties
            this.rad = 10;
            this.eyes = new List<Eye>();
            for (var k = 0; k < 9; k++) { this.eyes.Add(new Eye((k - 3) * 0.25f)); }

            // braaain
            this.brain = new Brain();
            //var spec = document.getElementById('qspec').value;
            //eval(spec);
            this.brain = brain;

            this.reward_bonus = 0.0f;
            this.digestion_signal = 0.0f;

            // outputs on world
            this.rot1 = 0.0f; // rotation speed of 1st wheel
            this.rot2 = 0.0f; // rotation speed of 2nd wheel

            this.prevactionix = -1;

        }

        public void forward()
        {
            // in forward pass the agent simply behaves in the environment
            // create input to brain
            var num_eyes = this.eyes.Count;
            var input_array = new float[num_eyes * 3];
            for (var i = 0; i < num_eyes; i++)
            {
                var e = this.eyes[i];
                input_array[i * 3] = 1.0f;
                input_array[i * 3 + 1] = 1.0f;
                input_array[i * 3 + 2] = 1.0f;
                if (e.sensed_type != -1)
                {
                    // sensed_type is 0 for wall, 1 for food and 2 for poison.
                    // lets do a 1-of-k encoding into the input array
                    input_array[i * 3 + (int)e.sensed_type] = e.sensed_proximity / e.max_range; // normalize to [0,1]
                }
            }
            // get action from brain
            var actionix = this.brain.forward(input_array);
            var action = this.actions[(int)actionix];
            this.actionix = actionix; //back this up

            // demultiplex into behavior variables
            this.rot1 = action[0] * 1;
            this.rot2 = action[1] * 1;
        }

        public void backward()
        {
            // in backward pass agent learns.
            // compute reward 
            var proximity_reward = 0.0f;
            var num_eyes = this.eyes.Count;
            for (var i = 0; i < num_eyes; i++)
            {
                var e = this.eyes[i];
                // agents dont like to see walls, especially up close
                proximity_reward += e.sensed_type == 0 ? e.sensed_proximity / e.max_range : 1.0f;
            }
            proximity_reward = proximity_reward / num_eyes;
            proximity_reward =(float) Math.Min(1.0f, proximity_reward * 2);

            // agents like to go straight forward
            var forward_reward = 0.0f;
            if (this.actionix == 0 && proximity_reward > 0.75f) forward_reward = 0.1f * proximity_reward;

            // agents like to eat good things
            var digestion_reward = this.digestion_signal;
            this.digestion_signal = 0.0f;

            var reward = proximity_reward + forward_reward + digestion_reward;

            // pass to brain for learning
            this.brain.backward((float)reward);

        }



        public float oangle { get; set; }
    }




    public class World
    {
        public List<Agent> agents { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public int clock { get; set; }

        public List<Wall> walls { get; set; }

        public List<Item> items { get; set; }



        public World()
        {

            this.agents = new List<Agent>();

            this.W = 700;
            this.H = 500;
      
            this.clock = 0;

            // set up walls in the world
            this.walls = new List<Wall>();
            var pad = 10;
            Helper.util_add_box(this.walls, pad, pad, this.W - pad * 2, this.H - pad * 2);
            Helper.util_add_box(this.walls, 100, 100, 200, 300); // inner walls
            this.walls.RemoveAt(this.walls.Count - 1);
            Helper.util_add_box(this.walls, 400, 100, 200, 300);
            this.walls.RemoveAt(this.walls.Count - 1);
            Random rand = new Random();
            // set up food and poison
            this.items = new List<Item>();
            for (var k = 0; k < 30; k++)
            {
                var x = Util.RandF(20, this.W - 20);
                var y = Util.RandF(20, this.H - 20);
                var t = rand.Next(1, 3); // food or poison (1 and 2)
                var it = new Item(x, y, t);
                this.items.Add(it);
            }
        }

        // helper function to get closest colliding walls/items
        public intersection stuff_collide_(Vec p1, Vec p2, bool check_walls, bool check_items)
        {
            intersection minres = null;

            // collide with walls
            if (check_walls)
            {
                for (int i = 0, n = this.walls.Count; i < n; i++)
                {
                    var wall = this.walls[i];
                    var res = Helper.line_intersect(p1, p2, wall.p1, wall.p2);
                    if (res != null)
                    {
                        res.type = 0; // 0 is wall
                        if (minres == null) { minres = res; }
                        else
                        {
                            // check if its closer
                            if (res.ua < minres.ua)
                            {
                                // if yes replace it
                                minres = res;
                            }
                        }
                    }
                }
            }

            // collide with items
            if (check_items)
            {
                for (int i = 0, n = this.items.Count; i < n; i++)
                {
                    var it = this.items[i];
                    var res = Helper.line_point_intersect(p1, p2, it.p, it.rad);
                    if (res != null)
                    {
                        res.type = it.type; // store type of item
                        if (minres == null) { minres = res; }
                        else
                        {
                            if (res.ua < minres.ua) { minres = res; }
                        }
                    }
                }
            }

            return minres;


        }

        public void tick()
        {
            // tick the environment
            this.clock++;

            // fix input to all agents based on environment

            for (int i = 0, n = this.agents.Count; i < n; i++)
            {
                var a = this.agents[i];
                for (int ei = 0, ne = a.eyes.Count; ei < ne; ei++)
                {
                    var e = a.eyes[ei];
                    // we have a line from p to p->eyep
                    var eyep = new Vec((float)a.p.X + (float)e.max_range * (float)Math.Sin(a.angle + e.angle),
                                       (float)a.p.Y + e.max_range * (float)Math.Cos(a.angle + e.angle));
                    var res = this.stuff_collide_(a.p, eyep, true, true);
                    if (res != null)
                    {
                        // eye collided with wall
                        e.sensed_proximity = res.up.dist_from(a.p);
                        e.sensed_type = res.type;
                    }
                    else
                    {
                        e.sensed_proximity = e.max_range;
                        e.sensed_type = -1;
                    }
                }
            }

            // let the agents behave in the world based on their input
            for (int i = 0, n = this.agents.Count; i < n; i++)
            {
                this.agents[i].forward();
            }

            // apply outputs of agents on evironment
            for (int i = 0, n = this.agents.Count; i < n; i++)
            {
                var a = this.agents[i];
                a.op = a.p; // back up old position
                a.oangle = a.angle; // and angle

                // steer the agent according to outputs of wheel velocities
                var v = new Vec(0, a.rad / 2.0f);
                v = v.rotate(a.angle + (float)Math.PI / 2);
                var w1p = a.p.add(v); // positions of wheel 1 and 2
                var w2p = a.p.sub(v);
                var vv = a.p.sub(w2p);
                vv = vv.rotate(-a.rot1);
                var vv2 = a.p.sub(w1p);
                vv2 = vv2.rotate(a.rot2);
                var np = w2p.add(vv);
                np.scale(0.5f);
                var np2 = w1p.add(vv2);
                np2.scale(0.5f);
                a.p = np.add(np2);

                a.angle -= a.rot1;
                if (a.angle < 0) a.angle += 2 * (float)Math.PI;
                a.angle += a.rot2;
                if (a.angle > 2 * Math.PI) a.angle -= 2 * (float)Math.PI;

                // agent is trying to move from p to op. Check walls
                var res = this.stuff_collide_(a.op, a.p, true, false);
                if (res != null)
                {
                    // wall collision! reset position
                    a.p = a.op;
                }

                // handle boundary conditions
                if (a.p.X < 0) a.p.X = 0;
                if (a.p.X > this.W) a.p.X = this.W;
                if (a.p.Y < 0) a.p.Y = 0;
                if (a.p.Y > this.H) a.p.Y = this.H;
            }

            // tick all items
            var update_items = false;
            for (int i = 0, n = this.items.Count; i < n; i++)
            {
                var it = this.items[i];
                it.age += 1;

                // see if some agent gets lunch
                for (int j = 0, m = this.agents.Count; j < m; j++)
                {
                    var a = this.agents[j];
                    var d = a.p.dist_from(it.p);
                    if (d < it.rad + a.rad)
                    {

                        // wait lets just make sure that this isn't through a wall
                        var rescheck = this.stuff_collide_(a.p, it.p, true, false);
                        if (rescheck == null)
                        {
                            // ding! nom nom nom
                            if (it.type == 1) a.digestion_signal += 5.0f; // mmm delicious apple
                              if (it.type == 2) a.digestion_signal += -6.0f; // ewww poison
                          
                            it.cleanup_ = true;
                            update_items = true;
                            break; // break out of loop, item was consumed
                        }
                    }
                }

                if (it.age > 5000 && this.clock % 100 == 0 && Util.RandF(0, 1) < 0.1)
                {
                    it.cleanup_ = true; // replace this one, has been around too long
                    update_items = true;
                }
            }
            if (update_items)
            {
                var nt = new List<Item>();
                for (int i = 0, n = this.items.Count; i < n; i++)
                {
                    var it = this.items[i];
                    if (!it.cleanup_) nt.Add(it);
                }
                this.items = nt; // swap
            }
            if (this.items.Count < 30 && this.clock % 10 == 0 && Util.RandF(0, 1) < 0.25)
            {
                var newitx = Util.RandF(20, this.W - 20);
                var newity = Util.RandF(20, this.H - 20);
                var newitt = Util.Randi(1, 3); // food or poison (1 and 2)
                var newit = new Item(newitx, newity, newitt);
                this.items.Add(newit);
            }

            // agents are given the opportunity to learn based on feedback of their action on environment
            for (int i = 0, n = this.agents.Count; i < n; i++)
            {
                this.agents[i].backward();
            }
        }

    }
}
