// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillMessages.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kuka.FlexDrill.SmartHMI.Production.Messages
{
    public enum FlexDrillMessages
    {
        // Production
        UnexpectedError = 0,

        NoProgramInFolder = 1,
        ProgramFolderNotFound = 2,
        ConfigFileNotFound = 3,
        FolderNameEmpty = 4,
        LoadProrgamListFailed = 5,
        LoadCellProgramFailed = 6,
        StartCycleFailed = 7,
        PauseCyleFiled = 8,
        StopCycleFailed = 9,
        CellInitFailed = 10,
        NoProgranLoadedInfo = 11,
        ProgramRunningInfo = 12,
        ProgramNotRunningInfo = 13,
        CellInitSucceeded = 14,
        CommissioningNotDone = 15,
        AllStatusReseted = 16,
        OperationsStatusReseted = 17,
        WorkingPointsStatusReseted = 18,
        TcpVisionSelected = 19,
        InitIsAlreadyRunning = 20,
        NoJobTobeDone = 21,
        ProgramIsBeingLoaded = 22,
        TcpMachiningSelected = 23,

        // Head management
        GraspHeadFailed = 30,

        DropHeadFailed = 31,
        ToggleVacuumFailed = 32,
        InitHeadFailed = 33,
        RunHeadProcessFailed = 34,

        // Clamping
        ForceCalibrationSucceeded = 40,

        SlidingCalibrationSucceeded = 41,
        NormalityCalibrationSucceeded = 42,
        ForceCalibrationFailed = 43,
        SlidingCalibrationFailed = 44,
        NormalityCalibrationFailed = 45,
        ForceCalibrationRunning = 46,
        SlidingCalibrationRunning = 47,
        NormalityCalibrationRunning = 48,

        // Common
        KrlVariableNotDefined = 100
    }
}