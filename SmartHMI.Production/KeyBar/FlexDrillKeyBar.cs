// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillKeyBar.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Base;
using Kuka.FlexDrill.SmartHMI.Production.Exceptions;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using Kuka.FlexDrill.SmartHMI.Production.Messages;
using KukaRoboter.Common.ApplicationServices.UserkeyBar;
using KukaRoboter.Common.ApplicationServices.UserkeyBar.Implementation;
using KUKARoboter.KRCModel.Robot.Interpreter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace Kuka.FlexDrill.SmartHMI.Production.KeyBar
{
    public class FlexDrillKeyBar : FlexDrillViewModelBase, IUserkeyBarRepository
    {
        #region Constants and Fields

        private const string InitCellKeyId = "InitCell";

        private const string StartKeyId = "Start";

        private const string PauseKeyId = "Pause";

        private const string StopKeyId = "Stop";

        private const string FlyOverKeyId = "FlyOver";

        private const string LightKeyId = "Light";

        private const string LaserKeyId = "Laser";

        private const string ClampOnlyKeyId = "ClampOnly";

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private List<IUserkeyBar> flexDrillKeyBars;

        private IUserkey initCellKey;

        private IUserkey startKey;

        private IUserkey pauseKey;

        private IUserkey stopKey;

        private IUserkey flyOverKey;

        private IUserkey lightKey;

        private IUserkey laserKey;

        private IUserkey clampOnlyKey;

        private bool commissioningDone;

        private bool LoadNextJobOn;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public FlexDrillKeyBar()
           : base("FlexDrill_KeyBar")
        {
        }

        #endregion Constructors and Destructor

        #region IUserkeyBarRepository Members

        public bool TryGetUserkeyBar(string barID, out IUserkeyBar userKeyBar)
        {
            userKeyBar = UserkeyBarCollection.Single(ukb => ukb.BarID == barID);
            return userKeyBar != null;
        }

        public IEnumerable<IUserkeyBar> UserkeyBarCollection
        {
            get
            {
                if (flexDrillKeyBars != null && flexDrillKeyBars.Count != 0)
                {
                    return flexDrillKeyBars;
                }

                try
                {
                    CreateRepository();
                    UpdateIcons();
                    RegisterEvents();
                }
                catch (KrlVarNotDefinedException e)
                {
                    MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                    FlexDrillService.Log.WriteException(e);
                }
                catch (Exception ex)
                {
                    FlexDrillService.Log.WriteException(ex);
                }

                return flexDrillKeyBars;
            }
        }

        #endregion IUserkeyBarRepository Members

        #region Methods

        private void RegisterEvents()
        {
            FlexDrillService.CellInitializedChanged += OnCellInitializedChanged;
            FlexDrillService.ProgramLoadedChanged += OnProgramLoadedChanged;
            RobotInterpreter.ProgramStateChanged += OnProgramStateChanged;

            Robot.KRLVariables[KrlVariableNames.CellInitIsRunning].Changed += CellInitIsRunningChanged;
            Robot.KRLVariables[KrlVariableNames.HeadInitIsRunning].Changed += HeadInitIsRunningChanged;
            Robot.KRLVariables[KrlVariableNames.HeadProcessIsRunning].Changed += HeadPRocessIsRunning;
            Robot.KRLVariables[KrlVariableNames.GraspIsRunning].Changed += GraspIsRunningChanged;
            Robot.KRLVariables[KrlVariableNames.DropIsRunning].Changed += DropIsRunningChanged;
            Robot.KRLVariables[KrlVariableNames.StartFlyOver].Changed += StartFlyOverChanged;
            Robot.KRLVariables[KrlVariableNames.LightIsOn].Changed += LightIsOnChanged;
            Robot.KRLVariables[KrlVariableNames.LaserIsOn].Changed += LaserIsOnChanged;
            Robot.KRLVariables[KrlVariableNames.CommissioningDone].Changed += CommissioningDoneChanged;
            Robot.KRLVariables[KrlVariableNames.ClampOnlyActivated].Changed += ClampOnlyActivatedChanged;
            Robot.KRLVariables[KrlVariableNames.LoadNextJob].Changed += LoadNextJobChanged;

            FlexDrillService.JobListChanged += JobListChanged;
        }

        private void LoadNextJobChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void JobListChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void ClampOnlyActivatedChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void CommissioningDoneChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void LaserIsOnChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void LightIsOnChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void StartFlyOverChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void DropIsRunningChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void GraspIsRunningChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void HeadPRocessIsRunning(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void HeadInitIsRunningChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void CellInitIsRunningChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void OnProgramLoadedChanged(object sender, EventArgs e)
        {
            UpdateIcons();
        }

        private void OnProgramStateChanged(object sender, ProgramStateChangedEventArgs ea)
        {
            UpdateIcons();
        }

        private void OnCellInitializedChanged(object sender, EventArgs e)
        {
            MessageHandler.ShowInfoMessage(
               FlexDrillService.Process.CellInitialized
                  ? FlexDrillMessages.CellInitSucceeded
                  : FlexDrillMessages.CellInitFailed, new[] { string.Empty });
        }

        private void OnKeyEvent(KeyEventArgs args)
        {
            if (!args.IsDown)
            {
                return;
            }

            switch (args.Key.ID)
            {
                // Initialize cell button has been pressed
                case InitCellKeyId:
                    InitializeCell();
                    break;

                // Start button has been pressed
                case StartKeyId:
                    StartProgram();
                    break;

                // Pause button has been pressed
                case PauseKeyId:
                    PauseProgram();
                    break;

                // Stop button has been pressed
                case StopKeyId:
                    StopProgram();
                    break;

                // FlyOver
                case FlyOverKeyId:
                    ActivateFlyOver();
                    break;

                // Light
                case LightKeyId:
                    ChangeLightState();
                    break;

                // Laser
                case LaserKeyId:
                    ChangeLaserState();
                    break;

                // ClampOnly
                case ClampOnlyKeyId:
                    ActivateClampOnly();
                    break;

                default:
                    // Do nothing
                    break;
            }
        }

        private void ActivateClampOnly()
        {
            try
            {
                KrlVarHandler.WriteVariable(KrlVariableNames.ClampOnlyActivated, true, Robot);
            }
            catch (KrlVarNotDefinedException e)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                FlexDrillService.Log.WriteException(e);
            }
        }

        private void ChangeLaserState()
        {
            try
            {
                bool CurrentLaserState = KrlVarHandler.ReadBoolVariable(KrlVariableNames.LaserIsOn, Robot);
                KrlVarHandler.WriteVariable(CurrentLaserState ? KrlVariableNames.LaserOff : KrlVariableNames.LaserOn, true, Robot);
            }
            catch (KrlVarNotDefinedException e)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                FlexDrillService.Log.WriteException(e);
            }
        }

        private void ChangeLightState()
        {
            try
            {
                bool CurrentLightState = KrlVarHandler.ReadBoolVariable(KrlVariableNames.LightIsOn, Robot);
                KrlVarHandler.WriteVariable(CurrentLightState ? KrlVariableNames.LightOff : KrlVariableNames.LightOn, true, Robot);
            }
            catch (KrlVarNotDefinedException e)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                FlexDrillService.Log.WriteException(e);
            }
        }

        private void ActivateFlyOver()
        {
            try
            {
                KrlVarHandler.WriteVariable(KrlVariableNames.StartFlyOver, true, Robot);
            }
            catch (KrlVarNotDefinedException e)
            {
                MessageHandler.ShowErrorMessage(FlexDrillMessages.KrlVariableNotDefined, new[] { e.VariableName });
                FlexDrillService.Log.WriteException(e);
            }
        }

        private void InitializeCell()
        {
            if (NoProgramIsRunning() && InitIsNotRunning() && (!LoadNextJobOn))
            {
                // Thread safe call
                dispatcher.Invoke(
                   () =>
                   {
                       try
                       {
                           FlexDrillService.InitializeCell();
                       }
                       catch (Exception e)
                       {
                           // TODO: improve this try/catch when the possible exception types will be known
                           MessageHandler.ShowErrorMessage(FlexDrillMessages.CellInitFailed, new[] { e.Message });

                           // Log
                           FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                              "An unexpected error occurred while trying to initialize the cell. " + e.Message + ".",
                              e.StackTrace);
                       }
                   });
            }
        }

        private bool InitIsNotRunning()
        {
            bool CellInitIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CellInitIsRunning, Robot);
            bool InitHeadIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.HeadInitIsRunning, Robot);

            if (CellInitIsRunning || InitHeadIsRunning)
            {
                MessageHandler.ShowInfoMessage(FlexDrillMessages.InitIsAlreadyRunning,
                   new[] { string.Empty });
                return false;
            }

            return true;
        }

        private bool ProgramIsLoaded()
        {
            if (FlexDrillService.JobList.Count > 0)
            {
                return true;
            }

            MessageHandler.ShowInfoMessage(FlexDrillMessages.NoProgranLoadedInfo,
               new[] { string.Empty });
            return false;
        }

        private bool NoProgramIsRunning()
        {
            if (RobotInterpreter.ProgramState == ProStates.Active)
            {
                MessageHandler.ShowInfoMessage(FlexDrillMessages.ProgramRunningInfo,
                   new[] { string.Empty });
                return false;
            }

            return true;
        }

        private bool CommissioningIsDone()
        {
            if (!commissioningDone)
            {
                MessageHandler.ShowInfoMessage(FlexDrillMessages.CommissioningNotDone,
                   new[] { string.Empty });
            }

            return commissioningDone;
        }

        private void StartProgram()
        {
            if (NoProgramIsRunning() && ProgramIsLoaded() && CommissioningIsDone() && (!LoadNextJobOn))
            {
                // Thread safe call
                dispatcher.Invoke(
                   () =>
                   {
                       try
                       {
                           FlexDrillService.StartProgram();
                       }
                       catch (Exception e)
                       {
                           // TODO: improve this try/catch when the possible exception types will be known
                           MessageHandler.ShowErrorMessage(FlexDrillMessages.StartCycleFailed, new[] { e.Message });

                           // Log
                           FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                              $"An unexpected error occurred while trying to start the program. {e.Message}",
                              e.StackTrace);
                       }
                   });
            }
        }

        private bool MessageDisplayed()
        {
            KrlVarHandler.WriteVariable(KrlVariableNames.DisplayProgramLoading, true, Robot);
            bool bMsgDisplayed = false;
            while (!bMsgDisplayed)
            {
                bMsgDisplayed = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ProgramLoadingDisplayed, Robot);
            }
            KrlVarHandler.WriteVariable(KrlVariableNames.ProgramLoadingDisplayed, false, Robot);
            return true;
        }

        private bool ProgramIsRunning()
        {
            if (RobotInterpreter.ProgramState != ProStates.Active)
            {
                MessageHandler.ShowInfoMessage(FlexDrillMessages.ProgramNotRunningInfo,
                   new[] { string.Empty });
                return false;
            }

            return true;
        }

        private void PauseProgram()
        {
            if (ProgramIsLoaded() && ProgramIsRunning())
            {
                // Thread safe call
                dispatcher.Invoke(
                   () =>
                   {
                       try
                       {
                           FlexDrillService.PauseProgram();
                       }
                       catch (Exception e)
                       {
                           // TODO: improve this try/catch when the possible exception types will be known
                           MessageHandler.ShowErrorMessage(FlexDrillMessages.PauseCyleFiled, new[] { e.Message });

                           // Log
                           FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                              $"An unexpected error occurred while trying to pause the program. {e.Message}",
                              e.StackTrace);
                       }
                   });
            }
        }

        private void StopProgram()
        {
            if (ProgramIsLoaded() && ProgramIsRunning())
            {
                // Thread safe call
                dispatcher.Invoke(
                   () =>
                   {
                       try
                       {
                           FlexDrillService.StopProgram();
                       }
                       catch (Exception e)
                       {
                           // TODO: improve this try/catch when the possible exception types will be known
                           MessageHandler.ShowErrorMessage(FlexDrillMessages.StopCycleFailed, new[] { e.Message });

                           // Log
                           FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                              $"An unexpected error occurred while trying to stop the program. {e.Message}",
                              e.StackTrace);
                       }
                   });
            }
        }

        private string GetCriticalInfo(KeyEventArgs args)
        {
            return "Critical info callback.";
        }

        private void CreateRepository()
        {
            flexDrillKeyBars = new List<IUserkeyBar>
         {
            CreateProductionKeyBar(),
            CreateFlyOverKeyBar(),
            CreateLightLaserKeyBar()
         };
        }

        private IUserkeyBar CreateFlyOverKeyBar()
        {
            var keyBar = UserkeybarManager.CreateBar("WorkMode", "WorkMode", OnKeyEvent, GetCriticalInfo);

            ILayoutedArea keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.FlyOverEnabled);
            flyOverKey = UserkeybarManager.CreateKey(FlyOverKeyId, false, 0, keyLayout, keyBar);

            keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.ClampOnlyEnable);
            clampOnlyKey = UserkeybarManager.CreateKey(ClampOnlyKeyId, false, 1, keyLayout, keyBar);

            return keyBar;
        }

        private IUserkeyBar CreateLightLaserKeyBar()
        {
            var keyBar = UserkeybarManager.CreateBar("Light/Laser", "Light/Laser", OnKeyEvent, GetCriticalInfo);

            ILayoutedArea keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.LightOff);
            lightKey = UserkeybarManager.CreateKey(LightKeyId, false, 0, keyLayout, keyBar);

            keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.LaserOff);
            laserKey = UserkeybarManager.CreateKey(LaserKeyId, false, 1, keyLayout, keyBar);

            return keyBar;
        }

        private IUserkeyBar CreateProductionKeyBar()
        {
            var keyBar = UserkeybarManager.CreateBar("Production", "Production", OnKeyEvent, GetCriticalInfo);

            ILayoutedArea keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.InitCell);
            initCellKey = UserkeybarManager.CreateKey(InitCellKeyId, false, 0, keyLayout, keyBar);

            keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.Start);
            startKey = UserkeybarManager.CreateKey(StartKeyId, false, 1, keyLayout, keyBar);

            keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.Pause);
            pauseKey = UserkeybarManager.CreateKey(PauseKeyId, false, 2, keyLayout, keyBar);

            keyLayout = new LayoutedArea();
            keyLayout.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter, IconPaths.Stop);
            stopKey = UserkeybarManager.CreateKey(StopKeyId, false, 3, keyLayout, keyBar);

            return keyBar;
        }

        private void EnableInitCellKey(bool enable)
        {
            initCellKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.InitCell : IconPaths.InitCellDisabled);
        }

        private void EnableStartKey(bool enable)
        {
            startKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.Start : IconPaths.StartDisabled);
        }

        private void EnablePauseKey(bool enable)
        {
            pauseKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.Pause : IconPaths.PauseDisabled);
        }

        private void EnableStopKey(bool enable)
        {
            stopKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.Stop : IconPaths.StopDisabled);
        }

        private void EnableFlyOverKey(bool enable)
        {
            flyOverKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.FlyOverEnabled : IconPaths.FlyOverDisabled);
        }

        private void EnableLightKey(bool enable)
        {
            lightKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.LightOn : IconPaths.LightOff);
        }

        private void EnableLaserKey(bool enable)
        {
            laserKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.LaserOn : IconPaths.LaserOff);
        }

        private void EnableClampOnlyKey(bool enable)
        {
            clampOnlyKey.View.SetImage(LayoutPosition.VCenter | LayoutPosition.HCenter,
               enable ? IconPaths.ClampOnlyEnable : IconPaths.ClampOnlyDisable);
        }

        private void UpdateIcons()
        {
            bool programIsLoaded = (FlexDrillService.JobList.Count > 0);
            bool programIsRunning = RobotInterpreter.ProgramState == ProStates.Active;
            bool cellInitIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CellInitIsRunning, Robot);
            commissioningDone = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CommissioningDone, Robot);
            bool startFlyOver = KrlVarHandler.ReadBoolVariable(KrlVariableNames.StartFlyOver, Robot);
            bool LightIsOn = KrlVarHandler.ReadBoolVariable(KrlVariableNames.LightIsOn, Robot);
            bool LaserIsOn = KrlVarHandler.ReadBoolVariable(KrlVariableNames.LaserIsOn, Robot);
            bool clampOnlyActivated = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ClampOnlyActivated, Robot);
            LoadNextJobOn = KrlVarHandler.ReadBoolVariable(KrlVariableNames.LoadNextJob, Robot);

            // Enable cell initialization key if no program and no initialization is running.
            EnableInitCellKey(!programIsRunning && !cellInitIsRunning && !IsHeadManagementActionRunning() && (!LoadNextJobOn));

            // Enable start key if no program is running.
            EnableStartKey(programIsLoaded && !programIsRunning && !cellInitIsRunning && !IsHeadManagementActionRunning() && commissioningDone && (!LoadNextJobOn));

            // Enable pause key if a program is running.
            EnablePauseKey(programIsLoaded && programIsRunning && !cellInitIsRunning && !IsHeadManagementActionRunning());

            // Enable stop key if a program is running.
            EnableStopKey(programIsLoaded && programIsRunning && !cellInitIsRunning && !IsHeadManagementActionRunning());

            // Disable FlyOver key if FlyOver is on already
            EnableFlyOverKey(!startFlyOver);

            //! Change Light Icon
            EnableLightKey(!LightIsOn);

            //! Change Laser Icon
            EnableLaserKey(!LaserIsOn);

            // Disable ClampOnly key if ClampOnly is on already
            EnableClampOnlyKey(!clampOnlyActivated);
        }

        private bool IsHeadManagementActionRunning()
        {
            bool initHeadIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.HeadInitIsRunning, Robot);
            bool processIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.HeadProcessIsRunning, Robot);
            bool graspIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.GraspIsRunning, Robot);
            bool dropIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.DropIsRunning, Robot);

            return (initHeadIsRunning || processIsRunning || graspIsRunning || dropIsRunning);
        }

        #endregion Methods
    }
}