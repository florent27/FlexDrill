// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeadManager.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using Kuka.FlexDrill.SmartHMI.Production.Helper;

using KUKARoboter.KRCModel.Robot;

namespace Kuka.FlexDrill.SmartHMI.Production.HeadManagement.Model
{
   public static class HeadManager
   {
      #region Interface

      public static List<FlexDrillHead> LoadHeads(IRobot robot)
      {
         List<FlexDrillHead> heads = new List<FlexDrillHead>();

         // Initialize 3 head objects
         const int slotCout = 3;
         for (int i = 1; i <= slotCout; i++)
         {
            FlexDrillHead head = LoadHead(i, robot);
            heads.Add(head);
         }

         return heads;
      }

      /// <summary>
      /// Load the head data.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns></returns>
      public static FlexDrillHead LoadHead(int headIndex, IRobot robot)
      {
         FlexDrillHead head = new FlexDrillHead(headIndex);

         if (robot != null)
         {
            head.SlotIndex = GetSlotIndex(headIndex, robot);
            head.Name = GetName(headIndex, robot);
            head.Type = GetType(headIndex, robot);
            head.Id = GetUid(headIndex, robot);
            head.Oscillation = GetOscillation(headIndex, robot);
            head.Amplitude = GetAmplitude(headIndex, robot);
            head.GearBoxFeed = GetBoxFeed(headIndex, robot);
            head.GearBoxSpeed = GetBoxSpeed(headIndex, robot);
            head.CustomerInfo = GetCustomerInfo(headIndex, robot);
         }

         return head;
      }

      public static List<FlexDrillSlot> GetSlots(List<FlexDrillHead> heads, IRobot robot)
      {
         List<FlexDrillSlot> slots = new List<FlexDrillSlot>();

         if (heads != null)
         {
            // Important: the slot index is 1-based
            for (int i = 1; i <= heads.Count; i++)
            {
               FlexDrillHead head = heads.FirstOrDefault(h => h.SlotIndex == i);

               int zeroBasedSlotIndex = i - 1;

               // Requirement from KUKA Bordeaux: 
               //    --> The IsOccupied value must be read from the KRL variables HeadPresenceSlot1, HeadPresenceSlot2 and HeadPresenceSlot3.
               bool occupied = GetOccupiedState(zeroBasedSlotIndex, robot);

               FlexDrillSlot slot = new FlexDrillSlot(i, head, occupied);
               slots.Add(slot);
            }
         }

         return slots;
      }

      /// <summary>
      /// Gets the one-based slot index.
      /// </summary>
      /// <param name="headIndex">The one-based headIndex</param>
      /// <param name="robot">The robot model</param>
      /// <returns></returns>
      public static int GetSlotIndex(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlSlotIndex;
         return KrlVarHandler.ReadIntVariable(varName, robot);
      }

      #endregion

      #region Methods

      /// <summary>
      /// Returns the slot's occupied state.
      /// </summary>
      /// <param name="slotIndex">The zero-based slot index.</param>
      /// <param name="robot">The robot object.</param>
      /// <returns>True if the slot is empty (doesn't contain a head).</returns>
      private static bool GetOccupiedState(int slotIndex, IRobot robot)
      {
         string varName = KrlVariableNames.HeadPresentInSlot[slotIndex];
         return KrlVarHandler.ReadBoolVariable(varName, robot);
      }

      /// <summary>
      /// Get the head's name.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The head's name.</returns>
      private static string GetName(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlName;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      /// <summary>
      /// Get the head's type.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The head's type.</returns>
      private static string GetType(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlType;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      /// <summary>
      /// Get the customer information.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The customer information.</returns>
      private static string GetCustomerInfo(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlCustomerInfo;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      /// <summary>
      /// Get the gear box's speed.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The gear box's speed.</returns>
      private static string GetBoxSpeed(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlGearBoxSpeed;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      /// <summary>
      /// Get the gear box's feed.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The gear box's feed.</returns>
      private static string GetBoxFeed(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlGearBoxFeed;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      /// <summary>
      /// Get the amplitude.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The amplitude.</returns>
      private static string GetAmplitude(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlAmplitude;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      /// <summary>
      /// Get the oscillation.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The oscillation.</returns>
      private static string GetOscillation(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlOscillation;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      /// <summary>
      /// Get the unique identifier.
      /// </summary>
      /// <param name="headIndex">The one-based head index.</param>
      /// <param name="robot">The robot model.</param>
      /// <returns>The unique identifier.</returns>
      private static string GetUid(int headIndex, IRobot robot)
      {
         string varName = new FlexDrillHead(headIndex).KrlUid;
         return KrlVarHandler.ReadCharVariable(varName, robot);
      }

      #endregion
   }
}