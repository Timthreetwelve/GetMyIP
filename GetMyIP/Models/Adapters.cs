// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

public sealed partial class Adapters : ObservableObject
{
    #region Properties
    [ObservableProperty]
    private string _adapterType = string.Empty;

    [ObservableProperty]
    private string _defaultGateway = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _ipAddress = string.Empty;

    [ObservableProperty]
    private string _ipV6Address = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _operationalStatus = string.Empty;

    [ObservableProperty]
    private string _physicalAddress = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    [ObservableProperty]
    private string _subnetMask = string.Empty;
    #endregion Properties
}
