// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstSrc.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using JetBrains.Annotations;

namespace Kuka.FlexDrill.Process.Helper
{
   public static class Constants
   {
      #region Constants and Fields

      //
      //! Path
      //
      public static string OlpFolder => @"D:\FlexDrill\OLP";

      public static string OlpOutputFileNameWithExt => @"FlexDrill_OLP.src";
      public static string OlpOutputFileName => "FlexDrill_OLP";

      public static string HeadManagementFilePath => @"KRC:\R1\TP\FlexDrill\SetiTec_HeadManagement";

      public static string OlpGeneratorPositionning => @"KRC:\R1\TP\FlexDrill\OlpGeneratorPositionning";

      public static string DisplaySienSensorValues => @"KRC:\R1\TP\FlexDrill\DisplaySienSensorValues";

      public static string TCPCalibration => @"KRC:\R1\TP\FlexDrill\TCPCalibration";
      public static string OlpDestinationFolder => @"KRC:\R1\Program";

      //! KRL Variables                             
      public static string OperationStart => "OperationStart";

      public static string OperationStarted => "OperationStarted";

      public static string UpdateOperationData => "UpdateOperationData";
      public static string OperationDataUpdated => "OperationDataUpdated";

      public static string UpdateRobotPointData => "UpdateRobotPointData";
      public static string RobotPointDataUpdated => "RobotPointDataUpdated";

      public static string OperationStop => "OperationStop";
      public static string OperationStopped => "OperationStopped";

      public static string OperationAbort => "OperationAbort";
      public static string OperationAborted => "OperationUpdated";

      public static string OperationCompleted => "OperationCompleted";
      public static string AckOperationCompleted => "AckOperationCompleted";

      public static string CurrentOperationId => "CurrentOperationID";
      public static string CurrentOperationName => "CurrentOperationName[]";
      public static string CurrentOperationStatus => "CurrentOperationStatus";

      public static string CurrentPointId => "CurrentPointID";
      public static string CurrentPointName => "CurrentPointName[]";
      public static string CurrentPointStatus => "CurrentPointStatus";

      public static string VacuumOn => "VacuumOn";
      public static string VacuumState => "VacuumState";

      public static string DropHeadRackId => "DropHeadRackId";
      public static string DropHead => "DropHead";

      public static string GraspHeadRackId => "GraspHeadRackId";
      public static string GraspHead => "GraspHead";

      public static string HeadToGrasp => "HeadToGrasp[]";
      public static string HeadToDrop => "HeadToDrop[]";

      public static string InitHead => "InitHead";
      public static string InitCell => "InitCell";

      public static string RunProcessId => "RunProcessId";
      public static string RunProcess => "RunProcess";

      public static string KrlTrue => "TRUE";
      public static string KrlFalse => "FALSE";

      public static string MoveBco => "$MOVE_BCO";
      public static string FullDebug => "FullDebug";

      public static string MsgInfoMessage => "MsgInfoMessage[]";
      public static string MsgQuitMessage => "MsgQuitMessage[]";

      public static string StartClampingCalib => "StartClampingCalib";

      public static string ClampCalibType => "ClampCalibType";

      public static string SafetyDrivesEnabled => "$SAFETY_DRIVES_ENABLED";

      public static string MsgInfoListCount => "MsgInfoListCount";

      public static string DoCancelProgram => "DoCancelProgram";

      public static string ClampPlugginOpen => "ClampPlugginOpen";

      public static string TCPCalibrationType => "TCPCalibrationType";
      #endregion
   }
}