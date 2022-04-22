// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KRLGeneratorUtils.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.Process.OLPParser;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Kuka.FlexDrill.Process.KRLGenerator
{
    internal static class KrlGeneratorUtils
    {
        #region Interface

        public static CellProgram GetOlpData(string AFileName)
        {
            //
            //! Declare an object variable of the type to be deserialized
            //
            CellProgram FCellProgram;
            //
            //! Create an instance of the XmlSerializer specifying type and namespace
            //
            XmlSerializer serializer = new XmlSerializer(typeof(CellProgram));
            //
            //! A FileStream is needed to read the XML document
            //
            FileStream fs = new FileStream(AFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            //
            //! Use the Deserialize method to restore the object's state
            //
            FCellProgram = (CellProgram)serializer.Deserialize(reader);
            //
            //! Free Up File Stream
            //
            fs.Close();
            return FCellProgram;
        }

        public static ObservableCollection<CorrectorData> GetAllCorrectorFromOperation(Operation AOperation)
        {
            ObservableCollection<CorrectorData> CorrectorList = new ObservableCollection<CorrectorData>();
            if (AOperation.Correctors != null)
            {
                foreach (Corrector ACorrector in AOperation.Correctors.LCorrector)
                {
                    CorrectorData FCorrectorData = new CorrectorData
                    {
                        Name = ACorrector.Name,
                        Type = ACorrector.Type
                    };
                    if (ACorrector.LTarget.Count > 0)
                    {
                        FCorrectorData.Targets = ACorrector.LTarget;
                    }
                    CorrectorList.Add(FCorrectorData);
                }
            }
            return CorrectorList;
        }

        public static bool AreOperationsNameUnique(WorkSequence AWorkSequence)
        {
            bool IsUnique = true;
            bool PresentInList;
            List<string> OperationsName = new List<string>();
            foreach (Operation AOperation in AWorkSequence.LOperation)
            {
                PresentInList = OperationsName.Contains(AOperation.Name);
                if (!PresentInList)
                {
                    OperationsName.Add(AOperation.Name);
                }
                else
                {
                    Logger.Logger.WriteLog("Operation Name <" + AOperation.Name + "> is present several times", false);
                }
                IsUnique = IsUnique && (!PresentInList);
            }
            return IsUnique;
        }

        public static bool ArePointNameUnique(WorkSequence AWorkSequence)
        {
            bool IsUnique = true;
            bool PresentInList;
            List<string> PointsName = new List<string>();
            foreach (Operation AOperation in AWorkSequence.LOperation)
            {
                foreach (RobotPoint ARobotPoint in AOperation.RobotPoints.LRobotPoint)
                {
                    PresentInList = PointsName.Contains(ARobotPoint.Name);
                    if (!PresentInList)
                    {
                        PointsName.Add(ARobotPoint.Name);
                    }
                    else
                    {
                        Logger.Logger.WriteLog("Robot Point Name <" + ARobotPoint.Name + "> is present several times", false);
                    }
                    IsUnique = IsUnique && (!PresentInList);
                }
            }
            return IsUnique;
        }

        #endregion Interface
    }
}