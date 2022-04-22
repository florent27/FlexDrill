// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFlexDrillProcess.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.CustomClasses;
using System;
using System.Collections.ObjectModel;

namespace Kuka.FlexDrill.Process
{
    public interface IFlexDrillProcess
    {
        #region Interface

        //
        //! Properties
        //
        CellProgram CurrentCellProgram { get; set; }

        ObservableCollection<CellProgram> Programs { get; set; }

        CellProgram GeneratedCellProgram { get; set; }

        bool CellInitialized { get; set; }

        //
        //! Production Management
        //
        event EventHandler CellInitializedChanged;

        event EventHandler LoadNextJobChanged;

        CellProgram ParseXmlFile(string AXMLFilePath);

        bool GenerateSrcFile(CellProgram ACellProgram, string AOutputSRCFolderPath);

        void RemoveProgram(string programName);

        void AddProgram(string programPath);

        void RenameProgram(string oldName, string newName);

        void StartCycle();

        void PauseCycle();

        void AbortCycle();

        void InitCell();

        //
        //! Seti-Tec
        //
        void DropHead(int ARackID);

        void GraspHead(int ARackID);

        void InitHead();

        void RunProcess(int AProcessID);

        void StartVacuum(bool AStartVacuum);

        void HeadChange(string AHeadToDrop, string AHeadToGrasp);

        void StartManualPositionning();

        //
        //! Clamping
        //
        void TareForceSensor();

        void StartAntiSliddingCalibration();

        void StartNormalityCalibration();

        //
        //! TCP Calibration
        //
        void StartTCPCalibration(int ACalibrationType);

        #endregion Interface
    }
}