// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

public partial class MainWindow : MaterialWindow
{
    #region NLog Instance
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Stopwatch
    private readonly Stopwatch _stopwatch = new();
    #endregion Stopwatch

    public MainWindow()
    {
        ConfigHelpers.InitializeSettings();

        InitializeComponent();

        ReadSettings();

        ProcessCommandLine();

        SettingsViewModel.ParseInitialPage();
    }

    #region Settings
    public void ReadSettings()
    {
        // Set NLog configuration
        NLogHelpers.NLogConfig();

        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // Put the version number in the title bar
        Title = $"{AppInfo.AppProduct} - {AppInfo.TitleVersion}";

        // Window position
        Top = UserSettings.Setting.WindowTop;
        Left = UserSettings.Setting.WindowLeft;
        Height = UserSettings.Setting.WindowHeight;
        Width = UserSettings.Setting.WindowWidth;
        Topmost = UserSettings.Setting.KeepOnTop;

        // Log the version, build date and commit id
        _log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {AppInfo.AppVersion} is starting up");
        _log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        _log.Debug($"{AppInfo.AppName} Build date: {BuildInfo.BuildDateUtc.ToUniversalTime():f} (UTC)");
        _log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString} ");

        // Log the .NET version, app framework and OS platform
        string version = Environment.Version.ToString();
        _log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}  ({version})");
        _log.Debug(AppInfo.Framework);
        _log.Debug(AppInfo.OsPlatform);

        // Light or dark
        SetBaseTheme(UserSettings.Setting.UITheme);

        // Primary color
        SetPrimaryColor((AccentColor)UserSettings.Setting.PrimaryColor);

        // UI size
        double size = UIScale(UserSettings.Setting.UISize);
        MainGrid.LayoutTransform = new ScaleTransform(size, size);

        // Initial page viewed
        NavigateToPage((NavPage)UserSettings.Setting.InitialPage);

        // Settings change event
        UserSettings.Setting.PropertyChanged += UserSettingChanged;
    }
    #endregion Settings

    #region Setting change
    private async void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        object newValue = prop?.GetValue(sender, null);
        _log.Debug($"Setting change: {e.PropertyName} New Value: {newValue}");
        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.KeepOnTop):
                Topmost = (bool)newValue;
                break;

            case nameof(UserSettings.Setting.IncludeV6):
                await InternalIP.GetMyInternalIP();
                break;

            case nameof(UserSettings.Setting.IncludeDebug):
                NLogHelpers.SetLogLevel((bool)newValue);
                break;

            case nameof(UserSettings.Setting.UITheme):
                SetBaseTheme((ThemeType)newValue);
                break;

            case nameof(UserSettings.Setting.PrimaryColor):
                SetPrimaryColor((AccentColor)newValue);
                break;

            case nameof(UserSettings.Setting.LogFile):
                using (FileTarget nlogTarget = LogManager.Configuration.FindTargetByName("logPerm") as FileTarget)
                {
                    nlogTarget.FileName = UserSettings.Setting.LogFile;
                }
                LogManager.ReconfigExistingLoggers();
                break;

            case nameof(UserSettings.Setting.UISize):
                int size = (int)newValue;
                double newSize = UIScale((MySize)size);
                MainGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
                break;

            case nameof(UserSettings.Setting.MinimizeToTray):
                MinimizeToTray((bool)newValue);
                break;
        }
    }
    #endregion Setting change

    #region Navigation
    /// <summary>
    /// Navigates to the requested dialog or page
    /// </summary>
    private void NavigateToPage(NavPage page)
    {
        NavListBox.SelectedIndex = (int)page;
        _ = NavListBox.Focus();
        switch (page)
        {
            default:
                _ = MainFrame.Navigate(new Page1());
                PageTitle.Text = "Internal IP Addresses";
                break;
            case NavPage.External:
                _ = MainFrame.Navigate(new Page2());
                PageTitle.Text = "External IP & Geolocation Info";
                break;
            case NavPage.Settings:
                _ = MainFrame.Navigate(new SettingsPage());
                PageTitle.Text = "Settings";
                break;
            case NavPage.About:
                _ = MainFrame.Navigate(new AboutPage());
                PageTitle.Text = "About";
                break;
            case NavPage.Exit:
                Application.Current.Shutdown();
                break;
        }
    }

    private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        NavigateToPage((NavPage)NavListBox.SelectedIndex);
    }
    #endregion Navigation

    #region Theme
    /// <summary>
    /// Gets the current theme
    /// </summary>
    /// <returns>Dark or Light</returns>
    internal static string GetSystemTheme()
    {
        BaseTheme? sysTheme = Theme.GetSystemTheme();
        return sysTheme != null ? sysTheme.ToString() : string.Empty;
    }

    /// <summary>
    /// Sets the theme
    /// </summary>
    /// <param name="mode">Light, Dark, Darker or System</param>
    internal void SetBaseTheme(ThemeType mode)
    {
        SolidColorBrush _titleBrush;

        //Get the current Windows accent color
        try
        {
            _titleBrush = ColorHelper.GetAccentColor();
        }
        catch (Exception ex)
        {
            _titleBrush = (SolidColorBrush)SystemParameters.WindowGlassBrush;
            _log.Debug($"GetAccentColor failed: {ex.Message}. Using WindowGlassBrush.");
        }

        //Retrieve the app's existing theme
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();

        if (mode == ThemeType.System)
        {
            mode = GetSystemTheme()!.Equals("light", StringComparison.Ordinal) ? ThemeType.Light : ThemeType.Darker;
        }

        switch (mode)
        {
            case ThemeType.Light:
                theme.SetBaseTheme(Theme.Light);
                BorderBackgroundBrush = _titleBrush;
                BorderForegroundBrush = ColorHelper.IsBrushDark(_titleBrush) ? Brushes.White : (Brush)Brushes.Black;
                theme.Paper = Colors.WhiteSmoke;
                break;
            case ThemeType.Dark:
                theme.SetBaseTheme(Theme.Dark);
                BorderBackgroundBrush = _titleBrush;
                BorderForegroundBrush = ColorHelper.IsBrushDark(_titleBrush) ? Brushes.White : (Brush)Brushes.Black;
                break;
            case ThemeType.Darker:
                // Set card and paper background colors a bit darker
                theme.SetBaseTheme(Theme.Dark);
                theme.Body = (Color)ColorConverter.ConvertFromString("#FFCCCCCC");
                theme.Paper = (Color)ColorConverter.ConvertFromString("#FF141414");
                theme.CardBackground = (Color)ColorConverter.ConvertFromString("#FF202020");
                BorderForegroundBrush = Brushes.WhiteSmoke;
                BorderBackgroundBrush = new SolidColorBrush(theme.Paper);
                break;
            default:
                theme.SetBaseTheme(Theme.Light);
                break;
        }

        //Change the app's current theme
        paletteHelper.SetTheme(theme);
    }
    #endregion Theme

    #region Set primary color
    /// <summary>
    /// Sets the MDIX primary accent color
    /// </summary>
    /// <param name="color">One of the 18 color values</param>
    private static void SetPrimaryColor(AccentColor color)
    {
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();
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
            AccentColor.Grey => PrimaryColor.Grey,
            AccentColor.BlueGray => PrimaryColor.BlueGrey,
            _ => PrimaryColor.Blue,
        };
        Color primaryColor = SwatchHelper.Lookup[(MaterialDesignColor)primary];
        theme.SetPrimaryColor(primaryColor);
        paletteHelper.SetTheme(theme);
    }
    #endregion Set primary color

    #region UI scale converter
    /// <summary>
    /// Sets the value for UI scaling
    /// </summary>
    /// <param name="size">One of 7 values</param>
    /// <returns>double used by LayoutTransform</returns>
    private static double UIScale(MySize size)
    {
        return size switch
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
    }
    #endregion UI scale converter

    #region Command line arguments
    /// <summary>
    /// Processes any command line arguments
    /// </summary>
    private async void ProcessCommandLine()
    {
        string[] args = Environment.GetCommandLineArgs();

        if (args.Length < 2)
        {
            return;
        }

        foreach (string item in args)
        {
            // If command line argument "write" or "hide" is found, execute without showing window.
            string arg = item.Replace("-", "").Replace("/", "").ToLower();

            if (arg == "write" || arg == "hide")
            {
                _log.Info($"Command line argument \"{item}\" found.");
                Visibility = Visibility.Hidden;
                await ExternalInfo.GetExtInfo();
                ExternalInfo.LogIPInfo();
                Application.Current.Shutdown();
            }
            else if (item != args[0])
            {
                _log.Info($"Extraneous command line argument  \"{item}\" found.");
            }
        }
    }
    #endregion Command line arguments

    #region Window Events
    private async void Window_ContentRendered(object sender, EventArgs e)
    {
        await InternalIP.GetMyInternalIP();

        await ExternalInfo.GetExtInfo();

        MinimizeToTray(UserSettings.Setting.MinimizeToTray);
    }

    private void Window_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized && UserSettings.Setting.MinimizeToTray)
        {
            Hide();
        }
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        _stopwatch.Stop();
        _log.Info($"{AppInfo.AppName} is shutting down.  Elapsed time: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

        tbIcon.Dispose();

        // Shut down NLog
        LogManager.Shutdown();

        // Save settings
        UserSettings.Setting.WindowLeft = Math.Floor(Left);
        UserSettings.Setting.WindowTop = Math.Floor(Top);
        UserSettings.Setting.WindowWidth = Math.Floor(Width);
        UserSettings.Setting.WindowHeight = Math.Floor(Height);
        ConfigHelpers.SaveSettings();
    }
    #endregion Window Events

    #region PopupBox button events
    /// <summary>
    /// Handles the view log button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnLog_Click(object sender, RoutedEventArgs e)
    {
        TextFileViewer.ViewTextFile(NLogHelpers.GetLogfileName());
    }

    /// <summary>
    /// Handles the view ReadMe button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnReadme_Click(object sender, RoutedEventArgs e)
    {
        string dir = AppInfo.AppDirectory;
        TextFileViewer.ViewTextFile(Path.Combine(dir, "ReadMe.txt"));
    }

    /// <summary>
    /// Handles the copy to clipboard button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnCopyToClip_Click(object sender, RoutedEventArgs e)
    {
        CopytoClipBoard();
    }

    /// <summary>
    /// Handles the save to text file button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSaveText_Click(object sender, RoutedEventArgs e)
    {
        Copyto2TextFile();
    }

    /// <summary>
    /// Handles the view on map button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnShowMap_Click(object sender, RoutedEventArgs e)
    {
        ShowMap();
    }
    #endregion PopupBox button events

    #region Show Lat. Long. location in browser
    /// <summary>
    /// Opens the selected browser to the specified latitude and longitude
    /// </summary>
    private static void ShowMap()
    {
        IPInfo lat = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Latitude");
        IPInfo lon = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Longitude");
        string url = UserSettings.Setting.MapProvider switch
        {
            (int)MapProvider.Bing => $"https://www.bing.com/maps/default.aspx?cp={lat.Value}~{lon.Value}&lvl=12",
            (int)MapProvider.LatLong => $"https://www.latlong.net/c/?lat={lat.Value}&long={lon.Value}",
            _ => $"https://www.google.com/maps/@{lat.Value},{lon.Value},12z",
        };
        try
        {
            using Process p = new();
            p.StartInfo.FileName = url;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.ErrorDialog = false;
            _ = p.Start();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Unable to open default browser");

            _ = MessageBox.Show("Unable to open default browser. See the log file",
                                "ERROR",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
        }
    }
    #endregion Show Lat. Long. location in browser

    #region Keyboard Events
    /// <summary>
    /// Keyboard events for window
    /// </summary>
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            if (e.Key == Key.E)
            {
                NavigateToPage(NavPage.About);
            }

            if (e.Key == Key.I)
            {
                NavigateToPage(0);
            }

            if (e.Key == Key.M)
            {
                switch (UserSettings.Setting.UITheme)
                {
                    case ThemeType.Light:
                        UserSettings.Setting.UITheme = ThemeType.Dark;
                        break;
                    case ThemeType.Dark:
                        UserSettings.Setting.UITheme = ThemeType.Darker;
                        break;
                    case ThemeType.Darker:
                        UserSettings.Setting.UITheme = ThemeType.System;
                        break;
                    case ThemeType.System:
                        UserSettings.Setting.UITheme = ThemeType.Light;
                        break;
                }
            }

            if (e.Key == Key.N)
            {
                if (UserSettings.Setting.PrimaryColor >= (int)AccentColor.BlueGray)
                {
                    UserSettings.Setting.PrimaryColor = 0;
                }
                else
                {
                    UserSettings.Setting.PrimaryColor++;
                }
            }

            if (e.Key == Key.Add)
            {
                EverythingLarger();
            }

            if (e.Key == Key.Subtract)
            {
                EverythingSmaller();
            }

            if (e.Key == Key.OemComma)
            {
                NavigateToPage(NavPage.Settings);
            }
        }

        if (e.Key == Key.F1)
        {
            NavigateToPage(NavPage.About);
        }
    }
    #endregion Keyboard Events

    #region Minimize to tray
    private void MinimizeToTray(bool value)
    {
        if (value)
        {
            tbIcon.Visibility = Visibility.Visible;
            BuildToolTip();
        }
        else
        {
            tbIcon.Visibility = Visibility.Collapsed;
        }
    }
    #endregion Minimize to tray

    #region Smaller/Larger
    private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.Control)
            return;

        if (e.Delta > 0)
        {
            EverythingLarger();
        }
        else if (e.Delta < 0)
        {
            EverythingSmaller();
        }
    }

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
    #endregion Smaller/Larger

    #region Window Title
    /// <summary>
    /// Sets the window title
    /// </summary>
    public void WindowTitleVersion()
    {
        Title = $"{AppInfo.AppName} - {AppInfo.TitleVersion}";
    }
    #endregion Window Title

    #region Copy to clipboard and text file
    /// <summary>
    /// Copy related methods
    /// </summary>
    private static void CopytoClipBoard()
    {
        StringBuilder sb = ListView2Sb();
        // Clear the clipboard of any previous text
        Clipboard.Clear();
        // Copy to clipboard
        Clipboard.SetText(sb.ToString());
        _log.Debug("IP information copied to clipboard");
    }

    private static void Copyto2TextFile()
    {
        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Title = "Save",
            Filter = "Text File|*.txt",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            FileName = "IP_Info.txt"
        };
        var result = dialog.ShowDialog();
        if (result == true)
        {
            StringBuilder sb = ListView2Sb();
            File.WriteAllText(dialog.FileName, sb.ToString());
            _log.Debug($"IP information written to {dialog.FileName}");
        }
    }

    private static StringBuilder ListView2Sb()
    {
        //Get ListView contents and separate parameter and value with a tab
        StringBuilder sb = new();
        foreach (IPInfo item in IPInfo.InternalList)
        {
            sb.Append(item.Parameter).Append('\t').AppendLine(item.Value);
        }
        foreach (IPInfo item in IPInfo.GeoInfoList)
        {
            sb.Append(item.Parameter).Append('\t').AppendLine(item.Value);
        }
        return sb;
    }
    #endregion Copy to clipboard and text file

    #region Unhandled Exception Handler
    /// <summary>
    /// Handles any exceptions that weren't caught by a try-catch statement
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        _log.Error("Unhandled Exception");
        Exception e = (Exception)args.ExceptionObject;
        _log.Error(e.Message);
        if (e.InnerException != null)
        {
            _log.Error(e.InnerException.ToString());
        }
        _log.Error(e.StackTrace);

        _ = MessageBox.Show("An error has occurred. See the log file",
                            "ERROR",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
    }
    #endregion Unhandled Exception Handler

    #region Refresh IP info
    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await RefreshInfo();
    }

    private async Task RefreshInfo()
    {
        _log.Debug("Refreshing IP info...");

        await InternalIP.GetMyInternalIP();

        await ExternalInfo.GetExtInfo();

        BuildToolTip();
    }
    #endregion Refresh IP info

    #region RoutedUICommand methods
    private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void QuitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }

    private void ShowCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        ShowMainWindow();
    }

    private async void RefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        await RefreshInfo();
    }
    #endregion RoutedUICommand methods

    #region Show the main window
    /// <summary>
    /// Show the main window and set it's state to normal
    /// </summary>
    public static void ShowMainWindow()
    {
        Application.Current.MainWindow.Show();
        Application.Current.MainWindow.Visibility = Visibility.Visible;
        Application.Current.MainWindow.WindowState = WindowState.Normal;
        _ = Application.Current.MainWindow.Activate();
    }
    #endregion Show the main window

    #region Build the tool tip text
    /// <summary>
    /// Builds the tool tip text based on options selected by the user.
    /// </summary>
    public void BuildToolTip()
    {
        if (tbIcon is not null)
        {
            StringBuilder sb = new();

            if (IPInfo.InternalList.Any(x => x.Parameter == "Internal IPv4 Address") && UserSettings.Setting.ShowInternalIPv4)
            {
                string intAddrV4 = IPInfo.InternalList.FirstOrDefault(x => x.Parameter == "Internal IPv4 Address").Value;
                _ = sb.AppendLine(intAddrV4);
            }

            if (IPInfo.InternalList.Any(x => x.Parameter == "Internal IPv6 Address") && UserSettings.Setting.ShowInternalIPv6)
            {
                string intAddrV6 = IPInfo.InternalList.FirstOrDefault(x => x.Parameter == "Internal IPv6 Address").Value;
                _ = sb.AppendLine(intAddrV6);
            }

            if (IPInfo.GeoInfoList.Any(x => x.Parameter == "External IP Address") && UserSettings.Setting.ShowExternalIP)
            {
                string extAddr = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "External IP Address").Value;
                _ = sb.AppendLine(extAddr);
            }

            if (IPInfo.GeoInfoList.Any(x => x.Parameter == "City") && UserSettings.Setting.ShowCity)
            {
                string city = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "City").Value;
                _ = sb.AppendLine(city);
            }

            if (IPInfo.GeoInfoList.Any(x => x.Parameter == "State") && UserSettings.Setting.ShowState)
            {
                string state = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "State").Value;
                _ = sb.AppendLine(state);
            }

            if (IPInfo.GeoInfoList.Any(x => x.Parameter == "Country") && UserSettings.Setting.ShowCountry)
            {
                string country = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Country").Value;
                _ = sb.AppendLine(country);
            }

            if (IPInfo.GeoInfoList.Any(x => x.Parameter == "Offset from UTC") && UserSettings.Setting.ShowOffset)
            {
                string offset = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Offset from UTC").Value;
                _ = sb.AppendLine(offset);
            }

            if (IPInfo.GeoInfoList.Any(x => x.Parameter == "Time Zone") && UserSettings.Setting.ShowTimeZone)
            {
                string timezone = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Time Zone").Value;
                _ = sb.AppendLine(timezone);
            }

            if (IPInfo.GeoInfoList.Any(x => x.Parameter == "ISP") && UserSettings.Setting.ShowISP)
            {
                string isp = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "ISP").Value;
                _ = sb.AppendLine(isp);
            }
            int tooltipSize = sb.Length;
            if (tooltipSize == 0)
            {
                _ = sb.AppendLine(AppInfo.AppProduct);
            }
            _log.Debug($"Tooltip text is {sb.Length} bytes.");
            tbIcon.ToolTipText = sb.ToString();
        }
    }
    #endregion Build the tool tip text
}