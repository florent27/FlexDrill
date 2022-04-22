// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateToFullTextConverter.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.OLPParser;
using KukaRoboter.Common.ResourceAccess;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.Converters
{
    public class StateToFullTextConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        // Converts operation or robot point states into a user friendly text
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Values[] should contain the state value and the string resources
            const int valueCount = 2;

            if (values == null || values.Length < valueCount || values[0] == null ||
                !(values[1] is IIndexedResourceAccessor))
            {
                return string.Empty;
            }

            IIndexedResourceAccessor textResources = (IIndexedResourceAccessor)values[1];

            // Convert the operation work mode to a full text
            if (values[0] is OperationWorkStatus)
            {
                return ConvertOperationMode((OperationWorkStatus)values[0], textResources);
            }

            // Convert the robot point mode to a full text
            if (values[0] is RobotPointWorkStatus)
            {
                return ConvertPointModes((RobotPointWorkStatus)values[0], textResources);
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Return a default value because the function is not needed
            return new object[] { };
        }

        #endregion IMultiValueConverter Members

        #region Methods

        private string ConvertPointModes(RobotPointWorkStatus robotPointMode, IIndexedResourceAccessor resources)
        {
            switch (robotPointMode)
            {
                case RobotPointWorkStatus.AssyDone:
                    return resources.Strings["PointState_AssyDone"];

                case RobotPointWorkStatus.AssyInError:
                    return resources.Strings["PointState_AssyError"];

                case RobotPointWorkStatus.AssyStarted:
                    return resources.Strings["PointState_AssyStarted"];

                case RobotPointWorkStatus.Done:
                    return resources.Strings["PointState_Done"];

                case RobotPointWorkStatus.InError:
                    return resources.Strings["PointState_Error"];

                case RobotPointWorkStatus.InProgress:
                    return resources.Strings["PointState_InProgress"];

                case RobotPointWorkStatus.PositionReached:
                    return resources.Strings["PointState_PositionReached"];

                case RobotPointWorkStatus.Idle:
                    return resources.Strings["PointState_Idle"];

                default:
                    return resources.Strings["PointState_Idle"];
            }
        }

        private string ConvertOperationMode(OperationWorkStatus operationMode, IIndexedResourceAccessor resources)
        {
            switch (operationMode)
            {
                case OperationWorkStatus.Done:
                    return resources.Strings["OperationState_Done"];

                case OperationWorkStatus.DoneWithError:
                    return resources.Strings["OperationState_DoneWithError"];

                case OperationWorkStatus.InProgress:
                    return resources.Strings["OperationState_InProgress"];

                case OperationWorkStatus.Idle:
                    return resources.Strings["OperationState_Idle"];

                default:
                    return resources.Strings["OperationState_Idle"];
            }
        }

        #endregion Methods
    }
}