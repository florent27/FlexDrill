// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrectorData.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kuka.FlexDrill.Process.OLPParser
{
   public class CorrectorData
   {
      #region Interface

      public string Name { get; set; }
      public string Type { get; set; }

      public ObservableCollection<Target> Targets { get; set; }

      #endregion
   }
}