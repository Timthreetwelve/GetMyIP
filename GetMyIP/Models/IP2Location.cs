// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class used to deserialize the JSON returned from https://api.ip2location.io
/// </summary>
/// <remarks>Ensure that PropertyNameCaseInsensitive is set to true in deserialize method.</remarks>
internal sealed class IP2Location
{
    /// <summary>
    /// IP address
    /// </summary>
    public string Ip { get; init; } = string.Empty;
    public string IpAddress => Ip;

    /// <summary>
    /// Country Code (2 characters)
    /// </summary>
    public string Country_Code { get; init; } = string.Empty;

    /// <summary>
    /// Country Name
    /// </summary>
    public string Country_Name { get; init; } = string.Empty;

    /// <summary>
    /// Region or State
    /// </summary>
    public string Region_Name { get; init; } = string.Empty;

    /// <summary>
    /// City Name
    /// </summary>
    public string City_Name { get; init; } = string.Empty;

    /// <summary>
    /// Latitude (decimal)
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude (decimal)
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Postal or Zip code
    /// </summary>
    public string Zip_Code { get; init; } = string.Empty;

    /// <summary>
    /// Time Zone
    /// </summary>
    public string Time_Zone { get; init; } = string.Empty;

    /// <summary>
    /// Autonomous System Number
    /// </summary>
    public string ASN { get; init; } = string.Empty;

    /// <summary>
    /// Autonomous System Name
    /// </summary>
    public string AS { get; init; } = string.Empty;

    /// <summary>
    /// Proxy as bool
    /// </summary>
    public bool Is_Proxy { get; init; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; init; } = string.Empty;
}
