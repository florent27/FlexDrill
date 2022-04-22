// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="KUKA Systems Aerospace">
//    Copyright (c) KUKA Systems Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;

namespace Kuka.FlexDrill.Process.Logger
{
    public static class Logger
    {
        private const int MaxRowInLog = 20000;
        private const string logFileFolder = "FlexDrill";
        private const string cszFormatDateFileName = "MM-dd-yyyy_HH_mm_ss";
        private const string cszFormatDateLogLine = "MM/dd/yyyy HH:mm:ss:fff - ";
        private const string logFileFoler = @"C:\KRC\ROBOTER\LOG";

        public static bool FullDebug;

        private static FileStream fileStream;
        private static int RowCount;
        private static string logFilePath;
        private static int CurrentDay = (int)DateTime.Now.DayOfWeek;

        public static void WriteLog(string strLog, bool IsPartOfFullDebug)
        {
            StreamWriter log;
            try
            {
                if (strLog != "")
                {
                    if ((!IsPartOfFullDebug) || (IsPartOfFullDebug && FullDebug))
                    {
                        if (logFilePath != "")
                        {
                            FileInfo logFileInfo = new FileInfo(logFilePath);
                            DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);

                            //! Create Directory if not exists
                            if (!logDirInfo.Exists)
                            {
                                logDirInfo.Create();
                            }
                            //
                            //! Check File Number & Build File Name
                            //
                            CheckFileNumber();
                            CheckDayChange();
                            if (!logFileInfo.Exists)
                            {
                                fileStream = logFileInfo.Create();
                            }
                            else
                            {
                                fileStream = new FileStream(logFilePath, FileMode.Append);
                            }

                            log = new StreamWriter(fileStream);
                            strLog = DateTime.Now.ToString(cszFormatDateLogLine) + strLog;
                            log.WriteLine(strLog);
                            RowCount++;
                            log.Close();
                        }
                    }
                }
            }
            catch
            {
                BuildLogFileName();
            }
        }

        private static void CheckDayChange()
        {
            int cd = (int)DateTime.Now.DayOfWeek;
            if ((cd != CurrentDay) || (RowCount > MaxRowInLog))
            {
                CurrentDay = cd;
                RowCount = 0;
                BuildLogFileName();
            }
        }

        private static void CheckFileNumber()
        {
            FileInfo logFileInfo = new FileInfo(logFilePath);
            string[] files = Directory.GetFiles(logFileInfo.DirectoryName);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                //! Do not Delete FlexDrill.log
                if (fi.Name != "FlexDrill.log")
                {
                    if (fi.CreationTime < DateTime.Now.AddDays(-30))
                        fi.Delete();
                }
            }
        }

        public static void BuildLogFileName()
        {
            string FileName = "Log_" + DateTime.Now.ToString(cszFormatDateFileName) + ".txt";
            logFilePath = Path.Combine(logFileFoler, logFileFolder, FileName);
        }
    }
}