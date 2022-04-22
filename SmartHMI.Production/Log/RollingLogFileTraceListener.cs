// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RollingLogFileTraceListener.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ade.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Kuka.FlexDrill.SmartHMI.Production.Log
{
    /// <summary>The rolling log file trace listener.</summary>
    public sealed class RollingLogFileTraceListener : TextWriterTraceListener
    {
        #region Constants and Fields

        private const string DateTimeFormatString = "dd/MM/yyyy hh:mm:ss.fff tt";

        private const string FullTraceFormatString = "{0} [{1}] {2}: {3}";

        private const string IsEnabledPropertyName = "isEnabled";

        private const string LiteTraceFormatString = "{0} [{1}] {2}";

        private const string TransferTraceFormatString = "{0} [{1}] ({2}): {3}";

        // The maximum log file size in kB
        private const int MaxLogSize = 500;

        private const string SeparatorString =
           "\n--------------------------------------------------------------------------------\n";

        private string logFilePath;

        #endregion Constants and Fields

        #region Constructors and Destructor

        /// <summary>Initializes a new instance of the <see cref="RollingLogFileTraceListener" /> class.</summary>
        /// <param name="path">The path.</param>
        public RollingLogFileTraceListener(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Argument 'path' must not be null!");
            }

            Name = "KukaDeutschland.RollingLogFileTraceListener";

            Writer = GetWriter(path);

            WriteLine(SeparatorString);
        }

        #endregion Constructors and Destructor

        #region Interface

        #region public methods

        /// <summary>Gets or sets a value indicating whether this instance is enabled.</summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get
            {
                EnsureStateKnown();
                return state == EnabledState.Enabled;
            }

            set
            {
                state = value ? EnabledState.Enabled : EnabledState.Disabled;
            }
        }

        #endregion public methods

        /// <summary>The write line.</summary>
        /// <param name="message">The message.</param>
        public override void WriteLine(string message)
        {
            HandleMaxFileSize();

            base.WriteLine(message);
        }

        #endregion Interface

        #region private fields

        private enum EnabledState
        {
            /// <summary>The unknown.</summary>
            Unknown = 0,

            /// <summary>The disabled.</summary>
            Disabled,

            /// <summary>The enabled.</summary>
            Enabled
        }

        private EnabledState state = EnabledState.Unknown;

        #endregion private fields

        #region public overrides

        /// <summary>The trace event.</summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="source">The source.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="id">The id.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public override void TraceEvent(
           TraceEventCache eventCache,
           string source,
           TraceEventType eventType,
           int id,
           string format,
           params object[] args)
        {
            if (IsEnabled)
            {
                if (args.Length == 0)
                {
                    TraceEvent(eventCache, source, eventType, id, format);
                }
                else
                {
                    WriteLine(string.Format(FullTraceFormatString, DateTimeToString(eventCache.DateTime), eventType, source,
                       string.Format(format, args)));
                }
            }
        }

        /// <summary>The trace event.</summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="source">The source.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="id">The id.</param>
        /// <param name="message">The message.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
           string message)
        {
            if (IsEnabled)
            {
                WriteLine(string.Format(FullTraceFormatString, DateTimeToString(eventCache.DateTime), eventType, source,
                   message));
            }
        }

        /// <summary>The trace event.</summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="source">The source.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="id">The id.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            if (IsEnabled)
            {
                WriteLine(string.Format(FullTraceFormatString, DateTimeToString(eventCache.DateTime), eventType, source,
                   id));
            }
        }

        /// <summary>The trace data.</summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="source">The source.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="id">The id.</param>
        /// <param name="data">The data.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
           object data)
        {
            if (IsEnabled)
            {
                WriteLine(string.Format(LiteTraceFormatString, DateTimeToString(eventCache.DateTime), eventType, source));
                WriteLine(data);
            }
        }

        /// <summary>The trace data.</summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="source">The source.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="id">The id.</param>
        /// <param name="data">The data.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
           params object[] data)
        {
            if (IsEnabled)
            {
                WriteLine(string.Format(LiteTraceFormatString, DateTimeToString(eventCache.DateTime), eventType, source));
                foreach (object o in data)
                {
                    WriteLine(o);
                }
            }
        }

        /// <summary>The trace transfer.</summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="source">The source.</param>
        /// <param name="id">The id.</param>
        /// <param name="message">The message.</param>
        /// <param name="relatedActivityId">The related activity id.</param>
        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message,
           Guid relatedActivityId)
        {
            if (IsEnabled)
            {
                WriteLine(string.Format(TransferTraceFormatString, DateTimeToString(eventCache.DateTime), source,
                   relatedActivityId, message));
            }
        }

        /// <summary>The get supported attributes.</summary>
        /// <returns>The <see cref="string" />.</returns>
        protected override string[] GetSupportedAttributes()
        {
            List<string> attrs = new List<string>();
            string[] baseAttributes = base.GetSupportedAttributes();
            if (baseAttributes != null)
            {
                attrs.AddRange(baseAttributes);
            }

            attrs.Add(IsEnabledPropertyName);
            return attrs.ToArray();
        }

        #endregion public overrides

        #region private helpers

        private void HandleMaxFileSize()
        {
            const int kByteToByteFactor = 1024;

            FileInfo fi = new FileInfo(logFilePath);

            if (fi.Length > MaxLogSize * kByteToByteFactor)
            {
                base.WriteLine("Size exceeded: " + fi.Length / kByteToByteFactor + "kB");

                Writer.Close();

                if (File.Exists(logFilePath + ".bak"))
                {
                    File.Delete(logFilePath + ".bak");
                }

                File.Copy(logFilePath, logFilePath + ".bak");
                File.Delete(logFilePath);

                Writer = GetWriter(logFilePath);
            }
        }

        private TextWriter GetWriter(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("No filename found in argument 'path'.");
            }

            if (string.IsNullOrEmpty(dir))
            {
                dir = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            }

            if (!Directory.Exists(dir))
            {
                throw new IOException("Trace log directory does not exist: " + dir);
            }

            // Creates a new file.
            TextWriter myWriter = new StreamWriter(dir + "\\" + fileName, true);
            logFilePath = dir + "\\" + fileName;

            return myWriter;
        }

        private void EnsureStateKnown()
        {
            if (state == EnabledState.Unknown)
            {
                state = EnabledState.Disabled; // prevent further calls on exceptions
                bool isEnabled = true; // default if attribute is missing
                if (Attributes.ContainsKey(IsEnabledPropertyName))
                {
                    string propertyValue = Attributes[IsEnabledPropertyName];
                    if (!bool.TryParse(propertyValue, out isEnabled))
                    {
                        throw new InvalidConfigFileException(
                           "LogFileTraceListener: Cannot parse value" + propertyValue + " for property " +
                           IsEnabledPropertyName + "!");
                    }
                }

                state = EnabledState.Enabled; // enable in any case for WriteLine() below
                if (!isEnabled)
                {
                    WriteLine(
                       string.Format(
                          FullTraceFormatString,
                          DateTimeToString(DateTime.Now),
                          TraceEventType.Information,
                          Name,
                          "Logging to text file is disabled by configuration."));
                    state = EnabledState.Disabled;
                }
            }
        }

        private string DateTimeToString(DateTime dt)
        {
            // we log local times!
            return dt.ToLocalTime().ToString(DateTimeFormatString, CultureInfo.InvariantCulture);
        }

        #endregion private helpers
    }
}