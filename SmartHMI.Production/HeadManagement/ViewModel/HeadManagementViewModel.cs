// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeadManagementViewModel.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using Ade.ObjectStore;
using Kuka.FlexDrill.SmartHMI.Production.Base;
using Kuka.FlexDrill.SmartHMI.Production.Exceptions;
using Kuka.FlexDrill.SmartHMI.Production.HeadManagement.Model;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using Kuka.FlexDrill.SmartHMI.Production.Messages;

using KukaRoboter.CoreUtil.Windows.Input;

using KUKARoboter.KRCModel.Robot.Interpreter;
using KUKARoboter.KRCModel.Robot.Variables;

namespace Kuka.FlexDrill.SmartHMI.Production.HeadManagement.ViewModel
{
   public class HeadManagementViewModel : FlexDrillViewModelBase
   {
      #region Constants and Fields

      private const int ActiveHeadIndex = -1;

      private List<FlexDrillHead> heads;

      private FlexDrillHead activeHead;

      private List<FlexDrillSlot> slots;

      private RelayCommand graspCommand;

      private RelayCommand dropCommand;

      private FlexDrillSlot selectedSlot;

      private RelayCommand vacuumCommand;

      private RelayCommand initHeadCommand;

      private RelayCommand runProcessCommand;

      private RelayCommand headChangeCommand;

      private int selectedHeadProcess;

      private FlexDrillHead selectedHead;

      private bool vacuumIsActive;

      private bool headInitIsRunning;

      private bool headProcessIsRunning;

      private bool graspIsRunning;

      private bool dropIsRunning;

      private bool initCellIsRunning;

      private DispatcherTimer RefreshVarTimer;

      private bool PrevHeadInitIsRunning;

      private bool PrevHeadProcessIsRunning;

      private bool PrevGraspIsRunning;

      private bool PrevDropIsRunning;

      private bool PrevInitCellIsRunning;

      private bool PrevVacuumIsActive;

      private bool FOnTimer = false;

      #endregion

      #region Constructors and Destructor

      public HeadManagementViewModel()
         : base("FlexDrill_HeadManagement")
      {
      }

      #endregion

      #region Interface

