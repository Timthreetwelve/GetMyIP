﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Configuration;

/// <summary>
/// A class containing the properties used to persist user settings.
/// </summary>
[INotifyPropertyChanged]
public partial class UserSettings : ConfigManager<UserSettings>
{
    #region Properties
    /// <summary>
    /// <see langword="bool"/> <see langword="property"/> used to set automatic refresh.
    /// </summary>
    [ObservableProperty]
    private bool _autoRefresh;

    /// <summary>
    /// Used to set the refresh interval from one of the values in the RefreshInterval enum.
    /// </summary>
    [ObservableProperty]
    private RefreshIntervals _autoRefreshInterval = RefreshIntervals.Minutes30;

    /// <summary>
    ///  Used to determine if Debug level messages are included in the application log.
    /// </summary>
    [ObservableProperty]
    private bool _includeDebug = true;

    /// <summary>
    /// Used to determine if IPv6 addresses are to be displayed.
    /// </summary>
    [ObservableProperty]
    private bool _includeV6 = true;

    /// <summary>
    /// The External IP information provider.
    /// </summary>
    [ObservableProperty]
    private PublicInfoProvider _infoProvider = PublicInfoProvider.IpApiCom;

    /// <summary>
    /// The page displayed on startup.
    /// </summary>
    [ObservableProperty]
    private NavPage _initialPage = NavPage.External;

    /// <summary>
    /// Keep window topmost.
    /// </summary>
    [ObservableProperty]
    private bool _keepOnTop;

    /// <summary>
    /// Keep window on screen.
    /// </summary>
    [ObservableProperty]
    private bool _keepWindowOnScreen = true;

    /// <summary>
    /// Enable language testing.
    /// </summary>
    [ObservableProperty]
    private bool _languageTesting;

    /// <summary>
    /// File name and path of the permanent log file.
    /// </summary>
    [ObservableProperty]
    private string _logFile = string.Empty;

    /// <summary>
    /// Map provider.
    /// </summary>
    [ObservableProperty]
    private int _mapProvider = 1;

    /// <summary>
    /// Put icon in system tray when minimized.
    /// </summary>
    [ObservableProperty]
    private bool _minimizeToTray;

    /// <summary>
    /// Minimize to tray instead of closing.
    /// </summary>
    [ObservableProperty]
    private bool _minimizeToTrayOnClose;

    /// <summary>
    /// Use toast notification when external IP address has changed.
    /// </summary>
    [ObservableProperty]
    private bool _notifyOnIpChange;

    /// <summary>
    /// Obfuscate sensitive information in the log file.
    /// </summary>
    [ObservableProperty]
    private bool _obfuscateLog;

    /// <summary>
    /// Accent color.
    /// </summary>
    [ObservableProperty]
    private AccentColor _primaryColor = AccentColor.Blue;

    /// <summary>
    /// Refresh IP information after restoring minimized window.
    /// </summary>
    [ObservableProperty]
    private bool _refreshAfterRestore;

    /// <summary>
    /// Restore window to center of screen.
    /// </summary>
    [ObservableProperty]
    private bool _restoreToCenter;

    /// <summary>
    /// Restore window to initial page.
    /// </summary>
    [ObservableProperty]
    private bool _restoreToInitialPage = true;

    /// <summary>
    /// Maximum number of retry attempts.
    /// </summary>
    [ObservableProperty]
    private int _retryMax = 4;

    /// <summary>
    /// Number of seconds to wait between retry attempts.
    /// </summary>
    [ObservableProperty]
    private int _retrySeconds = 15;

    /// <summary>
    /// Vertical spacing in the data grids.
    /// </summary>
    [ObservableProperty]
    private Spacing _rowSpacing = Spacing.Comfortable;

    /// <summary>
    /// Font used in datagrids.
    /// </summary>
    [ObservableProperty]
    private string? _selectedFont = "Segoe UI";

