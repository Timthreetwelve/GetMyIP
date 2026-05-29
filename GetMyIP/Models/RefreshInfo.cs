// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

public partial class RefreshInfo : ObservableObject
{
    #region Properties
#pragma warning disable MVVMTK0042 // Prefer using [ObservableProperty] on partial properties
    // Suppressing the MVVMTK0042 warning for this class until such time as it no longer requires Preview features.
    [ObservableProperty]
    private string? _lastRefresh;

    [ObservableProperty]
    private string? _lastIPAddress = string.Empty;
#pragma warning restore MVVMTK0042 // Prefer using [ObservableProperty] on partial properties
    #endregion Properties

    #region Static instance
    public static RefreshInfo Instance { get; private set; }
    #endregion Static instance

    #region Constructor
    static RefreshInfo()
    {
        Instance = new();
    }
    #endregion
}
