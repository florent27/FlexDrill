// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidXmlSyntaxException.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kuka.FlexDrill.SmartHMI.Production.Exceptions
{
   /// <summary>This exception will be thrown when an error in the XML syntax has been detected</summary>
   public class InvalidXmlSyntaxException : Exception
   {
      #region Constructors and Destructor

      /// <summary>Initializes a new instance of the <see cref="InvalidXmlSyntaxException" /> class.</summary>
      /// <param name="message">The message.</param>
      public InvalidXmlSyntaxException(string message)
         : base(message)
      {
      }

      #endregion
   }
}