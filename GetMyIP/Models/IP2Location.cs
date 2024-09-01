// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class used to deserialize the JSON returned from https://api.ip2location.io
/// </summary>
/// <remarks>Ensure that PropertyNameCaseInsensitive is set to true in deserialize method.</remarks>
internal class IP2Location
{
    public string Ip { get; set; } = string.Empty;
    public string IpAddress => Ip!;

    public string Country_Code { get; set; } = string.Empty;

    public string Country_Name { get; set; } = string.Empty;

    public string Region_Name { get; set; } = string.Empty;

    public string City_Name { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Zip_Code { get; set; } = string.Empty;

    public string Time_Zone { get; set; } = string.Empty;

    public string ASN { get; set; } = string.Empty;

    public string AS { get; set; } = string.Empty;

    public bool Is_Proxy { get; set; }

    public string Message { get; set; } = string.Empty;
}
