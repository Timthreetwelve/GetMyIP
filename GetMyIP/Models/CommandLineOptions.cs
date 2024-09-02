// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

internal sealed class CommandLineOptions
{
    [Option('h', "hide", Required = false)]
    public bool Hide { get; set; }

    [Option('r', "restart", Required = false)]
    public bool Restart { get; set; }
}
