// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Constants;

/// <summary>
/// Class for constant strings
/// </summary>
public static class AppConstString
{
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

    /// <summary>
    /// Gets the URL for ip-api.com.
    /// </summary>
    /// <value>
    /// The information URL.
    /// </value>
    public static string InfoUrl { get; } = "http://ip-api.com/json/?fields=status,message,country,continent,regionName,city,zip,lat,lon,timezone,offset,isp,asname,as,query";
}