// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;
internal class SeeIP
{
    public string? Ip { get; set; }

    public string IpAddress => Ip!;
}
