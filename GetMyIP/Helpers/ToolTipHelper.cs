// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

public static class ToolTipHelper
{
    #region Build the tool tip text
    /// <summary>
    /// Builds the tool tip text based on options selected by the user.
    /// </summary>
    public static string BuildToolTip(bool isQuiet)
    {
        StringBuilder sb = new();
        bool isValid = true;

        if (UserSettings.Setting!.ShowHeader && !string.IsNullOrEmpty(UserSettings.Setting.TooltipHeading))
        {
            _ = sb.AppendLine(UserSettings.Setting.TooltipHeading);
        }

        if (IPInfo.InternalList.Any(x => x.Parameter == GetStringResource("Internal_IPv4Address")) && UserSettings.Setting.ShowInternalIPv4)
        {
            string intAddressV4 = IPInfo.InternalList.FirstOrDefault(x => x.Parameter == GetStringResource("Internal_IPv4Address"))?.Value!;
            _ = sb.AppendLine(intAddressV4);
        }

        if (UserSettings.Setting.ShowInternalIPv6 && IPInfo.InternalList.Any(x => x.Parameter == GetStringResource("Internal_IPv6Address")))
        {
            string intAddressV6 = IPInfo.InternalList.FirstOrDefault(x => x.Parameter == GetStringResource("Internal_IPv6Address"))?.Value!;
            _ = sb.AppendLine(intAddressV6);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_IpAddress")) && UserSettings.Setting.ShowExternalIP)
        {
            string extAddress = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_IpAddress"))?.Value!;
            _ = sb.AppendLine(extAddress);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_City")) && UserSettings.Setting.ShowCity)
        {
            string city = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_City"))?.Value!;
            _ = sb.AppendLine(city);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_State")) && UserSettings.Setting.ShowState)
        {
            string state = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_State"))?.Value!;
            _ = sb.AppendLine(state);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_Country")) && UserSettings.Setting.ShowCountry)
        {
            string country = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_Country"))?.Value!;
            _ = sb.AppendLine(country);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_UTCOffset")) && UserSettings.Setting.ShowOffset)
        {
            string offset = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_UTCOffset"))?.Value!;
            _ = sb.Append("UTC  ").AppendLine(offset);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_TimeZone")) && UserSettings.Setting.ShowTimeZone)
        {
            string timeZone = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_TimeZone"))?.Value!;
            _ = sb.AppendLine(timeZone);
        }

        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_Provider")) && UserSettings.Setting.ShowISP)
        {
            string isp = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_Provider"))?.Value!;
            _ = sb.AppendLine(isp);
        }
        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_ASNumber")) && UserSettings.Setting.ShowASNumber)
        {
            string asNumber = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_ASNumber"))?.Value!;
            _ = sb.AppendLine(asNumber);
        }
        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_ASName")) && UserSettings.Setting.ShowASName)
        {
            string asName = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_ASName"))?.Value!;
            _ = sb.AppendLine(asName);
        }
        if (IPInfo.GeoInfoList.Any(x => x.Parameter == GetStringResource("External_IpType")) && UserSettings.Setting.ShowIpVersion)
        {
            string ipVersion = IPInfo.GeoInfoList.FirstOrDefault(x => x.Parameter == GetStringResource("External_IpType"))?.Value!;
            _ = sb.AppendLine(ipVersion);
        }
        if (sb.Length == 0)
        {
            _ = sb.AppendLine(GetStringResource("MsgText_TooltipNothingToDisplay"));
            isValid = false;
        }

        string tooltip = sb.ToString().TrimEnd('\n', '\r');
        if (isValid)
        {
            if (!isQuiet)
            {
                _log.Debug($"Tooltip is {tooltip.Length} bytes");
            }
            CustomToolTip.Instance.ToolTipSize = tooltip.Length;
        }
        else
        {
            _log.Debug("Tooltip is empty. Will use 'Nothing to display' message. ");
            CustomToolTip.Instance.ToolTipSize = 0;
        }

        return tooltip;
    }
    #endregion Build the tool tip text
}
