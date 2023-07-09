// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

class CommandLineOptions
{
    [Option('h', "hide", Required = false)]
    public bool Hide { get; set; }
}
