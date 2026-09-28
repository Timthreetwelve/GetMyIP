// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

/// <summary>
/// ViewModel for viewing network adapters.
/// </summary>
internal sealed partial class AdaptersViewModel : ObservableObject
{
    #region Properties
    [ObservableProperty]
    private Adapters _adapters = new();

    [ObservableProperty]
    private ObservableCollection<Adapters> _adaptersList = [];

    [ObservableProperty]
    private Adapters? _selectedAdapter;
    #endregion Properties

    #region Static Instance
    private static AdaptersViewModel Instance { get; set; } = null!;
    #endregion Static Instance

    #region Constructor
    public AdaptersViewModel()
    {
        _log.Debug("...AdaptersViewModel constructor called.");
        Instance = this;
        if (AdaptersList.Count == 0)
        {
            _log.Debug("AdaptersList is empty. Updating AdaptersList.");
            AdaptersList = UpdateAdaptersList();
        }
    }
    #endregion Constructor

    #region Methods
    /// <summary>
    /// Updates the list of network adapters.
    /// </summary>
    /// <returns>The updated ObservableCollection of Adapters.</returns>
    public static ObservableCollection<Adapters> UpdateAdaptersList()
    {
        if (Instance is null)
        {
            _log.Error("AdaptersViewModel instance is null. Cannot update adapters list.");
            return [];
        }

        Adapters? previousSelection = Instance.SelectedAdapter;

        Instance.AdaptersList.Clear();
        foreach (Adapters adapter in AdaptersHelpers.GetAdaptersList())
        {
            Instance.AdaptersList.Add(adapter);
        }

        if (previousSelection is not null)
        {
            Instance.SelectedAdapter = Instance.AdaptersList.FirstOrDefault(x =>
                x.PhysicalAddress == previousSelection.PhysicalAddress);
        }

        _log.Debug($"Adapters list updated. Count: {Instance.AdaptersList.Count}");
        return Instance.AdaptersList;
    }
    #endregion Methods
}
