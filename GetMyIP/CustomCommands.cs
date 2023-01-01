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
}
