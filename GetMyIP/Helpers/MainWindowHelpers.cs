// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class MainWindowHelpers
{
    #region Properties
    /// <summary>
    /// Used to hold the previous WindowState. Used to determine if a refresh is needed.
    /// </summary>
    private static WindowState PreviousState { get; set; }
    #endregion Properties

    #region Startup
    internal static async Task GetMyIPStartUp()
    {
        EventHandlers();

        if (!App.LogOnly)
        {
            ApplyUISettings();

            string returnedJson = await IpHelpers.GetAllInfoAsync();
            IpHelpers.ProcessProvider(returnedJson, false);

            if (UserSettings.Setting!.StartMinimized && UserSettings.Setting.MinimizeToTray)
            {
                // Minimized is needed here so that the WindowState changed event
                // will fire for the first restore.
                _mainWindow!.WindowState = WindowState.Minimized;
                WindowExtensions.Hide(_mainWindow);
                EnableTrayIcon(true);
            }
            else if (UserSettings.Setting.StartMinimized && !UserSettings.Setting.MinimizeToTray)
            {
                _mainWindow!.WindowState = WindowState.Minimized;
                _mainWindow.Visibility = Visibility.Visible;
                EnableTrayIcon(false);
            }
            else if (!UserSettings.Setting.StartMinimized && UserSettings.Setting.MinimizeToTray)
            {
                _mainWindow!.WindowState = WindowState.Normal;
                _mainWindow.Visibility = Visibility.Visible;
                EnableTrayIcon(true);
            }
            else
            {
                _mainWindow!.WindowState = WindowState.Normal;
                _mainWindow.Visibility = Visibility.Visible;
                EnableTrayIcon(false);
            }
            PreviousState = _mainWindow.WindowState;
        }
        else
        {
            _mainWindow!.Visibility = Visibility.Hidden;
            string returnedJson = await IpHelpers.GetExternalInfo();
            IpHelpers.LogIPInfo(returnedJson);
            App.ExplicitClose = true;
            _mainWindow.Close();
        }
    }
    #endregion Startup

    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region StopWatch
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    #endregion StopWatch

    #region Set and Save MainWindow position and size
    /// <summary>
    /// Sets the MainWindow position and size.
    /// </summary>
    private static void SetWindowPosition()
    {
        if (_mainWindow is null)
        {
            return;
        }
        _mainWindow.Height = UserSettings.Setting!.WindowHeight;
        _mainWindow.Left = UserSettings.Setting.WindowLeft;
        _mainWindow.Top = UserSettings.Setting.WindowTop;
        _mainWindow.Width = UserSettings.Setting.WindowWidth;

        if (UserSettings.Setting.StartCentered)
        {
            _mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        KeepWindowOnScreen();
    }

    /// <summary>
    /// Saves the MainWindow position and size.
    /// </summary>
    private static void SaveWindowPosition()
    {
        Window? mainWindow = Application.Current.MainWindow;
        UserSettings.Setting!.WindowHeight = Math.Floor(mainWindow!.Height);
        UserSettings.Setting.WindowLeft = Math.Floor(mainWindow.Left);
        UserSettings.Setting.WindowTop = Math.Floor(mainWindow.Top);
        UserSettings.Setting.WindowWidth = Math.Floor(mainWindow.Width);
    }
    #endregion Set and Save MainWindow position and size

    #region Reposition off-screen window back to the desktop
    /// <summary>
    /// Keep the window on the screen.
    /// </summary>
    private static void KeepWindowOnScreen()
    {
        if (_mainWindow is null || !UserSettings.Setting!.KeepWindowOnScreen)
        {
            return;
        }

        if (_mainWindow.Top < SystemParameters.VirtualScreenTop)
        {
            _mainWindow.Top = SystemParameters.VirtualScreenTop;
        }

        if (_mainWindow.Left < SystemParameters.VirtualScreenLeft)
        {
            _mainWindow.Left = SystemParameters.VirtualScreenLeft;
        }

        if (_mainWindow.Left + _mainWindow.Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
        {
            _mainWindow.Left = SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft - _mainWindow.Width;
        }

        if (_mainWindow.Top + _mainWindow.Height > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight)
        {
            _mainWindow.Top = SystemParameters.WorkArea.Size.Height + SystemParameters.VirtualScreenTop - _mainWindow.Height;
        }
    }
    #endregion Reposition off-screen window back to the desktop

    #region Center the window on the screen

    #region Window Title
    /// <summary>
    /// Puts the version number in the title bar as well as Administrator if running elevated
    /// </summary>
    private static string WindowTitleVersionAdmin()
    {
        // Set the windows title
        return AppInfo.IsAdmin
            ? $"{AppInfo.AppProduct}  {AppInfo.AppVersion} - ({GetStringResource("MsgText_WindowTitleAdministrator")})"
            : $"{AppInfo.AppProduct}  {AppInfo.AppVersion}";
    }
    #endregion Window Title

    #region Event handlers
    /// <summary>
    /// Event handlers.
    /// </summary>
    private static void EventHandlers()
    {
        // Settings change events
        UserSettings.Setting!.PropertyChanged += SettingChange.UserSettingChanged!;
        TempSettings.Setting!.PropertyChanged += SettingChange.TempSettingChanged!;

        // Window closing event
        _mainWindow!.Closing += MainWindow_Closing!;

        //Window loaded event
        _mainWindow.Loaded += MainWindow_Loaded;

        // Window state changed (minimized, maximized, etc.)
        _mainWindow.StateChanged += MainWindow_StateChanged!;
    }
    #endregion Event handlers

    #region Window Events

    #region State changed
    private static async void MainWindow_StateChanged(object sender, EventArgs e)
    {
        try
        {
            if (_mainWindow!.WindowState == WindowState.Normal)
            {
                KeepWindowOnScreen();
            }

            // if window state is minimized and minimize to tray setting is true then hide the window
            if (_mainWindow.WindowState == WindowState.Minimized &&
                UserSettings.Setting!.MinimizeToTray)
            {
                _mainWindow.Hide();
            }

            // if window was minimized and refresh after restoring setting is true then refresh IP info
            if (PreviousState == WindowState.Minimized && UserSettings.Setting!.RefreshAfterRestore)
            {
                _log.Debug("Main window restored from minimized. Initiating a refresh.");
                await NavigationViewModel.RefreshExternalAsync();
            }

            // set property to the current window state
            PreviousState = _mainWindow.WindowState;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error in MainWindow_StateChanged method");
        }
    }
    #endregion State changed

    #region Loaded
    private static void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (UserSettings.Setting!.AutoRefresh)
        {
            RefreshHelpers.StartTimer();
        }
    }
    #endregion Loaded

    #region Closing
    private static void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // If MinimizeToTrayOnClose is true then clicking X on title bar will minimize instead of closing the app
        if (!App.ExplicitClose && UserSettings.Setting!.MinimizeToTray && UserSettings.Setting.MinimizeToTrayOnClose)
        {
            // Minimized is needed here so that the WindowState changed event will fire.
            _mainWindow!.WindowState = WindowState.Minimized;
            _mainWindow.Hide();
            e.Cancel = true;
        }
        else
        {
            // Clear any remaining messages
            _mainWindow!.SnackBar1.MessageQueue!.Clear();

            // Stop the _stopwatch and record elapsed time
            _stopwatch.Stop();
            _log.Info($"{AppInfo.AppName} {GetStringResource("MsgText_ApplicationShutdown")}.  " +
                $"{GetStringResource("MsgText_ElapsedTime")}: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

            // Shut down NLog
            LogManager.Shutdown();

            // Dispose of the tray icon
            _mainWindow.TbIcon.Dispose();

            // Save settings
            if (_mainWindow.Visibility == Visibility.Visible)
            {
                SaveWindowPosition();
            }
            ConfigHelpers.SaveSettings();
        }
    }
    #endregion Closing

    #endregion Window Events

    #region Log Startup messages
    /// <summary>
    /// Initializes NLog and writes startup messages to the log.
    /// </summary>
    internal static void LogStartup()
    {
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
        if (CommandLineHelpers.CommandLineParserError is not null)
        {
            _log.Warn(CommandLineHelpers.CommandLineParserError);
        }

        // Log the .NET version and OS platform
        _log.Debug($"Operating System version: {AppInfo.OsPlatform}");
        _log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}");
    }
    #endregion Log Startup messages

    #region Set theme
    /// <summary>
    /// Gets the current theme
    /// </summary>
    /// <returns>Dark or Light</returns>
    private static string GetSystemTheme()
    {
        BaseTheme? sysTheme = Theme.GetSystemTheme();
        return sysTheme != null ? sysTheme.ToString()! : string.Empty;
    }

    /// <summary>
    /// Sets the theme
    /// </summary>
    /// <param name="mode">Light, Dark, Darker or System</param>
    internal static void SetBaseTheme(ThemeType mode)
    {
        //Retrieve the app's existing theme
        PaletteHelper paletteHelper = new();
        Theme theme = paletteHelper.GetTheme();

        if (mode == ThemeType.System)
        {
            mode = GetSystemTheme().Equals("light", StringComparison.OrdinalIgnoreCase) ? ThemeType.Light : ThemeType.Darker;
        }

        switch (mode)
        {
            case ThemeType.Light:
                theme.SetBaseTheme(BaseTheme.Light);
                theme.Background = Colors.WhiteSmoke;
                break;
            case ThemeType.Dark:
                theme.SetBaseTheme(BaseTheme.Dark);
                theme.DataGrids.RowHoverBackground = (Color)ColorConverter.ConvertFromString("#FF303030");
                break;
            case ThemeType.Darker:
                // Set card and paper background colors a bit darker
                theme.SetBaseTheme(BaseTheme.Dark);
                theme.Cards.Background = (Color)ColorConverter.ConvertFromString("#FF141414");
                theme.Background = (Color)ColorConverter.ConvertFromString("#FF202020");
                theme.Foreground = (Color)ColorConverter.ConvertFromString("#E5F0F0F0");
                theme.DataGrids.Selected = (Color)ColorConverter.ConvertFromString("#FF303030");
                break;
            default:
                theme.SetBaseTheme(BaseTheme.Light);
                break;
        }

        //Change the app's current theme
        paletteHelper.SetTheme(theme);
    }
    #endregion Set theme

    #region Set accent color
    /// <summary>
    /// Sets the MDIX primary accent color
    /// </summary>
    /// <param name="color">One of the 18 MDIX color values plus Black and White</param>
    internal static void SetPrimaryColor(AccentColor color)
    {
        PaletteHelper paletteHelper = new();
        Theme theme = paletteHelper.GetTheme();
        PrimaryColor primary = color switch
        {
            AccentColor.Red => PrimaryColor.Red,
            AccentColor.Pink => PrimaryColor.Pink,
            AccentColor.Purple => PrimaryColor.Purple,
            AccentColor.DeepPurple => PrimaryColor.DeepPurple,
            AccentColor.Indigo => PrimaryColor.Indigo,
            AccentColor.Blue => PrimaryColor.Blue,
            AccentColor.LightBlue => PrimaryColor.LightBlue,
            AccentColor.Cyan => PrimaryColor.Cyan,
            AccentColor.Teal => PrimaryColor.Teal,
            AccentColor.Green => PrimaryColor.Green,
            AccentColor.LightGreen => PrimaryColor.LightGreen,
            AccentColor.Lime => PrimaryColor.Lime,
            AccentColor.Yellow => PrimaryColor.Yellow,
            AccentColor.Amber => PrimaryColor.Amber,
            AccentColor.Orange => PrimaryColor.Orange,
            AccentColor.DeepOrange => PrimaryColor.DeepOrange,
            AccentColor.Brown => PrimaryColor.Brown,
            AccentColor.Gray => PrimaryColor.Grey,
            AccentColor.BlueGray => PrimaryColor.BlueGrey,
            _ => PrimaryColor.Blue,
        };
        if (color == AccentColor.Black)
        {
            theme.SetPrimaryColor(Colors.Black);
        }
        else if (color == AccentColor.White)
        {
            theme.SetPrimaryColor(Colors.White);
        }
        else
        {
            Color primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];
            theme.SetPrimaryColor(primaryColor);
        }
        paletteHelper.SetTheme(theme);
    }
    #endregion Set accent color

    #region Set UI size
    /// <summary>
    /// Sets the value for UI scaling
    /// </summary>
    /// <param name="size">One of 7 values</param>
    /// <returns>Scaling multiplier</returns>
    internal static void UIScale(MySize size)
    {
        double newSize = size switch
        {
            MySize.Smallest => 0.8,
            MySize.Smaller => 0.9,
            MySize.Small => 0.95,
            MySize.Default => 1.0,
            MySize.Large => 1.05,
            MySize.Larger => 1.1,
            MySize.Largest => 1.2,
            _ => 1.0,
        };
        _mainWindow!.MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
    }

    /// <summary>
    /// Decreases the size of the UI
    /// </summary>
    public static void EverythingSmaller()
    {
        MySize size = UserSettings.Setting!.UISize;
        if (size > 0)
        {
            size--;
            UserSettings.Setting.UISize = size;
            UIScale(UserSettings.Setting.UISize);
        }
    }

    /// <summary>
    /// Increases the size of the UI
    /// </summary>
    public static void EverythingLarger()
    {
        MySize size = UserSettings.Setting!.UISize;
        if (size < MySize.Largest)
        {
            size++;
            UserSettings.Setting.UISize = size;
            UIScale(UserSettings.Setting.UISize);
        }
    }
    #endregion Set UI size

    #region Apply UI settings
    /// <summary>
    /// Single method called during startup to apply UI settings.
    /// </summary>
    private static void ApplyUISettings()
    {
        // Put version number in window title
        _mainWindow!.Title = MainWindowHelpers.WindowTitleVersionAdmin();

        // Window position
        MainWindowHelpers.SetWindowPosition();

        // Light or dark theme
        SetBaseTheme(UserSettings.Setting!.UITheme);

        // Primary accent color
        SetPrimaryColor(UserSettings.Setting.PrimaryColor);

        // UI size
        UIScale(UserSettings.Setting.UISize);
    }
    #endregion Apply UI settings

    #region Show MainWindow
    /// <summary>
    /// Show the main window and set it's state to normal
    /// </summary>
    public static void ShowMainWindow()
    {
        Application.Current.MainWindow!.Show();
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
            _mainWindow!.TbIcon.ForceCreate();
            _mainWindow.TbIcon.Visibility = Visibility.Visible;
            CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(false);
        }
        else
        {
            _mainWindow!.TbIcon.Visibility = Visibility.Collapsed;
        }
    }
    #endregion Minimize to tray

    #region Find a parent of a control
    /// <summary>
    /// Finds the Parent of the given item in the visual tree.
    /// </summary>
    /// <typeparam name="T">The type of the queried item.</typeparam>
    /// <param name="child">x:Name or Name of child.</param>
    /// <returns>The parent object.</returns>
    public static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child)!;

        return parentObject switch
        {
            null => null!,
            T parent => parent,
            _ => FindParent<T>(parentObject),
        };
    }
    #endregion Find a parent of a control
}
