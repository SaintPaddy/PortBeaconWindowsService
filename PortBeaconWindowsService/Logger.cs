using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PortBeaconWindowsService
{
    public static class Logger
    {
        private static bool useEventLog = true;
        private static bool useFileLog = false;
        private static readonly string sourceName = "PortBeaconService";
        private static readonly string logFileName = "service.log";
        private static readonly long maxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public static void Configure(bool enableEventLog, bool enableFileLog)
        {
            useEventLog = enableEventLog;
            useFileLog = enableFileLog;

            if (useEventLog && !EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, "Application");
            }
        }

        public static void Log(string message)
        {
            string timestampedMessage = $"[{Utilities.Timestamp()}] {message}";

            if (useEventLog)
            {
                try
                {
                    EventLog.WriteEntry(sourceName, message, EventLogEntryType.Information);
                }
                catch
                {
                    // Ignore EventLog write failures (e.g., permission issues)
                }
            }

            if (useFileLog)
            {
                try
                {
                    RotateLogIfNeeded();
                    File.AppendAllText(logFileName, timestampedMessage + Environment.NewLine);
                }
                catch
                {
                    // Ignore file write errors
                }
            }
        }

        private static void RotateLogIfNeeded()
        {
            try
            {
                if (File.Exists(logFileName))
                {
                    var fileInfo = new FileInfo(logFileName);
                    if (fileInfo.Length > maxFileSizeBytes)
                    {
                        string archiveName = $"service-{DateTime.Now:yyyyMMdd-HHmmss}.log";
                        File.Move(logFileName, archiveName);

                        var logFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "service-*.log")
                            .OrderByDescending(f => File.GetCreationTime(f))
                            .ToList();

                        int maxLogs = ConfigLoader.Current?.MaxLogFiles ?? 10;

                        if (maxLogs > 0 && logFiles.Count > maxLogs)
                        {
                            foreach (var oldFile in logFiles.Skip(maxLogs))
                            {
                                File.Delete(oldFile);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ConfigLoader.Current?.EnableDebugLog == true)
                {
                    Log($"[DEBUG] Error during log rotation: {ex.Message}");
                }
            }
        }


    }
}
