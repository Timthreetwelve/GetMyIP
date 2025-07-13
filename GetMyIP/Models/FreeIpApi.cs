// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class to deserialize the JSON returned from https://freeipapi.com
/// </summary>
internal sealed class FreeIpApi
{
    /// <summary>
    /// IP Version (IPv4 or IPv6) as int
    /// </summary>
    public int IpVersion { get; init; }

    /// <summary>
    /// IP address
    /// </summary>
    public string IpAddress { get; init; } = string.Empty;

    /// <summary>
    /// Latitude (decimal)
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Longitude (decimal)
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Country Name
    /// </summary>
    public string CountryName { get; init; } = string.Empty;

    /// <summary>
    /// Country Code (2 characters)
    /// </summary>
    public string CountryCode { get; init; } = string.Empty;

    /// <summary>
    /// City Name
    /// </summary>
    public string CityName { get; init; } = string.Empty;

    /// <summary>
    /// Region or State
    /// </summary>
    public string RegionName { get; init; } = string.Empty;

    /// <summary>
    /// Continent
    /// </summary>
    public string Continent { get; init; } = string.Empty;

    /// <summary>
    /// Autonomous System Number as string
    /// </summary>
    public string ASN { get; init; } = string.Empty;

    /// <summary>
    /// Organization Name
    /// </summary>
    public string AsnOrganization { get; init; } = string.Empty;
}
