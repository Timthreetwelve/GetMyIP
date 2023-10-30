// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

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
    public static string IpApiUrl { get; } = "http://ip-api.com/json/?fields=status,message,country,continent,regionName,city,zip,lat,lon,timezone,offset,isp,asname,as,query";

    /// <summary>
    /// Gets the UTL for ipext.org
    /// </summary>
    /// <value>
    /// The URL including all of the parameters specified in the URL.
    /// </value>
    public static string IpExtUrl { get; } = "https://api.ipext.org/json";
    #endregion URL constants
}