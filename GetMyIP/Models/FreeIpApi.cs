// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class to deserialize the JSON returned from https://freeipapi.com
/// </summary>
internal class FreeIpApi
{
    public int IpVersion { get; init; }

    public string IpAddress { get; init; } = string.Empty;

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public string CountryName { get; init; } = string.Empty;

    public string CountryCode { get; init; } = string.Empty;

    public string CityName { get; init; } = string.Empty;

    public string RegionName { get; init; } = string.Empty;

    public string ZipCode { get; init; } = string.Empty;

    public string PostalCode => ZipCode;

    public string TimeZone {  get; init; } = string.Empty;

    public string Continent { get; init; } = string.Empty;
}
