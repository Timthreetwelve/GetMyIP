// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public static List<NavPage> NavPages { get; } = new();

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

    [RelayCommand]
    private static void ViewPermLog()
    {
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }

    [RelayCommand]
    private static void TestLogging()
    {
        ExternalInfoViewModel.LogIPInfo();
        Task.Delay(200);
        TextFileViewer.ViewTextFile(UserSettings.Setting.LogFile);
    }
}
