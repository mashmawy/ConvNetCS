using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayWithVGG
{
    public partial class ViewFeatureForm : Form
    {
        public ViewFeatureForm()
        {
            InitializeComponent();
        }

        private void ViewFeatureForm_Load(object sender, EventArgs e)
        {

        }
        public void SetImage(Image im)
        {
            this.pictureBox1.Image = im;
        }
    }
}
