// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class to deserialize the JSON returned from https://freeipapi.com
/// </summary>
internal class FreeIpApi
{
    public int IpVersion { get; set; }

    public string IpAddress { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string CountryName { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string CityName { get; set; } = string.Empty;

    public string RegionName { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public string PostalCode => ZipCode;

    public string TimeZone {  get; set; } = string.Empty;

    public string Continent { get; set; } = string.Empty;
}
