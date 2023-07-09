// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class to deserialize the JSON returned from http://ip-api.com/json
/// </summary>
public class IPGeoLocation
{
    /// <summary>
    /// Status: success or fail
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Message: only when status is fail
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Country: Country name
    /// </summary>
    public string Country { get; set; } = "*** missing ***";

    /// <summary>
    /// RegionName: Region/State
    /// </summary>
    public string RegionName { get; set; } = "*** missing ***";

    /// <summary>
    /// State: Mapped to RegionName
    /// </summary>
    public string State => RegionName;

    /// <summary>
    /// City: City
    /// </summary>
    public string City { get; set; } = "*** missing ***";

    /// <summary>
    /// Zip: Zip code
    /// </summary>
    public string Zip { get; set; } = "*** missing ***";

    /// <summary>
    /// Lat: Latitude
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Lon: Longitude
    /// </summary>
    public double Lon { get; set; }

    /// <summary>
    /// TimeZone: City TimeZone
    /// </summary>
    public string TimeZone { get; set; } = "*** missing ***";

    /// <summary>
    /// Isp: Internet service provider name
    /// </summary>
    public string Isp { get; set; } = "*** missing ***";

    /// <summary>
    /// Query: IP address used for the query
    /// </summary>
    public string Query { get; set; } = "*** missing ***";

    /// <summary>
    /// IPAddress: Mapped to Query
    /// </summary>
    public string IpAddress => Query;

    /// <summary>
    /// Continent (not abbreviated)
    /// </summary>
    public string Continent { get; set; } = "*** missing ***";

    /// <summary>
    /// Offset from UTC in seconds
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Autonomous System (AS) Number
    /// </summary>
    public string AS { get; set; } = "*** missing ***";

    /// <summary>
    /// Autonomous System (AS) Name
    /// </summary>
    public string ASName { get; set; } = "*** missing ***";
}
