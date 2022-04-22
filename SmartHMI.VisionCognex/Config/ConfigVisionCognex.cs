using System;

namespace Kuka.FlexDrill.SmartHMI.VisionCognex.ConfigVisionCognex
{
   [Serializable]
   public class ConfigVisionCognex
   {
   
      public ConfigVisionCognex ()
      {
      IpAddress = "192.168.0.1";
      UserName = "admin";
      SavePicturePath = @"D:\FlexDrill\Pictures\";
      SaveJobPath = @"D:\FlexDrill\VisionProgram\";
      
      KrlVarCamIsConnected = "CamIsConnected";
      KrlVarOpenPlugIn = "OpenCamPlugIn";
      KrlVarSavePicture = "SaveCognexPicture";
      KrlVarDisplayFullTab = "DisplayFullTab";
      KrlVarContinuousProcessing = "ContinuousProcessing";
      
      KrlVarCalibVision = "CalibrateVision";
      KrlVarCalibVisionOk = "VisionCalibrationOK";
      KrlVarCalibNormality = "CalibrateVisionNormality";
      KrlVarCalibNormalityOk = "VisionNormalityCalibOK";
      
      KrlVarFunctionCode = "FunctionCode";
      KrlVarClamped = "Clamped";
      KrlVarPatternCode = "PatternCode";
      KrlVarExposureRatio = "ExposureRatio";
      KrlVarCurrentPointName = "CurrentPointName";
      
      KrlVarReturnCode = "ReturnCode";
      KrlVarResultI4 = "ResultI4";
      KrlVarResultI5 = "ResultI5";
      KrlVarResultI6 = "ResultI6";
      KrlVarResultI8 = "ResultI8";
      KrlVarResultG3 = "ResultG3";
      KrlVarResultG4 = "ResultG4";
      KrlVarImageName = "ImageName[]";
      KrlVarResultZ3 = "ResultZ3";
      KrlVarResultZ4 = "ResultZ4";
      KrlVarResultX3 = "ResultX3";
      KrlVarResultY4 = "ResultY4";
      KrlVarResultX5 = "ResultX5";
      KrlVarResultY5 = "ResultY5";
      }
   
      //[System.Xml.Serialization.XmlAttribute()]
      public string IpAddress { get; set; }
      public string UserName { get; set; }
      public string SavePicturePath { get; set; }
      public string SaveJobPath { get; set; }
   
      public string KrlVarFunctionCode { get; set; }
      public string KrlVarOpenPlugIn { get; set; }
      public string KrlVarSavePicture { get; set; }
   
      public string KrlVarDisplayFullTab { get; set; }

      public string KrlVarContinuousProcessing { get; set; }


      public string KrlVarCalibVision { get; set; }
      public string KrlVarCalibVisionOk { get; set; }
      public string KrlVarCalibNormality { get; set; }
      public string KrlVarCalibNormalityOk { get; set; }
   
   
      public string KrlVarCamIsConnected { get; set; }
      public string KrlVarClamped { get; set; }
      public string KrlVarPatternCode { get; set; }
      public string KrlVarExposureRatio { get; set; }
      public string KrlVarCurrentPointName { get; set; }
   
      public string KrlVarCalibVisionRunning { get; set; }
      public string KrlVarCalibVisionNormalityRunning { get; set; }
   
      public string KrlVarReturnCode { get; set; }
      public string KrlVarResultI4 { get; set; }
      public string KrlVarResultI5 { get; set; }
      public string KrlVarResultI6 { get; set; }
      public string KrlVarResultI8 { get; set; }
      public string KrlVarResultG3 { get; set; }
      public string KrlVarResultG4 { get; set; }
      public string KrlVarImageName { get; set; }
      public string KrlVarResultZ3 { get; set; }
      public string KrlVarResultZ4 { get; set; }
      public string KrlVarResultX3 { get; set; }
      public string KrlVarResultY4 { get; set; }
      public string KrlVarResultX5 { get; set; }
      public string KrlVarResultY5 { get; set; }
   
      public static ConfigVisionCognex Read()
      {
         var folder = System.IO.Path.GetDirectoryName(typeof(ConfigVisionCognex).Assembly.Location);
         var path = System.IO.Path.Combine(folder, "ConfigVisionCognex.xml");
   
         ConfigVisionCognex result=null;
   
         try
         {
            result = KukaRoboter.CoreUtil.IO.XmlSerializerHelper.DeserializeFromFile<ConfigVisionCognex>(path);  
         }
         catch (Exception)
         {
             
         }
   
         if (result == null)
         {
            result = new ConfigVisionCognex();
            Write(result, path);
         }
          
          return result;
      }
      private static void Write(ConfigVisionCognex instance,string path)
      {
         KukaRoboter.CoreUtil.IO.XmlSerializerHelper.SerializeToFile<ConfigVisionCognex>(instance, path); 
      }
   }
}
