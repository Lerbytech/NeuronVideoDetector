using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace NeuronVideoDetector
{
  public partial class Form1 : Form
  {
    #region GLOBAL VARIABLES
    public string DATA_ROOT = @"C:\Users\Admin\Desktop\Антон\NeuronVideoDetectorFolders\";
    public string DATASET_PATH = @"RawInput\";
    public string DATASET_PREFIX = @"";
    public string DATASET_IMGTYPE = @".png";
    public string OUTPUT_FOLDER_FLTR = @"FilteredOutput\";
    public string OUTPUT_FOLDER_KUWAHARA = @"Kuwahara\";
    public string OUTPUT_FOLDER_SEPARATEDLAYERS = @"SeparatedLayers\";
    public string OUTPUT_FOLDER_COLORISED = @"Colorised\";
    public string TMP_FOLDER = @"TMP\";

    public bool DEBUG = true;

    public enum KuwaharaMode { NormalKuwahara = 0, ExternalKuwahara };
    #endregion

    public Form1()
    {
      InitializeComponent();
      
      Image<Gray, Byte> img = new Image<Gray,byte>(@"C:\Users\Admin\Desktop\Антон\NeuronVideoDetectorFolders\FilteredOutput\0.png");
     
      List<Image<Gray, Byte>> N_layers = new List<Image<Gray, byte>>();      
      
      N_layers = ProcessSingleFrame(img, KuwaharaMode.ExternalKuwahara, 0);
      List<Image<Gray, Byte>> N_Borders = CannyImages(N_layers);

      Image<Bgr, Byte> bckg = new Image<Bgr, Byte>(img.Size);
      Image<Bgr, Byte> color = new Image<Bgr, Byte>(img.Size);
      

      for (int i = 0; i < N_Borders.Count; i++)
      {
        CvInvoke.InsertChannel(N_Borders[i], color, 2);
        CvInvoke.AddWeighted(color, 1.0, img.Convert<Bgr, Byte>(), 1.0, 0, bckg);
        bckg.Resize(2.5, Inter.Area).Save(DATA_ROOT + TMP_FOLDER + "detecterd areas_0" + i.ToString() + DATASET_IMGTYPE);
      }


      ProcessLayers(N_layers);

      Image<Gray, Byte> merged = MergeLayers(N_layers);

      merged.Save(DATA_ROOT + TMP_FOLDER + "merged.png");
      for (int i = 0; i < N_layers.Count; i++) N_layers[i].Save(DATA_ROOT + TMP_FOLDER + "layer0" + i.ToString() + ".png");

    }

    public List<Image<Gray, Byte>> ProcessSingleFrame(Image<Gray, Byte> input, KuwaharaMode K, int number)
    {
      // Total time: 120-130 ms
      // Kuwahara: 90-95ms
    
      Image<Gray, Byte> medianImg = new Image<Gray, byte>(input.Size);
      Image<Gray, Byte> KuwaharaImg = new Image<Gray, byte>(input.Size);
      Image<Gray, Byte> ExternalKuwaharaImg = new Image<Gray, byte>(input.Size);

     
      double Threshold_median = Tools.Separation.MedianPixel(input); //7 ms
      medianImg = input.ThresholdToZero(new Gray(Threshold_median)); //2ms

      long T; // work time of externalKuwahara
      int MaxValue;

      
      
      if (K == KuwaharaMode.NormalKuwahara) ;
      else if (K == KuwaharaMode.ExternalKuwahara)
      {
        medianImg.Save(DATA_ROOT + TMP_FOLDER + "medianImg_" + Threshold_median.ToString() + ".png");
        try
        {

          Tools.Filters.ExternalKuwaharaFilter(DATA_ROOT + TMP_FOLDER + "medianImg_" + Threshold_median.ToString() + ".png",
                                                DATA_ROOT + TMP_FOLDER + "ExternalKuwaharaImg.png",
                                                out T, out ExternalKuwaharaImg);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }

     
      List<float> separation = Tools.Separation.CalculateSeparationValues(ExternalKuwaharaImg, (int)Threshold_median, out MaxValue); // 2 ms FUCK YEAH!!!!!!

      List<Image<Gray, Byte>> layers = new List<Image<Gray, byte>>();

      layers = Tools.Separation.SeparateToLayers(ExternalKuwaharaImg, separation, (int)Threshold_median); // 5-9 ms

      if (DEBUG)
      {
        ExternalKuwaharaImg.Save(DATA_ROOT + OUTPUT_FOLDER_KUWAHARA + DATASET_PREFIX + number.ToString() + DATASET_IMGTYPE);
        int prev = (int)Threshold_median;
        for (int i = 0; i < layers.Count; i++)
        {
          layers[i].Save(DATA_ROOT + TMP_FOLDER + DATASET_PREFIX + number.ToString() + "_" + prev + "_" + separation[i] + DATASET_IMGTYPE);
          prev = (int)separation[i];

          if (i >= 10) layers[i].Save(DATA_ROOT + TMP_FOLDER + DATASET_PREFIX + number.ToString() + "_0" + i.ToString() + DATASET_IMGTYPE);
          else layers[i].Save(DATA_ROOT + TMP_FOLDER + DATASET_PREFIX + number.ToString() + "_00" + i.ToString() + DATASET_IMGTYPE);
        }
      }
  

      return layers;
    }

    public void ProcessLayers(List<Image<Gray, Byte>> input)
    {
      foreach (var Im in input)
      {
        Im.Dilate(1);
        Im.Erode(1);
      }
    }

    public Image<Gray, Byte> MergeLayers(List<Image<Gray, Byte>> input)
    {
      Image<Gray, Byte> result = new Image<Gray,byte>(input[0].Size);
      
      double color = 0;
      double _N = 1.0/input.Count;
      int i = 0;
      for (int Y = 0; Y < result.Height; Y++)
          for (int X = 0; X < result.Width; X++)
          {
            for (i = 0; i < input.Count; i++)
            {
              color += input[i][Y, X].Intensity;
            }
            result[Y, X] = new Gray(color * _N);
            color = 0;
          }

      return result;
    }

    public Image<Bgr, Byte> Colorize(Image<Gray, Byte> input)
    {
      Image<Bgr, Byte> result = new Image<Bgr, byte>(input.Size);
      Image<Bgr, Byte> colorMap = new Image<Bgr, byte>(DATA_ROOT + "colorMap.png");

      DenseHistogram DH = new DenseHistogram(256, new RangeF(0, 255));
      DH.Calculate(new Image<Gray, Byte>[] { input }, false, null);
      float[] Hist = new float[256];
      DH.CopyTo(Hist);

      //Dictionary<int, 
      //find discretization for not null values of hisstogram and fill dictionary with colors


      
      return result;
    }

    public List<Image<Gray, Byte>> CannyImages(List<Image<Gray, Byte>> input)
    {
      List<Image<Gray, Byte>> cannyIm = new List<Image<Gray, byte>>();
    
      for (int i = 0; i < input.Count; i++)
      {
        cannyIm.Add(input[i].Canny(5, 255));
        if (DEBUG) cannyIm[i].Save(DATA_ROOT + TMP_FOLDER + "canny_0"  + i.ToString() + DATASET_IMGTYPE);
      }
      return cannyIm;
    }

  }
}
