// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RobotException.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kuka.FlexDrill.Process.Exceptions
{
    public class RobotException : Exception
    {
        #region Constructors and Destructor

        /// <summary>Initializes a new instance of the <see cref="RobotException" /> class.</summary>
        /// <param name="message">The message.</param>
        public RobotException(string message)
           : base(message)
        {
        }

        #endregion Constructors and Destructor
    }
}