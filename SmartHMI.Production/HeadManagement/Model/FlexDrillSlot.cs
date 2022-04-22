// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillSlot.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Base;

namespace Kuka.FlexDrill.SmartHMI.Production.HeadManagement.Model
{
   public class FlexDrillSlot : BindablObject
   {
      #region Constants and Fields

      private FlexDrillHead head;

      private bool occupied;

      #endregion

      #region Constructors and Destructor

      /// <summary>
      /// Initialize an instance of a FlexDrillSlot object
      /// </summary>
      /// <param name="index">The 1-based slot index.</param>
      public FlexDrillSlot(int index)
      {
         Index = index;
      }

      /// <summary>
      /// Initialize an instance of a FlexDrillSlot object
      /// </summary>
      /// <param name="index">The 1-based slot index.</param>
      /// <param name="head">The head stored in this slot.</param>
      /// <param name="occupied">True if the slot contains a head.</param>
      public FlexDrillSlot(int index, FlexDrillHead head, bool occupied)
      {
         Index = index;
         Head = head;
         Occupied = occupied;
      }

      #endregion

      #region Interface

      public FlexDrillHead Head
      {
         get
         {
            return head;
         }
         set
         {
            SetField(ref head, value);
         }
      }


      /// <summary>
      /// Gets the 1-based slot index
      /// </summary>
      public int Index { get; }

      public bool Occupied
      {
         get
         {
            return occupied;
         }
         set
         {
            SetField(ref occupied, value);
         }
      }

      #endregion
   }
}