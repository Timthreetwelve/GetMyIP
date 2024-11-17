// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Configuration;

/// <summary>
/// Class for non-persistent settings.
/// </summary>
[INotifyPropertyChanged]
internal sealed partial class TempSettings : ConfigManager<TempSettings>
{
    [ObservableProperty]
    private static bool _appExpanderOpen;

    [ObservableProperty]
    private static bool _uIExpanderOpen;

    [ObservableProperty]
    private static bool _iconExpanderOpen;

    [ObservableProperty]
    private static bool _langExpanderOpen;

    [ObservableProperty]
    private static bool _logExpanderOpen;

    [ObservableProperty]
    private static bool _refreshExpanderOpen;

    [ObservableProperty]
    private static bool _providerExpanderOpen;

    [ObservableProperty]
    private static bool _translateExpanderOpen;

    [ObservableProperty]
    private static bool _backupExpanderOpen;
}
