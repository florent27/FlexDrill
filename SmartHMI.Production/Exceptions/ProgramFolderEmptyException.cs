// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramFolderEmptyException.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kuka.FlexDrill.SmartHMI.Production.Exceptions
{
    public class ProgramFolderEmptyException : Exception
    {
        #region Constructors and Destructor

        public ProgramFolderEmptyException(string folderPath) :
           base($"No FlexDrill XML program file found in: \'{folderPath}\'.")
        {
            FolderPath = folderPath;
        }

        #endregion Constructors and Destructor

        #region Interface

        public string FolderPath { get; }

        #endregion Interface
    }
}