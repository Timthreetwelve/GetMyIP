// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class CommandLineHelpers
{
    #region Process the command line
    /// <summary>
    /// Parse any command line options
    /// </summary>
    /// <returns>CommandLineArgs.Hide if "hide" was found.</returns>
    public static CommandLineArgs ProcessCommandLine()
    {
        CommandLineParser.CommandLineParser parser = new();
        SwitchArgument hideArgument = new('h',
                                                      "hide",
                                                      "To hide or not to hide, that is the question.",
                                                      false);
        parser.Arguments.Add(hideArgument);
        parser.AcceptSlash = true;
        parser.IgnoreCase = true;

        parser.ParseCommandLine(App.Args);

        return hideArgument.Value ? CommandLineArgs.Hide : CommandLineArgs.None;
    }
    #endregion Process the command line

    #region Enum for command line args
    public enum CommandLineArgs
    {
        None = 0,
        Hide = 1,
    }
    #endregion Enum for command line args
}
