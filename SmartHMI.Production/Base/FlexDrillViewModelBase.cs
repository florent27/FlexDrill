// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillViewModelBase.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Messages;
using Kuka.FlexDrill.SmartHMI.Production.Service;
using KukaRoboter.Common.ApplicationServices.Dialogs;
using KukaRoboter.Common.ViewModel;
using KukaRoboter.SmartHMI.LegacySupport;
using KUKARoboter.KRCModel.Robot;
using KUKARoboter.KRCModel.Robot.Interpreter;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Kuka.FlexDrill.SmartHMI.Production.Base
{
    public class FlexDrillViewModelBase : ViewModelBase
    {
        #region Constants and Fields

        private IDialogService dialogService;

        private ILegacyKRCInterface legacyKrcInterface;

        private IRobot robot;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public FlexDrillViewModelBase(string moduleName)
           : base(moduleName)
        {
        }

        #endregion Constructors and Destructor

        #region Interface

        public override void Initialize()
        {
            base.Initialize();

            ComponentFramework.StartupCompleted += OnStartupCompleted;
        }

        public MsgHandler MessageHandler { get; private set; }

        public IFlexDrillService FlexDrillService { get; private set; }

        public IHmiDisplayService HmiDisplayService { get; private set; }

        public IInterpreter RobotInterpreter => Robot.Interpreters[InterpreterTypes.Robot];

        public ProStates RobotProgramState => Robot.Interpreters[InterpreterTypes.Robot].ProgramState;

        public IRobot Robot
        {
            get
            {
                if (robot == null)
                {
                    robot = LegacyKrcInterface.Robot;
                }
                return robot;
            }
        }

        #endregion Interface

        #region Properties

        private IDialogService DialogService
        {
            get
            {
                if (dialogService == null)
                {
                    dialogService = GetService(typeof(IDialogService)) as IDialogService;
                }

                return dialogService;
            }
        }

        protected ILegacyKRCInterface LegacyKrcInterface
        {
            get
            {
                if (legacyKrcInterface == null)
                {
                    legacyKrcInterface = GetService(typeof(ILegacyKRCInterface)) as ILegacyKRCInterface;
                }

                return legacyKrcInterface;
            }
        }

        #endregion Properties

        #region Methods

        private void OnStartupCompleted(object sender, EventArgs e)
        {
            MessageHandler = new MsgHandler(LegacyKrcInterface, DialogService, Resources);
            FlexDrillService = GetService(typeof(IFlexDrillService)) as IFlexDrillService;
            FlexDrillService.Robot = Robot;
            FlexDrillService.Message = MessageHandler;
            HmiDisplayService = GetService(typeof(IHmiDisplayService)) as IHmiDisplayService;
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            FirePropertyChanged(propertyName);
            return true;
        }

        protected void HandleError()
        {
            bool errorNotNull = !FlexDrillService.Error.Equals(default(KeyValuePair<FlexDrillMessages, string[]>));

            if (errorNotNull)
            {
                // Retry initialization. This step is needed to check if the user made modifications in between.
                FlexDrillService.RetryInitialization();

                // Re-check error state
                errorNotNull = !FlexDrillService.Error.Equals(default(KeyValuePair<FlexDrillMessages, string[]>));

                if (errorNotNull)
                {
                    // Show an user message
                    FlexDrillMessages messageKey = FlexDrillService.Error.Key;
                    string[] messageParameters = FlexDrillService.Error.Value;

                    MessageHandler.ShowErrorMessage(messageKey, messageParameters);
                }
            }
        }

        #endregion Methods
    }
}