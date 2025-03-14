﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Constants;

/// <summary>
/// Class for constant strings
/// </summary>
public static class AppConstString
{
    #region GitHub constants
    /// <summary>
    /// Gets the GitHub repository owner.
    /// </summary>
    /// <value>
    /// The repository owner.
    /// </value>
    public static string RepoOwner { get; } = "TimThreeTwelve";

    /// <summary>
    /// Gets the name of the GitHub repository.
    /// </summary>
    /// <value>
    /// The name of the repository.
    /// </value>
    public static string RepoName { get; } = "GetMyIP";
    #endregion GitHub constants

    #region URL constants
    /// <summary>
    /// Gets the URL for ip-api.com.
    /// </summary>
    /// <value>
    /// The URL including all of the parameters specified in the URL.
    /// </value>
    public static string IpApiUrl { get; } = "http://ip-api.com/json/?fields=status,message,country,countryCode,continent,regionName,city,zip,lat,lon,timezone,offset,isp,asname,as,query";

    /// <summary>
    /// Gets the URL for freeipapi.com
    /// </summary>
    /// <value>
    /// The URL including all of the parameters specified in the URL.
    /// </value>
    public static string FreeIpApiUrl { get; } = "https://freeipapi.com/api/json";

    /// <summary>
    /// Gets the URL for SeeIP.org
    /// </summary>
    /// <value>
    /// The URL including all of the parameters specified in the URL.
    /// </value>
    public static string SeeIpURL { get; } = "https://api.seeip.org/geoip";

    /// <summary>
    /// Gets the URL for IP2Location.io
    /// </summary>
    /// <value>
    /// The URL including all of the parameters specified in the URL.
    /// </value>
    public static string IP2LocationURL { get; } = "https://api.ip2location.io/?format=json";

    #endregion URL constants
}
