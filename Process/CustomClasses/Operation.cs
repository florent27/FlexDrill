// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operation.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.OLPParser;
using System.Collections.ObjectModel;
using KUKARoboter.KRCModel.Robot.Movement;

namespace Kuka.FlexDrill.Process.CustomClasses
{
   public class Operation : OperationXSD
   {
      #region Constants and Fields

      private OperationWorkMode workMode = OperationWorkMode.Run;

      private OperationWorkStatus workStatus = OperationWorkStatus.Idle;

      private ObservableCollection<RobotPoint> workPoints;

      private ObservableCollection<RobotPoint> inputApproachPoints;

      private ObservableCollection<RobotPoint> outputApproachPoints;

      private bool useSameInputOutputApproachPoint;

      public delegate void WorkModeChanged(string OperationName);
      public event WorkModeChanged WorkModeChangedEvent;

      #endregion

      #region Interface

      /// <remarks />
      public OperationWorkMode WorkMode
      {
         get
         {
            return workMode;
         }
         set
         {
            workMode = value;
            RaisePropertyChanged("WorkMode");
            if (WorkModeChangedEvent != null)
            {
               WorkModeChangedEvent.Invoke(this.Name);
            }
         }
      }

      /// <remarks />
      public OperationWorkStatus WorkStatus
      {
         get
         {
            return workStatus;
         }
         set
         {
            workStatus = value;
            RaisePropertyChanged("WorkStatus");
         }
      }

      public ObservableCollection<RobotPoint> WorkPoints
      {
         get
         {
            return workPoints;
         }
         set
         {
            workPoints = value;
            RaisePropertyChanged("WorkPoints");
         }
      }

      public ObservableCollection<RobotPoint> InputApproachPoints
      {
         get
         {
            return inputApproachPoints;
         }
         set
         {
            inputApproachPoints = value;
            RaisePropertyChanged("InputApproachPoints");
         }
      }

      public ObservableCollection<RobotPoint> OutputApproachPoints
      {
         get
         {
            return outputApproachPoints;
         }
         set
         {
            outputApproachPoints = value;
            RaisePropertyChanged("OutputApproachPoints");
         }
      }

      public bool UseSameInputOutputApproachPoint
      {
         get
         {
            return useSameInputOutputApproachPoint;
         }
         set
         {
            useSameInputOutputApproachPoint = value;
            RaisePropertyChanged("UseSameInputOutputApproachPoint");
         }
      }

      public delegate string MyDel(string str);
      #endregion

      #region Methods

      internal void ResetStatus()
      {
         // Reset the operation state
         WorkStatus = OperationWorkStatus.Idle;

         if (RobotPoints?.LRobotPoint != null)
         {
            foreach (RobotPoint point in RobotPoints.LRobotPoint)
            {
               // Reset the point state
               point?.ResetStatus();
            }
         }
      }

      #endregion
   }
}