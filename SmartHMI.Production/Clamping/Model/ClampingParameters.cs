// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClampingParameters.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Base;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using KUKARoboter.KRCModel.Robot;
using System;

namespace Kuka.FlexDrill.SmartHMI.Production.Clamping.Model
{
    public class ClampingParameters : BindablObject
    {
        #region Constants and Fields

        private double targetForce;

        private double speedFactor;

        private double gainFz;

        private double gainFx;

        private double gainFy;

        private double gainTx;

        private double gainTy;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public ClampingParameters(IRobot robot)
        {
            TargetForce = KrlVarHandler.ReadRealVariable(KrlVariableNames.ClampingTargetForce, robot);
            SpeedFactor = KrlVarHandler.ReadRealVariable(KrlVariableNames.ClampingSpeedFactor, robot);
            GainFz = KrlVarHandler.ReadRealVariable(KrlVariableNames.ClampingGain, robot);
            GainFx = KrlVarHandler.ReadRealVariable(KrlVariableNames.ClampingGainFx, robot);
            GainFy = KrlVarHandler.ReadRealVariable(KrlVariableNames.ClampingGainFy, robot);
            GainTx = KrlVarHandler.ReadRealVariable(KrlVariableNames.ClampingGainTx, robot);
            GainTy = KrlVarHandler.ReadRealVariable(KrlVariableNames.ClampingGainTy, robot);

            // Save Initial Value
            InitialTargetForce = TargetForce;
            InitialSpeedFactor = SpeedFactor;
            InitialGainFx = GainFx;
            InitialGainFy = GainFy;
            InitialGainFz = GainFz;
            InitialGainTx = GainTy;
            InitialGainTy = GainTy;
        }

        #endregion Constructors and Destructor

        #region Interface

        public bool ParametersHaveChanged()
        {
            const double tolerenceFactor = 0.001;

            bool targetForceChanged = Math.Abs(TargetForce - InitialTargetForce) > tolerenceFactor;
            bool speedFactorChanged = Math.Abs(SpeedFactor - InitialSpeedFactor) > tolerenceFactor;
            bool gainFzChanged = Math.Abs(GainFz - InitialGainFz) > tolerenceFactor;
            bool gainFxChanged = Math.Abs(GainFx - InitialGainFx) > tolerenceFactor;
            bool gainFyChanged = Math.Abs(GainFy - InitialGainFy) > tolerenceFactor;
            bool gainTxChanged = Math.Abs(GainTx - InitialGainTx) > tolerenceFactor;
            bool gainTyChanged = Math.Abs(GainTy - InitialGainTy) > tolerenceFactor;

            return targetForceChanged || speedFactorChanged || gainFzChanged || gainFxChanged || gainFyChanged || gainTxChanged || gainTyChanged;
        }

        public double TargetForce
        {
            get
            {
                return targetForce;
            }
            set
            {
                SetField(ref targetForce, value);
            }
        }

        public double SpeedFactor
        {
            get
            {
                return speedFactor;
            }
            set
            {
                SetField(ref speedFactor, value);
            }
        }

        public double GainFz
        {
            get
            {
                return gainFz;
            }
            set
            {
                SetField(ref gainFz, value);
            }
        }

        public double GainFx
        {
            get
            {
                return gainFx;
            }
            set
            {
                SetField(ref gainFx, value);
            }
        }

        public double GainFy
        {
            get
            {
                return gainFy;
            }
            set
            {
                SetField(ref gainFy, value);
            }
        }

        public double GainTx
        {
            get
            {
                return gainTx;
            }
            set
            {
                SetField(ref gainTx, value);
            }
        }

        public double GainTy
        {
            get
            {
                return gainTy;
            }
            set
            {
                SetField(ref gainTy, value);
            }
        }

        public void SaveChanges(IRobot robot)
        {
            // Update the initial values
            InitialTargetForce = TargetForce;
            InitialSpeedFactor = SpeedFactor;
            InitialGainFx = GainFx;
            InitialGainFy = GainFy;
            InitialGainFz = GainFz;
            InitialGainTx = GainTy;
            InitialGainTy = GainTy;

            // Write the values to KRL
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampingTargetForce, TargetForce, robot);
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampingSpeedFactor, SpeedFactor, robot);
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampingGain, GainFz, robot);
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampingGainFx, GainFx, robot);
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampingGainFy, GainFx, robot);
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampingGainTx, GainFx, robot);
            KrlVarHandler.WriteVariable(KrlVariableNames.ClampingGainTy, GainFx, robot);
        }

        public void CancelChanges()
        {
            TargetForce = InitialTargetForce;
            SpeedFactor = InitialSpeedFactor;
            GainFz = InitialGainFz;
            GainFx = InitialGainFx;
            GainFy = InitialGainFy;
            GainTx = InitialGainTx;
            GainTy = InitialGainTy;
        }

        #endregion Interface

        #region Properties

        private double InitialTargetForce { get; set; }

        private double InitialSpeedFactor { get; set; }

        private double InitialGainFz { get; set; }

        private double InitialGainFx { get; set; }

        private double InitialGainFy { get; set; }

        private double InitialGainTx { get; set; }

        private double InitialGainTy { get; set; }

        #endregion Properties
    }
}