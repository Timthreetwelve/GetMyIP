// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

public partial class RefreshInfo : ObservableObject
{
    [ObservableProperty]
    private string _lastRefresh;

    [ObservableProperty]
    private string _lastIPAddress = string.Empty;

    public static RefreshInfo Instance { get; private set; }

    static RefreshInfo()
    {
        Instance = new();
    }
}