    /// <summary>
    /// Font size used throughout the application.
    /// Defaults to 14 which was the original size.
    /// </summary>
    [ObservableProperty]
    private double _selectedFontSize = 14;

    /// <summary>
    /// Show Exit in the navigation menu.
    /// </summary>
    [ObservableProperty]
    private bool _showExitInNav = true;

    /// <summary>
    /// Option to show custom header in tray icon tooltip.
    /// </summary>
    [ObservableProperty]
    private bool _showHeader;

    /// <summary>
    /// Option to show ASName in results.
    /// </summary>
    [ObservableProperty]
    private bool _showASName;

    /// <summary>
    /// Option to show ASNumber in results.
    /// </summary>
    [ObservableProperty]
    private bool _showASNumber;

    /// <summary>
    /// Option to show City in results.
    /// </summary>
    [ObservableProperty]
    private bool _showCity;

    /// <summary>
    /// Option to show Country in results.
    /// </summary>
    [ObservableProperty]
    private bool _showCountry = true;

    /// <summary>
    /// Option to show external IP address in results.
    /// </summary>
    [ObservableProperty]
    private bool _showExternalIP = true;

    /// <summary>
    /// Option to show flag as tray icon.
    /// </summary>
    [ObservableProperty]
    private bool _showFlagIcon;

    /// <summary>
    /// Option to show internal IPv4 address in results.
    /// </summary>
    [ObservableProperty]
    private bool _showInternalIPv4 = true;

    /// <summary>
    /// Option to show internal IPv6 address in results.
    /// </summary>
    [ObservableProperty]
    private bool _showInternalIPv6;

    /// <summary>
    /// Option to show ISP in results.
    /// </summary>
    [ObservableProperty]
    private bool _showISP;

    /// <summary>
    /// Option to show IP version in results.
    /// </summary>
    [ObservableProperty]
    private bool _showIpVersion;

    /// <summary>
    /// Option to show offset from UTC in results.
    /// </summary>
    [ObservableProperty]
    private bool _showOffset;

    /// <summary>
    /// Option to show time of last refresh.
    /// </summary>
    [ObservableProperty]
    private bool _showLastRefresh = true;

    /// <summary>
    /// Option to show state in results.
    /// </summary>
    [ObservableProperty]
    private bool _showState;

    /// <summary>
    /// Option to show time zone in results.
    /// </summary>
    [ObservableProperty]
    private bool _showTimeZone;

    /// <summary>
    /// Option start with window centered on screen.
    /// </summary>
    [ObservableProperty]
    private bool _startCentered = true;

    /// <summary>
    /// Option to start minimized.
    /// </summary>
    [ObservableProperty]
    private bool _startMinimized;

    /// <summary>
    /// Option to start when Windows starts.
    /// </summary>
    [ObservableProperty]
    private bool _startWithWindows;

    /// <summary>
    /// Text for custom tooltip.
    /// </summary>
    [ObservableProperty]
    private string _tooltipHeading = string.Empty;

    /// <summary>
    /// Defined language to use in the UI.
    /// </summary>
    [ObservableProperty]
    private string _uILanguage = "en-US";

    /// <summary>
    /// Amount of zoom.
    /// </summary>
    [ObservableProperty]
    private MySize _uISize = MySize.Default;

    /// <summary>
    /// Theme type.
    /// </summary>
    [ObservableProperty]
    private ThemeType _uITheme = ThemeType.System;

    /// <summary>
    /// Use accent color for snack bar message background.
    /// </summary>
    [ObservableProperty]
    private bool _useAccentColorOnSnackbar;

    /// <summary>
    /// Use the operating system language (if one has been provided).
    /// </summary>
    [ObservableProperty]
    private bool _useOSLanguage = true;

    /// <summary>
    /// Height of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowHeight = 575;

    /// <summary>
    /// Position of left side of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowLeft = 200;

    /// <summary>
    /// Width of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowWidth = 700;

    /// <summary>
    /// Position of the top side of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowTop = 100;
    #endregion Properties
}
