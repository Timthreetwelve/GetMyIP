// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;
/// <summary>
/// Class for finding IP address related information.
/// </summary>
internal static class IpHelpers
{
    #region NLog Instance
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    private static readonly Logger _logPerm = LogManager.GetLogger("logPerm");
    #endregion NLog Instance

    #region Private fields
    private static IPGeoLocation _info;
    #endregion Private fields

    #region Get Internal IP
    /// <summary>
    /// Gets internal ip addresses asynchronously.
    /// </summary>
    public static async Task GetMyInternalIP()
    {
        _log.Debug("Starting discovery of internal IP information.");
        IPInfo.InternalList.Clear();
        if (!ConnectivityHelpers.IsConnectedToNetwork())
        {
            _log.Error("A network connection was not found.");
            ShowErrorMessage("Network connection not found.");
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
                IPInfo.InternalList.Add(new IPInfo("Internal IPv4 Address", address.ToString()));
                _log.Debug($"Internal IPv4 Address is {address}");
            }
        }
        // and optionally for IPv6 host
        if (UserSettings.Setting.IncludeV6)
        {
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily.ToString() == "InterNetworkV6")
                {
                    IPInfo.InternalList.Add(new IPInfo("Internal IPv6 Address", address.ToString()));
                    _log.Debug($"Internal IPv6 Address is {address}");
                }
            }
        }
        sw.Stop();
        _log.Debug($"Discovering internal addresses took {sw.Elapsed.TotalMilliseconds:N2} ms");
    }
    #endregion Get Internal IP

    #region Get External IP & Geolocation info
    /// <summary>
    /// Attempts to retrieve the external IP information and verifies the information retrieved is not null.
    /// </summary>
    public static async Task GetExtInfo()
    {
        Stopwatch sw = Stopwatch.StartNew();
        string someJson = await GetIPInfoAsync(AppConstString.InfoUrl);

        if (someJson == null)
        {
            sw.Stop();
            return;
        }
        ProcessIPInfo(someJson);
        sw.Stop();

        _log.Debug($"Discovering external IP information took {sw.Elapsed.TotalMilliseconds:N2} ms");
    }

    /// <summary>
    /// Gets the IP information asynchronously.
    /// </summary>
    /// <param name="url">The URL used to obtain external IP information.</param>
    /// <returns></returns>
    public static async Task<string> GetIPInfoAsync(string url)
    {
        Debug.WriteLine($"In Get IP Info Async {Environment.CurrentManagedThreadId}");
        _log.Debug("Starting discovery of external IP information.");
        if (!ConnectivityHelpers.IsConnectedToInternet())
        {
            _log.Error("Internet connection not found.");
            ShowErrorMessage("Internet connection not found.");
            return null;
        }
        if (!IsValidUrl(AppConstString.InfoUrl))
        {
            _log.Error($"The URL '{AppConstString.InfoUrl}' is not valid");
            ShowErrorMessage("Invalid URL found.");
            return null;
        }

        try
        {
            HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Task<string> x = response.Content.ReadAsStringAsync();
                return x.Result;
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                ShowErrorMessage("ip-api.com says: Too Many Requests.");
                return null;
            }
            else
            {
                ShowErrorMessage($"Error {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException hx)
        {
            _log.Error(hx, "Error retrieving data");
            ShowErrorMessage("Error connecting to ip-api.com.");
            return null;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving data");
            ShowErrorMessage("Error retrieving external IP address.");
            return null;
        }
    }
    #endregion Get External IP & Geolocation info

    #region Deserialize JSON containing IP info
    /// <summary>
    /// Deserialize the JSON containing the ip information.
    /// </summary>
    /// <param name="json">The json.</param>
    public static void ProcessIPInfo(string json)
    {
        try
        {
            JsonSerializerOptions opts = new()
            {
                PropertyNameCaseInsensitive = true
            };

            _info = JsonSerializer.Deserialize<IPGeoLocation>(json, opts);
            IPInfo.GeoInfoList.Clear();

            if (string.Equals(_info.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                IPInfo.GeoInfoList.Add(new IPInfo("External IP Address", _info.IpAddress));
                IPInfo.GeoInfoList.Add(new IPInfo("City", _info.City));
                IPInfo.GeoInfoList.Add(new IPInfo("State", _info.State));
                IPInfo.GeoInfoList.Add(new IPInfo("Zip Code", _info.Zip));
                IPInfo.GeoInfoList.Add(new IPInfo("Country", _info.Country));
                IPInfo.GeoInfoList.Add(new IPInfo("Continent", _info.Continent));
                IPInfo.GeoInfoList.Add(new IPInfo("Longitude", _info.Lon.ToString()));
                IPInfo.GeoInfoList.Add(new IPInfo("Latitude", _info.Lat.ToString()));
                IPInfo.GeoInfoList.Add(new IPInfo("Time Zone", _info.TimeZone));
                IPInfo.GeoInfoList.Add(new IPInfo("Offset from UTC", ConvertOffset(_info.Offset)));
                IPInfo.GeoInfoList.Add(new IPInfo("ISP", _info.Isp));
                IPInfo.GeoInfoList.Add(new IPInfo("AS Number", _info.AS));
                IPInfo.GeoInfoList.Add(new IPInfo("AS Name", _info.ASName));
            }
            else
            {
                IPInfo.GeoInfoList.Add(new IPInfo("Status", _info.Status));
                IPInfo.GeoInfoList.Add(new IPInfo("Message", _info.Message));
            }
        }
        catch (JsonException ex)
        {
            _log.Error(ex, "Error parsing JSON");
            _log.Error(json);
            ShowErrorMessage($"Error parsing JSON.\n{ex.Message}\n");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error parsing JSON");
            _log.Error(json);
            ShowErrorMessage("Error parsing JSON.");
        }

        foreach (IPInfo item in IPInfo.GeoInfoList)
        {
            _log.Debug($"{item.Parameter} is {item.Value}");
        }
    }
    #endregion Deserialize JSON containing IP info

    #region Log IP info
    /// <summary>
    /// Writes the external ip information to the log file.
    /// </summary>
    public static void LogIPInfo()
    {
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
            _ = sb.Append("  ").AppendFormat("{0,-25}", _info.AS);
            _logPerm.Info(sb.ToString());
        }
        else
        {
            _log.Error(_info.Message);
            _logPerm.Error($" {_info.Status,-16}  {_info.Message}");
        }
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
            _ = MessageBox.Show($"{errorMsg}\nSee log file for more information.",
                "Get My IP Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }));
    }
    #endregion Show MessageBox with error message
}
