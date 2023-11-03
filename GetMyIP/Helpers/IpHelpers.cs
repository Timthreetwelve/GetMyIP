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
    private static IPGeoLocation _info;
    private static IpExtOrg _infoExtOrg;
    #endregion Private fields

    #region Get Internal & External info
    /// <summary>
    /// Get internal and external IP info asynchronously. <see langword="async"/>
    /// </summary>
    /// <returns>External IP info as string.</returns>
    public static async Task<string> GetAllInfoAsync()
    {
        Task intInfo = GetMyInternalIPAsync();
        Task<string> extInfo = GetExternalInfo();

        await Task.WhenAll(intInfo, extInfo);
        return extInfo.Result;
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
            ShowErrorMessage(GetStringResource("Internal_Error_NetworkNotFound"));
            return;
        }

        Stopwatch sw = Stopwatch.StartNew();
        string host = Dns.GetHostName();
        IPHostEntry hostEntry = await Dns.GetHostEntryAsync(host);

        // Get info for each IPv4 host
        foreach (IPAddress address in hostEntry.AddressList)
        {
            if (address.AddressFamily.ToString() == "InterNetwork")
            {
                IPInfo.InternalList.Add(new IPInfo(GetStringResource("Internal_IPv4Address"), address.ToString()));
                if (UserSettings.Setting.ObfuscateLog)
                {
                    _log.Debug($"Internal IPv4 Address is {ObfuscateString(address.ToString())}");
                }
                else
                {
                    _log.Debug($"Internal IPv4 Address is {address}");
                }
            }
        }
        // and optionally for IPv6 host
        if (UserSettings.Setting.IncludeV6)
        {
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily.ToString() == "InterNetworkV6")
                {
                    IPInfo.InternalList.Add(new IPInfo(GetStringResource("Internal_IPv6Address"), address.ToString()));
                    if (UserSettings.Setting.ObfuscateLog)
                    {
                        _log.Debug($"Internal IPv6 Address is {ObfuscateString(address.ToString())}");
                    }
                    else
                    {
                        _log.Debug($"Internal IPv6 Address is {address}");
                    }
                }
            }
        }
        sw.Stop();
        _log.Debug($"Discovering internal addresses took {sw.Elapsed.TotalMilliseconds:N2} ms");
    }
    #endregion Get Internal IP

    #region Get External IP & Geolocation info
    /// <summary>
    /// Attempts to retrieve the external IP information.
    /// </summary>
    public static async Task<string> GetExternalInfo()
    {
        Stopwatch sw = Stopwatch.StartNew();
        string someJson;
        switch (UserSettings.Setting.InfoProvider)
        {
            case PublicInfoProvider.IpApiCom:
                someJson = await GetIPInfoAsync(AppConstString.IpApiUrl);
                 Map.Instance.CanMap = true;
                break;
            case PublicInfoProvider.IpExtOrg:
                someJson = await GetIPInfoAsync(AppConstString.IpExtUrl);
                Map.Instance.CanMap = false;
                break;
            default:
                someJson = await GetIPInfoAsync(AppConstString.IpApiUrl);
                Map.Instance.CanMap = true;
                break;
        }
        sw.Stop();
        _log.Debug($"Discovering external IP information took {sw.Elapsed.TotalMilliseconds:N2} ms");
        // It is possible that someJson is null at this point.
        return someJson;
    }

    /// <summary>
    /// Gets the IP information asynchronously.
    /// </summary>
    /// <param name="url">The URL used to obtain external IP information.</param>
    /// <returns></returns>
    public static async Task<string> GetIPInfoAsync(string url)
    {
        if (!ConnectivityHelpers.IsConnectedToInternet())
        {
            _log.Error("Internet connection not found.");
            ShowErrorMessage(GetStringResource("MsgText_Error_InternetNotFound"));
            return null;
        }
        if (!IsValidUrl(AppConstString.IpApiUrl))
        {
            _log.Error($"The URL '{AppConstString.IpApiUrl}' is not valid");
            ShowErrorMessage(GetStringResource("MsgText_Error_InvalidURL"));
            return null;
        }

        try
        {
            _log.Debug("Starting discovery of external IP information.");
            HttpClient client = new();
            Uri uri = new(url);
            string baseUri = uri.GetLeftPart(UriPartial.Authority);
            _log.Debug($"Connecting to: {baseUri}");
            using HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                Task<string> returnedText = response.Content.ReadAsStringAsync();
                _log.Debug($"Received status code: {response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                return returnedText.Result;
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _log.Error($"Received status code: {response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                ShowErrorMessage(GetStringResource("MsgText_Error_TooManyRequests"));
                return null;
            }
            else
            {
                _log.Error($"Received status code: {response.StatusCode} - {response.ReasonPhrase} from {baseUri}");
                Task<string> returnedText = response.Content.ReadAsStringAsync();
                if (returnedText.Exception != null)
                {
                    _log.Error(returnedText.Exception);
                }
                string msg = string.Format(GetStringResource("MsgText_Error_Connecting"), response.StatusCode);
                ShowErrorMessage(msg);
                return null;
            }
        }
        catch (HttpRequestException hx)
        {
            _log.Error(hx, "Error retrieving data");
            string msg = string.Format(GetStringResource("MsgText_Error_Connecting"), hx.Message);
            ShowErrorMessage(msg);
            return null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving data");
            string msg = string.Format(GetStringResource("MsgText_Error_Connecting"), ex.Message);
            ShowErrorMessage(msg);
            return null;
        }
    }
    #endregion Get External IP & Geolocation info

    #region Process Json based on which provider was used
    /// <summary>
    /// Process Json based on which provider was used
    /// </summary>
    /// <param name="returnedJson">Json file to process</param>
    public static void ProcessProvider(string returnedJson)
    {
        switch (UserSettings.Setting.InfoProvider)
        {
            case PublicInfoProvider.IpApiCom:
                ProcessIPApiCom(returnedJson);
                break;
            case PublicInfoProvider.IpExtOrg:
                ProcessIPExtOrg(returnedJson);
                break;
            default: throw new Exception("Invalid Provider");
                //ToDo: handle this more gracefully
        }
    }
    #endregion Process Json based on which provider was used

    #region Deserialize JSON from ip-api.com
    /// <summary>
    /// Deserialize the JSON containing the ip information.
    /// </summary>
    /// <param name="json">The json.</param>
    public static void ProcessIPApiCom(string json)
    {
        Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            try
            {
                JsonSerializerOptions opts = new()
                {
                    PropertyNameCaseInsensitive = true
                };
                if (json != null)
                {
                    _info = JsonSerializer.Deserialize<IPGeoLocation>(json, opts);

                    if (string.Equals(_info.Status, "success", StringComparison.OrdinalIgnoreCase))
                    {
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), _info.IpAddress));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_City"), _info.City));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_State"), _info.State));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_PostalCode"), _info.Zip));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Country"), _info.Country));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Continent"), _info.Continent));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Longitude"), _info.Lon.ToString()));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Latitude"), _info.Lat.ToString()));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_TimeZone"), _info.TimeZone));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_UTCOffset"), ConvertOffset(_info.Offset)));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Provider"), _info.Isp));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASNumber"), _info.AS));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_ASName"), _info.ASName));
                    }
                    else
                    {
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Status"), _info.Status));
                        IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_Message"), _info.Message));
                    }

                    foreach (IPInfo item in IPInfo.GeoInfoList)
                    {
                        if (UserSettings.Setting.ObfuscateLog)
                        {
                            _log.Debug($"{item.Parameter} is {ObfuscateString(item.Value)}");
                        }
                        else
                        {
                            _log.Debug($"{item.Parameter} is {item.Value}");
                        }
                    }
                }
                else
                {
                    _log.Error("JSON was null. Check for previous error messages.");
                    ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull"));
                }
            }
            catch (JsonException ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(GetStringResource("MsgText_Error_JsonParsing"), ex.Message);
                ShowErrorMessage(msg);
            }
            catch (ArgumentNullException ex)
            {
                _log.Error(ex, "Error parsing JSON. JSON was null.");
                ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull"));
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(GetStringResource("MsgText_Error_JsonParsing"), ex.Message);
                ShowErrorMessage(msg);
            }
        }));
    }
    #endregion Deserialize JSON from ip-api.com

    #region Obfuscate IP info
    /// <summary>
    /// Obfuscate a string by replacing letters with X, and numbers with #.
    /// Special characters are left unaltered.
    /// </summary>
    /// <param name="unalteredString">String to obfuscate.</param>
    /// <returns>Obfuscated string. If the string is null or empty "string.Empty" will be returned.</returns>
    private static string ObfuscateString(string unalteredString)
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

    #region Deserialize JSON from ipext.org
    /// <summary>
    /// Deserialize the JSON containing the ip information.
    /// </summary>
    /// <param name="json">The json.</param>
    public static void ProcessIPExtOrg(string json)
    {
        Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            try
            {
                JsonSerializerOptions opts = new()
                {
                    PropertyNameCaseInsensitive = true
                };

                if (json != null)
                {
                    _infoExtOrg = JsonSerializer.Deserialize<IpExtOrg>(json, opts);

                    IPInfo.GeoInfoList.Add(new IPInfo(GetStringResource("External_IpAddress"), _infoExtOrg.IpAddress));
                    IPInfo.GeoInfoList.Add(new IPInfo("IP Type", _infoExtOrg.IpType.Replace("ip", "IP")));

                    foreach (IPInfo item in IPInfo.GeoInfoList)
                    {
                        if (UserSettings.Setting.ObfuscateLog)
                        {
                            _log.Debug($"{item.Parameter} is {ObfuscateString(item.Value)}");
                        }
                        else
                        {
                            _log.Debug($"{item.Parameter} is {item.Value}");
                        }
                    }
                }
                else
                {
                    _log.Error("JSON was null. Check for previous error messages.");
                    ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull"));
                }
            }
            catch (JsonException ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(GetStringResource("MsgText_Error_JsonParsing"), ex.Message);
                ShowErrorMessage(msg);
            }
            catch (ArgumentNullException ex)
            {
                _log.Error(ex, "Error parsing JSON. JSON was null.");
                ShowErrorMessage(GetStringResource("MsgText_Error_JsonNull"));
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error parsing JSON");
                _log.Error(json);
                string msg = string.Format(GetStringResource("MsgText_Error_JsonParsing"), ex.Message);
                ShowErrorMessage(msg);
            }
        }));
    }
    #endregion Deserialize JSON from ipext.org

    #region Log IP info
    /// <summary>
    /// Writes the external ip information to the log file.
    /// </summary>
    /// <param name="json">JSON string containing public IP info</param>
    public static void LogIPInfo(string json)
    {
        Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            try
            {
                JsonSerializerOptions opts = new()
                {
                    PropertyNameCaseInsensitive = true
                };

                if (UserSettings.Setting.InfoProvider == PublicInfoProvider.IpApiCom)
                {
                    _info = JsonSerializer.Deserialize<IPGeoLocation>(json, opts);

                    if (string.Equals(_info.Status, "success", StringComparison.OrdinalIgnoreCase))
                    {
                        StringBuilder sb = new();
                        _ = sb.Append(' ').AppendFormat("{0,-16}", _info.IpAddress);
                        _ = sb.Append("  ").AppendFormat("{0,-10}", _info.City);
                        _ = sb.Append("  ").AppendFormat("{0,-12}", _info.State);
                        _ = sb.Append("  ").AppendFormat("{0,-5}", _info.Zip);
                        _ = sb.Append("  ").AppendFormat("{0,9}", _info.Lat);
                        _ = sb.Append("  ").AppendFormat("{0,9}", _info.Lon);
                        _ = sb.Append("  ").AppendFormat("{0,-25}", _info.Isp);
                        _ = sb.Append("  ").AppendLine(_info.AS);
                        _logPerm.Info(sb.ToString().TrimEnd('\n', '\r'));
                    }
                    else
                    {
                        _log.Error(_info.Message);
                        _logPerm.Error($" {_info.Status,-16}  {_info.Message}");
                    }
                }
                else if (UserSettings.Setting.InfoProvider == PublicInfoProvider.IpExtOrg)
                {
                    _infoExtOrg = JsonSerializer.Deserialize<IpExtOrg>(json, opts);
                    _logPerm.Info(" " + _infoExtOrg.Ip.TrimEnd('\n', '\r'));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error while attempting to log IP information");
                _logPerm.Error(ex, GetStringResource("MsgText_Error_Logging"));
            }
        }));
    }
    #endregion Log IP info

    #region Convert offset from seconds to hours and minutes
    /// <summary>
    /// Converts the offset value in the JSON to a more readable format.
    /// </summary>
    /// <param name="offset">The offset from UTC.</param>
    /// <returns></returns>
    private static string ConvertOffset(int offset)
    {
        string sign = "+";
        if (offset < 0)
        {
            offset = Math.Abs(offset);
            sign = "-";
        }
        TimeSpan ts = TimeSpan.FromSeconds(offset);
        string time = ts.ToString(@"hh\:mm");
        return sign + time;
    }
    #endregion Convert offset from seconds to hours and minutes

    #region Check Url
    /// <summary>
    /// Determines whether the specified URL appears to be valid.
    /// </summary>
    /// <param name="url">The URL to be checked.</param>
    /// <returns>
    ///   <c>true</c> if the URL appears to be valid; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    #endregion Check Url

    #region Show MessageBox with error message
    /// <summary>
    /// Shows an error message in a MessageBox.
    /// </summary>
    /// <param name="errorMsg">The error message.</param>
    private static void ShowErrorMessage(string errorMsg)
    {
        Application.Current.Dispatcher.Invoke(new Action(() =>
        {
            _ = MessageBox.Show($"{errorMsg}\n\n{GetStringResource("MsgText_Error_SeeLog")}",
                "Get My IP Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }));
    }
    #endregion Show MessageBox with error message
}
