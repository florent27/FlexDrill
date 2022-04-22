// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHmiDisplayService.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Log;

namespace Kuka.FlexDrill.SmartHMI.Production.Service
{
   public interface IHmiDisplayService
   {
      #region Interface

      void DoDisplayView(string viewToDisplay, bool displayView);

      Logger Log { get; }

      #endregion
   }
}