using Ade.Components;
using Cognex.InSight;
using Cognex.InSight.Sensor;
using Kuka.FlexDrill.Process.Logger;
using KukaRoboter.Common.ApplicationServices.Dialogs;
using KukaRoboter.Common.ApplicationServices.Messaging;
using KukaRoboter.Common.ApplicationServices.ViewManager;
using KukaRoboter.Common.Tracing;
using KukaRoboter.Common.ViewModel;
using KukaRoboter.CoreUtil.Windows.Input;
using KukaRoboter.SmartHMI.LegacySupport;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Kuka.FlexDrill.SmartHMI.VisionCognex.ViewModels
{
   /// <summary>
   /// This class describes the viewmodel for the VisionCognex plugin
   /// </summary>
   public class VisionCognexViewModel : ViewModelBase
   {
      #region Variables & Constants

      private const int CmdSetInitialState = 0;

      private const int CmdTakePicture = 1;

      private const int CmdLocateTargetXy = 4;

      private const int CmdLocateVisionCenter = 7;

      private const int CmdLocateModuleCenter = 8;

      private const int CmdMeasureNormality = 9;

      private const int CmdCalibrateVisionCenter = 97;

      private const int CmdCalibrateModuleCenter = 98;

      private const int CmdCalibrateVisionScale = 99;

      private const int CmdCalibrateNormality = 109;

      private const int CmdManualAcquire = 200;

      private const int CmdSaveJob = 300;

      private const int CmdStartContinuousProcessing = 400;

      private const int CmdConnect = 1000;

      private const int CmdDisconnect = 2000;

      private const int CmdManualAcquireOk = 200;

      private const int CmdManualAcquireNok = 201;

      private const int CmdSaveJobOk = 300;

      private const int CmdSaveJobNok = 301;

      private const int CmdConnectOk = 1000;

      private const int CmdConnectNok = 1001;

      private const int CmdDisconnectOk = 2000;

      private const int CmdDisconnectNok = 2001;

      private const int UserCmdOk = 62;

      private const int UserCmdSkip = 60;

      private const int UserCmdNok = 61;

      private const int UnknownCmd = -1;

      private CvsCellLocation CellCode;

      private CvsCellLocation CellClamped;

      private CvsCellLocation CellPattern;

      private CvsCellLocation CellExposureRatio;

      private CvsCellLocation CellOpV;

      private CvsCellLocation CellOpH;

      private CvsCellLocation CellDate;

      private CvsCellLocation CellContinuousProcessing;

      private int FunctionCode;

      private readonly PrettyTraceSource Trace = TraceSourceFactory.GetSource("VisionCognex");

      public CvsInSight CognexCamera;

      private int LastFunctionCode = 10000;

      private ConfigVisionCognex.ConfigVisionCognex configuration;
      public ConfigVisionCognex.ConfigVisionCognex Configuration
      {
         get
         {
            if (configuration == null)
            {
               configuration = ConfigVisionCognex.ConfigVisionCognex.Read();
            }
            return configuration;
         }
      }

      public bool CamIsOnLine
      {
         get
         {
            return camIsOnLine;
         }
         set
         {
            camIsOnLine = value;
            FirePropertyChanged("CamIsOnLine");
         }
      }
      private bool camIsOnLine;
      
      public bool CamIsConnected
      {
         get
         {
            return camIsConnected;
         }
         set
         {
            camIsConnected = value;
            FirePropertyChanged("CamIsConnected");
         }
      }
      private bool camIsConnected;

     public bool CamIsDisconnected
     {
       get
       {
         return camIsDisconnected;
       }
       set
       {
         camIsDisconnected = value;
         FirePropertyChanged("CamIsDisconnected");
       }
     }
     private bool camIsDisconnected;


     [RangeValidator(typeof(float), "0", RangeBoundaryType.Exclusive, "10", RangeBoundaryType.Inclusive, MessageTemplate = "{3}#;#{5}", Tag = "pixelValueNotOk")]
      public float NbPixel
      {
         get
         {
            return nbPixel;
         }

         set
         {
            nbPixel = value;
            FirePropertyChanged("NbPixel");
         }
      }
      private float nbPixel;

      private bool visionCalibrationOk;
      public bool VisionCalibrationOk
      {
         get
         {
            return visionCalibrationOk;
         }
         set
         {
            visionCalibrationOk = value;
            FirePropertyChanged("VisionCalibrationOk");
         }
      }

      private bool normalityCalibrationOk;
      public bool NormalityCalibrationOk
      {
         get
         {
            return normalityCalibrationOk;
         }
         set
         {
            normalityCalibrationOk = value;
            FirePropertyChanged("NormalityCalibrationOk");
         }
      }


      private bool calibrateVisionNotRunning;
      public bool CalibrateVisionNotRunning
      {
         get
         {
            return calibrateVisionNotRunning;
         }
         set
         {
            calibrateVisionNotRunning = value;
            FirePropertyChanged("CalibrateVisionNotRunning");
         }
      }

      private bool calibrateVisionNormalityNotRunning;
      public bool CalibrateVisionNormalityNotRunning
      {
         get
         {
            return calibrateVisionNormalityNotRunning;
         }
         set
         {
            calibrateVisionNormalityNotRunning = value;
            FirePropertyChanged("CalibrateVisionNormalityNotRunning");
         }
      }


      public bool DisplayFullTab
      {
        get
        {
           return LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarDisplayFullTab].ConvertToBoolean(true);
        }
      }
      #endregion

      #region Constructor

      public VisionCognexViewModel()
         : base("VisionCognex", true)
      {
         CognexCamera  = new CvsInSight();
         CellCode = new CvsCellLocation(3, 'C');
         CellClamped = new CvsCellLocation(4, 'C');
         CellPattern = new CvsCellLocation(5, 'C');
         CellExposureRatio = new CvsCellLocation(6, 'C');
         CellOpV = new CvsCellLocation(8, 'C');
         CellOpH = new CvsCellLocation(9, 'C');
         CellDate = new CvsCellLocation(10, 'C');
         CellContinuousProcessing = new CvsCellLocation(7, 'C');
      }


      #endregion

      #region Initialization
      public override void Initialize()
      {
         base.Initialize();
         ComponentFramework.StartupCompleted += ComponentFramework_StartupCompleted;
      }
      #endregion

      #region Startup/Termination

      private void ComponentFramework_StartupCompleted(object sender, EventArgs e)
      {
         AddTrace("Vision Cognex Events");
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarOpenPlugIn].Changed += OpenPluginChanged;
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarFunctionCode].Changed += FunctionCodeChanged;
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibVisionNormalityRunning].Changed +=
            CalibVisionNormalityRunningChanged;
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibVisionRunning].Changed +=
            CalibVisionRunningChanged;
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarContinuousProcessing].Changed +=
            ContinuousProcessingChanged;
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibVisionOk].Changed += VisionCalibrationOkChanged;
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibNormalityOk].Changed +=
            NormalityCalibrationOkChanged;


         CamIsConnected = CognexCamera.State != CvsInSightState.NotConnected;
         CamIsDisconnected = (!CamIsConnected);

         LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarCamIsConnected].RawValue = CamIsConnected;
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarOpenPlugIn].RawValue = "false";
      }

      #endregion

      #region ViewModelBase Members
      protected override void OnConnected()
      {
         base.OnConnected();
         NbPixel = 1;
      }
      
      protected override void OnDisconnecting()
      {
         base.OnDisconnecting();
         LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarOpenPlugIn].RawValue = "false";
      }


      #endregion
      
      #region Relay Commands
      
      /// <summary>
      /// This command will be called when the user wants to cancel/reset the activation process
      /// </summary>
      private ICommand mountReticule;
      public System.Windows.Input.ICommand MountReticule
      {
         get
         {
            if (mountReticule == null)
            {
               mountReticule = new RelayCommand(param => this.OnMountReticule());
            }
            return mountReticule;
         }
      }
      
      private ICommand downReticule;
      public ICommand DownReticule
      {
         get
         {
            if (downReticule == null)
            {
               downReticule = new RelayCommand(param => this.OnDownReticule());
            }
            return downReticule;
         }
      }
      
      private ICommand leftReticule;
      public ICommand LeftReticule
      {
         get
         {
            if (leftReticule == null)
            {
               leftReticule = new RelayCommand(param => this.OnLeftReticule());
            }
            return leftReticule;
         }
      }

      private ICommand rightReticule;
      public ICommand RightReticule
      {
         get
         {
            if (rightReticule == null)
            {
               rightReticule = new RelayCommand(param => this.OnRightReticule());
            }
            return rightReticule;
         }
      }

      private ICommand takeNewPictureCommand;
      public ICommand TakeNewPictureCommand
      {
         get
         {
            if (takeNewPictureCommand == null)
            {
               takeNewPictureCommand = new RelayCommand(param => this.OnTakeNewPicture());
            }
            return takeNewPictureCommand;
         }
      }

      private ICommand skipCommand;
      public ICommand SkipCommand
      {
         get
         {
            if (skipCommand == null)
            {
               skipCommand = new RelayCommand(param => this.OnSkipCommand());
            }
            return skipCommand;
         }
      }

      private ICommand okCommand;
      public ICommand OkCommand
      {
         get
         {
            if (okCommand == null)
            {
               okCommand = new RelayCommand(param => this.OnOkCommand());
            }
            return okCommand;
         }
      }

      private ICommand nokCommand;
      public ICommand NokCommand
      {
         get
         {
            if (nokCommand == null)
            {
               nokCommand = new RelayCommand(param => this.OnNokCommand());
            }
            return nokCommand;
         }
      }

      private ICommand connectionCommand;
      public ICommand ConnectionCommand
      {
         get
         {
            if (connectionCommand == null)
            {
               connectionCommand = new RelayCommand(param => this.ConnectCamera());
            }
            return connectionCommand;
         }
      }

      private ICommand disconnectionCommand;
      public ICommand DisconnectionCommand
      {
         get
         {
           if (disconnectionCommand == null)
           {
            disconnectionCommand = new RelayCommand(param => this.DisconnectCamera());
           }
           return disconnectionCommand;
         }
      }

     private ICommand calibrateVisionCommand;
      public ICommand CalibrateVisionCommand
      {
         get
         {
            if (calibrateVisionCommand == null)
            {
               calibrateVisionCommand = new RelayCommand(param => this.OnCalibrateVisionCommand());
            }
            return calibrateVisionCommand;
         }
      }

      private ICommand calibrateNormalityCommand;
      public ICommand CalibrateNormalityCommand
      {
         get
         {
            if (calibrateNormalityCommand == null)
            {
               calibrateNormalityCommand = new RelayCommand(param => this.OnCalibrateNormalityCommand());
            }
            return calibrateNormalityCommand;
         }
      }

      #endregion

      #region commands
      private void OnMountReticule()
      {
         if ((CognexCamera.State != CvsInSightState.NotConnected) && (NbPixel>0) && (NbPixel<=10))
         {
            float pixel = Single.Parse(CognexCamera.Results.Cells["C8"].Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) - NbPixel;
            CognexCamera.SetFloat(CellOpV, pixel);
         }
      }

      private void OnDownReticule()
      {
         if ((CognexCamera.State != CvsInSightState.NotConnected) && (NbPixel > 0) && (NbPixel <= 10))
         {
            float pixel = Single.Parse(CognexCamera.Results.Cells["C8"].Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) + NbPixel;
            CognexCamera.SetFloat(CellOpV, pixel);
         }
      }

      private void OnLeftReticule()
      {
         if ((CognexCamera.State != CvsInSightState.NotConnected) && (NbPixel > 0) && (NbPixel <= 10))
         {
            float pixel = Single.Parse(CognexCamera.Results.Cells["C9"].Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) - NbPixel;
            CognexCamera.SetFloat(CellOpH, pixel);
         }
      }

      private void OnRightReticule()
      {
         if ((CognexCamera.State != CvsInSightState.NotConnected) && (NbPixel > 0) && (NbPixel <= 10))
         {
            float pixel = Single.Parse(CognexCamera.Results.Cells["C9"].Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) + NbPixel;
            CognexCamera.SetFloat(CellOpH, pixel);
         }
      }

      private void OnTakeNewPicture()
      {
         DoManualAcquire();
      }

      private void OnSkipCommand()
      {
         if (CognexCamera.State != CvsInSightState.NotConnected)
         {
            AddTrace("Operation Pressed Skip");
            SendResultToRobot(UserCmdSkip.ToString());
         }
         DoClosePluggin();
      }

      private void OnOkCommand()
      {
         if (CognexCamera.State != CvsInSightState.NotConnected)
         {
            AddTrace("Operation Pressed OK");
            SendResultToRobot(UserCmdOk.ToString());
         }
         DoClosePluggin();
      }

      private void OnNokCommand()
      {
         if (CognexCamera.State != CvsInSightState.NotConnected)
         {
            AddTrace("Operation Pressed Not OK");
            SendResultToRobot(UserCmdNok.ToString());
         }
         DoClosePluggin();
      }

      private void OnCalibrateVisionCommand()
      {
         LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarCalibVision].RawValue = "true";
      }

      private void OnCalibrateNormalityCommand()
      {
         LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarCalibNormality].RawValue = "true";
      }

      #endregion

      #region Event Handler

      private void CognexCameraResultsChanged(object sender, EventArgs e)
      {
         CognexCamera.ResultsChanged -= CognexCameraResultsChanged;

         SendResultToRobot(CognexCamera.Results.Cells["I3"].Text.Replace(",", "."));
      }

      private void CognexCameraStateChanged(object sender, CvsStateChangedEventArgs e)
      {
         CamIsOnLine = (CognexCamera.State == CvsInSightState.Online);
         CamIsConnected = (CognexCamera.State != CvsInSightState.NotConnected);
         LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarCamIsConnected].RawValue = CamIsConnected;
         CamIsDisconnected = (!CamIsConnected);
      }

      private void CalibVisionRunningChanged(object sender, EventArgs e)
      {
         CalibrateVisionNotRunning = (!LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibVisionRunning].ConvertToBoolean(true));
      }

      private void CalibVisionNormalityRunningChanged(object sender, EventArgs e)
      {
         CalibrateVisionNormalityNotRunning = (!LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibVisionNormalityRunning].ConvertToBoolean(true));
      }

      private void VisionCalibrationOkChanged(object sender, EventArgs e)
      {
         try
         {
            VisionCalibrationOk = LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibVisionOk].ConvertToBoolean(true);
         }
         catch (Exception x)
         {
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void NormalityCalibrationOkChanged(object sender, EventArgs e)
      {        
         try
         {
            NormalityCalibrationOk = LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCalibNormalityOk].ConvertToBoolean(true);
         }
         catch (Exception x)
         {
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void FunctionCodeChanged(object sender, EventArgs e)
      {
         FunctionCode = LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarFunctionCode].ConvertToInt32(true);
         AddTrace("Function Code Changed Event - Code = " + FunctionCode.ToString());
         if (FunctionCode != LastFunctionCode)
         {
            switch (FunctionCode)
            {
               case CmdSetInitialState:
                  SetInitialState(true);
                  break;

               case CmdTakePicture:
               case CmdLocateTargetXy:
               case CmdLocateVisionCenter:
               case CmdLocateModuleCenter:
               case CmdMeasureNormality:
               case CmdCalibrateVisionCenter:
               case CmdCalibrateModuleCenter:
               case CmdCalibrateVisionScale:
               case CmdCalibrateNormality:
               case CmdStartContinuousProcessing:
                  CamMission(FunctionCode);
                  break;

               case CmdManualAcquire:
                  DoManualAcquire();
                  break;

               case CmdSaveJob:
                  DoSaveJob();
                  break;            

               case CmdConnect:
                  ConnectCamera();
                  break;

               case CmdDisconnect:
                  DisconnectCamera();
                  break;

               default:
                  LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = UnknownCmd.ToString();
                  break;
            }
            LastFunctionCode = FunctionCode;
         }
      }

      private void OpenPluginChanged(object sender, EventArgs e)
      {
         AddTrace("Open Pluggin Changed");
         if (LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarOpenPlugIn].ConvertToBoolean(true))
         {
            DoOpenPluggin();
         }
         else
         {
            DoClosePluggin();
         }
      }

      private void DoOpenPluggin()
      {
         if (ViewManager.IsViewKnown("VisionCognex"))
         {
            if (!ViewManager.IsViewOpen("VisionCognex"))
            {
               ViewManager.OpenView("VisionCognex");
            }
         }
      }

      private void DoClosePluggin()
      {
         if (ViewManager.IsViewKnown("VisionCognex"))
         {
            if (ViewManager.IsViewOpen("VisionCognex"))
            {
               ViewManager.CloseView("VisionCognex");
            }
         }
      }

      private void ContinuousProcessingChanged(object sender, EventArgs e)
      {
         try
         {
            if (CognexCamera.State != CvsInSightState.NotConnected)
            {
               if (LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarContinuousProcessing].ConvertToBoolean(true))
               {

                  AddTrace("Call To Start Continuous Processing)");
                  SetInitialState(false);

                  CognexCamera.SetInteger(CellClamped, LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarClamped].ConvertToInt32(true));
                  CognexCamera.SetInteger(CellPattern, LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarPatternCode].ConvertToInt32(true));
                  CognexCamera.SetInteger(CellExposureRatio, LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarExposureRatio].ConvertToInt32(true));
                  CognexCamera.SetString(CellDate, DateTime.Now.ToString("dd/MM/yyyy_HH_mm_ss"));
                  CognexCamera.SetInteger(CellCode, LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarFunctionCode].ConvertToInt32(true));
                  CognexCamera.SetInteger(CellContinuousProcessing, 1);
               }
               else
               {
                  AddTrace("Call To Stop Continuous Processing");
                  SetInitialState(false);
               }
            }
         }
         catch (Exception x)
         {
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }
      #endregion

         #region services

      private ILegacyKRCInterface legacykrcinterface;
      public ILegacyKRCInterface LegacyKRCInterface
      {
         get
         {
            if (legacykrcinterface == null)
            {
               legacykrcinterface = GetService(typeof(ILegacyKRCInterface)) as ILegacyKRCInterface;
               if (legacykrcinterface == null)
               {
                  throw new ServiceNotFoundException("LegacyKRCInterface");
               }
            }
            return legacykrcinterface;
         }
      }

      private IViewManager viewManager;
      public IViewManager ViewManager
      {
         get
         {
            if (viewManager == null)
            {
               viewManager = (GetService(typeof(IViewManager)) as IViewManager);
            }
            return viewManager;
         }
      }
      #endregion

      #region Methods
      private void SetInitialState(bool startCtrl)
      {
         try
         {
            if (CognexCamera.State != CvsInSightState.NotConnected)
            {
               AddTrace("Call To SetInitialState");
               CognexCamera.SetInteger(CellCode, 0);
               CognexCamera.SetInteger(CellClamped, 0);
               CognexCamera.SetInteger(CellPattern, 0);
               CognexCamera.SetInteger(CellExposureRatio, 0);
               CognexCamera.SetFloat(CellOpV, 0);
               CognexCamera.SetFloat(CellOpH, 0);
               CognexCamera.SetInteger(CellContinuousProcessing, 0);
               if (startCtrl)
               {
                  CognexCamera.ResultsChanged += CognexCameraResultsChanged;
               }

            }
         }
         catch (Exception x)
         {
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void DoManualAcquire()
      {
         try
         {
            if (CognexCamera.State != CvsInSightState.NotConnected)
            {
               AddTrace("Call To Manual Acquire");
               CognexCamera.SetFloat(CellOpV, 0);
               CognexCamera.SetFloat(CellOpH, 0);
               CognexCamera.ManualAcquire();
               CognexCamera.ResultsChanged += CognexCameraResultsChanged;
               SavePicture(true);
            }
         }
         catch (Exception x)
         {
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = CmdManualAcquireNok.ToString();
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void CamMission(int code)
      {
         try
         {
            if (CognexCamera.State != CvsInSightState.NotConnected)
            {
               AddTrace("Call To Cam Mission " + code.ToString());
               SetInitialState(false);

               CognexCamera.SetInteger(CellClamped, LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarClamped].ConvertToInt32(true));
               CognexCamera.SetInteger(CellPattern, LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarPatternCode].ConvertToInt32(true));
               CognexCamera.SetInteger(CellExposureRatio, LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarExposureRatio].ConvertToInt32(true));
               CognexCamera.SetString(CellDate, DateTime.Now.ToString("dd/MM/yyyy_HH_mm_ss"));

               CognexCamera.ManualAcquire();
               CognexCamera.ResultsChanged += CognexCameraResultsChanged;
               CognexCamera.SetInteger(CellCode, code);
            }
         }
         catch (Exception x)
         {
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void SendResultToRobot(string errorCode)
      {
         AddTrace("Call To Send Result To Robot");
         try
         {
            var ResultI4 = CognexCamera.Results.Cells["I4"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultI4].RawValue = ResultI4;
            AddTrace("ResultI4 = " + ResultI4.ToString());

            var ResultI5 = CognexCamera.Results.Cells["I5"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultI5].RawValue = ResultI5;
            AddTrace("ResultI5 = " + ResultI5.ToString());

            var ResultI6 = CognexCamera.Results.Cells["I6"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultI6].RawValue = ResultI6;
            AddTrace("ResultI6 = " + ResultI6.ToString());
            
            var ResultI8 = CognexCamera.Results.Cells["I8"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultI8].RawValue = ResultI8;
            AddTrace("ResultI8 = " + ResultI8.ToString());
            
            var ResultG3 = CognexCamera.Results.Cells["G3"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultG3].RawValue = ResultG3;
            AddTrace("ResultG3 = " + ResultG3.ToString());

            var ResultG4 = CognexCamera.Results.Cells["G4"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultG4].RawValue = ResultG4;
            AddTrace("ResultG4 = " + ResultG4.ToString());
        
            var ResultL7 = "\"" + CognexCamera.Results.Cells["L7"].Text + "\"";
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarImageName].RawValue = ResultL7;
            AddTrace("ImageName = " + ResultL7.ToString());

            var ResultZ3 = CognexCamera.Results.Cells["Z3"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultZ3].RawValue = ResultZ3;
            AddTrace("ResultZ3 = " + ResultZ3.ToString());

            var ResultZ4 = CognexCamera.Results.Cells["Z4"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultZ4].RawValue = ResultZ4;
            AddTrace("ResultZ4 = " + ResultZ4.ToString());
 
            var ResultX3 = CognexCamera.Results.Cells["X3"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultX3].RawValue = ResultX3;
            AddTrace("ResultX3 = " + ResultX3.ToString());

            var ResultY4 = CognexCamera.Results.Cells["Y4"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultY4].RawValue = ResultY4;
            AddTrace("ResultY4 = " + ResultY4.ToString());
                   
            var ResultX5 = CognexCamera.Results.Cells["X5"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultX5].RawValue = ResultX5;
            AddTrace("ResultX5 = " + ResultX5.ToString());

            var ResultY5 = CognexCamera.Results.Cells["Y5"].Text.Replace(",", ".");
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarResultY5].RawValue = ResultY5;
            AddTrace("ResultY5 = " + ResultY5.ToString());
         
            SavePicture(false);

            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = errorCode;
            AddTrace("Return Code = " + errorCode.ToString());

         }
         catch (Exception x)
         {
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void ConnectCamera()
      {
         try
         {
            if (CognexCamera.State == CvsInSightState.NotConnected)
            {
               AddTrace("Call To Connect Camera");
               CognexCamera.Connect(Configuration.IpAddress, Configuration.UserName, "", false, false);
               CognexCamera.SoftOnline = true;
               CognexCamera.StateChanged += CognexCameraStateChanged;
               LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = CmdConnectOk.ToString();
            }
         }
         catch (Exception x)
         {
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = CmdConnectNok.ToString();
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void DoSaveJob()
      {
         try
         {
            if (CognexCamera.State != CvsInSightState.NotConnected)
            {
               AddTrace("Call To Do Save Job");
               CognexCamera.SetString(CellDate, DateTime.Now.ToString("dd/MM/yyyy_HH_mm_ss"));
               //! Go Offline
               CognexCamera.SoftOnline = false;
               // Get the active job name
               string FActiveJobName = CognexCamera.JobInfo.ActiveJobFile;
               AddTrace("Active Job Name => " + FActiveJobName);
               // Get the startup job name
               CvsSettings FCvsSettings = CognexCamera.Sensor.GetSettings();
               string FStartupJobName = FCvsSettings.StartupJob;
               AddTrace("StartUp Job Name => " + FStartupJobName);

               //! Save Job Into Sensor
               AddTrace("Saving Job into Sensor...");
               CognexCamera.File.SaveJobFile(FStartupJobName);
               AddTrace("Job Saved into sensor!");
               ////! Save Job on Disk
               //string FLocalPath = Configuration.SaveJobPath + FStartupJobName;
               //AddTrace("Saving Job on " + FLocalPath + " ...");
               ////! Create Directory if it doesnt exist
               //FileInfo logFileInfo = new FileInfo(FLocalPath);
               //DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
               ////! Create Directory if not exists
               //if (!logDirInfo.Exists)
               //{
               //   logDirInfo.Create();
               //}
               //CognexCamera.File.GetFileFromInSight(FLocalPath, FStartupJobName);
               //AddTrace("Job Saved into " + FLocalPath);
               //! Go Online 
               CognexCamera.SoftOnline = true;
               LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = CmdSaveJobOk.ToString();
            }
            
         }
         catch (Exception x)
         {
            //! Go Online 
            CognexCamera.SoftOnline = true;
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = CmdSaveJobNok.ToString();
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void DisconnectCamera()
      {
         try
         {
            if (CognexCamera.State != CvsInSightState.NotConnected)
            {
               AddTrace("Call To Disconnect Camera");
               CognexCamera.Disconnect();
               CognexCamera.ResultsChanged -= CognexCameraResultsChanged;
               CognexCamera.StateChanged -= CognexCameraStateChanged;
               LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = CmdDisconnectOk.ToString();
            }
         }
         catch (Exception x)
         {
            LegacyKRCInterface.Robot.KRLVariables[configuration.KrlVarReturnCode].RawValue = CmdDisconnectNok.ToString();
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }
      }

      private void SavePicture(bool interactif)
      {
         try
         {
             string extension = interactif ? ".bmp" : ".jpg";

            if (LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarSavePicture].ConvertToBoolean(true))
            {
               AddTrace("Call To Save Picture");

               string picturePath = Configuration.SavePicturePath +
                                    LegacyKRCInterface.Robot.KRLVariables[Configuration.KrlVarCurrentPointName + "[]"].ConvertToString(true) + "_" +
                                    CognexCamera.Results.Cells["L4"].Text + "_" +
                                    CognexCamera.Results.Cells["L7"].Text + extension;
               FileInfo logFileInfo = new FileInfo(picturePath);

               DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);

               //! Create Directory if not exists
               if (!logDirInfo.Exists)
               {
                  logDirInfo.Create();
               }

               //! Keep 50 Oldest Picture
               foreach (var fi in logDirInfo.GetFiles().OrderByDescending(x => x.LastWriteTime).Skip(50))
               {
                  fi.Delete();
               }

               CognexCamera.Results.Image.Save(picturePath);
            }
         }
         catch (Exception x)
         {
            AddTrace(x.Message);
            Trace.WriteException(x, true);
         }

      }

      private void AddTrace(string ATrace)
      {
         Logger.WriteLog("Vision Cognex - " + ATrace, false);
      }
      #endregion


   }
}
