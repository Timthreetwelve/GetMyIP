// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

public static class ToolTipHelper
{
    #region NLog Instance
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Build the tool tip text
    /// <summary>
    /// Builds the tool tip text based on options selected by the user.
    /// </summary>
    public static string BuildToolTip()
    {
        Stopwatch sw = Stopwatch.StartNew();
        StringBuilder sb = new();

        if (IPInfo.InternalList.Any(x => x.Parameter == "Internal IPv4 Address") && UserSettings.Setting.ShowInternalIPv4)
        {
            string intAddressV4 = IPInfo.InternalList.FirstOrDefault(x => x.Parameter == "Internal IPv4 Address").Value;
            _ = sb.AppendLine(intAddressV4);
        }

        if (IPInfo.InternalList.Any(x => x.Parameter == "Internal IPv6 Address") && UserSettings.Setting.ShowInternalIPv6)
        {
            string intAddressV6 = IPInfo.InternalList.FirstOrDefault(x => x.Parameter == "Internal IPv6 Address").Value;
            _ = sb.AppendLine(intAddressV6);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "External IP Address") && UserSettings.Setting.ShowExternalIP)
        {
            string extAddress = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "External IP Address").Value;
            _ = sb.AppendLine(extAddress);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "City") && UserSettings.Setting.ShowCity)
        {
            string city = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "City").Value;
            _ = sb.AppendLine(city);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "State") && UserSettings.Setting.ShowState)
        {
            string state = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "State").Value;
            _ = sb.AppendLine(state);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "Country") && UserSettings.Setting.ShowCountry)
        {
            string country = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Country").Value;
            _ = sb.AppendLine(country);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "Offset from UTC") && UserSettings.Setting.ShowOffset)
        {
            string offset = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Offset from UTC").Value;
            _ = sb.Append("UTC  ").AppendLine(offset);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "Time Zone") && UserSettings.Setting.ShowTimeZone)
        {
            string timeZone = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "Time Zone").Value;
            _ = sb.AppendLine(timeZone);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "ISP") && UserSettings.Setting.ShowISP)
        {
            string isp = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "ISP").Value;
            _ = sb.AppendLine(isp);
        }
        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "AS Number") && UserSettings.Setting.ShowASNumber)
        {
            string asNumber = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "AS Number").Value;
            _ = sb.AppendLine(asNumber);
        }
        if (IPInfo.GeoInfoList.Any(x => x.Parameter == "AS Name") && UserSettings.Setting.ShowASName)
        {
            string asName = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == "AS Name").Value;
            _ = sb.AppendLine(asName);
        }
        int tooltipSize = sb.Length;
        if (tooltipSize == 0)
        {
            _ = sb.AppendLine("Nothing selected for display.");
        }
        sw.Stop();
        _log.Debug($"Building tooltip ({sb.Length} bytes) took {sw.Elapsed.TotalMilliseconds:N2} ms.");

        return sb.ToString().TrimEnd('\n', '\r');
    }
    #endregion Build the tool tip text
}
