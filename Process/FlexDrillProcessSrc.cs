// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillProcessSrc.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.Process.Exceptions;
using Kuka.FlexDrill.Process.Helper;
using Kuka.FlexDrill.Process.KRLGenerator;
using Kuka.FlexDrill.Process.Robot;
using KUKARoboter.KRCModel.Robot;
using KUKARoboter.KRCModel.Robot.Interpreter;
using KUKARoboter.KRCModel.Robot.Kcp;
using KUKARoboter.KRCModel.Robot.Variables;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace Kuka.FlexDrill.Process
{
    public class FlexDrillProcess : IFlexDrillProcess, INotifyPropertyChanged
    {
        #region Constants and Fields

        //! Const
        private const int TareForceSensorType = 1;

        private const int AntiSliddingCalibType = 2;
        private const int NormalityCalibType = 3;
        private const int ciStartCycle = 1;
        private const int ciDropHead = 2;
        private const int ciGraspHead = 3;
        private const int ciHeadChange = 4;
        private const int ciManualPositionning = 5;
        private const int ciRsiSien = 6;
        private const int ciTCPCalibration = 7;
        private const int RefreshTimerTimerInterval = 100;

        //
        //! var
        //
        private readonly IRobot Robot;

        private bool FUpdateInProgress;
        private bool FReportInProgress;

        private readonly VariableInfo BcoExecuted;
        private readonly VariableInfo UpdateOperationData;
        private readonly VariableInfo UpdateRobotPointData;
        private readonly VariableInfo MsgQuit;
        private bool BcoReached;

        private string OlpFileToSelect;
        private int LastRobotMoveCmd = -1;

        //
        //! Properties
        //
        private CellProgram FCurrentCellProgram;

        private ObservableCollection<CellProgram> programs;

        private CellProgram generatedCellProgram;

        private bool cellInitialized;

        private const string cszCurrentCellProgram = "CurrentCellProgram";

        private const string cszPrograms = "Programs";

        private const string cszGeneratedCellProgram = "GeneratedCellProgram";

        private const string cszCellInitialized = "CellInitialized";

        private DispatcherTimer UpdateDataTimer;

        #endregion Constants and Fields

        #region Constructors and Destructor

        #region Constructor

        //
        //! Constructor
        //
        public FlexDrillProcess()
        {
            VariableInfo InfoFullDebug;
            //
            //! Get Robot Instance
            //
            Robot = RobotImpl.Instance;
            //
            //! Create Timer
            //

            UpdateDataTimer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromMilliseconds(RefreshTimerTimerInterval)
            };
            UpdateDataTimer.Tick += UpdateDataOnTimer;

            DispatcherTimer MsgInfoTimer = new DispatcherTimer(DispatcherPriority.DataBind)
            {
                Interval = TimeSpan.FromMilliseconds(RefreshTimerTimerInterval)
            };
            MsgInfoTimer.Tick += MsgInfoOnTimer;
            MsgInfoTimer.Start();

            UpdateOperationData = Robot.KRLVariables[Constants.UpdateOperationData];
            UpdateRobotPointData = Robot.KRLVariables[Constants.UpdateRobotPointData];
            Robot.Interpreters[InterpreterTypes.Robot].ProgramStateChanged += ProgramStateChanged;
            Robot.KRLVariables[Constants.DoCancelProgram].Changed += DoCancelProgramChanged;
            Robot.KRLVariables[Constants.ClampPlugginOpen].Changed += ClampPlugginOpenChanged;
            //
            //! Start Key Pressed
            //
            Robot.Kcp.Keys.StartPressed += StartPressed;
            //
            //! BCO Reached
            //
            BcoExecuted = Robot.KRLVariables[Constants.MoveBco];
            BcoExecuted.Changed += BcoChanged;
            //
            //! Message
            //
            MsgQuit = Robot.KRLVariables[Constants.MsgQuitMessage];
            MsgQuit.Changed += MsgQuit_Changed;
            //
            //! Log
            //
            Logger.Logger.FullDebug = Robot.KRLVariables[Constants.FullDebug].ConvertToBoolean(true);
            InfoFullDebug = Robot.KRLVariables[Constants.FullDebug];
            InfoFullDebug.Changed += InfoFullDebug_Changed;
            Logger.Logger.BuildLogFileName();
            Logger.Logger.WriteLog("----------------------------------", false);
            Logger.Logger.WriteLog("Constructor KUKA.FlexDrill.Process", false);
        }

        #endregion Constructor

        #endregion Constructors and Destructor

        #region IFlexDrillProcess Members

        public CellProgram CurrentCellProgram
        {
            get
            {
                return FCurrentCellProgram;
            }
            set
            {
                FCurrentCellProgram = value;
                RaisePropertyChanged(cszCurrentCellProgram);
            }
        }

        public ObservableCollection<CellProgram> Programs
        {
            get
            {
                return programs;
            }

            set
            {
                if (programs != value)
                {
                    programs = value;
                    RaisePropertyChanged(cszPrograms);
                }
            }
        }

        public CellProgram GeneratedCellProgram
        {
            get
            {
                return generatedCellProgram;
            }

            set
            {
                if (generatedCellProgram != value)
                {
                    generatedCellProgram = value;
                    RaisePropertyChanged(cszGeneratedCellProgram);
                }
            }
        }

        public event EventHandler CellInitializedChanged;

        public bool CellInitialized
        {
            get
            {
                return cellInitialized;
            }
            set
            {
                if (cellInitialized != value)
                {
                    cellInitialized = value;
                    CellInitializedChanged?.Invoke(this, EventArgs.Empty);
                }
                RaisePropertyChanged(cszCellInitialized);
            }
        }

        public event EventHandler LoadNextJobChanged;

        #endregion IFlexDrillProcess Members

        #region INotifyPropertyChanged Members

        #region Event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Event

        #endregion INotifyPropertyChanged Members

        #region Production

        private void MsgInfoOnTimer(object sender, EventArgs e)
        {
            if (!FReportInProgress)
            {
                try
                {
                    FReportInProgress = true;
                    int MsgListCount = RobotUtils.DoGetIntKrlVariable(Constants.MsgInfoListCount, Robot);
                    if (MsgListCount > 0)
                    {
                        for (int i = 1; i < 21; i++)
                        {
                            string VarName = $"MsgInfoList[{i},]";
                            string sItem = RobotUtils.DoGetStringKrlVariable(VarName, Robot);
                            if (sItem != " ")
                            {
                                Logger.Logger.WriteLog("KRL MsgNotify - " + sItem, false);
                                RobotUtils.DoSetKrlVariable(VarName, " ", Robot);
                            }
                        }
                    }
                }
                finally
                {
                    FReportInProgress = false;
                }
            }
        }

        private void MsgQuit_Changed(object sender, EventArgs e)
        {
            if (MsgQuit.ConvertToString(true) != "")
            {
                Logger.Logger.WriteLog("KRL MsgQuit - " + MsgQuit.ConvertToString(true), false);
            }
        }

        private void InfoFullDebug_Changed(object sender, EventArgs e)
        {
            Logger.Logger.FullDebug = Robot.KRLVariables[Constants.FullDebug].ConvertToBoolean(true);
        }

        private void UpdateDataOnTimer(object sender, EventArgs e)
        {
            if (CurrentCellProgram != null)
            {
                if (!FUpdateInProgress)
                {
                    try
                    {
                        FUpdateInProgress = true;
                        //
                        //! If UpdateOperationData has been triggered
                        //
                        if (UpdateOperationData.ConvertToBoolean(true))
                        {
                            RobotUtils.DoUpdateOperationData(CurrentCellProgram, Robot);
                        }
                        else
                        {
                            //
                            //! If UpdateRobotPointData has been triggered
                            //
                            if (UpdateRobotPointData.ConvertToBoolean(true))
                            {
                                RobotUtils.DoUpdateRobotPointData(CurrentCellProgram, Robot);
                            }
                        }
                    }
                    finally
                    {
                        FUpdateInProgress = false;
                    }
                }
            }
        }

        private void StartPressed(object sender, KcpStartKeyPressedEventArgs e)
        {
            //! Set Variable Operation Start
            RobotUtils.DoSetKrlVariable(Constants.OperationStart, true, Robot);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void RemoveProgram(string programName)
        {
            // Reset the current program if it has been deleted by the user
            if (CurrentCellProgram?.Name == programName)
            {
                CurrentCellProgram?.Unload();
                CurrentCellProgram = null;
            }

            // Remove the program from the list
            CellProgram programToDelete = Programs.FirstOrDefault(p => p.Name == programName);

            if (programToDelete != null)
            {
                Programs.Remove(programToDelete);
            }
        }

        public void AddProgram(string programPath)
        {
            string programName = Path.GetFileNameWithoutExtension(programPath);

            // Only add a program if it doesn't already exists
            if (Programs.FirstOrDefault(p => p.Name == programName) == null)
            {
                CellProgram newProgram = new CellProgram(programName, programPath);
                Programs.Add(newProgram);
            }
        }

        public void RenameProgram(string oldName, string newName)
        {
            CellProgram renamedProgram = Programs.FirstOrDefault(p => p.Name == oldName);

            if (renamedProgram != null)
            {
                renamedProgram.Name = newName;
            }
        }

        public CellProgram ParseXmlFile(string AXMLFilePath)
        {
            FCurrentCellProgram = KrlGeneratorUtils.GetOlpData(AXMLFilePath);
            return FCurrentCellProgram;
        }

        public bool GenerateSrcFile(CellProgram ACellProgram, string AOutputSRCFolderPath)
        {
            //
            //! Log
            //
            if (ACellProgram != null)
            {
                Logger.Logger.WriteLog("Function Call - GenerateSRCFile(" + ACellProgram.Name + "," + AOutputSRCFolderPath + ")", false);
            }
            else
            {
                Logger.Logger.WriteLog("Function Call - GenerateSRCFile(null," + AOutputSRCFolderPath + ")", false);
            }
            return KrlGenerator.DoGenerateSrcFile(ACellProgram, AOutputSRCFolderPath);
        }

        public void StartCycle()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - StartCycle", false);
            //! Set Variable Operation Start
            RobotUtils.DoSetKrlVariable(Constants.OperationStart, true, Robot);
            if (KrlGeneratorUtils.AreOperationsNameUnique(CurrentCellProgram.WorkSequence))
            {
                if (KrlGeneratorUtils.ArePointNameUnique(CurrentCellProgram.WorkSequence))
                {
                    if (Robot.Interpreters[InterpreterTypes.Robot].ProgramState == ProStates.Free)
                    {
                        if (KrlGenerator.DoGenerateSrcFile(CurrentCellProgram, Constants.OlpFolder))
                        {
                            BcoReached = false;
                            //! Move Program To Ram
                            //
                            RobotUtils.DoLoadProgramToRobotRam(Robot);
                        }
                        else
                        {
                            Logger.Logger.WriteLog("An error occured during SRC File Generation", false);
                        }
                    }
                    //
                    //! Start Program
                    //
                    LastRobotMoveCmd = ciStartCycle;
                    OlpFileToSelect = Path.Combine(Constants.OlpDestinationFolder, Constants.OlpOutputFileName);
                    RobotUtils.DoStartProgram(Robot, OlpFileToSelect);
                }
                else
                {
                    Logger.Logger.WriteLog("Impossible to Start Program - Points Names must be unique", false);
                    throw new SrcGenerationException("Impossible to Start Program - Points Names must be unique");
                }
            }
            else
            {
                Logger.Logger.WriteLog("Impossible to Start Program - Operations Names must be unique", false);
                throw new SrcGenerationException("Impossible to Start Program - Operations Names must be unique");
            }
        }

        private void BcoChanged(object sender, EventArgs e)
        {
            if (Robot.ModeOperation == OperationModes.Aut)
            {
                if (BcoExecuted.ConvertToBoolean(true))
                {
                    BcoReached = true;
                }
                else
                {
                    if (BcoReached)
                    {
                        //
                        //! Second Start
                        //
                        Logger.Logger.WriteLog("Start Move BCO", false);
                        if (LastRobotMoveCmd == ciStartCycle)
                        {
                            RobotUtils.DoStartProgram(Robot, OlpFileToSelect);
                        }
                        else
                        {
                            RobotUtils.DoStartProgram(Robot, Constants.HeadManagementFilePath);
                        }
                    }
                }
            }
        }

        private void ProgramStateChanged(object sender, ProgramStateChangedEventArgs ea)
        {
            Logger.Logger.WriteLog($"Program State is <{ea.NewProState.ToString()}> - Bco Reached <{BcoReached.ToString()}>", false);
            if ((Robot.ModeOperation == OperationModes.Aut) && (!BcoReached) && (ea.NewProState == ProStates.Stop))
            {
                Logger.Logger.WriteLog("Start Program Stop", false);
                if (LastRobotMoveCmd == ciStartCycle)
                {
                    RobotUtils.DoStartProgram(Robot, OlpFileToSelect);
                }
                else
                {
                    RobotUtils.DoStartProgram(Robot, Constants.HeadManagementFilePath);
                }
            }

            //! Timer Update Data
            if (ea.NewProState == ProStates.Free)
            {
                UpdateDataTimer.Stop();
                FUpdateInProgress = false;
            }
            else
            {
                UpdateDataTimer.Start();
            }
        }

        private void DoCancelProgramChanged(object sender, EventArgs e)
        {
            if (Robot.KRLVariables[Constants.DoCancelProgram].ConvertToBoolean(true))
            {
                RobotUtils.DoCancelProgram(Robot);
            }
        }

        private void ClampPlugginOpenChanged(object sender, EventArgs e)
        {
            if (Robot.KRLVariables[Constants.ClampPlugginOpen].ConvertToBoolean(true))
            {
                if (Robot.Interpreters[InterpreterTypes.Robot].ProgramState == ProStates.Free)
                {
                    StartRsiSien();
                }
            }
            else
            {
                string ProgramName = Robot.Interpreters[InterpreterTypes.Robot].ProgramInfo.SelectedName;
                if (ProgramName.Contains("DISPLAYSIENSENSORVALUES"))
                {
                    RobotUtils.DoCancelProgram(Robot);
                }
            }
        }

        public void PauseCycle()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - PauseCycle", false);
            RobotUtils.DoSetKrlVariable(Constants.OperationStop, true, Robot);
        }

        public void AbortCycle()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - AbortCycle", false);
            RobotUtils.DoSetKrlVariable(Constants.OperationAbort, true, Robot);
        }

        #endregion Production

        #region Seti-Tec

        public void DropHead(int ARackID)
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - DropHead(" + ARackID.ToString() + ")", false);

            BcoReached = false;
            RobotUtils.DoSetKrlVariable(Constants.DropHeadRackId, ARackID, Robot);
            RobotUtils.DoSetKrlVariable(Constants.DropHead, true, Robot);
            RobotUtils.DoSetKrlVariable(Constants.GraspHeadRackId, -1, Robot);
            LastRobotMoveCmd = ciDropHead;
            RobotUtils.DoStartProgram(Robot, Constants.HeadManagementFilePath);
        }

        public void GraspHead(int ARackID)
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - GraspHead(" + ARackID.ToString() + ")", false);

            BcoReached = false;
            RobotUtils.DoSetKrlVariable(Constants.GraspHeadRackId, ARackID, Robot);
            RobotUtils.DoSetKrlVariable(Constants.GraspHead, true, Robot);
            RobotUtils.DoSetKrlVariable(Constants.DropHeadRackId, -1, Robot);
            LastRobotMoveCmd = ciGraspHead;
            RobotUtils.DoStartProgram(Robot, Constants.HeadManagementFilePath);
        }

        public void HeadChange(string AHeadToDrop, string AHeadToGrasp)
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - HeadChange : Drop : " + AHeadToDrop + " Grasp : " + AHeadToGrasp, false);

            BcoReached = false;
            RobotUtils.DoSetKrlVariable(Constants.HeadToDrop, AHeadToDrop, Robot);
            RobotUtils.DoSetKrlVariable(Constants.HeadToGrasp, AHeadToGrasp, Robot);
            LastRobotMoveCmd = ciHeadChange;
            RobotUtils.DoStartProgram(Robot, Constants.HeadManagementFilePath);
        }

        public void StartManualPositionning()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - Start Manual Positionning", false);

            BcoReached = false;
            LastRobotMoveCmd = ciManualPositionning;
            RobotUtils.DoStartProgram(Robot, Constants.OlpGeneratorPositionning);
        }

        public void InitHead()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - InitHead", false);
            RobotUtils.DoSetKrlVariable(Constants.InitHead, true, Robot);
        }

        public void RunProcess(int AProcessID)
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - RunProcess(" + AProcessID.ToString() + ")", false);
            RobotUtils.DoSetKrlVariable(Constants.RunProcessId, AProcessID, Robot);
            RobotUtils.DoSetKrlVariable(Constants.RunProcess, true, Robot);
        }

        public void InitCell()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - InitCell", false);
            RobotUtils.DoSetKrlVariable(Constants.InitCell, true, Robot);
        }

        public void StartVacuum(bool AStartVacuum)
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - StartVacuum(" + AStartVacuum.ToString() + ")", false);
            RobotUtils.DoSetKrlVariable(Constants.VacuumOn, AStartVacuum, Robot);
        }

        #endregion Seti-Tec

        #region Clamping

        public void TareForceSensor()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - TareForceSensor", false);
            //
            //! Set Calib Type Mode
            //
            RobotUtils.DoSetKrlVariable(Constants.ClampCalibType, TareForceSensorType, Robot);
            //
            //! Start Calib
            //
            RobotUtils.DoSetKrlVariable(Constants.StartClampingCalib, true, Robot);
        }

        public void StartAntiSliddingCalibration()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - StartAntiSliddingCalibration", false);
            //
            //! Set Calib Type Mode
            //
            RobotUtils.DoSetKrlVariable(Constants.ClampCalibType, AntiSliddingCalibType, Robot);
            //
            //! Start Calib
            //
            RobotUtils.DoSetKrlVariable(Constants.StartClampingCalib, true, Robot);
        }

        public void StartNormalityCalibration()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - StartNormalityCalibration", false);
            //
            //! Set Calib Type Mode
            //
            RobotUtils.DoSetKrlVariable(Constants.ClampCalibType, NormalityCalibType, Robot);
            //
            //! Start Calib
            //
            RobotUtils.DoSetKrlVariable(Constants.StartClampingCalib, true, Robot);
        }

        public void StartRsiSien()
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - Start Rsi Sien", false);

            BcoReached = false;
            LastRobotMoveCmd = ciRsiSien;
            RobotUtils.DoStartProgram(Robot, Constants.DisplaySienSensorValues);
        }

        public void StartTCPCalibration(int ACalibrationType)
        {
            //
            //! Log
            //
            Logger.Logger.WriteLog("Function Call - StartTCPCalibration Type = " + ACalibrationType.ToString(), false);
            //
            //! Set Calib Type Mode
            //
            RobotUtils.DoSetKrlVariable(Constants.TCPCalibrationType, ACalibrationType, Robot);

            BcoReached = false;
            LastRobotMoveCmd = ciTCPCalibration;
            RobotUtils.DoStartProgram(Robot, Constants.TCPCalibration);
        }

        #endregion Clamping
    }
}