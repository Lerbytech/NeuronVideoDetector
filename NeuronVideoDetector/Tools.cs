using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace NeuronVideoDetector
{
  public static class Tools
  {
    public static class IO
    {
      /// <summary>
      /// Reads files from given directory and returns list of names. 
      /// </summary>
      /// <param name="inputDir">Directory path</param>
      /// <remarks>It is aupposed that files are already sorted to their names: path/img0001.png</remarks>
      /// <returns>List of file names</returns>
      public static List<string> GetFilesFromDirectory(string inputDir)
      {
        List<string> result = new List<string>();

        string[] files = Directory.GetFiles(inputDir, "*.png");
        string tmp = string.Empty;
        for (int i = 0; i < files.Length; i++)
        {
          result.Add(files[i].Substring(inputDir.Length));
        }

        return result;
      }
    }


    public static class Filters
    {
      private static int WIDTH;
      private static int HEIGHT;
      private static byte[, ,] arr_Data0;
      private static byte[, ,] arr_Data1;
      private static byte[, ,] arr_Data2;
      private static byte[, ,] arr_Data3;
      //private static int N;
      //private static double[] tmp_pixels;
      //private static double avr;
      //private static double deviation;

      /// <summary>
      /// Sigma-rejection algorithm, 1 iteration
      /// </summary>
      /// <param name="arr">array of input images. 4 is ok</param>
      /// <returns>Filtred image, timed as newest one</returns>
      public static Image<Gray, Byte> SigmaReject(Image<Gray, Byte>[] arr)
      {
        if (arr.Length == 1) return arr[0];
        WIDTH = arr[0].Width;
        HEIGHT = arr[0].Height;

        arr_Data0 = arr[0].Data; // rows cols channels
        arr_Data1 = arr[1].Data;
        arr_Data2 = arr[2].Data;
        arr_Data3 = arr[3].Data;
        //byte[, ,] arr_Data4 = arr[4].Data;

        double[] tmp_pixels = new double[4];

        double avr = 0;
        double deviation = 0;

        for (int i = 0; i < HEIGHT; i++) //row
          for (int j = 0; j < WIDTH; j++) //col
          {
            // STEP 1
            tmp_pixels[0] = arr_Data0[i, j, 0];
            tmp_pixels[1] = arr_Data1[i, j, 0];
            tmp_pixels[2] = arr_Data2[i, j, 0];
            tmp_pixels[3] = arr_Data3[i, j, 0];

            avr = Average(tmp_pixels);
            deviation = FindDeviation(tmp_pixels, avr);
            RejectPixels(ref tmp_pixels, avr, deviation);
            avr = Average(tmp_pixels);
            arr_Data0[i, j, 0] = (byte)avr;
          }
        return arr[0];
      }
      /// <summary>
      /// Sigma-rejection algorithm, 2 iterations
      /// </summary>
      /// <param name="arr">array of input images. 4 is ok</param>
      /// <returns>Filtred image, timed as newest one</returns>
      public static Image<Gray, Byte> SigmaReject2(Image<Gray, Byte>[] arr)
      {
        if (arr.Length == 1) return arr[0];
        WIDTH = arr[0].Width;
        HEIGHT = arr[0].Height;

        arr_Data0 = arr[0].Data; // rows cols channels
        arr_Data1 = arr[1].Data;
        arr_Data2 = arr[2].Data;
        arr_Data3 = arr[3].Data;

        double[] tmp_pixels = new double[4];

        double avr = 0;
        double deviation = 0;

        for (int i = 0; i < HEIGHT; i++) //row
          for (int j = 0; j < WIDTH; j++) //col
          {
            // STEP 1
            tmp_pixels[0] = arr_Data0[i, j, 0];
            tmp_pixels[1] = arr_Data1[i, j, 0];
            tmp_pixels[2] = arr_Data2[i, j, 0];
            tmp_pixels[3] = arr_Data3[i, j, 0];

            avr = Average(tmp_pixels);
            deviation = FindDeviation(tmp_pixels, avr);
            RejectPixels(ref tmp_pixels, avr, deviation);
            avr = Average(tmp_pixels);
            //result[i, j] = new Gray(avr);

            //STEP 2
            deviation = FindDeviation(tmp_pixels, avr);
            RejectPixels(ref tmp_pixels, avr, deviation);
            avr = Average(tmp_pixels);
            arr_Data0[i, j, 0] = (byte)avr;
          }

        return arr[0];
      }
      /// <summary>
      /// Uses external Kuwahara filtration from ImageMagick utility
      /// 
      /// </summary>
      /// <param name="From">Path to image</param>
      /// <param name="To">Path to save place</param>
      /// <param name="T">Time of work</param>
      /// <param name="result">Output - image</param>
      public static void ExternalKuwaharaFilter(string From, string To, out long T, out Image<Gray, Byte> result)
      {
        // Use ProcessStartInfo class
        ProcessStartInfo startInfo = new ProcessStartInfo();

        startInfo.CreateNoWindow = false;
        startInfo.UseShellExecute = false;
        startInfo.FileName = @"C:\Windows\System32\cmd.exe";
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.Arguments = "/c convert " + From + " -kuwahara 2 " + To;
        // startInfo.Arguments = @"/c convert C:\\Users\\Admin\\Downloads\\medianImg_39.png -kuwahara 3 C:\\Users\\Admin\\Downloads\\ExternalKuwaharaImg.png";
        //Stopwatch W = new Stopwatch();
        //W.Start();
        try
        {
          // Start the process with the info we specified.
          // Call WaitForExit and then the using statement will close.
          using (Process exeProcess = Process.Start(startInfo))
          {
            exeProcess.WaitForExit();
          }
        }
        catch
        {
          // Log error.
        }
        //W.Stop();
        //T = W.ElapsedMilliseconds;
        T = 0;
        result = new Image<Gray, byte>(To);
      }
      /// <summary>
      /// DIY kuwahara filter. Slow and not isotrophic
      /// </summary>
      /// <param name="src">Src Image</param>
      /// <returns>rOutput</returns>/*
      /// 

      public static Image<Gray, Byte> KuwaharaFilter(Image<Gray, Byte> SRC)
      {
        int W = SRC.Width;
        int H = SRC.Height;

        Image<Gray, Byte> UPPER = new Image<Gray, byte>(W + 4, H + 4, new Gray(0));
        CvInvoke.cvSetImageROI(UPPER, new Rectangle(2, 2, W, H));
        SRC.CopyTo(UPPER);
        CvInvoke.cvResetImageROI(UPPER);

        //AB
        //CD
        float[, ,] DATA_QA = new float[H, W, 1];
        float[, ,] DATA_QB = new float[H, W, 1];
        float[, ,] DATA_QC = new float[H, W, 1];
        float[, ,] DATA_QD = new float[H, W, 1];
        int[,] meanA = new int[H, W];
        int[,] meanB = new int[H, W];
        int[,] meanC = new int[H, W];
        int[,] meanD = new int[H, W];

        Parallel.Invoke(() => calculateQA(UPPER, ref DATA_QA, ref meanA),
                        () => calculateQB(UPPER, ref DATA_QB, ref meanB),
                        () => calculateQC(UPPER, ref DATA_QC, ref meanC),
                        () => calculateQD(UPPER, ref DATA_QD, ref meanD));
        
        return SRC;
  }

      private static void calculateQA(Image<Gray, Byte> INPUT, ref float[, ,] DATA_QA, ref int[,] meanA)
      {
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        meanA = new int[H, W];
        DATA_QA = new float[INPUT.Height, INPUT.Width, 1];
        int SX = 0;
        int SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y - 2, x - 2, 0]; SX2 += DATA_INPUT[y - 2, x - 2, 0] * DATA_INPUT[y - 2, x - 2, 0];
            SX += DATA_INPUT[y - 2, x - 1, 0]; SX2 += DATA_INPUT[y - 2, x - 1, 0] * DATA_INPUT[y - 2, x - 1, 0];
            SX += DATA_INPUT[y - 2, x, 0]; SX2 += DATA_INPUT[y - 2, x, 0] * DATA_INPUT[y - 2, x, 0];
            SX += DATA_INPUT[y - 1, x - 2, 0]; SX2 += DATA_INPUT[y - 1, x - 2, 0] * DATA_INPUT[y - 1, x - 2, 0];
            SX += DATA_INPUT[y - 1, x - 1, 0]; SX2 += DATA_INPUT[y - 1, x - 1, 0] * DATA_INPUT[y - 1, x - 1, 0];
            SX += DATA_INPUT[y - 1, x, 0]; SX2 += DATA_INPUT[y - 1, x, 0] * DATA_INPUT[y - 1, x, 0];
            SX += DATA_INPUT[y, x - 2, 0]; SX2 += DATA_INPUT[y, x - 2, 0] * DATA_INPUT[y, x - 2, 0];
            SX += DATA_INPUT[y, x - 1, 0]; SX2 += DATA_INPUT[y, x - 1, 0] * DATA_INPUT[y, x - 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QA[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
            meanA[y, x] = SX;
          }

      }

      private static void calculateQB(Image<Gray, Byte> INPUT, ref float[, ,] DATA_QB, ref int[,] meanA)
      {
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        DATA_QB = new float[INPUT.Height, INPUT.Width, 1];
        int SX = 0;
        int SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y - 2, x + 2, 0]; SX2 += DATA_INPUT[y - 2, x + 2, 0] * DATA_INPUT[y - 2, x + 2, 0];
            SX += DATA_INPUT[y - 2, x + 1, 0]; SX2 += DATA_INPUT[y - 2, x + 1, 0] * DATA_INPUT[y - 2, x + 1, 0];
            SX += DATA_INPUT[y - 2, x, 0]; SX2 += DATA_INPUT[y - 2, x, 0] * DATA_INPUT[y - 2, x, 0];
            SX += DATA_INPUT[y - 1, x + 2, 0]; SX2 += DATA_INPUT[y - 1, x + 2, 0] * DATA_INPUT[y - 1, x + 2, 0];
            SX += DATA_INPUT[y - 1, x + 1, 0]; SX2 += DATA_INPUT[y - 1, x + 1, 0] * DATA_INPUT[y - 1, x + 1, 0];
            SX += DATA_INPUT[y - 1, x, 0]; SX2 += DATA_INPUT[y - 1, x, 0] * DATA_INPUT[y - 1, x, 0];
            SX += DATA_INPUT[y, x + 2, 0]; SX2 += DATA_INPUT[y, x + 2, 0] * DATA_INPUT[y, x + 2, 0];
            SX += DATA_INPUT[y, x + 1, 0]; SX2 += DATA_INPUT[y, x + 1, 0] * DATA_INPUT[y, x + 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QB[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
          }

      }

      private static void calculateQC(Image<Gray, Byte> INPUT, ref float[, ,] DATA_QC, ref int[,] meanA)
      {
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        DATA_QC = new float[INPUT.Height, INPUT.Width, 1];
        int SX = 0;
        int SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y + 2, x - 2, 0]; SX2 += DATA_INPUT[y + 2, x - 2, 0] * DATA_INPUT[y + 2, x - 2, 0];
            SX += DATA_INPUT[y + 2, x - 1, 0]; SX2 += DATA_INPUT[y + 2, x - 1, 0] * DATA_INPUT[y + 2, x - 1, 0];
            SX += DATA_INPUT[y + 2, x, 0]; SX2 += DATA_INPUT[y + 2, x, 0] * DATA_INPUT[y + 2, x, 0];
            SX += DATA_INPUT[y + 1, x - 2, 0]; SX2 += DATA_INPUT[y + 1, x - 2, 0] * DATA_INPUT[y + 1, x - 2, 0];
            SX += DATA_INPUT[y + 1, x - 1, 0]; SX2 += DATA_INPUT[y + 1, x - 1, 0] * DATA_INPUT[y + 1, x - 1, 0];
            SX += DATA_INPUT[y + 1, x, 0]; SX2 += DATA_INPUT[y + 1, x, 0] * DATA_INPUT[y + 1, x, 0];
            SX += DATA_INPUT[y, x - 2, 0]; SX2 += DATA_INPUT[y, x - 2, 0] * DATA_INPUT[y, x - 2, 0];
            SX += DATA_INPUT[y, x - 1, 0]; SX2 += DATA_INPUT[y, x - 1, 0] * DATA_INPUT[y, x - 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QC[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
          }
      }

      private static void calculateQD(Image<Gray, Byte> INPUT, ref float[, ,] DATA_QD, ref int[,] meanA)
      {
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        DATA_QD = new float[INPUT.Height, INPUT.Width, 1];
        int SX = 0;
        int SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y + 2, x + 2, 0]; SX2 += DATA_INPUT[y + 2, x + 2, 0] * DATA_INPUT[y + 2, x + 2, 0];
            SX += DATA_INPUT[y + 2, x + 1, 0]; SX2 += DATA_INPUT[y + 2, x + 1, 0] * DATA_INPUT[y + 2, x + 1, 0];
            SX += DATA_INPUT[y + 2, x, 0]; SX2 += DATA_INPUT[y + 2, x, 0] * DATA_INPUT[y + 2, x, 0];
            SX += DATA_INPUT[y + 1, x + 2, 0]; SX2 += DATA_INPUT[y + 1, x + 2, 0] * DATA_INPUT[y + 1, x + 2, 0];
            SX += DATA_INPUT[y + 1, x + 1, 0]; SX2 += DATA_INPUT[y + 1, x + 1, 0] * DATA_INPUT[y + 1, x + 1, 0];
            SX += DATA_INPUT[y + 1, x, 0]; SX2 += DATA_INPUT[y + 1, x, 0] * DATA_INPUT[y + 1, x, 0];
            SX += DATA_INPUT[y, x + 2, 0]; SX2 += DATA_INPUT[y, x + 2, 0] * DATA_INPUT[y, x + 2, 0];
            SX += DATA_INPUT[y, x + 1, 0]; SX2 += DATA_INPUT[y, x + 1, 0] * DATA_INPUT[y, x + 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QD[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
          }
      }

      private static float[, ,] calculateQA(Image<Gray, Byte> INPUT)
      {
        /*int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        float[, ,] DATA_QA = new float[INPUT.Height, INPUT.Width, 1];
        float mean = 0.0f;
        float M2 = 0;
        float delta = 0;

        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          { 
            //A
            delta = DATA_INPUT[y - 2, x - 2, 0] - mean; mean += delta; M2 += delta * (DATA_INPUT[y - 2, x - 2, 0] - mean);
            delta = DATA_INPUT[y - 2, x - 1, 0] - mean; mean += delta * 0.5f; M2 += delta * (DATA_INPUT[y - 2, x - 1, 0] - mean);
            delta = DATA_INPUT[y - 2, x, 0] - mean; mean += delta * 0.333333f; M2 += delta * (DATA_INPUT[y - 2, x, 0] - mean);
            delta = DATA_INPUT[y - 1, x - 2, 0] - mean; mean += delta * 0.25f; M2 += delta * (DATA_INPUT[y - 1, x - 2, 0] - mean);
            delta = DATA_INPUT[y - 1, x - 1, 0] - mean; mean += delta * 0.2f; M2 += delta * (DATA_INPUT[y - 1, x - 1, 0] - mean);
            delta = DATA_INPUT[y - 1, x, 0] - mean; mean += delta * 0.166667f; M2 += delta * (DATA_INPUT[y - 1, x, 0] - mean);
            delta = DATA_INPUT[y, x - 2, 0] - mean; mean += delta * 0.142857f; M2 += delta * (DATA_INPUT[y, x - 2, 0] - mean);
            delta = DATA_INPUT[y, x - 1, 0] - mean; mean += delta * 0.125f; M2 += delta * (DATA_INPUT[y, x - 1, 0] - mean);
            delta = DATA_INPUT[y, x, 0] - mean; mean += delta * 0.111111f; M2 += delta * (DATA_INPUT[y, x, 0] - mean);
            DATA_QA[y - 2, x - 2, 0] = M2 * 0.125f;
            mean = 0;
            M2 = 0;
            delta = 0;
          }*/

        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        float[, ,] DATA_QA = new float[INPUT.Height, INPUT.Width, 1];
        float SX = 0;
        float SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y - 2, x - 2, 0]; SX2 += DATA_INPUT[y - 2, x - 2, 0] * DATA_INPUT[y - 2, x - 2, 0];
            SX += DATA_INPUT[y - 2, x - 1, 0]; SX2 += DATA_INPUT[y - 2, x - 1, 0] * DATA_INPUT[y - 2, x - 1, 0];
            SX += DATA_INPUT[y - 2, x, 0]; SX2 += DATA_INPUT[y - 2, x, 0] * DATA_INPUT[y - 2, x, 0];
            SX += DATA_INPUT[y - 1, x - 2, 0]; SX2 += DATA_INPUT[y - 1, x - 2, 0] * DATA_INPUT[y - 1, x - 2, 0];
            SX += DATA_INPUT[y - 1, x - 1, 0]; SX2 += DATA_INPUT[y - 1, x - 1, 0] * DATA_INPUT[y - 1, x - 1, 0];
            SX += DATA_INPUT[y - 1, x, 0]; SX2 += DATA_INPUT[y - 1, x, 0] * DATA_INPUT[y - 1, x, 0];
            SX += DATA_INPUT[y, x - 2, 0]; SX2 += DATA_INPUT[y, x - 2, 0] * DATA_INPUT[y, x - 2, 0];
            SX += DATA_INPUT[y, x - 1, 0]; SX2 += DATA_INPUT[y, x - 1, 0] * DATA_INPUT[y, x - 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QA[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
          }

        return DATA_QA;
      }

      private static float[, ,] calculateQB(Image<Gray, Byte> INPUT)
      {
        /*
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        float[, ,] DATA_QB = new float[INPUT.Height, INPUT.Width, 1];
        float mean = 0.0f;
        float M2 = 0;
        float delta = 0;

        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //B
            delta = DATA_INPUT[y - 2, x + 2, 0] - mean; mean += delta; M2 += delta * (DATA_INPUT[y - 2, x + 2, 0] - mean);
            delta = DATA_INPUT[y - 2, x + 1, 0] - mean; mean += delta * 0.5f; M2 += delta * (DATA_INPUT[y - 2, x + 1, 0] - mean);
            delta = DATA_INPUT[y - 2, x, 0] - mean; mean += delta * 0.333333f; M2 += delta * (DATA_INPUT[y - 2, x, 0] - mean);
            delta = DATA_INPUT[y - 1, x + 2, 0] - mean; mean += delta * 0.25f; M2 += delta * (DATA_INPUT[y - 1, x + 2, 0] - mean);
            delta = DATA_INPUT[y - 1, x + 1, 0] - mean; mean += delta * 0.2f; M2 += delta * (DATA_INPUT[y - 1, x + 1, 0] - mean);
            delta = DATA_INPUT[y - 1, x, 0] - mean; mean += delta * 0.166667f; M2 += delta * (DATA_INPUT[y - 1, x, 0] - mean);
            delta = DATA_INPUT[y, x + 2, 0] - mean; mean += delta * 0.142857f; M2 += delta * (DATA_INPUT[y, x + 2, 0] - mean);
            delta = DATA_INPUT[y, x + 1, 0] - mean; mean += delta * 0.125f; M2 += delta * (DATA_INPUT[y, x + 1, 0] - mean);
            delta = DATA_INPUT[y, x, 0] - mean; mean += delta * 0.111111f; M2 += delta * (DATA_INPUT[y, x, 0] - mean);
            DATA_QB[y, x, 0] = M2 * 0.125f;
            mean = 0;
            M2 = 0;
            delta = 0;
          }

        return DATA_QB;
        */
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        float[, ,] DATA_QB = new float[INPUT.Height, INPUT.Width, 1];
        float SX = 0;
        float SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y - 2, x + 2, 0]; SX2 += DATA_INPUT[y - 2, x + 2, 0] * DATA_INPUT[y - 2, x + 2, 0];
            SX += DATA_INPUT[y - 2, x + 1, 0]; SX2 += DATA_INPUT[y - 2, x + 1, 0] * DATA_INPUT[y - 2, x + 1, 0];
            SX += DATA_INPUT[y - 2, x, 0]; SX2 += DATA_INPUT[y - 2, x, 0] * DATA_INPUT[y - 2, x, 0];
            SX += DATA_INPUT[y - 1, x + 2, 0]; SX2 += DATA_INPUT[y - 1, x + 2, 0] * DATA_INPUT[y - 1, x + 2, 0];
            SX += DATA_INPUT[y - 1, x + 1, 0]; SX2 += DATA_INPUT[y - 1, x + 1, 0] * DATA_INPUT[y - 1, x + 1, 0];
            SX += DATA_INPUT[y - 1, x, 0]; SX2 += DATA_INPUT[y - 1, x, 0] * DATA_INPUT[y - 1, x, 0];
            SX += DATA_INPUT[y, x + 2, 0]; SX2 += DATA_INPUT[y, x + 2, 0] * DATA_INPUT[y, x + 2, 0];
            SX += DATA_INPUT[y, x + 1, 0]; SX2 += DATA_INPUT[y, x + 1, 0] * DATA_INPUT[y, x + 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QB[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
          }

        return DATA_QB;
      }

      private static float[, ,] calculateQC(Image<Gray, Byte> INPUT)
      {/*
      int W = INPUT.Width - 3;
      int H = INPUT.Height - 3;
      byte[, ,] DATA_INPUT = INPUT.Data;
      float[, ,] DATA_QC = new float[INPUT.Height, INPUT.Width, 1];
      float mean = 0.0f;
      float M2 = 0;
      float delta = 0;

      for (int x = 2; x < W; x++)
        for (int y = 2; y < H; y++)
        {
          //С
          delta = DATA_INPUT[y + 2, x - 2, 0] - mean; mean += delta; M2 += delta * (DATA_INPUT[y + 2, x - 2, 0] - mean);
          delta = DATA_INPUT[y + 2, x - 1, 0] - mean; mean += delta * 0.5f; M2 += delta * (DATA_INPUT[y + 2, x - 1, 0] - mean);
          delta = DATA_INPUT[y + 2, x, 0] - mean; mean += delta * 0.333333f; M2 += delta * (DATA_INPUT[y + 2, x, 0] - mean);
          delta = DATA_INPUT[y + 1, x - 2, 0] - mean; mean += delta * 0.25f; M2 += delta * (DATA_INPUT[y + 1, x - 2, 0] - mean);
          delta = DATA_INPUT[y + 1, x - 1, 0] - mean; mean += delta * 0.2f; M2 += delta * (DATA_INPUT[y + 1, x - 1, 0] - mean);
          delta = DATA_INPUT[y + 1, x, 0] - mean; mean += delta * 0.166667f; M2 += delta * (DATA_INPUT[y + 1, x, 0] - mean);
          delta = DATA_INPUT[y, x - 2, 0] - mean; mean += delta * 0.142857f; M2 += delta * (DATA_INPUT[y, x - 2, 0] - mean);
          delta = DATA_INPUT[y, x - 1, 0] - mean; mean += delta * 0.125f; M2 += delta * (DATA_INPUT[y, x - 1, 0] - mean);
          delta = DATA_INPUT[y, x, 0] - mean; mean += delta * 0.111111f; M2 += delta * (DATA_INPUT[y, x, 0] - mean);
          DATA_QC[y, x, 0] = M2 * 0.125f;
          mean = 0;
          M2 = 0;
          delta = 0;
        }

      return DATA_QC;
      */
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        float[, ,] DATA_QC = new float[INPUT.Height, INPUT.Width, 1];
        float SX = 0;
        float SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y + 2, x - 2, 0]; SX2 += DATA_INPUT[y + 2, x - 2, 0] * DATA_INPUT[y + 2, x - 2, 0];
            SX += DATA_INPUT[y + 2, x - 1, 0]; SX2 += DATA_INPUT[y + 2, x - 1, 0] * DATA_INPUT[y + 2, x - 1, 0];
            SX += DATA_INPUT[y + 2, x, 0]; SX2 += DATA_INPUT[y + 2, x, 0] * DATA_INPUT[y + 2, x, 0];
            SX += DATA_INPUT[y + 1, x - 2, 0]; SX2 += DATA_INPUT[y + 1, x - 2, 0] * DATA_INPUT[y + 1, x - 2, 0];
            SX += DATA_INPUT[y + 1, x - 1, 0]; SX2 += DATA_INPUT[y + 1, x - 1, 0] * DATA_INPUT[y + 1, x - 1, 0];
            SX += DATA_INPUT[y + 1, x, 0]; SX2 += DATA_INPUT[y + 1, x, 0] * DATA_INPUT[y + 1, x, 0];
            SX += DATA_INPUT[y, x - 2, 0]; SX2 += DATA_INPUT[y, x - 2, 0] * DATA_INPUT[y, x - 2, 0];
            SX += DATA_INPUT[y, x - 1, 0]; SX2 += DATA_INPUT[y, x - 1, 0] * DATA_INPUT[y, x - 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QC[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
          }

        return DATA_QC;
      }

      private static float[, ,] calculateQD(Image<Gray, Byte> INPUT)
      {
        /*
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        float[, ,] DATA_QD = new float[INPUT.Height, INPUT.Width, 1];
        float mean = 0.0f;
        float M2 = 0;
        float delta = 0;

        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //D
            delta = DATA_INPUT[y + 2, x + 2, 0] - mean; 
            mean += delta; M2 += delta * (DATA_INPUT[y + 2, x + 2, 0] - mean);

            delta = DATA_INPUT[y + 2, x + 1, 0] - mean; 
            mean += delta * 0.5f; M2 += delta * (DATA_INPUT[y + 2, x + 1, 0] - mean);

            delta = DATA_INPUT[y + 2, x, 0] - mean; 
            mean += delta * 0.333333f; M2 += delta * (DATA_INPUT[y + 2, x, 0] - mean);
          
            delta = DATA_INPUT[y + 1, x + 2, 0] - mean; 
            mean += delta * 0.25f; M2 += delta * (DATA_INPUT[y + 1, x + 2, 0] - mean);
          
            delta = DATA_INPUT[y + 1, x + 1, 0] - mean; 
            mean += delta * 0.2f; M2 += delta * (DATA_INPUT[y + 1, x + 1, 0] - mean);

            delta = DATA_INPUT[y + 1, x, 0] - mean; 
            mean += delta * 0.166667f; M2 += delta * (DATA_INPUT[y + 1, x, 0] - mean);
          
            delta = DATA_INPUT[y, x + 2, 0] - mean;
            mean += delta * 0.142857f; M2 += delta * (DATA_INPUT[y, x + 2, 0] - mean);
          
            delta = DATA_INPUT[y, x + 1, 0] - mean;
            mean += delta * 0.125f; M2 += delta * (DATA_INPUT[y, x + 1, 0] - mean);
          
            delta = DATA_INPUT[y, x, 0] - mean;
            mean += delta * 0.111111f; M2 += delta * (DATA_INPUT[y, x, 0] - mean);
          
            DATA_QD[y,x, 0] = M2 * 0.125f;
            mean = 0;
            M2 = 0;
            delta = 0;
          }

        return DATA_QD;
        */
        int W = INPUT.Width - 3;
        int H = INPUT.Height - 3;
        byte[, ,] DATA_INPUT = INPUT.Data;
        float[, ,] DATA_QD = new float[INPUT.Height, INPUT.Width, 1];
        float SX = 0;
        float SX2 = 0;
        for (int x = 2; x < W; x++)
          for (int y = 2; y < H; y++)
          {
            //A
            SX += DATA_INPUT[y + 2, x + 2, 0]; SX2 += DATA_INPUT[y + 2, x + 2, 0] * DATA_INPUT[y + 2, x + 2, 0];
            SX += DATA_INPUT[y + 2, x + 1, 0]; SX2 += DATA_INPUT[y + 2, x + 1, 0] * DATA_INPUT[y + 2, x + 1, 0];
            SX += DATA_INPUT[y + 2, x, 0]; SX2 += DATA_INPUT[y + 2, x, 0] * DATA_INPUT[y + 2, x, 0];
            SX += DATA_INPUT[y + 1, x + 2, 0]; SX2 += DATA_INPUT[y + 1, x + 2, 0] * DATA_INPUT[y + 1, x + 2, 0];
            SX += DATA_INPUT[y + 1, x + 1, 0]; SX2 += DATA_INPUT[y + 1, x + 1, 0] * DATA_INPUT[y + 1, x + 1, 0];
            SX += DATA_INPUT[y + 1, x, 0]; SX2 += DATA_INPUT[y + 1, x, 0] * DATA_INPUT[y + 1, x, 0];
            SX += DATA_INPUT[y, x + 2, 0]; SX2 += DATA_INPUT[y, x + 2, 0] * DATA_INPUT[y, x + 2, 0];
            SX += DATA_INPUT[y, x + 1, 0]; SX2 += DATA_INPUT[y, x + 1, 0] * DATA_INPUT[y, x + 1, 0];
            SX += DATA_INPUT[y, x, 0]; SX2 += DATA_INPUT[y, x, 0] * DATA_INPUT[y, x, 0];
            DATA_QD[y, x, 0] = 0.5f * SX2 - 0.015625f * SX * SX;
          }

        return DATA_QD;
      }


      /*public static Image<Gray, Byte> KuwaharaFilter(Image<Gray, Byte> src)
      {
        Image<Gray, Byte> result = new Image<Gray, Byte>(src.Size);
        double[] Mean = { 0.0, 0.0, 0.0, 0.0 };
        double[] Deviation = { 0.0, 0.0, 0.0, 0.0 };

        double M = 0.0;
        double D = 0.0;
        double tmpM = 0.0;
        double Min = 0.0;
        int n = 0;
        int r = 2;

        Point[] frames = new Point[4];
        frames[0] = new Point(-r, -r);
        frames[1] = new Point(0, -r);
        frames[2] = new Point(-r, 0);
        frames[3] = new Point(0, 0);


        for (int Y = r; Y < src.Height - r; Y++)
          for (int X = r; X < src.Width - r; X++)
          {

            for (int k = 0; k < 4; k++)
            {

              n = 1; M = 0.0; D = 0.0;
              //
              tmpM = M;

              M += (src[Y + frames[k].Y, X + frames[k].X].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y, X + frames[k].X].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X].Intensity - M);
              n++;

              tmpM = M;
              M += (src[Y + frames[k].Y, X + frames[k].X + 1].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X + 1].Intensity - M);
              n++;

              tmpM = M;
              M += (src[Y + frames[k].Y, X + frames[k].X + 2].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X + 2].Intensity - M);
              n++;

              if (r == 3)
              {
                tmpM = M;
                M += (src[Y + frames[k].Y, X + frames[k].X + 3].Intensity - tmpM) / n;
                D += (src[Y + frames[k].Y, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X + 3].Intensity - M);
                n++;
              }
              //-----------------------------------------------------------------
              tmpM = M;
              M += (src[Y + frames[k].Y + 1, X + frames[k].X].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y + 1, X + frames[k].X].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X].Intensity - M);
              n++;

              tmpM = M;
              M += (src[Y + frames[k].Y + 1, X + frames[k].X + 1].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y + 1, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X + 1].Intensity - M);
              n++;

              tmpM = M;
              M += (src[Y + frames[k].Y + 1, X + frames[k].X + 2].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y + 1, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X + 2].Intensity - M);
              n++;

              if (r == 3)
              {
                tmpM = M;
                M += (src[Y + frames[k].Y + 1, X + frames[k].X + 3].Intensity - tmpM) / n;
                D += (src[Y + frames[k].Y + 1, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X + 3].Intensity - M);
                n++;
              }
              //-----------------------------------------------------------------
              tmpM = M;
              M += (src[Y + frames[k].Y + 2, X + frames[k].X].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y + 2, X + frames[k].X].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X].Intensity - M);
              n++;

              tmpM = M;
              M += (src[Y + frames[k].Y + 2, X + frames[k].X + 1].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y + 2, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X + 1].Intensity - M);
              n++;

              tmpM = M;
              M += (src[Y + frames[k].Y + 2, X + frames[k].X + 2].Intensity - tmpM) / n;
              D += (src[Y + frames[k].Y + 2, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X + 2].Intensity - M);
              n++;
              if (r == 3)
              {
                tmpM = M;
                M += (src[Y + frames[k].Y + 2, X + frames[k].X + 3].Intensity - tmpM) / n;
                D += (src[Y + frames[k].Y + 2, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X + 3].Intensity - M);
                n++;
              }
              //-----------------------------------------------------------------
              if (r == 3)
              {
                tmpM = M;
                M += (src[Y + frames[k].Y + 3, X + frames[k].X].Intensity - tmpM) / n;
                D += (src[Y + frames[k].Y + 3, X + frames[k].X].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X].Intensity - M);
                n++;

                tmpM = M;
                M += (src[Y + frames[k].Y + 3, X + frames[k].X + 1].Intensity - tmpM) / n;
                D += (src[Y + frames[k].Y + 3, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X + 1].Intensity - M);
                n++;

                tmpM = M;
                M += (src[Y + frames[k].Y + 3, X + frames[k].X + 2].Intensity - tmpM) / n;
                D += (src[Y + frames[k].Y + 3, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X + 2].Intensity - M);
                n++;

                tmpM = M;
                M += (src[Y + frames[k].Y + 3, X + frames[k].X + 3].Intensity - tmpM) / n;
                D += (src[Y + frames[k].Y + 3, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X + 3].Intensity - M);
                n++;
              }
              Mean[k] = M;
              Deviation[k] = Math.Sqrt(D / (n - 2));
            }

            Min = Deviation[0];
            int P = 0;
            if (Deviation[1] < Min) { Min = Deviation[1]; P = 1; }
            if (Deviation[2] < Min) { Min = Deviation[2]; P = 2; }
            if (Deviation[3] < Min) { Min = Deviation[3]; P = 3; }

            result[Y, X] = new Gray(Mean[P]);
          }

        return result;
      }
      */
      private static double Average(double[] arr)
      {
        double avr = 0.0;
        double n_avr = 0;
        for (int i = 0; i < arr.Length; i++)
        {
          if (arr[i] < 0) continue;
          avr += arr[i];
          n_avr = n_avr + 1;
        }
        if (avr == 0) return avr;
        else return avr / n_avr;
      }
      private static void RejectPixels(ref double[] arr, double avr, double dev)
      {
        for (int i = 0; i < arr.Length; i++)
        {
          if (arr[i] < 0) continue;
          if (Math.Abs(arr[i] - avr) > dev) arr[i] = -1;
        }
      }
      private static double FindDeviation(double[] arr, double avr)
      {
        double result = 0;
        double n = 0;
        for (int i = 0; i < arr.Length; i++)
        {
          if (arr[i] < 0) continue;
          else
          {
            result = result + (avr - arr[i]) * (avr - arr[i]);
            n = n + 1;
          }
        }
        if (result == 0) return 0;
        return Math.Sqrt(result / n);
      }
    }

    public static class Separation
    {
      /// <summary>
      /// Calculates intensity of Median pixel of image
      /// </summary>
      /// <param name="src">Image</param>
      /// <returns>Intensity of Median pixel</returns>
      public static double MedianPixel(Image<Gray, Byte> src)
      {
        //List<byte> L = new List<byte>();
        int[] Hist = new int[256];

        //src = src.ThresholdToZero(new Gray(38));

        int SRC_Width = src.Width;
        int SRC_Height = src.Height;
        byte[, ,] SRC_Data = src.Data;
        byte nullB = (byte)0;
        for (int i = 0; i < SRC_Width; i++)
          for (int j = 0; j < SRC_Height; j++)
            //if (SRC_Data[j, i, 0] != nullB) L.Add(SRC_Data[j, i, 0]);
            if (SRC_Data[j, i, 0] != nullB)
            {
              Hist[SRC_Data[j, i, 0]]++;
              //L.Add(SRC_Data[j, i, 0]);
            }

        int[] Sum = new int[256];
        for (int i = 1; i < 256; i++)
        {
          Sum[i] = Sum[i - 1] + Hist[i];
        }

        //double result = 0;


        if (Sum[255] == 0) return -1;
        int k = Sum[255] / 2;


        // ЗДЕСЬ И НИЖЕ - полная, не заслуживающая доверия херня. проверить в деле ( по формулам выходит, что всегда мажем на 1 единицу яркости как бы ни старались, так как неизвестно как на гистограмме вынуть правильное значениее
        if (Sum[255] % 2 != 0)
        {
          for (int j = 1; j < 255; j++)
            if (Sum[j] > k) return j;
            else continue;
        }
        else
        {
          for (int j = 1; j < 255; j++)
            if (Sum[j] > k) return j;
            else continue;
        }

        return -1;
        // Test
        //L = new List<byte>();
        //L.Add(5);
        //L.Add(8);
        //L.Add(13);  - w/o this. 8 and 11.5
        //L.Add(15);
        //L.Add(17);

        /*
        if (L.Count == 0) return -1;
        L.Sort();
        if (L.Count % 2 != 0) result = L[L.Count / 2];
        else
        {
          int P = (int)(L.Count / 2) - 1;
          int PPP = (int)(L.Count / 2) + 1;
          int LP = L[P];
          int LPP = L[PPP];

          result = L[(L.Count / 2) - 1]; 
          result += L[(L.Count / 2)];
          result /= 2;
        }*/
        //return result;

      }

      /// <summary>
      /// Calculates intensity of separate layers.
      /// </summary>
      /// <param name="src">Input src file</param>
      /// <param name="MinValue"> Lower boundary</param>
      /// <param name="MaxValue"> Upper boundary</param>
      /// <returns></returns>
      public static List<float> CalculateSeparationValues(Image<Gray, Byte> src, int MinValue, out int MaxValue)
      {
        List<float> result = new List<float>();

        result.Add(MinValue);

        //на вход приходит кувахара. но у него есть значения меньше MinValue. Гистограмма считается без учета этих величин
        DenseHistogram DH = new DenseHistogram(256 - MinValue, new RangeF(MinValue, 255));
        DH.Calculate(new Image<Gray, Byte>[] { src }, false, null);
        float[] Hist = new float[256 - MinValue];
        DH.CopyTo(Hist);

        // не считаем все нули справа по гистограмме
        int MaxZeroLimit;
        for (MaxZeroLimit = Hist.Length - 1; Hist[MaxZeroLimit] == 0; MaxZeroLimit--) ; // = 111

        int R = MaxZeroLimit;


        //чтобы не было множества слоев с разницей в 1-2, сливаем в один слой все, что справа по гистограмме, но меньше 10 по интенсивности. 
        //for (R = MaxZeroLimit - 1; Hist[R] < 10; R--) ;
        /*
        Hist = new float[13];
        Hist[0] = 19;
        Hist[1] = 5;
        Hist[2] = 6;
        Hist[3] = 9;
        Hist[4] = 0;
        Hist[5] = 7;
        Hist[6] = 8;
        Hist[7] = 0;
        Hist[8] = 0;
        Hist[9] = 1;
        Hist[10] = 0;
        Hist[11] = 0;
        Hist[12] = 0;
        */

        float[] SumArray = new float[R + 1];
        //R = 12;
        //float[] SumArray = new float[13];
        float s = 0;
        for (int i = R; i >= 0; i--)
        {
          s += Hist[i];
          SumArray[i] = s;
        }

        double Median = Math.Round(SumArray[0] / 2);

        for (int i = 0; i <= R; i++)
        {
          if (Median < SumArray[i]) continue;
          else
          {
            result.Add(i + MinValue - 1);
            Median = Math.Round(SumArray[i] / 2);
            if (Median < 20) break;
          }
        }

        //result.Add(MaxZeroLimit);
        MaxValue = MaxZeroLimit;
        return result;

        //Оптимизация подсчета суммы чисел
        /*float[] Hist2 = new float[10];
        for( int i = 0; i < 10; i++) Hist2[i] = 10 - i;

        float[] SummArray = new float[Hist2.Length];
        SummArray[ Hist2.Length - 1] = Hist2[ Hist2.Length - 1];
        for (int i = Hist2.Length - 2; i >= 0; i--)
        {
          SummArray[i] = SummArray[i + 1] + Hist2[i];
        }*/
        /*

        int L = 0;
        float Sum = 0.0f;

        float MedianPos = 0;
        for (int i = L; i < R; i++)
        {
          Sum += Hist[i];
        }
        MedianPos = (float)Math.Ceiling( 0.5f * Sum);

        float N;
        int j = 0;

        while (R - L > 20)
        {
          N = 0.0f;
          for (j = L; j < R && N <= MedianPos; j++)
          {
            N += Hist[j];
          }
          L = j - 1;
          result.Add(L + MinValue);

          Sum = 0.0f;
          for (int i = L; i < R; i++)
          {
            Sum += Hist[i];
          };
          MedianPos = 0.5f * Sum;
        }

        result.Add(R + MinValue + 1);
        //MaxValue = R + MinValue + 1;
        MaxValue = MaxZeroLimit;
        return result;
         * */
      }

      /// <summary>
      /// Separate image to layers according to separation data and bondaries
      /// </summary>
      /// <param name="src">Input image</param>
      /// <param name="Intensities">List of intensities</param>
      /// <param name="MinValue">Lower boundary</param>
      /// <param name="MaxValue">Upper boundary</param>
      /// <returns></returns>
      public static List<Image<Gray, Byte>> SeparateToLayers(Image<Gray, Byte> src, List<float> Intensities, int MinValue)
      {
        int SRC_Width = src.Width;
        int SRC_Height = src.Height;

        List<Image<Gray, Byte>> result = new List<Image<Gray, byte>>();
        for (int i = 0; i < Intensities.Count; i++) result.Add(new Image<Gray, byte>(SRC_Width, SRC_Height, new Gray(0)));

        float I = 0;
        int N = 0;
        byte[, ,] SRC_Data = src.Data;
        byte[, ,] RES_Data = result[0].Data;

        for (int Y = 0; Y < SRC_Height; Y++)
          for (int X = 0; X < SRC_Width; X++)
          {
            I = SRC_Data[Y, X, 0];
            if (I < MinValue) continue; // optimization

            for (N = 1; N < Intensities.Count; N++)
            {
              if (I >= Intensities[N - 1] && I < Intensities[N]) break;
            }
            RES_Data = result[N - 1].Data;
            RES_Data[Y, X, 0] = (byte)I;
            //result[N - 1][Y, X] = new Gray(I); //заменить на заранее созданный словарь цветов
          }
        return result;
      }


    }
  }
}


















