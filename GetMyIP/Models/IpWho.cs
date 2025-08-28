// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class used to deserialize the JSON returned from https://ipwho.org/me
/// </summary>
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
/// <remarks>This class uses the JsonPropertyName attribute to align the JSON names to the class names.</remarks>
internal sealed class Data
{
    /// <summary>
    /// IP address
    /// </summary>
    [JsonPropertyName("ip")]
    public string IpAddress { get; init; } = string.Empty;

    /// <summary>
    /// Continent
    /// </summary>
    [JsonPropertyName("continent")]
    public string Continent { get; init; } = string.Empty;

    /// <summary>
    /// Continent Code
    /// </summary>
    [JsonPropertyName("continentCode")]
    public string ContinentCode { get; init; } = string.Empty;

    /// <summary>
    /// Country Name
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; init; } = string.Empty;

    /// <summary>
    /// Country Code (2 characters)
    /// </summary>
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; init; } = string.Empty;

    /// <summary>
    /// Region or State
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Region Code (2 characters)
    /// </summary>
    [JsonPropertyName("regionCode")]
    public string RegionCode { get; init; } = string.Empty;

    /// <summary>
    /// City Name
    /// </summary>
    [JsonPropertyName("city")]
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Postal or Zip code
    /// </summary>
    [JsonPropertyName("postal_Code")]
    public string PostalCode { get; init; } = string.Empty;

    /// <summary>
    /// Latitude (decimal)
    /// </summary>
    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude (decimal)
    /// </summary>
    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }

    /// <summary>
    /// Timezone info
    /// </summary>
    [JsonPropertyName("timezone")]
    public Timezone Timezone { get; init; } = new();

    /// <summary>
    /// Connection info (ASN)
    /// </summary>
    [JsonPropertyName("connection")]
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
    [JsonPropertyName("time_zone")]
    public string TimeZoneName { get; init; } = string.Empty;

    /// <summary>
    /// Timezone abbreviation
    /// </summary>
    [JsonPropertyName("abbr")]
    public string Abbr { get; init; } = string.Empty;

    /// <summary>
    /// Offset from UTC as a string
    /// </summary>
    [JsonPropertyName("utc")]
    public string UtcOffset { get; init; } = string.Empty;
}

/// <summary>
/// Connection object (ASN info)
/// </summary>
internal sealed class Connection
{
    /// <summary>
    /// Autonomous System Number
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; init; }

    /// <summary>
    /// Autonomous System Organization
    /// </summary>
    [JsonPropertyName("org")]
    public string Org { get; init; } = string.Empty;
}
