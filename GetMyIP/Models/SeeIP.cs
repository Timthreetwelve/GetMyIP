// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;
internal sealed class SeeIP
{
    public string Ip { get; init; } = string.Empty;

    public string IpAddress => Ip;
}
