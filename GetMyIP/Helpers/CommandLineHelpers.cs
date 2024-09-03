// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Helpers;

static class CommandLineHelpers
{
    #region Process the command line
    /// <summary>
    /// Parse any command line options
    /// </summary>
    /// <returns>False if "hide" was found, True otherwise.</returns>
    public static CommandLineArgs ProcessCommandLine()
    {
        // Since this is not a console app, get the command line args
        string[] args = Environment.GetCommandLineArgs();

        // Parser settings
        Parser parser = new(s =>
        {
            s.CaseSensitive = false;
            s.IgnoreUnknownArguments = true;
        });

        // parses the command line. result object will hold the arguments
        ParserResult<CommandLineOptions> result = parser.ParseArguments<CommandLineOptions>(args);

        // Check options
        if (result?.Value.Hide == true)
        {
            return CommandLineArgs.Hide;
        }
        else if (result?.Value.Restart == true)
        {
            return CommandLineArgs.Restart;
        }
        return CommandLineArgs.None;
    }
    #endregion Process the command line

    #region Enum for command line args
    public enum CommandLineArgs
    {
        None = 0,
        Hide = 1,
        Restart = 2,
    }
    #endregion Enum for command line args
}
