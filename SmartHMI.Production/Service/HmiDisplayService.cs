// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlexDrillService.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Ade.Components;

using Kuka.FlexDrill.SmartHMI.Production.Exceptions;
using Kuka.FlexDrill.SmartHMI.Production.Helper;
using Kuka.FlexDrill.SmartHMI.Production.Log;

using KukaRoboter.Common.ApplicationServices.ViewManager;

using KUKARoboter.KRCModel.Robot;

namespace Kuka.FlexDrill.SmartHMI.Production.Service
{
   public class HmiDisplayService : AdeComponent, IHmiDisplayService
   {
      #region Constants and Fields

      private IViewManager viewManager;

      private IRobot Robot;
      #endregion

      #region Constructors and Destructor

      public HmiDisplayService()
      {
         try
         {
            InitFlexDrillService();
         }
         catch (KrlVarNotDefinedException ex)
         {
            Log.WriteException(ex);
         }
      }
      public IViewManager ViewManager
      {
         get
         {
            if (viewManager == null)
            {
               viewManager = (GetService(typeof(IViewManager)) as IViewManager);
            }
            return viewManager;
         }
      }

      #endregion

      #region IHmiDisplayService Members

      public Logger Log { get; }

      public void DoDisplayView(string viewToDisplay, bool displayView)
      {
         if ((ViewManager != null) && (viewToDisplay != ""))
         {
            if (ViewManager.IsViewKnown(viewToDisplay))
            {
               if (displayView)
               {
                  if (!ViewManager.IsViewOpen(viewToDisplay))
                  {
                     ViewManager.OpenView(viewToDisplay);
                  }
               }
               else
               {
                  if (ViewManager.IsViewOpen(viewToDisplay))
                  {
                     ViewManager.CloseView(viewToDisplay);
                  }
               }
            }
         }
      }

      #endregion



      #region Methods

      private void InitFlexDrillService()
      {
         Robot = RobotImpl.Instance;
         try
         {
            Robot.KRLVariables[KrlVariableNames.DisplayView].Changed += DisplayViewChanged;
         }
         catch (KrlVarNotDefinedException)
         {
            throw new KrlVarNotDefinedException(KrlVariableNames.DisplayView);
         }
      }

      private void DisplayViewChanged(object sender, EventArgs e)
      {
         try
         {
            bool DisplayView = KrlVarHandler.ReadBoolVariable(KrlVariableNames.DisplayView, Robot);
            string ViewToDisplay = KrlVarHandler.ReadCharVariable(KrlVariableNames.ViewToDisplay, Robot);
            DoDisplayView(ViewToDisplay, DisplayView);
         }
         catch (KrlVarNotDefinedException ex)
         {
            throw new KrlVarNotDefinedException(ex.VariableName);
         }

      }

      #endregion
   }
}
 