      public void InitializePlugin()
      {
         if (FlexDrillService.InitSucceeded)
         {
            try
            {
               LoadHeads();

               LoadSlots();

               LoadVacuumState();

               LoadFirstValues();

               FOnTimer = false;

               RefreshVarTimer = new DispatcherTimer(DispatcherPriority.DataBind)
               {
                  Interval = TimeSpan.FromMilliseconds(100)
               };
               RefreshVarTimer.Tick += OnRefreshVar;
               RefreshVarTimer.Start();

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
         }
         else
         {
            HandleError();
         }
      }

      public bool CanReadVacuumState { get; private set; }

      public void LoadVacuumState()
      {
         SetVacuumState();

         // This line will only be reached if the variable could be read.
         CanReadVacuumState = true;
      }

      public void LoadHeads()
      {
         Heads = HeadManager.LoadHeads(Robot);
         SetActiveHead();
         SelectedHead = Heads?.FirstOrDefault();
      }

      public List<FlexDrillHead> Heads
      {
         get
         {
            return heads;
         }
         set
         {
            SetField(ref heads, value);
         }
      }

      public void SetActiveHead()
      {
         ActiveHead = Heads.FirstOrDefault(h => h.SlotIndex == ActiveHeadIndex);
      }

      public FlexDrillHead ActiveHead
      {
         get
         {
            return activeHead;
         }
         set
         {
            SetField(ref activeHead, value);
         }
      }

      public List<string> HeadProcesses => new List<string> { "Pset0", "Pset1", "Pset2", "Pset3" };

      public List<FlexDrillSlot> Slots
      {
         get
         {
            return slots;
         }
         set
         {
            SetField(ref slots, value);
         }
      }

      public RelayCommand GraspCommand
      {
         get
         {
            if (graspCommand == null)
            {
               graspCommand = new RelayCommand(action => GraspHead(), cond => CanGraspHead);
            }
            return graspCommand;
         }
      }

      public bool CanGraspHead
      {
         get
         {
            bool initSucceeded = FlexDrillService.InitSucceeded;
            bool slotIsSelected = SelectedSlot != null;
            bool headAvailableInSlot = SelectedSlot?.Head != null;
            bool noActiveHead = ActiveHead == null;
            bool noProcessRunning = !GraspIsRunning;
            bool headPresence = (SelectedSlot?.Occupied == true);
            bool ProgramIsLoaded = (RobotInterpreter.ProgramState != ProStates.Free);
            bool isInitCellRunning = InitCellIsRunning;
            return initSucceeded && slotIsSelected && headAvailableInSlot && noActiveHead && noProcessRunning && headPresence && (!ProgramIsLoaded) && (!isInitCellRunning);
         }
      }

      public RelayCommand DropCommand
      {
         get
         {
            if (dropCommand == null)
            {
               dropCommand = new RelayCommand(action => DropHead(), cond => CanDropHead);
            }
            return dropCommand;
         }
      }

      public bool CanDropHead
      {
         get
         {
            bool initSucceeded = FlexDrillService.InitSucceeded;
            bool slotIsSelected = SelectedSlot != null;
            bool selectedSlotIsEmpty = SelectedSlot?.Head == null;
            bool noDropRunning = !DropIsRunning;
            bool headPresence = (SelectedSlot?.Occupied == true);
            bool ProgramIsLoaded = (RobotInterpreter.ProgramState != ProStates.Free);
            bool isInitCellRunning = InitCellIsRunning;

            return initSucceeded && slotIsSelected && selectedSlotIsEmpty && noDropRunning && (!headPresence) && (!ProgramIsLoaded) && (!isInitCellRunning);
         }
      }

      public FlexDrillSlot SelectedSlot
      {
         get
         {
            return selectedSlot;
         }
         set
         {
            if (value == selectedSlot)
            {
               selectedSlot = null;
            }
            else
            { 
               SetField(ref selectedSlot, value);
            }

         }
      }

      public RelayCommand VacuumCommand
      {
         get
         {
            if (vacuumCommand == null)
            {
               vacuumCommand = new RelayCommand(action => ToggleVacuum(), cond => CanToggleVacuum);
            }
            return vacuumCommand;
         }
      }

      public bool CanToggleVacuum => FlexDrillService.InitSucceeded && CanReadVacuumState;

      /// <summary>
      /// Head can be initialized if the plug-in initialization succeeded, a head is active and the head's initialization is NOT
      /// running.
      /// </summary>
      public bool CanInitializeHead
      {
         get
         {
            bool initSucceeded = FlexDrillService.InitSucceeded;
            bool isHeadActive = ActiveHead != null;
            bool noHeadInitIsRunning = !HeadInitIsRunning;
            bool ProgramIsActive = (RobotInterpreter.ProgramState == ProStates.Active);
            bool isInitCellRunning = InitCellIsRunning;


            return initSucceeded && isHeadActive && noHeadInitIsRunning && (!ProgramIsActive) && (!isInitCellRunning);
         }
      }
         
      public int SelectedHeadProcess
      {
         get
         {
            return selectedHeadProcess;
         }
         set
         {
            SetField(ref selectedHeadProcess, value);
         }
      }

      public bool VacuumIsActive
      {
         get
         {
            return vacuumIsActive;
         }

         set
         {
            SetField(ref vacuumIsActive, value);
         }
      }

      public RelayCommand InitHeadCommand
      {
         get
         {
            if (initHeadCommand == null)
            {
               initHeadCommand = new RelayCommand(action => InitializeHead(), cond => CanInitializeHead);
            }
            return initHeadCommand;
         }
      }

      /// <summary>
      /// Head process can be started if the plug-in initialization succeeded, a head is active and the head's process is NOT
      /// running.
      /// </summary>
      public bool CanRunHeadProcess
      {
         get
         {
            bool initSucceeded = FlexDrillService.InitSucceeded;
            bool isHeadActive = ActiveHead != null;
            bool noHeadProcessIsRunning = !HeadProcessIsRunning;
            bool ProgramIsLoaded = (RobotInterpreter.ProgramState != ProStates.Free);
            bool isInitCellRunning = InitCellIsRunning;

            return initSucceeded && isHeadActive && noHeadProcessIsRunning && (!ProgramIsLoaded) && (!isInitCellRunning);
         }
      }
         
      public RelayCommand RunProcessCommand
      {
         get
         {
            if (runProcessCommand == null)
            {
               runProcessCommand = new RelayCommand(action => RunProcess(), cond => CanRunHeadProcess);
            }

            return runProcessCommand;
         }
      }

      public RelayCommand HeadChangeCommand
      {
         get
         {
            if (headChangeCommand == null)
            {
               headChangeCommand = new RelayCommand(action => StartHeadChange(), cond => CanStartHeadChange);
            }

            return headChangeCommand;
         }
      }

      public bool CanStartHeadChange
      {
         get
         {
            bool DropAndGraspNull = ((ActiveHead == null) && (SelectedSlot?.Head == null));
            return (! DropAndGraspNull);

         }
      }

      public FlexDrillHead SelectedHead
      {
         get
         {
            return selectedHead;
         }
         set
         {
            SetField(ref selectedHead, value);
         }
      }

      public bool HeadInitIsRunning
      {
         get
         {
            return headInitIsRunning;
         }
         set
         {
            SetField(ref headInitIsRunning, value);
         }
      }

      public bool HeadProcessIsRunning
      {
         get
         {
            return headProcessIsRunning;
         }
         set
         {
            SetField(ref headProcessIsRunning, value);
         }
      }

      public bool GraspIsRunning
      {
         get
         {
            return graspIsRunning;
         }
         set
         {
            SetField(ref graspIsRunning, value);
         }
      }

      public bool DropIsRunning
      {
         get
         {
            return dropIsRunning;
         }
         set
         {
            SetField(ref dropIsRunning, value);
         }
      }

      public bool InitCellIsRunning
      {
         get
         {
            return initCellIsRunning;
         }
         set
         {
            SetField(ref initCellIsRunning, value);
         }
      }

      #endregion

      #region Methods

      private void InitializeHead()
      {
         try
         {
            FlexDrillService.InitHead();
         }
         catch (Exception e)
         {
            MessageHandler.ShowErrorMessage(FlexDrillMessages.InitHeadFailed, new[] { e.Message });

            // Log
            FlexDrillService.Log.WriteMessage(TraceEventType.Error,
               $"An unexpected error while trying to initialized head. {e.Message}.",
               e.StackTrace);
         }
      }

      private void RunProcess()
      {
         try
         {
            FlexDrillService.RunHeadProcess(SelectedHeadProcess);
         }
         catch (Exception e)
         {
            MessageHandler.ShowErrorMessage(FlexDrillMessages.RunHeadProcessFailed, new[] { e.Message });

            // Log
            FlexDrillService.Log.WriteMessage(TraceEventType.Error,
               $"An unexpected error while trying to run head process. {e.Message}.",
               e.StackTrace);
         }
      }

      private void ToggleVacuum()
      {
         try
         {
            // Toggle vacuum
            // Don't pass "!VacuumIsActive" in parameter, because the variable is toggled automatically by the checkBox control
            FlexDrillService.ToggleVacuum(VacuumIsActive);
         }
         catch (Exception e)
         {
            MessageHandler.ShowErrorMessage(FlexDrillMessages.ToggleVacuumFailed, new[] { e.Message });

            // Log
            FlexDrillService.Log.WriteMessage(TraceEventType.Error,
               $"An unexpected error occurred while toggling vacuum. {e.Message}.",
               e.StackTrace);
         }
         finally
         {
            SetVacuumState();
         }
      }

      private void GraspHead()
      {
         if (SelectedSlot == null)
         {
            return;
         }

         try
         {
            // Grasp head
            FlexDrillService.GraspHead(SelectedSlot.Index);
         }
         // TODO: add other exception types if there are new ones
         catch (Exception e)
         {
            MessageHandler.ShowErrorMessage(FlexDrillMessages.GraspHeadFailed, new[] { e.Message });

            // Log
            FlexDrillService.Log.WriteMessage(TraceEventType.Error,
               $"An unexpected error occurred while grasping head. {e.Message}.",
               e.StackTrace);
         }
      }

      private void DropHead()
      {
         if (SelectedSlot == null)
         {
            return;
         }
         try
         {
            // Drop head
            FlexDrillService.DropHead(SelectedSlot.Index);
         }
         catch (Exception e)
         {
            MessageHandler.ShowErrorMessage(FlexDrillMessages.DropHeadFailed, new[] { e.Message });

            // Log
            FlexDrillService.Log.WriteMessage(TraceEventType.Error,
               $"An unexpected error occurred while dropping head. {e.Message}.",
               e.StackTrace);
         }
      }

      private void StartHeadChange()
      {
         try
         {
            const string NoHead = "None";
            string LHeadToDrop;
            string LHeadToGrasp;

            if (ActiveHead == null)
            {
               LHeadToDrop = NoHead;
            }
            else
            {
               LHeadToDrop = ActiveHead.Name;
            }

            if (SelectedSlot?.Head == null)
            {
               LHeadToGrasp = NoHead;
            }
            else
            {
               LHeadToGrasp = SelectedSlot.Head.Name;
            }

            FlexDrillService.HeadChange(LHeadToDrop, LHeadToGrasp);
         }
         catch (Exception e)
         {
            // Log
            FlexDrillService.Log.WriteMessage(TraceEventType.Error,
               $"An unexpected error occurred while dropping head. {e.Message}.",
               e.StackTrace);
         }
      }

      private void RegisterEvents()
      {
         RobotInterpreter.ProgramStateChanged += RobotInterpreter_ProgramStateChanged; 
      }

      public void ReleaseEvents()
      {
         RefreshVarTimer.Stop();
         RobotInterpreter.ProgramStateChanged -= RobotInterpreter_ProgramStateChanged;
      }

      private void LoadFirstValues()
      {
         PrevHeadInitIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.HeadInitIsRunning, Robot);

         PrevHeadProcessIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.HeadProcessIsRunning, Robot);

         PrevGraspIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.GraspIsRunning, Robot);

         PrevDropIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.DropIsRunning, Robot);

         PrevInitCellIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CellInitIsRunning, Robot);

         PrevVacuumIsActive = KrlVarHandler.ReadBoolVariable(KrlVariableNames.VacuumState, Robot);
      }
      private void OnRefreshVar(object sender, EventArgs e)
      {
         if (!FOnTimer)
         {
            try
            {
               FOnTimer = true;

               InitCellIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CellInitIsRunning, Robot);
               DropIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.DropIsRunning, Robot);
               GraspIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.GraspIsRunning, Robot);
               HeadProcessIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.HeadProcessIsRunning, Robot);
               HeadInitIsRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.HeadInitIsRunning, Robot);
               VacuumIsActive = KrlVarHandler.ReadBoolVariable(KrlVariableNames.VacuumState, Robot);

               if (PrevInitCellIsRunning != InitCellIsRunning)
               {
                  PrevInitCellIsRunning = InitCellIsRunning;
               }

               if (PrevDropIsRunning != DropIsRunning)
               {
                  PrevDropIsRunning = DropIsRunning;
               }

               if (PrevGraspIsRunning != GraspIsRunning)
               {
                  PrevGraspIsRunning = GraspIsRunning;
               }

               if (PrevHeadProcessIsRunning != HeadProcessIsRunning)
               {
                  PrevHeadProcessIsRunning = HeadProcessIsRunning;
               }

               if (PrevHeadInitIsRunning != HeadProcessIsRunning)
               {
                  PrevHeadInitIsRunning = HeadInitIsRunning;
               }

               if (PrevVacuumIsActive != VacuumIsActive)
               {
                  PrevVacuumIsActive = VacuumIsActive;
                  SetVacuumState();
               }

               for (int i = 0; i < Slots.Count; i++)
               {
                  ActualizeIsEmptyState(i);
                  ActualizeSlotIndex(i);
               }

               for (int i = 0; i < Heads.Count; i++)
               {
                  ActualizeHeadDetails(i);
               }

               UpdateButtonState();

            }
            catch (Exception ex)
            {
               FOnTimer = false;
               FlexDrillService.Log.WriteMessage(TraceEventType.Error,
                  $"An unexpected error occurred in Head Management Timer. {ex.Message}.",
                  ex.StackTrace);
            }
            finally
            {
               FOnTimer = false;
            }
         }
      }
      
      private void UpdateButtonState()
      {
         GraspCommand.RaiseCanExecuteChanged();
         DropCommand.RaiseCanExecuteChanged();
         InitHeadCommand.RaiseCanExecuteChanged();
         RunProcessCommand.RaiseCanExecuteChanged();
      }


      private void RobotInterpreter_ProgramStateChanged(object sender, ProgramStateChangedEventArgs ea)
      {
         UpdateButtonState();
      }

      /// <summary>
      /// Actualize the head details
      /// </summary>
      /// <param name="index">The zero-based head index.</param>
      private void ActualizeHeadDetails(int index)
      {
         int oneBasedIndex = index + 1;
         FlexDrillHead updatedHead = HeadManager.LoadHead(oneBasedIndex, Robot);
         Heads[index].CopyDetailInfo(updatedHead);
      }

      /// <summary>
      /// Actualize the Slot Occupied state.
      /// </summary>
      /// <param name="slotIndex">Zero-based slot index.</param>
      private void ActualizeIsEmptyState(int slotIndex)
      {
         string varName = KrlVariableNames.HeadPresentInSlot[slotIndex];
         Slots[slotIndex].Occupied = KrlVarHandler.ReadBoolVariable(varName, Robot);
      }

      private void SetVacuumState()
      {
         VacuumIsActive = KrlVarHandler.ReadBoolVariable(KrlVariableNames.VacuumState, Robot);
      }

      private void ActualizeSlotIndex(int zeroBasedHeadIndex)
      {
         int oneBasedHeadIndex = Heads[zeroBasedHeadIndex].KrlHeadIndex;

         // actualize the head index
         Heads[zeroBasedHeadIndex].SlotIndex = HeadManager.GetSlotIndex(oneBasedHeadIndex, Robot);

         // Re-load the slots to actualize the head data
         LoadSlots();

         SetActiveHead();
      }

      private void LoadSlots()
      {
         Slots = HeadManager.GetSlots(Heads, Robot);
      }

      #endregion
   }
}