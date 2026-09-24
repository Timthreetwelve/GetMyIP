// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class RefreshHelpers
{
    #region Private fields
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
        int intervalSeconds = UserSettings.Setting.AutoRefreshSeconds;
        intervalSeconds = SettingsViewModel.VerifyRefreshInterval(intervalSeconds);
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
            string refreshInterval = SettingsViewModel.ToHourString(interval);
            _log.Info($"Periodic refresh timer started. Refresh interval is {refreshInterval}");
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
                if (!UserSettings.Setting.ObfuscateLog)
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
                    currentIP = GetStringResource("MsgText_ExternalUnknown");
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
}