//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Diagnostics;
//using System.Drawing;

//using Emgu.CV;
//using Emgu.CV.CvEnum;
//using Emgu.CV.Structure;
//using Emgu.Util;

//namespace NeuronVideoDetector
//{
//  public static class Tools
//  {
//    public static class IO
//    {
//      /// <summary>
//      /// Reads files from given directory and returns list of names. 
//      /// </summary>
//      /// <param name="inputDir">Directory path</param>
//      /// <remarks>It is aupposed that files are already sorted to their names: path/img0001.png</remarks>
//      /// <returns>List of file names</returns>
//      public static List<string> GetFilesFromDirectory(string inputDir)
//      {
//        List<string> result = new List<string>();

//        string[] files = Directory.GetFiles(inputDir, "*.png");
//        string tmp = string.Empty;
//        for (int i = 0; i < files.Length; i++)
//        {
//          result.Add(files[i].Substring(inputDir.Length));
//        }
        
//        return result;
//      }
//    }

//    public static class Filters
//    {
//      /// <summary>
//      /// Sigma-rejection algorithm, 1 iteration
//      /// </summary>
//      /// <param name="arr">array of input images. 4 is ok</param>
//      /// <returns>Filtred image, timed as newest one</returns>
//      public static Image<Gray, Byte> SigmaReject(Image<Gray, Byte>[] arr)
//      {
//        if (arr.Length == 1) return arr[0];
//        int IMG_Width = arr[0].Width;
//        int IMG_Height = arr[0].Height;

