// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KrlVarNotDefinedException.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kuka.FlexDrill.SmartHMI.Production.Exceptions
{
   public class KrlVarNotDefinedException : Exception
   {
      #region Constructors and Destructor

      public KrlVarNotDefinedException(string krlVariableName)
         : base($"The KRL variable \'{krlVariableName}\' is not defined.")
      {
         VariableName = krlVariableName;
      }

      #endregion

      #region Interface

      public string VariableName { get; }

      #endregion
   }
}