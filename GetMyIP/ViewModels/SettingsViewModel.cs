// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    #region Remove Exit page from list of initial pages
    public static List<NavPage> NavPages { get; } = new();

    /// <summary>
    /// Gets the list of nav pages and removes the exit page.
    /// </summary>
    public static void ParseInitialPage()
    {
        foreach (NavPage page in Enum.GetValues<NavPage>())
        {
            if (!page.Equals(NavPage.Exit))
            {
                NavPages.Add(page);
            }
        }
    }
    #endregion Remove Exit page from list of initial pages

    #region Relay Commands
    [RelayCommand]
    private static void ViewPermLog()
    {
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }

    [RelayCommand]
    private static void TestLogging()
    {
        IpHelpers.LogIPInfo();
        Task.Delay(200);
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }
    #endregion Relay Commands
}