//        Image<Gray, Byte> result = new Image<Gray, byte>(IMG_Width, IMG_Height);

//        int N = arr.Length;
//        double[] tmp_pixels = new double[N];
//        for (int i = 0; i < N; i++) tmp_pixels[i] = 0;
//        double avr = 0;
//        double deviation = 0;

//        for (int i = 0; i < result.Height; i++) //row
//          for (int j = 0; j < result.Width; j++) //col
//          {
//            // STEP 1
//            for (int k = 0; k < N; k++) tmp_pixels[k] = arr[k][i, j].Intensity;

//            avr = Average(tmp_pixels);
//            deviation = FindDeviation(tmp_pixels, avr);
//            RejectPixels(ref tmp_pixels, avr, deviation);
//            avr = Average(tmp_pixels);
//            result[i, j] = new Gray(avr);

//          }
//        return result;
//      }
//      /// <summary>
//      /// Sigma-rejection algorithm, 2 iterations
//      /// </summary>
//      /// <param name="arr">array of input images. 4 is ok</param>
//      /// <returns>Filtred image, timed as newest one</returns>
//      public static Image<Gray, Byte> SigmaReject2(Image<Gray, Byte>[] arr)
//      {
//        if (arr.Length == 1) return arr[0];
//        Image<Gray, Byte> result = new Image<Gray, byte>(arr[0].Width, arr[0].Height);

