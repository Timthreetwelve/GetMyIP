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
}
