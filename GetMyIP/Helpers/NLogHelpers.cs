﻿// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

/// <summary>
/// Class for NLog helper methods
/// </summary>
internal static class NLogHelpers
{
    /// <summary>
    /// Static instance for NLog Logger.
    /// </summary>
    /// <remarks>
    /// Used with a "static using" in GlobalUsings.cs to avoid creating an instance in every class.
    /// </remarks>
    internal static readonly Logger _log = LogManager.GetLogger("logTemp");

    #region Create the NLog configuration
    /// <summary>
    /// Configure NLog
    /// </summary>
    public static void NLogConfig()
    {
        // Uncomment the following lines to enable NLog internal logging for debugging purposes
        //NLog.Common.InternalLogger.LogLevel = LogLevel.Debug;
        //NLog.Common.InternalLogger.LogToConsole = true;
        //NLog.Common.InternalLogger.LogFile = @"d:\temp\nlog-internal.txt";

        LoggingConfiguration config = new();

        #region Log file in temp folder
        // create log file Target for NLog
        FileTarget logtemp = new("logTemp")
        {
            FileName = CreateFilename(),
            Footer = "${date:format=yyyy/MM/dd HH\\:mm\\:ss.ff}",
            Layout = "${date:format=yyyy/MM/dd HH\\:mm\\:ss.ff} " +
                         "${pad:padding=-5:inner=${level:uppercase=true}}  " +
                         "${message}${onexception:${newline}${exception:format=tostring}}"
        };

        // add the log file target
        config.AddTarget(logtemp);

        // add the rule for the log file
        LoggingRule file = new("logTemp", LogLevel.Debug, logtemp)
        {
            RuleName = "LogToFile"
        };
        config.LoggingRules.Add(file);
        #endregion Log file in temp folder

        #region Permanent log file
        // create log file Target for NLog

        string permLogFile = string.IsNullOrEmpty(UserSettings.Setting!.LogFile)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GetMyIP.log")
            : UserSettings.Setting.LogFile;

        FileTarget logperm = new("logPerm")
        {
            // get file name from settings
            FileName = permLogFile,

            // message layout
            Layout = "${date:format=yyyy/MM/dd HH\\:mm\\:ss}  ${message}",

            // Close the log file after writing to it
            KeepFileOpen = false,
        };

        // add the file target
        config.AddTarget(logperm);

        // add the rule for the log file
        LoggingRule perm = new("logPerm", LogLevel.Debug, logperm)
        {
            RuleName = "LogPerm"
        };
        config.LoggingRules.Add(perm);
        #endregion Permanent log file

        #region Debugger
        // create debugger target
        DebuggerTarget debugger = new("debugger")
        {
            Layout = "${processtime} >>> ${message} "
        };

        // add the target
        config.AddTarget(debugger);

        // add the rule
        LoggingRule bug = new("*", LogLevel.Trace, debugger);
        config.LoggingRules.Add(bug);

        // add the configuration to NLog
        LogManager.Configuration = config;
        #endregion Debugger

        // Lastly, set the logging level based on setting
        SetLogLevel(UserSettings.Setting.IncludeDebug);
    }
    #endregion Create the NLog configuration

    #region Create the file path in the temp folder
    /// <summary>
    /// Creates the file path.
    /// </summary>
    /// <returns></returns>
    private static string CreateFilename()
    {
        // create filename string
        string myname = AppInfo.AppName;
        string today = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        string filename = Debugger.IsAttached ? $"{myname}.{today}.debug.log" : $"{myname}.{today}.log";

        // combine temp folder with filename
        string tempdir = Path.GetTempPath();
        return Path.Combine(tempdir, "T_K", filename);
    }
    #endregion Create the file path in the temp folder

    #region Set NLog logging level
    /// <summary>
    /// Set the NLog logging level to Debug or Info
    /// </summary>
    /// <param name="debug">If true set level to Debug, otherwise set to Info</param>
    public static void SetLogLevel(bool debug)
    {
        LoggingConfiguration? config = LogManager.Configuration;

        LoggingRule? rule = config!.FindRuleByName("LogToFile");
        if (rule != null)
        {
            LogLevel level = debug ? LogLevel.Debug : LogLevel.Info;
            rule.SetLoggingLevels(level, LogLevel.Fatal);
            LogManager.ReconfigExistingLoggers();
        }
    }
    #endregion Set NLog logging level

    #region Get the log file name
    /// <summary>
    /// Gets the filename for the NLog log fie
    /// </summary>
    /// <returns></returns>
    public static string GetLogfileName()
    {
        LoggingConfiguration? config = LogManager.Configuration;
        Target? target = config!.FindTargetByName("logtemp");
        if (target is FileTarget ft)
        {
            // remove the enclosing apostrophes
            return ft.FileName.ToString()!.Trim('\'');
        }
        return string.Empty;
    }
    #endregion Get the log file name
}
