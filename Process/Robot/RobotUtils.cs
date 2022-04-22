// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RobotUtils.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;

using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.Process.Exceptions;
using Kuka.FlexDrill.Process.Helper;
using Kuka.FlexDrill.Process.OLPParser;

using KUKARoboter.KrcFileLib;
using KUKARoboter.KRCModel.Robot;
using KUKARoboter.KRCModel.Robot.Interpreter;

namespace Kuka.FlexDrill.Process.Robot
{
   public static class RobotUtils
   {
      #region KRL Variables
      const string cszImpossibleToGetKrlVariable = "Impossible to get Robot Variable ";

      public static int DoGetIntKrlVariable(string AVariableName, IRobot AIRobot)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoGetIntKrlVariable(" + AVariableName + ")", true);
         if (AIRobot != null)
         {
            try
            {
               return AIRobot.KRLVariables.ShowVar<int>("", AVariableName);
            }
            catch
            {
               Logger.Logger.WriteLog(cszImpossibleToGetKrlVariable + AVariableName, false);
               throw new RobotException(cszImpossibleToGetKrlVariable + AVariableName);
            }
         }
         return -1;
      }

      public static string DoGetStringKrlVariable(string AVariableName, IRobot AIRobot)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoGetStringKrlVariable(" + AVariableName + ")", true);
         if (AIRobot != null)
         {
            try
            {
               return AIRobot.KRLVariables.ShowVar<string>("", AVariableName).Trim('"');
            }
            catch
            {
               Logger.Logger.WriteLog(cszImpossibleToGetKrlVariable + AVariableName, false);
               throw new RobotException(cszImpossibleToGetKrlVariable + AVariableName);
            }

         }
         return "";
      }

      public static bool DoGetBoolKrlVariable(string AVariableName, IRobot AIRobot)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoGetBoolKrlVariable(" + AVariableName + ")", true);
         if (AIRobot != null)
         {
            try
            {
               return AIRobot.KRLVariables.ShowVar<bool>("", AVariableName);
            }
            catch
            {
               Logger.Logger.WriteLog(cszImpossibleToGetKrlVariable + AVariableName, false);
               throw new RobotException(cszImpossibleToGetKrlVariable + AVariableName);
            }

         }
         return false;
      }

      public static void DoLoadProgramToRobotRam(IRobot robot)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoLoadProgramToRobotRam", true);
         try
         {
            string SourceFilePath = Path.Combine(Constants.OlpFolder, Constants.OlpOutputFileName);
            string DestFilePath = Path.Combine(Constants.OlpDestinationFolder, Constants.OlpOutputFileName);

            ProStates CurrentProgramState = robot.Interpreters[InterpreterTypes.Robot].ProgramState;
            if (CurrentProgramState == ProStates.Free)
            {
               KRCFile.Copy(SourceFilePath, DestFilePath, true, false);
            }
         }
         catch
         {
            Logger.Logger.WriteLog("Impossible to load Program to Robot RAM", false);
            throw new RobotException("Impossible to load Program to Robot RAM");
         }
      }


      public static void DoSetKrlVariable(string AVariableName, bool AValue, IRobot AIRobot)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoSetKrlVariable(" + AVariableName + "," + AValue.ToString() + ")", true);
         try
         {
            if (AIRobot != null)
            {
               AIRobot.KRLVariables.SetVar("", AVariableName, AValue);
            }
         }
         catch
         {
            Logger.Logger.WriteLog("Exception - Impossible to set KRL variables " + AVariableName + " to " + AValue.ToString(), false);
            throw new RobotException("Impossible to set KRL variables " + AVariableName + " to " + AValue.ToString());
         }
      }

      public static void DoSetKrlVariable(string AVariableName, int AValue, IRobot AIRobot)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoSetKrlVariable(" + AVariableName + "," + AValue.ToString() + ")", true);
         try
         {
            if (AIRobot != null)
            {
               AIRobot.KRLVariables.SetVar("", AVariableName, AValue);
            }
         }
         catch
         {
            Logger.Logger.WriteLog("Exception - Impossible to set KRL variables " + AVariableName + " to " + AValue.ToString(), false);
            throw new RobotException("Impossible to set KRL variables " + AVariableName + " to " + AValue.ToString());
         }
      }

      public static void DoSetKrlVariable(string AVariableName, string AValue, IRobot AIRobot)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoSetKrlVariable(" + AVariableName + "," + AValue.ToString() + ")", true);
         try
         {
            if (AIRobot != null)
            {
               string ClearVar = $"STRCLEAR({AVariableName})";
               AIRobot.KRLVariables.ShowExpression(ClearVar);
               AIRobot.KRLVariables.SetVar("", AVariableName, "\"" + AValue + "\"");
            }
         }
         catch
         {
            Logger.Logger.WriteLog("Exception - Impossible to set KRL variables " + AVariableName + " to " + AValue.ToString(), false);
            throw new RobotException("Impossible to set KRL variables " + AVariableName + " to " + AValue.ToString());
         }
      }


      #endregion

      #region Status Update

      public static void DoUpdateOperationData(CellProgram ACellProgram, IRobot AIRobot)
      {
         //
         //! Log
         //
         if (ACellProgram != null)
         {
            Logger.Logger.WriteLog("Function Call - DoUpdateOperationData(" + ACellProgram.Name  + ")", true);
         }
         else
         {
            Logger.Logger.WriteLog("Function Call - DoUpdateOperationData(null)", true);
         }

         int OpStatus = DoGetIntKrlVariable(Constants.CurrentOperationStatus, AIRobot);

         //
         //! Get Current Op
         //
         Operation SelectedOperation = DoGetCurrentOperation(ACellProgram, AIRobot);
         if (SelectedOperation != null)
         {
            //
            //! Set Operation Status
            //
            SelectedOperation.WorkStatus = (OperationWorkStatus)OpStatus;
            //
            //! Tell KRL Program that Status has been updated
            //
            DoSetKrlVariable(Constants.OperationDataUpdated, true, AIRobot);
         }
      }

      public static void DoUpdateRobotPointData(CellProgram ACellProgram, IRobot AIRobot)
      {
         int PointStatus = DoGetIntKrlVariable(Constants.CurrentPointStatus, AIRobot);

         //
         //! Get Current Point
         //
         RobotPoint SelectedRobotPoint = DoGetCurrentPoint(ACellProgram, AIRobot);
         if (SelectedRobotPoint != null)
         {
            //
            //! Set Robot Point Status
            //
            SelectedRobotPoint.WorkStatus = (RobotPointWorkStatus)PointStatus;
            //
            //! Tell KRL Program that Status has been updated
            //
            DoSetKrlVariable(Constants.RobotPointDataUpdated, true, AIRobot);
         }
      }

      private static Operation DoGetCurrentOperation(CellProgram ACellProgram, IRobot AIRobot)
      {
         string OpName = DoGetStringKrlVariable(Constants.CurrentOperationName, AIRobot);
         int OpID = DoGetIntKrlVariable(Constants.CurrentOperationId, AIRobot);

         if (ACellProgram != null)
         {
            if (OpID > 0)
            {
               //! Find Operation
               Operation SelectedOp = ACellProgram.WorkSequence.LOperation[OpID - 1];
               //! Control Operation Name
               if (SelectedOp.Name == OpName)
               {
                  return SelectedOp;
               }
            }
         }
         return null;
      }


      private static RobotPoint DoGetCurrentPoint(CellProgram ACellProgram, IRobot AIRobot)
      {
         string PointName = DoGetStringKrlVariable(Constants.CurrentPointName, AIRobot);
         int PointID = DoGetIntKrlVariable(Constants.CurrentPointId, AIRobot);

         //
         //! Get Current Operation
         //
         Operation CurrentOP = DoGetCurrentOperation(ACellProgram, AIRobot);
         //
         //! Get Current Point
         //
         if (CurrentOP != null)
         {
            //! Find Robot Point
            if (PointID > 0)
            {
               RobotPoint SelectedRobotPoint = CurrentOP.RobotPoints.LRobotPoint[PointID - 1];
               //! Control Point Name
               if (SelectedRobotPoint.Name == PointName)
               {
                  return SelectedRobotPoint;
               }
            }
         }
         return null;
      }

      internal static void DoStartProgram(IRobot robot, string FileToSelect)
      {
         //
         //! Log
         //
         Logger.Logger.WriteLog("Function Call - DoStartProgram(" + FileToSelect + ")", false);
         if (robot != null)
         {
            ProStates CurrentProgramState = robot.Interpreters[InterpreterTypes.Robot].ProgramState;
            bool SafetyDrivesEnabled = DoGetBoolKrlVariable(Constants.SafetyDrivesEnabled, robot);

            if (CurrentProgramState == ProStates.Free)
            {
               
               try
               {
                  Logger.Logger.WriteLog("Select File (" + FileToSelect + ")", false);
                  robot.Interpreters.Select(FileToSelect, "", false);
               }
               catch
               {
                  throw new RobotException("Impossible to select File " + FileToSelect);
               }
            }
            try
            {
               //
               //! Perform AutoStart only if Auto Mode is selected
               //
               if (robot.ModeOperation == OperationModes.Aut)
               {
                  Logger.Logger.WriteLog("Start Program", false);

                  if (SafetyDrivesEnabled)
                  {
                     if (robot.DrivesReady)
                     {
                        Logger.Logger.WriteLog("Drives ready", false);
                     }
                     else
                     {
                        Logger.Logger.WriteLog("Drives  not ready", false);
                        //
                        //! Enable Drives
                        //
                        DoSetKrlVariable("$DRIVES_ENABLE", true, robot);

                        for (int i = 0; i <= 3; i++)
                        {
                           if (robot.DrivesReady)
                           {
                              break;
                           }
                           else
                           {
                              Logger.Logger.WriteLog("Wait 1 Second that Drives are enabled", false);
                              //
                              //! Wait 1 Second before recontrol drive status
                              //
                              System.Threading.Thread.Sleep(1000);
                           }
                        }
                     }
                  }
                  robot.Interpreters[InterpreterTypes.Robot].Start();
               }
            }
            catch
            {
               
               if (! SafetyDrivesEnabled)
               {
                  throw new RobotException("EStop Active -> Impossible to Start Robot Program " + FileToSelect);
               }
               else if (! robot.DrivesReady)
               {
                  throw new RobotException("Drives Not Ready -> Impossible to Start Robot Program " + FileToSelect);
               }
               else
               {
                  throw new RobotException("Impossible to Start Robot Program " + FileToSelect);
               }
            }
         }
      }

      internal static void DoCancelProgram(IRobot robot)
      {
         robot.Interpreters[InterpreterTypes.Robot].Cancel();
      }
      #endregion
   }
}