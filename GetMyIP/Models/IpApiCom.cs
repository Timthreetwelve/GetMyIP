// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class to deserialize the JSON returned from http://ip-api.com/json
/// </summary>
public class IpApiCom
{
    /// <summary>
    /// Status: success or fail
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// Message: only when status is fail
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Country: Country name
    /// </summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Country: Country code
    /// </summary>
    public string CountryCode { get; init; } = string.Empty;

    /// <summary>
    /// RegionName: Region/State
    /// </summary>
    public string RegionName { get; init; } = string.Empty;

    /// <summary>
    /// State: Mapped to RegionName
    /// </summary>
    public string State => RegionName;

    /// <summary>
    /// City: City
    /// </summary>
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Zip: Zip code
    /// </summary>
    public string Zip { get; init; } = string.Empty;

    /// <summary>
    /// Lat: Latitude
    /// </summary>
    public double Lat { get; init; }

    /// <summary>
    /// Lon: Longitude
    /// </summary>
    public double Lon { get; init; }

    /// <summary>
    /// TimeZone: City TimeZone
    /// </summary>
    public string TimeZone { get; init; } = string.Empty;

    /// <summary>
    /// Isp: Internet service provider name
    /// </summary>
    public string Isp { get; init; } = string.Empty;

    /// <summary>
    /// Query: IP address used for the query
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// IPAddress: Mapped to Query
    /// </summary>
    public string IpAddress => Query;

    /// <summary>
    /// Continent (not abbreviated)
    /// </summary>
    public string Continent { get; init; } = string.Empty;

    /// <summary>
    /// Offset from UTC in seconds
    /// </summary>
    public int Offset { get; init; }

    /// <summary>
    /// Autonomous System (AS) Number
    /// </summary>
    public string AS { get; init; } = string.Empty;

    /// <summary>
    /// Autonomous System (AS) Name
    /// </summary>
    public string ASName { get; init; } = string.Empty;
}
