// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;
/// <summary>
/// Class for finding IP address related information.
/// </summary>
internal static class IpHelpers
{
    #region NLog Permanent log
    private static readonly Logger _logPerm = LogManager.GetLogger("logPerm");
    #endregion NLog Permanent log

    #region Private fields
    private static IpApiCom? _infoIpApi;
    private static SeeIP? _seeIp;
    private static FreeIpApi? _infoFreeIpApi;
    private static IP2Location? _infoIP2Location;
    private static int _retryCount;
    // Reuse the HttpClient instance across requests.
    private static readonly HttpClient _httpClient = new();
    #endregion Private fields

    #region Enum for error source
    /// <summary>
    /// Enums to define where error originated. Used by <see cref="ShowErrorMessage"/>.
    /// </summary>
    private enum ErrorSource
    {
        internalIP = 1,
        externalIP = 2
    }
    #endregion Enum for error source

    #region Get only external info
    /// <summary>
    /// Get external (only) IP info <see langword="async"/>
    /// </summary>
    /// <returns>External IP info as string.</returns>
    public static async Task<string> GetExternalAsync()
    {
        Task<string> extInfo = GetExternalInfo();
        return await extInfo;
    }
    #endregion Get only external info

    #region Get Internal & External info
    /// <summary>
    /// Get internal and external IP info <see langword="async"/>
    /// </summary>
    /// <returns>External IP info as string.</returns>
    public static async Task<string> GetAllInfoAsync()
    {
        Task intInfo = GetMyInternalIPAsync();
        Task<string> extInfo = GetExternalInfo();

        await Task.WhenAll(intInfo, extInfo);
        return await extInfo;
    }
    #endregion Get Internal & External info

