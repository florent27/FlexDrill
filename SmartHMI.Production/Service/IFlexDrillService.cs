// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFlexDrillService.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process;
using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.SmartHMI.Production.Log;
using Kuka.FlexDrill.SmartHMI.Production.Messages;
using KUKARoboter.KRCModel.Robot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kuka.FlexDrill.SmartHMI.Production.Service
{
    public interface IFlexDrillService
    {
        #region UI

        /// <summary>
        /// This event will be triggered if the job list has changed
        /// </summary>
        event EventHandler JobListChanged;

        /// <summary>
        /// This event will be triggered if a program is loaded.
        /// </summary>
        event EventHandler ProgramLoadedChanged;

        ///
        /// This event will be triggered when a job is loaded
        event EventHandler<CellProgram> JobLoadedChanged;

        /// <summary>
        /// Get the logger object to log information and errors in a text file.
        /// </summary>
        Logger Log { get; }

        /// <summary>
        /// Gets the initialization flag. It returns 'true' if the initialization succeeded.
        /// </summary>
        bool InitSucceeded { get; }

        /// <summary>
        /// Gets the Job List
        /// </summary>
        ObservableCollection<CellProgram> JobList { get; set; }

        /// <summary>
        /// Returns the error kxr key and the error parameters
        /// </summary>
        KeyValuePair<FlexDrillMessages, string[]> Error { get; }

        void RetryInitialization();

        /// <summary>
        /// IRobot Paremeter
        /// /// </summary>
        IRobot Robot { get; set; }

        /// <summary>
        /// Msg Handler
        /// </summary>
        MsgHandler Message { get; set; }

        #endregion UI

        #region Production

        IFlexDrillProcess Process { get; }

        event EventHandler CellInitializedChanged;

        void InitializeCell();

        void StartProgram();

        void StopProgram();

        void PauseProgram();

        #endregion Production

        #region Clamping

        void CalibrateAntiSliding();

        void CalibrateNormality();

        void TareForceSensor();

        #endregion Clamping

        #region Head management

        /// <summary>
        /// Grasp a head from a given slot.
        /// </summary>
        /// <param name="slotIndex">The 1-based slot index.</param>
        void GraspHead(int slotIndex);

        /// <summary>
        /// Drop a head in a given slot.
        /// </summary>
        /// <param name="slotIndex">The 1-based slot index.</param>
        void DropHead(int slotIndex);

        /// <summary>
        /// Turn on or off the vacuum.
        /// </summary>
        /// <param name="vacuumOn">True will activate the vacuum.</param>
        void ToggleVacuum(bool vacuumOn);

        void InitHead();

        /// <summary>
        /// Start active head process.
        /// </summary>
        /// <param name="processIndex">The 0-based process index.</param>
        void RunHeadProcess(int processIndex);

        /// <summary>
        /// Start Head Change
        /// </summary>

        void HeadChange(string AHeadToDrop, string AHeadToGrasp);

        void StartManualPositionning();

        #endregion Head management

        #region TCPCalibration

        void StartTCPCalibration(int ACalibrationType);

        #endregion TCPCalibration
    }
}