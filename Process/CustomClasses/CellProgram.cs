// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CellProgram.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.KRLGenerator;
using Kuka.FlexDrill.Process.OLPParser;

namespace Kuka.FlexDrill.Process.CustomClasses
{
    public class CellProgram : CellProgramXSD
    {
        #region Constants and Fields

        private string name;

        private string xmlFilePath;

        private bool isLoaded;

        #endregion Constants and Fields

        #region Constructors and Destructor

        /// <summary>
        /// Constructor : Call Parent
        /// </summary>
        public CellProgram()
        {
        }

        public CellProgram(string name, string filePath)
        {
            Name = name;
            XmlFilePath = filePath;
        }

        #endregion Constructors and Destructor

        #region Interface

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string XmlFilePath
        {
            get
            {
                return xmlFilePath;
            }

            set
            {
                if (name == value)
                {
                    return;
                }

                xmlFilePath = value;
                RaisePropertyChanged("XmlFilePath");
            }
        }

        public void Load()
        {
            CellProgram program = DoReadDataFromXml();

            CellConfiguration = program.CellConfiguration;
            Informations = program.Informations;
            CustomerInfo = program.CustomerInfo;
            CellConfiguration = program.CellConfiguration;
            WorkSequence = program.WorkSequence;
            SchemaVersion = program.SchemaVersion;

            // Don't remove this tag, it's needed for the UI
            IsLoaded = true;
        }

        // This function is needed for the UI
        public void Unload()
        {
            // Release the resources
            if (WorkSequence?.LOperation != null)
            {
                foreach (Operation operation in WorkSequence.LOperation)
                {
                    operation.RobotPoints?.LRobotPoint?.Clear();
                }

                WorkSequence.LOperation?.Clear();
            }

            IsLoaded = false;
        }

        public void ResetStatus()
        {
            if (WorkSequence?.LOperation != null)
            {
                foreach (Operation operation in WorkSequence.LOperation)
                {
                    operation?.ResetStatus();
                }
            }
        }

        /// <remarks />
        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }
            set
            {
                isLoaded = value;
                RaisePropertyChanged("IsLoaded");
            }
        }

        #endregion Interface

        #region Methods

        public CellProgram DoReadDataFromXml()
        {
            CellProgram program = KrlGeneratorUtils.GetOlpData(xmlFilePath);
            return program;
        }

        #endregion Methods
    }
}