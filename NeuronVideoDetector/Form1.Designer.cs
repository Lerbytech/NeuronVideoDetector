namespace NeuronVideoDetector
{
  partial class Form1
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
      this.imageBox1 = new Emgu.CV.UI.ImageBox();
      this.imageBox2 = new Emgu.CV.UI.ImageBox();
      this.imageBox3 = new Emgu.CV.UI.ImageBox();
      ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).BeginInit();
      this.SuspendLayout();
      // 
      // imageBox1
      // 
      this.imageBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.imageBox1.Location = new System.Drawing.Point(12, 27);
      this.imageBox1.Name = "imageBox1";
      this.imageBox1.Size = new System.Drawing.Size(208, 190);
      this.imageBox1.TabIndex = 2;
      this.imageBox1.TabStop = false;
      // 
      // imageBox2
      // 
      this.imageBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.imageBox2.Location = new System.Drawing.Point(235, 27);
      this.imageBox2.Name = "imageBox2";
      this.imageBox2.Size = new System.Drawing.Size(208, 190);
      this.imageBox2.TabIndex = 3;
      this.imageBox2.TabStop = false;
      // 
      // imageBox3
      // 
      this.imageBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.imageBox3.Location = new System.Drawing.Point(449, 27);
      this.imageBox3.Name = "imageBox3";
      this.imageBox3.Size = new System.Drawing.Size(208, 190);
      this.imageBox3.TabIndex = 4;
      this.imageBox3.TabStop = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(711, 253);
      this.Controls.Add(this.imageBox3);
      this.Controls.Add(this.imageBox2);
      this.Controls.Add(this.imageBox1);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.imageBox3)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Emgu.CV.UI.ImageBox imageBox1;
    private Emgu.CV.UI.ImageBox imageBox2;
    private Emgu.CV.UI.ImageBox imageBox3;
  }
}

