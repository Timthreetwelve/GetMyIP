// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class RefreshHelpers
{
    #region Private fields
    private const int MinRefreshSeconds = 10;
    private const int MaxRefreshSeconds = 86399; // One second less than 24 hours
    private const int DefaultRefreshSeconds = 3600; // One hour
    private const int SnackBarDuration1500 = 1500;
    #endregion Private fields

    #region The refresh timer
    /// <summary>
    /// The timer used to trigger periodic refreshes.
    /// </summary>
    private static System.Timers.Timer? _refreshTimer;
    #endregion The refresh timer

    #region Start the refresh timer
    /// <summary>
    /// Starts the periodic refresh timer.
    /// </summary>
    public static void StartTimer()
    {
        int intervalSeconds = UserSettings.Setting!.AutoRefreshSeconds;
        intervalSeconds = VerifyRefreshInterval(intervalSeconds);
        TimeSpan interval = TimeSpan.FromSeconds(intervalSeconds);
        _refreshTimer ??= new System.Timers.Timer()
        {
            AutoReset = true,
            Interval = interval.TotalMilliseconds
        };
        if (!_refreshTimer.Enabled)
        {
            _refreshTimer.Elapsed += TimerElapsed;
            _refreshTimer.Start();

            RefreshInfo.Instance.LastRefresh = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            _log.Info($"Periodic refresh timer started. Refresh interval is {interval:hh\\:mm\\:ss}");
            SnackBarMsg.QueueMessageNoClear(GetStringResource("MsgText_PeriodicRefreshStarted"), SnackBarDuration1500);
        }
    }
    #endregion Start the refresh timer

    #region Stop the timer
    /// <summary>
    /// Stops the periodic refresh timer.
    /// </summary>
    public static void StopTimer()
    {
        if (_refreshTimer?.Enabled == true)
        {
            _refreshTimer.Stop();
            _refreshTimer.Elapsed -= TimerElapsed;
            _refreshTimer.Dispose();
            _refreshTimer = null;
            _log.Info("Periodic refresh timer stopped");
            SnackBarMsg.QueueMessageNoClear(GetStringResource("MsgText_PeriodicRefreshStopped"), SnackBarDuration1500);
        }
    }
    #endregion Stop the timer

    #region Timer elapsed
    /// <summary>
    /// Handles the Elapsed event of the refresh timer.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
    private static async void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            _log.Info("Periodic IP address refresh starting");
            await NavigationViewModel.RefreshExternalAsync();
            RefreshInfo.Instance.LastRefresh = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            CompareIP();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Refresh timer failed");
        }
    }
    #endregion Timer elapsed

    #region Compare IP address to previous
    /// <summary>
    /// Compares the current external IP address to the previous one and logs any changes.
    /// </summary>
    private static void CompareIP()
    {
        try
        {
            if (IPInfo.GeoInfoList.Count < 1)
            {
                return;
            }
            string currentIP = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_IpAddress"))?.Value!;

            if (string.IsNullOrEmpty(RefreshInfo.Instance.LastIPAddress))
            {
                if (!string.IsNullOrEmpty(currentIP))
                {
                    RefreshInfo.Instance.LastIPAddress = currentIP;
                }
            }
            else if (RefreshInfo.Instance.LastIPAddress != currentIP)
            {
                if (!UserSettings.Setting!.ObfuscateLog)
                {
                    _log.Info($"External IP address has changed. Was {RefreshInfo.Instance.LastIPAddress} is now {currentIP}");
                }
                else
                {
                    string lastAddress = IpHelpers.ObfuscateString(RefreshInfo.Instance.LastIPAddress);
                    string newAddress = IpHelpers.ObfuscateString(currentIP);
                    _log.Info($"External IP address has changed. Was {lastAddress} is now {newAddress}");
                }

                RefreshInfo.Instance.LastIPAddress = currentIP;
                if (string.IsNullOrEmpty(currentIP))
                {
                    currentIP = "?.?.?.?";
                }
                ToolTipHelper.BuildToolTip(true);
                TrayIconHelpers.SetTrayIcon();
                if (UserSettings.Setting.NotifyOnIpChange)
                {
                    ToastHelpers.ShowToast(GetStringResource("MsgText_IpChangedToastLine1"),
                                        $"{GetStringResource("MsgText_IpChangedToastLine2")} {currentIP}");
                }
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error in the CompareIP method.");
        }
    }
    #endregion Compare IP address to previous

    #region Handle refresh interval change
    /// <summary>
    /// Validates the combined Hours/Minutes/Seconds refresh interval. If the total is outside
    /// the allowed range, reverts to one hour and notifies the user; otherwise restarts the timer.
    /// </summary>
    public static void HandleRefreshIntervalChanged()
    {
        int refreshSeconds = (UserSettings.Setting!.RefreshHours * 3600) + (UserSettings.Setting.RefreshMinutes * 60) + UserSettings.Setting.RefreshSeconds;
        int autoRefreshSeconds = VerifyRefreshInterval(refreshSeconds);
        UserSettings.Setting.AutoRefreshSeconds = autoRefreshSeconds;
        SettingsViewModel.UpdateRefresh();
    }
    #endregion Handle refresh interval change

    #region Verify refresh interval
    /// <summary>
    /// Verifies that the refresh interval is within the allowed range. If not, it reverts to the default value and shows a message box.
    /// </summary>
    /// <param name="intervalSeconds">The refresh interval in seconds.</param>
    /// <returns>The verified refresh interval in seconds.</returns>
    private static int VerifyRefreshInterval(int intervalSeconds)
    {
        if (intervalSeconds is < MinRefreshSeconds or > MaxRefreshSeconds)
        {
            _log.Warn($"Invalid refresh interval ({intervalSeconds} seconds). Must be between {MinRefreshSeconds} sec and {MaxRefreshSeconds} sec. Reverting to one hour.");
            var defaultTime = TimeSpan.FromSeconds(DefaultRefreshSeconds);
            UserSettings.Setting!.RefreshHours = defaultTime.Hours;
            UserSettings.Setting.RefreshMinutes = defaultTime.Minutes;
            UserSettings.Setting.RefreshSeconds = defaultTime.Seconds;
            intervalSeconds = DefaultRefreshSeconds;
            string localizedMinimum = TimeSpan.FromSeconds(MinRefreshSeconds).ToString("g", CultureInfo.CurrentCulture);
            string localizedMaximum = TimeSpan.FromSeconds(MaxRefreshSeconds).ToString("g", CultureInfo.CurrentCulture);
            string msg = string.Format(CultureInfo.CurrentCulture, MsgTextErrorInvalidRefreshInterval, localizedMinimum, localizedMaximum);
            msg += $"\n\n{GetStringResource("MsgText_RefreshIntervalReverted")} ({TimeSpan.FromSeconds(DefaultRefreshSeconds):g})";

            _ = ShowMsgBox(msg,
                    GetStringResource("MsgText_Error_Caption"),
                    true);
        }
        return intervalSeconds;
    }
    #endregion Verify refresh interval

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
}
