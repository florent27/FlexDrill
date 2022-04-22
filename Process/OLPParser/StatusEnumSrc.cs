// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusEnumSrc.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Kuka.FlexDrill.Process.OLPParser
{
   public enum OperationWorkMode
   {
      Idle = -1,
      Skip = 0,
      Run = 1,
      HaltAndProcess = 2,
      ProcessAndHalt = 3
   }

   public enum OperationWorkStatus
   {
      Idle = -1,
      InProgress = 0,
      Done = 1,
      DoneWithError = 2
   }

   public enum RobotPointWorkMode
   {
      Idle = -1,
      Skip = 0,
      Run = 1,
      HaltAndProcess = 2,
      ProcessAndHalt = 3
   }

   public enum RobotPointWorkStatus
   {
      Idle = -1,
      PositionReached = 0,
      InProgress = 1,
      Done = 2,
      InError = 3,
      AssyStarted = 4,
      AssyDone = 5,
      AssyInError = 6
   }
}