// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP;

/// <summary>
/// Navigation Page
/// </summary>
public enum NavPage
{
    Logs = 0,
    // separator
    Start = 2,
    Stop = 3,
    // separator
    Settings = 5,
    About = 6,
    Exit = 7
}

/// <summary>
/// Theme type, Light, Dark, or System
/// </summary>
internal enum ThemeType
{
    Light = 0,
    Dark = 1,
    Darker = 2,
    System = 3
}

/// <summary>
/// Size of the UI, Smallest, Smaller, Default, Larger, or Largest
/// </summary>
internal enum MySize
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
/// One of the 19 predefined Material Design in XAML colors
/// </summary>
internal enum AccentColor
{
    Red = 0,
    Pink = 1,
    Purple = 2,
    DeepPurple = 3,
    Indigo = 4,
    Blue = 5,
    LightBlue = 6,
    Cyan = 7,
    Teal = 8,
    Green = 9,
    LightGreen = 10,
    Lime = 11,
    Yellow = 12,
    Amber = 13,
    Orange = 14,
    DeepOrange = 15,
    Brown = 16,
    Grey = 17,
    BlueGray = 18
}

public enum Spacing
{
    Compact = 0,
    Comfortable = 1,
    Wide = 2
}