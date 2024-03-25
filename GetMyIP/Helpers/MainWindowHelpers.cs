// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class MainWindowHelpers
{
    #region Startup
    internal static async Task GetMyIPStartUp()
    {
        EventHandlers();

        if (!App.LogOnly)
        {
            MainWindowUIHelpers.ApplyUISettings();

            string returnedJson = await IpHelpers.GetAllInfoAsync();
            IpHelpers.ProcessProvider(returnedJson, false);

            if (UserSettings.Setting.StartMinimized && UserSettings.Setting.MinimizeToTray)
            {
                EnableTrayIcon(true);
                WindowExtensions.Hide(_mainWindow);
            }
            else if (UserSettings.Setting.StartMinimized && !UserSettings.Setting.MinimizeToTray)
            {
                _mainWindow.WindowState = WindowState.Minimized;
                _mainWindow.Visibility = Visibility.Visible;
                EnableTrayIcon(false);
            }
            else if (!UserSettings.Setting.StartMinimized && UserSettings.Setting.MinimizeToTray)
            {
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Visibility = Visibility.Visible;
                EnableTrayIcon(true);
            }
            else
            {
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Visibility = Visibility.Visible;
                EnableTrayIcon(false);
            }
        }
        else
        {
            _mainWindow.Visibility = Visibility.Hidden;
            string returnedJson = await IpHelpers.GetExternalInfo();
            IpHelpers.LogIPInfo(returnedJson);
            App.ExplicitClose = true;
            _mainWindow.Close();
        }
    }
    #endregion Startup

    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region StopWatch
    public static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    #endregion StopWatch

    #region Set and Save MainWindow position and size
    /// <summary>
    /// Sets the MainWindow position and size.
    /// </summary>
    public static void SetWindowPosition()
    {
        Window mainWindow = Application.Current.MainWindow;
        mainWindow.Height = UserSettings.Setting.WindowHeight;
        mainWindow.Left = UserSettings.Setting.WindowLeft;
        mainWindow.Top = UserSettings.Setting.WindowTop;
        mainWindow.Width = UserSettings.Setting.WindowWidth;

        if (UserSettings.Setting.StartCentered)
        {
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }

    /// <summary>
    /// Saves the MainWindow position and size.
    /// </summary>
    public static void SaveWindowPosition()
    {
        Window mainWindow = Application.Current.MainWindow;
        UserSettings.Setting.WindowHeight = Math.Floor(mainWindow.Height);
        UserSettings.Setting.WindowLeft = Math.Floor(mainWindow.Left);
        UserSettings.Setting.WindowTop = Math.Floor(mainWindow.Top);
        UserSettings.Setting.WindowWidth = Math.Floor(mainWindow.Width);
    }
    #endregion Set and Save MainWindow position and size

    #region Get property value
    /// <summary>
    /// Gets the value of the property
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns>An object containing the value of the property</returns>
    public static object GetPropertyValue(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        return prop?.GetValue(sender, null);
    }
    #endregion Get property value

    #region Window Title
    /// <summary>
    /// Puts the version number in the title bar as well as Administrator if running elevated
    /// </summary>
    public static string WindowTitleVersionAdmin()
    {
        // Set the windows title
        return AppInfo.IsAdmin
            ? $"{AppInfo.AppProduct}  {AppInfo.AppProductVersion} - ({GetStringResource("MsgText_WindowTitleAdministrator")})"
            : $"{AppInfo.AppProduct}  {AppInfo.AppProductVersion}";
    }
    #endregion Window Title

    #region Event handlers
    /// <summary>
    /// Event handlers.
    /// </summary>
    internal static void EventHandlers()
    {
        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // Settings change events
        UserSettings.Setting.PropertyChanged += SettingChange.UserSettingChanged;
        TempSettings.Setting.PropertyChanged += SettingChange.TempSettingChanged;

        // Window closing event
        _mainWindow.Closing += MainWindow_Closing;

        //Window loaded event
        _mainWindow.Loaded += MainWindow_Loaded;

        // Window state changed (minimized, maximized, etc.)
        _mainWindow.StateChanged += MainWindow_StateChanged;
    }
    #endregion Event handlers

    #region Window Events
    private static void MainWindow_StateChanged(object sender, EventArgs e)
    {
        if (_mainWindow.WindowState == WindowState.Minimized && UserSettings.Setting.MinimizeToTray)
        {
            _mainWindow.Hide();
        }
    }

    private static void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (UserSettings.Setting.AutoRefresh)
        {
            RefreshHelpers.StartTimer();
        }
    }

    private static void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // If MinimizeToTrayOnClose is true then clicking X on title bar will minimize instead of closing the app
        if (!App.ExplicitClose && UserSettings.Setting.MinimizeToTray && UserSettings.Setting.MinimizeToTrayOnClose)
        {
            _mainWindow.Hide();
            e.Cancel = true;
        }
        else
        {
            // Clear any remaining messages
            _mainWindow.SnackBar1.MessageQueue.Clear();

            // Stop the _stopwatch and record elapsed time
            _stopwatch.Stop();
            _log.Info($"{AppInfo.AppName} {GetStringResource("MsgText_ApplicationShutdown")}.  " +
                $"{GetStringResource("MsgText_ElapsedTime")}: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

            // Shut down NLog
            LogManager.Shutdown();

            // Dispose of the tray icon
            _mainWindow.tbIcon.Dispose();

            // Save settings
            if (_mainWindow.Visibility == Visibility.Visible)
            {
                SaveWindowPosition();
            }
            ConfigHelpers.SaveSettings();

            // Remove any outstanding toast notifications
            ToastNotificationManagerCompat.History.Clear();
            ToastNotificationManagerCompat.Uninstall();
        }
    }
    #endregion Window Events

    #region Show MainWindow
    /// <summary>
    /// Show the main window and set it's state to normal
    /// </summary>
    public static void ShowMainWindow()
    {
        Application.Current.MainWindow.Show();
        Application.Current.MainWindow.Visibility = Visibility.Visible;
        Application.Current.MainWindow.WindowState = WindowState.Normal;
        Application.Current.MainWindow.ShowInTaskbar = true;
        _ = Application.Current.MainWindow.Activate();
    }
    #endregion Show MainWindow

    #region Minimize to tray
    public static void EnableTrayIcon(bool value)
    {
        if (value)
        {
            _mainWindow.tbIcon.ForceCreate();
            _mainWindow.tbIcon.Visibility = Visibility.Visible;
            CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(false);
        }
        else
        {
            _mainWindow.tbIcon.Visibility = Visibility.Collapsed;
        }
    }
    #endregion Minimize to tray

    #region Write startup messages to the log
    /// <summary>
    /// Initializes NLog and writes startup messages to the log.
    /// </summary>
    internal static void LogStartup()
    {
        // Set NLog configuration
        NLogConfig();

        // Log the version, build date and commit id
        _log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {AppInfo.AppVersion} {GetStringResource("MsgText_ApplicationStarting")}");
        _log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        _log.Debug($"{AppInfo.AppName} Build date: {BuildInfo.BuildDateStringUtc}");
        _log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString}");
        _log.Debug($"{AppInfo.AppName} was started from {AppInfo.AppPath}");
        _log.Debug($"{AppInfo.AppName} Process ID: {AppInfo.AppProcessID}");
        if (AppInfo.IsAdmin)
        {
            _log.Debug($"{AppInfo.AppName} is running as Administrator");
        }

        // Log the .NET version and OS platform
        _log.Debug($"Operating System version: {AppInfo.OsPlatform}");
        _log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}");
    }
    #endregion Write startup messages to the log

    #region Unhandled Exception Handler
    /// <summary>
    /// Handles any exceptions that weren't caught by a try-catch statement.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <remarks>
    /// This uses default message box.
    /// </remarks>
    internal static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        _log.Error("Unhandled Exception");
        Exception e = (Exception)args.ExceptionObject;
        _log.Error(e.Message);
        if (e.InnerException != null)
        {
            _log.Error(e.InnerException.ToString());
        }
        _log.Error(e.StackTrace);

        string msg = string.Format($"{GetStringResource("MsgText_Error")}\n{e.Message}");
        _ = MessageBox.Show(msg,
            "Get My IP ERROR",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
    #endregion Unhandled Exception Handler
}
