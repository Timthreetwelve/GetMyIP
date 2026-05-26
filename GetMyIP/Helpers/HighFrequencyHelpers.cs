// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class HighFrequencyHelpers
{
    #region Fields
    private static readonly HttpClient _httpClient = new();
    private static System.Timers.Timer? _HiFreqTimer;
    private static int _errorCount;
    #endregion Fields

    #region Start/Stop refresh timer
    /// <summary>
    /// Methods for starting and stopping the high-frequency timer
    /// </summary>
    public static void UpdateHighFrequencyRefresh()
    {
        if (UserSettings.Setting!.EnableHighFrequencyRefresh)
        {
            _ = StartTimer();
        }
        else
        {
            StopTimer();
        }
    }

    private static async Task StartTimer()
    {
        _HiFreqTimer ??= new System.Timers.Timer();
        if (!_HiFreqTimer.Enabled)
        {
            _HiFreqTimer.AutoReset = true;
            _HiFreqTimer.Enabled = true;
            _HiFreqTimer.Elapsed += HiFreqTimer_Elapsed;
            await JitterDelayAsync(_HiFreqTimer);
            _HiFreqTimer.Start();
            RefreshInfo.Instance.HighFrequencyLastRefresh = DateTime.Now.ToString("G", CultureInfo.CurrentCulture);
            _log.Debug("High-frequency refresh timer started.");
            SnackBarMsg.ClearAndQueueMessage("High-frequency refresh started.");
        }
    }

    public static void StopTimer()
    {
        if (_HiFreqTimer?.Enabled == true)
        {
            _HiFreqTimer.Stop();
            _HiFreqTimer.Elapsed -= HiFreqTimer_Elapsed;
            _HiFreqTimer.Dispose();
            _HiFreqTimer = null;
            _log.Debug("High-frequency refresh timer stopped.");
            SnackBarMsg.ClearAndQueueMessage("High-frequency refresh stopped.");
        }
    }
    #endregion Start/Stop refresh timer

    #region Timer elapsed
    /// <summary>
    /// When the timer elapses, this method is called to perform the high-frequency refresh. It checks the error count
    /// to prevent excessive errors, calls the method to get the external IP address, and updates the last refresh time.
    /// If there are too many errors, it stops the timer and shows an error message to the user.
    /// </summary>
    private static async void HiFreqTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            if (_errorCount >= 10)
            {
                _log.Error("Too many errors in high-frequency refresh. Stopping timer.");
                StopTimer();
                MessageHelpers.ShowErrorMessage("High-frequency refresh stopped due to repeated errors.", MessageHelpers.ErrorSource.externalIP, false);
                _errorCount = 0;
                return;
            }
            _ = await GetExternalIPHighFrequencyAsync();
            await JitterDelayAsync(_HiFreqTimer!);
            RefreshInfo.Instance.HighFrequencyLastRefresh = DateTime.Now.ToString("T", CultureInfo.CurrentCulture);
            CustomToolTip.Instance.ToolTipText = ToolTipHelper.BuildToolTip(true);
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"High-frequency refresh failed for reason: {ex.Message}");
        }
    }
    #endregion Timer elapsed

    #region Gets the external IP address for high-frequency refresh
    /// <summary>
    /// Gets the external Ip address for the high-frequency refresh.
    /// </summary>
    private static async Task<bool> GetExternalIPHighFrequencyAsync()
    {
        try
        {
            Uri uri = new(AppConstString.IpifyOrg);
            using HttpResponseMessage response = await _httpClient.GetAsync(uri);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _log.Error($"Failed to get IP address. Status code: {response.StatusCode}");
                return false;
            }

            string json = await response.Content.ReadAsStringAsync();
            JsonNodeOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };
            JsonNode? jsonNode = JsonNode.Parse(json, options);

            if (jsonNode != null)
            {
                string? highFreqIP = jsonNode["IP"]?.ToString();
                LogHiFreq(UserSettings.Setting!.ObfuscateLog
                    ? $"ipify.org response: {IpHelpers.ObfuscateString(highFreqIP!)}"
                    : $"ipify.org response: {highFreqIP}");
                RefreshInfo.Instance.HighFrequencyIPAddress = highFreqIP ?? string.Empty;
                if (RefreshInfo.Instance.HighFrequencyIPAddress != RefreshInfo.Instance.LastIPAddress)
                {
                    _log.Debug("High-frequency IP address has changed. Updating main IP information.");
                    RefreshHelpers.CompareIP();
                    _ = await IpHelpers.GetExternalAsync();
                }
                return true;
            }

            _log.Error("Failed to parse JSON response from ipify.org.");
            _log.Error("Response content: " + json);
            _errorCount++;
            return false;
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"High-frequency refresh failed for reason: {ex.Message}");
            _errorCount++;
            if (_errorCount >= 3)
            {
                _log.Error("Too many errors in high-frequency refresh. Stopping timer.");
                StopTimer();
                MessageHelpers.ShowErrorMessage("High-frequency refresh stopped due to repeated errors.", MessageHelpers.ErrorSource.externalIP, false);
                _errorCount = 0;
            }
            return false;
        }
    }
    #endregion Gets the external IP address for high-frequency refresh

    #region Log high-frequency messages
    /// <summary>
    /// Log a message at Debug level if high-frequency logging is enabled in user settings. Logging high-frequency
    /// refresh messages can generate a lot of log entries, so this method checks the user setting before logging to
    /// avoid unnecessary log clutter.
    /// </summary>
    private static void LogHiFreq(string logMessage)
    {
        if (UserSettings.Setting!.LogHighFrequency)
        {
            _log.Debug(logMessage);
        }
    }
    #endregion Log high-frequency messages

    #region Jitter delay
    /// <summary>
    /// Introduces a random delay between 4 and 10 seconds to prevent hitting the API too frequently.
    /// </summary>
    private static Task JitterDelayAsync(System.Timers.Timer timer)
    {
        timer.Interval = Random.Shared.Next(4, 11) * 1000;
        return Task.CompletedTask;
    }
    #endregion Jitter delay
}
