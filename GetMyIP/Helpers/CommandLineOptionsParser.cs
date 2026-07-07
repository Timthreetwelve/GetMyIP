// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal sealed record CommandLineOptions(bool Hide, string? ErrorMessage);

internal static class CommandLineOptionsParser
{
    internal static CommandLineOptions Parse(IEnumerable<string> arguments)
    {
        foreach (string? rawArgument in arguments)
        {
            if (string.IsNullOrWhiteSpace(rawArgument))
            {
                continue;
            }

            string argument = rawArgument.Trim();

            if (IsHideArgument(argument))
            {
                return new CommandLineOptions(true, null);
            }

            if (IsSwitchArgument(argument))
            {
                return new CommandLineOptions(false, $"Unknown argument: {rawArgument}");
            }
        }

        return new CommandLineOptions(false, null);
    }

    private static bool IsHideArgument(string argument)
    {
        string normalized = argument.TrimStart('-', '/');

        return normalized.Equals("h", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("hide", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSwitchArgument(string argument)
    {
        return argument.StartsWith('-') ||
               argument.StartsWith('/');
    }
}
