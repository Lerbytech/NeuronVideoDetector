using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace NeuronVideoDetector
{
  public partial class MainForm : Form
  {
    public string VideoFile = null;
    public bool DEBUG = true;
    
    private Capture _capture = null;
    bool isPlaying = false; // flag of state. if TRUE then we are playing file, i.e. _capture != null

    FIP_WorkParams FipParams;

    public FluroImageParser FIP;

    public ImagePackage IMAGES;

    Mat retrieveMat; // rgb mat from video
    Mat grayscaleMat;// rgb to grayscale
    Image<Gray, Byte> SourceImg;
    Image<Bgr, Byte> LeftImage;
    Image<Bgr, Byte> RightImage;

    bool ShowNeuronCenters_left;
    bool ShowNeuronCenters_right;

    int N_layers;
    int ImgLayers_curr;
    int BordersLayers_curr;
    int BodiesLayers_curr;

    public MainForm()
    {
      InitializeComponent();

      if (VideoFile != null) StartCapture(VideoFile);

    }

    private void ProcessFrame(object sender, EventArgs arg)
    {
      Stopwatch tt = new Stopwatch();
      List<long> timers = new List<long>();
      string output = "";
      
      if (DEBUG) tt.Restart();

      // capture frame and send to work
      _capture.Retrieve(retrieveMat, 0);
      CvInvoke.CvtColor(retrieveMat, grayscaleMat, ColorConversion.Bgr2Gray);
      SourceImg = grayscaleMat.ToImage<Gray, Byte>();
      /*
      if (DEBUG)
      {
        tt.Stop();
        timers.Add(tt.ElapsedMilliseconds);
        output = output + "Capture>" + tt.ElapsedMilliseconds.ToString() + "|";
        tt.Restart();
      }
       * */
      // interaction with FIP
      FIP.ProcessSingleFrame(SourceImg);
      /*if (DEBUG) 
      {tt.Stop();
      timers.Add(tt.ElapsedMilliseconds);
      output = output + "FIP>" + tt.ElapsedMilliseconds.ToString() + "|";
       tt.Restart();
      }*/

      //Get Images from FIP
      IMAGES = FIP.IMAGES;
      
      N_layers = IMAGES.LayersList.Count;
      /*
      if (DEBUG)
      {
        tt.Stop();
        timers.Add(tt.ElapsedMilliseconds);
        output = output + "get images>" + tt.ElapsedMilliseconds.ToString() + "|";
        tt.Restart();
      }
      */
      #region RENDERER_LEFT
      //LeftImage = new Image<Bgr, byte>(IMAGES.Img.Width, IMAGES.Img.Height, new Bgr(0, 0, 0));
      #region Left
      CvInvoke.InsertChannel(IMAGES.Img, LeftImage, 0);
      if (RB_Bodies.Checked) CvInvoke.InsertChannel(IMAGES.Img + IMAGES.Img_Total_BoolMask[BodiesLayers_curr], LeftImage, 1);
      else CvInvoke.InsertChannel(IMAGES.Img, LeftImage, 1);
      if (RB_Borders.Checked) CvInvoke.InsertChannel(IMAGES.Img + IMAGES.CannyList[BordersLayers_curr], LeftImage, 2);
      CvInvoke.InsertChannel(IMAGES.Img, LeftImage, 2);

      if (CB_ShowCenters_Left.Checked) { }
      #endregion

      #region Right 
      //RightImage = new Image<Bgr, byte>(IMAGES.Img.Width, IMAGES.Img.Height, new Bgr(0, 0, 0));
      if (FipParams.doColorize)
      {
        //RightImage = IMAGES.Img_Color;
      }
      else
      {
        //CvInvoke.InsertChannel(IMAGES.Img, RightImage, 0);
        if (RB_ImgLayers.Checked)
        {
          CvInvoke.InsertChannel(IMAGES.LayersList[ImgLayers_curr], RightImage, 0);
          CvInvoke.InsertChannel(IMAGES.LayersList[ImgLayers_curr], RightImage, 1);
          CvInvoke.InsertChannel(IMAGES.LayersList[ImgLayers_curr], RightImage, 2);
        }
        if (RB_Bodies.Checked) 
        {
          CvInvoke.InsertChannel(IMAGES.Img_Total_BoolMask[BodiesLayers_curr], RightImage, 0);
         //CvInvoke.InsertChannel(IMAGES.Img_Total_BoolMask[BodiesLayers_curr], RightImage, 1);
         //CvInvoke.InsertChannel(IMAGES.Img_Total_BoolMask[BodiesLayers_curr], RightImage, 2);
        }        
        if (RB_Borders.Checked) 
        {
          CvInvoke.InsertChannel(IMAGES.Img + IMAGES.CannyList[BordersLayers_curr], RightImage, 2);
        }
        if (CB_ShowCenters_Right.Checked) { }
      }
      #endregion


      /*
      if (DEBUG)
      {
        tt.Stop();
        timers.Add(tt.ElapsedMilliseconds);
        output = output + "Images>" + tt.ElapsedMilliseconds.ToString() + "|";
        tt.Restart();
      }
       * */
      #endregion

      /*
      if (DEBUG)
      {
        tt.Stop();
        timers.Add(tt.ElapsedMilliseconds);
        tt.Restart();
      }
      */
      tt.Stop();
//      LBL_FPS.Text = tt.ElapsedMilliseconds.ToString(); 
      
     if (LBL_FPS.InvokeRequired) LBL_FPS.Invoke(new Action<string>((s) => LBL_FPS.Text = s), ((int)(1000/tt.ElapsedMilliseconds)).ToString());
     else LBL_FPS.Text = ((int)(1000/tt.ElapsedMilliseconds)).ToString();

     if (label4.InvokeRequired) label4.Invoke(new Action<string>((s) => label4.Text = s),  tt.ElapsedMilliseconds.ToString());
     else label4.Text = tt.ElapsedMilliseconds.ToString();


     if (tt.ElapsedMilliseconds < 40) Thread.Sleep(40 - (int)tt.ElapsedMilliseconds);
     IB_Neurons.Image = LeftImage.Resize(IB_Neurons.Width, IB_Neurons.Height, Inter.Nearest);
     IB_Video.Image = IMAGES.Img.Resize(IB_Video.Width, IB_Video.Height, Inter.Nearest);
     IB_Heatmap.Image = RightImage.Resize(IB_Neurons.Width, IB_Neurons.Height, Inter.Nearest);

      //MessageBox.Show(output.ToString());
    }

    //first time start capture
    public void StartCapture(string path)
    {
      CvInvoke.UseOpenCL = false;

      if (_capture != null) ReleaseData();
      try
      {
        _capture = new Capture(path);
        _capture.ImageGrabbed += ProcessFrame;
        
      }
      catch (NullReferenceException excpt)
      {
        MessageBox.Show(excpt.Message);
      }

      //Prepare logical flags
      FipParams.Reset();
      
      //
      int width = Convert.ToInt32(_capture.GetCaptureProperty(CapProp.FrameWidth));
      int height = Convert.ToInt32(_capture.GetCaptureProperty(CapProp.FrameHeight));
      FIP = new FluroImageParser(width, height);
      IMAGES = new ImagePackage(width, height); 
      InitializeImages(width, height); // forms inner images
      
      //on form images counters
      ImgLayers_curr = 0;
      BordersLayers_curr = 0;
      BodiesLayers_curr = 0;
      N_layers = 0;
      TB_BodiesNum.Text = "0";
      TB_BordersNum.Text = "0";
      TB_ImgLayersNum.Text = "0";

      //do we play video? (_capture != null)
      isPlaying = false;
      ShowNeuronCenters_left = true;
      ShowNeuronCenters_right = true;

      #region Read params from form
      //Filters
      FipParams.doKillNoise = CB_KillNoise.Checked;
      FipParams.doNoLow = CB_KillLow.Checked;
      FipParams.doKuwaharaSmooth = CB_Kuwahara.Checked;

      //Augmentations
      FipParams.doShowCenters = CB_ShowCenters_Left.Checked || CB_ShowCenters_Right.Checked;
      FipParams.doShowBordersUni = CB_ShowBordersUni.Checked;
      FipParams.doShowBodiesUni = CB_ShowBodiesUni.Checked;

      //Right Controls
      FipParams.doChooseImageLayers = RB_ImgLayers.Checked;
      FipParams.doChooseBordersLayer = RB_Borders.Checked;
      FipParams.doChooseBodiesLayer = RB_Bodies.Checked;
      FipParams.doColorize = RB_Colorize.Checked;
      FipParams.doShowKuwahara = RB_Kuwahara.Checked;

      FIP.UpdateProcessingModes(FipParams);
      #endregion
    }
    private void InitializeImages(int x, int y)
    {
      retrieveMat = new Mat(y, x, DepthType.Cv32S, 3); // rgb mat from video
      grayscaleMat = new Mat();// rgb to grayscale
      SourceImg = new Image<Gray, byte>(x, y, new Gray(0));
      LeftImage = new Image<Bgr,byte>(x, y, new Bgr(0,0,0));
      RightImage = new Image<Bgr, byte>(x, y, new Bgr(0, 0, 0));
    }


    #region Capturing routines
    private void Kill()
    {
      if (_capture != null)
      {
        _capture.Stop();
        _capture.Dispose();
      }
      _capture = null;
      isPlaying = false;
      BTN_Start.Text = "Start";
    }
    private void Pause()
    {
      if (_capture != null)
      {
        if (isPlaying) { _capture.Pause(); isPlaying = false; }
      }

      else throw new Exception("_capture not created in pause() ");
    }
    private void Play()
    {
      if (_capture != null)
      { // !isPly = onPause
        if (!isPlaying) { _capture.Start(); isPlaying = true; }
      }
      else throw new Exception("_capture not created in play() ");
    }
    private void ReleaseData()
    {

      try
      {
        Kill();
      }
      catch (Exception)
      {
        throw new Exception("Release data exception");
      }

      isPlaying = false;
      VideoFile = null;
    }
    #endregion

    #region CONTROLS

    private void BTN_Open_Click(object sender, EventArgs e)
    {
      OpenFileDialog OF = new OpenFileDialog();
      OF.Filter = "Video Files|*.avi;*.mp4;*.mpg";

      if (_capture != null && isPlaying)
        Pause();

      ///--------------------------
      if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        if (_capture != null)
            Kill();
              
        StartCapture(OF.FileName);
      }

    }

    private void BTN_Start_Click(object sender, EventArgs e)
    {
      if (_capture != null)
      {
        if (!isPlaying && _capture != null) { Play(); BTN_Start.Text = "Pause"; }
        else { Pause(); BTN_Start.Text = "Play"; }
      }
    }

    private void RB_ImgLayers_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doChooseImageLayers = RB_ImgLayers.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void RB_Borders_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doChooseBordersLayer = RB_Borders.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void RB_Bodies_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doChooseBodiesLayer = RB_Bodies.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void RB_Colorize_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doColorize = RB_Colorize.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void BTN_ImgLayers_Left_Click(object sender, EventArgs e)
    {
      if (ImgLayers_curr > 0) ImgLayers_curr--;
      else ImgLayers_curr = 0;
      TB_ImgLayersNum.Text = ImgLayers_curr.ToString();
    }

    private void BTN_ImgLayers_Right_Click(object sender, EventArgs e)
    {
      if (ImgLayers_curr < N_layers - 1) ImgLayers_curr++;
      else ImgLayers_curr = N_layers - 1;
      TB_ImgLayersNum.Text = ImgLayers_curr.ToString();
    }

    private void BTN_Borders_Left_Click(object sender, EventArgs e)
    {
      if (BordersLayers_curr > 0) BordersLayers_curr--;
      else BordersLayers_curr = 0;
      TB_BordersNum.Text = BordersLayers_curr.ToString();
    }

    private void BTN_Borders_Right_Click(object sender, EventArgs e)
    {
      if (BordersLayers_curr < N_layers - 1) BordersLayers_curr++;
      else BordersLayers_curr = N_layers - 1;
      TB_BordersNum.Text = BordersLayers_curr.ToString();
    }

    private void BTN_Bodies_Left_Click(object sender, EventArgs e)
    {
      if (BodiesLayers_curr > 0) BodiesLayers_curr--;
      else BodiesLayers_curr = 0;
      TB_BodiesNum.Text = BodiesLayers_curr.ToString();
    }

    private void BTN_Bodies_Right_Click(object sender, EventArgs e)
    {
      if (BodiesLayers_curr < N_layers - 1) BodiesLayers_curr++;
      else BodiesLayers_curr = N_layers - 1;
      TB_BodiesNum.Text = BodiesLayers_curr.ToString();
    }

    private void CB_ShowCenters_Right_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doShowCenters = CB_ShowCenters_Right.Checked;
      ShowNeuronCenters_right = true;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void CB_KillNoise_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doKillNoise = CB_KillNoise.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void CB_KillLow_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doNoLow = CB_KillLow.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void CB_Kuwahara_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doKuwaharaSmooth = CB_Kuwahara.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void CB_ShowCenters_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doShowCenters = CB_ShowCenters_Right.Checked;
      ShowNeuronCenters_left = true;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void CB_ShowBorders_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doShowBordersUni = CB_ShowBordersUni.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void CB_ShowBodies_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doShowBodiesUni = CB_ShowBodiesUni.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }

    private void RB_Kuwahara_CheckedChanged(object sender, EventArgs e)
    {
      FipParams.doShowKuwahara = RB_Kuwahara.Checked;
      FIP.UpdateProcessingModes(FipParams);
    }
    #endregion

  }
}