//        int N = arr.Length;
//        double[] tmp_pixels = new double[N];
//        for (int i = 0; i < N; i++) tmp_pixels[i] = 0;
//        double avr = 0;
//        double deviation = 0;

//        for (int i = 0; i < result.Height; i++) //row
//          for (int j = 0; j < result.Width; j++) //col
//          {
//            // STEP 1
//            for (int k = 0; k < N; k++) tmp_pixels[k] = arr[k][i, j].Intensity;

//            avr = Average(tmp_pixels);
//            deviation = FindDeviation(tmp_pixels, avr);
//            RejectPixels(ref tmp_pixels, avr, deviation);
//            avr = Average(tmp_pixels);
//            //result[i, j] = new Gray(avr);

//            //STEP 2
//            deviation = FindDeviation(tmp_pixels, avr);
//            RejectPixels(ref tmp_pixels, avr, deviation);
//            avr = Average(tmp_pixels);
//            result[i, j] = new Gray(avr);
//          }

//        return result;
//      }
//      /// <summary>
//      /// Uses external Kuwahara filtration from ImageMagick utility
//      /// 
//      /// </summary>
//      /// <param name="From">Path to image</param>
//      /// <param name="To">Path to save place</param>
//      /// <param name="T">Time of work</param>
//      /// <param name="result">Output - image</param>
//      public static void ExternalKuwaharaFilter(string From, string To, out long T, out Image<Gray, Byte> result)
//      {
//        // Use ProcessStartInfo class
//        ProcessStartInfo startInfo = new ProcessStartInfo();

