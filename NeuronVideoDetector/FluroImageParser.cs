using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace NeuronVideoDetector
{

  public class FluroImageParser : IDisposable
  {
    public int CurNum { get; private set; }
    public string DATA_ROOT { get; private set; }
    public string DATASET_PATH { get; private set; }
    public string DATASET_PREFIX { get; private set; }
    public string DATASET_IMGTYPE { get; private set; }
    public string OUTPUT_FOLDER_FLTR { get; private set; }
    public string OUTPUT_FOLDER_KUWAHARA { get; private set; }
    public string OUTPUT_FOLDER_SEPARATEDLAYERS { get; private set; }
    public string OUTPUT_FOLDER_COLORISED { get; private set; }
    public string TMP_FOLDER { get; private set; }

    public List<float> separation_values { get; private set; }

    public enum KuwaharaMode { NormalKuwahara = 0, ExternalKuwahara };

    [Flags]
    public enum Processing_SetInput { ProcessRaw = 0, ProcessKuwahara = 2}
    [Flags]
    public enum Processing_SetMode { SetNoNoise = 0, SetNoLow = 2, SetKuwahara = 4 }
    [Flags] 
    public enum Processing_SetGet {  GetLayersList = 0, GetCannyList = 2, GetBoolMaskList = 4, GetCenters = 8, GetKuwahara = 16}

    public ImagePackage IMAGES;
    
    public Image<Gray, Byte> TMP_Img { get; private set; }
    /*
    public Image<Gray, Byte> Img_NoNoise { get; private set; }
    public Image<Gray, Byte> Img_NoLow { get; private set; } // threshold < 39
    public Image<Gray, Byte> Img_Kuwahara { get; private set; }
    public Image<Gray, Byte> Img_ExternalKuwahara; // НЕ СВОЙСТВО!!!
    public List<Image<Gray, Byte>> LayersList { get; private set; } // separated to layers img
    public List<Image<Gray, Byte>> BoolMaskList { get; private set; } // list of black/white mask from each layer
    public Image<Gray, Byte> Img_Total_BoolMask { get; private set; } // merged list by clever way
    public List<Image<Gray, Byte>> CannyList { get; private set; }
    public Image<Gray, Byte> Img_Total_CannyMask { get; private set; } // merged canny by clever way
    public Image<Bgr, Byte> Img_Color { get; private set; }
    */
    public List<Point> NeuronCenters;


    FIP_WorkParams Params;

    public FluroImageParser(int width, int height)
    {
      //Prepare 
      CurNum = 0;
      separation_values = new List<float>();

      #region flags
      Params.Reset();
      /*
      useRawAsInput = true; // false = setKuwaharaAsInput = true
      doKillNoise = true;
      doLow = true;
      doKuwahara = true;
      doColorize = true;
      doDrawBorders = true;
      doFindCenters = true;
      doMaskList = true;
      doSoleKuwahara = false;*/
      #endregion

      #region Images initialisation
      IMAGES = new ImagePackage(width, height);
      TMP_Img = new Image<Gray,byte>(width, height, new Gray(0));
      /*
      Img_NoNoise =  new Image<Gray,byte>(width, height, new Gray(0));
      Img_NoLow  = new Image<Gray,byte>(width, height, new Gray(0));
      Img_Kuwahara = new Image<Gray,byte>(width, height, new Gray(0));
      Img_ExternalKuwahara = new Image<Gray,byte>(width, height, new Gray(0));
      LayersList = new List<Image<Gray,byte>>();
      BoolMaskList = new List<Image<Gray, byte>>();
      Img_Total_BoolMask = new Image<Gray,byte>(width, height, new Gray(0));
      CannyList = new List<Image<Gray,byte>>();
      Img_Total_CannyMask = new Image<Gray,byte>(width, height, new Gray(0));
      Img_Color = new Image<Bgr, byte>(width, height, new Bgr(0,0,0));
       * */
      NeuronCenters = new List<Point>();
      #endregion

      #region folder stuff
      DirectoryInfo CurDirInfo = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
      DATA_ROOT = CurDirInfo.Parent.Parent.Parent.Parent.FullName + "\\NeuronVideoDetectorFolders\\";

      DATASET_PATH = DATA_ROOT + "RawInput";
      DATASET_PREFIX = "";
      DATASET_IMGTYPE = ".png";
      OUTPUT_FOLDER_FLTR = DATA_ROOT + "FilteredOutput\\";
      OUTPUT_FOLDER_KUWAHARA = DATA_ROOT + "Kuwahara\\";
      OUTPUT_FOLDER_SEPARATEDLAYERS = DATA_ROOT + "SeparatedLayers\\";
      OUTPUT_FOLDER_COLORISED = DATA_ROOT + "Colorised\\";
      TMP_FOLDER = DATA_ROOT + "TMP\\";

      if ( !Directory.Exists(OUTPUT_FOLDER_COLORISED)) Directory.CreateDirectory(OUTPUT_FOLDER_COLORISED);
      if ( !Directory.Exists(OUTPUT_FOLDER_FLTR)) Directory.CreateDirectory(OUTPUT_FOLDER_FLTR);
      if ( !Directory.Exists(OUTPUT_FOLDER_KUWAHARA)) Directory.CreateDirectory(OUTPUT_FOLDER_KUWAHARA);
      if ( !Directory.Exists(OUTPUT_FOLDER_SEPARATEDLAYERS)) Directory.CreateDirectory(OUTPUT_FOLDER_SEPARATEDLAYERS);
      #endregion
    }

    public void Dispose()
    {
    }
/*
    public void UpdateProcessingModes(Processing_SetInput Pm)
    {
      if (Pm == Processing_SetInput.ProcessRaw) useRawAsInput = true;
      else
      {
        doKuwahara = false;
        useRawAsInput = false;
      }
    }

    public void UpdateProcessingModes(Processing_SetMode Pm)
    {
      if (Pm == Processing_SetMode.SetKuwahara)
      {
        if (!useRawAsInput) doKuwahara = false;
        else doKuwahara = !doKuwahara;
      }
      if (Pm == Processing_SetMode.SetNoLow) doLow = !doLow;
      if (Pm == Processing_SetMode.SetNoNoise) doKillNoise = !doKillNoise;
    }

    public void UpdateProcessingModes(Processing_SetGet Pm)
    {
      if (Pm == Processing_SetGet.GetLayersList) doFindCenters =! doFindCenters;
      if (Pm == Processing_SetGet.GetCannyList) doDrawBorders =! doDrawBorders;
      if (Pm == Processing_SetGet.GetBoolMaskList) doMaskList =! doMaskList;
      if (Pm == Processing_SetGet.GetCenters) doFindCenters =! doFindCenters;
      if (Pm == Processing_SetGet.GetKuwahara && !useRawAsInput) doSoleKuwahara = !doSoleKuwahara;
      else doSoleKuwahara = false;
    }
    */
    public void UpdateProcessingModes(FIP_WorkParams input)
    {
      
      Params = input;
      //if (input.doColorize) Params.doKuwaharaSmooth = true;
      //if (input.doShowCenters_L || input.doShowCenters_R) Params.doChooseBordersLayer = Params.doChooseBodiesLayer = Params.
    }

    public FIP_WorkParams GetFIPParameters()
    {
      return Params;
    }

    private bool PerformCanny()
    {
      return (Params.doShowBordersUni || Params.doChooseBordersLayer);
    }

    private bool PerformBodies()
    {
      return (Params.doShowBodiesUni || Params.doShowBordersUni);
    }

    /*
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

      if (K == KuwaharaMode.NormalKuwahara) Tools.Filters.KuwaharaFilter(medianImg);
      else if (K == KuwaharaMode.ExternalKuwahara)
      {
        //medianImg.Save(FIP.TMP_FOLDER + "medianImg_" + Threshold_median.ToString() + ".png");
        try
        {
          Tools.Filters.ExternalKuwaharaFilter(TMP_FOLDER + "medianImg_" + Threshold_median.ToString() + ".png",
                                                TMP_FOLDER + "ExternalKuwaharaImg.png",
                                                out T, out ExternalKuwaharaImg);
        }
        catch (Exception ex)
        {
          //MessageBox.Show(ex.Message);
        }
      }

      List<float> separation = Tools.Separation.CalculateSeparationValues(ExternalKuwaharaImg, (int)Threshold_median, out MaxValue); // 2 ms FUCK YEAH!!!!!!

      List<Image<Gray, Byte>> layers = new List<Image<Gray, byte>>();

      layers = Tools.Separation.SeparateToLayers(ExternalKuwaharaImg, separation, (int)Threshold_median); // 5-9 ms
      /*
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
*/
    public void ProcessSingleFrame(Image<Gray, Byte> input)
    {
       // Total time: 120-130 ms
      // Kuwahara: 90-95ms
      double Threshold_median = 0;
      //tmp = new Image<Gray, Byte>(width, height, new Gray(0));
      IMAGES.Img = input;

      if (Params.doKillNoise && Params.doNoLow)
      {
        IMAGES.Img_NoNoise = Denoise(input);
        Threshold_median = Tools.Separation.MedianPixel(IMAGES.Img_NoNoise);
        IMAGES.Img_NoLow = IMAGES.Img_NoNoise.ThresholdToZero(new Gray(Threshold_median));
        TMP_Img = IMAGES.Img_NoLow; 
      }
      else if (Params.doKillNoise)
        {
          IMAGES.Img_NoNoise = Denoise(input);
          TMP_Img = IMAGES.Img_NoNoise;
          IMAGES.Img_NoLow = null;
        }
        else if (Params.doNoLow)
        {
          //Reject low values  
          Threshold_median = Tools.Separation.MedianPixel(input);
          IMAGES.Img_NoLow = input.ThresholdToZero(new Gray(Threshold_median)); //2ms
          TMP_Img = IMAGES.Img_NoLow;
          IMAGES.Img_NoNoise = null;
        }

      long T; // work time of externalKuwahara
      int MaxValue = 255;

      if (Params.doKuwaharaSmooth)
      {
        //Kuwahara

        // читы
        KuwaharaMode K = KuwaharaMode.NormalKuwahara;
        //

        if (K == KuwaharaMode.NormalKuwahara)
        {
          IMAGES.Img_Kuwahara  = Tools.Filters.KuwaharaFilter(TMP_Img);
          /*
          float[,] matrixKerA = new float[,] { { 2,  1,  0}, 
                                              { 1,  1, -1}, 
                                              { 0, -1, -2}};
          ConvolutionKernelF KA = new ConvolutionKernelF(matrixKerA, new Point(1, 1));
          Image<Gray, float> src1 = TMP_Img.Convert<Gray, float>();
          src1 = src1.Convolution(KA);
           
          TMP_Img = src1.Convert<Gray, Byte>();
           * */
          TMP_Img = IMAGES.Img_Kuwahara;
        }
        else if (K == KuwaharaMode.ExternalKuwahara)
        {
          TMP_Img.Save(TMP_FOLDER + "medianImg_" + Threshold_median.ToString() + ".png");
          try
          {
            Tools.Filters.ExternalKuwaharaFilter(TMP_FOLDER + "medianImg_" + Threshold_median.ToString() + ".png",
                                                  TMP_FOLDER + "ExternalKuwaharaImg.png",
                                                  out T, out IMAGES.Img_Kuwahara);
          }
          catch (Exception ex)
          {
            //MessageBox.Show(ex.Message);
          }
          TMP_Img = IMAGES.Img_Kuwahara;
        }
      }
      //elseIMAGES.Img_Kuwahara = null;
      // Separation of image to layers according to values

      if (Params.doChooseImageLayers)
      {
       
        IMAGES.LayersList = Tools.Separation.SeparateToLayers(TMP_Img, separation_values, (int)Threshold_median); // 5-9 ms
      }
      //elseIMAGES.LayersList = null;

      // Canny Images
      if (PerformCanny()) IMAGES.CannyList = CannyImages(IMAGES.LayersList);
      //elseIMAGES.CannyList = null;
      //Bodies
      if (PerformBodies())
      {
        IMAGES.BoolMaskList = new List<Image<Gray, byte>>();
        // Mask
        for (int i = 0; i < IMAGES.LayersList.Count; i++)
        {
          IMAGES.BoolMaskList.Add(IMAGES.LayersList[i].ThresholdBinary(new Gray(2), new Gray(255)));
        }
      }
      //elseIMAGES.BoolMaskList = null;
      //if (doFindCenters) ;
      //if (doColorize) ;
    }

    public Image<Gray, Byte> Denoise(Image<Gray, Byte> new_img)
    {
      return new_img;
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

    #region List-for functions
    
    public List<Image<Gray, Byte>> CannyImages(List<Image<Gray, Byte>> input)
    {
      List<Image<Gray, Byte>> cannyIm = new List<Image<Gray, byte>>();

      for (int i = 0; i < input.Count; i++)
      {
        cannyIm.Add(input[i].Canny(5, 255));
        //if (DEBUG) cannyIm[i].Save(TMP_FOLDER + "canny_0" + i.ToString() + DATASET_IMGTYPE);
      }
      return cannyIm;
    }
    
    public Image<Gray, Byte> MergeLayers(List<Image<Gray, Byte>> input)
    {
      Image<Gray, Byte> result = new Image<Gray, byte>(input[0].Size);

      double color = 0;
      double _N = 1.0 / input.Count;
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


    public void MorphologyLayers(List<Image<Gray, Byte>> input)
    {
      foreach (var Im in input)
      {
        Im.Dilate(1);
        Im.Erode(1);
      }
    }
    #endregion


  }
}

