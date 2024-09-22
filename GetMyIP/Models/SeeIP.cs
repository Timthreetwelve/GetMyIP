// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class used to deserialize the JSON returned from https://api.seeip.org/geoip
/// </summary>
/// <remarks>Ensure that PropertyNameCaseInsensitive is set to true in deserialize method.</remarks>
internal sealed class SeeIP
{
    /// <summary>
    /// IP address
    /// </summary>
    public string Ip { get; init; } = string.Empty;
    public string IpAddress => Ip;

    /// <summary>
    /// City Name
    /// </summary>
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Region or State
    /// </summary>
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Region Code (2 characters)
    /// </summary>
    public string Region_Code { get; init; } = string.Empty;

    /// <summary>
    /// Country Name
    /// </summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Country Code (2 characters)
    /// </summary>
    public string Country_Code { get; init; } = string.Empty;

    /// <summary>
    /// Country Code (3 characters)
    /// </summary>
    public string Country_Code3 { get; init; } = string.Empty;

    /// <summary>
    /// Continent Code
    /// </summary>
    public string Continent_Code { get; init; } = string.Empty;

    /// <summary>
    /// Postal or Zip code
    /// </summary>
    public string Postal_Code { get; init; } = string.Empty;

    /// <summary>
    /// Latitude (decimal)
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude (decimal)
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Time Zone
    /// </summary>
    public string TimeZone { get; init; } = string.Empty;

    /// <summary>
    /// Offset from UTC in seconds
    /// </summary>
    public int Offset { get; init; }

    /// <summary>
    /// Autonomous System Number as int
    /// </summary>
    public int ASN { get; init; }

    /// <summary>
    /// Organization Name
    /// </summary>
    public string Organization { get; init; } = string.Empty;
}
