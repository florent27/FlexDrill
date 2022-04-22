// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileHandler.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.SmartHMI.Production.Production.Config;
using System.Collections.ObjectModel;
using System.IO;

namespace Kuka.FlexDrill.SmartHMI.Production.Helper
{
    public static class FileHandler
    {
        #region Interface

        public static ObservableCollection<CellProgram> GetPrograms()
        {
            string programFolder = ProgramFolderPath;
            ObservableCollection<CellProgram> programs = new ObservableCollection<CellProgram>();
            bool NoProgramPresent =
               (Directory.GetFiles(programFolder) == null || Directory.GetFiles(programFolder).Length == 0);

            if (!NoProgramPresent)
            {
                foreach (string filePath in Directory.GetFiles(programFolder))
                {
                    // Only add *.xml files
                    if (Path.GetExtension(filePath) == ".xml")
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        programs.Add(new CellProgram(fileName, filePath));
                    }
                }
            }

            return programs;
        }

        public static string ProgramFolderPath
        {
            get
            {
                FlexDrillConfig config = XmlHandler.Deserialize<FlexDrillConfig>(Constants.ConfigFilePath);
                return config.ProgramFolderPath;
            }
        }

        #endregion Interface
    }
}