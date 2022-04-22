// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Helper;
using KukaRoboter.Common.Tracing;
using System;
using System.Diagnostics;
using System.IO;

namespace Kuka.FlexDrill.SmartHMI.Production.Log
{
    /// <summary>The logger.</summary>
    public class Logger
    {
        #region Constants and Fields

        private const SourceLevels LogWriterLevel = SourceLevels.All;

        private static readonly PrettyTraceSource LogSource = TraceSourceFactory.GetSource("FlexDrill");

        #endregion Constants and Fields

        #region Constructors and Destructor

        /// <summary>Initializes a new instance of the <see cref="Logger" /> class.</summary>
        public Logger()
        {
            InitLogging();
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <summary>The write exception in log file.</summary>
        public void WriteException(Exception e)
        {
            LogSource.WriteException(e, true);
        }

        /// <summary>The write message in log file.</summary>
        public void WriteMessage(TraceEventType type, string text, string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                LogSource.TraceEvent(type, 4, text);
            }
            else
            {
                LogSource.TraceEvent(type, 4, text + "\n" + stackTrace);
            }
        }

        /// <summary>Close the logging source.</summary>
        public void Close()
        {
            if (LogSource != null)
            {
                WriteMessage(TraceEventType.Stop, "LogSource closing...", string.Empty);
                LogSource.Close();
            }
        }

        #endregion Interface

        #region Methods

        private void InitLogging()
        {
            Trace.AutoFlush = true;

            string LogFilePath = Path.Combine(Constants.LogFileFolder, Constants.LogFileName);
            FileInfo logFileInfo = new FileInfo(LogFilePath);
            DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);

            //! Create Directory if not exists
            if (!logDirInfo.Exists)
            {
                logDirInfo.Create();
            }

            RollingLogFileTraceListener textTraceWriter = new RollingLogFileTraceListener(LogFilePath);

            LogSource.Switch.Level = LogWriterLevel;
            LogSource.Listeners.Add(textTraceWriter);

            WriteMessage(TraceEventType.Start, "Logging started", string.Empty);
        }

        #endregion Methods
    }
}