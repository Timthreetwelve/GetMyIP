// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

public static class CustomCommands
{
    /// <summary>
    /// Gets or sets the test logging.
    /// </summary>
    /// <value>
    /// The test logging.
    /// </value>
    public static RoutedUICommand TestLogging { get; set; } = new RoutedUICommand();

    /// <summary>
    /// Gets or sets the view log.
    /// </summary>
    /// <value>
    /// The view log.
    /// </value>
    public static RoutedUICommand ViewLog { get; set; } = new RoutedUICommand();

    /// <summary>
    /// Gets the exit.
    /// </summary>
    /// <value>
    /// The exit.
    /// </value>
    public static RoutedUICommand Exit { get; } =
    new("E_xit", "Exit", typeof(CustomCommands));

    /// <summary>
    /// Gets the show main window.
    /// </summary>
    /// <value>
    /// The show main window.
    /// </value>
    public static RoutedUICommand ShowMainWindow { get; } =
        new("_Show Main Window", "Show", typeof(CustomCommands));

    /// <summary>
    /// Gets the refresh information.
    /// </summary>
    /// <value>
    /// The refresh information.
    /// </value>
    public static RoutedUICommand RefreshInfo { get; } =
        new RoutedUICommand("_Refresh IP Info", "Refresh", typeof(CustomCommands));
}
