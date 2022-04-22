// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillService.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ade.Components;
using JetBrains.Annotations;
using Kuka.FlexDrill.Process;
using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.Process.OLPParser;
using Kuka.FlexDrill.SmartHMI.Production.Exceptions;
using Kuka.FlexDrill.SmartHMI.Production.Geometry;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using Kuka.FlexDrill.SmartHMI.Production.Log;
using Kuka.FlexDrill.SmartHMI.Production.Messages;
using KUKARoboter.KRCModel.Robot;
using KUKARoboter.KRCModel.Robot.Interpreter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Kuka.FlexDrill.SmartHMI.Production.Service
{
    public class FlexDrillService : AdeComponent, IFlexDrillService, INotifyPropertyChanged
    {
        #region Constants and Fields

        private bool initSucceeded;

        private IFlexDrillProcess process;

        private ObservableCollection<CellProgram> jobList;

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private DispatcherTimer FTimer;

        private bool IsOnTimer = false;

        private Queue<CellProgram> QueueJobList;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public FlexDrillService()
        {
            Log = new Logger();

            InitFlexDrillService();

            FTimer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            FTimer.Tick += OnFTimer;
            FTimer.Start();

            QueueJobList = new Queue<CellProgram>();
        }

        #endregion Constructors and Destructor

        #region IFlexDrillService Members

        public void RetryInitialization()
        {
            InitFlexDrillService();
        }

        public KeyValuePair<FlexDrillMessages, string[]> Error { get; set; }

        /// <inheritdoc />
        public event EventHandler ProgramLoadedChanged;

        public IFlexDrillProcess Process
        {
            get
            {
                return process;
            }
            private set
            {
                if (process != value)
                {
                    process = value;
                    FirePropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        public Logger Log { get; }

        /// <inheritdoc />
        public bool InitSucceeded
        {
            get
            {
                return initSucceeded;
            }
            private set
            {
                initSucceeded = value;

                if (value)
                {
                    // Set a default value
                    Error = new KeyValuePair<FlexDrillMessages, string[]>();
                }
            }
        }

        public event EventHandler JobListChanged;

        public ObservableCollection<CellProgram> JobList
        {
            get
            {
                if (jobList == null)
                {
                    jobList = new ObservableCollection<CellProgram>();
                    jobList.CollectionChanged += JobListCollectionChanged;
                }

                return jobList;
            }

            set
            {
                if (jobList == null)
                {
                    jobList = new ObservableCollection<CellProgram>();
                    jobList.CollectionChanged += JobListCollectionChanged;
                }
                jobList = value;
            }
        }

        public IRobot Robot { get; set; }
        public MsgHandler Message { get; set; }

        // Use this event to Tell UserKey Bar that Job List has changed
        private void JobListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (JobListChanged != null)
            {
                JobListChanged.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public void InitializeCell()
        {
            Process.InitCell();
        }

        private void ActualizeCurrentCellProgram(CellProgram AProgram)
        {
            //! Needed for the UI
            AProgram.IsLoaded = true;

            // Actualize the current cell program in the process task
            Process.CurrentCellProgram = AProgram;

            Log.WriteMessage(TraceEventType.Information, "Actualize Cell Program - XML File is " + AProgram.XmlFilePath, "");
        }

        // Operator Pressed Start
        public void StartProgram()
        {
            ProStates CurrentProState = Robot.Interpreters[InterpreterTypes.Robot].ProgramState;
            bool NewJobLoaded = false;

            if (CurrentProState == ProStates.Free)
            {
                //! Buid Queue
                QueueJobList.Clear();
                foreach (CellProgram LProgram in JobList)
                {
                    QueueJobList.Enqueue(LProgram);
                    Log.WriteMessage(TraceEventType.Information, "Add " + LProgram.XmlFilePath + " to queue (Size = " + QueueJobList.Count + ")", "");
                }

                NewJobLoaded = LoadJob();
                SetIsFirstJob(true);
                Log.WriteMessage(TraceEventType.Information, "Start Program -Pro State is Free", "");
            }

            if (((CurrentProState == ProStates.Free) && (NewJobLoaded)) || (CurrentProState != ProStates.Free))
            {
                Log.WriteMessage(TraceEventType.Information, "Start Program -Start Cycle", "");
                Process.StartCycle();
            }
        }

        //! Load Next Job Krl Var Has been Triggered by KRL
        private void OnLoadNexJobChanged()
        {
            //! Load Next Job If Possible
            if (LoadJob())
            {
                Process.StartCycle();
                SetIsFirstJob(false);

                ProStates CurrentProState = Robot.Interpreters[InterpreterTypes.Robot].ProgramState;
                if (CurrentProState == ProStates.Free)
                {
                    Log.WriteMessage(TraceEventType.Information, "Pro State is Free", "");
                    //! Check If Program Has been loaded
                    for (int i = 0; i < 5; i++)
                    {
                        CurrentProState = Robot.Interpreters[InterpreterTypes.Robot].ProgramState;
                        if (CurrentProState == ProStates.Free)
                        {
                            Log.WriteMessage(TraceEventType.Information, "Try " + i.ToString(), "");
                            Process.StartCycle();
                            SetIsFirstJob(false);
                        }
                    }
                }
            }
        }

        public void SetIsFirstJob(bool AIsFirstJob)
        {
            Log.WriteMessage(TraceEventType.Information, "Set Is First Job - Value : " + AIsFirstJob.ToString(), "");
            KrlVarHandler.WriteVariable(KrlVariableNames.IsFirstJob, AIsFirstJob, Robot);
        }

        public event EventHandler<CellProgram> JobLoadedChanged;

        private bool LoadJob()
        {
            bool lJobLoaded = false;

            // Unload All Job
            foreach (CellProgram ACellProgram in JobList)
            {
                ACellProgram.IsLoaded = false;
            }
            if (Process.CurrentCellProgram != null)
            {
                Process.CurrentCellProgram.IsLoaded = false;
            }

            CellProgram FirstProgramToLoad = GetFirstProgramToLoad();
            if (FirstProgramToLoad != null)
            {
                ActualizeCurrentCellProgram(FirstProgramToLoad);

                // Notify Production View Model
                if (JobLoadedChanged != null)
                {
                    JobLoadedChanged.Invoke(this, FirstProgramToLoad);
                }
                Log.WriteMessage(TraceEventType.Information, "Loading Program " + FirstProgramToLoad.WorkSequence?.Name, string.Empty);
                lJobLoaded = true;
            }

            return lJobLoaded;
        }

        private CellProgram GetFirstProgramToLoad()
        {
            CellProgram lProgram = new CellProgram();
            bool lProgramFound = false;

            if (QueueJobList.Count > 0)
            {
                while ((QueueJobList.Count > 0) && (!lProgramFound))
                {
                    CellProgram lJob = QueueJobList.Dequeue();

                    if (JobNeedsToBeDone(lJob))
                    {
                        lProgramFound = true;
                        lProgram = lJob;
                        Log.WriteMessage(TraceEventType.Information, "Job " + lJob.XmlFilePath + " is to be done", "");
                    }
                    else
                    {
                        Log.WriteMessage(TraceEventType.Information, "Job " + lJob.XmlFilePath + " is not to be done", "");
                    }
                }
            }
            else
            {
                Log.WriteMessage(TraceEventType.Information, "Queue is empty - No Job To Be Loaded", "");
            }

            if (!lProgramFound)
            {
                Message.ShowInfoMessage(FlexDrillMessages.NoJobTobeDone, new[] { String.Empty });
                lProgram = null;
            }
            return lProgram;
        }

        private bool JobNeedsToBeDone(CellProgram ACellProgram)
        {
            bool lNeedsToBeDone = false;

            foreach (Operation AOperation in ACellProgram.WorkSequence.LOperation)
            {
                if (!lNeedsToBeDone)
                {
                    lNeedsToBeDone = GetOpNeedToBeDone(AOperation);
                }
            }

            return lNeedsToBeDone;
        }

        private bool GetOpNeedToBeDone(Operation AOperation)
        {
            return (AOperation.WorkMode == OperationWorkMode.Run ||
                    AOperation.WorkMode == OperationWorkMode.HaltAndProcess ||
                    AOperation.WorkMode == OperationWorkMode.ProcessAndHalt) &&
                   (AOperation.WorkStatus == OperationWorkStatus.Idle ||
                    AOperation.WorkStatus == OperationWorkStatus.InProgress);
        }

        private void OnFTimer(object sender, EventArgs e)
        {
            if (!IsOnTimer)
            {
                IsOnTimer = true;
                try
                {
                    if (Robot != null)
                    {
                        //! Load Next Job
                        bool LoadNextJob = KrlVarHandler.ReadBoolVariable(KrlVariableNames.LoadNextJob, Robot);
                        ProStates lCurrentState = Robot.Interpreters[InterpreterTypes.Robot].ProgramState;
                        if (LoadNextJob && (lCurrentState == ProStates.Free))
                        {
                            Log.WriteMessage(TraceEventType.Information, "Load Next Job has been trigerred", String.Empty);
                            // Reset KRL Var Load Next Job
                            KrlVarHandler.WriteVariable(KrlVariableNames.LoadNextJob, false, Robot);
                            OnLoadNexJobChanged();
                        }

                        //! Geometry Functions
                        bool CanExecuteGeometryFct = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CanExecuteGeometryFct, Robot);

                        if (CanExecuteGeometryFct)
                        {
                            Log.WriteMessage(TraceEventType.Information, "Can Exectute Geometry Function has been trigerred", String.Empty);
                            //! Reset Var
                            KrlVarHandler.WriteVariable(KrlVariableNames.CanExecuteGeometryFct, false, Robot);
                            OnCanExecuteGeometryFctChanged();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex);
                }
                finally
                {
                    IsOnTimer = false;
                }
                IsOnTimer = false;
            }
        }

        private void OnCanExecuteGeometryFctChanged()
        {
            if (InitSucceeded)
            {
                try
                {
                    int lFctCode = KrlVarHandler.ReadIntVariable(KrlVariableNames.GeometryFctCode, Robot);
                    Log.WriteMessage(TraceEventType.Information, "Call To Geometry Function - Code " + lFctCode.ToString(), String.Empty);
                    GeometryFunctions.ExecuteGeometryFunction(lFctCode, Robot);
                }
                catch (KrlVarNotDefinedException ex)
                {
                    Log.WriteException(ex);
                }
                catch (Exception ex)
                {
                    Log.WriteException(ex);
                }
            }
        }

        /// <inheritdoc />
        public void StopProgram()
        {
            Process.AbortCycle();
        }

        /// <inheritdoc />
        public void PauseProgram()
        {
            Process.PauseCycle();
        }

        /// <inheritdoc />
        public event EventHandler CellInitializedChanged;

        /// <inheritdoc />
        public void CalibrateAntiSliding()
        {
            Process.StartAntiSliddingCalibration();
        }

        /// <inheritdoc />
        public void CalibrateNormality()
        {
            Process.StartNormalityCalibration();
        }

        /// <inheritdoc />
        public void TareForceSensor()
        {
            Process.TareForceSensor();
        }

        /// <inheritdoc />
        public void GraspHead(int slotIndex)
        {
            Process.GraspHead(slotIndex);
        }

        /// <inheritdoc />
        public void DropHead(int slotIndex)
        {
            Process.DropHead(slotIndex);
        }

        /// <inheritdoc />
        public void ToggleVacuum(bool vacuumOn)
        {
            Process.StartVacuum(vacuumOn);
        }

        /// <inheritdoc />
        public void InitHead()
        {
            Process.InitHead();
        }

        /// <inheritdoc />
        public void RunHeadProcess(int processIndex)
        {
            Process.RunProcess(processIndex);
        }

        public void HeadChange(string AHeadToDrop, string AHeadToGrasp)
        {
            Process.HeadChange(AHeadToDrop, AHeadToGrasp);
        }

        public void StartManualPositionning()
        {
            Process.StartManualPositionning();
        }

        public void StartTCPCalibration(int ACalibrationType)
        {
            Process.StartTCPCalibration(ACalibrationType);
        }

        #endregion IFlexDrillService Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        #region Methods

        private void InitFlexDrillService()
        {
            try
            {
                // Initialize process model
                Process = new FlexDrillProcess();
                Process.CellInitializedChanged += OnCellInitializedChanged;

                // Load the available XML program files
                LoadProgramList();

                // Watch the program file directory to detect if a program is added or deleted
                InitializeFileWatcher();

                // No error occurred
                InitSucceeded = true;
            }
            catch (ProgramFolderEmptyException ex)
            {
                Error = new KeyValuePair<FlexDrillMessages, string[]>
                (
                   FlexDrillMessages.NoProgramInFolder,
                   new[] { ex.FolderPath }
                );

                Log.WriteException(ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                Error = new KeyValuePair<FlexDrillMessages, string[]>
                (
                   FlexDrillMessages.NoProgramInFolder,
                   new[] { FileHandler.ProgramFolderPath }
                );

                string text = $"FlexDrill program directory not found: \'{FileHandler.ProgramFolderPath}\'.";
                Log.WriteMessage(TraceEventType.Error, text
                   , ex.StackTrace);
            }
            catch (FileNotFoundException ex)
            {
                Error = new KeyValuePair<FlexDrillMessages, string[]>
                (
                   FlexDrillMessages.ConfigFileNotFound,
                   new[] { ex.FileName }
                );

                string text = $"FlexDrill configuration file not found: \'{ex.FileName}\'.";
                Log.WriteMessage(TraceEventType.Error, text
                   , ex.StackTrace);
            }
            catch (InvalidXmlSyntaxException ex)
            {
                Log.WriteException(ex);
            }
            catch (ArgumentException ex)
            {
                Error = new KeyValuePair<FlexDrillMessages, string[]>
                (
                   FlexDrillMessages.FolderNameEmpty,
                   new[] { Constants.ConfigFilePath }
                );

                string text = $"No program folder configured in: \'{Constants.ConfigFilePath}\'.";
                Log.WriteMessage(TraceEventType.Error, text
                   , ex.StackTrace);
            }
            catch (Exception ex)
            {
                string LogFilePath = Path.Combine(Constants.LogFileFolder, Constants.LogFileName);
                FileInfo logFileInfo = new FileInfo(LogFilePath);
                DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);

                //! Create Directory if not exists
                if (!logDirInfo.Exists)
                {
                    logDirInfo.Create();
                }

                Error = new KeyValuePair<FlexDrillMessages, string[]>
                (
                   // No kxr key for unexpected exception
                   FlexDrillMessages.UnexpectedError,
                   new[] { LogFilePath }
                );

                Log.WriteException(ex);
            }
        }

        /// <inheritdoc />
        private void LoadProgramList()
        {
            // Set the list of all available programs
            Process.Programs = FileHandler.GetPrograms();
        }

        private void InitializeFileWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = FileHandler.ProgramFolderPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                Filter = "*.xml"
            };

            watcher.Changed += OnProgramsChanged;
            watcher.Created += OnProgramsChanged;
            watcher.Deleted += OnProgramsChanged;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private void OnProgramsChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                // Thread safe call
                dispatcher.Invoke(
                   () =>
                   {
                       Process.Programs.Clear();

                       LoadProgramList();
                   });
            }
            catch (Exception ex)
            {
                Log.WriteMessage(TraceEventType.Error,
                   $"An error occurred while getting all Programs. {ex.Message}", ex.StackTrace);
            }
        }

        private void OnCellInitializedChanged(object sender, EventArgs e)
        {
            CellInitializedChanged?.Invoke(this, EventArgs.Empty);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Methods
    }
}