//        startInfo.CreateNoWindow = false;
//        startInfo.UseShellExecute = false;
//        startInfo.FileName = @"C:\Windows\System32\cmd.exe";
//        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
//        startInfo.Arguments = "/c convert " + From + " -kuwahara 2 " + To;
//        // startInfo.Arguments = @"/c convert C:\\Users\\Admin\\Downloads\\medianImg_39.png -kuwahara 3 C:\\Users\\Admin\\Downloads\\ExternalKuwaharaImg.png";
//        //Stopwatch W = new Stopwatch();
//        //W.Start();
//        try
//        {
//          // Start the process with the info we specified.
//          // Call WaitForExit and then the using statement will close.
//          using (Process exeProcess = Process.Start(startInfo))
//          {
//            exeProcess.WaitForExit();
//          }
//        }
//        catch
//        {
//          // Log error.
//        }
//        //W.Stop();
//        //T = W.ElapsedMilliseconds;
//        T = 0;
//        result = new Image<Gray, byte>(To);
//      }
//      /// <summary>
//      /// DIY kuwahara filter. Slow and not isotrophic
//      /// </summary>
//      /// <param name="src">Src Image</param>
//      /// <returns>rOutput</returns>
//      public static Image<Gray, Byte> KuwaharaFilter(Image<Gray, Byte> src)
//      {
//        Image<Gray, Byte> result = new Image<Gray, Byte>(src.Size);
//        double[] Mean = { 0.0, 0.0, 0.0, 0.0 };
//        double[] Deviation = { 0.0, 0.0, 0.0, 0.0 };

