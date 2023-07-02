// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

public static class ExternalInfo
{
    #region NLog
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    private static readonly Logger _logPerm = LogManager.GetLogger("logPerm");
    #endregion NLog

    #region Private fields
    private static IPGeoLocation _info;
    #endregion Private fields

    #region Get External IP & Geolocation info
    /// <summary>
    /// Attempts to retrieve the external IP information and verifies the information retrieved is not null.
    /// </summary>
    public static async Task GetExtInfo()
    {
        if (IsValidUrl(UserSettings.Setting.URL))
        {
            Stopwatch sw = Stopwatch.StartNew();
            string someJson = await GetIPInfoAsync(UserSettings.Setting.URL);

            if (someJson != null)
            {
                ProcessIPInfo(someJson);
                sw.Stop();
                _log.Debug($"Discovering external IP information took {sw.ElapsedMilliseconds} ms");
            }
            else
            {
                sw.Stop();
                _log.Error("GetIPInfoAsync returned null");
                IPInfo.GeoInfoList.Add(new IPInfo("Error", "Error retrieving external IP address. See log file."));
            }
        }
        else
        {
            _log.Error($"The URL '{UserSettings.Setting.URL}' is not valid");
            IPInfo.GeoInfoList.Add(new IPInfo("Error", "Invalid URL found. See log file."));
        }
    }

    /// <summary>
    /// Gets the ip information asynchronously.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns></returns>
    public static async Task<string> GetIPInfoAsync(string url)
    {
        try
        {
            HttpClient client = new();
            using HttpResponseMessage response = await client
                .GetAsync(url)
                .ConfigureAwait(false);
            Task<string> x = response.Content.ReadAsStringAsync();
            return x.Result;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error retrieving data");
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
                IPInfo.GeoInfoList.Add(new IPInfo("Time Zone", _info.Timezone));
                IPInfo.GeoInfoList.Add(new IPInfo("Offset from UTC", ConvertOffset(_info.Offset)));
                IPInfo.GeoInfoList.Add(new IPInfo("ISP", _info.Isp));
            }
            else
            {
                IPInfo.GeoInfoList.Add(new IPInfo("Status", _info.Status));
                IPInfo.GeoInfoList.Add(new IPInfo("Message", _info.Message));
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error parsing JSON");
            IPInfo.GeoInfoList.Add(new IPInfo("Error", "Error parsing JSON. See log file."));
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
            _ = sb.Append("  ").AppendFormat("{0}", _info.Timezone);
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
    /// <param name="offset">The offset.</param>
    /// <returns></returns>
    private static string ConvertOffset(int offset)
    {
        string neg = "";
        if (offset < 0)
        {
            offset = Math.Abs(offset);
            neg = "-";
        }
        TimeSpan ts = TimeSpan.FromSeconds(offset);
        string hhmm = ts.ToString(@"hh\:mm");
        return neg + hhmm;
    }
    #endregion Convert offset from seconds to hours and minutes

    #region Check Url
    /// <summary>
    /// Determines whether the specified URL appears to be valid.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>
    ///   <c>true</c> if the URL appears to be valid; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    #endregion Check Url
}
