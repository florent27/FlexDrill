// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlHandler.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Kuka.FlexDrill.SmartHMI.Production.Exceptions;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.Config
{
   public static class XmlHandler
   {
      #region Interface

      public static T Deserialize<T>(string configPath) where T : class
      {
         if (!File.Exists(configPath))
         {
            throw new FileNotFoundException("File not found: \"" + Path.GetFullPath(configPath) + "\".",
               Path.GetFullPath(configPath));
         }

         T config;

         try
         {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));

            using (var myFileStream = new FileStream(configPath, FileMode.Open))
            {
               config = (T)deserializer.Deserialize(myFileStream);
            }
         }
         catch (InvalidOperationException e)
         {
            if (e.InnerException is XmlException)
            {
               throw new InvalidXmlSyntaxException("Error in file: \"" + Path.GetFullPath(configPath) + "\". " +
                                                   e.InnerException.Message);
            }

            throw new InvalidXmlSyntaxException("Error in file: \"" + Path.GetFullPath(configPath) + "\". " +
                                                e.Message);
         }

         return config;
      }

      #endregion
   }
}