//        double M = 0.0;
//        double D = 0.0;
//        double tmpM = 0.0;
//        double Min = 0.0;
//        int n = 0;
//        int r = 2;

//        Point[] frames = new Point[4];
//        frames[0] = new Point(-r, -r);
//        frames[1] = new Point(0, -r);
//        frames[2] = new Point(-r, 0);
//        frames[3] = new Point(0, 0);


//        for (int Y = r; Y < src.Height - r; Y++)
//          for (int X = r; X < src.Width - r; X++)
//          {

//            for (int k = 0; k < 4; k++)
//            {

//              n = 1; M = 0.0; D = 0.0;
//              //
//              tmpM = M;

//              M += (src[Y + frames[k].Y, X + frames[k].X].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y, X + frames[k].X].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X].Intensity - M);
//              n++;

//              tmpM = M;
//              M += (src[Y + frames[k].Y, X + frames[k].X + 1].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X + 1].Intensity - M);
//              n++;

//              tmpM = M;
//              M += (src[Y + frames[k].Y, X + frames[k].X + 2].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X + 2].Intensity - M);
//              n++;

//              if (r == 3)
//              {
//                tmpM = M;
//                M += (src[Y + frames[k].Y, X + frames[k].X + 3].Intensity - tmpM) / n;
//                D += (src[Y + frames[k].Y, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + frames[k].Y, X + frames[k].X + 3].Intensity - M);
//                n++;
//              }
//              //-----------------------------------------------------------------
//              tmpM = M;
//              M += (src[Y + frames[k].Y + 1, X + frames[k].X].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y + 1, X + frames[k].X].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X].Intensity - M);
//              n++;

