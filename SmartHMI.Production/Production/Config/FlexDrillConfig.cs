// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillConfig.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.Config
{
   [XmlRoot("Settings")]
   public class FlexDrillConfig
   {
      #region Interface

      [XmlAttribute("ProgramFolderPath")]
      public string ProgramFolderPath { get; set; }

      #endregion
   }
}