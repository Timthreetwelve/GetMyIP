// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal sealed record CommandLineOptions(bool Hide, string? ErrorMessage);

internal static class CommandLineOptionsParser
{
    internal static CommandLineOptions Parse(IEnumerable<string> arguments)
    {
        bool hide = false;

        foreach (string? rawArgument in arguments)
        {
            if (string.IsNullOrWhiteSpace(rawArgument))
            {
                continue;
            }

            string argument = rawArgument.Trim();

            if (IsHideArgument(argument))
            {
                hide = true;
                continue;
            }

            if (IsSwitchArgument(argument))
            {
                return new CommandLineOptions(hide, $"Unknown argument: {rawArgument}");
            }
        }

        return new CommandLineOptions(hide, null);
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
