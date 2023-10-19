// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    #region Relay Commands
    [RelayCommand]
    private static void ViewPermLog()
    {
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }

    [RelayCommand]
    private static async Task TestLogging()
    {
        string json = await IpHelpers.GetExtInfo();
        IpHelpers.LogIPInfo(json);
        _ = Task.Delay(200);
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }
    #endregion Relay Commands
}
