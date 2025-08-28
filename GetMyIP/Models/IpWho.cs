// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class used to deserialize the JSON returned from https://ipwho.org/me
/// </summary>
/// <remarks>Ensure that PropertyNameCaseInsensitive is set to true in deserialize method.</remarks>
internal sealed class IpWho
{
    /// <summary>
    /// Success flag
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Data object
    /// </summary>
    public Data? Data { get; init; } = new();
}

/// <summary>
/// Data object
/// </summary>
internal sealed class Data
{
    /// <summary>
    /// IP address
    /// </summary>
    public string Ip { get; init; } = string.Empty;
    public string IpAddress => Ip;

    /// <summary>
    /// Continent
    /// </summary>
    public string Continent { get; init; } = string.Empty;

    /// <summary>
    /// Continent Code
    /// </summary>
    public string ContinentCode { get; init; } = string.Empty;

    /// <summary>
    /// Country Name
    /// </summary>
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Country Code (2 characters)
    /// </summary>
    public string CountryCode { get; init; } = string.Empty;

    /// <summary>
    /// Region or State
    /// </summary>
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Region Code (2 characters)
    /// </summary>
    public string RegionCode { get; init; } = string.Empty;

    /// <summary>
    /// City Name
    /// </summary>
    public string City { get; init; } = string.Empty;

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
    /// Timezone info
    /// </summary>
    public Timezone Timezone { get; init; } = new();

    /// <summary>
    /// Connection info (ASN)
    /// </summary>
    public Connection Connection { get; init; } = new();
}

/// <summary>
/// Timezone object
/// </summary>
internal sealed class Timezone
{
    /// <summary>
    /// Timezone name
    /// </summary>
    public string Time_Zone { get; init; } = string.Empty;

    /// <summary>
    /// Timezone abbreviation
    /// </summary>
    public string Abbr { get; init; } = string.Empty;

    /// <summary>
    /// Offset from UTC as a string
    /// </summary>
    public string Utc { get; init; } = string.Empty;
}

/// <summary>
/// Connection object (ASN info)
/// </summary>
internal sealed class Connection
{
    /// <summary>
    /// Autonomous System Number
    /// </summary>
    public int Number { get; init; }

    /// <summary>
    /// Autonomous System Organization
    /// </summary>
    public string Org { get; init; } = string.Empty;
}
