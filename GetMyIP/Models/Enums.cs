// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Navigation Page
/// </summary>
public enum NavPage
{
    Internal = 0,
    External = 1,
    // separator
    Settings = 3,
    About = 4,
    // separator
    Exit = 6
}

/// <summary>
/// Theme type, Light, Dark, or System
/// </summary>
public enum ThemeType
{
    Light = 0,
    [Description("Material Dark")]
    Dark = 1,
    Darker = 2,
    System = 3
}

/// <summary>
/// Size of the UI, Smallest, Smaller, Default, Larger, or Largest
/// </summary>
public enum MySize
{
    Smallest = 0,
    Smaller = 1,
    Small = 2,
    Default = 3,
    Large = 4,
    Larger = 5,
    Largest = 6
}

/// <summary>
/// One of the 21 predefined Material Design in XAML colors
/// </summary>
public enum AccentColor
{
    Red = 0,
    Pink = 1,
    Purple = 2,
    [Description("Deep Purple")]
    DeepPurple = 3,
    Indigo = 4,
    Blue = 5,
    [Description("Light Blue")]
    LightBlue = 6,
    Cyan = 7,
    Teal = 8,
    Green = 9,
    [Description("Light Green")]
    LightGreen = 10,
    Lime = 11,
    Yellow = 12,
    Amber = 13,
    Orange = 14,
    [Description("Deep Orange")]
    DeepOrange = 15,
    Brown = 16,
    Grey = 17,
    [Description("Blue Gray")]
    BlueGray = 18,
    Black = 19,
    White = 20,
}

/// <summary>
/// Spacing of rows in the DataGrids
/// </summary>
public enum Spacing
{
    Compact = 0,
    Comfortable = 1,
    Spacious = 2
}

/// <summary>
/// The website to navigate to when showing a map in the browser
/// </summary>
public enum MapProvider
{
    Google = 0,
    Bing = 1,
    LatLong = 2
}
