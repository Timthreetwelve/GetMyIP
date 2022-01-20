// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using GetMyIp;
using Newtonsoft.Json;
using NLog;
#endregion Using directives

namespace GetMyIP
{
    public static class ExternalInfo
    {
        #region NLog
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        #endregion NLog

        #region Get External IP & Geolocation info
        public static void GetExtInfo()
        {
            if (IsValidUrl(UserSettings.Setting.URL))
            {
                Stopwatch sw = Stopwatch.StartNew();
                Task<string> someJson = GetIPInfoAsync(UserSettings.Setting.URL);

                if (someJson.Result != null)
                {
                    ProcessIPInfo(someJson.Result);
                    sw.Stop();
                    log.Debug($"Discovering external IP information took {sw.ElapsedMilliseconds} ms");
                }
                else
                {
                    sw.Stop();
                    log.Error("GetIPInfoAsync returned null");
                    IPInfo.GeoInfoList.Add(new IPInfo("Error", "Error retrieving external IP address. See log file."));
                }
            }
            else
            {
                log.Error($"The URL '{UserSettings.Setting.URL}' is not valid");
                IPInfo.GeoInfoList.Add(new IPInfo("Error", "Invalid URL found. See log file."));
            }
        }

        public static async Task<string> GetIPInfoAsync(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                using (HttpResponseMessage response = await client
                    .GetAsync(url)
                    .ConfigureAwait(false))
                {
                    Task<string> x = response.Content.ReadAsStringAsync();
                    return x.Result;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error retrieving data");
                return null;
            }
        }
        #endregion Get External IP & Geolocation info

        #region Deserialize JSON containing IP info
        public static void ProcessIPInfo(string json)
        {
            try
            {
                IPGeoLocation info = JsonConvert.DeserializeObject<IPGeoLocation>(json);
                IPInfo.GeoInfoList.Clear();

                if (string.Equals(info.Status, "success", StringComparison.OrdinalIgnoreCase))
                {
                    IPInfo.GeoInfoList.Add(new IPInfo("External IP Address", info.IpAddress));
                    IPInfo.GeoInfoList.Add(new IPInfo("City", info.City));
                    IPInfo.GeoInfoList.Add(new IPInfo("State", info.State));
                    IPInfo.GeoInfoList.Add(new IPInfo("Zip Code", info.Zip));
                    IPInfo.GeoInfoList.Add(new IPInfo("Country", info.Country));
                    IPInfo.GeoInfoList.Add(new IPInfo("Continent", info.Continent));
                    IPInfo.GeoInfoList.Add(new IPInfo("Longitude", info.Lon.ToString()));
                    IPInfo.GeoInfoList.Add(new IPInfo("Latitude", info.Lat.ToString()));
                    IPInfo.GeoInfoList.Add(new IPInfo("Time Zone", info.Timezone));
                    IPInfo.GeoInfoList.Add(new IPInfo("UTC Offset", ConvertOffset(info.Offset)));
                    IPInfo.GeoInfoList.Add(new IPInfo("ISP", info.Isp));
                }
                else
                {
                    IPInfo.GeoInfoList.Add(new IPInfo("Status", info.Status));
                    IPInfo.GeoInfoList.Add(new IPInfo("Message", info.Message));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error parsing JSON");
                IPInfo.GeoInfoList.Add(new IPInfo("Error", "Error parsing JSON. See log file."));
            }

            foreach (IPInfo item in IPInfo.GeoInfoList)
            {
                log.Debug($"{item.Parameter} is {item.Value}");
            }
        }
        #endregion Deserialize JSON containing IP info

        #region Convert offset from seconds to hours and minutes
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
        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        #endregion Check Url
    }
}
