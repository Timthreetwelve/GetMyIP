﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

static class CommandLineHelpers
{
    #region NLog Instance
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Process the command line
    /// <summary>
    /// Parse any command line options
    /// </summary>
    public static async void ProcessCommandLine()
    {
        // Since this is not a console app, get the command line args
        string[] args = Environment.GetCommandLineArgs();

        // Parser settings
        Parser parser = new(s =>
        {
            s.CaseSensitive = false;
            s.IgnoreUnknownArguments = true;
        });

        // parses the command line. result object will hold the arguments
        ParserResult<CommandLineOptions> result = parser.ParseArguments<CommandLineOptions>(args);

        // Check options
        if (result?.Value.Hide == true)
        {
            _log.Debug("Argument \"hide\" specified.");
            _mainWindow.Visibility = Visibility.Hidden;
            await ExternalInfoViewModel.GetExtInfo();
            ExternalInfoViewModel.LogIPInfo();
            _mainWindow.Close();
        }
        else
        {
            // for performance
            List<Task> tasks = new()
            {
                new Task(async () => await ExternalInfoViewModel.GetExtInfo() ),
                new Task(async () => await InternalInfoViewModel.GetMyInternalIP()),
                new Task(() => SettingsViewModel.ParseInitialPage())
            };
            _ = Parallel.ForEach(tasks, task => task.Start());
            await Task.WhenAll(tasks);
            MainWindowHelpers.EnableTrayIcon(UserSettings.Setting.MinimizeToTray);
        }
    }
    #endregion Process the command line
}
