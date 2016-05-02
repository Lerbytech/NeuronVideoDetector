using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace NeuronVideoDetector
{
  public struct ImagePackage
  {
    public int Width;
    public int Height;
    //Image<Gray, Byte> Img; // img from grayscale mat
    public Image<Gray, Byte> Img;
    public Image<Gray, Byte> Img_NoNoise; 
    public Image<Gray, Byte> Img_NoLow; // threshold < 39
    public Image<Gray, Byte> Img_Kuwahara;

    public List<Image<Gray, Byte>> LayersList; // separated to layers img

    public List<Image<Gray, Byte>> BoolMaskList; // list of black/white mask from each layer
    public Image<Gray, Byte> Img_Total_BoolMask;// merged list by clever way

    public List<Image<Gray, Byte>> CannyList;
    public Image<Gray, Byte> Img_Total_CannyMask; // merged canny by clever way

    public ImagePackage(int width, int height)
    {
      this.Width = width;
      this.Height = height;
      Img = new Image<Gray, byte>(width, height, new Gray(0));
      Img_NoNoise = new Image<Gray, byte>(width, height, new Gray(0));
      Img_NoLow = new Image<Gray, byte>(width, height, new Gray(0)); // threshold < 39
      Img_Kuwahara = new Image<Gray, byte>(width, height, new Gray(0));
      Img_Total_BoolMask = new Image<Gray, byte>(width, height, new Gray(0)); // merged list by clever way
      Img_Total_CannyMask = new Image<Gray, byte>(width, height, new Gray(0)); // merged canny by clever way

      CannyList = new List<Image<Gray, byte>>();
      LayersList = new List<Image<Gray, byte>>(); // separated to layers img
      BoolMaskList = new List<Image<Gray, byte>>(); // list of black/white mask from each layer
      CannyList = new List<Image<Gray, byte>>();
    }
  }
}
