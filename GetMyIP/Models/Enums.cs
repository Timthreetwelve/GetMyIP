// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

#region Navigation
/// <summary>
/// Navigation Page enumeration used by the Initial page ComboBox in Settings
/// </summary>
/// <remarks>
/// THe "Exit" nav page is not listed here
/// </remarks>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum NavPage
{
    [LocalizedDescription("SettingsEnum_Navigation_Internal")]
    Internal,
    [LocalizedDescription("SettingsEnum_Navigation_External")]
    External,
    [LocalizedDescription("SettingsEnum_Navigation_Settings")]
    Settings,
    [LocalizedDescription("SettingsEnum_Navigation_About")]
    About
}
#endregion Navigation

#region Theme
/// <summary>
/// Theme type
/// </summary>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum ThemeType
{
    [LocalizedDescription("SettingsEnum_Theme_Light")]
    Light = 0,
    [LocalizedDescription("SettingsEnum_Theme_Dark")]
    Dark = 1,
    [LocalizedDescription("SettingsEnum_Theme_Darker")]
    Darker = 2,
    [LocalizedDescription("SettingsEnum_Theme_System")]
    System = 3
}
#endregion Theme

#region UI size
/// <summary>
/// Size of the UI
/// </summary>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum MySize
{
    [LocalizedDescription("SettingsEnum_Size_Smallest")]
    Smallest = 0,
    [LocalizedDescription("SettingsEnum_Size_Smaller")]
    Smaller = 1,
    [LocalizedDescription("SettingsEnum_Size_Small")]
    Small = 2,
    [LocalizedDescription("SettingsEnum_Size_Default")]
    Default = 3,
    [LocalizedDescription("SettingsEnum_Size_Large")]
    Large = 4,
    [LocalizedDescription("SettingsEnum_Size_Larger")]
    Larger = 5,
    [LocalizedDescription("SettingsEnum_Size_Largest")]
    Largest = 6
}
#endregion UI size

#region Accent color
/// <summary>
/// One of the 19 predefined Material Design in XAML colors plus Black & White
/// </summary>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum AccentColor
{
    [LocalizedDescription("SettingsEnum_AccentColor_Red")]
    Red = 0,
    [LocalizedDescription("SettingsEnum_AccentColor_Pink")]
    Pink = 1,
    [LocalizedDescription("SettingsEnum_AccentColor_Purple")]
    Purple = 2,
    [LocalizedDescription("SettingsEnum_AccentColor_DeepPurple")]
    Deep_Purple = 3,
    [LocalizedDescription("SettingsEnum_AccentColor_Indigo")]
    Indigo = 4,
    [LocalizedDescription("SettingsEnum_AccentColor_Blue")]
    Blue = 5,
    [LocalizedDescription("SettingsEnum_AccentColor_LightBlue")]
    Light_Blue = 6,
    [LocalizedDescription("SettingsEnum_AccentColor_Cyan")]
    Cyan = 7,
    [LocalizedDescription("SettingsEnum_AccentColor_Teal")]
    Teal = 8,
    [LocalizedDescription("SettingsEnum_AccentColor_Green")]
    Green = 9,
    [LocalizedDescription("SettingsEnum_AccentColor_LightGreen")]
    Light_Green = 10,
    [LocalizedDescription("SettingsEnum_AccentColor_Lime")]
    Lime = 11,
    [LocalizedDescription("SettingsEnum_AccentColor_Yellow")]
    Yellow = 12,
    [LocalizedDescription("SettingsEnum_AccentColor_Amber")]
    Amber = 13,
    [LocalizedDescription("SettingsEnum_AccentColor_Orange")]
    Orange = 14,
    [LocalizedDescription("SettingsEnum_AccentColor_DeepOrange")]
    Deep_Orange = 15,
    [LocalizedDescription("SettingsEnum_AccentColor_Brown")]
    Brown = 16,
    [LocalizedDescription("SettingsEnum_AccentColor_Gray")]
    Gray = 17,
    [LocalizedDescription("SettingsEnum_AccentColor_BlueGray")]
    Blue_Gray = 18,
    [LocalizedDescription("SettingsEnum_AccentColor_Black")]
    Black = 19,
    [LocalizedDescription("SettingsEnum_AccentColor_White")]
    White = 20,
}
#endregion Accent color

#region Spacing
/// <summary>
/// Space between rows in the data grids
/// </summary>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum Spacing
{
    [LocalizedDescription("SettingsEnum_Spacing_Compact")]
    Compact = 0,
    [LocalizedDescription("SettingsEnum_Spacing_Comfortable")]
    Comfortable = 1,
    [LocalizedDescription("SettingsEnum_Spacing_Wide")]
    Wide = 2
}
#endregion Spacing

#region Map provider
/// <summary>
/// The website to navigate to when showing a map in the browser
/// </summary>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum MapProvider
{
    [LocalizedDescription("SettingsEnum_MapProvider_Google")]
    Google = 0,
    [LocalizedDescription("SettingsEnum_MapProvider_Bing")]
    Bing = 1,
    [LocalizedDescription("SettingsEnum_MapProvider_LatLong")]
    LatLong = 2
}
#endregion Map provider

#region External IP information source
/// <summary>
/// External IP information provider
/// </summary>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum PublicInfoProvider
{
    [LocalizedDescription("SettingsEnum_Provider_IpApiCom")]
    IpApiCom = 0,
    [LocalizedDescription("SettingsEnum_Provider_IpExtOrg")]
    IpExtOrg = 1,
}
#endregion External IP information source

#region Refresh intervals
/// <summary>
/// Refresh Interval values
/// </summary>
[TypeConverter(typeof(EnumDescriptionTypeConverter))]
public enum RefreshIntervals
{
    [LocalizedDescription("SettingsEnum_RefreshInterval_1Minute")]
    Minutes_1 = 1,
    [LocalizedDescription("SettingsEnum_RefreshInterval_3Minutes")]
    Minutes_3 = 3,
    [LocalizedDescription("SettingsEnum_RefreshInterval_5Minutes")]
    Minutes_5 = 5,
    [LocalizedDescription("SettingsEnum_RefreshInterval_10Minutes")]
    Minutes_10 = 10,
    [LocalizedDescription("SettingsEnum_RefreshInterval_15Minutes")]
    Minutes_15 = 15,
    [LocalizedDescription("SettingsEnum_RefreshInterval_30Minutes")]
    Minutes_30 = 30,
    [LocalizedDescription("SettingsEnum_RefreshInterval_1Hour")]
    Hours_1 = 60,
    [LocalizedDescription("SettingsEnum_RefreshInterval_2Hours")]
    Hours_2 = 120,
    [LocalizedDescription("SettingsEnum_RefreshInterval_3Hours")]
    Hours_3 = 180,
    [LocalizedDescription("SettingsEnum_RefreshInterval_4Hours")]
    Hours_4 = 240,
    [LocalizedDescription("SettingsEnum_RefreshInterval_6Hours")]
    Hours_6 = 360,
    [LocalizedDescription("SettingsEnum_RefreshInterval_8Hours")]
    Hours_8 = 480,
    [LocalizedDescription("SettingsEnum_RefreshInterval_12Hours")]
    Hours_12 = 720,
    [LocalizedDescription("SettingsEnum_RefreshInterval_24Hours")]
    Hours_24 = 1440,
}
#endregion Refresh intervals