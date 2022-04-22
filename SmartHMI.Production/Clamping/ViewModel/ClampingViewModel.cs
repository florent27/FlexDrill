// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClampingViewModel.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Base;
using Kuka.FlexDrill.SmartHMI.Production.Clamping.Model;
using Kuka.FlexDrill.SmartHMI.Production.Exceptions;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using Kuka.FlexDrill.SmartHMI.Production.Messages;
using KukaRoboter.CoreUtil.Windows.Input;
using System;
using System.Windows.Threading;

namespace Kuka.FlexDrill.SmartHMI.Production.Clamping.ViewModel
{
    public class ClampingViewModel : FlexDrillViewModelBase
    {
        #region Constants and Fields

        private double clampingForce;

        private double slidingDistanceX;

        private double slidingDistanceY;

        private double normalityAngleX;

        private double normalityAngleY;

        private bool forceCalibrationState;

        private bool slidingCalibrationState;

        private bool normalityCalibrationState;

        private bool slidingCalibStarted;

        private bool normalityCalibStarted;

        private bool forceCalibStarted;

        private ClampingParameters currentClampingParameters;

        private RelayCommand saveCommand;

        private RelayCommand cancelCommand;

        private RelayCommand tareCommand;

        private RelayCommand calibrateNormalityCommand;

        private RelayCommand calibrateSlidingCommand;

        private RelayCommand lockNoseCommand;

        private RelayCommand unlockNoseCommand;

        private bool noseLocked;

        private DispatcherTimer UpdateDataTimer;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public ClampingViewModel()
           : base("FlexDrill_Clamping")
        {
        }

        #endregion Constructors and Destructor

        #region Interface

        public void InitializePlugin()
        {
            // Initialize settings data
            try
            {
                RegisterEvents();
                if (CurrentClampingParameters == null)
                {
                    CurrentClampingParameters = new ClampingParameters(Robot);
                }

                if (UpdateDataTimer == null)
                {
                    UpdateDataTimer = new DispatcherTimer(DispatcherPriority.DataBind)
                    {
                        Interval = TimeSpan.FromMilliseconds(500)
                    };
                    UpdateDataTimer.Tick += UpdateDataOnTimer;
                }
                UpdateDataTimer.Start();
                KrlVarHandler.WriteVariable(KrlVariableNames.ClampPlugginOpen, true, Robot);
            }
            catch (KrlVarNotDefinedException e)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                FlexDrillService.Log.WriteException(e);
            }
            catch (ArgumentException e)
            {
                FlexDrillService.Log.WriteException(e);
            }
            catch (Exception e)
            {
                FlexDrillService.Log.WriteException(e);
            }
        }

        public double ClampingForce
        {
            get
            {
                return clampingForce;
            }
            set
            {
                SetField(ref clampingForce, value);
            }
        }

