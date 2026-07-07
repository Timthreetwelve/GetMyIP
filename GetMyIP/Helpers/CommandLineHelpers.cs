// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

internal static class CommandLineHelpers
{
    #region Properties
    /// <summary>
    /// Holds the parser validation message when command-line parsing fails.
    /// </summary>
    public static string? CommandLineParserError { get; private set; }
    #endregion Properties

    #region Process the command line
    /// <summary>
    /// Parse any command line options.
    /// </summary>
    /// <returns>CommandLineArgs.Hide if "hide" was found.</returns>
    public static CommandLineArgs ProcessCommandLine()
    {
        CommandLineParserError = null;

        CommandLineOptions options = CommandLineOptionsParser.Parse(App.Args);
        CommandLineParserError = options.ErrorMessage;

        return options.Hide ? CommandLineArgs.Hide : CommandLineArgs.None;
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
