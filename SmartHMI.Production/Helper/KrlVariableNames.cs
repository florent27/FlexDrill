// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KrlVariableNames.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kuka.FlexDrill.SmartHMI.Production.Helper
{
   public static class KrlVariableNames
   {
      #region Constants and Fields

      #region Program management

      public static readonly string CellInitIsRunning = "InitCellIsRunning";

      public static readonly string CommissioningDone = "CommissioningDone";
      public static readonly string DisplayProgramLoading = "DisplayProgramLoading";
      public static readonly string ProgramLoadingDisplayed = "CommissioningDone";

      #endregion

      #endregion

      #region Clamping

      public static readonly string ForceZ = "ForceZ";

      public static readonly string SlidingX = "SlidingX";

      public static readonly string SlidingY = "SlidingY";

      public static readonly string NormalityX = "NormalityX";

      public static readonly string NormalityY = "NormalityY";

      public static readonly string ForceCalibrationOk = "TareForceOK";

      public static readonly string SlidingCalibrationOk = "SlidingCalibrationOK";

      public static readonly string NormalityCalibrationOk = "NormalityCalibrationOK";

      public static readonly string ForceCalibrationRunning = "TareForceRunning";

      public static readonly string SlidingCalibrationRunning = "SlidingCalibRunning";

      public static readonly string NormalityCalibrationRunning = "NormalityCalibRunning";

      public static readonly string ClampingTargetForce = "ClampingTargetForce";

      public static readonly string ClampingSpeedFactor = "ClampingSpeedFactor";

      public static readonly string ClampingGain = "ClampingGain";

      public static readonly string ClampingGainFx = "ClampingGainFx";

      public static readonly string ClampingGainFy = "ClampingGainFy";

      public static readonly string ClampingGainTx = "ClampingGainTx";

      public static readonly string ClampingGainTy = "ClampingGainTy";

      public static readonly string ClampPlugginOpen = "ClampPlugginOpen";

      public static readonly string ClampOnlyActivated = "ClampOnlyActivated";

      public static readonly string LockNose = "LockNose";

      public static readonly string UnlockNose = "UnlockNose";

      public static readonly string NoseLocked = "NoseLocked";

      #endregion

      #region HeadManagement

      public static readonly string HeadInitIsRunning = "InitHeadIsRunning";

      public static readonly string HeadProcessIsRunning = "ProcessIsRunning";

      public static readonly string GraspIsRunning = "GraspHeadIsRunning";

      public static readonly string DropIsRunning = "DropHeadIsRunning";

      public static readonly string VacuumState = "VacuumState";

      public static readonly string HeadPresenceSlot1 = "HeadPresenceSlot1";

      public static readonly string HeadPresenceSlot2 = "HeadPresenceSlot2";

      public static readonly string HeadPresenceSlot3 = "HeadPresenceSlot3";

      public static readonly string[] HeadPresentInSlot = { HeadPresenceSlot1, HeadPresenceSlot2, HeadPresenceSlot3 };

      #endregion

      #region Seti-Tec

      public static readonly string MachineOk = "bMachineOK";

      public static readonly string MachineNotOk = "bMachineNOK";

      public static readonly string CycleCompleted = "bCycleCompleted";

      public static readonly string CycleStop = "bCycleStop";

      public static readonly string CycleAbort = "bCycleAbort";

      public static readonly string ToolReady = "bToolReady";

      public static readonly string ToolRunning = "bToolRunning";

      public static readonly string ToolLocked = "bToolLocked";

      public static readonly string ToolConnected = "bToolConnected";

      public static readonly string NoToolConnected = "bNoToolConnected";

      public static readonly string InitDone = "bInitDone";

      public static readonly string PSetLoaded = "bPSetLoaded";

      public static readonly string LubTankEmpty = "bLubTankEmpty";

      public static readonly string ServiceIndicator = "bServiceIndicator";

      public static readonly string MotorConnected = "bMotorConnected";

      public static readonly string NoMotorConnected = "bNoMotorConnected";

      public static readonly string CanUpdateLastCycleInfo = "CanUpdateLastCycleInfo";

      #endregion

      #region Hmi Display

      public static readonly string DisplayView = "DisplayView";

      public static readonly string ViewToDisplay = "ViewToDisplay[]";

      #endregion

      #region FlyOver

      public static readonly string StartFlyOver = "StartFlyOver";

      #endregion

      #region Light/Laser
      public static readonly string LightOn = "bLightOn";

      public static readonly string LightOff = "bLightOff";

      public static readonly string LaserOn = "bLaserOn";

      public static readonly string LaserOff = "bLaserOff";

      public static readonly string LightIsOn = "LightIsOn";

      public static readonly string LaserIsOn = "LaserIsOn";

      #endregion

      #region Olp Generator
      public static readonly string PosAct = "$POS_ACT";

      public static readonly string AxisAct = "$AXIS_ACT";

      public static readonly string TcpVision = "TCP_VISION";

      public static readonly string TcpDrill = "TCP_DRILL";

      public static readonly string TcpCFrame = "TCP_CFRAME";

      public static readonly string Tool = "$TOOL";

      public static readonly string Base = "$BASE";

      public static readonly string BaseOlpGenerator = "BaseOlpGenerator";

      public static readonly string NullFrame = "$NULLFRAME";

      #endregion

      #region Production

      public static readonly string CurrentOperationName = "CurrentOperationName[]";

      public static readonly string CurrentPointName = "CurrentPointName[]";

      public static readonly string LoadNextJob = "LoadNextJob";

      public static readonly string IsFirstJob = "IsFirstJob";

      #endregion

      # region TCPCalibration

      public static readonly string TCPCalibXYDx = "TCPCalibXYDx";
      public static readonly string TCPCalibXYDy = "TCPCalibXYDy";
      public static readonly string TCPCalibXYScore = "TCPCalibXYScore";
      public static readonly string TCPCalibXYListFmt_Angle = "TCPCalibXYList[{0}].Angle";
      public static readonly string TCPCalibXYListFmt_Measure = "TCPCalibXYList[{0}].Measure";
      public static readonly string XYListSize = "XYListSize";

      public static readonly string TCPCalibZDz = "TCPCalibZDz";
      public static readonly string TCPCalibZScore = "TCPCalibZScore";
      public static readonly string TCPCalibZList = "TCPCalibZList[]";
      public static readonly string TCPCalibZListFmt_Angle = "TCPCalibZList[{0}].Angle";
      public static readonly string TCPCalibZListFmt_Measure = "TCPCalibZList[{0}].Measure";


      public static readonly string ZListSize = "ZListSize";

      public static readonly string TCPCalibRxRyDRx = "TCPCalibRxRyDRx";
      public static readonly string TCPCalibRxRyDRy = "TCPCalibRxRyDRy";
      public static readonly string TCPCalibRxRyScore = "TCPCalibRxRyScore";

      public static readonly string TCPCalibRxRy1Dx = "TCPCalibRxRy1Dx";
      public static readonly string TCPCalibRxRy1Dy = "TCPCalibRxRy1Dy";
      public static readonly string TCPCalibRxRy1Score = "TCPCalibRxRy1Score";
      public static readonly string TCPCalibRxRy1ListFmt_Angle = "TCPCalibRxRy1List[{0}].Angle";
      public static readonly string TCPCalibRxRy1ListFmt_Measure = "TCPCalibRxRy1List[{0}].Measure";

      public static readonly string TCPCalibRxRy2Dx = "TCPCalibRxRy2Dx";
      public static readonly string TCPCalibRxRy2Dy = "TCPCalibRxRy2Dy";
      public static readonly string TCPCalibRxRy2Score = "TCPCalibRxRy2Score";
      public static readonly string TCPCalibRxRy2ListFmt_Angle = "TCPCalibRxRy2List[{0}].Angle";
      public static readonly string TCPCalibRxRy2ListFmt_Measure = "TCPCalibRxRy2List[{0}].Measure";

      public static readonly string RxRyZDistance = "RxRyZDistance";

      public static readonly string RxRyListSize = "RxRyListSize";

      public static readonly string TCPCalibVisionXYDx = "TCPCalibVisionXYDx";
      public static readonly string TCPCalibVisionXYDy = "TCPCalibVisionXYDy";
      public static readonly string TCPCalibVisionXYDRx = "TCPCalibVisionXYDRx";
      public static readonly string TCPCalibVisionXYDRy = "TCPCalibVisionXYDRy";

      public static readonly string TCPCalibVisionXY1List_MeasureX = "TCPCalibVisionXY1List[{0}].MeasureX";
      public static readonly string TCPCalibVisionXY1List_MeasureY = "TCPCalibVisionXY1List[{0}].MeasureY";

      public static readonly string TCPCalibVisionXY2List_MeasureX = "TCPCalibVisionXY2List[{0}].MeasureX";
      public static readonly string TCPCalibVisionXY2List_MeasureY = "TCPCalibVisionXY2List[{0}].MeasureY";

      public static readonly string TCPCalibVisionXYScore = "TCPCalibVisionXYScore"; 
      public static readonly string VisionXYListSize = "VisionXYListSize";

      public static readonly string TCPCalibVisionXY1Dx = "TCPCalibVisionXY1Dx";
      public static readonly string TCPCalibVisionXY1Dy = "TCPCalibVisionXY1Dy";
      public static readonly string TCPCalibVisionXY1Score = "TCPCalibVisionXY1Score";

      public static readonly string TCPCalibVisionXY2Dx = "TCPCalibVisionXY2Dx";
      public static readonly string TCPCalibVisionXY2Dy = "TCPCalibVisionXY2Dy";
      public static readonly string TCPCalibVisionXY2Score = "TCPCalibVisionXY2Score";

      public static readonly string ComputeXYMeasures = "ComputeXYMeasures";
      public static readonly string MeasuresXYComputed = "MeasuresXYComputed";
      public static readonly string MeasuresXYComputedOK = "MeasuresXYComputedOK";

      public static readonly string ComputeZMeasures = "ComputeZMeasures";
      public static readonly string MeasuresZComputed = "MeasuresZComputed";
      public static readonly string MeasuresZComputedOK = "MeasuresZComputedOK";

      public static readonly string ComputeRxRyMeasures = "ComputeRxRyMeasures";
      public static readonly string MeasuresRxRyComputed = "MeasuresRxRyComputed";
      public static readonly string MeasuresRxRyComputedOK = "MeasuresRxRyComputedOK";

      public static readonly string ComputeRxRy1Measures = "ComputeRxRy1Measures";
      public static readonly string MeasuresRxRy1Computed = "MeasuresRxRy1Computed";
      public static readonly string MeasuresRxRy1ComputedOK = "MeasuresRxRy1ComputedOK";

      public static readonly string ComputeRxRy2Measures= "ComputeRxRy2Measures";
      public static readonly string MeasuresRxRy2Computed = "MeasuresRxRy2Computed";
      public static readonly string MeasuresRxRy2ComputedOK = "MeasuresRxRy2ComputedOK"; 

      public static readonly string ComputeVisionXY1Measures = "ComputeVisionXY1Measures";
      public static readonly string MeasuresVisXY1Computed = "MeasuresVisXY1Computed";
      public static readonly string MeasuresVisXY1ComputedOK = "MeasuresVisXY1ComputedOK";

      public static readonly string ComputeVisionXY2Measures = "ComputeVisionXY2Measures";
      public static readonly string MeasuresVisXY2Computed = "MeasuresVisXY2Computed";
      public static readonly string MeasuresVisXY2ComputedOK = "MeasuresVisXY2ComputedOK";

      public static readonly string ZCalibAroundY = "ZCalibAroundY";
      public static readonly string IterationNumber = "IterationNumber";

      public static readonly string TCPCalibrationType = "TCPCalibrationType";
      public static readonly string SaveTCPCalibration = "SaveTCPCalibration";
      public static readonly string TCPCalibStartDistance = "TCPCalibStartDistance";

      #endregion

      #region Geometry Functions

      public static readonly string GeometryFctCode = "GeometryFctCode";
      public static readonly string CanExecuteGeometryFct = "CanExecuteGeometryFct";
      public static readonly string GeometryFctExecuted = "GeometryFctExecuted";
      public static readonly string GeometryFctExecutedOK = "GeometryFctExecutedOK";

      public static readonly string InverseFrame_FrameToInverse = "InverseFrame.FrameToInverse";
      public static readonly string InverseFrame_ComputedFrame = "InverseFrame.ComputedFrame";

      public static readonly string MultipltFrame_LeftFrame = "MultipltFrame.LeftFrame";
      public static readonly string MultipltFrame_RightFrame = "MultipltFrame.RightFrame";
      public static readonly string MultipltFrame_ComputedFrame = "MultipltFrame.ComputedFrame";

      public static readonly string FrameAngle_LeftFrame = "FrameAngle.LeftFrame";
      public static readonly string FrameAngle_RightFrame = "FrameAngle.RightFrame";
      public static readonly string FrameAngle_ComputedAngle = "FrameAngle.ComputedAngle";

      public static readonly string FrameDistance_LeftFrame = "FrameDistance.LeftFrame";
      public static readonly string FrameDistance_RightFrame = "FrameDistance.RightFrame";
      public static readonly string FrameDistance_ComputedDistance = "FrameDistance.ComputedDistance";

      public static readonly string FrameFrom3Points_Point1 = "FrameFrom3Points.Point1";
      public static readonly string FrameFrom3Points_Point2 = "FrameFrom3Points.Point2";
      public static readonly string FrameFrom3Points_Point3 = "FrameFrom3Points.Point3";
      public static readonly string FrameFrom3Points_ComputedFrame = "FrameFrom3Points.ComputedFrame";

      public static readonly string AverageFrameFromNPts_TheoFrame = "AverageFrameFromNPts.TheoFrame";
      public static readonly string AverageFrameFromNPts_PointPairListCnt = "AverageFrameFromNPts.PointPairListCnt";
      public static readonly string AverageFrameFromNPts_MaxDistance = "AverageFrameFromNPts.MaxDistance";
      public static readonly string AverageFrameFromNPts_MaxAngle = "AverageFrameFromNPts.MaxAngle";
      public static readonly string AverageFrameFromNPts_ComputedFrame = "AverageFrameFromNPts.ComputedFrame";
      public static readonly string BaseDistanceThreshold = "BaseDistanceThreshold";
      public static readonly string BaseAngleThreshold = "BaseAngleThreshold";

      public static readonly string PointPairListFmt_TheoriticalPoint = "PointPairList[{0}].TheoriticalPoint";
      public static readonly string PointPairListFmt_MeasuredPoint = "PointPairList[{0}].MeasuredPoint";
      #endregion


   }
}