//              tmpM = M;
//              M += (src[Y + frames[k].Y + 1, X + frames[k].X + 1].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y + 1, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X + 1].Intensity - M);
//              n++;

//              tmpM = M;
//              M += (src[Y + frames[k].Y + 1, X + frames[k].X + 2].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y + 1, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X + 2].Intensity - M);
//              n++;

//              if (r == 3)
//              {
//                tmpM = M;
//                M += (src[Y + frames[k].Y + 1, X + frames[k].X + 3].Intensity - tmpM) / n;
//                D += (src[Y + frames[k].Y + 1, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + 1 + frames[k].Y, X + frames[k].X + 3].Intensity - M);
//                n++;
//              }
//              //-----------------------------------------------------------------
//              tmpM = M;
//              M += (src[Y + frames[k].Y + 2, X + frames[k].X].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y + 2, X + frames[k].X].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X].Intensity - M);
//              n++;

//              tmpM = M;
//              M += (src[Y + frames[k].Y + 2, X + frames[k].X + 1].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y + 2, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X + 1].Intensity - M);
//              n++;

//              tmpM = M;
//              M += (src[Y + frames[k].Y + 2, X + frames[k].X + 2].Intensity - tmpM) / n;
//              D += (src[Y + frames[k].Y + 2, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X + 2].Intensity - M);
//              n++;
//              if (r == 3)
//              {
//                tmpM = M;
//                M += (src[Y + frames[k].Y + 2, X + frames[k].X + 3].Intensity - tmpM) / n;
//                D += (src[Y + frames[k].Y + 2, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + 2 + frames[k].Y, X + frames[k].X + 3].Intensity - M);
//                n++;
//              }
//              //-----------------------------------------------------------------
//              if (r == 3)
//              {
//                tmpM = M;
//                M += (src[Y + frames[k].Y + 3, X + frames[k].X].Intensity - tmpM) / n;
//                D += (src[Y + frames[k].Y + 3, X + frames[k].X].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X].Intensity - M);
//                n++;

//                tmpM = M;
//                M += (src[Y + frames[k].Y + 3, X + frames[k].X + 1].Intensity - tmpM) / n;
//                D += (src[Y + frames[k].Y + 3, X + frames[k].X + 1].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X + 1].Intensity - M);
//                n++;

//                tmpM = M;
//                M += (src[Y + frames[k].Y + 3, X + frames[k].X + 2].Intensity - tmpM) / n;
//                D += (src[Y + frames[k].Y + 3, X + frames[k].X + 2].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X + 2].Intensity - M);
//                n++;

//                tmpM = M;
//                M += (src[Y + frames[k].Y + 3, X + frames[k].X + 3].Intensity - tmpM) / n;
//                D += (src[Y + frames[k].Y + 3, X + frames[k].X + 3].Intensity - tmpM) * (src[Y + 3 + frames[k].Y, X + frames[k].X + 3].Intensity - M);
//                n++;
//              }
//              Mean[k] = M;
//              Deviation[k] = Math.Sqrt(D / (n - 2));
//            }

