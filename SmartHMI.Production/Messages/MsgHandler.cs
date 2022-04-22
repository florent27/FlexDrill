// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MsgHandler.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using JetBrains.Annotations;
using KukaRoboter.Common.ApplicationServices.Dialogs;
using KukaRoboter.Common.ResourceAccess;
using KukaRoboter.SmartHMI.LegacySupport;

using KUKARoboter.KRCModel.Messages;

namespace Kuka.FlexDrill.SmartHMI.Production.Messages
{
   /// <summary>The messages handler.</summary>
   public class MsgHandler
   {
      #region Constants and Fields

      private const string ModuleName = "FlexDrill";

      private readonly IDialogService dialogService;

      private readonly ILegacyKRCInterface legacyKrcInterface;

      private readonly IIndexedResourceAccessor resources;

      #endregion

      #region Constructors and Destructor

      /// <summary>Initializes a new instance of the <see cref="MsgHandler" /> class.</summary>
      /// <param name="legacyKrcInterface">The legacy KRC interface.</param>
      /// <param name="dialogService">The dialog service.</param>
      /// <param name="resources">The resources.</param>
      public MsgHandler(ILegacyKRCInterface legacyKrcInterface, IDialogService dialogService,
         IIndexedResourceAccessor resources)
      {
         this.legacyKrcInterface = legacyKrcInterface;
         this.dialogService = dialogService;
         this.resources = resources;
      }

      #endregion

      #region Interface

      public MessageBoxButtons AskForSaving()
      {
         string caption = ModuleName;

         // Ask if changes should be saved before closing
         return dialogService.ModalMessageBox(
            resources,
            caption,
            MessageBoxSymbol.Question,
            MessageBoxButtons.Yes | MessageBoxButtons.No | MessageBoxButtons.Cancel,
            $"{ModuleName}#SaveChangesQuestion");
      }

      public void ShowErrorMessage(FlexDrillMessages message, string[] parameters)
      {
         int messageIndex = (int)message;
         string messageKey = Convert.ToString(message, CultureInfo.CurrentUICulture);

         MessageData messageData = KrcMessages.CreateMessage(
            MessageData.MessageTypes.Quitt,
            ModuleName,
            messageIndex,
            ModuleName,
            messageKey,
            parameters);

         KrcMessages.Add(messageData);
      }

      public void ShowInfoMessage(FlexDrillMessages message, string[] parameters)
      {
         int messageIndex = (int)message;
         string messageKey = Convert.ToString(message, CultureInfo.CurrentUICulture);

         MessageData messageData = KrcMessages.CreateMessage(
            MessageData.MessageTypes.Info,
            ModuleName,
            messageIndex,
            ModuleName,
            messageKey,
            parameters);
         KrcMessages.Add(messageData);
         messageData.Remove();
      }

      public void ConfirmAllMessage()
      {
         KrcMessages.Consumer.ConfirmAll();
      }

      public MessageBoxButtons ShowWarningMessage(string AMessage)
      {
         string caption = ModuleName;

         // Ask if changes should be saved before closing
         return dialogService.ModalMessageBox(
            resources,
            caption,
            MessageBoxSymbol.Warning,
            MessageBoxButtons.Yes | MessageBoxButtons.No,
            AMessage);
      }

      public MessageBoxButtons ShowWarningMessageAck(string AMessage)
      {
         string caption = ModuleName;

         // Ask if changes should be saved before closing
         return dialogService.ModalMessageBox(
            resources,
            caption,
            MessageBoxSymbol.Warning,
            MessageBoxButtons.Ok,
            AMessage);
      }
      #endregion

      #region Properties

      private IMessage KrcMessages => legacyKrcInterface.KRCModel.Messages;

      #endregion
   }
}