// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class RefreshHelpers
{
    #region The timer
    private static System.Timers.Timer _refreshTimer;
    #endregion The timer

    #region Start the refresh timer
    public static void StartTimer()
    {
        int intervalMinutes = (int)UserSettings.Setting.AutoRefreshInterval;

        if (intervalMinutes < 1)
        {
            _log.Debug($"Invalid refresh timer interval - {intervalMinutes}");
            return;
        }
        TimeSpan interval = TimeSpan.FromMinutes(intervalMinutes);
        _refreshTimer = new System.Timers.Timer(interval.TotalMilliseconds)
        {
            AutoReset = true
        };
        if (!_refreshTimer.Enabled)
        {
            _refreshTimer.Elapsed += TimerElapsed;
            _refreshTimer.Start();
            _log.Debug($"Refresh timer started. Refresh interval is {intervalMinutes} minutes");
            RefreshInfo.Instance.LastRefresh = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
        }
        else
        {
            _log.Debug("Refresh timer is already running");
        }
    }
    #endregion Start the refresh timer

    #region Stop the timer
    public static void StopTimer()
    {
        _refreshTimer.Stop();
        _log.Debug("Refresh timer stopped");
    }
    #endregion Stop the timer

    #region Timer elapsed
    private static async void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        _log.Debug("Periodic IP address refresh starting");
        await NavigationViewModel.RefreshExternalAsync();
        RefreshInfo.Instance.LastRefresh = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
        CompareIP();
    }
    #endregion Timer elapsed

    #region Compare IP address to previous
    private static void CompareIP()
    {
        try
        {
            if (IPInfo.GeoInfoList.Count < 1)
            {
                return;
            }
            string currentIP = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_IpAddress"))?.Value;

            if (string.IsNullOrEmpty(RefreshInfo.Instance.LastIPAddress))
            {
                if (!string.IsNullOrEmpty(currentIP))
                {
                    RefreshInfo.Instance.LastIPAddress = currentIP;
                }
            }
            else if (RefreshInfo.Instance.LastIPAddress != currentIP)
            {
                _log.Info($"External IP address has changed. Was {RefreshInfo.Instance.LastIPAddress} is now {currentIP}");
                RefreshInfo.Instance.LastIPAddress = currentIP;
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

    #region Start/Stop refresh timer
    public static void StartRefresh()
    {
        if (UserSettings.Setting.AutoRefresh)
        {
            StartTimer();
        }
        else
        {
            StopTimer();
        }
    }
    #endregion Start/Stop refresh timer
}
