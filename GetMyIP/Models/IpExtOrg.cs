// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;
internal class IpExtOrg
{
    public string Ip { get; set; }

    public string IpAddress => Ip;

    public string Type { get; set; }

    public string IpType => Type;
}
