// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

// Based on https://github.com/it3xl/WPF-app-Single-Instance-in-one-line-of-code

namespace GetMyIP.Helpers;

/// <summary>
/// Class to only allow a single instance of the application to run.
/// </summary>
/// <remarks>
/// If another instance of the application is started, it will activate
/// the first instance and then shut down.
/// </remarks>
public static class SingleInstance
{
    #region Private fields
    private static bool _alreadyProcessedOnThisInstance;
    #endregion Private fields

    #region Create the application or exit if application exists
    /// <summary>Creates a single instance of the application.</summary>
    /// <param name="appName">Name of the application.</param>
    /// <param name="uniquePerUser">if set to <c>true</c> unique per user.</param>
    internal static void Create(string appName, bool uniquePerUser = true)
    {
        if (_alreadyProcessedOnThisInstance)
        {
            return;
        }
        _alreadyProcessedOnThisInstance = true;

        // If hide was specified, skip single instance check
        CommandLineHelpers.CommandLineArgs commandLine = CommandLineHelpers.ProcessCommandLine();
        if (commandLine == CommandLineHelpers.CommandLineArgs.Hide)
        {
            App.LogOnly = true;
            return;
        }

        // If restart was specified, pause for a bit to let the previous instance shut down
        if (commandLine == CommandLineHelpers.CommandLineArgs.Restart)
        {
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
        }

        Application app = Application.Current;

        const string uniqueID = "{5FBEE561-8ED8-4032-9587-C46259B4D3F6}";
        string eventName = uniquePerUser ? $"{appName}-{uniqueID}-{Environment.UserName}" : $"{appName}-{uniqueID}";

        if (EventWaitHandle.TryOpenExisting(eventName, out EventWaitHandle? eventWaitHandle))
        {
            ActivateFirstInstanceWindow(eventWaitHandle);

            Environment.Exit(0);
        }

        RegisterFirstInstanceWindowActivation(app, eventName);
    }
    #endregion Create the application or exit if application exists

    #region Set the event
    /// <summary>Sets the event</summary>
    /// <param name="eventWaitHandle">The event wait handle.</param>
    private static void ActivateFirstInstanceWindow(EventWaitHandle eventWaitHandle)
    {
        _ = eventWaitHandle.Set();
    }
    #endregion Set the event

    #region Create the event handle and register the instance
    /// <summary>Registers the first instance window activation.</summary>
    /// <param name="app">The application.</param>
    /// <param name="eventName">Name of the event.</param>
    private static void RegisterFirstInstanceWindowActivation(Application app, string eventName)
    {
        EventWaitHandle eventWaitHandle = new(
            false,
            EventResetMode.AutoReset,
            eventName);

        _ = ThreadPool.RegisterWaitForSingleObject(
                eventWaitHandle,
                WaitOrTimerCallback!,
                app,
                Timeout.Infinite, false);
        eventWaitHandle.Close();
    }
    #endregion Create the event handle and register the instance

    #region Show the main window
    /// <summary>Shows the main window of the original instance</summary>
    private static void WaitOrTimerCallback(object state, bool timedOut)
    {
        Application app = (Application)state;
        _ = app.Dispatcher.BeginInvoke(new Action(MainWindowHelpers.ShowMainWindow));
        if (_log.IsDebugEnabled)
        {
            _log.Debug($"This instance of {AppInfo.AppName} was activated because another instance attempted to start. ");
        }
    }
    #endregion Show the main window
}
