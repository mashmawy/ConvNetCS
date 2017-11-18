namespace PlayWithVGG
{
    partial class VGG16Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImageToPredict = new System.Windows.Forms.PictureBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.PredictionListBox = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.LayersComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DisplayFeaturesButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openImageNetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openPlaces365ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.PredictionTimeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.PredictionProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageToPredict)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ImageToPredict
            // 
            this.ImageToPredict.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageToPredict.Location = new System.Drawing.Point(12, 56);
            this.ImageToPredict.Name = "ImageToPredict";
            this.ImageToPredict.Size = new System.Drawing.Size(260, 203);
            this.ImageToPredict.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ImageToPredict.TabIndex = 0;
            this.ImageToPredict.TabStop = false;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Enabled = false;
            this.BrowseButton.Location = new System.Drawing.Point(197, 28);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 1;
            this.BrowseButton.Text = "Browse..";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // PredictionListBox
            // 
            this.PredictionListBox.FormattingEnabled = true;
            this.PredictionListBox.Location = new System.Drawing.Point(11, 291);
            this.PredictionListBox.Name = "PredictionListBox";
            this.PredictionListBox.Size = new System.Drawing.Size(261, 134);
            this.PredictionListBox.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(292, 97);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(579, 328);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // LayersComboBox
            // 
            this.LayersComboBox.FormattingEnabled = true;
            this.LayersComboBox.Location = new System.Drawing.Point(351, 28);
            this.LayersComboBox.Name = "LayersComboBox";
            this.LayersComboBox.Size = new System.Drawing.Size(192, 21);
            this.LayersComboBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(294, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Layers";
            // 
            // DisplayFeaturesButton
            // 
            this.DisplayFeaturesButton.Enabled = false;
            this.DisplayFeaturesButton.Location = new System.Drawing.Point(567, 27);
            this.DisplayFeaturesButton.Name = "DisplayFeaturesButton";
            this.DisplayFeaturesButton.Size = new System.Drawing.Size(75, 23);
            this.DisplayFeaturesButton.TabIndex = 6;
            this.DisplayFeaturesButton.Text = "Visualize";
            this.DisplayFeaturesButton.UseVisualStyleBackColor = true;
            this.DisplayFeaturesButton.Click += new System.EventHandler(this.DisplayFeaturesButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(894, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openImageNetToolStripMenuItem,
            this.openPlaces365ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openImageNetToolStripMenuItem
            // 
            this.openImageNetToolStripMenuItem.Name = "openImageNetToolStripMenuItem";
            this.openImageNetToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openImageNetToolStripMenuItem.Text = "Load ImageNet";
            this.openImageNetToolStripMenuItem.Click += new System.EventHandler(this.openImageNetToolStripMenuItem_Click);
            // 
            // openPlaces365ToolStripMenuItem
            // 
            this.openPlaces365ToolStripMenuItem.Name = "openPlaces365ToolStripMenuItem";
            this.openPlaces365ToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openPlaces365ToolStripMenuItem.Text = "Load Places365";
            this.openPlaces365ToolStripMenuItem.Click += new System.EventHandler(this.openPlaces365ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.PredictionProgress,
            this.PredictionTimeLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 430);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(894, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // PredictionTimeLabel
            // 
            this.PredictionTimeLabel.Name = "PredictionTimeLabel";
            this.PredictionTimeLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // PredictionProgress
            // 
            this.PredictionProgress.Name = "PredictionProgress";
            this.PredictionProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(52, 17);
            this.toolStripStatusLabel1.Text = "Progress";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(292, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Features :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Image :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 271);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Results :";
            // 
            // VGG16Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 452);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.DisplayFeaturesButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LayersComboBox);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.PredictionListBox);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.ImageToPredict);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "VGG16Form";
            this.Text = "VGG16 Playground";
            this.Load += new System.EventHandler(this.VGG16Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageToPredict)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImageToPredict;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.ListBox PredictionListBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ComboBox LayersComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button DisplayFeaturesButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openImageNetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openPlaces365ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel PredictionTimeLabel;
        private System.Windows.Forms.ToolStripProgressBar PredictionProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

