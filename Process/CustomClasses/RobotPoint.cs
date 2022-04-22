// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RobotPoint.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.OLPParser;

namespace Kuka.FlexDrill.Process.CustomClasses
{
    public class RobotPoint : RobotPointXSD
    {
        #region Constants and Fields

        private RobotPointWorkMode workMode = RobotPointWorkMode.Run;

        private RobotPointWorkStatus workStatus = RobotPointWorkStatus.Idle;

        private float zEscapeValue = 50;

        #endregion Constants and Fields

        #region Interface

        /// <remarks />
        public RobotPointWorkMode WorkMode
        {
            get
            {
                return workMode;
            }
            set
            {
                workMode = value;
                RaisePropertyChanged("WorkMode");
            }
        }

        /// <remarks />
        public RobotPointWorkStatus WorkStatus
        {
            get
            {
                return workStatus;
            }
            set
            {
                workStatus = value;
                RaisePropertyChanged("WorkStatus");
            }
        }

        public float ZEscapeValue
        {
            get
            {
                return zEscapeValue;
            }
            set
            {
                zEscapeValue = value;
                RaisePropertyChanged("ZEscapeValue");
            }
        }

        #endregion Interface

        #region Methods

        internal void ResetStatus()
        {
            WorkStatus = RobotPointWorkStatus.Idle;
        }

        #endregion Methods
    }
}