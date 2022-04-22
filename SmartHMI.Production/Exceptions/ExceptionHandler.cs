// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandler.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kuka.FlexDrill.SmartHMI.Production.Exceptions
{
    public static class ExceptionHandler
    {
        #region Interface

        public static string ExtractExceptionArgument(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return string.Empty;
            }

            int pathBeginIndex = message.IndexOf("'", 0, StringComparison.CurrentCulture);
            int pathEndIndex = message.IndexOf("'", pathBeginIndex + 1, StringComparison.CurrentCulture);

            const int numberOfQuotationMarks = 2;
            int argumentLength = pathEndIndex - pathBeginIndex - numberOfQuotationMarks;

            return message.Substring(pathBeginIndex + 1, argumentLength);
        }

        #endregion Interface
    }
}