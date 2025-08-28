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
    private static IP2Location? _infoIp2Location;
    private static IpWho? _infoIpWho;
    private static int _retryCount;
    // Reuse the HttpClient instance across requests.
    private static readonly HttpClient _httpClient = new();
    #endregion Private fields

    #region Private properties
    /// <summary>
    /// Stores the most recently retrieved raw external JSON.
    /// </summary>
    private static string? LatestRawExternalJson { get; set; }
    #endregion Private properties

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
            MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_NetworkNotFound"), MessageHelpers.ErrorSource.internalIP, true);
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
            case PublicInfoProvider.IpWho:
                url = AppConstString.IpWhoURL;
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
                            LatestRawExternalJson = await response.Content.ReadAsStringAsync();
                            _log.Debug($"Received status code: {(int)response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                            TrayIconHelpers.ShowProblemIcon = false;
                            return CheckJson(url, LatestRawExternalJson) ? LatestRawExternalJson : string.Empty;
                        }
                    case HttpStatusCode.TooManyRequests: // 429
                        _log.Error($"Received status code: {(int)response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                        _log.Error("For help with status codes see https://en.wikipedia.org/wiki/List_of_HTTP_status_codes");
                        MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_TooManyRequests"), MessageHelpers.ErrorSource.externalIP, true);
                        MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), MessageHelpers.ErrorSource.externalIP, false);
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
                            MessageHelpers.ShowErrorMessage(msg, MessageHelpers.ErrorSource.externalIP, true);
                            MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), MessageHelpers.ErrorSource.externalIP, false);
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
                    MessageHelpers.ShowErrorMessage(msg, MessageHelpers.ErrorSource.externalIP, true);
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
                    MessageHelpers.ShowErrorMessage(msg, MessageHelpers.ErrorSource.externalIP, true);
                    MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), MessageHelpers.ErrorSource.externalIP, false);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error retrieving data: {ex.Message}");
                TrayIconHelpers.ShowProblemIcon = true;
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorConnecting, ex.Message);
                MessageHelpers.ShowErrorMessage(msg, MessageHelpers.ErrorSource.externalIP, true);
                MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), MessageHelpers.ErrorSource.externalIP, false);
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
            MessageHelpers.ShowErrorMessage(msg, MessageHelpers.ErrorSource.externalIP, false);
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }
        else
        {
            _log.Error($"Max retry count ({_retryCount}) reached.");
            string msg = string.Format(CultureInfo.InvariantCulture, MsgTextMaxRetriesReached, _retryCount);
            MessageHelpers.ShowErrorMessage(msg, MessageHelpers.ErrorSource.externalIP, false);
        }
    }
    #endregion Delay for retry if needed

    #region Process Json based on which provider was used
    /// <summary>
    /// Process Json based on which provider was used
    /// </summary>
    /// <param name="returnedJson">Json file to process</param>
    /// <param name="quiet">If <see langword="true"/>, limit what is written to the log</param>
    public static void ProcessProvider(string returnedJson, bool quiet)
    {
        if (!string.IsNullOrEmpty(returnedJson))
        {
            ClearGeoInfoList();
            switch (UserSettings.Setting!.InfoProvider)
            {
                case PublicInfoProvider.IpApiCom:
                    ProcessJson<IpApiCom>(returnedJson, quiet, "SettingsEnum_Provider_IpApiCom");
                    break;
                case PublicInfoProvider.SeeIP:
                    ProcessJson<SeeIP>(returnedJson, quiet, "SettingsEnum_Provider_SeeIp");
                    break;
                case PublicInfoProvider.FreeIpApi:
                    ProcessJson<FreeIpApi>(returnedJson, quiet, "SettingsEnum_Provider_FreeIpApi");
                    break;
                case PublicInfoProvider.IP2Location:
                    ProcessJson<IP2Location>(returnedJson, quiet, "SettingsEnum_Provider_Ip2Location");
                    break;
                case PublicInfoProvider.IpWho:
                    ProcessJson<IpWho>(returnedJson, quiet, "SettingsEnum_Provider_IpWhoOrg");
                    break;
                default:
                    _log.Error("Invalid External IP information provider. Check the provider in Settings > Application Settings.");
                    // ToDo: Localize this in the next update.
                    MessageHelpers.ShowErrorMessage("Invalid External IP information provider. Check the provider in Settings > Application Settings.",
                                   MessageHelpers.ErrorSource.externalIP,
                                   true);
                    break;
            }
            ShowLastRefresh();
        }
        else
        {
            _log.Error("There was a problem either connecting to the information provider or with processing the returned data.");
        }
    }

    /// <summary>
    /// Generic method to process JSON based on the provider type.
    /// </summary>
    /// <typeparam name="T">The type of the provider.</typeparam>
    /// <param name="json">The JSON string to process.</param>
    /// <param name="quiet">If <see langword="true"/>, limit what is written to the log.</param>
    /// <param name="providerResourceKey">The resource key for the provider name.</param>
    private static void ProcessJson<T>(string json, bool quiet, string providerResourceKey) where T : class
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                T? info = JsonSerializer.Deserialize<T>(json, JsonHelpers.JsonOptions);
                if (info == null)
                {
                    _log.Error("JSON was null. Check for previous error messages.");
                    MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull2"), MessageHelpers.ErrorSource.externalIP, true);
                    return;
                }

                AddGeoInfo(info);
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("SettingsItem_PublicInfoProvider"), GetStringResource(providerResourceKey)));

                if (RefreshInfo.Instance.LastIPAddress?.Length == 0)
                {
                    RefreshInfo.Instance.LastIPAddress = GetIpAddress(info);
                }

                if (!quiet)
                {
                    foreach (IPInfo item in IPInfo.GeoInfoList)
                    {
                        _log.Debug($"{item.Parameter} is {(UserSettings.Setting!.ObfuscateLog ? ObfuscateString(item.Value!) : item.Value)}");
                    }
                }
                else
                {
                    _log.Debug($"External IP address is {(UserSettings.Setting!.ObfuscateLog ? ObfuscateString(GetIpAddress(info)) : GetIpAddress(info))}");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(JsonHelpers.TruncateJson(json, 500));
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextErrorJsonParsing2, ex.Message);
                MessageHelpers.ShowErrorMessage(msg, MessageHelpers.ErrorSource.externalIP, true);
            }
        });
    }

    /// <summary>
    /// Adds the geolocation information to the GeoInfoList based on the provider type.
    /// </summary>
    /// <typeparam name="T">The type of the provider.</typeparam>
    /// <param name="info">The deserialized provider information.</param>
    private static void AddGeoInfo<T>(T info) where T : class
    {
        switch (info)
        {
            case IpApiCom ipApiCom:
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), ipApiCom.IpAddress));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), ipApiCom.City));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), ipApiCom.State));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), ipApiCom.Zip));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), ipApiCom.Country));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), ipApiCom.CountryCode));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Continent"), ipApiCom.Continent));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), ipApiCom.Lon.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), ipApiCom.Lat.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_TimeZone"), ipApiCom.TimeZone));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), ConvertOffset(ipApiCom.Offset)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Provider"), ipApiCom.Isp));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), ipApiCom.AS));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASName"), ipApiCom.ASName));
                break;
            case SeeIP seeIp:
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), seeIp.IpAddress));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), seeIp.City));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), seeIp.Region));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), seeIp.Region_Code));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), seeIp.Country));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), seeIp.Country_Code));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), seeIp.Postal_Code));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ContinentCode"), seeIp.Continent_Code));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), seeIp.Longitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), seeIp.Latitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_TimeZone"), seeIp.TimeZone));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), ConvertOffset(seeIp.Offset)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), seeIp.ASN.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Organization"), seeIp.Organization));
                break;
            case FreeIpApi freeIpApi:
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), freeIpApi.IpAddress));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), freeIpApi.CityName));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), freeIpApi.RegionName));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), freeIpApi.CountryName));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), freeIpApi.CountryCode));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Continent"), freeIpApi.Continent));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), freeIpApi.Longitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), freeIpApi.Latitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpType"), $"IPv{freeIpApi.IpVersion}"));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), freeIpApi.ASN.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Organization"), freeIpApi.AsnOrganization));
                break;
            case IP2Location ip2Location:
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), ip2Location.IpAddress));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), ip2Location.City_Name));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), ip2Location.Region_Name));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), ip2Location.Country_Name));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), ip2Location.Country_Code));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), ip2Location.Zip_Code));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), ip2Location.Latitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), ip2Location.Longitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), ip2Location.Time_Zone));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASName"), ip2Location.AS));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), ip2Location.ASN));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IsProxy"), ip2Location.Is_Proxy.ToYesNoString()));
                break;
            case IpWho ipWho:
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), ipWho.Data!.IpAddress));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), ipWho.Data.City));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), ipWho.Data.Region));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), ipWho.Data.RegionCode));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), ipWho.Data.Country));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_CountryCode"), ipWho.Data.CountryCode));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), ipWho.Data.PostalCode));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Continent"), ipWho.Data.Continent));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ContinentCode"), ipWho.Data.ContinentCode));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), ipWho.Data.Longitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), ipWho.Data.Latitude.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_TimeZone"), ipWho.Data.Timezone.TimeZoneName));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_TimeZone"), ipWho.Data.Timezone.Abbr));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), ipWho.Data.Timezone.UtcOffset));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), ipWho.Data.Connection.Number.ToString(CultureInfo.InvariantCulture)));
                IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Organization"), ipWho.Data.Connection.Org));
                break;
        }
    }

    /// <summary>
    /// Gets the IP address from the provider information.
    /// </summary>
    /// <typeparam name="T">The type of the provider.</typeparam>
    /// <param name="info">The provider information.</param>
    /// <returns>The IP address as a string.</returns>
    private static string GetIpAddress<T>(T info) where T : class
    {
        switch (info)
        {
            case IpApiCom ipApiCom:
                return ipApiCom.IpAddress;
            case SeeIP seeIp:
                return seeIp.IpAddress;
            case FreeIpApi freeIpApi:
                return freeIpApi.IpAddress;
            case IP2Location ip2Location:
                return ip2Location.IpAddress;
            case IpWho ipWho:
                return ipWho.Data!.IpAddress;
            default:
                // Shouldn't ever get here.
                _log.Warn("Could not get IP address because provider is unknown.");
                return string.Empty;
        }
    }
    #endregion Process Json based on which provider was used

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
                        _infoIp2Location = JsonSerializer.Deserialize<IP2Location>(json, opts);
                        LogIP2LocationInfo(_infoIp2Location);
                        break;
                    case PublicInfoProvider.IpWho:
                        _infoIpWho = JsonSerializer.Deserialize<IpWho>(json, opts);
                        LogIpWhoInfo(_infoIpWho);
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
          .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-5}", "n/a")
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

    private static void LogIpWhoInfo(IpWho? info)
    {
        if (info == null)
            return;

        if (info.Success)
        {
            StringBuilder sb = new();
            sb.Append(' ').AppendFormat(CultureInfo.InvariantCulture, "{0,-16}", info.Data!.IpAddress)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-10}", info.Data.City)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-12}", info.Data.Region)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-5}", info.Data.PostalCode)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", info.Data.Latitude)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,9}", info.Data.Longitude)
              .Append("  ").AppendFormat(CultureInfo.InvariantCulture, "{0,-25}", info.Data.Connection.Org);
            _logPerm.Info(sb.ToString().TrimEnd('\n', '\r'));
        }
        else
        {
            _log.Error("Could not obtain external IP info for logging.");
        }
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

    #region Clear external geolocation info
    /// <summary>
    /// Clears the ObservableCollection used to hold the external geolocation info.
    /// </summary>
    public static void ClearGeoInfoList()
    {
        if (IPInfo.GeoInfoList.Count > 0)
        {
            Application.Current.Dispatcher.Invoke(IPInfo.GeoInfoList.Clear);
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
            MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_JsonParsing2"), MessageHelpers.ErrorSource.externalIP, true);
            MessageHelpers.ShowErrorMessage(GetStringResource("MsgText_Error_SeeLog"), MessageHelpers.ErrorSource.externalIP, false);
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

    #region Save Latest JSON to File
    /// <summary>
    /// Saves the latest raw external JSON to a file.
    /// This can be used to diagnose unexpected output on the External page.
    /// </summary>
    public static void SaveLatestJsonToFile()
    {
        // Ensure there is JSON to save
        if (string.IsNullOrWhiteSpace(LatestRawExternalJson))
        {
            _ = MessageBox.Show("No external JSON data available to save.",
                                "Save JSON",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            return;
        }

        string providerName = UserSettings.Setting!.InfoProvider.ToString();

        // Configure and show the SaveFileDialog
        SaveFileDialog saveFileDialog = new()
        {
            Title = "Save External IP JSON",
            Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = "json",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            FileName = providerName + ".json"
        };

        bool? result = saveFileDialog.ShowDialog();

        if (result == true)
        {
            try
            {
                JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(LatestRawExternalJson);
                string prettyJson = JsonSerializer.Serialize(jsonElement, JsonHelpers.JsonOptions);
                prettyJson = System.Text.RegularExpressions.Regex.Unescape(prettyJson);
                File.WriteAllText(saveFileDialog.FileName, prettyJson);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Error saving file: {ex.Message}",
                                    "Save JSON",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }
    }
    #endregion Save Latest JSON to File
}
