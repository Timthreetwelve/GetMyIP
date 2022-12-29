// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

/// <summary>
/// Class to deserialize the JSON returned from http://ip-api.com/json
/// </summary>
public class IPGeoLocation
{
    // Status: success or fail
    public string Status { get; set; }

    // Message: only when status is fail
    public string Message { get; set; }

    // Country: Country name
    public string Country { get; set; } = "*** missing ***";

    // RegionName: Region/State
    public string RegionName { get; set; } = "*** missing ***";

    // State: Mapped to RegionName
    public string State => RegionName;

    // City: City
    public string City { get; set; } = "*** missing ***";

    // Zip: Zip code
    public string Zip { get; set; } = "*** missing ***";

    // Lat: Latitude
    public double Lat { get; set; }

    // Lon: Longitude
    public double Lon { get; set; }

    // Timezone: City timezone
    public string Timezone { get; set; } = "*** missing ***";

    // Isp: Internet service provider name
    public string Isp { get; set; } = "*** missing ***";

    // Query: IP address used for the query
    public string Query { get; set; } = "*** missing ***";

    // IPAddress: Mapped to Query
    public string IpAddress => Query;

    // Continent (not abbreviated)
    public string Continent { get; set; } = "*** missing ***";

    // Offset from UTC in seconds
    public int Offset { get; set; }
}