    #region Get Internal IP
    /// <summary>
    /// Gets internal ip addresses asynchronously.
    /// </summary>
    public static async Task GetMyInternalIPAsync()
    {
        _log.Debug("Starting discovery of internal IP information.");
        IPInfo.InternalList.Clear();
        if (!ConnectivityHelpers.IsConnectedToNetwork())
        {
            _log.Error("A network connection was not found.");
            ShowErrorMessage(GetStringResource("MsgText_Error_NetworkNotFound"), ErrorSource.internalIP, true);
            return;
        }

        Stopwatch sw = Stopwatch.StartNew();
        string host = Dns.GetHostName();
        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(host);

        // Get info for each IPv4 host
        foreach (IPAddress address in hostEntry.AddressList)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                IPInfo.InternalList.Add(new IPInfo(GetStringResource("Internal_IPv4Address"), address.ToString()));
                _log.Debug($"Internal IPv4 Address is {(UserSettings.Setting!.ObfuscateLog ?
                                                              ObfuscateString(address.ToString()) :
                                                              address)}");
            }
        }
        // and optionally for IPv6 host
        if (UserSettings.Setting!.IncludeV6)
        {
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    IPInfo.InternalList.Add(new IPInfo(GetStringResource("Internal_IPv6Address"), address.ToString()));
                    _log.Debug($"Internal IPv6 Address is {(UserSettings.Setting.ObfuscateLog ?
                                                                  ObfuscateString(address.ToString()) :
                                                                  address)}");
                }
            }
        }
        sw.Stop();
        _log.Debug($"Discovering internal addresses took {sw.Elapsed.TotalMilliseconds:N2} ms");
    }
    #endregion Get Internal IP

    #region Get External IP & Geolocation info
    /// <summary>
    /// Retrieves external IP and geolocation information using the configured public information provider.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a JSON string with the external IP information.
    /// </returns>
    public static async Task<string> GetExternalInfo()
    {
        Stopwatch sw = Stopwatch.StartNew();
        string url;
        bool canMap;

        switch (UserSettings.Setting!.InfoProvider)
        {
            case PublicInfoProvider.SeeIP:
                url = AppConstString.SeeIpURL;
                canMap = true;
                break;
            case PublicInfoProvider.FreeIpApi:
                url = AppConstString.FreeIpApiUrl;
                canMap = true;
                break;
            case PublicInfoProvider.IP2Location:
                url = AppConstString.IP2LocationURL;
                canMap = true;
                break;
            default:
                // ip-api.com is the default
                url = AppConstString.IpApiUrl;
                canMap = true;
                break;
        }
        string someJson = await GetIPInfoAsync(url);
        Map.Instance.CanMap = canMap;
        sw.Stop();
        _log.Debug($"Discovering external IP information took {sw.Elapsed.TotalMilliseconds:N2} ms");
        return someJson;
    }

    /// <summary>
    /// Retrieves IP information from the specified URL.
    /// </summary>
    /// <param name="url">The URL to retrieve IP information from.</param>
    /// <returns>IP information as a string.</returns>
    private static async Task<string> GetIPInfoAsync(string url)
    {
        // Ensure retry value is between 1 and 100.
        int maxRetries = Math.Min(Math.Max(UserSettings.Setting!.RetryMax, 1), 100);

        // Limit delay to between 10 and 3600 seconds.
        int delay = Math.Min(Math.Max(UserSettings.Setting.RetrySeconds, 10), 3600);

        //url = "https://api.jsoning.com/mock/4jydqtore0/status/401";
        //url = "https://bing.com";

        while (_retryCount < maxRetries)
        {
            _log.Debug("Starting discovery of external IP information.");
            try
            {
                Uri uri = new(url);
                string baseUri = uri.GetLeftPart(UriPartial.Authority);
                _log.Debug($"Connecting to: {baseUri}");
                using HttpResponseMessage response = await _httpClient.GetAsync(uri);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK: // 200
                        {
                            ResetRetryCount();
                            string returnedText = await response.Content.ReadAsStringAsync();
                            _log.Debug($"Received status code: {(int)response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                            TrayIconHelpers.ShowProblemIcon = false;
                            return CheckJson(url, returnedText) ? returnedText : string.Empty;
                        }
                    case HttpStatusCode.TooManyRequests: // 429
                        _log.Error($"Received status code: {(int)response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                        _log.Error("For help with status codes see https://en.wikipedia.org/wiki/List_of_HTTP_status_codes");
                        ShowErrorMessage(GetStringResource("MsgText_Error_TooManyRequests"), ErrorSource.externalIP, true);
                        ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), ErrorSource.externalIP, false);
                        ShowLastRefresh();
                        TrayIconHelpers.ShowProblemIcon = true;
                        return string.Empty;
                    default:
                        {
                            _log.Error($"Received status code: {(int)response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                            _log.Error("For help with status codes see https://en.wikipedia.org/wiki/List_of_HTTP_status_codes");
                            Task<string> returnedText = response.Content.ReadAsStringAsync();
                            if (returnedText.Exception != null)
                            {
                                _log.Error(returnedText.Exception);
                            }
                            string status = $"{(int)response.StatusCode} - {response.ReasonPhrase}";
                            string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorConnecting, $" ({status})");
                            ShowErrorMessage(msg, ErrorSource.externalIP, true);
                            ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), ErrorSource.externalIP, false);
                            ShowLastRefresh();
                            TrayIconHelpers.ShowProblemIcon = true;
                            return string.Empty;
                        }
                }
            }
            catch (HttpRequestException hx)
            {
                _log.Error($"{hx.Message}");
                TrayIconHelpers.ShowProblemIcon = true;
                TrayIconHelpers.SetTrayIcon();
                if (_retryCount < maxRetries)
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorConnecting, hx.Message);
                    ShowErrorMessage(msg, ErrorSource.externalIP, true);
                    if (hx.StatusCode is not null)
                    {
                        _log.Warn($"Received status code {hx.StatusCode} from {url}");
                    }
                    await DelayAndRetry(delay, maxRetries);
                }
                else
                {
                    // Shouldn't ever reach here. 
                    string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorConnecting, hx.Message);
                    _log.Error(msg);
                    ShowErrorMessage(msg, ErrorSource.externalIP, true);
                    ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), ErrorSource.externalIP, false);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _log.Error(CultureInfo.InvariantCulture, ex.Message, "Error retrieving data");
                TrayIconHelpers.ShowProblemIcon = true;
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorConnecting, ex.Message);
                ShowErrorMessage(msg, ErrorSource.externalIP, true);
                ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), ErrorSource.externalIP, false);
                return string.Empty;
            }
        }
        return string.Empty;
    }
    #endregion Get External IP & Geolocation info

    #region Reset the retry counter to zero
    /// <summary>
    /// Resets the retry count to zero if it has been incremented.
    /// </summary>
    /// <remarks>
    /// This method is typically called after a successful HTTP request to ensure that
    /// the retry logic starts from zero for subsequent requests.
    /// </remarks>
    public static void ResetRetryCount()
    {
        if (_retryCount > 0)
        {
            _retryCount = 0;
        }
    }
    #endregion Reset the retry counter to zero

    #region Delay for retry if needed
    /// <summary>
    /// Delays the process before the next retry.
    /// </summary>
    /// <param name="delay">Delay between retries in seconds.</param>
    /// <param name="maxRetries">Maximum number of retries.</param>
    /// <returns>Task (which is not used).</returns>
    private static async Task DelayAndRetry(int delay, int maxRetries)
    {
        _retryCount++;
        if (_retryCount < maxRetries)
        {
            _log.Warn($"Retrying in {delay} seconds {_retryCount}/{maxRetries}");
            string msg = string.Format(CultureInfo.InvariantCulture, MsgTextRetryAttempt, delay, _retryCount, maxRetries);
            ShowErrorMessage(msg, ErrorSource.externalIP, false);
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }
        else
        {
            _log.Error($"Max retry count ({_retryCount}) reached.");
            string msg = string.Format(CultureInfo.InvariantCulture, MsgTextMaxRetriesReached, _retryCount);
            ShowErrorMessage(msg, ErrorSource.externalIP, false);
        }
    }
    #endregion Delay for retry if needed

    #region Process Json based on which provider was used
    /// <summary>
    /// Process Json based on which provider was used
    /// </summary>
    /// <param name="returnedJson">Json file to process</param>
    /// <param name="quiet">If true limit what is written to the log</param>
    public static void ProcessProvider(string returnedJson, bool quiet)
    {
        if (!string.IsNullOrEmpty(returnedJson))
        {
            switch (UserSettings.Setting!.InfoProvider)
            {
                case PublicInfoProvider.IpApiCom:
                    ProcessIPApiCom(returnedJson, quiet);
                    break;
                case PublicInfoProvider.SeeIP:
                    ProcessSeeIp(returnedJson, quiet);
                    break;
                case PublicInfoProvider.FreeIpApi:
                    ProcessFreeIpApi(returnedJson, quiet);
                    break;
                case PublicInfoProvider.IP2Location:
                    ProcessIp2Location(returnedJson, quiet);
                    break;
                default:
                    throw new InvalidOperationException("Invalid Provider");
            }
            ShowLastRefresh();
        }
        else
        {
            _log.Error("There was a problem either connecting to the information provider or with processing the returned data.");
        }
    }
    #endregion Process Json based on which provider was used

    #region Deserialize JSON from ip-api.com
    /// <summary>
    /// Deserialize the JSON containing the ip information.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="quiet">If true limit what is written to the log</param>
    private static void ProcessIPApiCom(string? json, bool quiet)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                ClearGeoInfoList();
                if (json != null)
                {
                    _infoIpApi = JsonSerializer.Deserialize<IpApiCom>(json, JsonHelpers.JsonOptions);

                    if (string.Equals(_infoIpApi!.Status, "success", StringComparison.OrdinalIgnoreCase))
                    {
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), _infoIpApi.IpAddress));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), _infoIpApi.City));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), _infoIpApi.State));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), _infoIpApi.Zip));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), _infoIpApi.Country));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), _infoIpApi.CountryCode));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Continent"), _infoIpApi.Continent));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), _infoIpApi.Lon.ToString(CultureInfo.InvariantCulture)));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), _infoIpApi.Lat.ToString(CultureInfo.InvariantCulture)));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_TimeZone"), _infoIpApi.TimeZone));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), ConvertOffset(_infoIpApi.Offset)));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Provider"), _infoIpApi.Isp));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), _infoIpApi.AS));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASName"), _infoIpApi.ASName));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("SettingsItem_PublicInfoProvider"),
                                                                GetStringResource("SettingsEnum_Provider_IpApiCom")));
                        if (RefreshInfo.Instance.LastIPAddress?.Length == 0)
                        {
                            RefreshInfo.Instance.LastIPAddress = _infoIpApi.IpAddress;
                        }
                    }
                    else
                    {
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Status"), _infoIpApi.Status!));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Message"), _infoIpApi.Message!));
                    }

                    if (!quiet)
                    {
                        foreach (IPInfo item in IPInfo.GeoInfoList)
                        {
                            _log.Debug($"{item.Parameter} is {(UserSettings.Setting!.ObfuscateLog ?
                                                                    ObfuscateString(item.Value!) :
                                                                    item.Value)}");
                        }
                    }
                    else
                    {
                        _log.Debug($"External IP address is {(UserSettings.Setting!.ObfuscateLog ?
                                                                    ObfuscateString(_infoIpApi.IpAddress) :
                                                                    _infoIpApi.IpAddress)}");
                    }
                }
                else
                {
                    _log.Error("JSON was null. Check for previous error messages.");
                    ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull2"), ErrorSource.externalIP, true);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(JsonHelpers.TruncateJson(json!, 500));
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorJsonParsing, ex.Message);
                ShowErrorMessage(msg, ErrorSource.externalIP, true);
            }
        });
    }
    #endregion Deserialize JSON from ip-api.com

    #region Deserialize JSON from seeip.org
    /// <summary>
    /// Deserialize the JSON containing the ip information.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="quiet">If true limit what is written to the log</param>
    private static void ProcessSeeIp(string? json, bool quiet)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                ClearGeoInfoList();
                if (json != null)
                {
                    _seeIp = JsonSerializer.Deserialize<SeeIP>(json, JsonHelpers.JsonOptions);

                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), _seeIp!.IpAddress));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), _seeIp!.City));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), _seeIp!.Region));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), _seeIp!.Region_Code));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), _seeIp!.Country));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), _seeIp!.Country_Code));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), _seeIp!.Postal_Code));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ContinentCode"), _seeIp!.Continent_Code));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), _seeIp.Longitude.ToString(CultureInfo.InvariantCulture)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), _seeIp.Latitude.ToString(CultureInfo.InvariantCulture)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_TimeZone"), _seeIp.TimeZone));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), ConvertOffset(_seeIp.Offset)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), _seeIp.ASN.ToString(CultureInfo.InvariantCulture)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Organization"), _seeIp.Organization));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("SettingsItem_PublicInfoProvider"),
                                                            GetStringResource("SettingsEnum_Provider_SeeIp")));

                    if (RefreshInfo.Instance.LastIPAddress?.Length == 0)
                    {
                        RefreshInfo.Instance.LastIPAddress = _seeIp.IpAddress;
                    }

                    if (!quiet)
                    {
                        foreach (IPInfo item in IPInfo.GeoInfoList)
                        {
                            _log.Debug($"{item.Parameter} is {(UserSettings.Setting!.ObfuscateLog ?
                                                                    ObfuscateString(item.Value!) :
                                                                    item.Value)}");
                        }
                    }
                    else
                    {
                        _log.Debug($"External IP address is {(UserSettings.Setting!.ObfuscateLog ?
                                                                    ObfuscateString(_seeIp.IpAddress) :
                                                                    _seeIp.IpAddress)}");
                    }
                }
                else
                {
                    _log.Error("JSON was null. Check for previous error messages.");
                    ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull2"), ErrorSource.externalIP, true);
                }
            }
            catch (JsonException ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorJsonParsing2, ex.Message);
                ShowErrorMessage(msg, ErrorSource.externalIP, true);
            }
            catch (ArgumentNullException ex)
            {
                _log.Error(ex, "Error parsing JSON. JSON was null.");
                ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull2"), ErrorSource.externalIP, true);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorJsonParsing2, ex.Message);
                ShowErrorMessage(msg, ErrorSource.externalIP, true);
            }
        });
    }
    #endregion Deserialize JSON from seeip.org

    #region Deserialize JSON from freeipapi.com
    /// <summary>
    /// Deserialize the JSON containing the ip information.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="quiet">If true limit what is written to the log</param>
    private static void ProcessFreeIpApi(string? json, bool quiet)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                ClearGeoInfoList();
                if (json != null)
                {
                    _infoFreeIpApi = JsonSerializer.Deserialize<FreeIpApi>(json, JsonHelpers.JsonOptions);

                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), _infoFreeIpApi!.IpAddress));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), _infoFreeIpApi.CityName));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), _infoFreeIpApi.RegionName));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), _infoFreeIpApi.PostalCode));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), _infoFreeIpApi.CountryName));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), _infoFreeIpApi.CountryCode));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Continent"), _infoFreeIpApi.Continent));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), _infoFreeIpApi.Longitude.ToString(CultureInfo.InvariantCulture)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), _infoFreeIpApi.Latitude.ToString(CultureInfo.InvariantCulture)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpType"), $"IPv{_infoFreeIpApi.IpVersion}"));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("SettingsItem_PublicInfoProvider"),
                                                              GetStringResource("SettingsEnum_Provider_FreeIpApi")));

                    if (RefreshInfo.Instance.LastIPAddress?.Length == 0)
                    {
                        RefreshInfo.Instance.LastIPAddress = _infoFreeIpApi.IpAddress;
                    }

                    if (!quiet)
                    {
                        foreach (IPInfo item in IPInfo.GeoInfoList)
                        {
                            _log.Debug($"{item.Parameter} is {(UserSettings.Setting!.ObfuscateLog ?
                                                                   ObfuscateString(item.Value!) :
                                                                   item.Value)}");
                        }
                    }
                    else
                    {
                        _log.Debug($"External IP address is {(UserSettings.Setting!.ObfuscateLog ?
                                                                    ObfuscateString(_infoFreeIpApi.IpAddress) :
                                                                    _infoFreeIpApi.IpAddress)}");
                    }
                }
                else
                {
                    _log.Error("JSON was null. Check for previous error messages.");
                    ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull2"), ErrorSource.externalIP, true);
                }
            }
            catch (JsonException ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorJsonParsing2, ex.Message);
                ShowErrorMessage(msg, ErrorSource.externalIP, true);
            }
            catch (ArgumentNullException ex)
            {
                _log.Error(ex, "Error parsing JSON. JSON was null.");
                ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull2"), ErrorSource.externalIP, true);
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorJsonParsing2, ex.Message);
                ShowErrorMessage(msg, ErrorSource.externalIP, true);
            }
        });
    }
    #endregion Deserialize JSON from freeipapi.com

    #region Deserialize JSON from ip2loacation.
    /// <summary>
    /// Deserialize the JSON containing the ip information.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <param name="quiet">If true limit what is written to the log</param>
    private static void ProcessIp2Location(string? json, bool quiet)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                ClearGeoInfoList();
                if (json != null)
                {
                    _infoIP2Location = JsonSerializer.Deserialize<IP2Location>(json, JsonHelpers.JsonOptions);

                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), _infoIP2Location!.IpAddress));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), _infoIP2Location.City_Name));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), _infoIP2Location.Region_Name));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), _infoIP2Location.Country_Name));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), _infoIP2Location.Country_Code));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), _infoIP2Location.Zip_Code));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), _infoIP2Location.Latitude.ToString(CultureInfo.InvariantCulture)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), _infoIP2Location.Longitude.ToString(CultureInfo.InvariantCulture)));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), _infoIP2Location.Time_Zone));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASName"), _infoIP2Location.AS));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), _infoIP2Location.ASN));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IsProxy"), _infoIP2Location.Is_Proxy.ToYesNoString()));
                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("SettingsItem_PublicInfoProvider"),
                                          GetStringResource("SettingsEnum_Provider_Ip2Location")));

                    if (RefreshInfo.Instance.LastIPAddress?.Length == 0)
                    {
                        RefreshInfo.Instance.LastIPAddress = _infoIP2Location.IpAddress;
                    }
                    if (!quiet)
                    {
                        foreach (IPInfo item in IPInfo.GeoInfoList)
                        {
                            _log.Debug($"{item.Parameter} is {(UserSettings.Setting!.ObfuscateLog ?
                                                                     ObfuscateString(item.Value!) :
                                                                     item.Value)}");
                        }
                    }
                    else
                    {
                        _log.Debug($"External IP address is {(UserSettings.Setting!.ObfuscateLog ?
                                                                    ObfuscateString(_infoIP2Location.IpAddress) :
                                                                    _infoIP2Location.IpAddress)}");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorJsonParsing2, ex.Message);
                ShowErrorMessage(msg, ErrorSource.externalIP, true);
            }
        });
    }
    #endregion Deserialize JSON from ip2loacation.

    #region Log IP info
    /// <summary>
    /// Writes the external IP information to the log file.
    /// </summary>
    /// <param name="json">JSON string containing public IP info</param>
    public static void LogIPInfo(string json)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                JsonSerializerOptions opts = new()
                {
                    PropertyNameCaseInsensitive = true
                };

                switch (UserSettings.Setting!.InfoProvider)
                {
                    case PublicInfoProvider.IpApiCom:
                        _infoIpApi = JsonSerializer.Deserialize<IpApiCom>(json, opts);
                        LogIpApiComInfo(_infoIpApi);
                        break;
                    case PublicInfoProvider.SeeIP:
                        _seeIp = JsonSerializer.Deserialize<SeeIP>(json, opts);
                        LogSeeIpInfo(_seeIp);
                        break;
                    case PublicInfoProvider.FreeIpApi:
                        _infoFreeIpApi = JsonSerializer.Deserialize<FreeIpApi>(json, opts);
                        LogFreeIpApiInfo(_infoFreeIpApi);
                        break;
                    case PublicInfoProvider.IP2Location:
                        _infoIP2Location = JsonSerializer.Deserialize<IP2Location>(json, opts);
                        LogIP2LocationInfo(_infoIP2Location);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid InfoProvider");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error while attempting to log IP information");
                _logPerm.Error(ex, GetStringResource("MsgText_Error_Logging"));
            }
        });
    }

    private static void LogIpApiComInfo(IpApiCom? info)
    {
        if (info == null)
            return;

        if (string.Equals(info.Status, "success", StringComparison.OrdinalIgnoreCase))
        {
            StringBuilder sb = new();
            sb.Append(' ').AppendFormat(CultureInfo.InvariantCulture, "{0,-16}", info.IpAddress)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-10}", info.City)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-12}", info.State)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-5}", info.Zip)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", info.Lat)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", info.Lon)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-25}", info.Isp)
              .Append("  ").AppendLine(info.AS);
            _logPerm.Info(sb.ToString().TrimEnd('\n', '\r'));
        }
        else
        {
            _log.Error(info.Message);
            _logPerm.Error($" {info.Status,-16}  {info.Message}");
        }
    }

    private static void LogSeeIpInfo(SeeIP? info)
    {
        if (info == null)
            return;

        StringBuilder sb = new();
        sb.Append(' ').AppendFormat(CultureInfo.InvariantCulture, "{0,-16}", info.IpAddress)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-10}", info.City)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-12}", info.Region)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-5}", info.Postal_Code)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", Math.Round(info.Latitude, 4))
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", Math.Round(info.Longitude, 4))
          .Append("  ").AppendLine(info.Organization);
        _logPerm.Info(sb.ToString().TrimEnd('\n', '\r'));
    }

    private static void LogFreeIpApiInfo(FreeIpApi? info)
    {
        if (info == null)
            return;

        StringBuilder sb = new();
        sb.Append(' ').AppendFormat(CultureInfo.InvariantCulture, "{0,-16}", info.IpAddress)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-10}", info.CityName)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-12}", info.RegionName)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-5}", info.PostalCode)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", Math.Round(info.Latitude, 4))
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", Math.Round(info.Longitude, 4));
        _logPerm.Info(sb.ToString().TrimEnd('\n', '\r'));
    }

    private static void LogIP2LocationInfo(IP2Location? info)
    {
        if (info == null)
            return;

        StringBuilder sb = new();
        sb.Append(' ').AppendFormat(CultureInfo.InvariantCulture, "{0,-16}", info.IpAddress)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-10}", info.City_Name)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-12}", info.Region_Name)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-5}", info.Zip_Code)
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", Math.Round(info.Latitude, 4))
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", Math.Round(info.Longitude, 4))
          .Append("  ").AppendLine(info.AS);
        _logPerm.Info(sb.ToString().TrimEnd('\n', '\r'));
    }
    #endregion Log IP info

    #region Convert offset from seconds to hours and minutes
    /// <summary>
    /// Converts the offset value in the JSON to a more readable format.
    /// </summary>
    /// <param name="offset">The offset from UTC.</param>
    /// <returns>Offset from UTC as a string. </returns>
    private static string ConvertOffset(int offset)
    {
        string sign = "+";
        if (offset < 0)
        {
            offset = Math.Abs(offset);
            sign = "-";
        }
        TimeSpan ts = TimeSpan.FromSeconds(offset);
        string time = ts.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
        return sign + time;
    }

    #endregion Convert offset from seconds to hours and minutes

    #region Obfuscate IP info
    /// <summary>
    /// Obfuscate a string by replacing letters with X, and numbers with #.
    /// Special characters are left unaltered.
    /// </summary>
    /// <param name="unalteredString">String to obfuscate.</param>
    /// <returns>Obfuscated string. If the string is null or empty, <c>string.Empty</c> will be returned.</returns>
    public static string ObfuscateString(string unalteredString)
    {
        if (string.IsNullOrEmpty(unalteredString))
        {
            return string.Empty;
        }

        StringBuilder obfuscatedString = new();
        foreach (char c in unalteredString)
        {
            if (char.IsDigit(c))
            {
                obfuscatedString.Append('#');
            }
            else if (char.IsLetter(c))
            {
                obfuscatedString.Append('X');
            }
            else
            {
                obfuscatedString.Append(c);
            }
        }
        return obfuscatedString.ToString();
    }
    #endregion Obfuscate IP info

    #region Show MessageBox with error message
    /// <summary>
    /// Shows an error message in a MessageBox.
    /// </summary>
    /// <param name="errorMsg">The error message.</param>
    /// <param name="source">Source of the error (internal or external).</param>
    /// <param name="clear">Clear the list before adding the message.</param>
    private static void ShowErrorMessage(string errorMsg, ErrorSource source, bool clear)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            string message = GetStringResource("External_Message");
            switch (source)
            {
                case ErrorSource.externalIP:
                    if (clear)
                    {
                        IPInfo.GeoInfoList.Clear();
                    }
                    IPInfo.GeoInfoList.Add(new IPInfo(message, errorMsg));
                    break;
                case ErrorSource.internalIP:
                    if (clear)
                    {
                        IPInfo.InternalList.Clear();
                    }
                    IPInfo.InternalList.Add(new IPInfo(message, errorMsg));
                    break;
                default:
                    if (clear)
                    {
                        IPInfo.InternalList.Clear();
                        IPInfo.GeoInfoList.Clear();
                    }
                    IPInfo.InternalList.Add(new IPInfo(message, errorMsg));
                    IPInfo.GeoInfoList.Add(new IPInfo(message, errorMsg));
                    break;
            }
        });
    }
    #endregion Show MessageBox with error message

    #region Clear external geolocation info
    /// <summary>
    /// Clears the ObservableCollection used to hold the external geolocation info.
    /// </summary>
    private static void ClearGeoInfoList()
    {
        if (IPInfo.GeoInfoList.Count > 0)
        {
            IPInfo.GeoInfoList.Clear();
        }
    }
    #endregion Clear external geolocation info

    #region Check the returned JSON
    /// <summary>
    /// Checks if the returned JSON is valid.
    /// </summary>
    /// <param name="url">The URL from which the JSON was retrieved.</param>
    /// <param name="returnedText">The JSON text to validate.</param>
    /// <returns><see langword="true"/> if the JSON is valid, otherwise <see langword="false"/>.</returns>
    private static bool CheckJson(string url, string returnedText)
    {
        if (!JsonHelpers.IsValid(returnedText))
        {
            _log.Error(GetStringResource("MsgText_Error_JsonParsing2"));
            _log.Error($"The url is: {url}");
            _log.Error(JsonHelpers.TruncateJson(returnedText, 2500));
            ShowErrorMessage(GetStringResource("MsgText_Error_JsonParsing2"), ErrorSource.externalIP, true);
            ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), ErrorSource.externalIP, false);
            return false;
        }
        return true;
    }
    #endregion Check the returned JSON

    #region Show the last refresh time
    /// <summary>
    /// Adds the last refresh time to the GeoInfoList if the setting is enabled.
    /// </summary>
    private static void ShowLastRefresh()
    {
        if (UserSettings.Setting!.ShowLastRefresh)
        {
            Application.Current.Dispatcher.Invoke(static () =>
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_LastRefresh"),
                                       DateTime.Now.ToString(CultureInfo.CurrentCulture))));
        }
    }
    #endregion Show the last refresh time
}
