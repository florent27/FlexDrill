// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetiTecInfoViewModel.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Base;
using Kuka.FlexDrill.SmartHMI.Production.Exceptions;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using Kuka.FlexDrill.SmartHMI.Production.Messages;
using Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.Model;
using System;
using System.Windows.Threading;

namespace Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.ViewModel
{
    public class SetiTecInfoViewModel : FlexDrillViewModelBase
    {
        #region Constants and Fields

        private bool initializationDone;

        private bool serviceIndicator;

        private bool lubTankEmpty;

        private bool noToolConnected;

        private bool toolConnected;

        private bool toolLocked;

        private bool toolRunning;

        private bool toolReady;

        private bool cycleAbort;

        private bool cycleStop;

        private bool cycleCompleted;

        private bool machineNotOk;

        private bool machineOk;

        private bool noMotorConnected;

        private bool motorConnected;

        private bool psetLoaded;

        private LastCycleData setiTeclastCycleData;

        private DispatcherTimer TimerSetiTec;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public SetiTecInfoViewModel()
           : base("FlexDrill_SetiTecInfo")
        {
        }

        #endregion Constructors and Destructor

        #region Interface

        public void InitializePlugin()
        {
            try
            {
                // Last Cycle Data
                DoUpdateLastCycleInfo();

                //
                //! Create Timer
                //
                if (TimerSetiTec == null)
                {
                    TimerSetiTec = new DispatcherTimer(DispatcherPriority.DataBind)
                    {
                        Interval = TimeSpan.FromMilliseconds(200)
                    };
                    TimerSetiTec.Tick += OnTimer;
                }
                TimerSetiTec.Start();
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

        public bool InitializationDone
        {
            get
            {
                return initializationDone;
            }
            set
            {
                SetField(ref initializationDone, value);
            }
        }

        public bool PsetLoaded
        {
            get
            {
                return psetLoaded;
            }
            set
            {
                SetField(ref psetLoaded, value);
            }
        }

        public bool MotorConnected
        {
            get
            {
                return motorConnected;
            }
            set
            {
                SetField(ref motorConnected, value);
            }
        }

        public bool NoMotorConnected
        {
            get
            {
                return noMotorConnected;
            }
            set
            {
                SetField(ref noMotorConnected, value);
            }
        }

        public bool MachineOk
        {
            get
            {
                return machineOk;
            }
            set
            {
                SetField(ref machineOk, value);
            }
        }

        public bool MachineNotOk
        {
            get
            {
                return machineNotOk;
            }
            set
            {
                SetField(ref machineNotOk, value);
            }
        }

        public bool CycleCompleted
        {
            get
            {
                return cycleCompleted;
            }
            set
            {
                SetField(ref cycleCompleted, value);
            }
        }

        public bool CycleStop
        {
            get
            {
                return cycleStop;
            }
            set
            {
                SetField(ref cycleStop, value);
            }
        }

        public bool CycleAbort
        {
            get
            {
                return cycleAbort;
            }
            set
            {
                SetField(ref cycleAbort, value);
            }
        }

        public bool ToolReady
        {
            get
            {
                return toolReady;
            }
            set
            {
                SetField(ref toolReady, value);
            }
        }

        public bool ToolRunning
        {
            get
            {
                return toolRunning;
            }
            set
            {
                SetField(ref toolRunning, value);
            }
        }

        public bool ToolLocked
        {
            get
            {
                return toolLocked;
            }
            set
            {
                SetField(ref toolLocked, value);
            }
        }

        public bool ToolConnected
        {
            get
            {
                return toolConnected;
            }
            set
            {
                SetField(ref toolConnected, value);
            }
        }

        public bool NoToolConnected
        {
            get
            {
                return noToolConnected;
            }
            set
            {
                SetField(ref noToolConnected, value);
            }
        }

        public bool LubTankEmpty
        {
            get
            {
                return lubTankEmpty;
            }
            set
            {
                SetField(ref lubTankEmpty, value);
            }
        }

        public bool ServiceIndicator
        {
            get
            {
                return serviceIndicator;
            }
            set
            {
                SetField(ref serviceIndicator, value);
            }
        }

        public LastCycleData SetiTeclastCycleData
        {
            get
            {
                return setiTeclastCycleData;
            }
            set
            {
                SetField(ref setiTeclastCycleData, value);
            }
        }

        public void DoStopTimer()
        {
            TimerSetiTec.Stop();
        }

        #endregion Interface

        #region Methods

        private void OnTimer(object sender, EventArgs e)
        {
            // Initialization
            InitializationDone = KrlVarHandler.ReadBoolVariable(KrlVariableNames.InitDone, Robot);
            PsetLoaded = KrlVarHandler.ReadBoolVariable(KrlVariableNames.PSetLoaded, Robot);

            // Motor
            MotorConnected = KrlVarHandler.ReadBoolVariable(KrlVariableNames.MotorConnected, Robot);
            NoMotorConnected = KrlVarHandler.ReadBoolVariable(KrlVariableNames.NoMotorConnected, Robot);

            // Machining
            MachineOk = KrlVarHandler.ReadBoolVariable(KrlVariableNames.MachineOk, Robot);
            MachineNotOk = KrlVarHandler.ReadBoolVariable(KrlVariableNames.MachineNotOk, Robot);

            // Cycle
            CycleCompleted = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CycleCompleted, Robot);
            CycleStop = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CycleStop, Robot);
            CycleAbort = KrlVarHandler.ReadBoolVariable(KrlVariableNames.CycleAbort, Robot);

            // Tool
            ToolReady = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ToolReady, Robot);
            ToolRunning = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ToolRunning, Robot);
            ToolLocked = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ToolLocked, Robot);
            ToolConnected = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ToolConnected, Robot);
            NoToolConnected = KrlVarHandler.ReadBoolVariable(KrlVariableNames.NoToolConnected, Robot);

            // Service
            LubTankEmpty = KrlVarHandler.ReadBoolVariable(KrlVariableNames.LubTankEmpty, Robot);
            ServiceIndicator = KrlVarHandler.ReadBoolVariable(KrlVariableNames.ServiceIndicator, Robot);

            //! Cycle Data
            if (KrlVarHandler.ReadBoolVariable(KrlVariableNames.CanUpdateLastCycleInfo, Robot))
            {
                DoUpdateLastCycleInfo();
                //! Reset Variable
                KrlVarHandler.WriteVariable(KrlVariableNames.CanUpdateLastCycleInfo, false, Robot);
            }
        }

        private void DoUpdateLastCycleInfo()
        {
            //! Create LastCycle Data
            if (SetiTeclastCycleData == null)
            {
                SetiTeclastCycleData = new LastCycleData();
            }
            if (SetiTeclastCycleData != null)
            {
                SetiTeclastCycleData.BoxName = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[1,]", Robot);
                SetiTeclastCycleData.BoxSn = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[2,]", Robot);

                SetiTeclastCycleData.MotorName = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[3,]", Robot);
                SetiTeclastCycleData.MotorSn = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[4,]", Robot);

                SetiTeclastCycleData.HeadUid = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[5,]", Robot);
                SetiTeclastCycleData.HeadType = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[6,]", Robot);
                SetiTeclastCycleData.HeadName = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[7,]", Robot);

                SetiTeclastCycleData.GlobalCounter = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[8,]", Robot);
                SetiTeclastCycleData.LocalCounter = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[9,]", Robot);

                SetiTeclastCycleData.CycleResult = KrlVarHandler.ReadCharVariable("LastCycleDataInfo[10,]", Robot);

                for (int i = 0; i < 10; i++)
                {
                    string varNameBase = $"LastCycleDataStep{i + 1}";
                    string varName = varNameBase + "[1,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].StopCode = KrlVarHandler.ReadCharVariable(varName, Robot);
                    varName = varNameBase + "[2,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].Duration = KrlVarHandler.ReadCharVariable(varName, Robot);
                    varName = varNameBase + "[3,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].DistanceM1 = KrlVarHandler.ReadCharVariable(varName, Robot);
                    varName = varNameBase + "[4,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].DistanceM2 = KrlVarHandler.ReadCharVariable(varName, Robot);
                    varName = varNameBase + "[5,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].M1MaxAmperage = KrlVarHandler.ReadCharVariable(varName, Robot);
                    varName = varNameBase + "[6,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].M2MaxAmperage = KrlVarHandler.ReadCharVariable(varName, Robot);
                    varName = varNameBase + "[7,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].M1NoLoadAmperage = KrlVarHandler.ReadCharVariable(varName, Robot);
                    varName = varNameBase + "[8,]";
                    SetiTeclastCycleData.LastCycleDataSteps[i].M2NoLoadAmperage = KrlVarHandler.ReadCharVariable(varName, Robot);
                }
            }
        }

        #endregion Methods
    }
}