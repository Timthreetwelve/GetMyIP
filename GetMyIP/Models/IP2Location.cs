// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class used to deserialize the JSON returned from https://api.ip2location.io
/// </summary>
/// <remarks>Ensure that PropertyNameCaseInsensitive is set to true in deserialize method.</remarks>
internal class IP2Location
{
    public string Ip { get; init; } = string.Empty;
    public string IpAddress => Ip!;

    public string Country_Code { get; init; } = string.Empty;

    public string Country_Name { get; init; } = string.Empty;

    public string Region_Name { get; init; } = string.Empty;

    public string City_Name { get; init; } = string.Empty;

    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public string Zip_Code { get; init; } = string.Empty;

    public string Time_Zone { get; init; } = string.Empty;

    public string ASN { get; init; } = string.Empty;

    public string AS { get; init; } = string.Empty;

    public bool Is_Proxy { get; init; }

    public string Message { get; init; } = string.Empty;
}