        public ClampingParameters CurrentClampingParameters
        {
            get
            {
                return currentClampingParameters;
            }
            set
            {
                SetField(ref currentClampingParameters, value);
            }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(action => SaveChanges(), cond => CanSave);
                }
                return saveCommand;
            }
        }

        public RelayCommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {
                    cancelCommand = new RelayCommand(action => CancelChanges(), cond => CanCancel);
                }
                return cancelCommand;
            }
        }

        public RelayCommand TareCommand
        {
            get
            {
                if (tareCommand == null)
                {
                    tareCommand = new RelayCommand(o => FlexDrillService.TareForceSensor(), cond => IsInteractionEnabled);
                }
                return tareCommand;
            }
        }

        public RelayCommand CalibrateNormalityCommand
        {
            get
            {
                if (calibrateNormalityCommand == null)
                {
                    calibrateNormalityCommand = new RelayCommand(o => FlexDrillService.CalibrateNormality(),
                       cond => IsInteractionEnabled);
                }
                return calibrateNormalityCommand;
            }
        }

        public RelayCommand CalibrateSlidingCommand
        {
            get
            {
                if (calibrateSlidingCommand == null)
                {
                    calibrateSlidingCommand = new RelayCommand(o => FlexDrillService.CalibrateAntiSliding(),
                       cond => IsInteractionEnabled);
                }
                return calibrateSlidingCommand;
            }
        }

        public RelayCommand LockNoseCommand
        {
            get
            {
                if (lockNoseCommand == null)
                {
                    lockNoseCommand = new RelayCommand(o => LockNose(),
                       cond => IsInteractionEnabled);
                }
                return lockNoseCommand;
            }
        }

        public RelayCommand UnlockNoseCommand
        {
            get
            {
                if (unlockNoseCommand == null)
                {
                    unlockNoseCommand = new RelayCommand(o => UnlockNose(),
                       cond => IsInteractionEnabled);
                }
                return unlockNoseCommand;
            }
        }

        public bool CanClose()
        {
            // Close the plugin
            return true;
        }

        public double SlidingDistanceX
        {
            get
            {
                return slidingDistanceX;
            }
            set
            {
                SetField(ref slidingDistanceX, value);
            }
        }

        public double SlidingDistanceY
        {
            get
            {
                return slidingDistanceY;
            }
            set
            {
                SetField(ref slidingDistanceY, value);
            }
        }

        public double NormalityAngleX
        {
            get
            {
                return normalityAngleX;
            }
            set
            {
                SetField(ref normalityAngleX, value);
            }
        }

        public double NormalityAngleY
        {
            get
            {
                return normalityAngleY;
            }
            set
            {
                SetField(ref normalityAngleY, value);
            }
        }

        public bool IsInteractionEnabled { get; set; } = true;

        public bool ForceCalibrationState
        {
            get
            {
                return forceCalibrationState;
            }
            set
            {
                SetField(ref forceCalibrationState, value);
            }
        }

        public bool SlidingCalibrationState
        {
            get
            {
                return slidingCalibrationState;
            }
            set
            {
                SetField(ref slidingCalibrationState, value);
            }
        }

        public bool NormalityCalibrationState
        {
            get
            {
                return normalityCalibrationState;
            }
            set
            {
                SetField(ref normalityCalibrationState, value);
            }
        }

        public bool NoseLocked
        {
            get
            {
                return noseLocked;
            }
            set
            {
                SetField(ref noseLocked, value);
            }
        }

        #endregion Interface

        #region Properties

        private bool CanSave => CurrentClampingParameters != null && CurrentClampingParameters.ParametersHaveChanged();

        private bool CanCancel => CurrentClampingParameters != null && CurrentClampingParameters.ParametersHaveChanged();

        private bool NormalityCalibRunning
        {
            get
            {
                try
                {
                    return KrlVarHandler.ReadBoolVariable(KrlVariableNames.NormalityCalibrationRunning,
                       Robot);
                }
                catch (KrlVarNotDefinedException e)
                {
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                    FlexDrillService.Log.WriteException(e);
                    IsInteractionEnabled = false;
                }
                catch (ArgumentException ex)
                {
                    FlexDrillService.Log.WriteException(ex);
                    IsInteractionEnabled = false;
                }

                return false;
            }
        }

        private bool SlidingCalibRunning
        {
            get
            {
                try
                {
                    return KrlVarHandler.ReadBoolVariable(KrlVariableNames.SlidingCalibrationRunning,
                       Robot);
                }
                catch (KrlVarNotDefinedException e)
                {
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                    FlexDrillService.Log.WriteException(e);
                    IsInteractionEnabled = false;
                }
                catch (ArgumentException ex)
                {
                    FlexDrillService.Log.WriteException(ex);
                    IsInteractionEnabled = false;
                }

                return false;
            }
        }

        private bool ForceCalibRunning
        {
            get
            {
                try
                {
                    return KrlVarHandler.ReadBoolVariable(KrlVariableNames.ForceCalibrationRunning,
                       Robot);
                }
                catch (KrlVarNotDefinedException e)
                {
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                    FlexDrillService.Log.WriteException(e);
                    IsInteractionEnabled = false;
                }
                catch (ArgumentException ex)
                {
                    FlexDrillService.Log.WriteException(ex);
                    IsInteractionEnabled = false;
                }

                return false;
            }
        }

        #endregion Properties

        #region Methods

        private void CancelChanges()
        {
            CurrentClampingParameters.CancelChanges();
        }

        private void SaveChanges()
        {
            CurrentClampingParameters.SaveChanges(Robot);
        }

        private void RegisterEvents()
        {
            IsInteractionEnabled = false;

            Robot.KRLVariables[KrlVariableNames.ForceCalibrationOk].Changed += OnForceCalibStateChanged;
            Robot.KRLVariables[KrlVariableNames.SlidingCalibrationOk].Changed += OnSlidingCalibStateChanged;
            Robot.KRLVariables[KrlVariableNames.NormalityCalibrationOk].Changed += OnNormalityCalibStateChanged;
            Robot.KRLVariables[KrlVariableNames.ForceCalibrationRunning].Changed += OnForceCalibRunningChanged;
            Robot.KRLVariables[KrlVariableNames.SlidingCalibrationRunning].Changed += OnSlidingCalibRunningChanged;
            Robot.KRLVariables[KrlVariableNames.NormalityCalibrationRunning].Changed += OnNormalityCalibRunningChanged;
            Robot.KRLVariables[KrlVariableNames.NoseLocked].Changed += OnNoseLockedChanged;

            IsInteractionEnabled = true;
        }

        public void ReleaseEvents()
        {
            Robot.KRLVariables[KrlVariableNames.ForceCalibrationOk].Changed -= OnForceCalibStateChanged;
            Robot.KRLVariables[KrlVariableNames.SlidingCalibrationOk].Changed -= OnSlidingCalibStateChanged;
            Robot.KRLVariables[KrlVariableNames.NormalityCalibrationOk].Changed -= OnNormalityCalibStateChanged;
            Robot.KRLVariables[KrlVariableNames.ForceCalibrationRunning].Changed -= OnForceCalibRunningChanged;
            Robot.KRLVariables[KrlVariableNames.SlidingCalibrationRunning].Changed -= OnSlidingCalibRunningChanged;
            Robot.KRLVariables[KrlVariableNames.NormalityCalibrationRunning].Changed -= OnNormalityCalibRunningChanged;
            Robot.KRLVariables[KrlVariableNames.NoseLocked].Changed -= OnNoseLockedChanged;
        }

        private void OnNormalityCalibRunningChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
            if (NormalityCalibRunning)
            {
                // Disable buttons during the calibration process
                IsInteractionEnabled = false;

                // Show "is running" message
                MessageHandler.ShowInfoMessage(FlexDrillMessages.NormalityCalibrationRunning, new[] { string.Empty });

                normalityCalibStarted = true;
            }

            if (normalityCalibStarted && !NormalityCalibRunning)
            {
                IsInteractionEnabled = true;
                normalityCalibStarted = false;

                if (NormalityCalibrationState)
                {
                    // Show "Calibration succeeded" message
                    MessageHandler.ShowInfoMessage(FlexDrillMessages.NormalityCalibrationSucceeded, new[] { string.Empty });
                }
                else
                {
                    // Show "Calibration failed" message
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.NormalityCalibrationFailed, new[] { string.Empty });
                }
            }
        }

        private void OnSlidingCalibRunningChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
            if (SlidingCalibRunning)
            {
                // Disable buttons during the calibration process
                IsInteractionEnabled = false;

                // Show "is running" message
                MessageHandler.ShowInfoMessage(FlexDrillMessages.SlidingCalibrationRunning, new[] { string.Empty });

                slidingCalibStarted = true;
            }

            if (slidingCalibStarted && !SlidingCalibRunning)
            {
                IsInteractionEnabled = true;
                slidingCalibStarted = false;

                if (ForceCalibrationState)
                {
                    // Show "Calibration succeeded" message
                    MessageHandler.ShowInfoMessage(FlexDrillMessages.SlidingCalibrationSucceeded, new[] { string.Empty });
                }
                else
                {
                    // Show "Calibration failed" message
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.SlidingCalibrationFailed, new[] { string.Empty });
                }
            }
        }

        private void OnForceCalibRunningChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
            if (ForceCalibRunning)
            {
                // Disable buttons during the calibration process
                IsInteractionEnabled = false;

                // Show "is running" message
                MessageHandler.ShowInfoMessage(FlexDrillMessages.ForceCalibrationRunning, new[] { string.Empty });

                forceCalibStarted = true;
            }

            if (forceCalibStarted && !ForceCalibRunning)
            {
                IsInteractionEnabled = true;
                forceCalibStarted = false;

                if (ForceCalibrationState)
                {
                    // Show "Calibration succeeded" message
                    MessageHandler.ShowInfoMessage(FlexDrillMessages.ForceCalibrationSucceeded, new[] { string.Empty });
                }
                else
                {
                    // Show "Calibration failed" message
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.ForceCalibrationFailed, new[] { string.Empty });
                }
            }
        }

        private void OnNormalityCalibStateChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
            try
            {
                NormalityCalibrationState = KrlVarHandler.ReadBoolVariable(KrlVariableNames.NormalityCalibrationOk, Robot);
            }
            catch (KrlVarNotDefinedException ex)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { ex.VariableName });
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
            catch (ArgumentException ex)
            {
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
        }

        private void OnSlidingCalibStateChanged(object sender, EventArgs e)
        {
            try
            {
                SlidingCalibrationState = KrlVarHandler.ReadBoolVariable(KrlVariableNames.SlidingCalibrationOk, Robot);
            }
            catch (KrlVarNotDefinedException ex)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { ex.VariableName });
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
            catch (ArgumentException ex)
            {
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
        }

        private void OnForceCalibStateChanged(object sender, EventArgs e)
        {
            try
            {
                ForceCalibrationState = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ForceCalibrationOk, Robot);
            }
            catch (KrlVarNotDefinedException ex)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { ex.VariableName });
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
            catch (ArgumentException ex)
            {
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
        }

        private void OnNoseLockedChanged(object sender, EventArgs e)
        {
            try
            {
                int iNoseLocked = KrlVarHandler.ReadIntVariable(KrlVariableNames.NoseLocked, Robot);

                NoseLocked = (iNoseLocked == 1);
            }
            catch (KrlVarNotDefinedException ex)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { ex.VariableName });
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
            catch (ArgumentException ex)
            {
                FlexDrillService.Log.WriteException(ex);
                IsInteractionEnabled = false;
            }
        }

        private void UpdateButtonStates()
        {
            TareCommand.RaiseCanExecuteChanged();
            CalibrateNormalityCommand.RaiseCanExecuteChanged();
            CalibrateNormalityCommand.RaiseCanExecuteChanged();
        }

        private void UpdateDataOnTimer(object sender, EventArgs e)
        {
            NormalityAngleX = KrlVarHandler.ReadRealVariable(KrlVariableNames.NormalityX, Robot);
            NormalityAngleY = KrlVarHandler.ReadRealVariable(KrlVariableNames.NormalityY, Robot);
            SlidingDistanceX = KrlVarHandler.ReadRealVariable(KrlVariableNames.SlidingX, Robot);
            SlidingDistanceY = KrlVarHandler.ReadRealVariable(KrlVariableNames.SlidingY, Robot);
            ClampingForce = KrlVarHandler.ReadRealVariable(KrlVariableNames.ForceZ, Robot);
            //! Control If Parameters have changed
            SaveCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        /// <inheritdoc />
        protected override void OnDisconnecting()
        {
            ReleaseEvents();
            UpdateDataTimer.Stop();
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampPlugginOpen, false, Robot);
        }

        private void LockNose()
        {
            KrlVarHandler.WriteVariable(KrlVariableNames.LockNose, true, Robot);
        }

        private void UnlockNose()
        {
            KrlVarHandler.WriteVariable(KrlVariableNames.UnlockNose, true, Robot);
        }

        #endregion Methods
    }
}