// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Configuration;

/// <summary>
/// A class and methods for reading, updating and saving user settings in a JSON file
/// </summary>
[INotifyPropertyChanged]
public partial class UserSettings : ConfigManager<UserSettings>
{
    #region Properties
    [ObservableProperty]
    private bool _includeDebug = true;

    [ObservableProperty]
    private bool _includeV6 = true;

    [ObservableProperty]
    private PublicInfoProvider _infoProvider = PublicInfoProvider.IpApiCom;

    [ObservableProperty]
    private NavPage _initialPage = NavPage.External;

    [ObservableProperty]
    private bool _keepOnTop;

    [ObservableProperty]
    private string _logFile = string.Empty;

    [ObservableProperty]
    private int _mapProvider = 1;

    [ObservableProperty]
    private bool _minimizeToTray;

    [ObservableProperty]
    private bool _obfuscateLog;

    [ObservableProperty]
    private AccentColor _primaryColor = AccentColor.Blue;

    [ObservableProperty]
    private Spacing _rowSpacing = Spacing.Comfortable;

    [ObservableProperty]
    private bool _showHeader;

    [ObservableProperty]
    private bool _showASName;

    [ObservableProperty]
    private bool _showASNumber;

    [ObservableProperty]
    private bool _showCity;

    [ObservableProperty]
    private bool _showCountry = true;

    [ObservableProperty]
    private bool _showExternalIP = true;

    [ObservableProperty]
    private bool _showInternalIPv4 = true;

    [ObservableProperty]
    private bool _showInternalIPv6;

    [ObservableProperty]
    private bool _showISP;

    [ObservableProperty]
    private bool _showOffset;

    [ObservableProperty]
    private bool _showState;

    [ObservableProperty]
    private bool _showTimeZone;

    [ObservableProperty]
    private bool _startCentered = true;

    [ObservableProperty]
    private string _tooltipHeading = string.Empty;

    [ObservableProperty]
    private string _uILanguage = "en-US";

    [ObservableProperty]
    private MySize _uISize = MySize.Default;

    [ObservableProperty]
    private ThemeType _uITheme = ThemeType.System;

    [ObservableProperty]
    private bool _useOSLanguage;

    [ObservableProperty]
    private double _windowHeight = 575;

    [ObservableProperty]
    private double _windowLeft = 200;

    [ObservableProperty]
    private double _windowWidth = 700;

    [ObservableProperty]
    private double _windowTop = 100;
    #endregion Properties
}
