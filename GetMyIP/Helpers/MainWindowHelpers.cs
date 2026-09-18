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

            SetInitialWindowState();

            await ProcessInitialDelay();

            string returnedJson = await IpHelpers.GetAllInfoAsync();

            IpHelpers.ProcessProvider(returnedJson, false);

            EnableTrayIcon(UserSettings.Setting.MinimizeToTray);
        }
        else
        {
            if (!TryGetMainWindow(out MainWindow? mainWindow))
            {
                return;
            }
            mainWindow.Visibility = Visibility.Hidden;
            string returnedJson = await IpHelpers.GetExternalInfo();
            IpHelpers.LogIPInfo(returnedJson);
            App.ExplicitClose = true;
            mainWindow.Close();
        }
    }
    #endregion Startup

    #region Startup delay
    /// <summary>
    /// Delay the startup of the application for a specified number of seconds.
    /// </summary>
    private static async Task ProcessInitialDelay()
    {
        // Ensure that the initial delay is between 0 and 600 seconds.
        // This will prevent the user from entering a negative number or a number greater than 10 minutes.
        UserSettings.Setting.InitialDelaySecs = Math.Min(Math.Max(UserSettings.Setting.InitialDelaySecs, 0), 600);

        if (UserSettings.Setting.InitialDelaySecs > 0)
        {
            _log.Debug($"Initial delay of {UserSettings.Setting.InitialDelaySecs} seconds");
            MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_WaitDelayExpire"),
                MessageHelpers.ErrorSource.both, true);
            await Task.Delay(TimeSpan.FromSeconds(UserSettings.Setting.InitialDelaySecs));
        }
    }
    #endregion Startup delay

    #region Set window state
    private static void SetInitialWindowState()
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        if (UserSettings.Setting.StartMinimized)
        {
            mainWindow.WindowState = WindowState.Minimized;
            if (UserSettings.Setting.MinimizeToTray)
            {
                WindowExtensions.Hide(mainWindow);
            }
            else
            {
                mainWindow.Visibility = Visibility.Visible;
            }
        }
        else
        {
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Visibility = Visibility.Visible;
        }

        PreviousState = mainWindow.WindowState;
    }
    #endregion Set window state

    #region StopWatch
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    #endregion StopWatch

    #region Set and Save MainWindow position and size
    /// <summary>
    /// Sets the MainWindow position and size.
    /// </summary>
    private static void SetWindowPosition()
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        mainWindow.Height = UserSettings.Setting.WindowHeight;
        mainWindow.Left = UserSettings.Setting.WindowLeft;
        mainWindow.Top = UserSettings.Setting.WindowTop;
        mainWindow.Width = UserSettings.Setting.WindowWidth;

        if (UserSettings.Setting.StartCentered)
        {
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        else if (UserSettings.Setting.KeepWindowOnScreen)
        {
            ScreenHelpers.KeepWindowOnScreen(mainWindow);
        }
    }

    /// <summary>
    /// Saves the MainWindow position and size.
    /// </summary>
    public static void SaveWindowPosition()
    {
        SaveWindowSize();
        SaveWindowLocation();
    }

    private static void SaveWindowSize()
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        UserSettings.Setting.WindowHeight = Math.Floor(mainWindow.Height);
        UserSettings.Setting.WindowWidth = Math.Floor(mainWindow.Width);
    }

    private static void SaveWindowLocation()
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        UserSettings.Setting.WindowLeft = Math.Floor(mainWindow.Left);
        UserSettings.Setting.WindowTop = Math.Floor(mainWindow.Top);
    }
    #endregion Set and Save MainWindow position and size

    #region Window Title
    /// <summary>
    /// Puts the version number in the title bar as well as Administrator if running elevated
    /// </summary>
    private static string WindowTitleVersionAdmin()
    {
        // Set the windows title
        return AppInfo.IsAdmin
            ? $"{AppInfo.AppProduct}  {BuildInfo.VersionString} - ({GetStringResource("MsgText_WindowTitleAdministrator")})"
            : $"{AppInfo.AppProduct}  {BuildInfo.VersionString}";
    }
    #endregion Window Title

    #region Event handlers
    /// <summary>
    /// Event handlers.
    /// </summary>
    private static void EventHandlers()
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        // Settings change events
        UserSettings.Setting.PropertyChanged += SettingChange.UserSettingChanged!;
        TempSettings.Setting.PropertyChanged += SettingChange.TempSettingChanged!;

        // Window closing event
        mainWindow.Closing += MainWindow_Closing!;

        //Window loaded event
        mainWindow.Loaded += MainWindow_Loaded;

        // Window state changed (minimized, maximized, etc.)
        mainWindow.StateChanged += MainWindow_StateChanged!;
    }
    #endregion Event handlers

    #region Window Events

    #region State changed
    private static void MainWindow_StateChanged(object sender, EventArgs e)
    {
        _ = MainWindow_StateChangedAsync(sender);
    }

    private static async Task MainWindow_StateChangedAsync(object sender)
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        try
        {
            if (!Equals(sender, mainWindow))
            {
                return;
            }

            switch (mainWindow.WindowState)
            {
                case WindowState.Minimized:
                    {
                        SaveWindowLocation();
                        ConfigHelpers.SaveSettings();

                        if (UserSettings.Setting.MinimizeToTray)
                        {
                            mainWindow.Hide();
                        }

                        PreviousState = mainWindow.WindowState;
                        break;
                    }

                case WindowState.Normal:
                    {
                        if (PreviousState == WindowState.Minimized)
                        {
                            if (UserSettings.Setting.RefreshAfterRestore)
                            {
                                _log.Debug("Main window restored from minimized. Initiating a refresh.");
                                await NavigationViewModel.RefreshExternalAsync();
                            }

                            if (UserSettings.Setting.RestoreToInitialPage)
                            {
                                mainWindow.NavigationListBox.SelectedValue = NavigationViewModel.FindNavPage(UserSettings.Setting.InitialPage);
                            }
                        }

                        if (UserSettings.Setting.StartCentered && UserSettings.Setting.RestoreToCenter)
                        {
                            ScreenHelpers.CenterTheWindow(mainWindow);
                            PreviousState = mainWindow.WindowState;
                            return;
                        }

                        if (UserSettings.Setting.KeepWindowOnScreen)
                        {
                            ScreenHelpers.KeepWindowOnScreen(mainWindow);
                            SaveWindowPosition();
                        }

                        PreviousState = mainWindow.WindowState;
                        break;
                    }

                case WindowState.Maximized:
                    {
                        PreviousState = mainWindow.WindowState;
                        break;
                    }
            }
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
        if (UserSettings.Setting.AutoRefresh)
        {
            RefreshHelpers.StartTimer();
        }
    }
    #endregion Loaded

    #region Closing
    private static void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        // If MinimizeToTrayOnClose is true then clicking X on title bar will minimize instead of closing the app
        if (!App.ExplicitClose && UserSettings.Setting.MinimizeToTray && UserSettings.Setting.MinimizeToTrayOnClose)
        {
            // Minimized is needed here so that the WindowState changed event will fire.
            mainWindow.WindowState = WindowState.Minimized;
            mainWindow.Hide();
            e.Cancel = true;
        }
        else
        {
            // Clear any remaining messages
            mainWindow.SnackBar1.MessageQueue!.Clear();

            // Stop the _stopwatch and record elapsed time
            _stopwatch.Stop();
            _log.Info($"{AppInfo.AppName} {GetStringResource("MsgText_ApplicationShutdown")}.  " +
                $"{GetStringResource("MsgText_ElapsedTime")}: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");


            // Dispose of the tray icon
            mainWindow.TbIcon.Dispose();

            if (mainWindow.Visibility == Visibility.Visible)
            {
                SaveWindowPosition();
            }

            // Save settings
            ConfigHelpers.SaveSettings();

            // Shut down NLog
            LogManager.Shutdown();
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
        // Log the version, commit date and commit id
        _log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {BuildInfo.VersionString} {GetStringResource("MsgText_ApplicationStarting")}");
        _log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        _log.Debug($"{AppInfo.AppName} Commit date: {BuildInfo.CommitDateStringUtc} - {BuildInfo.CommitDateStringLocal}");
        _log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString}");
        _log.Debug($"{AppInfo.AppName} was started from {PathHelpers.AnonymizePath(AppInfo.AppPath)}");
        _log.Debug($"{AppInfo.AppName} Process ID: {AppInfo.AppProcessID}");
        _log.Debug($"{AppInfo.AppName} App architecture: {AppInfo.Architecture}");
        if (!string.IsNullOrEmpty(BuildInfo.Prerelease))
        {
            _log.Warn($"{AppInfo.AppName} is a prerelease version: {BuildInfo.Prerelease}");
        }
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
            mode = GetSystemTheme().Equals("light", StringComparison.OrdinalIgnoreCase)
                ? UserSettings.Setting.SystemLightTheme
                : UserSettings.Setting.SystemDarkTheme;
        }

        switch (mode)
        {
            case ThemeType.Light: // Light
                theme.SetBaseTheme(BaseTheme.Light);
                theme.Background = Colors.WhiteSmoke;
                theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.RedSecondary]);
                break;
            case ThemeType.LightGray: // Pale Graphite
                theme.SetBaseTheme(BaseTheme.Light);
                theme.Background = (Color)ColorConverter.ConvertFromString("#FFD3D3D3");
                theme.Foreground = (Color)ColorConverter.ConvertFromString("#EE111111");
                theme.Cards.Background = (Color)ColorConverter.ConvertFromString("#FFE0E0E0");
                theme.DataGrids.Selected = (Color)ColorConverter.ConvertFromString("#FFC0C0C0");
                theme.Separators.Background = (Color)ColorConverter.ConvertFromString("#FFA9A9A9");
                theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.RedSecondary]);
                break;
            case ThemeType.Dark: // Material Design Dark
                theme.SetBaseTheme(BaseTheme.Dark);
                theme.DataGrids.RowHoverBackground = (Color)ColorConverter.ConvertFromString("#FF303030");
                theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.DeepOrange500]);
                break;
            case ThemeType.Darker: // Darker
                theme.SetBaseTheme(BaseTheme.Dark);
                theme.Cards.Background = (Color)ColorConverter.ConvertFromString("#FF141414");
                theme.Background = (Color)ColorConverter.ConvertFromString("#FF202020");
                theme.Foreground = (Color)ColorConverter.ConvertFromString("#E5F0F0F0");
                theme.DataGrids.Selected = (Color)ColorConverter.ConvertFromString("#FF303030");
                theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.DeepOrange500]);
                break;
            case ThemeType.DarkBlue: // Midnight Blue
                theme.SetBaseTheme(BaseTheme.Dark);
                theme.Background = (Color)ColorConverter.ConvertFromString("#FF000F25");
                theme.Cards.Background = (Color)ColorConverter.ConvertFromString("#FF011636");
                theme.DataGrids.Selected = (Color)ColorConverter.ConvertFromString("#FF274470");
                theme.Foreground = (Color)ColorConverter.ConvertFromString("#FFD3D3E3");
                theme.GridSplitters.Background = (Color)ColorConverter.ConvertFromString("#46516A");
                theme.Separators.Background = (Color)ColorConverter.ConvertFromString("#FF003C85");
                theme.ToolTips.Background = (Color)ColorConverter.ConvertFromString("#FF63afff");
                theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.DeepOrange500]);
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
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        mainWindow.MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
    }

    /// <summary>
    /// Decreases the size of the UI
    /// </summary>
    public static void EverythingSmaller()
    {
        MySize size = UserSettings.Setting.UISize;
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
        MySize size = UserSettings.Setting.UISize;
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
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        mainWindow.Title = WindowTitleVersionAdmin();

        // Window position
        SetWindowPosition();

        // Light or dark theme
        SetBaseTheme(UserSettings.Setting.UITheme);

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
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        mainWindow.Show();
        mainWindow.Visibility = Visibility.Visible;
        mainWindow.WindowState = WindowState.Normal;
        mainWindow.ShowInTaskbar = true;
        _ = mainWindow.Activate();
    }
    #endregion Show MainWindow

    #region Minimize to tray
    public static void EnableTrayIcon(bool value)
    {
        if (!TryGetMainWindow(out MainWindow? mainWindow))
        {
            return;
        }
        if (value)
        {
            mainWindow.TbIcon.ForceCreate();
            mainWindow.TbIcon.Visibility = Visibility.Visible;
            TrayIconHelpers.SetTrayIcon();
            CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(true);
        }
        else
        {
            mainWindow.TbIcon.Visibility = Visibility.Collapsed;
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

    #region Get MainWindow instance
    /// <summary>
    /// Tries to get the MainWindow instance.
    /// </summary>
    /// <param name="window">The MainWindow instance if available.</param>
    /// <returns>True if the MainWindow instance is available; otherwise, false.</returns>
    private static bool TryGetMainWindow([NotNullWhen(true)] out MainWindow? window)
    {
        window = Application.Current?.MainWindow as MainWindow;
        if (window is null)
        {
            _log.Warn("MainWindow is not available. Unable to perform tasks related to the MainWindow.");
            return false;
        }
        return true;
    }
    #endregion Get MainWindow instance
}
