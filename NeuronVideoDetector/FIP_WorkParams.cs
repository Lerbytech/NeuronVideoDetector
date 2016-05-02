using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuronVideoDetector
{
  public struct FIP_WorkParams
  {
    //Filters
    public bool doKillNoise;
    public bool doNoLow;
    public bool doKuwaharaSmooth;
      
      //Augmentations
    public bool doShowCenters;  // show bodies centers on left
    public bool doShowBordersUni;
    public bool doShowBodiesUni;

      //Right Controls
    public bool doChooseImageLayers;
    public bool doChooseBordersLayer;
    public bool doChooseBodiesLayer;
    public bool doColorize;
    public bool doShowKuwahara;
    
    public void Reset()
    {
      //Filters
      doKillNoise = true;
      doNoLow = true; 
      doKuwaharaSmooth = true;
      
      //Augmentations
      doShowCenters = true;
      doShowBordersUni = true;
      doShowBodiesUni = true;

      //Right Controls
      doChooseImageLayers = true;
      doChooseBordersLayer = true;
      doChooseBodiesLayer = true;
      doColorize = false;
      doShowKuwahara = false;
    }



  }
}
