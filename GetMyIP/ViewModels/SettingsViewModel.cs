// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Properties
    public static List<FontFamily>? FontList { get; private set; }

    public IEnumerable<ThemeType> ThemeTypes { get; }

    public IEnumerable<ThemeType> SystemThemeTypes { get; private set; }

    [ObservableProperty]
    private string _refreshIntervalDisplay = string.Empty;

    [ObservableProperty]
    private int _timePickerHours;

    [ObservableProperty]
    private int _timePickerMinutes;

    [ObservableProperty]
    private int _timePickerSeconds;
    #endregion Properties

    #region Constants
    private const int MinRefreshSeconds = 10;
    private const int MaxRefreshSeconds = 86400; // 24 hours
    private const int DefaultRefreshSeconds = 3600; // One hour
    #endregion Constants

    #region Constructor
    public SettingsViewModel()
    {
        FontList ??= [.. Fonts.SystemFontFamilies.OrderBy(x => x.Source)];

        ThemeTypes = [
            ThemeType.Light,
            ThemeType.LightGray,
            ThemeType.Dark,
            ThemeType.Darker,
            ThemeType.DarkBlue,
            ThemeType.System,
        ];

        // Used when ThemeType.System is selected. Will display all themes except System theme
        SystemThemeTypes = ThemeTypes.Where(static t => t != ThemeType.System);

        RefreshIntervalDisplay = FormatRefreshIntervalDisplay();

        UpdateTimePicker();
    }
    #endregion Constructor

    #region Handle refresh interval change
    /// <summary>
    /// Validates the combined Hours/Minutes/Seconds refresh interval. If the total is outside
    /// the allowed range, reverts to one hour and notifies the user; otherwise restarts the timer.
    /// </summary>
    public void HandleRefreshIntervalChanged()
    {
        int requestedSeconds = (TimePickerHours * 3600) + (TimePickerMinutes * 60) + TimePickerSeconds;
        UserSettings.Setting.AutoRefreshSeconds = VerifyRefreshInterval(requestedSeconds);

        UpdateRefresh();
        RefreshIntervalDisplay = FormatRefreshIntervalDisplay();
        UpdateTimePicker();
    }
    #endregion Handle refresh interval change

    #region Verify refresh interval
    /// <summary>
    /// Verifies that the refresh interval is within the allowed range. If not, it reverts to the default value and shows a message box.
    /// </summary>
    /// <param name="intervalSeconds">The refresh interval in seconds.</param>
    /// <returns>The verified refresh interval in seconds.</returns>
    public static int VerifyRefreshInterval(int intervalSeconds)
    {
        if (intervalSeconds is < MinRefreshSeconds or > MaxRefreshSeconds)
        {
            _log.Warn($"Invalid refresh interval ({intervalSeconds} seconds). Must be between {MinRefreshSeconds} sec and {MaxRefreshSeconds} sec. "
                      + "Reverting to one hour (01:00:00).");
            //TimeSpan defaultTime = TimeSpan.FromSeconds(DefaultRefreshSeconds);
            intervalSeconds = DefaultRefreshSeconds;
            string localizedMinimum = ToHourString(TimeSpan.FromSeconds(MinRefreshSeconds));
            string localizedMaximum = ToHourString(TimeSpan.FromSeconds(MaxRefreshSeconds));
            string msg = string.Format(CultureInfo.CurrentCulture, MsgTextErrorInvalidRefreshInterval, localizedMinimum, localizedMaximum);
            msg += $"\n\n{GetStringResource("MsgText_RefreshIntervalReverted")} ({TimeSpan.FromSeconds(DefaultRefreshSeconds):g})";

            _ = ShowMsgBox(msg,
                    GetStringResource("MsgText_Error_Caption"),
                    true);
        }
        return intervalSeconds;
    }
    #endregion Verify refresh interval

    #region Update TimePicker
    /// <summary>
    /// Updates the TimePickerHours, TimePickerMinutes, and TimePickerSeconds properties based on the current
    /// AutoRefreshSeconds setting.
    /// </summary>
    public void UpdateTimePicker()
    {
        TimeSpan totalSeconds = TimeSpan.FromSeconds(UserSettings.Setting.AutoRefreshSeconds);
        TimePickerHours = (int)totalSeconds.TotalHours;
        TimePickerMinutes = totalSeconds.Minutes;
        TimePickerSeconds = totalSeconds.Seconds;
    }
    #endregion Update TimePicker

    #region Format Refresh Interval Display
    /// <summary>
    /// Returns a string representation of the current refresh interval in HH:mm:ss format. The string is used for
    /// display purposes in the UI.
    /// </summary>
    private static string FormatRefreshIntervalDisplay()
    {
        return ToHourString(TimeSpan.FromSeconds(UserSettings.Setting.AutoRefreshSeconds));
    }
    #endregion Format Refresh Interval Display

    #region Convert TimeSpan to HH:mm:ss string
    /// <summary>
    /// Formats a TimeSpan as HH:mm:ss with hours not rolling over at 24.
    /// </summary>
    /// <param name="ts">The TimeSpan to format.</param>
    /// <returns>Formatted string with total hours.</returns>
    public static string ToHourString(TimeSpan ts)
    {
        // Ensure positive formatting for negative durations
        bool isNegative = ts.Ticks < 0;
        ts = ts.Duration();

        return string.Format(CultureInfo.CurrentCulture,
            "{0}{1:D2}:{2:D2}:{3:D2}",
            isNegative ? "-" : "",
            (int)ts.TotalHours,
            ts.Minutes,
            ts.Seconds
        );
    }
    #endregion Convert TimeSpan to HH:mm:ss string

    #region Show custom message box
    /// <summary>
    /// Shows the custom message box with the specified message, caption, and error status.
    /// The caption defaults to "Get My IP" and the error status defaults to false.
    /// </summary>
    private static Task<bool> ShowMsgBox(string msg, string caption = "Get My IP", bool isError = false)
    {
        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;
        Dispatcher? dispatcher = Application.Current.Dispatcher;

        if (dispatcher is null)
        {
            return Task.FromResult(false);
        }
        if (dispatcher.CheckAccess())
        {
            _ = new MDCustMsgBox(msg,
            caption,
            ButtonType.Ok,
            false,
            true,
            mainWindow,
            isError).ShowDialog();
        }
        else
        {
            _ = dispatcher.InvokeAsync(() =>
            {
                _ = new MDCustMsgBox(msg,
                caption,
                ButtonType.Ok,
                false,
                true,
                mainWindow,
                isError).ShowDialog();
            });
        }
        return Task.FromResult(true);
    }
    #endregion Show custom message box

    #region Relay Commands
    [RelayCommand]
    private static void ViewPermLog()
    {
        if (!string.IsNullOrEmpty(UserSettings.Setting.LogFile) && File.Exists(UserSettings.Setting.LogFile))
        {
            TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
        }
        else
        {
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_FileNotFound"),
                     GetStringResource("MsgText_Error_Caption"),
                     ButtonType.Ok,
                     false,
                     true,
                     _mainWindow,
                     true).ShowDialog();
        }
    }

    [RelayCommand]
    private static async Task TestLogging()
    {
        if (!string.IsNullOrEmpty(UserSettings.Setting.LogFile))
        {
            string json = await IpHelpers.GetExternalInfo();
            IpHelpers.LogIPInfo(json);
            _ = Task.Delay(200);
            TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
        }
        else
        {
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_FileNameMissing"),
                                 GetStringResource("MsgText_Error_Caption"),
                                 ButtonType.Ok,
                                 false,
                                 true,
                                 _mainWindow,
                                 true).ShowDialog();
        }
    }

    [RelayCommand]
    private static void RefreshTooltip()
    {
        CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(false);
        SnackBarMsg.ClearAndQueueMessage(GetStringResource("MsgText_TooltipRefreshed"));
    }

    [RelayCommand]
    private static void OpenAppFolder()
    {
        string filePath = string.Empty;
        try
        {
            filePath = Path.Combine(AppInfo.AppDirectory, "Strings.test.xaml");
            if (File.Exists(filePath))
            {
                string explorerPath = PathHelpers.FindOnPath("explorer.exe");
                if (string.IsNullOrEmpty(explorerPath))
                {
                    _log.Error($"Error trying to open {filePath}: FindOnPath returned null or empty");
                    string msg = $"{GetStringResource("MsgText_Error_FileExplorer")}" +
                                 $"\n\n{GetStringResource("MsgText_Error_SeeLog")}";
                    _ = new MDCustMsgBox(msg,
                             GetStringResource("MsgText_Error_Caption"),
                             ButtonType.Ok,
                             false,
                             true,
                             _mainWindow,
                             true).ShowDialog();
                    return;
                }
                Process.Start(explorerPath, $"/select,\"{filePath}\"");
            }
            else
            {
                using Process p = new();
                p.StartInfo.FileName = AppInfo.AppDirectory;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error trying to open {filePath}: {ex.Message}");
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_FileExplorer"),
                     GetStringResource("MsgText_Error_Caption"),
                     ButtonType.Ok,
                     false,
                     true,
                     _mainWindow,
                     true).ShowDialog();
        }
    }

    [RelayCommand]
    private static void StartWithWindows(RoutedEventArgs e)
    {
        const string _getmyip = "GetMyIP";

        CheckBox? cbx = e.Source as CheckBox;
        if (cbx!.IsChecked == true)
        {
            if (!RegRun.RegRunEntry(_getmyip))
            {
                string result = RegRun.AddRegEntry(_getmyip, AppInfo.AppPath);
                if (result == "OK")
                {
                    _log.Info(@"Get My IP added to HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    MDCustMsgBox mbox = new(GetStringResource("MsgText_WindowsStartupAdded"),
                                        "Get My IP",
                                        ButtonType.Ok,
                                        true,
                                        true,
                                        _mainWindow);
                    _ = mbox.ShowDialog();
                }
                else
                {
                    _log.Error($"Get My IP add to startup failed: {result}");
                    string msg = $"{GetStringResource("MsgText_Error_WindowsStartupLine1")}" +
                                 $"\n\n{GetStringResource("MsgText_Error_WindowsStartupLine2")}";
                    MDCustMsgBox mbox = new(msg,
                                        GetStringResource("MsgText_Error_Caption"),
                                        ButtonType.Ok,
                                        true,
                                        true,
                                        _mainWindow,
                                        true);
                    _ = mbox.ShowDialog();
                }
            }
        }
        else if (RegRun.RegRunEntry(_getmyip))
        {
            string result = RegRun.RemoveRegEntry(_getmyip);
            if (result == "OK")
            {
                _log.Info(@"Get My IP removed from HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                MDCustMsgBox mbox = new(GetStringResource("MsgText_WindowsStartupRemoved"),
                    "Get My IP",
                    ButtonType.Ok,
                    true,
                    true,
                    _mainWindow);
                _ = mbox.ShowDialog();
            }
            else
            {
                _log.Error($"Get My IP add to startup failed: {result}");

                string msg = $"{GetStringResource("MsgText_Error_WindowsStartupLine1")}" +
                             $"\n\n{GetStringResource("MsgText_Error_WindowsStartupLine2")}";
                MDCustMsgBox mbox = new(msg,
                                    GetStringResource("MsgText_Error_Caption"),
                                    ButtonType.Ok,
                                    true,
                                    true,
                                    _mainWindow,
                                    true);
                _ = mbox.ShowDialog();
            }
        }
    }

    [RelayCommand]
    public static void UpdateRefresh()
    {
        if (UserSettings.Setting.AutoRefresh)
        {
            RefreshHelpers.StopTimer();
            Task.Delay(50).Wait();
            RefreshHelpers.StartTimer();
            SnackBarMsg.QueueMessageNoClear(GetStringResource("MsgText_Refreshed"), 1500);
        }
        else
        {
            RefreshHelpers.StopTimer();
        }
    }

    [RelayCommand]
    private static void CompareLanguages()
    {
        CompareLanguageDictionaries();
        NavigationViewModel.ViewLog();
    }

    #region Open settings
    [RelayCommand]
    private static void OpenSettings()
    {
        ConfigHelpers.SaveSettings();
        TextFileViewer.ViewTextFile(ConfigHelpers.SettingsFileName!);
    }
    #endregion Open settings

    #region Export settings
    [RelayCommand]
    private static void ExportSettings()
    {
        ConfigHelpers.ExportSettings();
    }
    #endregion Export settings

    #region Import settings
    [RelayCommand]
    private static void ImportSettings()
    {
        ConfigHelpers.ImportSettings();
    }
    #endregion Import settings

    #region List (dump) settings to log file
    [RelayCommand]
    public static void DumpSettings()
    {
        ConfigHelpers.DumpSettings();
        NavigationViewModel.ViewLog();
    }
    #endregion List (dump) settings to log file

    [RelayCommand]
    private void UpdateRefreshInterval()
    {
        HandleRefreshIntervalChanged();
    }
    #endregion Relay Commands
}
