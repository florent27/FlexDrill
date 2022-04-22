// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductionViewModel.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.Process.OLPParser;
using Kuka.FlexDrill.SmartHMI.Production.Base;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using Kuka.FlexDrill.SmartHMI.Production.Messages;
using KukaRoboter.CoreUtil.Windows.Input;
using KUKARoboter.KRCModel.Robot.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel
{
    public class ProductionViewModel : FlexDrillViewModelBase
    {
        #region Constants and Fields

        private const string WorkPointType = "WORK";

        private RelayCommand runAllOperationsCommand;

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private bool userActionAllowed;

        private Operation selectedOperation;

        private CellProgram selectedProgram;

        private RobotPoint selectedWorkPoint;

        private RelayCommand skipAllOperationsCommand;

        private RelayCommand haltAndRunAllOperationsCmd;

        private RelayCommand runAndHaltAllOperationsCmd;

        private RelayCommand runAllPointsCommand;

        private RelayCommand skipAllPointsCommand;

        private RelayCommand haltAndRunAllPointsCmd;

        private RelayCommand runAndHaltAllPointsCmd;

        private RelayCommand resetAllPointsStatusCmd;

        private RelayCommand resetAllOperationsStatusCmd;

        private RelayCommand addToJobListCommand;

        private RelayCommand removeJobCommand;

        private RelayCommand moveJobDownCommand;

        private RelayCommand moveJobUpCommand;

        private CellProgram selectedJob;

        public delegate void SelectedOperationChanged(Operation Operation);

        public event SelectedOperationChanged SelectedOperationChangedEvent;

        public delegate void SelectedPointChanged(RobotPoint RobotPoint);

        public event SelectedPointChanged SelectedPointChangedEvent;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public ProductionViewModel() : base("FlexDrill_Production")
        {
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            ComponentFramework.StartupCompleted += OnStartupCompleted;
        }

        public void InitializePlugin()
        {
            if (FlexDrillService.InitSucceeded)
            {
                // Check whether a program is running
                UpdateUserActionMode();

                // Only display working points
                FilterPointList();

                //! Register Events
                RegisterEvents();
            }
            else
            {
                HandleError();
            }
        }

        // The list containing all the operation work modes. It is needed to show the possible choices in the ComboBox
        public KeyValuePair<OperationWorkMode, string>[] OperationWorkModes => new[]
        {
         new KeyValuePair<OperationWorkMode, string>(OperationWorkMode.Run,
            Resources.Strings["OperationMode_Run"]),
         new KeyValuePair<OperationWorkMode, string>(OperationWorkMode.Skip,
            Resources.Strings["OperationMode_Skip"]),
         new KeyValuePair<OperationWorkMode, string>(OperationWorkMode.HaltAndProcess,
            Resources.Strings["OperationMode_HaltAndProcess"]),
         new KeyValuePair<OperationWorkMode, string>(OperationWorkMode.ProcessAndHalt,
            Resources.Strings["OperationMode_ProcessAndHalt"])
      };

        public KeyValuePair<RobotPointWorkMode, string>[] PointWorkModes => new[]
        {
         new KeyValuePair<RobotPointWorkMode, string>(RobotPointWorkMode.Run,
            Resources.Strings["PointMode_Run"]),
         new KeyValuePair<RobotPointWorkMode, string>(RobotPointWorkMode.Skip,
            Resources.Strings["PointMode_Skip"]),
         new KeyValuePair<RobotPointWorkMode, string>(RobotPointWorkMode.HaltAndProcess,
            Resources.Strings["PointMode_HaltAndProcess"]),
         new KeyValuePair<RobotPointWorkMode, string>(RobotPointWorkMode.ProcessAndHalt,
            Resources.Strings["PointMode_ProcessAndHalt"])
      };

        public CellProgram SelectedJob
        {
            get
            {
                return selectedJob;
            }
            set
            {
                if (SelectedJob != null)
                {
                    SelectedOperation = selectedJob.WorkSequence.LOperation.FirstOrDefault();
                }
                SetField(ref selectedJob, value);
            }
        }

        public CellProgram SelectedProgram
        {
            get
            {
                return selectedProgram;
            }
            set
            {
                SetField(ref selectedProgram, value);
            }
        }

        public bool IsWorkPoint(object item)
        {
            if (!(item is RobotPoint))
            {
                return false;
            }

            return ((RobotPoint)item).Type == WorkPointType;
        }

        public bool UserActionAllowed
        {
            get
            {
                return userActionAllowed;
            }
            set
            {
                SetField(ref userActionAllowed, value);
            }
        }

        public Operation SelectedOperation
        {
            get
            {
                return selectedOperation;
            }
            set
            {
                if (selectedOperation != value)
                {
                    selectedOperation = value;
                    FirePropertyChanged("SelectedOperation");

                    if (SelectedOperationChangedEvent != null)
                    {
                        SelectedOperationChangedEvent.Invoke(selectedOperation);
                    }
                    // Only display working points
                    FilterPointList();
                }
            }
        }

        public RelayCommand RunAllOperationsCommand
        {
            get
            {
                if (runAllOperationsCommand == null)
                {
                    runAllOperationsCommand = new RelayCommand(action => RunAllOperations(),
                       cond => CanChangeAllOperationsMode);
                }
                return runAllOperationsCommand;
            }
        }

        public bool CanChangeAllOperationsMode
        {
            get
            {
                bool initSucceeded = FlexDrillService.InitSucceeded;
                bool noProgramRunning = UserActionAllowed;
                bool operationsNotNull = SelectedJob?.WorkSequence?.LOperation != null;
                bool operationsAvailable = SelectedJob?.WorkSequence?.LOperation?.Count >
                                           0;

                return initSucceeded && noProgramRunning && operationsNotNull && operationsAvailable;
            }
        }

        public bool CanChangeAllPointsMode
        {
            get
            {
                bool initSucceeded = FlexDrillService.InitSucceeded;
                bool noProgramRunning = UserActionAllowed;
                bool pointsNotNull = SelectedOperation?.RobotPoints?.LRobotPoint != null;
                bool pointsAvailable = SelectedOperation?.RobotPoints?.LRobotPoint?.Count > 0;

                return initSucceeded && noProgramRunning && pointsNotNull && pointsAvailable;
            }
        }

        public bool CanChangeAllPointsStatus
        {
            get
            {
                bool initSucceeded = FlexDrillService.InitSucceeded;
                bool noProgramRunning = UserActionAllowed;
                bool pointsNotNull = SelectedOperation?.RobotPoints?.LRobotPoint != null;
                bool pointsAvailable = SelectedOperation?.RobotPoints?.LRobotPoint?.Count > 0;

                return initSucceeded && noProgramRunning && pointsNotNull && pointsAvailable;
            }
        }

        public bool CanChangeAllOperationsStatus
        {
            get
            {
                bool initSucceeded = FlexDrillService.InitSucceeded;
                bool noProgramRunning = UserActionAllowed;
                bool operationsNotNull = SelectedJob?.WorkSequence?.LOperation != null;
                bool operationsAvailable = SelectedJob?.WorkSequence?.LOperation?.Count >
                                           0;

                return initSucceeded && noProgramRunning && operationsNotNull && operationsAvailable;
            }
        }

        public bool CanAddToJobList
        {
            get
            {
                return (SelectedProgram != null) && UserActionAllowed;
            }
        }

        public bool CanRemoveJob
        {
            get
            {
                return (SelectedJob != null) && UserActionAllowed;
            }
        }

        public bool CanMoveJobUp
        {
            get
            {
                int Idx = FlexDrillService.JobList.IndexOf(SelectedJob);
                return (SelectedJob != null) && (Idx > 0) && UserActionAllowed;
            }
        }

        private bool CanMoveJobDown
        {
            get
            {
                int Idx = FlexDrillService.JobList.IndexOf(SelectedJob);
                return (SelectedJob != null) && (Idx < FlexDrillService.JobList.Count - 1) && UserActionAllowed;
            }
        }

        public RelayCommand SkipAllOperationsCommand
        {
            get
            {
                if (skipAllOperationsCommand == null)
                {
                    skipAllOperationsCommand = new RelayCommand(action => SkipAllOperations(),
                       cond => CanChangeAllOperationsMode);
                }
                return skipAllOperationsCommand;
            }
        }

        public RelayCommand RunAndHaltAllOperationsCmd
        {
            get
            {
                if (runAndHaltAllOperationsCmd == null)
                {
                    runAndHaltAllOperationsCmd =
                       new RelayCommand(action => RunAndHaltAllOperations(), cond => CanChangeAllOperationsMode);
                }
                return runAndHaltAllOperationsCmd;
            }
        }

        public RelayCommand HaltAndRunAllOperationsCmd
        {
            get
            {
                if (haltAndRunAllOperationsCmd == null)
                {
                    haltAndRunAllOperationsCmd =
                       new RelayCommand(action => HaltAndRunAllOperations(), cond => CanChangeAllOperationsMode);
                }
                return haltAndRunAllOperationsCmd;
            }
        }

        public RobotPoint SelectedWorkPoint
        {
            get
            {
                return selectedWorkPoint;
            }
            set
            {
                if (selectedWorkPoint != value)
                {
                    selectedWorkPoint = value;
                    FirePropertyChanged("SelectedWorkPoint");

                    if (SelectedPointChangedEvent != null)
                    {
                        SelectedPointChangedEvent.Invoke(selectedWorkPoint);
                    }
                }
            }
        }

        public RelayCommand RunAllPointsCommand
        {
            get
            {
                if (runAllPointsCommand == null)
                {
                    runAllPointsCommand = new RelayCommand(action => RunAllPoints(), cond => CanChangeAllPointsMode);
                }
                return runAllPointsCommand;
            }
        }

        public RelayCommand SkipAllPointsCommand
        {
            get
            {
                if (skipAllPointsCommand == null)
                {
                    skipAllPointsCommand = new RelayCommand(action => SkipAllPoints(), cond => CanChangeAllPointsMode);
                }
                return skipAllPointsCommand;
            }
        }

        public RelayCommand HaltAndRunAllPointsCmd
        {
            get
            {
                if (haltAndRunAllPointsCmd == null)
                {
                    haltAndRunAllPointsCmd = new RelayCommand(action => HaltAndRunAllPoints(), cond => CanChangeAllPointsMode);
                }
                return haltAndRunAllPointsCmd;
            }
        }

        public RelayCommand RunAndHaltAllPointsCmd
        {
            get
            {
                if (runAndHaltAllPointsCmd == null)
                {
                    runAndHaltAllPointsCmd = new RelayCommand(action => RunAndHaltAllPoints(), cond => CanChangeAllPointsMode);
                }
                return runAndHaltAllPointsCmd;
            }
        }

        public RelayCommand ResetAllPointsStatusCmd
        {
            get
            {
                if (resetAllPointsStatusCmd == null)
                {
                    resetAllPointsStatusCmd = new RelayCommand(action => ResetAllPointsStatus(), cond => CanChangeAllPointsStatus);
                }
                return resetAllPointsStatusCmd;
            }
        }

        public RelayCommand ResetAllOperationsStatusCmd
        {
            get
            {
                if (resetAllOperationsStatusCmd == null)
                {
                    resetAllOperationsStatusCmd = new RelayCommand(action => ResetAllOperationsStatus(), cond => CanChangeAllOperationsStatus);
                }
                return resetAllOperationsStatusCmd;
            }
        }

        public RelayCommand AddToJobListCommand
        {
            get
            {
                if (addToJobListCommand == null)
                {
                    addToJobListCommand = new RelayCommand(action => AddToJobList(), cond => CanAddToJobList);
                }
                return addToJobListCommand;
            }
        }

        public RelayCommand RemoveJobCommand
        {
            get
            {
                if (removeJobCommand == null)
                {
                    removeJobCommand = new RelayCommand(action => RemoveJob(), cond => CanRemoveJob);
                }
                return removeJobCommand;
            }
        }

        public RelayCommand MoveJobDownCommand
        {
            get
            {
                if (moveJobDownCommand == null)
                {
                    moveJobDownCommand = new RelayCommand(action => MoveJob(false), cond => CanMoveJobDown);
                }
                return moveJobDownCommand;
            }
        }

        public RelayCommand MoveJobUpCommand
        {
            get
            {
                if (moveJobUpCommand == null)
                {
                    moveJobUpCommand = new RelayCommand(action => MoveJob(true), cond => CanMoveJobUp);
                }
                return moveJobUpCommand;
            }
        }

        public void FilterPointList()
        {
            if (selectedOperation?.RobotPoints != null)
            {
                // Set filter to only display points of type "WORK"
                ICollectionView collectionView =
                   CollectionViewSource.GetDefaultView(selectedOperation.RobotPoints.LRobotPoint);

                collectionView.Filter = IsWorkPoint;
                collectionView.Refresh();
            }
        }

        #endregion Interface

        #region Methods

        private void HaltAndRunAllOperations()
        {
            ChangeAllOperationsMode(OperationWorkMode.HaltAndProcess);
        }

        private void RunAndHaltAllOperations()
        {
            ChangeAllOperationsMode(OperationWorkMode.ProcessAndHalt);
        }

        private void SkipAllOperations()
        {
            ChangeAllOperationsMode(OperationWorkMode.Skip);
        }

        private void SkipAllPoints()
        {
            ChangeAllPointsMode(RobotPointWorkMode.Skip);
        }

        private void RunAllPoints()
        {
            ChangeAllPointsMode(RobotPointWorkMode.Run);
        }

        private void RunAndHaltAllPoints()
        {
            ChangeAllPointsMode(RobotPointWorkMode.ProcessAndHalt);
        }

        private void ResetAllPointsStatus()
        {
            ChangeAllPointsStatus(RobotPointWorkStatus.Idle);
            MessageHandler.ShowInfoMessage(FlexDrillMessages.WorkingPointsStatusReseted, new[] { string.Empty });
        }

        private void ResetAllOperationsStatus()
        {
            ChangeAllOperationsStatus(OperationWorkStatus.Idle);
            MessageHandler.ShowInfoMessage(FlexDrillMessages.OperationsStatusReseted, new[] { string.Empty });
        }

        private void HaltAndRunAllPoints()
        {
            ChangeAllPointsMode(RobotPointWorkMode.HaltAndProcess);
        }

        private void ChangeAllPointsMode(RobotPointWorkMode mode)
        {
            if (SelectedOperation != null)
            {
                foreach (RobotPoint robotPoint in SelectedOperation.RobotPoints.LRobotPoint)
                {
                    if (robotPoint.Type == WorkPointType)
                    {
                        robotPoint.WorkMode = mode;
                    }
                }
            }
        }

        private void ChangeAllOperationsMode(OperationWorkMode mode)
        {
            foreach (Operation operation in SelectedJob.WorkSequence.LOperation)
            {
                operation.WorkMode = mode;
            }
        }

        private void ChangeAllPointsStatus(RobotPointWorkStatus status)
        {
            if (SelectedOperation != null)
            {
                foreach (RobotPoint robotPoint in SelectedOperation.RobotPoints.LRobotPoint)
                {
                    if (robotPoint.Type == WorkPointType)
                    {
                        robotPoint.WorkStatus = status;
                    }
                }
            }
        }

        private void ChangeAllOperationsStatus(OperationWorkStatus status)
        {
            foreach (Operation operation in SelectedJob.WorkSequence.LOperation)
            {
                operation.WorkStatus = status;
            }
        }

        private void RunAllOperations()
        {
            ChangeAllOperationsMode(OperationWorkMode.Run);
        }

        private void AddToJobList()
        {
            if (SelectedProgram != null)
            {
                try
                {
                    CellProgram FProgram = SelectedProgram.DoReadDataFromXml();
                    if (FProgram != null)
                    {
                        FlexDrillService.JobList.Add(SelectedProgram);

                        SelectedProgram.CellConfiguration = FProgram.CellConfiguration;
                        SelectedProgram.Informations = FProgram.Informations;
                        SelectedProgram.CustomerInfo = FProgram.CustomerInfo;
                        SelectedProgram.CellConfiguration = FProgram.CellConfiguration;
                        SelectedProgram.WorkSequence = FProgram.WorkSequence;
                        SelectedProgram.SchemaVersion = FProgram.SchemaVersion;

                        //! Select Last Job Added
                        SelectedJob = SelectedProgram;
                        //! Select First Operation of Last Job Added
                        SelectedOperation = SelectedJob.WorkSequence?.LOperation?.FirstOrDefault();
                        //! Register Event for Each Job
                        RegisterOpModeEvents(SelectedJob);
                    }
                }
                catch (InvalidOperationException e)
                {
                    if (e.InnerException == null)
                    {
                        MessageHandler.ShowErrorMessage(FlexDrillMessages.LoadCellProgramFailed,
                           new[] { e.Message });

                        // Log
                        FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                           "Can not load cell program. " + e.Message + ".", e.StackTrace);
                    }
                    else
                    {
                        MessageHandler.ShowErrorMessage(FlexDrillMessages.LoadCellProgramFailed,
                           new[] { e.InnerException.Message });

                        // Log
                        FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                           "Can not load cell program. " + e.Message + "\n" + e.InnerException.Message, e.StackTrace);
                    }
                }
                catch (Exception e)
                {
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.LoadCellProgramFailed, new[] { e.Message });

                    // Log
                    FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                       "Can not load cell program. An unexpected exception occurred. " + e.Message + ".", e.StackTrace);
                }
            }
        }

        private void RemoveJob()
        {
            if (SelectedJob != null)
            {
                //! Remove Events
                foreach (Operation AOperation in SelectedJob.WorkSequence.LOperation)
                {
                    AOperation.WorkModeChangedEvent -= OperationWorkModeChanged;
                }

                SelectedJob.IsLoaded = false;
                FlexDrillService.JobList.Remove(SelectedJob);
                //! Select First Job
                SelectedJob = FlexDrillService.JobList.FirstOrDefault();
            }
        }

        private void MoveJob(bool AIsUp)
        {
            int Idx = FlexDrillService.JobList.IndexOf(SelectedJob);
            if (AIsUp)
            {
                FlexDrillService.JobList.Move(Idx, Idx - 1);
            }
            else
            {
                FlexDrillService.JobList.Move(Idx, Idx + 1);
            }
        }

        private void OnStartupCompleted(object sender, EventArgs e)
        {
            // Register events
            RobotInterpreter.ProgramStateChanged += OnProgramStateChanged;
        }

        private void OnProgramStateChanged(object sender, ProgramStateChangedEventArgs ea)
        {
            UpdateUserActionMode();

            // The following lines are necessary to update the enable/disable states of the buttons
            RunAllOperationsCommand.RaiseCanExecuteChanged();
            SkipAllOperationsCommand.RaiseCanExecuteChanged();
            HaltAndRunAllOperationsCmd.RaiseCanExecuteChanged();
            RunAndHaltAllOperationsCmd.RaiseCanExecuteChanged();

            RunAllPointsCommand.RaiseCanExecuteChanged();
            SkipAllPointsCommand.RaiseCanExecuteChanged();
            HaltAndRunAllPointsCmd.RaiseCanExecuteChanged();
            RunAndHaltAllPointsCmd.RaiseCanExecuteChanged();

            ResetAllOperationsStatusCmd.RaiseCanExecuteChanged();
            ResetAllPointsStatusCmd.RaiseCanExecuteChanged();

            AddToJobListCommand.RaiseCanExecuteChanged();
            MoveJobDownCommand.RaiseCanExecuteChanged();
            MoveJobUpCommand.RaiseCanExecuteChanged();
            RemoveJobCommand.RaiseCanExecuteChanged();
        }

        private void UpdateUserActionMode()
        {
            // Only allow the user to make modifications if the robot program is NOT running
            UserActionAllowed = RobotProgramState != ProStates.Active;
        }

        private void RegisterEvents()
        {
            Robot.KRLVariables[KrlVariableNames.CurrentOperationName].Changed += CurrentOperationNameChanged;
            Robot.KRLVariables[KrlVariableNames.CurrentPointName].Changed += CurrentPointNameChanged;
            FlexDrillService.JobLoadedChanged += OnJobLoadedChanged;
        }

        private void OnJobLoadedChanged(object sender, CellProgram e)
        {
            SelectedJob = e;
        }

        public void ReleaseEvents()
        {
            Robot.KRLVariables[KrlVariableNames.CurrentOperationName].Changed -= CurrentOperationNameChanged;
            Robot.KRLVariables[KrlVariableNames.CurrentPointName].Changed -= CurrentPointNameChanged;
        }

        private void CurrentPointNameChanged(object sender, EventArgs e)
        {
            if (SelectedJob != null)
            {
                Operation CurrentOp = null;
                RobotPoint CurrentPoint = null;
                string CurrentOpName = KrlVarHandler.ReadCharVariable(KrlVariableNames.CurrentOperationName, Robot);
                string CurrentPointName = KrlVarHandler.ReadCharVariable(KrlVariableNames.CurrentPointName, Robot);
                foreach (Operation AOperation in SelectedJob.WorkSequence.LOperation)
                {
                    if (AOperation.Name.Equals(CurrentOpName, StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentOp = AOperation;
                    }
                }

                if (CurrentOp != null)
                {
                    SelectedOperation = CurrentOp;
                    foreach (RobotPoint APoint in CurrentOp.RobotPoints.LRobotPoint)
                    {
                        if (APoint.Name.Equals(CurrentPointName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            CurrentPoint = APoint;
                        }
                    }
                }

                if (CurrentPoint != null)
                {
                    SelectedWorkPoint = CurrentPoint;
                }
            }
        }

        private void CurrentOperationNameChanged(object sender, EventArgs e)
        {
            if (SelectedJob != null)
            {
                Operation CurrentOp = null;
                string CurrentOpName = KrlVarHandler.ReadCharVariable(KrlVariableNames.CurrentOperationName, Robot);
                foreach (Operation AOperation in SelectedJob.WorkSequence.LOperation)
                {
                    if (AOperation.Name.Equals(CurrentOpName, StringComparison.OrdinalIgnoreCase))
                    {
                        CurrentOp = AOperation;
                    }
                }

                if (CurrentOp != null)
                {
                    SelectedOperation = CurrentOp;
                }
            }
        }

        private void RegisterOpModeEvents(CellProgram AJob)
        {
            foreach (Operation AOperation in AJob.WorkSequence.LOperation)
            {
                AOperation.WorkModeChangedEvent += OperationWorkModeChanged;
            }
        }

        //
        //! Event To Select Op When WorkMode Has been Changed
        //
        private void OperationWorkModeChanged(string AOperationName)
        {
            foreach (Operation AOperation in SelectedProgram.WorkSequence.LOperation)
            {
                if (AOperation.Name.Equals(AOperationName, StringComparison.InvariantCultureIgnoreCase))
                {
                    SelectedOperation = AOperation;
                }
            }
        }

        #endregion Methods
    }
}