// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateToFullPicResourceConverter .cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

using Kuka.FlexDrill.Process.OLPParser;
using Kuka.FlexDrill.SmartHMI.Production.Helper;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.Converters
{
   public class StateToFullPicResourceConverter : IValueConverter
   {
      #region IValueConverter Members

      // Converts operation or robot point states into a user friendly text
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value == null)
         {
            return string.Empty;
         }


         // Convert the operation work mode to a full text
         if (value is OperationWorkStatus)
         {
            return ConvertOperationMode((OperationWorkStatus)value);
         }

         // Convert the robot point mode to a full text
         if (value is RobotPointWorkStatus)
         {
            return ConvertPointModes((RobotPointWorkStatus)value);
         }

         return string.Empty;
      }

      /// <inheritdoc />
      public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
      {
         // Return a default value because the function is not needed
         return new object();
      }

      #endregion

      #region Methods

      private Uri ConvertPointModes(RobotPointWorkStatus robotPointMode)
      {
         switch (robotPointMode)
         {
         case RobotPointWorkStatus.AssyDone:
            return IconPaths.AssemblyDone;

         case RobotPointWorkStatus.AssyInError:
            return IconPaths.AssemblyError;

         case RobotPointWorkStatus.AssyStarted:
            return IconPaths.AssemblyInProgress;

         case RobotPointWorkStatus.Done:
            return IconPaths.AnalysisDone;

         case RobotPointWorkStatus.InError:
            return IconPaths.AnalysisError;

         case RobotPointWorkStatus.InProgress:
            return IconPaths.AnalysisInProgress;

         case RobotPointWorkStatus.PositionReached:
            return IconPaths.AnalysisPosition;

         case RobotPointWorkStatus.Idle:
            return IconPaths.OperationIdle;

         default:
            return IconPaths.OperationIdle;
         }
      }

      private Uri ConvertOperationMode(OperationWorkStatus operationMode)
      {
         switch (operationMode)
         {
         case OperationWorkStatus.Done:
            return IconPaths.OperationDone;

         case OperationWorkStatus.DoneWithError:
            return IconPaths.OperationError;

         case OperationWorkStatus.InProgress:
            return IconPaths.OperationInProgress;

         case OperationWorkStatus.Idle:
            return IconPaths.OperationIdle;

         default:
            return IconPaths.OperationIdle;
         }
      }

      #endregion
   }
}