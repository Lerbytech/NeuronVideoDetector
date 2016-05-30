namespace NeuronVideoDetector
{
  partial class MainForm
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
      this.components = new System.ComponentModel.Container();
      this.IB_Video = new Emgu.CV.UI.ImageBox();
      this.IB_Neurons = new Emgu.CV.UI.ImageBox();
      this.IB_Heatmap = new Emgu.CV.UI.ImageBox();
      this.BTN_Open = new System.Windows.Forms.Button();
      this.BTN_Start = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.LBL_FPS = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.CB_KillNoise = new System.Windows.Forms.CheckBox();
      this.CB_Kuwahara = new System.Windows.Forms.CheckBox();
      this.CB_KillLow = new System.Windows.Forms.CheckBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.CB_ShowBodiesUni = new System.Windows.Forms.CheckBox();
      this.CB_ShowBordersUni = new System.Windows.Forms.CheckBox();
      this.CB_ShowCenters_Left = new System.Windows.Forms.CheckBox();
      this.BTN_ImgLayers_Left = new System.Windows.Forms.Button();
      this.RB_ImgLayers = new System.Windows.Forms.RadioButton();
      this.RB_Borders = new System.Windows.Forms.RadioButton();
      this.RB_Bodies = new System.Windows.Forms.RadioButton();
      this.BTN_ImgLayers_Right = new System.Windows.Forms.Button();
      this.BTN_Borders_Right = new System.Windows.Forms.Button();
      this.BTN_Borders_Left = new System.Windows.Forms.Button();
      this.BTN_Bodies_Right = new System.Windows.Forms.Button();
      this.BTN_Bodies_Left = new System.Windows.Forms.Button();
      this.TB_ImgLayersNum = new System.Windows.Forms.TextBox();
      this.TB_BodiesNum = new System.Windows.Forms.TextBox();
      this.TB_BordersNum = new System.Windows.Forms.TextBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.RB_Kuwahara = new System.Windows.Forms.RadioButton();
      this.CB_ShowCenters_Right = new System.Windows.Forms.CheckBox();
      this.RB_Colorize = new System.Windows.Forms.RadioButton();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.IB_Video)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.IB_Neurons)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.IB_Heatmap)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // IB_Video
      // 
      this.IB_Video.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.IB_Video.Location = new System.Drawing.Point(411, 23);
      this.IB_Video.Name = "IB_Video";
      this.IB_Video.Size = new System.Drawing.Size(258, 195);
      this.IB_Video.TabIndex = 2;
      this.IB_Video.TabStop = false;
      // 
      // IB_Neurons
      // 
      this.IB_Neurons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.IB_Neurons.Location = new System.Drawing.Point(10, 263);
      this.IB_Neurons.Name = "IB_Neurons";
      this.IB_Neurons.Size = new System.Drawing.Size(697, 527);
      this.IB_Neurons.TabIndex = 3;
      this.IB_Neurons.TabStop = false;
      // 
      // IB_Heatmap
      // 
      this.IB_Heatmap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.IB_Heatmap.Location = new System.Drawing.Point(713, 263);
      this.IB_Heatmap.Name = "IB_Heatmap";
      this.IB_Heatmap.Size = new System.Drawing.Size(697, 527);
      this.IB_Heatmap.TabIndex = 4;
      this.IB_Heatmap.TabStop = false;
      // 
      // BTN_Open
      // 
      this.BTN_Open.Location = new System.Drawing.Point(15, 3);
      this.BTN_Open.Name = "BTN_Open";
      this.BTN_Open.Size = new System.Drawing.Size(75, 32);
      this.BTN_Open.TabIndex = 5;
      this.BTN_Open.Text = "Open...";
      this.BTN_Open.UseVisualStyleBackColor = true;
      this.BTN_Open.Click += new System.EventHandler(this.BTN_Open_Click);
      // 
      // BTN_Start
      // 
      this.BTN_Start.Location = new System.Drawing.Point(15, 41);
      this.BTN_Start.Name = "BTN_Start";
      this.BTN_Start.Size = new System.Drawing.Size(75, 33);
      this.BTN_Start.TabIndex = 6;
      this.BTN_Start.Text = "Start";
      this.BTN_Start.UseVisualStyleBackColor = true;
      this.BTN_Start.Click += new System.EventHandler(this.BTN_Start_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 79);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(42, 17);
      this.label1.TabIndex = 11;
      this.label1.Text = "FPS: ";
      // 
      // LBL_FPS
      // 
      this.LBL_FPS.AutoSize = true;
      this.LBL_FPS.Location = new System.Drawing.Point(55, 79);
      this.LBL_FPS.Name = "LBL_FPS";
      this.LBL_FPS.Size = new System.Drawing.Size(16, 17);
      this.LBL_FPS.TabIndex = 12;
      this.LBL_FPS.Text = "0";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.CB_KillNoise);
      this.groupBox1.Controls.Add(this.CB_Kuwahara);
      this.groupBox1.Controls.Add(this.CB_KillLow);
      this.groupBox1.Location = new System.Drawing.Point(96, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(154, 93);
      this.groupBox1.TabIndex = 13;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Filters";
      // 
      // CB_KillNoise
      // 
      this.CB_KillNoise.AutoSize = true;
      this.CB_KillNoise.Checked = true;
      this.CB_KillNoise.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CB_KillNoise.Location = new System.Drawing.Point(6, 18);
      this.CB_KillNoise.Name = "CB_KillNoise";
      this.CB_KillNoise.Size = new System.Drawing.Size(88, 21);
      this.CB_KillNoise.TabIndex = 14;
      this.CB_KillNoise.Text = "Kill Noise";
      this.CB_KillNoise.UseVisualStyleBackColor = true;
      this.CB_KillNoise.CheckedChanged += new System.EventHandler(this.CB_KillNoise_CheckedChanged);
      // 
      // CB_Kuwahara
      // 
      this.CB_Kuwahara.AutoSize = true;
      this.CB_Kuwahara.Checked = true;
      this.CB_Kuwahara.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CB_Kuwahara.Location = new System.Drawing.Point(6, 65);
      this.CB_Kuwahara.Name = "CB_Kuwahara";
      this.CB_Kuwahara.Size = new System.Drawing.Size(145, 21);
      this.CB_Kuwahara.TabIndex = 17;
      this.CB_Kuwahara.Text = "Kuwahara Smooth";
      this.CB_Kuwahara.UseVisualStyleBackColor = true;
      this.CB_Kuwahara.CheckedChanged += new System.EventHandler(this.CB_Kuwahara_CheckedChanged);
      // 
      // CB_KillLow
      // 
      this.CB_KillLow.AutoSize = true;
      this.CB_KillLow.Checked = true;
      this.CB_KillLow.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CB_KillLow.Location = new System.Drawing.Point(6, 38);
      this.CB_KillLow.Name = "CB_KillLow";
      this.CB_KillLow.Size = new System.Drawing.Size(77, 21);
      this.CB_KillLow.TabIndex = 14;
      this.CB_KillLow.Text = "No Low";
      this.CB_KillLow.UseVisualStyleBackColor = true;
      this.CB_KillLow.CheckedChanged += new System.EventHandler(this.CB_KillLow_CheckedChanged);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.CB_ShowBodiesUni);
      this.groupBox2.Controls.Add(this.CB_ShowBordersUni);
      this.groupBox2.Controls.Add(this.CB_ShowCenters_Left);
      this.groupBox2.Location = new System.Drawing.Point(256, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(149, 93);
      this.groupBox2.TabIndex = 16;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Augmentations";
      // 
      // CB_ShowBodiesUni
      // 
      this.CB_ShowBodiesUni.AutoSize = true;
      this.CB_ShowBodiesUni.Checked = true;
      this.CB_ShowBodiesUni.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CB_ShowBodiesUni.Location = new System.Drawing.Point(6, 65);
      this.CB_ShowBodiesUni.Name = "CB_ShowBodiesUni";
      this.CB_ShowBodiesUni.Size = new System.Drawing.Size(136, 21);
      this.CB_ShowBodiesUni.TabIndex = 19;
      this.CB_ShowBodiesUni.Text = "Show Bodies Uni";
      this.CB_ShowBodiesUni.UseVisualStyleBackColor = true;
      this.CB_ShowBodiesUni.CheckedChanged += new System.EventHandler(this.CB_ShowBodies_CheckedChanged);
      // 
      // CB_ShowBordersUni
      // 
      this.CB_ShowBordersUni.AutoSize = true;
      this.CB_ShowBordersUni.Checked = true;
      this.CB_ShowBordersUni.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CB_ShowBordersUni.Location = new System.Drawing.Point(6, 38);
      this.CB_ShowBordersUni.Name = "CB_ShowBordersUni";
      this.CB_ShowBordersUni.Size = new System.Drawing.Size(143, 21);
      this.CB_ShowBordersUni.TabIndex = 17;
      this.CB_ShowBordersUni.Text = "Show Borders Uni";
      this.CB_ShowBordersUni.UseVisualStyleBackColor = true;
      this.CB_ShowBordersUni.CheckedChanged += new System.EventHandler(this.CB_ShowBorders_CheckedChanged);
      // 
      // CB_ShowCenters_Left
      // 
      this.CB_ShowCenters_Left.AutoSize = true;
      this.CB_ShowCenters_Left.Checked = true;
      this.CB_ShowCenters_Left.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CB_ShowCenters_Left.Location = new System.Drawing.Point(6, 18);
      this.CB_ShowCenters_Left.Name = "CB_ShowCenters_Left";
      this.CB_ShowCenters_Left.Size = new System.Drawing.Size(117, 21);
      this.CB_ShowCenters_Left.TabIndex = 17;
      this.CB_ShowCenters_Left.Text = "Show Centers";
      this.CB_ShowCenters_Left.UseVisualStyleBackColor = true;
      this.CB_ShowCenters_Left.CheckedChanged += new System.EventHandler(this.CB_ShowCenters_CheckedChanged);
      // 
      // BTN_ImgLayers_Left
      // 
      this.BTN_ImgLayers_Left.Location = new System.Drawing.Point(122, 20);
      this.BTN_ImgLayers_Left.Name = "BTN_ImgLayers_Left";
      this.BTN_ImgLayers_Left.Size = new System.Drawing.Size(25, 23);
      this.BTN_ImgLayers_Left.TabIndex = 17;
      this.BTN_ImgLayers_Left.Text = "<";
      this.BTN_ImgLayers_Left.UseVisualStyleBackColor = true;
      this.BTN_ImgLayers_Left.Click += new System.EventHandler(this.BTN_ImgLayers_Left_Click);
      // 
      // RB_ImgLayers
      // 
      this.RB_ImgLayers.AutoSize = true;
      this.RB_ImgLayers.Checked = true;
      this.RB_ImgLayers.Location = new System.Drawing.Point(6, 21);
      this.RB_ImgLayers.Name = "RB_ImgLayers";
      this.RB_ImgLayers.Size = new System.Drawing.Size(114, 21);
      this.RB_ImgLayers.TabIndex = 18;
      this.RB_ImgLayers.TabStop = true;
      this.RB_ImgLayers.Text = "Image Layers";
      this.RB_ImgLayers.UseVisualStyleBackColor = true;
      this.RB_ImgLayers.CheckedChanged += new System.EventHandler(this.RB_ImgLayers_CheckedChanged);
      // 
      // RB_Borders
      // 
      this.RB_Borders.AutoSize = true;
      this.RB_Borders.Location = new System.Drawing.Point(6, 48);
      this.RB_Borders.Name = "RB_Borders";
      this.RB_Borders.Size = new System.Drawing.Size(79, 21);
      this.RB_Borders.TabIndex = 19;
      this.RB_Borders.TabStop = true;
      this.RB_Borders.Text = "Borders";
      this.RB_Borders.UseVisualStyleBackColor = true;
      this.RB_Borders.CheckedChanged += new System.EventHandler(this.RB_Borders_CheckedChanged);
      // 
      // RB_Bodies
      // 
      this.RB_Bodies.AutoSize = true;
      this.RB_Bodies.Location = new System.Drawing.Point(6, 75);
      this.RB_Bodies.Name = "RB_Bodies";
      this.RB_Bodies.Size = new System.Drawing.Size(72, 21);
      this.RB_Bodies.TabIndex = 20;
      this.RB_Bodies.TabStop = true;
      this.RB_Bodies.Text = "Bodies";
      this.RB_Bodies.UseVisualStyleBackColor = true;
      this.RB_Bodies.CheckedChanged += new System.EventHandler(this.RB_Bodies_CheckedChanged);
      // 
      // BTN_ImgLayers_Right
      // 
      this.BTN_ImgLayers_Right.Location = new System.Drawing.Point(153, 20);
      this.BTN_ImgLayers_Right.Name = "BTN_ImgLayers_Right";
      this.BTN_ImgLayers_Right.Size = new System.Drawing.Size(25, 23);
      this.BTN_ImgLayers_Right.TabIndex = 21;
      this.BTN_ImgLayers_Right.Text = ">";
      this.BTN_ImgLayers_Right.UseVisualStyleBackColor = true;
      this.BTN_ImgLayers_Right.Click += new System.EventHandler(this.BTN_ImgLayers_Right_Click);
      // 
      // BTN_Borders_Right
      // 
      this.BTN_Borders_Right.Location = new System.Drawing.Point(153, 46);
      this.BTN_Borders_Right.Name = "BTN_Borders_Right";
      this.BTN_Borders_Right.Size = new System.Drawing.Size(25, 23);
      this.BTN_Borders_Right.TabIndex = 23;
      this.BTN_Borders_Right.Text = ">";
      this.BTN_Borders_Right.UseVisualStyleBackColor = true;
      this.BTN_Borders_Right.Click += new System.EventHandler(this.BTN_Borders_Right_Click);
      // 
      // BTN_Borders_Left
      // 
      this.BTN_Borders_Left.Location = new System.Drawing.Point(122, 46);
      this.BTN_Borders_Left.Name = "BTN_Borders_Left";
      this.BTN_Borders_Left.Size = new System.Drawing.Size(25, 23);
      this.BTN_Borders_Left.TabIndex = 22;
      this.BTN_Borders_Left.Text = "<";
      this.BTN_Borders_Left.UseVisualStyleBackColor = true;
      this.BTN_Borders_Left.Click += new System.EventHandler(this.BTN_Borders_Left_Click);
      // 
      // BTN_Bodies_Right
      // 
      this.BTN_Bodies_Right.Location = new System.Drawing.Point(153, 73);
      this.BTN_Bodies_Right.Name = "BTN_Bodies_Right";
      this.BTN_Bodies_Right.Size = new System.Drawing.Size(25, 23);
      this.BTN_Bodies_Right.TabIndex = 25;
      this.BTN_Bodies_Right.Text = ">";
      this.BTN_Bodies_Right.UseVisualStyleBackColor = true;
      this.BTN_Bodies_Right.Click += new System.EventHandler(this.BTN_Bodies_Right_Click);
      // 
      // BTN_Bodies_Left
      // 
      this.BTN_Bodies_Left.Location = new System.Drawing.Point(122, 73);
      this.BTN_Bodies_Left.Name = "BTN_Bodies_Left";
      this.BTN_Bodies_Left.Size = new System.Drawing.Size(25, 23);
      this.BTN_Bodies_Left.TabIndex = 24;
      this.BTN_Bodies_Left.Text = "<";
      this.BTN_Bodies_Left.UseVisualStyleBackColor = true;
      this.BTN_Bodies_Left.Click += new System.EventHandler(this.BTN_Bodies_Left_Click);
      // 
      // TB_ImgLayersNum
      // 
      this.TB_ImgLayersNum.Location = new System.Drawing.Point(185, 21);
      this.TB_ImgLayersNum.Name = "TB_ImgLayersNum";
      this.TB_ImgLayersNum.ReadOnly = true;
      this.TB_ImgLayersNum.Size = new System.Drawing.Size(64, 22);
      this.TB_ImgLayersNum.TabIndex = 26;
      // 
      // TB_BodiesNum
      // 
      this.TB_BodiesNum.Location = new System.Drawing.Point(185, 73);
      this.TB_BodiesNum.Name = "TB_BodiesNum";
      this.TB_BodiesNum.ReadOnly = true;
      this.TB_BodiesNum.Size = new System.Drawing.Size(64, 22);
      this.TB_BodiesNum.TabIndex = 27;
      // 
      // TB_BordersNum
      // 
      this.TB_BordersNum.Location = new System.Drawing.Point(185, 47);
      this.TB_BordersNum.Name = "TB_BordersNum";
      this.TB_BordersNum.ReadOnly = true;
      this.TB_BordersNum.Size = new System.Drawing.Size(64, 22);
      this.TB_BordersNum.TabIndex = 28;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.RB_Kuwahara);
      this.groupBox3.Controls.Add(this.CB_ShowCenters_Right);
      this.groupBox3.Controls.Add(this.RB_Colorize);
      this.groupBox3.Controls.Add(this.RB_ImgLayers);
      this.groupBox3.Controls.Add(this.TB_BordersNum);
      this.groupBox3.Controls.Add(this.BTN_ImgLayers_Left);
      this.groupBox3.Controls.Add(this.TB_BodiesNum);
      this.groupBox3.Controls.Add(this.RB_Borders);
      this.groupBox3.Controls.Add(this.TB_ImgLayersNum);
      this.groupBox3.Controls.Add(this.RB_Bodies);
      this.groupBox3.Controls.Add(this.BTN_Bodies_Right);
      this.groupBox3.Controls.Add(this.BTN_ImgLayers_Right);
      this.groupBox3.Controls.Add(this.BTN_Bodies_Left);
      this.groupBox3.Controls.Add(this.BTN_Borders_Left);
      this.groupBox3.Controls.Add(this.BTN_Borders_Right);
      this.groupBox3.Location = new System.Drawing.Point(96, 102);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(296, 155);
      this.groupBox3.TabIndex = 29;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Right Controls";
      // 
      // RB_Kuwahara
      // 
      this.RB_Kuwahara.AutoSize = true;
      this.RB_Kuwahara.Location = new System.Drawing.Point(6, 127);
      this.RB_Kuwahara.Name = "RB_Kuwahara";
      this.RB_Kuwahara.Size = new System.Drawing.Size(130, 21);
      this.RB_Kuwahara.TabIndex = 31;
      this.RB_Kuwahara.TabStop = true;
      this.RB_Kuwahara.Text = "Show Kuwahara";
      this.RB_Kuwahara.UseVisualStyleBackColor = true;
      this.RB_Kuwahara.CheckedChanged += new System.EventHandler(this.RB_Kuwahara_CheckedChanged);
      // 
      // CB_ShowCenters_Right
      // 
      this.CB_ShowCenters_Right.AutoSize = true;
      this.CB_ShowCenters_Right.Checked = true;
      this.CB_ShowCenters_Right.CheckState = System.Windows.Forms.CheckState.Checked;
      this.CB_ShowCenters_Right.Location = new System.Drawing.Point(122, 101);
      this.CB_ShowCenters_Right.Name = "CB_ShowCenters_Right";
      this.CB_ShowCenters_Right.Size = new System.Drawing.Size(117, 21);
      this.CB_ShowCenters_Right.TabIndex = 30;
      this.CB_ShowCenters_Right.Text = "Show Centers";
      this.CB_ShowCenters_Right.UseVisualStyleBackColor = true;
      this.CB_ShowCenters_Right.CheckedChanged += new System.EventHandler(this.CB_ShowCenters_Right_CheckedChanged);
      // 
      // RB_Colorize
      // 
      this.RB_Colorize.AutoSize = true;
      this.RB_Colorize.Location = new System.Drawing.Point(6, 102);
      this.RB_Colorize.Name = "RB_Colorize";
      this.RB_Colorize.Size = new System.Drawing.Size(62, 21);
      this.RB_Colorize.TabIndex = 29;
      this.RB_Colorize.TabStop = true;
      this.RB_Colorize.Text = "Color";
      this.RB_Colorize.UseVisualStyleBackColor = true;
      this.RB_Colorize.CheckedChanged += new System.EventHandler(this.RB_Colorize_CheckedChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(408, 3);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(83, 17);
      this.label2.TabIndex = 30;
      this.label2.Text = "Input Video:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 102);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(52, 17);
      this.label3.TabIndex = 31;
      this.label3.Text = "PTime:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(34, 127);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(0, 17);
      this.label4.TabIndex = 32;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1422, 800);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.LBL_FPS);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.BTN_Start);
      this.Controls.Add(this.BTN_Open);
      this.Controls.Add(this.IB_Heatmap);
      this.Controls.Add(this.IB_Neurons);
      this.Controls.Add(this.IB_Video);
      this.Name = "MainForm";
      this.Text = "Neuron Video Detector";
      ((System.ComponentModel.ISupportInitialize)(this.IB_Video)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.IB_Neurons)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.IB_Heatmap)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Emgu.CV.UI.ImageBox IB_Video;
    private Emgu.CV.UI.ImageBox IB_Neurons;
    private Emgu.CV.UI.ImageBox IB_Heatmap;
    private System.Windows.Forms.Button BTN_Open;
    private System.Windows.Forms.Button BTN_Start;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label LBL_FPS;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox CB_Kuwahara;
    private System.Windows.Forms.CheckBox CB_KillLow;
    private System.Windows.Forms.CheckBox CB_KillNoise;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.CheckBox CB_ShowBodiesUni;
    private System.Windows.Forms.CheckBox CB_ShowBordersUni;
    private System.Windows.Forms.CheckBox CB_ShowCenters_Left;
    private System.Windows.Forms.Button BTN_ImgLayers_Left;
    private System.Windows.Forms.RadioButton RB_ImgLayers;
    private System.Windows.Forms.RadioButton RB_Borders;
    private System.Windows.Forms.RadioButton RB_Bodies;
    private System.Windows.Forms.Button BTN_ImgLayers_Right;
    private System.Windows.Forms.Button BTN_Borders_Right;
    private System.Windows.Forms.Button BTN_Borders_Left;
    private System.Windows.Forms.Button BTN_Bodies_Right;
    private System.Windows.Forms.Button BTN_Bodies_Left;
    private System.Windows.Forms.TextBox TB_ImgLayersNum;
    private System.Windows.Forms.TextBox TB_BodiesNum;
    private System.Windows.Forms.TextBox TB_BordersNum;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.RadioButton RB_Colorize;
    private System.Windows.Forms.CheckBox CB_ShowCenters_Right;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RadioButton RB_Kuwahara;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
  }
}