//            Min = Deviation[0];
//            int P = 0;
//            if (Deviation[1] < Min) { Min = Deviation[1]; P = 1; }
//            if (Deviation[2] < Min) { Min = Deviation[2]; P = 2; }
//            if (Deviation[3] < Min) { Min = Deviation[3]; P = 3; }

//            result[Y, X] = new Gray(Mean[P]);
//          }

//        return result;
//      }
//      private static double Average(double[] arr)
//      {
//        double avr = 0;
//        int n_avr = 0;
//        for (int i = 0; i < arr.Length; i++)
//        {
//          if (arr[i] < 0) continue;
//          avr += arr[i];
//          n_avr++;
//        }
//        if (avr == 0) return avr;
//        else return avr / n_avr;
//      }
//      private static void RejectPixels(ref double[] arr, double avr, double dev)
//      {
//        for (int i = 0; i < arr.Length; i++)
//        {
//          if (arr[i] < 0) continue;
//          if (Math.Abs(arr[i] - avr) > dev) arr[i] = -1;
//        }
//      }
//      private static double FindDeviation(double[] arr, double avr)
//      {
//        double result = 0;
//        int n = 0;
//        for (int i = 0; i < arr.Length; i++)
//        {
//          if (arr[i] < 0) continue;
//          else
//          {
//            result = result + (avr - arr[i]) * (avr - arr[i]);
//            n++;
//          }
//        }
//        if (result == 0) return 0;
//        return Math.Sqrt(result / n);
//      }
//    }

//    public static class Separation
//    {
//      /// <summary>
//      /// Calculates intensity of Median pixel of image
//      /// </summary>
//      /// <param name="src">Image</param>
//      /// <returns>Intensity of Median pixel</returns>
//      public static double MedianPixel(Image<Gray, Byte> src)
//      {
//        List<byte> L = new List<byte>();
//        for (int i = 0; i < src.Width; i++)
//          for (int j = 0; j < src.Height; j++) 
//            if (src[j, i].Intensity != 0) L.Add((byte)src[j, i].Intensity);
//        if (L.Count == 0) return -1;
//        L.Sort();
//        if (L.Count % 2 == 0) return L[L.Count / 2];
//        else return 0.5 * (L[(int)(L.Count / 2) - 1] + L[(int)(L.Count / 2) + 1]);
//      }

//      /// <summary>
//      /// Calculates intensity of separate layers.
//      /// </summary>
//      /// <param name="src">Input src file</param>
//      /// <param name="MinValue"> Lower boundary</param>
//      /// <param name="MaxValue"> Upper boundary</param>
//      /// <returns></returns>
//      public static List<float> CalculateSeparationValues(Image<Gray, Byte> src, int MinValue, out int MaxValue)
//      {
//        List<float> result = new List<float>();
        
//        result.Add(MinValue);

//        //на вход приходит кувахара. но у него есть значения меньше MinValue. Гистограмма считается без учета этих величин
//        DenseHistogram DH = new DenseHistogram(256 - MinValue, new RangeF(MinValue, 255));
//        DH.Calculate(new Image<Gray, Byte>[] { src }, false, null);
//        float[] Hist = new float[256 - MinValue];
//        DH.CopyTo(Hist);

//        // не считаем все нули справа по гистограмме
//        int MaxZeroLimit;
//        for (MaxZeroLimit = Hist.Length - 1; Hist[MaxZeroLimit] == 0; MaxZeroLimit--) ; // = 111
       
//        int R = MaxZeroLimit;
        

//        //чтобы не было множества слоев с разницей в 1-2, сливаем в один слой все, что справа по гистограмме, но меньше 10 по интенсивности. 
//        //for (R = MaxZeroLimit - 1; Hist[R] < 10; R--) ;
//        /*
//        Hist = new float[13];
//        Hist[0] = 19;
//        Hist[1] = 5;
//        Hist[2] = 6;
//        Hist[3] = 9;
//        Hist[4] = 0;
//        Hist[5] = 7;
//        Hist[6] = 8;
//        Hist[7] = 0;
//        Hist[8] = 0;
//        Hist[9] = 1;
//        Hist[10] = 0;
//        Hist[11] = 0;
//        Hist[12] = 0;
//        */
        
//        float[] SumArray = new float[R + 1];
//        //R = 12;
//        //float[] SumArray = new float[13];
//        float s = 0;
//        for (int i = R; i >= 0; i--)
//        {
//          s += Hist[i];
//          SumArray[i] = s; 
//        }
        
//        double Median = Math.Round(SumArray[0] / 2);

//        for (int i = 0; i <= R; i++)
//        {
//          if (Median < SumArray[i]) continue;
//          else
//          {
//            result.Add(i + MinValue - 1);
//            Median = Math.Round(SumArray[i] / 2);
//            if (Median < 20) break;
//          }
//        }

//        //result.Add(MaxZeroLimit);
//        MaxValue = MaxZeroLimit;  
//        return result;

//        //Оптимизация подсчета суммы чисел
//        /*float[] Hist2 = new float[10];
//        for( int i = 0; i < 10; i++) Hist2[i] = 10 - i;

//        float[] SummArray = new float[Hist2.Length];
//        SummArray[ Hist2.Length - 1] = Hist2[ Hist2.Length - 1];
//        for (int i = Hist2.Length - 2; i >= 0; i--)
//        {
//          SummArray[i] = SummArray[i + 1] + Hist2[i];
//        }*/
//        /*

//        int L = 0;
//        float Sum = 0.0f;

//        float MedianPos = 0;
//        for (int i = L; i < R; i++)
//        {
//          Sum += Hist[i];
//        }
//        MedianPos = (float)Math.Ceiling( 0.5f * Sum);

//        float N;
//        int j = 0;

//        while (R - L > 20)
//        {
//          N = 0.0f;
//          for (j = L; j < R && N <= MedianPos; j++)
//          {
//            N += Hist[j];
//          }
//          L = j - 1;
//          result.Add(L + MinValue);

//          Sum = 0.0f;
//          for (int i = L; i < R; i++)
//          {
//            Sum += Hist[i];
//          };
//          MedianPos = 0.5f * Sum;
//        }

//        result.Add(R + MinValue + 1);
//        //MaxValue = R + MinValue + 1;
//        MaxValue = MaxZeroLimit;
//        return result;
//         * */
//      }

//      /// <summary>
//      /// Separate image to layers according to separation data and bondaries
//      /// </summary>
//      /// <param name="src">Input image</param>
//      /// <param name="Intensities">List of intensities</param>
//      /// <param name="MinValue">Lower boundary</param>
//      /// <param name="MaxValue">Upper boundary</param>
//      /// <returns></returns>
//      public static List<Image<Gray, Byte>> SeparateToLayers(Image<Gray, Byte> src, List<float> Intensities, int MinValue)
//      {
//        List<Image<Gray, Byte>> result = new List<Image<Gray, byte>>();
//        for (int i = 0; i < Intensities.Count; i++) result.Add(new Image<Gray, byte>(src.Width, src.Height, new Gray(0)));

//        double I = 0.0f;
//        int N = 0;

//        for (int Y = 0; Y < src.Height; Y++)
//          for (int X = 0; X < src.Width; X++)
//          {
//            I = src[Y, X].Intensity;
//            if (I < MinValue) continue; // optimization

//            for (N = 1; N < Intensities.Count; N++)
//            {
//              if (I >= Intensities[N - 1] && I < Intensities[N]) break;
//            }
            
//            result[N - 1][Y, X] = new Gray(I); //заменить на заранее созданный словарь цветов
//          }
//        return result;
//      }


//    }
//  }
//}